using De.Mud.Telnet;
using Net.Graphite.Telnet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using wtKST.Properties;

namespace wtKST
{
    class KSTcom
    {
        public enum KST_STATE
        {
            Disconnected,
            Disconnecting,
            Standby,
            WaitLogin,
            WaitLogstat,
            WaitSPR,

            WaitTelnetConnect,
            WaitTelnetUserName,
            WaitTelnetPassword,
            WaitTelnetChat,
            WaitTelnetSetName,
            WaitReconnect,

            // must be last
            Connected = 128,
        }

        private KST_STATE KSTState = KST_STATE.Standby;

        public KST_STATE State
        {
            get { return KSTState; }
            set { KSTState = value; }
        }

        private DataTable MSG = new DataTable("MSG");    // holds all the messages exchanged as sent by the KST server
        private DataTable USER = new DataTable("USER");  // holds all the users that are currently logged into the KST server

        private TelnetWrapper tw;

        private DateTime latestMessageTimestamp = DateTime.MinValue;
        private bool latestMessageTimestampSet = false;

        private System.Timers.Timer ti_Receive;
        private System.Timers.Timer ti_Linkcheck;

        private bool SendMyName = false;
        private bool SendMyLocator = false;
        public bool msg_latest_first = false;
        private bool CheckStartUpAway = true;

        public KSTcom()
        {
            MSG = new DataTable("MSG");
            MSG.Columns.Add("TIME", typeof(DateTime));
            MSG.Columns.Add("CALL");
            MSG.Columns.Add("NAME");
            MSG.Columns.Add("MSG");
            MSG.Columns.Add("RECIPIENT");
            DataColumn[] MSGkeys = new DataColumn[]
            {
                MSG.Columns["TIME"],
                MSG.Columns["CALL"],
                MSG.Columns["MSG"]
            };
            MSG.PrimaryKey = MSGkeys;


            USER.Columns.Add("CALL");
            USER.Columns.Add("NAME");
            USER.Columns.Add("LOC");
            USER.Columns.Add("TIME", typeof(DateTime));
            USER.Columns.Add("CONTACTED", typeof(int));
            USER.Columns.Add("RECENTLOGIN", typeof(bool));
            USER.Columns.Add("AWAY", typeof(bool));
            DataColumn[] USERkeys = new DataColumn[]
            {
                USER.Columns["CALL"]
            };
            USER.PrimaryKey = USERkeys;


            ti_Receive = new System.Timers.Timer();
            ti_Linkcheck = new System.Timers.Timer();
            //
            // ti_Receive
            //
            ti_Receive.Enabled = true;
            ti_Receive.Interval = 1000;
            ti_Receive.Elapsed += new System.Timers.ElapsedEventHandler(this.ti_Receive_Tick);
            //
            // ti_Linkcheck
            //
            ti_Linkcheck.Interval = 120000D;
            ti_Linkcheck.Elapsed += new System.Timers.ElapsedEventHandler(this.ti_Linkcheck_Tick);
        }

        public void MSG_clear()
        {
            MSG.Clear();
            latestMessageTimestampSet = false;
        }

        public bool is_reconnect()
        {
            return latestMessageTimestampSet;
        }

        public event EventHandler<newdispTextEventArgs> dispText;
        public class newdispTextEventArgs : EventArgs
        {
            /// <summary>
            /// display String in the UI
            /// </summary>
            public newdispTextEventArgs(String text)
            {
                this.text = text;
            }
            public String text { get; private set; }
        }

        private void Say( String text)
        {
            if (dispText != null)
            { 
                dispText(this, new newdispTextEventArgs(text));
            }
        }

        public DataRow[] MSG_findcall( string call )
        {
            string findCall = string.Format("[CALL] = '{0}' OR [RECIPIENT] = '{0}'", call);
            DataRow[] selectRow = MSG.Select(findCall);
            return selectRow;
        }

        private void KST_Receive(string s)
        {
            switch (KSTState)
            {
                case KST_STATE.WaitLogin:
                    if (s.IndexOf("login") >= 0)
                    {
                        // LOGINC|callsign|password|chat id|client software version|past messages number|past dx/map number|
                        // users list/update flags|last Unix timestamp for messages|last Unix timestamp for dx/map|

                        tw.Send("LOGINC|" + Settings.Default.KST_UserName + "|" + Settings.Default.KST_Password + "|"
                        + Settings.Default.KST_Chat.Substring(0, 1) + "|wtKST " + typeof(MainDlg).Assembly.GetName().Version +
                        "|25|0|1|" +
                        // we try to get the messages up to our latest one
                        (!latestMessageTimestampSet ? "0" :
                        ((latestMessageTimestamp - new DateTime(1970, 1, 1)).TotalSeconds - 1).ToString())
                        + "|0|\r");
                        KSTState = KST_STATE.WaitLogstat;
                        Say("Login " + Settings.Default.KST_UserName + " send.");
                    }
                    break;
                case KST_STATE.WaitLogstat:
                    if (s.IndexOf("LOGSTAT") >= 0)
                    {
                        try
                        {
                            MainDlg.Log.WriteMessage("LOGSTAT: " + s);
                            string call = Settings.Default.KST_UserName.ToUpper();
                            if (WCCheck.WCCheck.IsCall(call) >= 0)
                            {
                                string[] subs = s.Split('|');
                                if (subs[2].Equals("Wrong password!")) // maybe test subs[1] for 114? OK seems 100
                                {
                                    System.Windows.Forms.MessageBox.Show("Password wrong", "Login failed",
                                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                    tw.Close();
                                    MainDlg.Log.WriteMessage("Password wrong ");
                                    break;
                                }
                                // If name not set... take it from ON4KST
                                if (Settings.Default.KST_Name.Length == 0)
                                {
                                    Settings.Default.KST_Name = subs[6];
                                }
                                else
                                {
                                    if (!Settings.Default.KST_Name.Trim().Equals(subs[6]))
                                    {
                                        SendMyName = true;
                                        MainDlg.Log.WriteMessage("KST Name " + Settings.Default.KST_Name + " not equal to name stored on server " + subs[6]);
                                        // we cannot set the name on KST side using port 23001 - no /setname there
                                        // but we can do this through the regular telnet port 23000, so disconnect and do it there
                                        tw.Disconnect();
                                        MainDlg.Log.WriteMessage("disconnect and connect by telnet to set name");
                                        break;
                                    }
                                }

                                if (WCCheck.WCCheck.IsLoc(Settings.Default.KST_Loc) > 0)
                                {
                                    if (!Settings.Default.KST_Loc.Equals(subs[8]))
                                    {
                                        // if KST_Loc option is set, set it on KST side if it does not match
                                        SendMyLocator = true;
                                    }
                                }
                                else
                                {
                                    // store the locator in settings
                                    Settings.Default.KST_Loc = subs[8];
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MainDlg.Log.WriteMessage("Error reading user configuration: " + e.Message);
                        }
                        // To set/reset/PRAU only propagation reception frames
                        // SPR | value |

                        tw.Send("SPR|2|\r");
                        KSTState = KST_STATE.WaitSPR;
                        Say("LOGSTAT " + s);
                    }
                    break;
                case KST_STATE.WaitSPR:
                    /* PRAU|time|Aurora level|
                        Aurora level:
                        2: High lat. AU warning
                        3: High lat. AU alert
                        5 :Mid lat. AU warning
                        6:Mid lat. AU alert
                        8:Low lat. AU warning
                        9:Low lat. AU alert
                        Other values: no alert
                    */
                    if (s.IndexOf("PRAU") >= 0)
                    {
                        try
                        {
                            MainDlg.Log.WriteMessage("Login at " + s.Split('|')[2] + " - " + s);
                        }
                        catch { }
                        // End of the settings frames (session only)
                        // SDONE | chat id |

                        tw.Send("SDONE|" + Settings.Default.KST_Chat.Substring(0, 1) + "|\r");
                        KSTState = KST_STATE.Connected;
                        Say("Connected to KST chat.");
                        MainDlg.Log.WriteMessage("Connected to: " + Settings.Default.KST_Chat);
                        msg_latest_first = true;
                        CheckStartUpAway = true;

                        if (SendMyLocator)
                        {
                            Setloc(Settings.Default.KST_Loc);
                            SendMyLocator = false;
                        }

                        ti_Linkcheck.Stop();   // restart the linkcheck timer
                        ti_Linkcheck.Start();
                    }
                    break;

                // special states called while doing Telnet to set Name
                case KST_STATE.WaitTelnetUserName:
                    if (s.IndexOf("Login:") >= 0)
                    {
                        Thread.Sleep(100);
                        this.tw.Send(Settings.Default.KST_UserName + "\r");
                        this.KSTState = KST_STATE.WaitTelnetPassword;
                        this.Say("Login " + Settings.Default.KST_UserName + " send.");
                    }
                    break;
                case KST_STATE.WaitTelnetPassword:
                    if (s.IndexOf("Password:") >= 0)
                    {
                        Thread.Sleep(100);
                        this.tw.Send(Settings.Default.KST_Password + "\r");
                        this.KSTState = KST_STATE.WaitTelnetChat;
                        this.Say("Password send.");
                    }
                    break;
                case KST_STATE.WaitTelnetChat:
                    if (s.IndexOf("Your choice           :") >= 0)
                    {
                        Thread.Sleep(100);
                        this.tw.Send(Settings.Default.KST_Chat.Substring(0, 1) + "\r");
                    }
                    if (s.IndexOf(">") > 0)
                    {
                        if (SendMyName)
                        {
                            this.tw.Send("/set name " + Settings.Default.KST_Name.Trim() + "\r");
                            MainDlg.Log.WriteMessage("send name " + Settings.Default.KST_Name.Trim() + " to server");
                        }
                        this.KSTState = KST_STATE.WaitTelnetSetName;
                    }
                    break;
                case KST_STATE.WaitTelnetSetName:
                    tw.Disconnect();

                    break;

                // normal KST reception
                case KST_STATE.Connected:
                    if (s.Substring(0, 1).Equals("C"))
                    {
                        Receive_MSG(s);
                    }
                    else if (s.Substring(0, 1).Equals("U"))
                    {
                        Receive_USR(s);
                    }
                    break;
            }
        }


        private void Receive_MSG(string s)
        {
            string[] msg = s.Split('|'); ;
            try
            {
                MainDlg.Log.WriteMessage("KST message: " + s);
                // CR|3|1480257613|F6DKW|Maurice|0|U were here all time on poor tropo anyway|F5BUU| -> chat review
                // CE|3|   -> end
                // CK|     ??? maybe... end of chat review - start of "normal" list?
                // CH|3|1480259640|F6APE|JEAN-NOEL|0|es tu tjrs present pr test 6/3cm b4 qsy garden il fait beau|F2CT|
                switch (s.Substring(0, 2))
                {
                    // Chat message frame at login
                    // CR | chat id | Unix time | callsign | firstname | destination | msg | highlight |
                    case "CR":
                    // Chat message frame after login
                    // CH | chat id | Unix time | callsign | firstname | destination | msg | highlight |
                    case "CH":
                        DataRow Row = MSG.NewRow();
                        DateTime dt = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(msg[2]));
                        Row["TIME"] = dt;
                        latestMessageTimestamp = dt;
                        Row["CALL"] = msg[3].Trim();
                        Row["NAME"] = msg[4].Trim();
                        var recipient = msg[7].Trim();
                        if (recipient.Equals("0"))
                        {
                            // check if forgotten "/cq" -> recipient just part of the message
                            int index = msg[6].IndexOf(' ');
                            if (index > 0)
                            {
                                string check_recipient = msg[6].Substring(0, index);
                                if (WCCheck.WCCheck.IsCall(check_recipient) >= 0)
                                    recipient = check_recipient;
                            }
                            Row["MSG"] = msg[6].Trim();
                        }
                        else
                            Row["MSG"] = "(" + recipient + ") " + msg[6].Trim();
                        Row["RECIPIENT"] = recipient;
                        msg_latest_first = (s.Substring(0, 2)).Equals("CR");

                        // check if the message is already in the list
                        DataRow check_row = MSG.Rows.Find(new object[] { Row["TIME"], Row["CALL"], Row["MSG"] });

                        if (check_row == null || !(check_row["MSG"].Equals(Row["MSG"])))
                        {
                            // add message to list, it is new
                            MSG.Rows.Add(Row);
                        }

                        lock (USER)
                        {
                            // check the recipient of the message
                            DataRow findrow = USER.Rows.Find(recipient);
                            if (findrow != null)
                            {
                                findrow["CONTACTED"] = (int)findrow["CONTACTED"] + 1;
                            }
                            findrow = USER.Rows.Find(Row["CALL"].ToString().Trim());
                            if (findrow != null)
                            {
                                findrow["CONTACTED"] = 0; // clear counter on activity
                                findrow["TIME"] = dt; // store time of activity
                            }
                        }

                        if (process_new_message != null)
                        {
                            process_new_message(this, new newMSGEventArgs(Row));
                        }

                        break;

                    // CK: link check
                    // CK |

                    case "CK": // Link Check
                        if (tw != null)
                            tw.Send("\r\n"); // need to reply
                        break;

                    // End of CR frames
                    case "CE":
                        latestMessageTimestampSet = true;
                        break;
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + s + "): " + e.Message);
                MainDlg.Log.WriteMessage(MethodBase.GetCurrentMethod().Name + "(" + s + "): " + e.Message + "\n" + e.StackTrace);

            }
        }

        public event EventHandler<newMSGEventArgs> process_new_message;
        public class newMSGEventArgs : EventArgs
        {
            /// <summary>
            /// new message received
            /// </summary>
            public newMSGEventArgs(DataRow msg)
            {
                this.msg = msg;
            }
            public DataRow msg { get; private set; }
        }



        private void Receive_USR(string s)
        {
            string[] usr = s.Split('|');
            try
            {
                MainDlg.Log.WriteMessage("KST user: " + s);
                DataRow Row = MSG.NewRow();
                //UA0|3|DF9QX|Matthias 23-1,2|JO42HD|1| -> away
                //UA0|3|DL7QY|Claus 1-122GHz|JN59BD|0|
                //UA0|3|DL8AAU|Alexander|JO41VL|2| -> new
                //UM3|3|DL8AAU|Alexander|JN49HU|2| -> modified
                //US4|3|EA3KZ|0| -> status (2->0)
                //UR6|3|F8DLS| -> left
                //UE|3|7042| -> end of list/update
                switch (s.Substring(0, 2))
                {
                    // User frame at login
                    // UA0 | chat id | callsign | firstname | locator | state |
                    // UA5 user connected (to add)
                    // UA5 | chat id | callsign | firstname | locator | state |

                    case "UA":
                        {
                            DataRow row = USER.NewRow();

                            string call = usr[2].Trim();
                            string loc = usr[4].Trim();
                            if (WCCheck.WCCheck.IsCall(WCCheck.WCCheck.Cut(call)) >= 0 && WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                row["CALL"] = call;
                                row["NAME"] = usr[3].Trim();
                                row["LOC"] = loc;

                                Int32 usr_state = Int32.Parse(usr[5]);
                                row["AWAY"] = (usr_state & 1) == 1;
                                row["RECENTLOGIN"] = (usr_state & 2) == 2;

                                row["CONTACTED"] = 0;
                                row["TIME"] = DateTime.MinValue;

                                lock (USER)
                                {
                                    USER.Rows.Add(row);

                                    if (process_user_update != null)
                                        process_user_update(this, new UserUpdateEventArgs(row, USER_OP.USER_NEW));
                                }
                            }
                            else
                            {
                                MainDlg.Log.WriteMessage("KST user UA: " + s + "not valid");
                            }
                        }
                        break;

                    //User already logged
                    //UM3 | chat id | callsign | firstname | locator | state |

                    case "UM":
                        {
                            lock (USER)
                            {
                                string call = usr[2].Trim();
                                DataRow row = USER.Rows.Find(call);
                                string loc = usr[4].Trim();

                                if (WCCheck.WCCheck.IsLoc(loc) >= 0)
                                {
                                    row["CALL"] = call;
                                    row["NAME"] = usr[3].Trim();
                                    row["LOC"] = loc;

                                    Int32 usr_state = Int32.Parse(usr[5]);
                                    row["AWAY"] = (usr_state & 1) == 1;
                                    row["RECENTLOGIN"] = (usr_state & 2) == 2;

                                    row["CONTACTED"] = 0; // clear counter on activity
                                    row["TIME"] = DateTime.UtcNow; // store time of activity

                                    if (process_user_update != null)
                                        process_user_update(this, new UserUpdateEventArgs(row, USER_OP.USER_MODIFY));
                                }
                            }
                        }
                        break;

                    // User state (here/not here/more than 5 min logged)
                    // US4 | chat id | callsign | state |
                    case "US":
                        {
                            lock (USER)
                            {
                                string call = usr[2].Trim();
                                DataRow row = USER.Rows.Find(call);
                                Int32 usr_state = Int32.Parse(usr[3]);
                                if (row != null)
                                {
                                    row["AWAY"] = (usr_state & 1) == 1;
                                    row["RECENTLOGIN"] = (usr_state & 2) == 2;
                                }
                                if (process_user_update != null)
                                    process_user_update(this, new UserUpdateEventArgs(row, USER_OP.USER_MODIFY_STATE));
                            }
                        }
                        break;

                    // User disconnected (to remove)
                    // UR6 | chat id | callsign |

                    case "UR":
                        {
                            lock (USER)
                            {
                                string call = usr[2].Trim();
                                DataRow row = USER.Rows.Find(call);

                                if (row != null)
                                {
                                    // make a copy
                                    var row_delete = USER.NewRow();
                                    row_delete.ItemArray = row.ItemArray.Clone() as object[];

                                if (process_user_update != null)
                                    process_user_update(this, new UserUpdateEventArgs(row_delete, USER_OP.USER_DELETE));
                                }

                                row.Delete();
                            }
                        }
                        break;

                    // Users statistics/end of users frames
                    // UE | chat id | nb registered users|
                    case "UE":
                        Process_MyCallAway();
                        if (CheckStartUpAway)
                        {
                            if (Settings.Default.KST_StartAsHere && (UserState != USER_STATE.Here))
                                Here();
                            else if (!Settings.Default.KST_StartAsHere && (UserState == USER_STATE.Here))
                                Away();
                            CheckStartUpAway = false;
                        }
                        if (process_user_update != null)
                            process_user_update(this, new UserUpdateEventArgs(null, USER_OP.USER_DONE));
                        break;
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + s + "): " + e.Message + e.StackTrace);
            }
        }


        public event EventHandler<UserUpdateEventArgs> process_user_update;
        public enum USER_OP { USER_NEW, USER_MODIFY, USER_MODIFY_STATE, USER_DELETE, USER_DONE };
        public class UserUpdateEventArgs : EventArgs
        {
            /// <summary>
            /// new user received
            /// </summary>
            public UserUpdateEventArgs(DataRow user, USER_OP user_op)
            {
                this.user = user;
                this.user_op = user_op;
            }
            public DataRow user { get; private set; }
            public USER_OP user_op { get; private set; }
        }


        private void Process_MyCallAway()
        {
            string MyCall = WCCheck.WCCheck.SanitizeCall(Settings.Default.KST_UserName.ToUpper());
            DataRow row = USER.Rows.Find(MyCall);
            if (row != null)
            {
                if (((bool)row["AWAY"] == true && UserState == USER_STATE.Here) ||
                     ((bool)row["AWAY"] == false && UserState == USER_STATE.Away))
                {
                    if ((bool)row["AWAY"] == true)
                    {
                        UserState = USER_STATE.Away;
                    }
                    else
                    {
                        UserState = USER_STATE.Here;
                    }
                    if (update_user_state != null)
                    {
                        update_user_state(this, new userStateEventArgs(UserState));
                    }
                }
            }
        }


        // state changes
        public void Connect()
        {
            if (KSTState == KST_STATE.Standby &&
                !string.IsNullOrEmpty(Settings.Default.KST_ServerName) &&
                !string.IsNullOrEmpty(Settings.Default.KST_UserName))
            {
                tw = new TelnetWrapper();
                tw.DataAvailable += new DataAvailableEventHandler(tw_DataAvailable);
                tw.Disconnected += new DisconnectedEventHandler(tw_Disconnected);
                try
                {
                    tw.Connect(Settings.Default.KST_ServerName, 23001);
                    if (!tw.Connected)
                    {
                        Say("Connection failed...");
                        return;
                    }
                    tw.Receive();
                    KSTState = KST_STATE.WaitLogin;
                    lock (USER)
                    {
                        USER.Clear();
                    }
                    /*
                     TODO: hier oder eher wenn connectet
                    if (Settings.Default.ShowBeacons)
                        KST_Add_Beacons_USR();
                    */
                    /*
                     TODO: brauche ich ti_Main?
                    ti_Main.Interval = 5000;
                    if (!ti_Main.Enabled)
                    {
                        ti_Main.Start();
                    }
                    */
                    Say("Connecting to KST chat..." + Settings.Default.KST_ServerName + " Port " + 23001);
                }
                catch (Exception e)
                {
                    Error(MethodBase.GetCurrentMethod().Name, e.Message);
                    throw;
                }
            }
        }

        public void Disconnect()
        {
            if (KSTState >= KST_STATE.Connected)
            {
                tw.Send("/q\r");
                Say("Disconnected from KST chat...");
                KSTState = KST_STATE.Disconnecting;
            }
        }

        public enum USER_STATE
        {
            Here,
            Away
        }

        private USER_STATE UserState = USER_STATE.Away;

        public event EventHandler<userStateEventArgs> update_user_state;
        public class userStateEventArgs : EventArgs
        {
            /// <summary>
            /// new message received
            /// </summary>
            public userStateEventArgs(USER_STATE state)
            {
                this.state = state;
            }
            public USER_STATE state { get; private set; }
        }



        public void Here()
        {
            UserState = USER_STATE.Here;
            if (KSTState >= KST_STATE.Connected)
            {
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/BACK|0|\r");
                if (update_user_state != null)
                    update_user_state(this, new userStateEventArgs(USER_STATE.Here));
            }
        }

        public void Away()
        {
            UserState = USER_STATE.Away;
            if (KSTState >= KST_STATE.Connected)
            {
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/AWAY|0|\r");
                if (update_user_state != null)
                    update_user_state(this, new userStateEventArgs(USER_STATE.Away));
            }
        }

        public void Setloc(string locator)
        {
            if (KSTState >= KST_STATE.Connected)
            {
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/SETLOC " + locator + "|0|\r");
            }
        }

        public bool Send(string text)
        {
            if (tw == null || !tw.Connected)
                throw new InvalidOperationException("not connected");

            if (text.Length > 0)
            {
                if (!text.StartsWith("/")
                    || text.ToUpper().StartsWith("/CQ")
                    || text.ToUpper().StartsWith("/SHLOC")
                    || text.ToUpper().StartsWith("/SHUSER")
                    )
                {
                    try
                    {
                        // Telnet server does not handle non-ASCII
                        String t = text;
                        t = t.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue");
                        t = t.Replace("Ä", "Ae").Replace("Ö", "Oe").Replace("Ü", "Ue");
                        t = t.Replace("ß", "ss");
                        System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
                        String to_sent = iso_8859_1.GetString(iso_8859_1.GetBytes(t));
                        to_sent = "MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|" + to_sent + "|0|\r";
                        tw.Send(to_sent);
                        MainDlg.Log.WriteMessage("KST message send: " + text);
                        /* ????? what is the code supposed to do?
                        if (cb_Command.FindStringExact(text) != 0)
                        {
                            cb_Command.Items.Insert(0, text);
                        }
                        */
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, e.Message);
                        throw;
                    }
                    return true;
                }
            }
            return false;
        }

        private string KSTBuffer = "";

        private Queue<string> MsgQueue = new Queue<string>();

        private void tw_Disconnected(object sender, TelnetWrapperDisconnctedEventArgs e)
        {
            if (e.Message.Length > 0)
            {
                MainDlg.Log.WriteMessage("Socket Error - " + e.Message);
            }
            MainDlg.Log.WriteMessage("Disconnected from: " + Settings.Default.KST_Chat);
            try
            {
                tw.Dispose();
                tw.Close();
                MsgQueue.Clear();
                KSTBuffer = "";
                Say("Disconnected from KST chat...");
            }
            catch
            {
            }
            if (SendMyName)
            {
                if (KSTState == KST_STATE.WaitLogstat)
                {
                    System.Timers.Timer tmrOnce = new System.Timers.Timer();
                    tmrOnce.Elapsed += Connect_tmrOnce_Tick;
                    tmrOnce.Interval = 500;
                    tmrOnce.Start();

                    KSTState = KST_STATE.WaitTelnetConnect;
                    return;
                }
                if (KSTState == KST_STATE.WaitTelnetSetName)
                {
                    System.Timers.Timer tmrOnce = new System.Timers.Timer();
                    tmrOnce.Elapsed += Connect_tmrOnce_Tick;
                    tmrOnce.Interval = 500;
                    tmrOnce.Start();

                    KSTState = KST_STATE.WaitReconnect;
                    return;
                }
            }
            KSTState = KST_STATE.Disconnected;
        }

        /* one shot timer to handle the connect/reconnect during /set name */

        private void Connect_tmrOnce_Tick(object sender, EventArgs ev)
        {
            if (KSTState == KST_STATE.WaitTelnetConnect ||
                KSTState == KST_STATE.WaitReconnect)
            {
                try
                {
                    tw = new TelnetWrapper();
                    tw.DataAvailable += new DataAvailableEventHandler(tw_DataAvailable);
                    tw.Disconnected += new DisconnectedEventHandler(tw_Disconnected);

                    if (KSTState == KST_STATE.WaitTelnetConnect)
                    {
                        tw.Connect(Settings.Default.KST_ServerName, 23000);
                        Console.WriteLine("connect 23000 " + tw.Connected);
                        MainDlg.Log.WriteMessage("connect to on4kst 23000 ");
                        tw.Receive();
                        KSTState = KST_STATE.WaitTelnetUserName;
                    }
                    else
                    {
                        tw.Connect(Settings.Default.KST_ServerName, 23001);
                        Console.WriteLine("reconnect");
                        tw.Receive();
                        KSTState = KST_STATE.WaitLogin;
                        Say("Connecting to KST chat..." + Settings.Default.KST_ServerName + " Port " + 23001);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
            ((System.Timers.Timer)sender).Dispose();
        }

        private void tw_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            string t = e.Data;
            byte[] bytes = new byte[1];
            try
            {
                for (byte b = 0; b <= 31; b += 1)
                {
                    bytes[0] = b;
                    string esc = Encoding.GetEncoding("iso-8859-1").GetString(bytes, 0, 1);
                    t = t.Replace(esc, "{" + b.ToString("X2") + "}");
                }
            }
            catch
            {
            }
            Console.WriteLine(t);
            string s = e.Data.Replace("\0", "");
            lock (MsgQueue)
            {
                KSTBuffer += s;
                if (KSTBuffer.IndexOf("\r\n") >= 0)
                {
                    string[] buffer = KSTBuffer.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //this.KSTBuffer.Split(new char[] {'\r', '\n' });
                    string[] array = buffer;
                    int number_strings = array.Length - 1;
                    if (KSTBuffer.EndsWith("\r\n"))
                        number_strings++;
                    for (int i = 0; i < number_strings; i++)
                    {
                        string buf = array[i];
                        StringWriter bufWriter = new StringWriter();
                        WebUtility.HtmlDecode(buf, bufWriter);
                        MsgQueue.Enqueue(bufWriter.ToString());
                    }
                    if (KSTBuffer.EndsWith("\r\n"))
                        KSTBuffer = "";
                    else
                        KSTBuffer = buffer[buffer.Length - 1]; // keep the tail
                }
            }
            if (KSTState >= KST_STATE.Connected)
            {
                ti_Linkcheck.Stop();   // restart the linkcheck timer
                ti_Linkcheck.Start();
            }
        }

        private void ti_Receive_Tick(object sender, EventArgs e)
        {
            ti_Receive.Stop();
            DateTime t = DateTime.Now;
            while (true)
            {
                string s;
                lock (MsgQueue)
                {
                    if (MsgQueue.Count > 0)
                    {
                        s = MsgQueue.Dequeue();
                    }
                    else
                    {
                        s = "";
                    }
                }
                if (s.Length > 0)
                {
                    KST_Receive(s);
                }
                else
                    break;
            }
            TimeSpan ts = DateTime.Now - t;
            if (ts.Seconds > 1)
            {
                MainDlg.Log.WriteMessage("ReceiveTimer Overflow: " + ts.ToString());
            }
            ti_Receive.Start();
        }

        private void ti_Linkcheck_Tick(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (KSTState == KST_STATE.Connected)
            {
                tw.Send("\r\n"); // send to check if link is still up
            }
        }


        public void KSTStateMachine()
        {
            try { 
                if (tw != null && !tw.Connected
                    && KSTState != KST_STATE.Standby
                    && KSTState != KST_STATE.WaitTelnetConnect
                    && KSTState != KST_STATE.WaitTelnetUserName
                    && KSTState != KST_STATE.WaitReconnect)
                {
                    KSTState = KST_STATE.Disconnected;
                }
            }
            catch
            {
                KSTState = KST_STATE.Disconnected;
            }

            if (KSTState == KST_STATE.Disconnecting)
            {
                if (tw != null && tw.Connected)
                    tw.Disconnect();
            }
            if (KSTState == KSTcom.KST_STATE.Disconnected)
            {
                if (ti_Linkcheck.Enabled)
                    ti_Linkcheck.Stop();
            }
        }

        private void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
        }
    }
}
