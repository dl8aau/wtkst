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
            Connected = 128,
        }

        public enum USER_STATE
        {
            Here,
            Away
        }

        private enum QRV_STATE : int
        {
            unknown = 0, qrv = 1, worked = 2, not_qrv = 3
        }

        private static readonly string[] BANDS = new string[] { "144M", "432M", "1_2G", "2_3G", "3_4G", "5_7G", "10G", "24G", "47G", "76G" };
        public DataTable MSG = new DataTable("MSG");

        public DataTable MYMSG = new DataTable("MYMSG");

        public DataTable QRV = new DataTable("QRV");

        public DataTable CALL = new DataTable("CALL");

        private WinTest.WinTestLog wtQSO = null;

        private wtKST.AirScoutInterface AS_if = new wtKST.AirScoutInterface();

        private bool DLLNotLoaded = false;

        private Point OldMousePos = new Point(0, 0);

        public static LogWriter Log = new LogWriter(Directory.GetParent(Application.LocalUserAppDataPath).ToString());

        private TelnetWrapper tw;

        private string KSTBuffer = "";

        private Queue<string> MsgQueue = new Queue<string>();

        private string MyCall = "DL2ALF";

        private string MyLoc = "JO50IW";

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

        private System.Windows.Forms.Timer ti_Main;

        private ListView lv_Calls;

        private ColumnHeader ch_Call;

        private ColumnHeader ch_Name;

        private ColumnHeader ch_Act;

        private ColumnHeader ch_Loc;

        private ColumnHeader columnHeader144;

        private ColumnHeader columnHeader432;

        private ImageList il_Calls;

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

        public ImageList il_Planes;

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
            QRV.Columns.Add("CALL");
            QRV.Columns.Add("TIME", typeof(DateTime));
            foreach (string band in BANDS)
                QRV.Columns.Add(band, typeof(int));
            DataColumn[] QRVkeys = new DataColumn[]
            {
                QRV.Columns["CALL"]
            };
            QRV.PrimaryKey = QRVkeys;
            InitializeQRV(false);

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
            DataColumn[] CALLkeys = new DataColumn[]
            {
                CALL.Columns["CALL"]
            };
            CALL.PrimaryKey = CALLkeys;
            // FIXME try to load wtlog dll...
            // only temporary solution... (checked only at program start...)
            string kstcall = WCCheck.WCCheck.Cut(Settings.Default.KST_UserName.ToUpper());
            if (kstcall.IndexOf("DL0GTH") >= 0 || kstcall.IndexOf("DL2ALF") >= 0 || kstcall.IndexOf("DL2ARD") >= 0 ||
                kstcall.IndexOf("DL2AKT") >= 0 || kstcall.IndexOf("DM5CT") >= 0 || kstcall.IndexOf("DL6AUI") >= 0 ||
                kstcall.IndexOf("DR9A") >= 0 || kstcall.IndexOf("DA0FF") >= 0 ||
                kstcall.IndexOf("DL8AAU") >= 0 || kstcall.IndexOf("DL0FTZ") >= 0)
            {
                wtQSO = new WinTestLog(MainDlg.Log.WriteMessage);
            }
            UpdateUserBandsWidth();
            bw_GetPlanes.RunWorkerAsync();
            if (Settings.Default.KST_AutoConnect)
            {
                KST_Connect();
            }
        }

        private void InitializeQRV(bool ForceReload)
        {
            QRV.Clear();
            try
            {
                string QRV_Table_Filename = Path.Combine(Application.LocalUserAppDataPath, Settings.Default.WinTest_QRV_Table_FileName);
                TimeSpan ts = DateTime.Now - File.GetLastWriteTime(QRV_Table_Filename);
                if (!ForceReload && File.Exists(QRV_Table_Filename) && ts.Hours < 48)
                {
                    try
                    {
                        QRV.BeginLoadData();
                        QRV.ReadXml(QRV_Table_Filename);
                        QRV.EndLoadData();
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml): " + e.Message);
                    }
                }
                // if we cannot read qrv.xml from appdata path, try current directoy (=previous default)
                if (QRV.Rows.Count == 0 && !ForceReload && File.Exists(Settings.Default.WinTest_QRV_Table_FileName))
                {
                    try
                    {
                        QRV.BeginLoadData();
                        QRV.ReadXml(Settings.Default.WinTest_QRV_Table_FileName);
                        QRV.EndLoadData();
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml): " + e.Message);
                    }
                }
                if (QRV.Rows.Count == 0)
                {
                    using (StreamReader sr = new StreamReader(Settings.Default.WinTest_QRV_FileName))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            DataRow Row = QRV.NewRow();
                            Row["CALL"] = line.Split(new char[0])[0];
                            Row["TIME"] = DateTime.Parse(line.Split(new char[0])[1].TrimStart(
                                new char[]{ '[' }).TrimEnd(new char[]{ ']' }) + " 00:00:00");
                            foreach (string band in BANDS)
                            {
                                if (line.IndexOf(band) > 0)
                                {
                                    Row[band] = QRV_STATE.qrv;
                                }
                                else
                                {
                                    Row[band] = QRV_STATE.unknown;
                                }
                            }
                            QRV.Rows.Add(Row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + Settings.Default.WinTest_QRV_FileName + "): " + e.Message);
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
            KSTState = MainDlg.KST_STATE.Disconnected;
        }

        private void set_KST_Status()
        {
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
                MyLoc,
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
                if (tw != null && !tw.Connected && KSTState != MainDlg.KST_STATE.Standby)
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
                CALL.Clear();
                lv_Calls.Items.Clear();
                tsi_KST_Connect.Enabled = true;
                tsi_KST_Disconnect.Enabled = false;
                tsi_KST_Here.Enabled = false;
                tsi_KST_Away.Enabled = false;
                cb_Command.Enabled = false;
                btn_KST_Send.Enabled = false;
                lbl_Call.Enabled = false;
                ti_Main.Stop();
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
            if (!cb_Command.IsDisposed && !cb_Command.Focused && !btn_KST_Send.Capture)
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
                                    Say("Password wrong.");
                                    tw.Close();
                                    MainDlg.Log.WriteMessage("Password wrong ");
                                    break;
                                }
                                Settings.Default.KST_Name = subs[6];
                                Settings.Default.KST_Loc = subs[8];
                                if (WCCheck.WCCheck.IsLoc(Settings.Default.KST_Loc) > 0)
                                {
                                    MyLoc = Settings.Default.KST_Loc;
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

                        ti_Linkcheck.Stop();   // restart the linkcheck timer
                        ti_Linkcheck.Start();
                    }
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
                            Row["MSG"] = msg[6].Trim();
                        else
                            Row["MSG"] = "(" + recipient + ") " + msg[6].Trim();
                        Row["RECIPIENT"] = recipient;
                        msg_latest_first = (s.Substring(0, 2)).Equals("CR");
                        KST_Process_new_message(Row);
                        break;

                        // CK: link check
                        // CK |

                    case "CK": // Link Check
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
            }
        }

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

                DataRow findrow = QRV.Rows.Find(Row["CALL"].ToString().Trim());
                // TODO: what if user is not in user list yet? time not updated then
                // -> count appearing on user list as "activity", too
                if (findrow != null)
                {
                    if ((DateTime)findrow["TIME"] < dt)
                        findrow["TIME"] = Row["TIME"];
                }
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
                    lv_Msg.Items[current_index].BackColor = Color.FromArgb(16777200);
                else
                    lv_Msg.Items[current_index].BackColor = Color.FromArgb(11516905);

                if (Row["RECIPIENT"].ToString().Equals(MyCall))
                {
                    lv_Msg.Items[current_index].BackColor = Color.FromArgb(16745026);
                }
                // check the recipient of the message
                findrow = CALL.Rows.Find(Row["RECIPIENT"].ToString());
                if (findrow != null)
                {
                    findrow["CONTACTED"] = (int)findrow["CONTACTED"] + 1;
                }
                findrow = CALL.Rows.Find(Row["CALL"].ToString().Trim());
                if (findrow != null)
                {
                    findrow["CONTACTED"] = 0; // clear counter on activity
                }
                if (Row["MSG"].ToString().ToUpper().StartsWith("(" + MyCall + ")") || Row["MSG"].ToString().ToUpper().StartsWith(MyCall))
                {
                    ListViewItem MyLV = new ListViewItem();
                    if (Row["MSG"].ToString().ToUpper().StartsWith("(" + MyCall + ")"))
                    {
                        MyLV.BackColor = Color.BlanchedAlmond;
                    }
                    else
                    {
                        MyLV.BackColor = Color.Cornsilk;
                    }
                    MyLV.Text = ((DateTime)Row["TIME"]).ToString("HH:mm");
                    MyLV.SubItems.Add(Row["CALL"].ToString());
                    MyLV.SubItems.Add(Row["NAME"].ToString());
                    MyLV.SubItems.Add(Row["MSG"].ToString());
                    for (int i = 0; i < MyLV.SubItems.Count; i++)
                    {
                        MyLV.SubItems[i].Name = lv_MyMsg.Columns[i].Text;
                    }
                    lv_MyMsg.Items.Insert(0, MyLV);
                    lv_MyMsg.Items[0].EnsureVisible();
                    int hwnd = MainDlg.GetForegroundWindow();
                    if (hwnd != base.Handle.ToInt32())
                    {
                        if (Settings.Default.KST_ShowBalloon)
                        {
                            // TODO: statt MSG war das s... s.Remove(0, s.IndexOf("> ") + 2).Trim();
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
                DataView view = CALL.DefaultView;
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
                    if (Settings.Default.AS_Active && qrb >= Convert.ToInt32(Settings.Default.AS_MinDist) && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                    {
                        LV.SubItems.Add(GetNearestPlanePotential(row["CALL"].ToString()).ToString());
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
                                row[band] = QRV_STATE.unknown;
                            CALL.Rows.Add(row);
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

        private void KST_Process_QRV(DataRow row, string qrvcall, bool call_new_in_userlist=false)
        {
            DataRow findrow = QRV.Rows.Find(qrvcall);
            if (findrow != null)
            {
                if (call_new_in_userlist)
                {
                    // if we have not just started (oldCALL empty) treat connecting
                    // to KST as activity
                    row["TIME"] = row["LOGINTIME"];
                    findrow["TIME"] = row["LOGINTIME"];
                }
                else
                    row["TIME"] = findrow["TIME"];
                foreach (string band in BANDS)
                    row[band] = findrow[band];
            }
            else
            {
                if (call_new_in_userlist)
                    row["TIME"] = row["LOGINTIME"];
                else
                    row["TIME"] = DateTime.MinValue;
                foreach (string band in BANDS)
                    row[band] = QRV_STATE.unknown;
                DataRow newrow = QRV.NewRow();
                newrow["CALL"] = qrvcall;
                if (call_new_in_userlist)
                    newrow["TIME"] = row["LOGINTIME"];
                else
                    newrow["TIME"] = DateTime.MinValue;
                foreach (string band in BANDS)
                    newrow[band] = QRV_STATE.unknown;
                try
                {
                    QRV.Rows.Add(newrow);
                }
                catch (Exception e)
                {
                    Error(MethodBase.GetCurrentMethod().Name, e.Message);
                }
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
            string[] usr = s.Split('|'); ;
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

                                int qrb = WCCheck.WCCheck.QRB(MyLoc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(MyLoc, loc);
                                row["QRB"] = qrb;
                                row["DIR"] = qtf;

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

                                KST_Process_QRV(row, qrvcall, call_new_in_userlist);

                                CALL.Rows.Add(row);
                                if (call_new_in_userlist)
                                    Check_QSO(row);
                            }
                        }
                        break;

                        //User already logged
                        //UM3 | chat id | callsign | firstname | locator | state |

                    case "UM":
                        {
                            string call = usr[2].Trim();
                            DataRow row = CALL.Rows.Find(call);
                            string loc = usr[4].Trim();

                            if (WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                row["CALL"] = call;
                                row["NAME"] = usr[3].Trim();
                                row["LOC"] = loc;

                                int qrb = WCCheck.WCCheck.QRB(MyLoc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(MyLoc, loc);
                                row["QRB"] = qrb;
                                row["DIR"] = qtf;

                                Int32 usr_state = Int32.Parse(usr[5]);
                                row["AWAY"] = (usr_state & 1) == 1;
                            }
                        }
                        break;

                        // User state (here/not here/more than 5 min logged)
                        // US4 | chat id | callsign | state |
                    case "US":
                        {
                            string call = usr[2].Trim();
                            DataRow row = CALL.Rows.Find(call);
                            Int32 usr_state = Int32.Parse(usr[3]);
                            row["AWAY"] = (usr_state & 1) == 1;
                        }
                        break;

                        // User disconnected (to remove)
                        // UR6 | chat id | callsign |

                    case "UR":
                        {
                            string call = usr[2].Trim();
                            DataRow row = CALL.Rows.Find(call);
                            row.Delete();
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
                Error(MethodBase.GetCurrentMethod().Name, "(" + s + "): " + e.Message);
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
                    CALL.Clear();
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
                tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1)+"|0|/AWAY|0|\r");
                UserState = MainDlg.USER_STATE.Away;
            }
        }

        private void KST_Send()
        {
            if (tw.Connected && cb_Command.Text.Length > 0)
            {
                if (!cb_Command.Text.StartsWith("/") || cb_Command.Text.ToUpper().StartsWith("/CQ"))
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
                    MessageBox.Show("Sending commands except /cq is not allowed!", "KST SendCommand");
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
            if (Settings.Default.Band_144 && (QRV_STATE)row["144M"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_432 && (QRV_STATE)row["432M"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_1296 && (QRV_STATE)row["1_2G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_2320 && (QRV_STATE)row["2_3G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_3400 && (QRV_STATE)row["3_4G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_5760 && (QRV_STATE)row["5_7G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_10368 && (QRV_STATE)row["10G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_24GHz && (QRV_STATE)row["24G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_47GHz && (QRV_STATE)row["47G"] != QRV_STATE.worked)
                return false;
            if (Settings.Default.Band_76GHz && (QRV_STATE)row["76G"] != QRV_STATE.worked)
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
                Dlg.tb_KST_ServerName.Enabled = false;
                Dlg.tb_KST_UserName.Enabled = false;
                Dlg.cbb_KST_Chat.Enabled = false;
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
                    CALL.Clear();
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
            if (Settings.Default.KST_AutoConnect && MessageBox.Show("The KSTAutoConnect function is on. You will be reconnected automatically after 30secs. Do you want to switch this function off?", "KSTAutoConnect", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
            foreach (string band in BANDS)
            {
                DataRow qso_row = wtQSO.QSO.Rows.Find(new object[] { call, band });
                if (qso_row != null)
                {
                    call_row[band] = (int)QRV_STATE.worked;
                    // check locator
                    if (call_row["LOC"].ToString() != qso_row["LOC"].ToString())
                    {
                        Say(call + " Locator wrong? Win-Test Log " + band + " " + qso_row["TIME"] + " " + call + " " + qso_row["LOC"] + " KST " + call_row["LOC"].ToString());
                        WinTestLocatorWarning = true;
                        Log.WriteMessage("Win-Test log locator mismatch: " + qso_row["BAND"] + " " + qso_row["TIME"] + " " + call + " Locator wrong? Win-Test Log " + qso_row["LOC"] + " KST " + call_row["LOC"].ToString());
                    }
                }
            }
        }

        private void Check_QSOs()
        {
            WinTestLocatorWarning = false;
            try
            {
                foreach (DataRow call_row in CALL.Rows)
                {
                    Check_QSO(call_row);
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + wtQSO.getFileName() + "): " + e.Message);
            }
        }

        private void Get_Planes()
        {
        }

        public string GetNearestPlanes(string call)
        {
            call = call.TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' });
            PlaneInfoList infolist = null;
            string result;
            if (AS_if.planes.TryGetValue(call, out infolist))
            {
                string s = DateTime.UtcNow.ToString("HH:mm") + " [" + (DateTime.UtcNow - infolist.UTC).Minutes.ToString() + "mins ago]\n\n";
                foreach (PlaneInfo info in infolist)
                {
                    s = string.Concat(new object[]
                    {
                        s,
                        info.Potential.ToString(),
                        " : ",
                        info.Call,
                        "[",
                        info.Category,
                        "] --> ",
                        info.IntQRB.ToString(),
                        "km [",
                        info.Mins,
                        "mins]\n"
                    });
                }
                result = s;
            }
            else
            {
                result = "";
            }
            return result;
        }

        public int GetNearestPlanePotential(string call)
        {
            call = call.TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' });
            PlaneInfoList infolist = null;
            int result;
            if (AS_if.planes.TryGetValue(call, out infolist))
            {
                result = infolist[0].Potential;
            }
            else
            {
                result = 0;
            }
            return result;
        }

        private void ti_Main_Tick(object sender, EventArgs e)
        {
            ti_Main.Stop();
            if (KSTState == MainDlg.KST_STATE.Connected)
            {
                if (wtQSO != null && Settings.Default.WinTest_Activate)
                {
                    try
                    {
                        MainDlg.Log.WriteMessage("KST wt Get_QSOs start.");

                        wtQSO.Get_QSOs(Settings.Default.WinTest_INI_FileName);
                        if (WCCheck.WCCheck.IsLoc(wtQSO.MyLoc) > 0 && !wtQSO.MyLoc.Equals(Settings.Default.KST_Loc))
                        {
                            MyLoc = wtQSO.MyLoc;
                            set_KST_Status();
                        }
                        Check_QSOs();
                    }
                    catch
                    {

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
        }

        private void MainDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                bw_GetPlanes.CancelAsync();
                ni_Main.Visible = false; // hide NotificationIcon
                string FileName = Path.Combine(Application.LocalUserAppDataPath, Settings.Default.WinTest_QRV_Table_FileName);
                Say("Saving QRV-Database to " + FileName + "...");
                QRV.WriteXml(FileName, XmlWriteMode.IgnoreSchema);
            }
            catch (Exception e2)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(qrv.xml): " + e2.Message);
            }
            // Copy window location to app settings
            Settings.Default.WindowLocation = Location;

            // Copy window size to app settings
            if (WindowState == FormWindowState.Normal)
            {
                Settings.Default.WindowSize = Size;
            }
            else
            {
                Settings.Default.WindowSize = RestoreBounds.Size;
            }

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
            p = control.PointToScreen(p);
            p = base.PointToClient(p);
            p.X += BorderWith;
            p.Y = p.Y + TitleBarHeight + Cursor.Size.Height;
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
                QRV_STATE state = QRV_STATE.unknown;
                try
                {
                    Enum.TryParse<QRV_STATE>(e.SubItem.Text, out state);
                }
                catch
                {
                }
                switch (state)
                {
                case QRV_STATE.unknown:
                    e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                    break;
                case QRV_STATE.qrv:
                    e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
                    break;
                case QRV_STATE.worked:
                    e.Graphics.FillRectangle(Brushes.Green, e.Bounds);
                    break;
                case QRV_STATE.not_qrv:
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
                        e.DrawBackground();
                        int pot = Convert.ToInt32(e.SubItem.Text);
                        Rectangle b = e.Bounds;
                        b.Inflate(-2, -2);
                        int num = pot;
                        if (num != 50)
                        {
                            if (num != 75)
                            {
                                if (num == 100)
                                {
                                    e.Graphics.FillEllipse(new SolidBrush(Color.Magenta), b);
                                }
                            }
                            else
                            {
                                e.Graphics.FillEllipse(new SolidBrush(Color.Red), b);
                            }
                        }
                        else
                        {
                            e.Graphics.FillEllipse(new SolidBrush(Color.DarkOrange), b);
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
                if (info.SubItem.Name == "Call" || info.SubItem.Name == "Name" || info.SubItem.Name == "Locator" || info.SubItem.Name == "UTC" || info.SubItem.Name == "AS")
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
                        WCCheck.WCCheck.QTF(MyLoc, info.Item.SubItems[2].Text).ToString("000"),
                        "°\nQRB:\t",
                        WCCheck.WCCheck.QRB(MyLoc, info.Item.SubItems[2].Text),
                        " km\n\nLeft click to\nSend Message."
                    });
                }
                if (info.SubItem.Name == "AS")
                {
                    string s = GetNearestPlanes(info.Item.Text);
                    if (string.IsNullOrEmpty(s))
                    {
                        ToolTipText = "No planes\n\nLeft click for map";
                        DataRow Row = CALL.Rows.Find(info.Item.Text);
                        if (Row != null && Settings.Default.AS_Active)
                        {
                            int qrb = (int)Row["QRB"];
                            if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                                ToolTipText = "Too close for planes\n\nLeft click for map";
                            else if (qrb > Convert.ToInt32(Settings.Default.AS_MaxDist))
                                ToolTipText = "Too far away for planes\n\nLeft click for map";
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
                    QRV_STATE state = QRV_STATE.unknown;
                    try
                    {
                        Enum.TryParse<QRV_STATE>(info.SubItem.Text, out state);
                    }
                    catch
                    {
                    }
                    if (state != QRV_STATE.worked)
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
                cb_Command.Text = "/cq " + lv_Calls.SelectedItems[0].Text + " ";
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
                QRV_STATE state = QRV_STATE.unknown;
                DataRow Row = null;
                ListViewHitTestInfo info = lv_Calls.HitTest(p);
                if (info != null && info.SubItem != null)
                {
                    if (info.SubItem.Name == "Call" || info.SubItem.Name == "Name" || info.SubItem.Name == "Locator" || info.SubItem.Name == "UTC")
                    {
                        if (info.Item.Text.Length > 0 && KSTState == MainDlg.KST_STATE.Connected)
                        {
                            cb_Command.Text = "/cq " + info.Item.Text.Replace("(", "").Replace(")", "") + " ";
                            cb_Command.SelectionStart = cb_Command.Text.Length;
                            cb_Command.SelectionLength = 0;
                        }
                    }
                    if (info.SubItem.Name == "AS" && Settings.Default.AS_Active)
                    {
                        string call = WCCheck.WCCheck.Cut(info.Item.Text.Replace("(", "").Replace(")", ""));
                        string loc = info.Item.SubItems[2].Text;

                        AS_if.show_path(call, loc, MyCall, MyLoc);
                    }
                    if (info.SubItem.Name[0] > '0' && info.SubItem.Name[0] < '9')
                    {
                        // band columns
                        state = QRV_STATE.unknown;
                        try
                        {
                            Enum.TryParse<QRV_STATE>(info.SubItem.Text, out state);
                        }
                        catch
                        {
                        }
                        switch (state)
                        {
                            case QRV_STATE.unknown:
                                info.SubItem.Text = QRV_STATE.qrv.ToString();
                                break;
                            case QRV_STATE.qrv:
                                info.SubItem.Text = QRV_STATE.not_qrv.ToString();
                                break;
                            case QRV_STATE.not_qrv:
                                info.SubItem.Text = QRV_STATE.unknown.ToString();
                                break;
                        }
                        Row = QRV.Rows.Find(info.Item.Text);
                        if (Row != null)
                        {
                            state = QRV_STATE.unknown;
                            try
                            {
                                state = (QRV_STATE)Convert.ToInt32(Row[info.SubItem.Name].ToString());
                            }
                            catch
                            {
                            }
                            switch (state)
                            {
                                case QRV_STATE.unknown:
                                    Row[info.SubItem.Name] = QRV_STATE.qrv;
                                break;
                                case QRV_STATE.qrv:
                                    Row[info.SubItem.Name] = QRV_STATE.not_qrv;
                                    break;
                                case QRV_STATE.not_qrv:
                                    Row[info.SubItem.Name] = QRV_STATE.unknown;
                                    break;
                            }
                        }
                        Row = CALL.Rows.Find(info.Item.Text);
                        if (Row != null)
                        {
                            state = QRV_STATE.unknown;
                            try
                            {
                                Enum.TryParse<QRV_STATE>(Row[info.SubItem.Name].ToString(), out state);
                            }
                            catch
                            {
                            }
                            switch (state)
                            {
                                case QRV_STATE.unknown:
                                    Row[info.SubItem.Name] = QRV_STATE.qrv;
                                    break;
                                case QRV_STATE.qrv:
                                    Row[info.SubItem.Name] = QRV_STATE.not_qrv;
                                    break;
                                case QRV_STATE.not_qrv:
                                    Row[info.SubItem.Name] = QRV_STATE.unknown;
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
                    if (info.SubItem.Name == "AS" && Settings.Default.AS_Active)
                    {
                        string s = GetNearestPlanes(info.Item.Text);
                        if (string.IsNullOrEmpty(s))
                        {
                            ToolTipText = "No planes\n\nLeft click for map";
                            DataRow Row = CALL.Rows.Find(info.Item.Text);
                            if (Row != null && Settings.Default.AS_Active)
                            {
                                int qrb = (int)Row["QRB"];
                                if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                                    ToolTipText = "Too close for planes\n\nLeft click for map";
                                else if (qrb > Convert.ToInt32(Settings.Default.AS_MaxDist))
                                    ToolTipText = "Too far away for planes\n\nLeft click for map";
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
                if (info.Item.SubItems[1].Text.Length > 0 && KSTState == MainDlg.KST_STATE.Connected)
                {
                    cb_Command.Text = "/cq " + info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "") + " ";
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
                if (info.Item.SubItems[1].Text.Length > 0 && KSTState == MainDlg.KST_STATE.Connected)
                {
                    cb_Command.Text = "/cq " + info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "") + " ";
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
                ShowToolTip("Left click to\nSend Message.", lv_MyMsg, p);
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
            foreach (DataRow row in CALL.Rows)
            {
                string dxcall = WCCheck.WCCheck.Cut(row["CALL"].ToString().TrimStart(
                    new char[] { '(' }).TrimEnd(new char[] { ')' }));
                string dxloc = row["LOC"].ToString();
                watchlist += string.Concat(new string[] { ",", dxcall, ",", dxloc });
            }
            AS_if.send_watchlist(watchlist, MyCall, MyLoc);
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
                for (int i = 0; Settings.Default.AS_Active && i < CALL.Rows.Count; i++)
                {
                    try
                    {
                        int qrb = (int)CALL.Rows[i]["QRB"];
                        string mycall = WCCheck.WCCheck.Cut(MyCall);
                        string dxcall = WCCheck.WCCheck.Cut(CALL.Rows[i]["CALL"].ToString().TrimStart(
                            new char[]{ '(' }).TrimEnd(new char[]{ ')' }));
                        string dxloc = CALL.Rows[i]["LOC"].ToString();
                        if (Settings.Default.AS_Active && qrb >= Convert.ToInt32(Settings.Default.AS_MinDist) 
                            && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                        {
                            if (!AS_if.GetPlanes(mycall, MyLoc, dxcall, dxloc, ref bw_GetPlanes))
                            {
                                errors++;
                                if (errors > 10)
                                {
                                    bw_GetPlanes.ReportProgress(0, null);
                                    break;
                                }

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
            if (e.ProgressPercentage > 0)
            {
                wtMessage Msg = (wtMessage)e.UserState;
                PlaneInfoList infolist = new PlaneInfoList();

                string dxcall;
                AS_if.process_msg_asnearest(Msg, infolist, out dxcall);
                if (infolist.Count > 0)
                {
                    foreach (ListViewItem call_lvi in lv_Calls.Items)
                    {
                        if (call_lvi.Text.IndexOf(dxcall) >= 0)
                        {
                            string newtext = infolist[0].Potential.ToString();
                            if (call_lvi.SubItems[4].Text != newtext)
                            {
                                call_lvi.SubItems[4].Text = newtext;
                                lv_Calls.Refresh();
                            }
                            break;
                        }
                    }
                }
            }
            else if (e.ProgressPercentage == 0)
            {
                string dxcall = (string)e.UserState;
                if (dxcall == null)
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
                else
                {
                    AS_if.planes.Remove(dxcall);
                    foreach (ListViewItem lvi in lv_Calls.Items)
                    {
                        if (lvi.Text.IndexOf(dxcall) >= 0)
                        {
                            string newtext = "";
                            if (lvi.SubItems[4].Text != newtext)
                            {
                                lvi.SubItems[4].Text = newtext;
                                lv_Calls.Refresh();
                            }
                            break;
                        }
                    }
                }
            }
            else /* e.ProgressPercentage <0 */
            {
                wtMessage Msg = (wtMessage)e.UserState;
                AS_if.process_msg_setpath(Msg);
            }
        }

        private void bw_GetPlanes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDlg));
            ss_Main = new System.Windows.Forms.StatusStrip();
            tsl_Info = new System.Windows.Forms.ToolStripStatusLabel();
            tsl_Error = new System.Windows.Forms.ToolStripStatusLabel();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            splitContainer3 = new System.Windows.Forms.SplitContainer();
            cb_Command = new System.Windows.Forms.ComboBox();
            btn_KST_Send = new System.Windows.Forms.Button();
            lbl_KST_Status = new System.Windows.Forms.Label();
            lbl_Call = new System.Windows.Forms.Label();
            lv_Msg = new System.Windows.Forms.ListView();
            lvh_Time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            lvh_Call = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            lvh_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            lvh_Msg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            lbl_KST_Msg = new System.Windows.Forms.Label();
            lv_MyMsg = new System.Windows.Forms.ListView();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            lbl_KST_MyMsg = new System.Windows.Forms.Label();
            lv_Calls = new System.Windows.Forms.ListView();
            ch_Call = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ch_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ch_Loc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ch_Act = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ch_AS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader144 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader432 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader1296 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader2320 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader3400 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader5760 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader10368 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader24GHz = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader47GHz = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            columnHeader76GHz = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            il_Calls = new System.Windows.Forms.ImageList(components);
            lbl_KST_Calls = new System.Windows.Forms.Label();
            mn_Main = new System.Windows.Forms.MenuStrip();
            tsm_File = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tsi_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
            tsm_KST = new System.Windows.Forms.ToolStripMenuItem();
            tsi_KST_Connect = new System.Windows.Forms.ToolStripMenuItem();
            tsi_KST_Disconnect = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            tsi_KST_Here = new System.Windows.Forms.ToolStripMenuItem();
            tsi_KST_Away = new System.Windows.Forms.ToolStripMenuItem();
            tsi_Options = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ti_Main = new System.Windows.Forms.Timer(components);
            ni_Main = new System.Windows.Forms.NotifyIcon(components);
            cmn_Notify = new System.Windows.Forms.ContextMenuStrip(components);
            cmi_Notify_Restore = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            cmi_Notify_Quit = new System.Windows.Forms.ToolStripMenuItem();
            ti_Receive = new System.Windows.Forms.Timer(components);
            ti_Error = new System.Windows.Forms.Timer(components);
            ti_Top = new System.Windows.Forms.Timer(components);
            ti_Reconnect = new System.Windows.Forms.Timer(components);
            ti_Linkcheck = new System.Timers.Timer();
            il_Planes = new System.Windows.Forms.ImageList(components);
            tt_Info = new System.Windows.Forms.ToolTip(components);
            bw_GetPlanes = new System.ComponentModel.BackgroundWorker();
            ss_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer3)).BeginInit();
            splitContainer3.Panel1.SuspendLayout();
            splitContainer3.Panel2.SuspendLayout();
            splitContainer3.SuspendLayout();
            mn_Main.SuspendLayout();
            cmn_Notify.SuspendLayout();
            SuspendLayout();
            // 
            // ss_Main
            // 
            ss_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsl_Info,
            tsl_Error});
            ss_Main.Location = new System.Drawing.Point(0, 708);
            ss_Main.Name = "ss_Main";
            ss_Main.Size = new System.Drawing.Size(1202, 22);
            ss_Main.TabIndex = 9;
            ss_Main.Text = "statusStrip1";
            // 
            // tsl_Info
            // 
            tsl_Info.Name = "tsl_Info";
            tsl_Info.Size = new System.Drawing.Size(28, 17);
            tsl_Info.Text = "Info";
            // 
            // tsl_Error
            // 
            tsl_Error.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tsl_Error.ForeColor = System.Drawing.Color.Red;
            tsl_Error.Name = "tsl_Error";
            tsl_Error.Size = new System.Drawing.Size(1159, 17);
            tsl_Error.Spring = true;
            tsl_Error.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(lv_Calls);
            splitContainer1.Panel2.Controls.Add(lbl_KST_Calls);
            splitContainer1.Size = new System.Drawing.Size(1202, 684);
            splitContainer1.SplitterDistance = 843;
            splitContainer1.TabIndex = 10;
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(splitContainer3);
            splitContainer2.Panel1MinSize = 75;
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(lv_MyMsg);
            splitContainer2.Panel2.Controls.Add(lbl_KST_MyMsg);
            splitContainer2.Panel2MinSize = 75;
            splitContainer2.Size = new System.Drawing.Size(843, 684);
            splitContainer2.SplitterDistance = 346;
            splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer3.Location = new System.Drawing.Point(0, 0);
            splitContainer3.Name = "splitContainer3";
            splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            splitContainer3.Panel1.Controls.Add(cb_Command);
            splitContainer3.Panel1.Controls.Add(btn_KST_Send);
            splitContainer3.Panel1.Controls.Add(lbl_KST_Status);
            splitContainer3.Panel1.Controls.Add(lbl_Call);
            splitContainer3.Panel1MinSize = 80;
            // 
            // splitContainer3.Panel2
            // 
            splitContainer3.Panel2.Controls.Add(lv_Msg);
            splitContainer3.Panel2.Controls.Add(lbl_KST_Msg);
            splitContainer3.Size = new System.Drawing.Size(843, 346);
            splitContainer3.SplitterDistance = 80;
            splitContainer3.TabIndex = 0;
            // 
            // cb_Command
            // 
            cb_Command.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_Command.FormattingEnabled = true;
            cb_Command.Location = new System.Drawing.Point(11, 41);
            cb_Command.MaxDropDownItems = 5;
            cb_Command.Name = "cb_Command";
            cb_Command.Size = new System.Drawing.Size(469, 24);
            cb_Command.TabIndex = 17;
            cb_Command.TextUpdate += new System.EventHandler(cb_Command_TextUpdate);
            cb_Command.DropDownClosed += new System.EventHandler(cb_Command_DropDownClosed);
            cb_Command.TextChanged += new System.EventHandler(cb_Command_TextChanged);
            // 
            // btn_KST_Send
            // 
            btn_KST_Send.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_KST_Send.Location = new System.Drawing.Point(508, 41);
            btn_KST_Send.Name = "btn_KST_Send";
            btn_KST_Send.Size = new System.Drawing.Size(75, 23);
            btn_KST_Send.TabIndex = 16;
            btn_KST_Send.Text = "Send";
            btn_KST_Send.UseVisualStyleBackColor = true;
            btn_KST_Send.Click += new System.EventHandler(btn_KST_Send_Click_1);
            // 
            // lbl_KST_Status
            // 
            lbl_KST_Status.BackColor = System.Drawing.SystemColors.ScrollBar;
            lbl_KST_Status.Dock = System.Windows.Forms.DockStyle.Top;
            lbl_KST_Status.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            lbl_KST_Status.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbl_KST_Status.Location = new System.Drawing.Point(0, 0);
            lbl_KST_Status.Name = "lbl_KST_Status";
            lbl_KST_Status.Padding = new System.Windows.Forms.Padding(4);
            lbl_KST_Status.Size = new System.Drawing.Size(841, 26);
            lbl_KST_Status.TabIndex = 15;
            lbl_KST_Status.Text = "Status";
            lbl_KST_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl_Call
            // 
            lbl_Call.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbl_Call.Location = new System.Drawing.Point(589, 41);
            lbl_Call.Name = "lbl_Call";
            lbl_Call.Size = new System.Drawing.Size(100, 23);
            lbl_Call.TabIndex = 14;
            lbl_Call.Text = "Call";
            lbl_Call.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lv_Msg
            // 
            lv_Msg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            lvh_Time,
            lvh_Call,
            lvh_Name,
            lvh_Msg});
            lv_Msg.Dock = System.Windows.Forms.DockStyle.Fill;
            lv_Msg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lv_Msg.FullRowSelect = true;
            lv_Msg.GridLines = true;
            lv_Msg.Location = new System.Drawing.Point(0, 26);
            lv_Msg.MultiSelect = false;
            lv_Msg.Name = "lv_Msg";
            lv_Msg.Size = new System.Drawing.Size(841, 234);
            lv_Msg.TabIndex = 12;
            lv_Msg.UseCompatibleStateImageBehavior = false;
            lv_Msg.View = System.Windows.Forms.View.Details;
            lv_Msg.MouseDown += new System.Windows.Forms.MouseEventHandler(lv_Msg_MouseDown);
            lv_Msg.MouseMove += new System.Windows.Forms.MouseEventHandler(lv_Msg_MouseMove);
            lv_Msg.Resize += new System.EventHandler(lv_Msg_Resize);
            // 
            // lvh_Time
            // 
            lvh_Time.Text = "Time";
            lvh_Time.Width = 50;
            // 
            // lvh_Call
            // 
            lvh_Call.Text = "Call";
            lvh_Call.Width = 100;
            // 
            // lvh_Name
            // 
            lvh_Name.Text = "Name";
            lvh_Name.Width = 150;
            // 
            // lvh_Msg
            // 
            lvh_Msg.Text = "Messages";
            lvh_Msg.Width = 600;
            // 
            // lbl_KST_Msg
            // 
            lbl_KST_Msg.BackColor = System.Drawing.SystemColors.ScrollBar;
            lbl_KST_Msg.Dock = System.Windows.Forms.DockStyle.Top;
            lbl_KST_Msg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            lbl_KST_Msg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbl_KST_Msg.Location = new System.Drawing.Point(0, 0);
            lbl_KST_Msg.Name = "lbl_KST_Msg";
            lbl_KST_Msg.Padding = new System.Windows.Forms.Padding(4);
            lbl_KST_Msg.Size = new System.Drawing.Size(841, 26);
            lbl_KST_Msg.TabIndex = 11;
            lbl_KST_Msg.Text = "Messages";
            lbl_KST_Msg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lv_MyMsg
            // 
            lv_MyMsg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            columnHeader2,
            columnHeader3,
            columnHeader4});
            lv_MyMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            lv_MyMsg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lv_MyMsg.FullRowSelect = true;
            lv_MyMsg.GridLines = true;
            lv_MyMsg.Location = new System.Drawing.Point(0, 26);
            lv_MyMsg.MultiSelect = false;
            lv_MyMsg.Name = "lv_MyMsg";
            lv_MyMsg.Size = new System.Drawing.Size(841, 306);
            lv_MyMsg.TabIndex = 13;
            lv_MyMsg.UseCompatibleStateImageBehavior = false;
            lv_MyMsg.View = System.Windows.Forms.View.Details;
            lv_MyMsg.MouseDown += new System.Windows.Forms.MouseEventHandler(lv_MyMsg_MouseDown);
            lv_MyMsg.MouseMove += new System.Windows.Forms.MouseEventHandler(lv_MyMsg_MouseMove);
            lv_MyMsg.Resize += new System.EventHandler(lv_MyMsg_Resize);
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Time";
            columnHeader1.Width = 50;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Call";
            columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Name";
            columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Messages";
            columnHeader4.Width = 600;
            // 
            // lbl_KST_MyMsg
            // 
            lbl_KST_MyMsg.BackColor = System.Drawing.Color.BlanchedAlmond;
            lbl_KST_MyMsg.Dock = System.Windows.Forms.DockStyle.Top;
            lbl_KST_MyMsg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            lbl_KST_MyMsg.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbl_KST_MyMsg.Location = new System.Drawing.Point(0, 0);
            lbl_KST_MyMsg.Name = "lbl_KST_MyMsg";
            lbl_KST_MyMsg.Padding = new System.Windows.Forms.Padding(4);
            lbl_KST_MyMsg.Size = new System.Drawing.Size(841, 26);
            lbl_KST_MyMsg.TabIndex = 10;
            lbl_KST_MyMsg.Text = "My Messages";
            lbl_KST_MyMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lv_Calls
            // 
            lv_Calls.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            ch_Call,
            ch_Name,
            ch_Loc,
            ch_Act,
            ch_AS,
            columnHeader144,
            columnHeader432,
            columnHeader1296,
            columnHeader2320,
            columnHeader3400,
            columnHeader5760,
            columnHeader10368,
            columnHeader24GHz,
            columnHeader47GHz,
            columnHeader76GHz});
            lv_Calls.Dock = System.Windows.Forms.DockStyle.Fill;
            lv_Calls.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lv_Calls.GridLines = true;
            lv_Calls.Location = new System.Drawing.Point(0, 24);
            lv_Calls.MultiSelect = false;
            lv_Calls.Name = "lv_Calls";
            lv_Calls.OwnerDraw = true;
            lv_Calls.Size = new System.Drawing.Size(353, 658);
            lv_Calls.SmallImageList = il_Calls;
            lv_Calls.TabIndex = 14;
            lv_Calls.UseCompatibleStateImageBehavior = false;
            lv_Calls.View = System.Windows.Forms.View.Details;
            lv_Calls.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(lv_Calls_ColumnClick);
            lv_Calls.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(lv_Calls_DrawColumnHeader);
            lv_Calls.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(lv_Calls_DrawItem);
            lv_Calls.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(lv_Calls_DrawSubItem);
            lv_Calls.MouseDown += new System.Windows.Forms.MouseEventHandler(lv_Calls_MouseDown);
            lv_Calls.MouseMove += new System.Windows.Forms.MouseEventHandler(lv_Calls_MouseMove);
            // 
            // ch_Call
            // 
            ch_Call.Text = "Call";
            ch_Call.Width = 80;
            // 
            // ch_Name
            // 
            ch_Name.Text = "Name";
            ch_Name.Width = 100;
            // 
            // ch_Loc
            // 
            ch_Loc.Text = "Locator";
            // 
            // ch_Act
            // 
            ch_Act.Text = "Act";
            ch_Act.Width = 30;
            // 
            // ch_AS
            // 
            ch_AS.Text = "AS";
            ch_AS.Width = 20;
            // 
            // columnHeader144
            // 
            columnHeader144.Text = "144M";
            columnHeader144.Width = 20;
            // 
            // columnHeader432
            // 
            columnHeader432.Text = "432M";
            columnHeader432.Width = 20;
            // 
            // columnHeader1296
            // 
            columnHeader1296.Text = "1.2G";
            columnHeader1296.Width = 20;
            // 
            // columnHeader2320
            // 
            columnHeader2320.Text = "2.3G";
            columnHeader2320.Width = 20;
            // 
            // columnHeader3400
            // 
            columnHeader3400.Text = "3.4G";
            columnHeader3400.Width = 20;
            // 
            // columnHeader5760
            // 
            columnHeader5760.Text = "5.7G";
            columnHeader5760.Width = 20;
            // 
            // columnHeader10368
            // 
            columnHeader10368.Text = "10G";
            columnHeader10368.Width = 20;
            // 
            // columnHeader24GHz
            // 
            columnHeader24GHz.Text = "24G";
            columnHeader24GHz.Width = 20;
            // 
            // columnHeader47GHz
            // 
            columnHeader47GHz.Text = "47G";
            columnHeader47GHz.Width = 20;
            // 
            // columnHeader76GHz
            // 
            columnHeader76GHz.Text = "76G";
            columnHeader76GHz.Width = 20;
            // 
            // il_Calls
            // 
            il_Calls.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_Calls.ImageStream")));
            il_Calls.TransparentColor = System.Drawing.Color.Transparent;
            il_Calls.Images.SetKeyName(0, "JEWEL_GRAY.PNG");
            il_Calls.Images.SetKeyName(1, "JEWEL_GREEN.PNG");
            il_Calls.Images.SetKeyName(2, "JEWEL_RED.PNG");
            il_Calls.Images.SetKeyName(3, "JEWEL_YELLWO.PNG");
            // 
            // lbl_KST_Calls
            // 
            lbl_KST_Calls.BackColor = System.Drawing.SystemColors.ScrollBar;
            lbl_KST_Calls.Dock = System.Windows.Forms.DockStyle.Top;
            lbl_KST_Calls.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            lbl_KST_Calls.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbl_KST_Calls.Location = new System.Drawing.Point(0, 0);
            lbl_KST_Calls.Name = "lbl_KST_Calls";
            lbl_KST_Calls.Padding = new System.Windows.Forms.Padding(4);
            lbl_KST_Calls.Size = new System.Drawing.Size(353, 24);
            lbl_KST_Calls.TabIndex = 0;
            lbl_KST_Calls.Text = "Calls";
            lbl_KST_Calls.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lbl_KST_Calls.UseMnemonic = false;
            // 
            // mn_Main
            // 
            mn_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsm_File,
            tsm_KST,
            tsi_Options,
            toolStripMenuItem1});
            mn_Main.Location = new System.Drawing.Point(0, 0);
            mn_Main.Name = "mn_Main";
            mn_Main.Size = new System.Drawing.Size(1202, 24);
            mn_Main.TabIndex = 11;
            mn_Main.Text = "menuStrip1";
            // 
            // tsm_File
            // 
            tsm_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripSeparator1,
            tsi_File_Exit});
            tsm_File.Name = "tsm_File";
            tsm_File.Size = new System.Drawing.Size(37, 20);
            tsm_File.Text = "&File";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(89, 6);
            // 
            // tsi_File_Exit
            // 
            tsi_File_Exit.Name = "tsi_File_Exit";
            tsi_File_Exit.Size = new System.Drawing.Size(92, 22);
            tsi_File_Exit.Text = "E&xit";
            tsi_File_Exit.Click += new System.EventHandler(tsi_File_Exit_Click);
            // 
            // tsm_KST
            // 
            tsm_KST.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsi_KST_Connect,
            tsi_KST_Disconnect,
            toolStripSeparator2,
            tsi_KST_Here,
            tsi_KST_Away});
            tsm_KST.Name = "tsm_KST";
            tsm_KST.Size = new System.Drawing.Size(39, 20);
            tsm_KST.Text = "&KST";
            // 
            // tsi_KST_Connect
            // 
            tsi_KST_Connect.Name = "tsi_KST_Connect";
            tsi_KST_Connect.Size = new System.Drawing.Size(160, 22);
            tsi_KST_Connect.Text = "&Connect";
            tsi_KST_Connect.Click += new System.EventHandler(tsi_KST_Connect_Click);
            // 
            // tsi_KST_Disconnect
            // 
            tsi_KST_Disconnect.Enabled = false;
            tsi_KST_Disconnect.Name = "tsi_KST_Disconnect";
            tsi_KST_Disconnect.Size = new System.Drawing.Size(160, 22);
            tsi_KST_Disconnect.Text = "&Disconnect";
            tsi_KST_Disconnect.Click += new System.EventHandler(tsi_KST_Disconnect_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // tsi_KST_Here
            // 
            tsi_KST_Here.Name = "tsi_KST_Here";
            tsi_KST_Here.Size = new System.Drawing.Size(160, 22);
            tsi_KST_Here.Text = "&I am on the chat";
            tsi_KST_Here.Click += new System.EventHandler(tsi_KST_Here_Click);
            // 
            // tsi_KST_Away
            // 
            tsi_KST_Away.Name = "tsi_KST_Away";
            tsi_KST_Away.Size = new System.Drawing.Size(160, 22);
            tsi_KST_Away.Text = "I am &away";
            tsi_KST_Away.Click += new System.EventHandler(tsi_KST_Away_Click);
            // 
            // tsi_Options
            // 
            tsi_Options.Name = "tsi_Options";
            tsi_Options.Size = new System.Drawing.Size(61, 20);
            tsi_Options.Text = "&Options";
            tsi_Options.Click += new System.EventHandler(tsi_Options_Click);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            aboutToolStripMenuItem});
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            toolStripMenuItem1.Text = "&?";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            aboutToolStripMenuItem.Text = "&About";
            // 
            // ti_Main
            // 
            ti_Main.Enabled = true;
            ti_Main.Interval = 30000;
            ti_Main.Tick += new System.EventHandler(ti_Main_Tick);
            // 
            // ni_Main
            // 
            ni_Main.ContextMenuStrip = cmn_Notify;
            ni_Main.Icon = ((System.Drawing.Icon)(resources.GetObject("ni_Main.Icon")));
            ni_Main.Text = "wtKST";
            ni_Main.Visible = true;
            ni_Main.BalloonTipClicked += new System.EventHandler(ni_Main_BalloonTipClicked);
            ni_Main.MouseClick += new System.Windows.Forms.MouseEventHandler(ni_Main_MouseClick);
            // 
            // cmn_Notify
            // 
            cmn_Notify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            cmi_Notify_Restore,
            toolStripSeparator3,
            cmi_Notify_Quit});
            cmn_Notify.Name = "cmn_Notify";
            cmn_Notify.Size = new System.Drawing.Size(119, 54);
            // 
            // cmi_Notify_Restore
            // 
            cmi_Notify_Restore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cmi_Notify_Restore.Name = "cmi_Notify_Restore";
            cmi_Notify_Restore.Size = new System.Drawing.Size(118, 22);
            cmi_Notify_Restore.Text = "&Restore";
            cmi_Notify_Restore.Click += new System.EventHandler(cmi_Notify_Restore_Click);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(115, 6);
            // 
            // cmi_Notify_Quit
            // 
            cmi_Notify_Quit.Name = "cmi_Notify_Quit";
            cmi_Notify_Quit.Size = new System.Drawing.Size(118, 22);
            cmi_Notify_Quit.Text = "&Quit";
            cmi_Notify_Quit.Click += new System.EventHandler(cmi_Notify_Quit_Click);
            // 
            // ti_Receive
            // 
            ti_Receive.Enabled = true;
            ti_Receive.Interval = 1000;
            ti_Receive.Tick += new System.EventHandler(ti_Receive_Tick);
            // 
            // ti_Error
            // 
            ti_Error.Interval = 10000;
            ti_Error.Tick += new System.EventHandler(ti_Error_Tick);
            // 
            // ti_Top
            // 
            ti_Top.Interval = 60000;
            ti_Top.Tick += new System.EventHandler(ti_Top_Tick);
            // 
            // ti_Reconnect
            // 
            ti_Reconnect.Enabled = false;
            ti_Reconnect.Interval = 30000;
            ti_Reconnect.Tick += new System.EventHandler(ti_Reconnect_Tick);
            // 
            // ti_Linkcheck
            // 
            ti_Linkcheck.Enabled = false;
            ti_Linkcheck.AutoReset = true;
            ti_Linkcheck.Interval = 120000;
            ti_Linkcheck.Elapsed += ti_Linkcheck_Tick;
            // 
            // il_Planes
            // 
            il_Planes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_Planes.ImageStream")));
            il_Planes.TransparentColor = System.Drawing.Color.Transparent;
            il_Planes.Images.SetKeyName(0, "Green");
            il_Planes.Images.SetKeyName(1, "Orange");
            il_Planes.Images.SetKeyName(2, "Blue");
            // 
            // tt_Info
            // 
            tt_Info.ShowAlways = true;
            // 
            // bw_GetPlanes
            // 
            bw_GetPlanes.WorkerReportsProgress = true;
            bw_GetPlanes.WorkerSupportsCancellation = true;
            bw_GetPlanes.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_GetPlanes_DoWork);
            bw_GetPlanes.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(bw_GetPlanes_ProgressChanged);
            bw_GetPlanes.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(bw_GetPlanes_RunWorkerCompleted);
            // 
            // MainDlg
            // 
            AcceptButton = btn_KST_Send;
            ClientSize = new System.Drawing.Size(1202, 730);
            Controls.Add(splitContainer1);
            Controls.Add(ss_Main);
            Controls.Add(mn_Main);
            DataBindings.Add(new System.Windows.Forms.Binding("Location", global::wtKST.Properties.Settings.Default, "WindowLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Location = global::wtKST.Properties.Settings.Default.WindowLocation;
            Name = "MainDlg";
            Text = "wtKST";
            FormClosing += new System.Windows.Forms.FormClosingEventHandler(MainDlg_FormClosing);
            Load += new System.EventHandler(MainDlg_Load);
            ss_Main.ResumeLayout(false);
            ss_Main.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer3.Panel1.ResumeLayout(false);
            splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer3)).EndInit();
            splitContainer3.ResumeLayout(false);
            mn_Main.ResumeLayout(false);
            mn_Main.PerformLayout();
            cmn_Notify.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }
    }
}
