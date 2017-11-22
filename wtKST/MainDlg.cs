using De.Mud.Telnet;
using Net.Graphite.Telnet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WinTest;
using wtKST.Properties;

namespace wtKST
{
    public class MainDlg : Form
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

        public enum USER_STATE
        {
            Here,
            Away
        }

        private static readonly string[] BANDS = new string[] { "144M", "432M", "1_2G", "2_3G", "3_4G", "5_7G", "10G", "24G", "47G", "76G" };
        public DataTable MSG = new DataTable("MSG");

        public DataTable MYMSG = new DataTable("MYMSG");

        private QRVdb qrv = new QRVdb(BANDS);

        public DataTable CALL = new DataTable("CALL");

        private WinTest.WinTestLog wtQSO = null;

        private wtKST.AirScoutInterface AS_if;

        private bool DLLNotLoaded = false;

        private Point OldMousePos = new Point(0, 0);

        public static LogWriter Log = new LogWriter(Directory.GetParent(Application.LocalUserAppDataPath).ToString());

        private TelnetWrapper tw;

        private string KSTBuffer = "";

        private Queue<string> MsgQueue = new Queue<string>();

        private string MyCall = "DL2ALF";

        private MainDlg.KST_STATE KSTState = KST_STATE.Standby;

        private MainDlg.USER_STATE UserState;

        private IContainer components = null;

        private StatusStrip ss_Main;

        private SplitContainer splitContainer1;

        private SplitContainer splitContainer2;

        private SplitContainer splitContainer3;

        private MenuStrip mn_Main;

        private ToolStripMenuItem tsm_File;

        private ToolStripSeparator toolStripSeparator1;

        private ToolStripMenuItem tsi_File_Exit;

        private Label lbl_KST_MyMsg;

        private Label lbl_KST_Calls;

        private ListView lv_Msg;

        private ColumnHeader lvh_Time;

        private ColumnHeader lvh_Call;

        private ColumnHeader lvh_Name;

        private ColumnHeader lvh_Msg;

        private Label lbl_KST_Msg;

        private ListView lv_MyMsg;

        private ColumnHeader columnHeader1;

        private ColumnHeader columnHeader2;

        private ColumnHeader columnHeader3;

        private ColumnHeader columnHeader4;

        private Label lbl_Call;

        private ToolStripMenuItem tsm_KST;

        private ToolStripMenuItem tsi_KST_Connect;

        private ToolStripMenuItem tsi_KST_Disconnect;

        private ToolStripSeparator toolStripSeparator2;

        private ToolStripMenuItem tsi_KST_Here;

        private ToolStripMenuItem tsi_KST_Away;

        private ToolStripMenuItem tsi_Options;

        private ToolStripMenuItem toolStripMenuItem1;

        private ToolStripMenuItem aboutToolStripMenuItem;

        private AboutBox aboutBox1 = new AboutBox();

        private System.Windows.Forms.Timer ti_Main;
        private wtKST.MainDlg.DoubleBufferedListView lv_Calls;
        private ColumnHeader ch_Call;

        private ColumnHeader ch_Name;

        private ColumnHeader ch_Act;

        private ColumnHeader ch_Loc;

        private ColumnHeader columnHeader144;

        private ColumnHeader columnHeader432;

        private ColumnHeader columnHeader1296;

        private ColumnHeader columnHeader2320;

        private ColumnHeader columnHeader3400;

        private ColumnHeader columnHeader5760;

        private ColumnHeader columnHeader10368;

        private ColumnHeader columnHeader24GHz;

        private ColumnHeader columnHeader47GHz;

        private ColumnHeader columnHeader76GHz;

        private const int COLUMN_WIDTH = 20;

        private Label lbl_KST_Status;

        private ToolStripStatusLabel tsl_Info;

        private Button btn_KST_Send;

        private ToolStripStatusLabel tsl_Error;

        private NotifyIcon ni_Main;

        private ContextMenuStrip cmn_Notify;

        private ToolStripMenuItem cmi_Notify_Restore;

        private ToolStripSeparator toolStripSeparator3;

        private ToolStripMenuItem cmi_Notify_Quit;

        private System.Windows.Forms.Timer ti_Receive;

        private System.Windows.Forms.Timer ti_Error;

        private ComboBox cb_Command;

        private System.Windows.Forms.Timer ti_Top;

        private System.Windows.Forms.Timer ti_Reconnect;

        private System.Timers.Timer ti_Linkcheck;

        private ColumnHeader ch_AS;

        private ToolTip tt_Info;

        private BackgroundWorker bw_GetPlanes;

        private bool WinTestLocatorWarning = false;
        private bool msg_latest_first = false;
        private bool hide_away = false;
        private bool sort_by_dir = false;
        private bool ignore_inactive = false;
        private bool hide_worked = false;
        private DateTime latestMessageTimestamp = DateTime.MinValue;
        private bool latestMessageTimestampSet = false;
        private bool CheckStartUpAway = true;
        private bool SendMyLocator = false;
        private bool SendMyName = false;
        private ContextMenuStrip cmn_userlist;
        private ToolStripMenuItem cmn_userlist_chatReviewT;


        public MainDlg()
        {
            InitializeComponent();

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
            MainDlg.Log.FileFormat = "wtKST_{0:d}.log";
            MainDlg.Log.MessageFormat = "{0:u}: {1}";
            MainDlg.Log.WriteMessage("Startup.");
            if (!File.Exists("Telnet.dll"))
            {
                DLLNotLoaded = true;
            }
            Application.Idle += new EventHandler(OnIdle);
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            cb_Command.MouseWheel += new MouseEventHandler(cb_Command_MouseWheel);
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

            CALL.Columns.Add("CALL");
            CALL.Columns.Add("NAME");
            CALL.Columns.Add("LOC");
            CALL.Columns.Add("TIME", typeof(DateTime));
            CALL.Columns.Add("CONTACTED", typeof(int));
            CALL.Columns.Add("LOGINTIME", typeof(DateTime));
            CALL.Columns.Add("ASLAT", typeof(double));
            CALL.Columns.Add("ASLON", typeof(double));
            CALL.Columns.Add("QRB", typeof(int));
            CALL.Columns.Add("DIR", typeof(int));
            CALL.Columns.Add("AWAY", typeof(bool));
            CALL.Columns.Add("AS", typeof(int));
            foreach (string band in BANDS)
                CALL.Columns.Add(band, typeof(int));
            CALL.Columns.Add("COLOR", typeof(int));
            DataColumn[] CALLkeys = new DataColumn[]
            {
                CALL.Columns["CALL"]
            };
            CALL.PrimaryKey = CALLkeys;

            string kstcall = WCCheck.WCCheck.Cut(Settings.Default.KST_UserName.ToUpper());
            // check if we are running on Windows, otherwise Win-Test will not run
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                wtQSO = new WinTestLog(MainDlg.Log.WriteMessage);
            }
            UpdateUserBandsWidth();
            bw_GetPlanes.RunWorkerAsync();
            AS_if = new wtKST.AirScoutInterface(ref bw_GetPlanes);
            if (Settings.Default.KST_AutoConnect)
            {
                KST_Connect();
            }
        }


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
                if (KSTState == MainDlg.KST_STATE.WaitLogstat)
                {
                    System.Windows.Forms.Timer tmrOnce = new System.Windows.Forms.Timer();
                    tmrOnce.Tick += Connect_tmrOnce_Tick;
                    tmrOnce.Interval = 500;
                    tmrOnce.Start();

                    KSTState = MainDlg.KST_STATE.WaitTelnetConnect;
                    return;
                }
                if (KSTState == MainDlg.KST_STATE.WaitTelnetSetName)
                {
                    System.Windows.Forms.Timer tmrOnce = new System.Windows.Forms.Timer();
                    tmrOnce.Tick += Connect_tmrOnce_Tick;
                    tmrOnce.Interval = 500;
                    tmrOnce.Start();

                    KSTState = MainDlg.KST_STATE.WaitReconnect;
                    return;
                }
            }
            KSTState = MainDlg.KST_STATE.Disconnected;
        }

        /* one shot timer to handle the connect/reconnect during /set name */

        private void Connect_tmrOnce_Tick(object sender, EventArgs ev)
        {
            if (KSTState == MainDlg.KST_STATE.WaitTelnetConnect ||
                KSTState == MainDlg.KST_STATE.WaitReconnect)
            {
                try
                {
                    tw = new TelnetWrapper();
                    tw.DataAvailable += new DataAvailableEventHandler(tw_DataAvailable);
                    tw.Disconnected += new DisconnectedEventHandler(tw_Disconnected);

                    if (KSTState == MainDlg.KST_STATE.WaitTelnetConnect)
                    {
                        tw.Connect(Settings.Default.KST_ServerName, 23000);
                        Console.WriteLine("connect 23000 " + tw.Connected);
                        tw.Receive();
                        KSTState = MainDlg.KST_STATE.WaitTelnetUserName;
                    }
                    else
                    {
                        tw.Connect(Settings.Default.KST_ServerName, Convert.ToInt32(Settings.Default.KST_ServerPort));
                        Console.WriteLine("reconnect");
                        tw.Receive();
                        KSTState = MainDlg.KST_STATE.WaitLogin;
                        Say("Connecting to KST chat..." + Settings.Default.KST_ServerName + " Port " + Settings.Default.KST_ServerPort);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
            ((System.Windows.Forms.Timer)sender).Dispose();
        }


        private void set_KST_Status()
        {
            if (KSTState >= MainDlg.KST_STATE.WaitTelnetConnect && 
                KSTState <= MainDlg.KST_STATE.WaitReconnect)
            {
                lbl_KST_Status.BackColor = Color.LightYellow;
                lbl_KST_Status.Text = "Status: Setting Name ";
                return;
            }
            if (KSTState < MainDlg.KST_STATE.Connected)
            {
                lbl_KST_Status.BackColor = Color.LightGray;
                lbl_KST_Status.Text = "Status: Disconnected ";
                return;
            }
            lbl_KST_Status.BackColor = Color.PaleTurquoise;
            lbl_KST_Status.Text = string.Concat(new string[]
            {
                "Status: Connected to ",
                Settings.Default.KST_Chat,
                " chat   [",
                Settings.Default.KST_UserName,
                " ",
                Settings.Default.KST_Name,
                " ",
                Settings.Default.KST_Loc,
                "]"
            });
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
                if (KSTBuffer.IndexOf("\r\n")>=0)
                {
                    string[] buffer = KSTBuffer.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //this.KSTBuffer.Split(new char[] {'\r', '\n' });
                    string[] array = buffer;
                    int number_strings = array.Length-1;
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
            if (KSTState >= MainDlg.KST_STATE.Connected)
            {
                ti_Linkcheck.Stop();   // restart the linkcheck timer
                ti_Linkcheck.Start();
            }
        }

        private void OnIdle(object sender, EventArgs args)
        {
            if (DLLNotLoaded)
            {
                MessageBox.Show("The file telnet.dll could not be found in program directory.", "Error");
                base.Close();
            }
            try
            {
                if (tw != null && !tw.Connected 
                    && KSTState != MainDlg.KST_STATE.Standby 
                    && KSTState != MainDlg.KST_STATE.WaitTelnetConnect
                    && KSTState != MainDlg.KST_STATE.WaitTelnetUserName
                    && KSTState != MainDlg.KST_STATE.WaitReconnect)
                {
                    KSTState = MainDlg.KST_STATE.Disconnected;
                }
            }
            catch
            {
                KSTState = MainDlg.KST_STATE.Disconnected;
            }
            if (KSTState == MainDlg.KST_STATE.Disconnecting)
            {
                if (tw != null && tw.Connected)
                    tw.Disconnect();
            }
            if (KSTState == MainDlg.KST_STATE.Disconnected)
            {
                lock(CALL)
                {
                    CALL.Clear();
                }
                lv_Calls.Items.Clear();
                tsi_KST_Connect.Enabled = true;
                tsi_KST_Disconnect.Enabled = false;
                tsi_KST_Here.Enabled = false;
                tsi_KST_Away.Enabled = false;
                cb_Command.Text = "";
                cb_Command.Enabled = false;
                btn_KST_Send.Enabled = false;
                lbl_Call.Enabled = false;
                ti_Main.Stop();
                AS_if.planes.Clear();
                if (Settings.Default.KST_AutoConnect && !ti_Reconnect.Enabled)
                {
                    ti_Reconnect.Start();
                }
                if (ti_Linkcheck.Enabled)
                    ti_Linkcheck.Stop();

                KSTState = MainDlg.KST_STATE.Standby;
            }
            if (KSTState >= MainDlg.KST_STATE.Connected)
            {
                tsi_KST_Connect.Enabled = false;
                tsi_KST_Disconnect.Enabled = true;
                tsi_KST_Here.Enabled = true;
                tsi_KST_Away.Enabled = true;
                cb_Command.Enabled = true;
                btn_KST_Send.Enabled = true;
                lbl_Call.Enabled = true;
                if (ti_Reconnect.Enabled)
                {
                    ti_Reconnect.Stop();
                }
            }
            // FIXME: don't touch UI elements here without checking wether they need to be updated. Otherwise CPU load may be high
            if (UserState == MainDlg.USER_STATE.Here)
            {
                lbl_Call.Text = Settings.Default.KST_UserName.ToUpper();
            }
            else
            {
                lbl_Call.Text = "(" + Settings.Default.KST_UserName.ToUpper() + ")";
            }
            string KST_Calls_Text = lbl_KST_Calls.Text;
            if (lv_Calls.Items.Count > 0)
            {
                KST_Calls_Text = "Calls [" + lv_Calls.Items.Count.ToString() + "]";
                if (wtQSO != null && Settings.Default.WinTest_Activate)
                    KST_Calls_Text += " - " + Path.GetFileName(wtQSO.getFileName());
            }
            else
            {
                KST_Calls_Text = "Calls";
            }
            if (!KST_Calls_Text.Equals(lbl_KST_Calls.Text))
                lbl_KST_Calls.Text = KST_Calls_Text;
            if (lv_Msg.Items.Count > 0)
            {
                lbl_KST_Msg.Text = "Messages [" + lv_Msg.Items.Count.ToString() + "]";
            }
            else
            {
                lbl_KST_Msg.Text = "Messages";
            }
            if (lv_MyMsg.Items.Count > 0)
            {
                lbl_KST_MyMsg.Text = "My Messages [" + lv_MyMsg.Items.Count.ToString() + "]";
            }
            else
            {
                lbl_KST_MyMsg.Text = "My Messages";
            }

            set_KST_Status();

            ni_Main.Text = "wtKST\nLeft click to activate";

            if (!aboutBox1.Visible &&!cb_Command.IsDisposed && !cb_Command.Focused && !btn_KST_Send.Capture)
            {
                cb_Command.Focus();
                cb_Command.SelectionLength = 0;
                cb_Command.SelectionStart = cb_Command.Text.Length;
            }
        }

        private void KST_Receive(string s)
        {
            MainDlg.KST_STATE kSTState = KSTState;
            switch (kSTState)
            {
            case MainDlg.KST_STATE.WaitLogin:
                if (s.IndexOf("login") >= 0)
                {
                        // LOGINC|callsign|password|chat id|client software version|past messages number|past dx/map number|
                        // users list/update flags|last Unix timestamp for messages|last Unix timestamp for dx/map|

                        tw.Send("LOGINC|" + Settings.Default.KST_UserName + "|" + Settings.Default.KST_Password + "|"
                        + Settings.Default.KST_Chat.Substring(0, 1) + "|wtKST " + typeof(MainDlg).Assembly.GetName().Version +
                        "|25|0|1|" + 
                        // we try to get the messages up to our latest one
                        (!latestMessageTimestampSet ? "0" :
                        ((latestMessageTimestamp - new DateTime(1970, 1, 1)).TotalSeconds - 1).ToString() )
                        +  "|0|\r");
                        KSTState = MainDlg.KST_STATE.WaitLogstat;
                        Say("Login " + Settings.Default.KST_UserName + " send.");
                }
                break;
            case MainDlg.KST_STATE.WaitLogstat:
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
                                    MessageBox.Show("Password wrong", "Login failed", 
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    tw.Close();
                                    MainDlg.Log.WriteMessage("Password wrong ");
                                    break;
                                }
                                //FIXME currently no way to set the name... so just take it from ON4KST
                                if (Settings.Default.KST_Name.Length == 0)
                                {
                                Settings.Default.KST_Name = subs[6];
                                } 
                                else
                                {
                                    if (!Settings.Default.KST_Name.Equals(subs[6]))
                                    {
                                        SendMyName = true;
                                        // we cannot set the name on KST side using port 23001 - no /setname there
                                        // but we can do this through the regular telnet port 23000, so disconnect and do it there
                                        tw.Disconnect();
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
                        KSTState = MainDlg.KST_STATE.WaitSPR;
                        Say("LOGSTAT " + s);
                }
                break;
            case MainDlg.KST_STATE.WaitSPR:
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
                        KSTState = MainDlg.KST_STATE.Connected;
                        Say("Connected to KST chat.");
                        MainDlg.Log.WriteMessage("Connected to: " + Settings.Default.KST_Chat);
                        msg_latest_first = true;
                        CheckStartUpAway = true;

                        if (SendMyLocator)
                        {
                            KST_Setloc(Settings.Default.KST_Loc);
                            SendMyLocator = false;
                        }

                        ti_Linkcheck.Stop();   // restart the linkcheck timer
                        ti_Linkcheck.Start();
                    }
                    break;

            // special parts called while doing Telnet to set Name
            case MainDlg.KST_STATE.WaitTelnetUserName:
                if (s.IndexOf("Login:") >= 0)
                {
                    Thread.Sleep(100);
                    this.tw.Send(Settings.Default.KST_UserName + "\r");
                    this.KSTState = MainDlg.KST_STATE.WaitTelnetPassword;
                    this.Say("Login " + Settings.Default.KST_UserName + " send.");
                }
                break;
            case MainDlg.KST_STATE.WaitTelnetPassword:
                if (s.IndexOf("Password:") >= 0)
                {
                    Thread.Sleep(100);
                    this.tw.Send(Settings.Default.KST_Password + "\r");
                    this.KSTState = MainDlg.KST_STATE.WaitTelnetChat;
                    this.Say("Password send.");
                }
                break;
            case MainDlg.KST_STATE.WaitTelnetChat:
                if (s.IndexOf("Your choice           :") >= 0)
                {
                    Thread.Sleep(100);
                    this.tw.Send(Settings.Default.KST_Chat.Substring(0, 1) + "\r");
                }
                if (s.IndexOf(">") > 0)
                {
                    if (SendMyName)
                    {
                        this.tw.Send("/set name " + Settings.Default.KST_Name + "\r");
                    }
                    this.KSTState = MainDlg.KST_STATE.WaitTelnetSetName;
                }
                break;
            case MainDlg.KST_STATE.WaitTelnetSetName:
                tw.Disconnect();

                break;

            default:
                switch (kSTState)
                {
                case MainDlg.KST_STATE.Connected:
                    if(s.Substring(0,1).Equals("C"))
                    {
                        KST_Receive_MSG(s);
                    }
                    else if (s.Substring(0, 1).Equals("U"))
                    {
                        KST_Receive_USR(s);
                    }
                    break;
                }
                break;
            }
        }


        private void KST_Receive_MSG(string s)
        {
            string[] msg = s.Split('|'); ;
            MyCall = Settings.Default.KST_UserName.ToUpper();
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
                        KST_Process_new_message(Row);
                        break;

                        // CK: link check
                        // CK |

                    case "CK": // Link Check
                        if (tw!= null)
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

        private int next_color = 1;

        private void KST_Process_new_message(DataRow Row)
        {
            try
            {
                // check if the message is already in the list
                DataRow check_row = MSG.Rows.Find(new string[] { Row["TIME"].ToString(), Row["CALL"].ToString(), Row["MSG"].ToString() });

                if (check_row != null)
                {
                    if (check_row["MSG"].Equals(Row["MSG"]))
                    {
                        return;
                    }
                }
                // add message to list
                MSG.Rows.Add(Row);

                DateTime dt = (DateTime)Row["TIME"];

                // TODO: what if user is not in user list yet? time not updated then
                // -> count appearing on user list as "activity", too
                qrv.set_time(Row["CALL"].ToString().Trim(), dt);

                ListViewItem LV = new ListViewItem();
                LV.Text = ((DateTime)Row["TIME"]).ToString("HH:mm");
                LV.Tag = dt; // store the timestamp in the tag field
                LV.SubItems.Add(Row["CALL"].ToString());
                LV.SubItems.Add(Row["NAME"].ToString());
                LV.SubItems.Add(Row["MSG"].ToString());
                for (int i = 0; i < LV.SubItems.Count; i++)
                {
                    LV.SubItems[i].Name = lv_Msg.Columns[i].Text;
                }
                int current_index = 0;
                if (lv_Msg.Items.Count == 0)
                {
                    // first entry
                    lv_Msg.Items.Insert(0, LV);
                    lv_Msg.TopItem = lv_Msg.Items[0];
                }
                else
                {
                    ListViewItem topitem = lv_Msg.TopItem;
                    Boolean AtBeginningOfList = (lv_Msg.TopItem == lv_Msg.Items[0]);
                        // display at beginning of list (so newest entry displayed)

                    DateTime dt_top = (DateTime)lv_Msg.Items[0].Tag;

                    if (msg_latest_first)
                    {
                        if (latestMessageTimestampSet)
                        {
                            // reconnect, sort CR at begining
                            // probably better to re-generate the whole list... alternate line highlighting will not work
                            // not great, but ok for the time being...
                            for (int i=0; i< lv_Msg.Items.Count; i++ )
                            {
                                if (dt > (DateTime)lv_Msg.Items[i].Tag)
                                {
                                    lv_Msg.Items.Insert(i, LV);
                                    current_index = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // add at end of list
                            lv_Msg.Items.Insert(lv_Msg.Items.Count, LV);
                            current_index = lv_Msg.Items.Count - 1;
                        }
                    }
                    else
                    {
                        lv_Msg.Items.Insert(0, LV);
                    }
                    if (AtBeginningOfList)
                        lv_Msg.TopItem = lv_Msg.Items[0];
                    else
                    {
                        if (topitem != null)
                        {
                            lv_Msg.TopItem = topitem;
                        }
                        if (!ti_Top.Enabled)
                        {
                            ti_Top.Start();
                        }
                    }
                }

                if (lv_Msg.Items.Count % 2 == 0)
                    lv_Msg.Items[current_index].BackColor = Color.Ivory;
                else
                    lv_Msg.Items[current_index].BackColor = Color.FromArgb(0xAFBBE9); //approx. Pale cornflower blue or LightSteelBlue?
                if (Row["RECIPIENT"].ToString().Equals(MyCall))
                {
                    lv_Msg.Items[current_index].BackColor = Color.Coral;
                }
                lock (CALL)
                {
                    // check the recipient of the message
                    DataRow findrow = CALL.Rows.Find(Row["RECIPIENT"].ToString());
                    if (findrow != null)
                    {
                        findrow["CONTACTED"] = (int)findrow["CONTACTED"] + 1;
                    }
                    findrow = CALL.Rows.Find(Row["CALL"].ToString().Trim());
                    if (findrow != null)
                    {
                        findrow["CONTACTED"] = 0; // clear counter on activity
                        findrow["TIME"] = dt; // store time of activity
                    }
                }
                bool fromMe = Row["CALL"].ToString().ToUpper().StartsWith(MyCall);

                if (Row["MSG"].ToString().ToUpper().StartsWith("(" + MyCall + ")") || Row["MSG"].ToString().ToUpper().StartsWith(MyCall) 
                    || ( fromMe && Settings.Default.KST_Show_Own_Messages ))
                {
                    // https://material.io/guidelines/style/color.html#color-color-palette
                    DataRow findrow;
                    if (fromMe)
                        findrow = CALL.Rows.Find(Row["RECIPIENT"].ToString().Trim());
                    else
                        findrow = CALL.Rows.Find(Row["CALL"].ToString().Trim());

                    int color_index = next_color;

                    if (findrow != null)
                    {
                        color_index = (int)findrow["COLOR"];

                        if (color_index == 0)
                        {
                            color_index = next_color;
                            next_color++;
                            if (next_color > 9)
                                next_color = 1;
                            lock (CALL)
                            {
                                findrow["COLOR"] = color_index;
                            }
                        }
                    }

                    ListViewItem MyLV = new ListViewItem();
                    if (Row["MSG"].ToString().ToUpper().StartsWith("(" + MyCall + ")"))
                    {
                        switch(color_index)
                        {
                            case 1: MyLV.BackColor = Color.FromArgb(0xEF9A9A); break;// red 200
                            case 2: MyLV.BackColor = Color.FromArgb(0xCE93D8); break;// purple 200
                            case 3: MyLV.BackColor = Color.FromArgb(0x9FA8DA); break;// indigo 200
                            case 4: MyLV.BackColor = Color.FromArgb(0x80DEEA); break;// cyan 200
                            case 5: MyLV.BackColor = Color.FromArgb(0xA5D6A7); break;// green 200
                            case 6: MyLV.BackColor = Color.FromArgb(0xE6EE9C); break;// lime 200
                            case 7: MyLV.BackColor = Color.FromArgb(0xFFE082); break;// amber 200
                            case 8: MyLV.BackColor = Color.FromArgb(0xBCAAA4); break;// brown 200
                            case 9: MyLV.BackColor = Color.FromArgb(0x90CAF9); break;// blue 200
                        }
                    }
                    else if (fromMe)
                    {
                        switch (color_index)
                        {
                            case 1: MyLV.BackColor = Color.FromArgb(0xFFEBEE); break;// red 50
                            case 2: MyLV.BackColor = Color.FromArgb(0xF3E5F5); break;// purple 50
                            case 3: MyLV.BackColor = Color.FromArgb(0xE8EAF6); break;// indigo 50
                            case 4: MyLV.BackColor = Color.FromArgb(0xE0F7FA); break;// cyan 50
                            case 5: MyLV.BackColor = Color.FromArgb(0xE8F5E9); break;// green 50
                            case 6: MyLV.BackColor = Color.FromArgb(0xF9FBE7); break;// lime 50
                            case 7: MyLV.BackColor = Color.FromArgb(0xFFF8E1); break;// amber 50
                            case 8: MyLV.BackColor = Color.FromArgb(0xEFEBE9); break;// brown 50
                            case 9: MyLV.BackColor = Color.FromArgb(0xE3F2FD); break;// blue 50
                        }
                    }
                    else
                    {
                        switch (color_index)
                        {
                            case 1: MyLV.BackColor = Color.FromArgb(0xFFCDD2); break;// red 100
                            case 2: MyLV.BackColor = Color.FromArgb(0xE1BEE7); break;// purple 100
                            case 3: MyLV.BackColor = Color.FromArgb(0xC5CAE9); break;// indigo 100
                            case 4: MyLV.BackColor = Color.FromArgb(0xB2EBF2); break;// cyan 100
                            case 5: MyLV.BackColor = Color.FromArgb(0xC8E6C9); break;// green 100
                            case 6: MyLV.BackColor = Color.FromArgb(0xF0F4C3); break;// lime 100
                            case 7: MyLV.BackColor = Color.FromArgb(0xFFECB3); break;// amber 100
                            case 8: MyLV.BackColor = Color.FromArgb(0xD7CCC8); break;// brown 100
                            case 9: MyLV.BackColor = Color.FromArgb(0xBBDEFB); break;// blue 100
                        }
                    }
                    MyLV.Text = ((DateTime)Row["TIME"]).ToString("HH:mm");
                    MyLV.SubItems.Add(Row["CALL"].ToString());
                    MyLV.SubItems.Add(Row["NAME"].ToString());
                    MyLV.SubItems.Add(Row["MSG"].ToString());
                    for (int i = 0; i < MyLV.SubItems.Count; i++)
                    {
                        MyLV.SubItems[i].Name = lv_MyMsg.Columns[i].Text;
                    }
                    if (msg_latest_first)
                    {
                        lv_MyMsg.Items.Insert(lv_MyMsg.Items.Count, MyLV);
                    }
                    else
                    {
                        lv_MyMsg.Items.Insert(0, MyLV);
                    }
                    lv_MyMsg.Items[0].EnsureVisible();
                    int hwnd = MainDlg.GetForegroundWindow();
                    if (hwnd != base.Handle.ToInt32())
                    {
                        if (Settings.Default.KST_ShowBalloon && !fromMe)
                        {
                            ni_Main.ShowBalloonTip(30000, "New MyMessage received", Row["MSG"].ToString(), ToolTipIcon.Info);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, e.Message);
                MainDlg.Log.WriteMessage(MethodBase.GetCurrentMethod().Name + Row.ToString() + e.Message + "\n" + e.StackTrace);
            }
        }

        private void KST_Update_USR_Window()
        {
            if (KSTState <= KST_STATE.WaitLogin) //FIXME needed still?
                return;
            try
            {
                string topcall = "";
                if (lv_Calls.TopItem != null)
                {
                    topcall = lv_Calls.TopItem.Text;
                }
                lv_Calls.Items.Clear();
                lv_Calls.BeginUpdate();
                DataView view;
                lock (CALL)
                {
                    view = CALL.DefaultView;
                }
                if (sort_by_dir)
                    view.Sort = "DIR ASC";
                else
                    view.Sort = "CALL ASC";
                DataTable tbl = view.ToTable();

                foreach (DataRow row in tbl.Rows)
                {
                    // ignore own call
                    if (row["CALL"].ToString().IndexOf(Settings.Default.KST_UserName.ToUpper()) >= 0)
                        continue;

                    ListViewItem LV = new ListViewItem();

                    if ((bool)row["AWAY"])
                    {
                        if (hide_away)
                            continue;
                        LV.Text = "(" + row["CALL"].ToString() + ")";
                        // Italic is too difficult to read and the font gets bigger
                        //LV.Font = new Font(LV.Font, FontStyle.Italic);
                    }
                    else
                        LV.Text = row["CALL"].ToString();
                    // Ignore stations too far away
                    int MaxDist = Convert.ToInt32(Settings.Default.KST_MaxDist);
                    if (MaxDist !=0 && (int)row["QRB"] > MaxDist)
                        continue;

                    // drop calls that are already in log
                    if (hide_worked && check_in_log(row))
                        continue;

                    // login time - new calls should be bold
                    DateTime logintime = (DateTime)row["LOGINTIME"];
                    double loggedOnMinutes = (DateTime.UtcNow.Subtract(logintime)).TotalMinutes;
                    if (loggedOnMinutes < 5)
                        LV.Font = new Font(LV.Font, FontStyle.Bold);

                    LV.UseItemStyleForSubItems = false;
                    LV.SubItems.Add(row["NAME"].ToString());
                    LV.SubItems.Add(row["LOC"].ToString());

                    // last activity
                    double lastActivityMinutes = (DateTime.UtcNow.Subtract((DateTime)row["TIME"])).TotalMinutes;
                    //MainDlg.Log.WriteMessage("KST Time " + LV.Text + " " + DateTime.UtcNow + " " + (DateTime)row["TIME"] + " " + lastActivityMinutes.ToString("0"));
                    if (lastActivityMinutes < 120.0)
                        LV.SubItems.Add(lastActivityMinutes.ToString("0"));
                    else
                    {
                        if (ignore_inactive)
                            continue;
                        if ((int)row["CONTACTED"] < 3)
                            LV.SubItems.Add("---");
                        else
                            LV.SubItems.Add("xxx"); // if contacted 3 times without answer then probably really not available
                    }
                    int qrb = (int)row["QRB"];
                    if (Settings.Default.AS_Active)
                    {
                        if (qrb >= Convert.ToInt32(Settings.Default.AS_MinDist) && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                        {
                            LV.SubItems.Add(AS_if.GetNearestPlanePotential(row["CALL"].ToString()));
                        }
                        else if (qrb >= Convert.ToInt32(Settings.Default.AS_MinDist))
                        {
                            // too far
                            LV.SubItems.Add(">");
                        }
                        else
                        {
                            // too close
                            LV.SubItems.Add("<");
                        }
                    }
                    else
                    {
                        LV.SubItems.Add("0");
                    }
                    foreach (string band in BANDS)
                        LV.SubItems.Add(row[band].ToString());

                    for (int j = 0; j < lv_Calls.Columns.Count; j++)
                    {
                        LV.SubItems[j].Name = lv_Calls.Columns[j].Text.Replace(".", "_");
                    }
                    lv_Calls.Items.Add(LV);
                }
                lv_Calls.EndUpdate();
                ListViewItem toplv = lv_Calls.FindItemWithText(topcall);
                if (toplv != null)
                {
                    // strange behavior of ListView... you need to set the TopItem to the end, otherwise the list
                    // starts to scroll even the TopItem is set correctly... thanks Frank
                    lv_Calls.TopItem = lv_Calls.Items[lv_Calls.Items.Count - 1];
                    lv_Calls.TopItem = toplv;
                }
                if (wtQSO != null & WinTestLocatorWarning)
                    Say("");
                MainDlg.Log.WriteMessage("KST GetUsers finished: " + lv_Calls.Items.Count.ToString() + " Calls.");
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, e.Message);
                MainDlg.Log.WriteMessage(MethodBase.GetCurrentMethod().Name + e.Message + "\n" + e.StackTrace);
            }
        }

        private void KST_Add_Beacons_USR()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Settings.Default.BeaconFileName))
                {
                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            string bs = sr.ReadLine();
                            DataRow row = CALL.NewRow();
                            row["CALL"] = bs.Split(new char[] { ' ' })[0].Trim();
                            row["NAME"] = "Beacon";
                            row["LOC"] = bs.Split(new char[] { ' ' })[1].Trim();
                            row["TIME"] = DateTime.MinValue;
                            row["CONTACTED"] = 0;
                            foreach (string band in BANDS)
                                row[band] = QRVdb.QRV_STATE.unknown;
                            lock (CALL)
                            {
                                CALL.Rows.Add(row);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        private void KST_Process_MyCallAway()
        {
            DataRow row = CALL.Rows.Find(MyCall);
            if (row != null)
            {
                if ( ((bool)row["AWAY"] == true && UserState == MainDlg.USER_STATE.Here) ||
                     ((bool)row["AWAY"] == false && UserState == MainDlg.USER_STATE.Away) )
                {
                    UserState = (bool)row["AWAY"] ? MainDlg.USER_STATE.Away : MainDlg.USER_STATE.Here;
                }
            }
        }


        private void KST_Receive_USR(string s)
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
                            DataRow row = CALL.NewRow();

                            string call = usr[2].Trim();
                            string loc = usr[4].Trim();
                            if (WCCheck.WCCheck.IsCall(WCCheck.WCCheck.Cut(call)) >= 0 && WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                row["CALL"] = call;
                                row["NAME"] = usr[3].Trim();
                                row["LOC"] = loc;

                                int qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc);
                                row["QRB"] = qrb;
                                row["DIR"] = qtf;

                                row["COLOR"] = 0;

                                Int32 usr_state = Int32.Parse(usr[5]);
                                row["AWAY"] = (usr_state & 1) == 1;
                                string qrvcall = usr[2].Trim();
                                if (qrvcall.IndexOf("-") > 0)
                                {
                                    qrvcall = qrvcall.Remove(qrvcall.IndexOf("-"));
                                }
                                bool call_new_in_userlist = false;

                                if ((usr_state & 2) == 2)
                                {
                                    call_new_in_userlist = true;
                                    row["LOGINTIME"] = DateTime.UtcNow; // remember time of login
                                }
                                else
                                {
                                    row["LOGINTIME"] = DateTime.MinValue;
                                }
                                row["CONTACTED"] = 0;

                                qrv.Process_QRV(row, qrvcall, call_new_in_userlist);

                                lock (CALL)
                                {
                                    CALL.Rows.Add(row);
                                    if (call_new_in_userlist && wtQSO != null && wtQSO.QSO.Rows.Count > 0)
                                        Check_QSO(row);
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
                            lock (CALL)
                            {
                                string call = usr[2].Trim();
                                DataRow row = CALL.Rows.Find(call);
                                string loc = usr[4].Trim();

                                if (WCCheck.WCCheck.IsLoc(loc) >= 0)
                                {
                                    row["CALL"] = call;
                                    row["NAME"] = usr[3].Trim();
                                    row["LOC"] = loc;

                                    int qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc);
                                    int qtf = (int)WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc);
                                    row["QRB"] = qrb;
                                    row["DIR"] = qtf;

                                    Int32 usr_state = Int32.Parse(usr[5]);
                                    row["AWAY"] = (usr_state & 1) == 1;
                                }
                            }
                        }
                        break;

                        // User state (here/not here/more than 5 min logged)
                        // US4 | chat id | callsign | state |
                    case "US":
                        {
                            lock (CALL)
                            {
                                string call = usr[2].Trim();
                                DataRow row = CALL.Rows.Find(call);
                                Int32 usr_state = Int32.Parse(usr[3]);
                                if (row != null)
                                    row["AWAY"] = (usr_state & 1) == 1;
                            }
                        }
                        break;

                        // User disconnected (to remove)
                        // UR6 | chat id | callsign |

                    case "UR":
                        {
                            lock (CALL)
                            {
                                string call = usr[2].Trim();
                                DataRow row = CALL.Rows.Find(call);
                                if (row != null)
                                    row.Delete();
                            }
                        }
                        break;

                        // Users statistics/end of users frames
                        // UE | chat id | nb registered users|
                    case "UE":
                        KST_Process_MyCallAway();
                        if (CheckStartUpAway)
                        {
                            if (Settings.Default.KST_StartAsHere && (UserState != MainDlg.USER_STATE.Here))
                                KST_Here();
                            else if (!Settings.Default.KST_StartAsHere && (UserState == MainDlg.USER_STATE.Here))
                                KST_Away();
                            CheckStartUpAway = false;
                        }
                        KST_Update_USR_Window();
                        if (Settings.Default.AS_Active)
                            AS_send_ASWATCHLIST();
                        // TODO wt?
                        break;
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + s + "): " + e.Message + e.StackTrace);
            }
        }

        private void KST_Connect()
        {
            if (KSTState == MainDlg.KST_STATE.Standby &&
                !string.IsNullOrEmpty(Settings.Default.KST_ServerName) &&
                !string.IsNullOrEmpty(Settings.Default.KST_UserName))
            {
                tw = new TelnetWrapper();
                tw.DataAvailable += new DataAvailableEventHandler(tw_DataAvailable);
                tw.Disconnected += new DisconnectedEventHandler(tw_Disconnected);
                try
                {
                    tw.Connect(Settings.Default.KST_ServerName, Convert.ToInt32(Settings.Default.KST_ServerPort));
                    tw.Receive();
                    KSTState = MainDlg.KST_STATE.WaitLogin;
                    lock (CALL)
                    {
                        CALL.Clear();
                    }
                    if (Settings.Default.ShowBeacons)
                        KST_Add_Beacons_USR();
                    ti_Main.Interval = 5000;
                    if (!ti_Main.Enabled)
                    {
                        ti_Main.Start();
                    }
                    Say("Connecting to KST chat..." + Settings.Default.KST_ServerName + " Port "+ Settings.Default.KST_ServerPort);
                }
                catch (Exception e)
                {
                    Error(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        private void KST_Disconnect()
        {
            if (KSTState >= MainDlg.KST_STATE.Connected)
            {
                tw.Send("/q\r");
                Say("Disconnected from KST chat...");
                KSTState = MainDlg.KST_STATE.Disconnecting;
            }
        }

        private void KST_Here()
        {
            if (KSTState >= MainDlg.KST_STATE.Connected)
            {
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/BACK|0|\r");
                UserState = MainDlg.USER_STATE.Here;
            }
        }

        private void KST_Away()
        {
            if (KSTState >= MainDlg.KST_STATE.Connected)
            {
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/AWAY|0|\r");
                UserState = MainDlg.USER_STATE.Away;
            }
        }

        private void KST_Setloc(string locator)
        {
            if (KSTState >= MainDlg.KST_STATE.Connected)
            {
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/SETLOC " + locator + "|0|\r");
            }
        }

        private void KST_Send()
        {
            if (tw != null && tw.Connected && cb_Command.Text.Length > 0)
            {
                if ( !cb_Command.Text.StartsWith("/")
                    || cb_Command.Text.ToUpper().StartsWith("/CQ")
                    || cb_Command.Text.ToUpper().StartsWith("/SHLOC")
                    || cb_Command.Text.ToUpper().StartsWith("/SHUSER")
                    )
                {
                    try
                    {
                        // Telnet server does not handle non-ASCII
                        String t = cb_Command.Text;
                        t = t.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue");
                        t = t.Replace("Ä", "Ae").Replace("Ö", "Oe").Replace("Ü", "Ue");
                        t = t.Replace("ß", "ss");
                        System.Text.Encoding iso_8859_1 = System.Text.Encoding.GetEncoding("iso-8859-1");
                        String to_sent = iso_8859_1.GetString(iso_8859_1.GetBytes(t));
                        to_sent = "MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|" + to_sent + "|0|\r";
                        tw.Send(to_sent);
                        MainDlg.Log.WriteMessage("KST message send: " + cb_Command.Text);
                        if (cb_Command.FindStringExact(cb_Command.Text) != 0)
                        {
                            cb_Command.Items.Insert(0, cb_Command.Text);
                        }
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, e.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Sending commands except \"/cq\", \"/shloc\" and \"/shuser\" is not allowed!", "KST SendCommand");
                }
                cb_Command.ResetText();
            }
        }

        private void tsi_File_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                KST_Disconnect();
            }
            catch
            {
            }
            base.Close();
        }

        // hide bands that should not be displayed by making their width = 0
        private void UpdateUserBandsWidth()
        {
            if (Settings.Default.Band_144)
                columnHeader144.Width = COLUMN_WIDTH;
            else
                columnHeader144.Width = 0;
            if (Settings.Default.Band_432)
                columnHeader432.Width = COLUMN_WIDTH;
            else
                columnHeader432.Width = 0;
            if (Settings.Default.Band_1296)
                columnHeader1296.Width = COLUMN_WIDTH;
            else
                columnHeader1296.Width = 0;
            if (Settings.Default.Band_2320)
                columnHeader2320.Width = COLUMN_WIDTH;
            else
                columnHeader2320.Width = 0;
            if (Settings.Default.Band_3400)
                columnHeader3400.Width = COLUMN_WIDTH;
            else
                columnHeader3400.Width = 0;
            if (Settings.Default.Band_5760)
                columnHeader5760.Width = COLUMN_WIDTH;
            else
                columnHeader5760.Width = 0;
            if (Settings.Default.Band_10368)
                columnHeader10368.Width = COLUMN_WIDTH;
            else
                columnHeader10368.Width = 0;
            if (Settings.Default.Band_24GHz)
                columnHeader24GHz.Width = COLUMN_WIDTH;
            else
                columnHeader24GHz.Width = 0;
            if (Settings.Default.Band_47GHz)
                columnHeader47GHz.Width = COLUMN_WIDTH;
            else
                columnHeader47GHz.Width = 0;
            if (Settings.Default.Band_76GHz)
                columnHeader76GHz.Width = COLUMN_WIDTH;
            else
                columnHeader76GHz.Width = 0;
        }

        bool check_in_log(DataRow row)
        {
            if (Settings.Default.Band_144 && 
                (QRVdb.QRV_STATE)row["144M"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["144M"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_432 && 
                (QRVdb.QRV_STATE)row["432M"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["432M"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_1296 && 
                (QRVdb.QRV_STATE)row["1_2G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["1_2G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_2320 && 
                (QRVdb.QRV_STATE)row["2_3G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["2_3G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_3400 && 
                (QRVdb.QRV_STATE)row["3_4G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["3_4G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_5760 && 
                (QRVdb.QRV_STATE)row["5_7G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["5_7G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_10368 && 
                (QRVdb.QRV_STATE)row["10G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["10G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_24GHz && 
                (QRVdb.QRV_STATE)row["24G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["24G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_47GHz && 
                (QRVdb.QRV_STATE)row["47G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["47G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            if (Settings.Default.Band_76GHz && 
                (QRVdb.QRV_STATE)row["76G"] != QRVdb.QRV_STATE.worked &&
                (QRVdb.QRV_STATE)row["76G"] != QRVdb.QRV_STATE.not_qrv)
                return false;
            return true;
        }

        private void tsi_Options_Click(object sender, EventArgs e)
        {
            OptionsDlg Dlg = new OptionsDlg();
            Dlg.cbb_KST_Chat.SelectedIndex = 2;
            if (KSTState != MainDlg.KST_STATE.Standby)
            {
                Dlg.tb_KST_Password.Enabled = false;
                Dlg.tb_KST_UserName.Enabled = false;
                Dlg.cbb_KST_Chat.Enabled = false;
                Dlg.tb_KST_Locator.Enabled = false;
                Dlg.tb_KST_Name.Enabled = false;
            }
            if(wtQSO == null)
            {
                int idx = Dlg.tabControl1.TabPages.IndexOf(Dlg.tabPage2);
                if (idx >= 0)
                    Dlg.tabControl1.TabPages.RemoveAt(idx);
            }
            string oldchat = Settings.Default.KST_Chat;
            int KST_MaxDist = Convert.ToInt32(Settings.Default.KST_MaxDist);
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.Save();
                Settings.Default.Reload();
                if (oldchat != Settings.Default.KST_Chat)
                {
                    lock (CALL)
                    {
                        CALL.Clear();
                    }
                    MSG.Clear();
                    latestMessageTimestampSet = false;
                    lv_Calls.Items.Clear();
                    lv_Msg.Items.Clear();
                    lv_MyMsg.Items.Clear();
                }
                UpdateUserBandsWidth();
                set_KST_Status();
                if (KST_MaxDist != Convert.ToInt32(Settings.Default.KST_MaxDist))
                    KST_Update_USR_Window();
            }
        }

        private void tsi_KST_Connect_Click(object sender, EventArgs e)
        {
            KST_Connect();
        }

        private void tsi_KST_Disconnect_Click(object sender, EventArgs e)
        {
            if (Settings.Default.KST_AutoConnect 
                && MessageBox.Show("The KSTAutoConnect function is on. You will be reconnected automatically after 30secs. Do you want to switch this function off?", "KSTAutoConnect", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Settings.Default.KST_AutoConnect = false;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            KST_Disconnect();
        }

        private void tsi_KST_Here_Click(object sender, EventArgs e)
        {
            KST_Here();
        }

        private void tsi_KST_Away_Click(object sender, EventArgs e)
        {
            KST_Away();
        }

        private void btn_KST_Send_Click(object sender, EventArgs e)
        {
            KST_Send();
        }

        private void Check_QSO(DataRow call_row)
        {
            string call = call_row["CALL"].ToString();
            call = call.TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' });
            if (call.IndexOf("-") > 0)
            {
                call = call.Remove(call.IndexOf("-"));
            }
            string wcall = WCCheck.WCCheck.Cut(call);
            string findCall = string.Format("[CALL] LIKE '*{0}*'", wcall);
            DataRow[] selectRow = wtQSO.QSO.Select(findCall);

            bool[] found = new bool[BANDS.Length]; // defaults to false

            foreach (var qso_row in selectRow)
            {
                var band = qso_row["BAND"].ToString();

                if ( WCCheck.WCCheck.Cut(qso_row["CALL"].ToString()).Equals(wcall))
                {
                    call_row[band] = QRVdb.QRV_STATE.worked;
                    qrv.set_qrv_state(wcall, band, QRVdb.QRV_STATE.qrv); // if worked, mark as QRV in data base - FIXME locator...
                    found[Array.IndexOf(BANDS, band)] = true;
                    // check locator
                    if (call_row["LOC"].ToString() != qso_row["LOC"].ToString())
                    {
                        Say(call + " Locator wrong? Win-Test Log " + band + " " + qso_row["TIME"] + " " + call + " " + qso_row["LOC"] + " KST " + call_row["LOC"].ToString());
                        WinTestLocatorWarning = true;
                        Log.WriteMessage("Win-Test log locator mismatch: " + qso_row["BAND"] + " " + qso_row["TIME"] + " " + call + " Locator wrong? Win-Test Log " + qso_row["LOC"] + " KST " + call_row["LOC"].ToString());
                    }
                }
            }
            foreach (string band in BANDS)
            {
                // if marked as worked - but not in the log anymore, just leave it as "qrv"
                if (!found[Array.IndexOf(BANDS, band)] && (QRVdb.QRV_STATE)call_row[band] == QRVdb.QRV_STATE.worked)
                    call_row[band] = QRVdb.QRV_STATE.qrv;
            }

        }

        private void Check_QSOs()
        {
            WinTestLocatorWarning = false;
            try
            {
                lock (CALL)
                {
                    foreach (DataRow call_row in CALL.Rows) // FIXME: CALL muss gelockt werden, während foreach drüber läuft
                    {
                        Check_QSO(call_row);
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + wtQSO.getFileName() + "): " + e.Message);
                MainDlg.Log.WriteMessage(MethodBase.GetCurrentMethod().Name + "(" + wtQSO.getFileName() + "): " + e.Message + "\n" + e.StackTrace);
            }
        }




        private void ti_Main_Tick(object sender, EventArgs e)
        {
            ti_Main.Stop();
            if (KSTState == MainDlg.KST_STATE.Connected)
            {
                if (wtQSO != null)
                {
                    if (Settings.Default.WinTest_Activate)
                    {
                        try
                        {
                            MainDlg.Log.WriteMessage("KST wt Get_QSOs start.");

                            wtQSO.Get_QSOs(Settings.Default.WinTest_INI_FileName);
                            if (WCCheck.WCCheck.IsLoc(wtQSO.MyLoc) > 0 && !wtQSO.MyLoc.Equals(Settings.Default.KST_Loc))
                            {
                                MessageBox.Show("KST locator " + Settings.Default.KST_Loc + " does not match locator in Win-Test " + wtQSO.MyLoc + " !!!", "Win-Test Log",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                set_KST_Status();
                            }
                            Check_QSOs();
                        }
                        catch
                        {

                        }
                    }
                    if (!Settings.Default.WinTest_Activate && wtQSO.QSO.Rows.Count > 0)
                    {
                        wtQSO.Clear_QSOs();
                    }
                }
                KST_Update_USR_Window();
                if (Settings.Default.AS_Active)
                    AS_send_ASWATCHLIST();
            }
            int interval = Convert.ToInt32(Settings.Default.UpdateInterval) * 1000;
            if (interval > 10000)
            {
                ti_Main.Interval = interval;
            }
            ti_Main.Start();
        }

        public void Say(string Text)
        {
            tsl_Info.Text = Text;
            ss_Main.Refresh();
            Application.DoEvents();
        }

        public void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
            tsl_Error.Text = "<" + methodname + "> " + Text;
            ss_Main.Refresh();
            Application.DoEvents();
            if (!ti_Error.Enabled)
            {
                ti_Error.Interval = 30000;
                ti_Error.Start();
            }
        }

        private void MainDlg_Load(object sender, EventArgs e)
        {
            // Set window location
            if (Settings.Default.WindowLocation != null)
            {
                Location = Settings.Default.WindowLocation;
            }

            // Set window size
            if (Settings.Default.WindowSize != null)
            {
                Size = Settings.Default.WindowSize;
            }
            // horizontal splitter between user list and messages
            if (Settings.Default.WindowSplitterDistance1 > 200)
                this.splitContainer1.SplitterDistance = Settings.Default.WindowSplitterDistance1;
            // vertical between messages and mymessages
            if (Settings.Default.WindowSplitterDistance2 > 200)
                this.splitContainer2.SplitterDistance = Settings.Default.WindowSplitterDistance2;
        }

        private void MainDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                bw_GetPlanes.CancelAsync();
                ni_Main.Visible = false; // hide NotificationIcon
                Say("Saving QRV-Database");
                qrv.save_db();
            }
            catch (Exception e2)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(qrv.xml): " + e2.Message);
            }

            // Copy window size to app settings
            if (WindowState == FormWindowState.Normal)
            {
                // Copy window location to app settings
                Settings.Default.WindowLocation = Location;
                Settings.Default.WindowSize = Size;
            }
            else
            {
                Settings.Default.WindowLocation = RestoreBounds.Location;
                Settings.Default.WindowSize = RestoreBounds.Size;
            }

            // horizontal splitter between user list and messages
            Settings.Default.WindowSplitterDistance1 = this.splitContainer1.SplitterDistance;
            // vertical between messages and mymessages
            Settings.Default.WindowSplitterDistance2 = this.splitContainer2.SplitterDistance;

            // Save settings
            Settings.Default.Save();

            MainDlg.Log.WriteMessage("Closed down.");
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            ni_Main.Dispose(); //remove notification icon
        }

        private void ShowToolTip(string text, Control control, Point p)
        {
            int BorderWith = (base.Width - base.ClientSize.Width) / 2;
            int TitleBarHeight = base.Height - base.ClientSize.Height - 2 * BorderWith;
            int TextBlockHeight = TextRenderer.MeasureText(text, control.Font).Height;
            int TextLineHeight = TextRenderer.MeasureText("M", control.Font).Height;
            Point sp = control.PointToScreen(p);
            p = base.PointToClient(sp);
            p.X += BorderWith;
            // check if (in screen coordinates) the text would lie outside or too close to the bottom of the current screen
            if (Screen.FromControl(control).Bounds.Height - (sp.Y + TitleBarHeight + Cursor.Size.Height + TextBlockHeight) > 0)
            {
                p.Y += TitleBarHeight + Cursor.Size.Height;
            }
            else 
            if( p.Y + TitleBarHeight - Cursor.Size.Height + TextLineHeight - TextBlockHeight > 0)// move text above the mouse pointer
            {
                p.Y += TitleBarHeight - Cursor.Size.Height + TextLineHeight - TextBlockHeight;
            }
            else
            {
                p.Y = 0;
            }
            tt_Info.Show(text, this, p, 5000);
        }

        private void lv_Calls_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void lv_Calls_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            if ((e.Header.Text[0] > '0' && e.Header.Text[0] < '9') || e.Header.Text == "AS")
            {
                Rectangle rect = e.Bounds;
                if (e.ColumnIndex > 4 && hide_worked) // Band
                    e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                else
                    e.DrawBackground();
                Font headerfont = new Font(e.Font.OriginalFontName, 6f);
                Size titlesize = TextRenderer.MeasureText(e.Header.Text, headerfont);
                e.Graphics.TranslateTransform(0f, (float)titlesize.Width);
                e.Graphics.RotateTransform(-90f);
                e.Graphics.DrawString(e.Header.Text.ToString(), headerfont, Brushes.Black, new PointF((float)rect.Y, (float)(rect.X + 3)));
                e.Graphics.RotateTransform(90f);
                e.Graphics.TranslateTransform(0f, (float)(-(float)titlesize.Width));
            }
            else
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 2 || e.ColumnIndex == 3)
                {
                    // CALL column
                    if ((e.ColumnIndex == 0 && hide_away) ||   // CALL
                        (e.ColumnIndex == 2 && sort_by_dir) || // LOCATOR
                        (e.ColumnIndex == 3 && ignore_inactive)) // ACT
                        e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                    else
                        e.DrawBackground();
                    e.DrawDefault = false;
                    e.DrawText(TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    return;
                }
                e.DrawDefault = true;
            }
        }

        private void lv_Calls_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Header.Text[0] > '0' && e.Header.Text[0] < '9')
            {
                QRVdb.QRV_STATE state = QRVdb.QRV_STATE.unknown;
                try
                {
                    Enum.TryParse<QRVdb.QRV_STATE>(e.SubItem.Text, out state);
                }
                catch
                {
                }
                switch (state)
                {
                case QRVdb.QRV_STATE.unknown:
                    e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                    break;
                case QRVdb.QRV_STATE.qrv:
                    e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
                    break;
                case QRVdb.QRV_STATE.worked:
                    e.Graphics.FillRectangle(Brushes.Green, e.Bounds);
                    break;
                case QRVdb.QRV_STATE.not_qrv:
                    e.Graphics.FillRectangle(Brushes.Black, e.Bounds);
                    break;
                default:
                    e.DrawDefault = true;
                    break;
                }
            }
            else if (e.Header.Text.StartsWith("AS"))
            {
                try
                {
                    if (e.SubItem.Text.Length > 0)
                    {
                        if (e.SubItem.Text.Equals("<") || e.SubItem.Text.Equals(">"))
                        {
                            e.DrawDefault = true;
                            return;
                        }
                        e.DrawBackground();
                        string[] a = e.SubItem.Text.Split(new char[] { ',' });
                        int pot = Convert.ToInt32(a[0]);
                        if (pot > 0)
                        {
                            if (a.Length < 2)
                                Console.WriteLine("ups " + a.Length + " " + e.SubItem.Text);
                            var cat = a[1];
                            Rectangle b = e.Bounds;
                            if (cat.Equals("S"))
                                b.Inflate(-1, -1);
                            else if (cat.Equals("H"))
                                b.Inflate(-2, -2);
                            else if (cat.Equals("M") || cat.Equals("[unknown]"))
                                b.Inflate(-4, -4);
                            else if (cat.Equals("L"))
                                b.Inflate(-6, -6);
                            else // unknown
                                b.Inflate(-5, -5);
                            int num = pot;
                            if (num == 100)
                            {
                                // 100
                                e.Graphics.FillEllipse(new SolidBrush(Color.Magenta), b);
                            }
                            else if (num > 50)
                            {
                                // 51..99
                                e.Graphics.FillEllipse(new SolidBrush(Color.Red), b);
                            }
                            else
                            {
                                // 0..50
                                e.Graphics.FillEllipse(new SolidBrush(Color.DarkOrange), b);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private void lv_Calls_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_Calls.HitTest(p);
            string ToolTipText = "";
            if (OldMousePos != p && info != null && info.SubItem != null)
            {
                OldMousePos = p;
                if (info.SubItem.Name == "Call" || info.SubItem.Name == "Name" || info.SubItem.Name == "Locator" || info.SubItem.Name == "Act" )
                {
                    ToolTipText = string.Concat(new object[]
                    {
                        "Call:\t",
                        info.Item.Text,
                        "\nName:\t",
                        info.Item.SubItems[1].Text,
                        "\nLoc:\t",
                        info.Item.SubItems[2].Text,
                        "\nQTF:\t",
                        WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, info.Item.SubItems[2].Text).ToString("000"),
                        "°\nQRB:\t",
                        WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, info.Item.SubItems[2].Text),
                        " km\n\nLeft click to\nSend Message."
                    });
                }
                if (info.SubItem.Name == "AS")
                {
                    string call = WCCheck.WCCheck.Cut(info.Item.Text.Replace("(", "").Replace(")", ""));
                    string s = AS_if.GetNearestPlanes(call);
                    if (string.IsNullOrEmpty(s))
                    {
                        ToolTipText = "No planes\n\nLeft click for map";
                        lock (CALL)
                        {
                            DataRow Row = CALL.Rows.Find(call);
                            if (Row != null && Settings.Default.AS_Active)
                            {
                                int qrb = (int)Row["QRB"];
                                if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                                    ToolTipText = "Too close for planes\n\nLeft click for map";
                                else if (qrb > Convert.ToInt32(Settings.Default.AS_MaxDist))
                                    ToolTipText = "Too far away for planes\n\nLeft click for map";
                            }
                        }
                    }
                    else
                    {
                        string t = s.Remove(0, s.IndexOf("\n\n") + 2);
                        t = t.Remove(t.IndexOf("\n"));
                        ToolTipText = t + "\n\nLeft click for map\nRight click for more";
                    }
                }
                if (info.SubItem.Name[0] > '0' && info.SubItem.Name[0] < '9')
                {
                    QRVdb.QRV_STATE state = QRVdb.QRV_STATE.unknown;
                    try
                    {
                        Enum.TryParse<QRVdb.QRV_STATE>(info.SubItem.Text, out state);
                    }
                    catch
                    {
                    }
                    if (state != QRVdb.QRV_STATE.worked)
                    {
                        ToolTipText = info.SubItem.Name.Replace("_", ".") + ": Left click to \ntoggle QRV info";
                    }
                    else
                    {
                        if (wtQSO != null)
                        {
                            string call = info.Item.Text;
                            string band = info.SubItem.Name;
                            DataRow findrow = wtQSO.QSO.Rows.Find(new object[] { call, band.Replace(".", "_") });
                            if (findrow != null)
                            {
                                ToolTipText = string.Concat(new object[]
                                {
                                    band.Replace("_", "."),
                                    ": ",
                                    findrow["CALL"],
                                    "  ",
                                    findrow["TIME"],
                                    "  ",
                                    findrow["SENT"],
                                    "  ",
                                    findrow["RCVD"],
                                    "  ",
                                    findrow["LOC"]
                                });
                            }
                        }
                    }
                }
                ShowToolTip(ToolTipText, lv_Calls, p);
            }
        }

        private void cmi_Calls_SendMessage_Click(object sender, EventArgs e)
        {
            if (lv_Calls.SelectedItems != null && lv_Calls.SelectedItems[0].Text.Length > 0)
            {
                cb_Command.Text = "/cq " + lv_Calls.SelectedItems[0].Text.Replace("(", "").Replace(")", "") + " ";
                cb_Command.Focus();
                cb_Command.SelectionStart = cb_Command.Text.Length;
                cb_Command.SelectionLength = 0;
            }
        }


        private void lv_Calls_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // CALL column
            if (e.Column == 0)
            {
                if (hide_away)
                    hide_away = false;
                else
                    hide_away = true;
                KST_Update_USR_Window();
            }

            // LOCATOR column
            if (e.Column == 2)
            {
                if (sort_by_dir)
                    sort_by_dir = false;
                else
                    sort_by_dir = true;
                KST_Update_USR_Window();
            }

            // ACT column
            if (e.Column == 3)
            {
                if (ignore_inactive)
                    ignore_inactive = false;
                else
                    ignore_inactive = true;
                KST_Update_USR_Window();
            }
            // band columns
            if (e.Column > 4)
            {
                if (hide_worked)
                    hide_worked = false;
                else
                    hide_worked = true;
                lv_Calls.Invalidate(true);
                KST_Update_USR_Window();
            }
        }

        private void lv_Calls_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = new Point(e.X, e.Y);
                QRVdb.QRV_STATE state = QRVdb.QRV_STATE.unknown;
                ListViewHitTestInfo info = lv_Calls.HitTest(p);
                if (info != null && info.SubItem != null)
                {
                    string username = info.Item.Text.Replace("(", "").Replace(")", "");
                    string call = WCCheck.WCCheck.Cut(username);

                    if (info.SubItem.Name == "Call" || info.SubItem.Name == "Name" || info.SubItem.Name == "Locator" || info.SubItem.Name == "Act")
                    {
                        if (username.Length > 0 && KSTState == MainDlg.KST_STATE.Connected)
                        {
                            cb_Command.Text = "/cq " + username + " ";
                            cb_Command.SelectionStart = cb_Command.Text.Length;
                            cb_Command.SelectionLength = 0;
                        }
                    }
                    if (info.SubItem.Name == "AS" && Settings.Default.AS_Active)
                    {
                        string loc = info.Item.SubItems[2].Text;

                        AS_if.show_path(call, loc, MyCall, Settings.Default.KST_Loc);
                    }
                    if (info.SubItem.Name[0] > '0' && info.SubItem.Name[0] < '9')
                    {
                        lock (CALL)
                        {
                            // band columns
                            DataRow CallsRow = CALL.Rows.Find(call);
                            string band = info.SubItem.Name;
                            state = QRVdb.QRV_STATE.unknown;
                            try
                            {
                                Enum.TryParse<QRVdb.QRV_STATE>(info.SubItem.Text, out state);
                            }
                            catch
                            {
                            }
                            switch (state)
                            {
                                case QRVdb.QRV_STATE.unknown:
                                    info.SubItem.Text = QRVdb.QRV_STATE.qrv.ToString();
                                    qrv.set_qrv_state(call, band, QRVdb.QRV_STATE.qrv);
                                    if (CallsRow != null)
                                        CallsRow[band] = QRVdb.QRV_STATE.qrv;
                                    break;
                                case QRVdb.QRV_STATE.qrv:
                                    info.SubItem.Text = QRVdb.QRV_STATE.not_qrv.ToString();
                                    qrv.set_qrv_state(call, band, QRVdb.QRV_STATE.not_qrv);
                                    if (CallsRow != null)
                                        CallsRow[band] = QRVdb.QRV_STATE.not_qrv;
                                    break;
                                case QRVdb.QRV_STATE.not_qrv:
                                    info.SubItem.Text = QRVdb.QRV_STATE.unknown.ToString();
                                    qrv.set_qrv_state(call, band, QRVdb.QRV_STATE.unknown);
                                    if (CallsRow != null)
                                        CallsRow[band] = QRVdb.QRV_STATE.unknown;
                                    break;
                            }
                        }
                        lv_Calls.Refresh();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                string ToolTipText = "";
                ListViewHitTestInfo info = lv_Calls.HitTest(p);
                if (info != null && info.SubItem != null)
                {
                    if (info.SubItem.Name == "Call" /*|| info.SubItem.Name == "Name" || info.SubItem.Name == "Locator" || info.SubItem.Name == "Act"*/)
                    {
                        string call = WCCheck.WCCheck.Cut(info.Item.Text.Replace("(", "").Replace(")", ""));
                        string findCall = string.Format("[CALL] = '{0}' OR [RECIPIENT] = '{0}'", call);
                        DataRow[] selectRow = MSG.Select(findCall);

                        this.cmn_userlist_chatReviewT.Visible = (selectRow.Length > 0);

                        this.cmn_userlist.Show(lv_Calls, p);
                    }
                    else if (info.SubItem.Name == "AS" && Settings.Default.AS_Active)
                    {
                        string call = WCCheck.WCCheck.Cut(info.Item.Text.Replace("(", "").Replace(")", ""));
                        string s = AS_if.GetNearestPlanes(call);
                        if (string.IsNullOrEmpty(s))
                        {
                            ToolTipText = "No planes\n\nLeft click for map";
                            lock (CALL)
                            {
                                DataRow Row = CALL.Rows.Find(call);
                                if (Row != null && Settings.Default.AS_Active)
                                {
                                    int qrb = (int)Row["QRB"];
                                    if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                                        ToolTipText = "Too close for planes\n\nLeft click for map";
                                    else if (qrb > Convert.ToInt32(Settings.Default.AS_MaxDist))
                                        ToolTipText = "Too far away for planes\n\nLeft click for map";
                                }
                            }
                        }
                        else
                        {
                            ToolTipText = s + "\n\nLeft click for map\nRight click for more";
                        }
                    }
                    ShowToolTip(ToolTipText, lv_Calls, p);
                }
            }
        }

        private void lv_Msg_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_Msg.HitTest(p);
            if (info != null && info.Item != null)
            {
                string username = info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "");
                if (username.Equals(MyCall))
                {
                    // if we would be sending to ourselves, use recipient call instead
                    username = info.Item.SubItems[3].Text.Split(new char[] { ' ' })[0].Replace("(", "").Replace(")", "");
                }

                if (username.Length > 0 && KSTState == MainDlg.KST_STATE.Connected)
                {
                    cb_Command.Text = "/cq " + username + " ";
                    cb_Command.SelectionStart = cb_Command.Text.Length;
                    cb_Command.SelectionLength = 0;
                }
            }
        }

        private void lv_MyMsg_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_MyMsg.HitTest(p);
            if (info != null && info.Item != null)
            {
                string username = info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "");
                if (username.Equals(MyCall)) 
                {
                    // if we would be sending to ourselves, use recipient call instead
                    username = info.Item.SubItems[3].Text.Split(new char[] { ' ' })[0].Replace("(", "").Replace(")", "");
                }
                if (username.Length > 0 && KSTState == MainDlg.KST_STATE.Connected)
                {
                    cb_Command.Text = "/cq " + username + " ";
                    cb_Command.SelectionStart = cb_Command.Text.Length;
                    cb_Command.SelectionLength = 0;
                }
            }
        }

        private void lv_Msg_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_Msg.HitTest(p);
            if (OldMousePos != p && info != null && info.SubItem != null)
            {
                OldMousePos = p;
                if (info.SubItem.Name == "Messages")
                {
                    ShowToolTip(info.Item.SubItems[3].Text, lv_Msg, p);
                }
                else
                {
                    ShowToolTip("Left click to\nSend Message.", lv_Msg, p);
                }
            }
        }

        private void lv_MyMsg_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_MyMsg.HitTest(p);
            if (OldMousePos != p && info != null && info.SubItem != null)
            {
                OldMousePos = p;
                if (info.SubItem.Name == "Messages")
                {
                    ShowToolTip(info.Item.SubItems[3].Text, lv_MyMsg, p);
                }
                else
                {
                    ShowToolTip("Left click to\nSend Message.", lv_MyMsg, p);
                }
            }
        }

        private void lv_Msg_Resize(object sender, EventArgs e)
        {
            int colwidth = 0;
            for (int i = 0; i < lv_Msg.Columns.Count - 1; i++)
            {
                colwidth += lv_Msg.Columns[i].Width;
            }
            lv_Msg.Columns[lv_Msg.Columns.Count - 1].Width = lv_Msg.Width - colwidth - 3;
        }

        private void lv_MyMsg_Resize(object sender, EventArgs e)
        {
            int colwidth = 0;
            for (int i = 0; i < lv_MyMsg.Columns.Count - 1; i++)
            {
                colwidth += lv_MyMsg.Columns[i].Width;
            }
            lv_MyMsg.Columns[lv_MyMsg.Columns.Count - 1].Width = lv_MyMsg.Width - colwidth - 3;
        }

        private void cmn_userlist_chatReviewT_Click(object sender, EventArgs e)
        {
            ToolStripItem clickedItem =  sender as ToolStripItem;
            var contextMenu = clickedItem.Owner as ContextMenuStrip;
            var yourControl = contextMenu.SourceControl as DoubleBufferedListView;
            if (yourControl.SelectedItems.Count > 0) // FIXME: geht nur in 1. Spalte (Call)
            {
                string call = yourControl.SelectedItems[0].Text.Replace("(", "").Replace(")", "");
                Console.WriteLine("clicked " + call);

                DataTable chat_review_table = new DataTable("ChatReviewTable");
                chat_review_table.Columns.Add("Time", typeof(DateTime));
                chat_review_table.Columns.Add("Message");

                string findCall = string.Format("[CALL] = '{0}' OR [RECIPIENT] = '{0}'", call);
                DataRow[] selectRow = MSG.Select(findCall);
                foreach (var msg_row in selectRow)
                {
                    Console.WriteLine(msg_row["TIME"].ToString() + " " + msg_row["CALL"].ToString() + " -> " + msg_row["RECIPIENT"].ToString() + " " + msg_row["MSG"].ToString());
                    chat_review_table.Rows.Add(msg_row["TIME"], msg_row["MSG"] );
                }
                ChatReview cr = new ChatReview(chat_review_table, call);
                cr.ShowDialog();
            }

        }

        private void btn_KST_Send_Click_1(object sender, EventArgs e)
        {
            KST_Send();
        }

        private void cmi_Notify_Restore_Click(object sender, EventArgs e)
        {
            base.Activate();
        }

        private void cmi_Notify_Quit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void ni_Main_DoubleClick(object sender, EventArgs e)
        {
            base.Activate();
        }

        private void ni_Main_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                base.Activate();
            }
        }

        private void ni_Main_BalloonTipClicked(object sender, EventArgs e)
        {
            base.Activate();
        }

        private void ti_Receive_Tick(object sender, EventArgs e)
        {
            ti_Receive.Stop();
            DateTime t = DateTime.Now;
            while (true)
            {
                string s;
                lock(MsgQueue)
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

        private void ti_Error_Tick(object sender, EventArgs e)
        {
            tsl_Error.Text = "";
            ti_Error.Stop();
        }

        private void cb_Command_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string LastCommand = cb_Command.Text;
                if (cb_Command.Text.ToUpper().StartsWith("/CQ ") && cb_Command.SelectedItem != null)
                {
                    LastCommand = LastCommand.Split(new char[] { ' ' })[0] + " " + LastCommand.Split(new char[] { ' ' })[1];
                    string NewCommand = (string)cb_Command.SelectedItem;
                    if (NewCommand.ToUpper().StartsWith("/CQ "))
                    {
                        NewCommand = NewCommand.Remove(0, 4);
                        NewCommand = NewCommand.Remove(0, NewCommand.IndexOf(" ") + 1);
                    }
                    NewCommand = LastCommand + " " + NewCommand;
                    if (cb_Command.FindStringExact(NewCommand) != 0)
                    {
                        cb_Command.Items.Insert(0, NewCommand);
                    }
                    cb_Command.SelectedIndex = 0;
                }
            }
            finally
            {
                lv_Calls.Focus();
            }
        }

        private void cb_Command_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!cb_Command.DroppedDown)
            {
                Point p = new Point(e.X, e.Y);
                p = cb_Command.PointToScreen(p);
                Point cp = lv_Calls.PointToClient(p);
                ListViewHitTestInfo info = lv_Calls.HitTest(cp);
                if (info != null && info.Item != null)
                {
                    int newindex = lv_Calls.TopItem.Index - Math.Sign(e.Delta);
                    if (newindex < 0)
                    {
                        newindex = 0;
                    }
                    if (newindex > lv_Calls.Items.Count)
                    {
                        newindex = lv_Calls.Items.Count;
                    }
                    lv_Calls.TopItem = lv_Calls.Items[newindex];
                    ((HandledMouseEventArgs)e).Handled = true;
                }
                else
                {
                    cp = lv_Msg.PointToClient(p);
                    info = lv_Msg.HitTest(cp);
                    if (info != null && info.Item != null)
                    {
                        int newindex = lv_Msg.TopItem.Index - Math.Sign(e.Delta);
                        if (newindex < 0)
                        {
                            newindex = 0;
                        }
                        if (newindex > lv_Msg.Items.Count)
                        {
                            newindex = lv_Msg.Items.Count;
                        }
                        lv_Msg.TopItem = lv_Msg.Items[newindex];
                        ((HandledMouseEventArgs)e).Handled = true;
                    }
                    else
                    {
                        cp = lv_Msg.PointToClient(p);
                        info = lv_MyMsg.HitTest(cp);
                        if (info != null && info.Item != null)
                        {
                            int newindex = lv_MyMsg.TopItem.Index - Math.Sign(e.Delta);
                            if (newindex < 0)
                            {
                                newindex = 0;
                            }
                            if (newindex > lv_MyMsg.Items.Count)
                            {
                                newindex = lv_MyMsg.Items.Count;
                            }
                            lv_MyMsg.TopItem = lv_MyMsg.Items[newindex];
                            ((HandledMouseEventArgs)e).Handled = true;
                        }
                        else
                        {
                            ((HandledMouseEventArgs)e).Handled = true;
                        }
                    }
                }
            }
        }

        private void cb_Command_TextChanged(object sender, EventArgs e)
        {
        }

        private void cb_Command_TextUpdate(object sender, EventArgs e)
        {
        }

        private void ti_Top_Tick(object sender, EventArgs e)
        {
            if (lv_Msg.Items.Count > 0)
            {
                lv_Msg.TopItem = lv_Msg.Items[0];
            }
            ti_Top.Stop();
        }

        private void ti_Reconnect_Tick(object sender, EventArgs e)
        {
            if (Settings.Default.KST_AutoConnect && KSTState == MainDlg.KST_STATE.Standby)
            {
                KST_Connect();
            }
        }

        private void ti_Linkcheck_Tick(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (KSTState == MainDlg.KST_STATE.Connected)
            {
                tw.Send("\r\n"); // send to check if link is still up
            }
        }

        /* send the current list of CALLS to AS server - allows AS to batch process 
         * TODO: better to send only calls that are not away or still needed
         */
        private void AS_send_ASWATCHLIST()
        {
            //return; // FIXME!!! altes AS
            string watchlist = "";
            lock (CALL)
            {
                foreach (DataRow row in CALL.Rows)
                {
                    string dxcall = WCCheck.WCCheck.Cut(row["CALL"].ToString().TrimStart(
                        new char[] { '(' }).TrimEnd(new char[] { ')' }));
                    string dxloc = row["LOC"].ToString();
                    watchlist += string.Concat(new string[] { ",", dxcall, ",", dxloc });
                }
            }
            AS_if.send_watchlist(watchlist, MyCall, Settings.Default.KST_Loc);
        }

        private void bw_GetPlanes_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!bw_GetPlanes.CancellationPending)
            {
                if (KSTState < MainDlg.KST_STATE.Connected || !Settings.Default.AS_Active)
                {
                    Thread.Sleep(200); // TODO: better to use WaitHandles
                    continue;
                }
                int errors = 0;
                //FIXME - lock (CALL) not possible here, as it keeps CALL locked -> deadlock
                for (int i = 0; Settings.Default.AS_Active && i < CALL.Rows.Count; i++)
                {
                    try
                    {
                        int qrb = (int)CALL.Rows[i]["QRB"];
                        string mycall = WCCheck.WCCheck.Cut(MyCall);
                        string dxcall = WCCheck.WCCheck.Cut(CALL.Rows[i]["CALL"].ToString().TrimStart(
                            new char[]{ '(' }).TrimEnd(new char[]{ ')' }));
                        if (mycall.Equals(dxcall))
                            continue;
                        string dxloc = CALL.Rows[i]["LOC"].ToString();
                        if (Settings.Default.AS_Active)
                        {
                            if (qrb >= Convert.ToInt32(Settings.Default.AS_MinDist)
                            && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                            {
                                if (!AS_if.GetPlanes(mycall, Settings.Default.KST_Loc, dxcall, dxloc))
                                {
                                    errors++;
                                    if (errors > 10)
                                    {
                                        bw_GetPlanes.ReportProgress(0, null);
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                if (qrb > Convert.ToInt32(Settings.Default.AS_MaxDist) &&
                                    (qrb < Convert.ToInt32(Settings.Default.KST_MaxDist)))
                                    // too far
                                    bw_GetPlanes.ReportProgress(-2, dxcall);
                                else
                                    // too close
                                    bw_GetPlanes.ReportProgress(-3, dxcall);

                            }
                        }
                        Thread.Sleep(200);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void bw_GetPlanes_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string dxcall = (string)e.UserState;
            if (dxcall == null)
            {
                if (e.ProgressPercentage == 0)
                {
                    try
                    {
                        AS_if.planes.Clear();
                        foreach (ListViewItem lvi in lv_Calls.Items)
                        {
                            lvi.SubItems[4].Text = "";
                        }
                        lv_Calls.Refresh();
                    }
                    catch
                    {
                    }
                }
                return;
            }
            // search the listview for matching call - note that dxcall is the bare callsign, whereas
            // the list contains () for users that are away and may contain things like /p
            // so this is safer...
            ListViewItem call_lvi = null;
            foreach (ListViewItem lvi in lv_Calls.Items)
            {
                if (lvi.Text.IndexOf(dxcall) >= 0)
                {
                    call_lvi = lvi;
                    break;
                }
            }

            if (e.ProgressPercentage > 0)
            {
                if (call_lvi != null)
                {
                    string newtext = AS_if.GetNearestPlanePotential(dxcall);
                    if (call_lvi.SubItems[4].Text != newtext)
                    {
                        call_lvi.SubItems[4].Text = newtext;
                        lv_Calls.Refresh();
                    }
                }
            }
            else if (e.ProgressPercentage == -2) // too far
            {
                AS_if.planes.Remove(dxcall);
                if (call_lvi != null)
                {
                    string newtext = ">";
                    if (call_lvi.SubItems[4].Text != newtext)
                    {
                        call_lvi.SubItems[4].Text = newtext;
                        lv_Calls.Refresh();
                    }
                }
            }
            else if (e.ProgressPercentage == -3) // too close
            {
                AS_if.planes.Remove(dxcall);
                if (call_lvi != null)
                {
                    string newtext = "<";
                    if (call_lvi.SubItems[4].Text != newtext)
                    {
                        call_lvi.SubItems[4].Text = newtext;
                        lv_Calls.Refresh();
                    }
                }
            }
            else /* e.ProgressPercentage == 0 or e.ProgressPercentage == -1 */
            {
                Console.WriteLine("remove " + dxcall);
                AS_if.planes.Remove(dxcall);
                if (call_lvi != null)
                {
                    string newtext = "";
                    if (call_lvi.SubItems[4].Text != newtext)
                    {
                        call_lvi.SubItems[4].Text = newtext;
                        lv_Calls.Refresh();
                    }
                }
            }
        }

        private void bw_GetPlanes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        class DoubleBufferedListView : ListView
        {
            public DoubleBufferedListView()
            {
                this.DoubleBuffered = true;
            }
        }

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDlg));
            this.ss_Main = new System.Windows.Forms.StatusStrip();
            this.tsl_Info = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsl_Error = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.cb_Command = new System.Windows.Forms.ComboBox();
            this.btn_KST_Send = new System.Windows.Forms.Button();
            this.lbl_KST_Status = new System.Windows.Forms.Label();
            this.lbl_Call = new System.Windows.Forms.Label();
            this.lv_Msg = new System.Windows.Forms.ListView();
            this.lvh_Time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvh_Call = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvh_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvh_Msg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbl_KST_Msg = new System.Windows.Forms.Label();
            this.lv_MyMsg = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbl_KST_MyMsg = new System.Windows.Forms.Label();
            this.lv_Calls = new wtKST.MainDlg.DoubleBufferedListView();
            this.ch_Call = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Loc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_Act = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ch_AS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader144 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader432 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1296 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2320 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3400 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5760 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10368 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader24GHz = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader47GHz = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader76GHz = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lbl_KST_Calls = new System.Windows.Forms.Label();
            this.mn_Main = new System.Windows.Forms.MenuStrip();
            this.tsm_File = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsi_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_KST = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_KST_Connect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_KST_Disconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsi_KST_Here = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_KST_Away = new System.Windows.Forms.ToolStripMenuItem();
            this.tsi_Options = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ti_Main = new System.Windows.Forms.Timer(this.components);
            this.ni_Main = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmn_Notify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmi_Notify_Restore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmi_Notify_Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.ti_Receive = new System.Windows.Forms.Timer(this.components);
            this.ti_Error = new System.Windows.Forms.Timer(this.components);
            this.ti_Top = new System.Windows.Forms.Timer(this.components);
            this.ti_Reconnect = new System.Windows.Forms.Timer(this.components);
            this.ti_Linkcheck = new System.Timers.Timer();
            this.tt_Info = new System.Windows.Forms.ToolTip(this.components);
            this.bw_GetPlanes = new System.ComponentModel.BackgroundWorker();
            this.cmn_userlist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmn_userlist_chatReviewT = new System.Windows.Forms.ToolStripMenuItem();
            this.ss_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.mn_Main.SuspendLayout();
            this.cmn_Notify.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ti_Linkcheck)).BeginInit();
            this.cmn_userlist.SuspendLayout();
            this.SuspendLayout();
            // 
            // ss_Main
            // 
            this.ss_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsl_Info,
            this.tsl_Error});
            this.ss_Main.Location = new System.Drawing.Point(0, 708);
            this.ss_Main.Name = "ss_Main";
            this.ss_Main.Size = new System.Drawing.Size(1202, 22);
            this.ss_Main.TabIndex = 9;
            this.ss_Main.Text = "statusStrip1";
            // 
            // tsl_Info
            // 
            this.tsl_Info.Name = "tsl_Info";
            this.tsl_Info.Size = new System.Drawing.Size(28, 17);
            this.tsl_Info.Text = "Info";
            // 
            // tsl_Error
            // 
            this.tsl_Error.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsl_Error.ForeColor = System.Drawing.Color.Red;
            this.tsl_Error.Name = "tsl_Error";
            this.tsl_Error.Size = new System.Drawing.Size(1159, 17);
            this.tsl_Error.Spring = true;
            this.tsl_Error.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lv_Calls);
            this.splitContainer1.Panel2.Controls.Add(this.lbl_KST_Calls);
            this.splitContainer1.Size = new System.Drawing.Size(1202, 684);
            this.splitContainer1.SplitterDistance = 843;
            this.splitContainer1.TabIndex = 10;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            this.splitContainer2.Panel1MinSize = 75;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lv_MyMsg);
            this.splitContainer2.Panel2.Controls.Add(this.lbl_KST_MyMsg);
            this.splitContainer2.Panel2MinSize = 75;
            this.splitContainer2.Size = new System.Drawing.Size(843, 684);
            this.splitContainer2.SplitterDistance = 346;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.cb_Command);
            this.splitContainer3.Panel1.Controls.Add(this.btn_KST_Send);
            this.splitContainer3.Panel1.Controls.Add(this.lbl_KST_Status);
            this.splitContainer3.Panel1.Controls.Add(this.lbl_Call);
            this.splitContainer3.Panel1MinSize = 80;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.lv_Msg);
            this.splitContainer3.Panel2.Controls.Add(this.lbl_KST_Msg);
            this.splitContainer3.Size = new System.Drawing.Size(843, 346);
            this.splitContainer3.SplitterDistance = 80;
            this.splitContainer3.TabIndex = 0;
            // 
            // cb_Command
            // 
            this.cb_Command.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_Command.FormattingEnabled = true;
            this.cb_Command.Location = new System.Drawing.Point(11, 41);
            this.cb_Command.MaxDropDownItems = 5;
            this.cb_Command.Name = "cb_Command";
            this.cb_Command.Size = new System.Drawing.Size(469, 24);
            this.cb_Command.TabIndex = 17;
            this.cb_Command.TextUpdate += new System.EventHandler(this.cb_Command_TextUpdate);
            this.cb_Command.DropDownClosed += new System.EventHandler(this.cb_Command_DropDownClosed);
            this.cb_Command.TextChanged += new System.EventHandler(this.cb_Command_TextChanged);
            // 
            // btn_KST_Send
            // 
            this.btn_KST_Send.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_KST_Send.Location = new System.Drawing.Point(508, 41);
            this.btn_KST_Send.Name = "btn_KST_Send";
            this.btn_KST_Send.Size = new System.Drawing.Size(75, 23);
            this.btn_KST_Send.TabIndex = 16;
            this.btn_KST_Send.Text = "Send";
            this.btn_KST_Send.UseVisualStyleBackColor = true;
            this.btn_KST_Send.Click += new System.EventHandler(this.btn_KST_Send_Click_1);
            // 
            // lbl_KST_Status
            // 
            this.lbl_KST_Status.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lbl_KST_Status.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_KST_Status.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbl_KST_Status.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_KST_Status.Location = new System.Drawing.Point(0, 0);
            this.lbl_KST_Status.Name = "lbl_KST_Status";
            this.lbl_KST_Status.Padding = new System.Windows.Forms.Padding(4);
            this.lbl_KST_Status.Size = new System.Drawing.Size(841, 26);
            this.lbl_KST_Status.TabIndex = 15;
            this.lbl_KST_Status.Text = "Status";
            this.lbl_KST_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Call
            // 
            this.lbl_Call.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Call.Location = new System.Drawing.Point(589, 41);
            this.lbl_Call.Name = "lbl_Call";
            this.lbl_Call.Size = new System.Drawing.Size(100, 23);
            this.lbl_Call.TabIndex = 14;
            this.lbl_Call.Text = "Call";
            this.lbl_Call.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lv_Msg
            // 
            this.lv_Msg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvh_Time,
            this.lvh_Call,
            this.lvh_Name,
            this.lvh_Msg});
            this.lv_Msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_Msg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_Msg.FullRowSelect = true;
            this.lv_Msg.GridLines = true;
            this.lv_Msg.Location = new System.Drawing.Point(0, 26);
            this.lv_Msg.MultiSelect = false;
            this.lv_Msg.Name = "lv_Msg";
            this.lv_Msg.Size = new System.Drawing.Size(841, 234);
            this.lv_Msg.TabIndex = 12;
            this.lv_Msg.UseCompatibleStateImageBehavior = false;
            this.lv_Msg.View = System.Windows.Forms.View.Details;
            this.lv_Msg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_Msg_MouseDown);
            this.lv_Msg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lv_Msg_MouseMove);
            this.lv_Msg.Resize += new System.EventHandler(this.lv_Msg_Resize);
            // 
            // lvh_Time
            // 
            this.lvh_Time.Text = "Time";
            this.lvh_Time.Width = 50;
            // 
            // lvh_Call
            // 
            this.lvh_Call.Text = "Call";
            this.lvh_Call.Width = 100;
            // 
            // lvh_Name
            // 
            this.lvh_Name.Text = "Name";
            this.lvh_Name.Width = 150;
            // 
            // lvh_Msg
            // 
            this.lvh_Msg.Text = "Messages";
            this.lvh_Msg.Width = 600;
            // 
            // lbl_KST_Msg
            // 
            this.lbl_KST_Msg.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lbl_KST_Msg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_KST_Msg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbl_KST_Msg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_KST_Msg.Location = new System.Drawing.Point(0, 0);
            this.lbl_KST_Msg.Name = "lbl_KST_Msg";
            this.lbl_KST_Msg.Padding = new System.Windows.Forms.Padding(4);
            this.lbl_KST_Msg.Size = new System.Drawing.Size(841, 26);
            this.lbl_KST_Msg.TabIndex = 11;
            this.lbl_KST_Msg.Text = "Messages";
            this.lbl_KST_Msg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lv_MyMsg
            // 
            this.lv_MyMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lv_MyMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_MyMsg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_MyMsg.FullRowSelect = true;
            this.lv_MyMsg.GridLines = true;
            this.lv_MyMsg.Location = new System.Drawing.Point(0, 26);
            this.lv_MyMsg.MultiSelect = false;
            this.lv_MyMsg.Name = "lv_MyMsg";
            this.lv_MyMsg.Size = new System.Drawing.Size(841, 306);
            this.lv_MyMsg.TabIndex = 13;
            this.lv_MyMsg.UseCompatibleStateImageBehavior = false;
            this.lv_MyMsg.View = System.Windows.Forms.View.Details;
            this.lv_MyMsg.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_MyMsg_MouseDown);
            this.lv_MyMsg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lv_MyMsg_MouseMove);
            this.lv_MyMsg.Resize += new System.EventHandler(this.lv_MyMsg_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Time";
            this.columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Call";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Messages";
            this.columnHeader4.Width = 600;
            // 
            // lbl_KST_MyMsg
            // 
            this.lbl_KST_MyMsg.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.lbl_KST_MyMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_KST_MyMsg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbl_KST_MyMsg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_KST_MyMsg.Location = new System.Drawing.Point(0, 0);
            this.lbl_KST_MyMsg.Name = "lbl_KST_MyMsg";
            this.lbl_KST_MyMsg.Padding = new System.Windows.Forms.Padding(4);
            this.lbl_KST_MyMsg.Size = new System.Drawing.Size(841, 26);
            this.lbl_KST_MyMsg.TabIndex = 10;
            this.lbl_KST_MyMsg.Text = "My Messages";
            this.lbl_KST_MyMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lv_Calls
            // 
            this.lv_Calls.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ch_Call,
            this.ch_Name,
            this.ch_Loc,
            this.ch_Act,
            this.ch_AS,
            this.columnHeader144,
            this.columnHeader432,
            this.columnHeader1296,
            this.columnHeader2320,
            this.columnHeader3400,
            this.columnHeader5760,
            this.columnHeader10368,
            this.columnHeader24GHz,
            this.columnHeader47GHz,
            this.columnHeader76GHz});
            this.lv_Calls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_Calls.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_Calls.GridLines = true;
            this.lv_Calls.Location = new System.Drawing.Point(0, 24);
            this.lv_Calls.MultiSelect = false;
            this.lv_Calls.Name = "lv_Calls";
            this.lv_Calls.OwnerDraw = true;
            this.lv_Calls.Size = new System.Drawing.Size(353, 658);
            this.lv_Calls.TabIndex = 14;
            this.lv_Calls.UseCompatibleStateImageBehavior = false;
            this.lv_Calls.View = System.Windows.Forms.View.Details;
            this.lv_Calls.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lv_Calls_ColumnClick);
            this.lv_Calls.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lv_Calls_DrawColumnHeader);
            this.lv_Calls.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lv_Calls_DrawItem);
            this.lv_Calls.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lv_Calls_DrawSubItem);
            this.lv_Calls.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lv_Calls_MouseDown);
            this.lv_Calls.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lv_Calls_MouseMove);
            // 
            // ch_Call
            // 
            this.ch_Call.Text = "Call";
            this.ch_Call.Width = 80;
            // 
            // ch_Name
            // 
            this.ch_Name.Text = "Name";
            this.ch_Name.Width = 100;
            // 
            // ch_Loc
            // 
            this.ch_Loc.Text = "Locator";
            // 
            // ch_Act
            // 
            this.ch_Act.Text = "Act";
            this.ch_Act.Width = 30;
            // 
            // ch_AS
            // 
            this.ch_AS.Text = "AS";
            this.ch_AS.Width = 20;
            // 
            // columnHeader144
            // 
            this.columnHeader144.Text = "144M";
            this.columnHeader144.Width = 20;
            // 
            // columnHeader432
            // 
            this.columnHeader432.Text = "432M";
            this.columnHeader432.Width = 20;
            // 
            // columnHeader1296
            // 
            this.columnHeader1296.Text = "1.2G";
            this.columnHeader1296.Width = 20;
            // 
            // columnHeader2320
            // 
            this.columnHeader2320.Text = "2.3G";
            this.columnHeader2320.Width = 20;
            // 
            // columnHeader3400
            // 
            this.columnHeader3400.Text = "3.4G";
            this.columnHeader3400.Width = 20;
            // 
            // columnHeader5760
            // 
            this.columnHeader5760.Text = "5.7G";
            this.columnHeader5760.Width = 20;
            // 
            // columnHeader10368
            // 
            this.columnHeader10368.Text = "10G";
            this.columnHeader10368.Width = 20;
            // 
            // columnHeader24GHz
            // 
            this.columnHeader24GHz.Text = "24G";
            this.columnHeader24GHz.Width = 20;
            // 
            // columnHeader47GHz
            // 
            this.columnHeader47GHz.Text = "47G";
            this.columnHeader47GHz.Width = 20;
            // 
            // columnHeader76GHz
            // 
            this.columnHeader76GHz.Text = "76G";
            this.columnHeader76GHz.Width = 20;
            // 
            // lbl_KST_Calls
            // 
            this.lbl_KST_Calls.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.lbl_KST_Calls.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl_KST_Calls.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbl_KST_Calls.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_KST_Calls.Location = new System.Drawing.Point(0, 0);
            this.lbl_KST_Calls.Name = "lbl_KST_Calls";
            this.lbl_KST_Calls.Padding = new System.Windows.Forms.Padding(4);
            this.lbl_KST_Calls.Size = new System.Drawing.Size(353, 24);
            this.lbl_KST_Calls.TabIndex = 0;
            this.lbl_KST_Calls.Text = "Calls";
            this.lbl_KST_Calls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbl_KST_Calls.UseMnemonic = false;
            // 
            // mn_Main
            // 
            this.mn_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_File,
            this.tsm_KST,
            this.tsi_Options,
            this.toolStripMenuItem1});
            this.mn_Main.Location = new System.Drawing.Point(0, 0);
            this.mn_Main.Name = "mn_Main";
            this.mn_Main.Size = new System.Drawing.Size(1202, 24);
            this.mn_Main.TabIndex = 11;
            this.mn_Main.Text = "menuStrip1";
            // 
            // tsm_File
            // 
            this.tsm_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.tsi_File_Exit});
            this.tsm_File.Name = "tsm_File";
            this.tsm_File.Size = new System.Drawing.Size(37, 20);
            this.tsm_File.Text = "&File";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(89, 6);
            // 
            // tsi_File_Exit
            // 
            this.tsi_File_Exit.Name = "tsi_File_Exit";
            this.tsi_File_Exit.Size = new System.Drawing.Size(92, 22);
            this.tsi_File_Exit.Text = "E&xit";
            this.tsi_File_Exit.Click += new System.EventHandler(this.tsi_File_Exit_Click);
            // 
            // tsm_KST
            // 
            this.tsm_KST.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsi_KST_Connect,
            this.tsi_KST_Disconnect,
            this.toolStripSeparator2,
            this.tsi_KST_Here,
            this.tsi_KST_Away});
            this.tsm_KST.Name = "tsm_KST";
            this.tsm_KST.Size = new System.Drawing.Size(39, 20);
            this.tsm_KST.Text = "&KST";
            // 
            // tsi_KST_Connect
            // 
            this.tsi_KST_Connect.Name = "tsi_KST_Connect";
            this.tsi_KST_Connect.Size = new System.Drawing.Size(160, 22);
            this.tsi_KST_Connect.Text = "&Connect";
            this.tsi_KST_Connect.Click += new System.EventHandler(this.tsi_KST_Connect_Click);
            // 
            // tsi_KST_Disconnect
            // 
            this.tsi_KST_Disconnect.Enabled = false;
            this.tsi_KST_Disconnect.Name = "tsi_KST_Disconnect";
            this.tsi_KST_Disconnect.Size = new System.Drawing.Size(160, 22);
            this.tsi_KST_Disconnect.Text = "&Disconnect";
            this.tsi_KST_Disconnect.Click += new System.EventHandler(this.tsi_KST_Disconnect_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // tsi_KST_Here
            // 
            this.tsi_KST_Here.Name = "tsi_KST_Here";
            this.tsi_KST_Here.Size = new System.Drawing.Size(160, 22);
            this.tsi_KST_Here.Text = "&I am on the chat";
            this.tsi_KST_Here.Click += new System.EventHandler(this.tsi_KST_Here_Click);
            // 
            // tsi_KST_Away
            // 
            this.tsi_KST_Away.Name = "tsi_KST_Away";
            this.tsi_KST_Away.Size = new System.Drawing.Size(160, 22);
            this.tsi_KST_Away.Text = "I am &away";
            this.tsi_KST_Away.Click += new System.EventHandler(this.tsi_KST_Away_Click);
            // 
            // tsi_Options
            // 
            this.tsi_Options.Name = "tsi_Options";
            this.tsi_Options.Size = new System.Drawing.Size(61, 20);
            this.tsi_Options.Text = "&Options";
            this.tsi_Options.Click += new System.EventHandler(this.tsi_Options_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "&?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ti_Main
            // 
            this.ti_Main.Enabled = true;
            this.ti_Main.Interval = 30000;
            this.ti_Main.Tick += new System.EventHandler(this.ti_Main_Tick);
            // 
            // ni_Main
            // 
            this.ni_Main.ContextMenuStrip = this.cmn_Notify;
            this.ni_Main.Icon = ((System.Drawing.Icon)(resources.GetObject("ni_Main.Icon")));
            this.ni_Main.Text = "wtKST";
            this.ni_Main.Visible = true;
            this.ni_Main.BalloonTipClicked += new System.EventHandler(this.ni_Main_BalloonTipClicked);
            this.ni_Main.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ni_Main_MouseClick);
            // 
            // cmn_Notify
            // 
            this.cmn_Notify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmi_Notify_Restore,
            this.toolStripSeparator3,
            this.cmi_Notify_Quit});
            this.cmn_Notify.Name = "cmn_Notify";
            this.cmn_Notify.Size = new System.Drawing.Size(119, 54);
            // 
            // cmi_Notify_Restore
            // 
            this.cmi_Notify_Restore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmi_Notify_Restore.Name = "cmi_Notify_Restore";
            this.cmi_Notify_Restore.Size = new System.Drawing.Size(118, 22);
            this.cmi_Notify_Restore.Text = "&Restore";
            this.cmi_Notify_Restore.Click += new System.EventHandler(this.cmi_Notify_Restore_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(115, 6);
            // 
            // cmi_Notify_Quit
            // 
            this.cmi_Notify_Quit.Name = "cmi_Notify_Quit";
            this.cmi_Notify_Quit.Size = new System.Drawing.Size(118, 22);
            this.cmi_Notify_Quit.Text = "&Quit";
            this.cmi_Notify_Quit.Click += new System.EventHandler(this.cmi_Notify_Quit_Click);
            // 
            // ti_Receive
            // 
            this.ti_Receive.Enabled = true;
            this.ti_Receive.Interval = 1000;
            this.ti_Receive.Tick += new System.EventHandler(this.ti_Receive_Tick);
            // 
            // ti_Error
            // 
            this.ti_Error.Interval = 10000;
            this.ti_Error.Tick += new System.EventHandler(this.ti_Error_Tick);
            // 
            // ti_Top
            // 
            this.ti_Top.Interval = 60000;
            this.ti_Top.Tick += new System.EventHandler(this.ti_Top_Tick);
            // 
            // ti_Reconnect
            // 
            this.ti_Reconnect.Interval = 30000;
            this.ti_Reconnect.Tick += new System.EventHandler(this.ti_Reconnect_Tick);
            // 
            // ti_Linkcheck
            // 
            this.ti_Linkcheck.Interval = 120000D;
            this.ti_Linkcheck.SynchronizingObject = this;
            // 
            // tt_Info
            // 
            this.tt_Info.ShowAlways = true;
            // 
            // bw_GetPlanes
            // 
            this.bw_GetPlanes.WorkerReportsProgress = true;
            this.bw_GetPlanes.WorkerSupportsCancellation = true;
            this.bw_GetPlanes.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bw_GetPlanes_DoWork);
            this.bw_GetPlanes.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bw_GetPlanes_ProgressChanged);
            this.bw_GetPlanes.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bw_GetPlanes_RunWorkerCompleted);
            // 
            // cmn_userlist
            // 
            this.cmn_userlist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmn_userlist_chatReviewT});
            this.cmn_userlist.Name = "cmn_userlist";
            this.cmn_userlist.Size = new System.Drawing.Size(150, 92);
            // 
            // cmn_userlist_chatReviewT
            // 
            this.cmn_userlist_chatReviewT.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cmn_userlist_chatReviewT.Name = "cmn_userlist_chatReviewT";
            this.cmn_userlist_chatReviewT.Size = new System.Drawing.Size(149, 22);
            this.cmn_userlist_chatReviewT.Text = "Chat &Review";
            this.cmn_userlist_chatReviewT.Click += new System.EventHandler(this.cmn_userlist_chatReviewT_Click);
            // 
            // MainDlg
            // 
            this.AcceptButton = this.btn_KST_Send;
            this.ClientSize = new System.Drawing.Size(1202, 730);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ss_Main);
            this.Controls.Add(this.mn_Main);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::wtKST.Properties.Settings.Default, "WindowLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::wtKST.Properties.Settings.Default.WindowLocation;
            this.Name = "MainDlg";
            this.Text = "wtKST";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainDlg_FormClosing);
            this.Load += new System.EventHandler(this.MainDlg_Load);
            this.ss_Main.ResumeLayout(false);
            this.ss_Main.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.mn_Main.ResumeLayout(false);
            this.mn_Main.PerformLayout();
            this.cmn_Notify.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ti_Linkcheck)).EndInit();
            this.cmn_userlist.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox1.ShowDialog();
        }
    }
}
