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
            WaitUserName,
            WaitPassword,
            WaitChat,
            WaitAway,
            WaitConfig,
            WaitLogin,
            WaitLogstat,
            WaitSPR,
            Connected = 128,
            WaitUser
        }

        public enum USER_STATE
        {
            Here,
            Away
        }

        public DataTable MSG = new DataTable("MSG");

        public DataTable MYMSG = new DataTable("MYMSG");

        public DataTable QRV = new DataTable("QRV");

        public DataTable CALL = new DataTable("CALL");
        private DataTable oldCALL = new DataTable("CALL"); // used to hold copy of CALL

        public DataTable QSO = new DataTable("QSO");

        private bool lv_Calls_Updating = false;

        private bool DLLNotLoaded = false;

        private Point OldMousePos = new Point(0, 0);

        public static LogWriter Log = new LogWriter(Application.LocalUserAppDataPath);

        private TelnetWrapper tw;

        private string KSTBuffer = "";

        private Queue<string> MsgQueue = new Queue<string>();

        private string MyCall = "DL2ALF";

        private string MyLoc = "JO50IW";

        private MainDlg.KST_STATE KSTState;

        private MainDlg.USER_STATE UserState;

        public Dictionary<string, PlaneInfoList> planes = new Dictionary<string, PlaneInfoList>();

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
        private bool KST_Use_New_Feed;
        private DateTime latestMessageTimestamp = DateTime.MinValue;
        private bool latestMessageTimestampSet = false;
        private bool CheckStartUpAway = true;

        public MainDlg()
        {
            this.InitializeComponent();
            MainDlg.Log.FileFormat = "wtKST_{0:d}.log";
            MainDlg.Log.MessageFormat = "{0:u}: {1}";
            MainDlg.Log.WriteMessage("Startup.");
            if (!File.Exists("Telnet.dll"))
            {
                this.DLLNotLoaded = true;
            }
            Application.Idle += new EventHandler(this.OnIdle);
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            this.cb_Command.MouseWheel += new MouseEventHandler(this.cb_Command_MouseWheel);
            this.MSG.Columns.Add("TIME", typeof(DateTime));
            this.MSG.Columns.Add("CALL");
            this.MSG.Columns.Add("NAME");
            this.MSG.Columns.Add("MSG");
            this.MSG.Columns.Add("RECIPIENT");
            DataColumn[] MSGkeys = new DataColumn[]
            {
                MSG.Columns["TIME"],
                MSG.Columns["CALL"],
                MSG.Columns["MSG"]
            };
            this.MSG.PrimaryKey = MSGkeys;
            this.QRV.Columns.Add("CALL");
            this.QRV.Columns.Add("TIME", typeof(DateTime));
            this.QRV.Columns.Add("144M", typeof(int));
            this.QRV.Columns.Add("432M", typeof(int));
            this.QRV.Columns.Add("1_2G", typeof(int));
            this.QRV.Columns.Add("2_3G", typeof(int));
            this.QRV.Columns.Add("3_4G", typeof(int));
            this.QRV.Columns.Add("5_7G", typeof(int));
            this.QRV.Columns.Add("10G", typeof(int));
            this.QRV.Columns.Add("24G", typeof(int));
            this.QRV.Columns.Add("47G", typeof(int));
            this.QRV.Columns.Add("76G", typeof(int));
            DataColumn[] QRVkeys = new DataColumn[]
            {
                this.QRV.Columns["CALL"]
            };
            this.QRV.PrimaryKey = QRVkeys;
            this.InitializeQRV(false);
            this.QSO.Columns.Add("CALL");
            this.QSO.Columns.Add("BAND");
            this.QSO.Columns.Add("TIME");
            this.QSO.Columns.Add("SENT");
            this.QSO.Columns.Add("RCVD");
            this.QSO.Columns.Add("LOC");
            DataColumn[] QSOkeys = new DataColumn[]
            {
                this.QSO.Columns["CALL"],
                this.QSO.Columns["BAND"]
            };
            this.QSO.PrimaryKey = QSOkeys;
            this.CALL.Columns.Add("CALL");
            this.CALL.Columns.Add("NAME");
            this.CALL.Columns.Add("LOC");
            this.CALL.Columns.Add("TIME", typeof(DateTime));
            this.CALL.Columns.Add("CONTACTED", typeof(int));
            this.CALL.Columns.Add("LOGINTIME", typeof(DateTime));
            this.CALL.Columns.Add("ASLAT", typeof(double));
            this.CALL.Columns.Add("ASLON", typeof(double));
            this.CALL.Columns.Add("QRB", typeof(int));
            this.CALL.Columns.Add("DIR", typeof(int));
            this.CALL.Columns.Add("AWAY", typeof(bool));
            this.CALL.Columns.Add("AS", typeof(int));
            this.CALL.Columns.Add("144M", typeof(int));
            this.CALL.Columns.Add("432M", typeof(int));
            this.CALL.Columns.Add("1_2G", typeof(int));
            this.CALL.Columns.Add("2_3G", typeof(int));
            this.CALL.Columns.Add("3_4G", typeof(int));
            this.CALL.Columns.Add("5_7G", typeof(int));
            this.CALL.Columns.Add("10G", typeof(int));
            this.CALL.Columns.Add("24G", typeof(int));
            this.CALL.Columns.Add("47G", typeof(int));
            this.CALL.Columns.Add("76G", typeof(int));
            DataColumn[] CALLkeys = new DataColumn[]
            {
                this.CALL.Columns["CALL"]
            };
            this.CALL.PrimaryKey = CALLkeys;

            UpdateUserBandsWidth();
            this.bw_GetPlanes.RunWorkerAsync();
            if (Settings.Default.KST_AutoConnect)
            {
                this.KST_Connect();
            }
        }

        private void InitializeQRV(bool ForceReload)
        {
            this.QRV.Clear();
            try
            {
                string QRV_Table_Filename = Application.LocalUserAppDataPath + "\\" + Settings.Default.WinTest_QRV_Table_FileName;
                TimeSpan ts = DateTime.Now - File.GetLastWriteTime(QRV_Table_Filename);
                if (!ForceReload && File.Exists(QRV_Table_Filename) && ts.Hours < 48)
                {
                    try
                    {
                        this.QRV.BeginLoadData();
                        this.QRV.ReadXml(QRV_Table_Filename);
                        this.QRV.EndLoadData();
                    }
                    catch (Exception e)
                    {
                        this.Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml): " + e.Message);
                    }
                }
                // if we cannot read qrv.xml from appdata path, try current directoy (=previous default)
                if (this.QRV.Rows.Count == 0 && !ForceReload && File.Exists(Settings.Default.WinTest_QRV_Table_FileName))
                {
                    try
                    {
                        this.QRV.BeginLoadData();
                        this.QRV.ReadXml(Settings.Default.WinTest_QRV_Table_FileName);
                        this.QRV.EndLoadData();
                    }
                    catch (Exception e)
                    {
                        this.Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml): " + e.Message);
                    }
                }
                if (this.QRV.Rows.Count == 0)
                {
                    using (StreamReader sr = new StreamReader(Settings.Default.WinTest_QRV_FileName))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            DataRow Row = this.QRV.NewRow();
                            Row["CALL"] = line.Split(new char[0])[0];
                            Row["TIME"] = DateTime.Parse(line.Split(new char[0])[1].TrimStart(
                                new char[]{ '[' }).TrimEnd(new char[]{ ']' }) + " 00:00:00");
                            if (line.IndexOf("144M") > 0)
                            {
                                Row["144M"] = 1;
                            }
                            else
                            {
                                Row["144M"] = 0;
                            }
                            if (line.IndexOf("144M") > 0)
                            {
                                Row["144M"] = 1;
                            }
                            else
                            {
                                Row["432M"] = 0;
                            }
                            if (line.IndexOf("432M") > 0)
                            {
                                Row["432M"] = 1;
                            }
                            else
                            {
                                Row["432M"] = 0;
                            }
                            if (line.IndexOf("1.2G") > 0)
                            {
                                Row["1_2G"] = 1;
                            }
                            else
                            {
                                Row["1_2G"] = 0;
                            }
                            if (line.IndexOf("2.3G") > 0)
                            {
                                Row["2_3G"] = 1;
                            }
                            else
                            {
                                Row["2_3G"] = 0;
                            }
                            if (line.IndexOf("3.4G") > 0)
                            {
                                Row["3_4G"] = 1;
                            }
                            else
                            {
                                Row["3_4G"] = 0;
                            }
                            if (line.IndexOf("5.7G") > 0)
                            {
                                Row["5_7G"] = 1;
                            }
                            else
                            {
                                Row["5_7G"] = 0;
                            }
                            if (line.IndexOf("10G") > 0)
                            {
                                Row["10G"] = 1;
                            }
                            else
                            {
                                Row["10G"] = 0;
                            }
                            if (line.IndexOf("24G") > 0)
                            {
                                Row["24G"] = 1;
                            }
                            else
                            {
                                Row["24G"] = 0;
                            }
                            if (line.IndexOf("47G") > 0)
                            {
                                Row["47G"] = 1;
                            }
                            else
                            {
                                Row["47G"] = 0;
                            }
                            if (line.IndexOf("76G") > 0)
                            {
                                Row["76G"] = 1;
                            }
                            else
                            {
                                Row["76G"] = 0;
                            }
                            this.QRV.Rows.Add(Row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, "(" + Settings.Default.WinTest_QRV_FileName + "): " + e.Message);
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
                this.tw.Dispose();
                this.tw.Close();
                this.MsgQueue.Clear();
                this.KSTBuffer = "";
            }
            catch
            {
            }
            this.KSTState = MainDlg.KST_STATE.Disconnected;
        }

        private void set_KST_Status()
        {
            if (this.KSTState < MainDlg.KST_STATE.Connected)
            {
                this.lbl_KST_Status.BackColor = Color.LightGray;
                this.lbl_KST_Status.Text = "Status: Disconnected ";
                return;
            }
            this.lbl_KST_Status.BackColor = Color.PaleTurquoise;
            this.lbl_KST_Status.Text = string.Concat(new string[]
            {
                "Status: Connected to ",
                Settings.Default.KST_Chat,
                " chat   [",
                Settings.Default.KST_UserName,
                " ",
                Settings.Default.KST_Name,
                " ",
                this.MyLoc,
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
            lock (this.MsgQueue)
            {
                this.KSTBuffer += s;
                if (this.KSTBuffer.IndexOf("\r\n")>=0)
                {
                    string[] buffer = this.KSTBuffer.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries); //this.KSTBuffer.Split(new char[] {'\r', '\n' });
                    string[] array = buffer;
                    int number_strings = array.Length-1;
                    if (KSTBuffer.EndsWith("\r\n"))
                        number_strings++;
                    for (int i = 0; i < number_strings; i++)
                    {
                        string buf = array[i];
                        StringWriter bufWriter = new StringWriter();
                        WebUtility.HtmlDecode(buf, bufWriter);
                        this.MsgQueue.Enqueue(bufWriter.ToString());
                    }
                    if (KSTBuffer.EndsWith("\r\n"))
                        this.KSTBuffer = "";
                    else
                        this.KSTBuffer = buffer[buffer.Length - 1]; // keep the tail
                }
            }
            if (this.KST_Use_New_Feed && this.KSTState >= MainDlg.KST_STATE.Connected)
            {
                this.ti_Linkcheck.Stop();   // restart the linkcheck timer
                this.ti_Linkcheck.Start();
            }
        }

        private void OnIdle(object sender, EventArgs args)
        {
            if (this.DLLNotLoaded)
            {
                MessageBox.Show("The file telnet.dll could not be found in program directory.", "Error");
                base.Close();
            }
            try
            {
                if (this.tw != null && !this.tw.Connected && this.KSTState != MainDlg.KST_STATE.Standby)
                {
                    this.KSTState = MainDlg.KST_STATE.Disconnected;
                }
            }
            catch
            {
                this.KSTState = MainDlg.KST_STATE.Disconnected;
            }
            if (this.KSTState == MainDlg.KST_STATE.Disconnecting)
            {
                if (this.tw != null && this.tw.Connected)
                    tw.Disconnect();
            }
            if (this.KSTState == MainDlg.KST_STATE.Disconnected)
            {
                this.CALL.Clear();
                this.lv_Calls.Items.Clear();
                this.tsi_KST_Connect.Enabled = true;
                this.tsi_KST_Disconnect.Enabled = false;
                this.tsi_KST_Here.Enabled = false;
                this.tsi_KST_Away.Enabled = false;
                this.cb_Command.Enabled = false;
                this.btn_KST_Send.Enabled = false;
                this.lbl_Call.Enabled = false;
                this.ti_Main.Stop();
                if (Settings.Default.KST_AutoConnect && !this.ti_Reconnect.Enabled)
                {
                    this.ti_Reconnect.Start();
                }
                if (this.KST_Use_New_Feed && this.ti_Linkcheck.Enabled)
                    this.ti_Linkcheck.Stop();

                this.KSTState = MainDlg.KST_STATE.Standby;
            }
            if (this.KSTState >= MainDlg.KST_STATE.Connected)
            {
                this.tsi_KST_Connect.Enabled = false;
                this.tsi_KST_Disconnect.Enabled = true;
                this.tsi_KST_Here.Enabled = true;
                this.tsi_KST_Away.Enabled = true;
                this.cb_Command.Enabled = true;
                this.btn_KST_Send.Enabled = true;
                this.lbl_Call.Enabled = true;
                if (this.ti_Reconnect.Enabled)
                {
                    this.ti_Reconnect.Stop();
                }
            }
            // FIXME: don't touch UI elements here without checking wether they need to be updated. Otherwise CPU load may be high
            if (this.UserState == MainDlg.USER_STATE.Here)
            {
                this.lbl_Call.Text = Settings.Default.KST_UserName.ToUpper();
            }
            else
            {
                this.lbl_Call.Text = "(" + Settings.Default.KST_UserName.ToUpper() + ")";
            }
            string KST_Calls_Text = this.lbl_KST_Calls.Text;
            if (this.lv_Calls.Items.Count > 0)
            {
                KST_Calls_Text = "Calls [" + this.lv_Calls.Items.Count.ToString() + "]";
                if (Settings.Default.WinTest_Activate)
                    KST_Calls_Text += " - " + Path.GetFileName(Settings.Default.WinTest_FileName);
            }
            else
            {
                KST_Calls_Text = "Calls";
            }
            if (!KST_Calls_Text.Equals(this.lbl_KST_Calls.Text))
                this.lbl_KST_Calls.Text = KST_Calls_Text;
            if (this.lv_Msg.Items.Count > 0)
            {
                this.lbl_KST_Msg.Text = "Messages [" + this.lv_Msg.Items.Count.ToString() + "]";
            }
            else
            {
                this.lbl_KST_Msg.Text = "Messages";
            }
            if (this.lv_MyMsg.Items.Count > 0)
            {
                this.lbl_KST_MyMsg.Text = "My Messages [" + this.lv_MyMsg.Items.Count.ToString() + "]";
            }
            else
            {
                this.lbl_KST_MyMsg.Text = "My Messages";
            }

            set_KST_Status();

            this.ni_Main.Text = "wtKST\nLeft click to activate";
            if (!this.cb_Command.IsDisposed && !this.cb_Command.Focused && !this.btn_KST_Send.Capture)
            {
                this.cb_Command.Focus();
                this.cb_Command.SelectionLength = 0;
                this.cb_Command.SelectionStart = this.cb_Command.Text.Length;
            }
        }

        private void KST_Receive(string s)
        {
            MainDlg.KST_STATE kSTState = this.KSTState;
            switch (kSTState)
            {
            case MainDlg.KST_STATE.WaitUserName:
                if (s.IndexOf("Login:") >= 0)
                {
                    Thread.Sleep(100);
                    this.tw.Send(Settings.Default.KST_UserName + "\r");
                    this.KSTState = MainDlg.KST_STATE.WaitPassword;
                    this.Say("Login " + Settings.Default.KST_UserName + " send.");
                }
                break;
            case MainDlg.KST_STATE.WaitPassword:
                if (s.IndexOf("Password:") >= 0)
                {
                    Thread.Sleep(100);
                    this.tw.Send(Settings.Default.KST_Password + "\r");
                    this.KSTState = MainDlg.KST_STATE.WaitChat;
                    this.Say("Password send.");
                }
                break;
            case MainDlg.KST_STATE.WaitChat:
                if (s.IndexOf("Wrong password!") >= 0)
                {
                    Thread.Sleep(100);
                    this.Say("Password wrong.");
                    this.tw.Close();
                    MainDlg.Log.WriteMessage("Password wrong ");
                    break;
                }
                if (s.IndexOf("Your choice           :") >= 0)
                {
                    Thread.Sleep(100);
                    this.tw.Send(Settings.Default.KST_Chat.Substring(0, 1) + "\r");
                }
                if (s.IndexOf(">") > 0)
                {
                    if (Settings.Default.KST_StartAsHere)
                    {
                        this.tw.Send("/set here\r");
                    }
                    else
                    {
                        this.tw.Send("/unset here\r");
                    }
                    this.KSTState = MainDlg.KST_STATE.WaitAway;
                }
                break;
            case MainDlg.KST_STATE.WaitAway:
                if (s.IndexOf('>') <= 0)
                {
                    if (s.ToUpper().IndexOf("HERE SET") >= 0)
                    {
                        this.UserState = MainDlg.USER_STATE.Here;
                    }
                    else
                    {
                        this.UserState = MainDlg.USER_STATE.Away;
                    }
                }
                else
                {
                    this.KSTState = MainDlg.KST_STATE.WaitConfig;
                    Thread.Sleep(100);
                    this.tw.Send("/show config\r");
                }
                break;
            case MainDlg.KST_STATE.WaitConfig:
                if (s.IndexOf('>') <= 0)
                {
                    try
                    {
                        string call = s.Substring(0, s.IndexOf(" ")).ToUpper();
                        if (WCCheck.WCCheck.IsCall(call) >= 0)
                        {
                            string conf = s.Remove(0, s.IndexOf(" ") + 1);
                            Settings.Default.KST_Name = conf.Substring(0, conf.LastIndexOf(" "));
                            conf = conf.Remove(0, conf.LastIndexOf(" ") + 1);
                            Settings.Default.KST_Loc = conf;
                            if (WCCheck.WCCheck.IsLoc(Settings.Default.KST_Loc) > 0)
                            {
                                this.MyLoc = Settings.Default.KST_Loc;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MainDlg.Log.WriteMessage("Error reading user configuration: " + e.Message);
                    }
                }
                else
                {
                    this.KSTState = MainDlg.KST_STATE.Connected;
                    this.Say("Connected to KST chat.");
                    MainDlg.Log.WriteMessage("Connected to: " + Settings.Default.KST_Chat);
                    // get some historic messages
                    Thread.Sleep(100);
                    this.tw.Send("/sh msg 25\r");
                    this.msg_latest_first = true;
                    this.CheckStartUpAway = true;

                    this.ti_Main.Interval = 5000;
                    if (!this.ti_Main.Enabled)
                    {
                        this.ti_Main.Start();
                    }
                }
                break;
            case MainDlg.KST_STATE.WaitLogin:
                if (s.IndexOf("login") >= 0)
                {
                        // LOGINC|callsign|password|chat id|client software version|past messages number|past dx/map number|
                        // users list/update flags|last Unix timestamp for messages|last Unix timestamp for dx/map|

                        this.tw.Send("LOGINC|" + Settings.Default.KST_UserName + "|" + Settings.Default.KST_Password + "|"
                        + Settings.Default.KST_Chat.Substring(0, 1) + "|wtKST " + typeof(MainDlg).Assembly.GetName().Version +
                        "|25|0|1|" + 
                        // we try to get the messages up to our latest one
                        (!latestMessageTimestampSet ? "0" :
                        ((latestMessageTimestamp - new DateTime(1970, 1, 1)).TotalSeconds - 1).ToString() )
                        +  "|0|\r");
                    this.KSTState = MainDlg.KST_STATE.WaitLogstat;
                    this.Say("Login " + Settings.Default.KST_UserName + " send.");
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
                                    this.Say("Password wrong.");
                                    this.tw.Close();
                                    MainDlg.Log.WriteMessage("Password wrong ");
                                    break;
                                }
                                Settings.Default.KST_Name = subs[6];
                                Settings.Default.KST_Loc = subs[8];
                                if (WCCheck.WCCheck.IsLoc(Settings.Default.KST_Loc) > 0)
                                {
                                    this.MyLoc = Settings.Default.KST_Loc;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MainDlg.Log.WriteMessage("Error reading user configuration: " + e.Message);
                        }
                        // To set/reset/PRAU only propagation reception frames
                        // SPR | value |

                        this.tw.Send("SPR|2|\r");
                        this.KSTState = MainDlg.KST_STATE.WaitSPR;
                    this.Say("LOGSTAT " + s);
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

                        this.tw.Send("SDONE|" + Settings.Default.KST_Chat.Substring(0, 1) + "|\r");
                        this.KSTState = MainDlg.KST_STATE.Connected;
                        this.Say("Connected to KST chat.");
                        MainDlg.Log.WriteMessage("Connected to: " + Settings.Default.KST_Chat);
                        this.msg_latest_first = true;
                        this.CheckStartUpAway = true;

                        this.ti_Linkcheck.Stop();   // restart the linkcheck timer
                        this.ti_Linkcheck.Start();
                    }
                    break;
            default:
                switch (kSTState)
                {
                case MainDlg.KST_STATE.Connected:
                    if (this.KST_Use_New_Feed)
                    {
                        if(s.Substring(0,1).Equals("C"))
                        {
                            KST_Receive_MSG2(s);
                        }
                        else if (s.Substring(0, 1).Equals("U"))
                        {
                            KST_Receive_USR2(s);
                        }
                    }
                    else
                        this.KST_Receive_MSG(s);
                    break;
                case MainDlg.KST_STATE.WaitUser:
                    this.KST_Receive_USR(s);
                    break;
                }
                break;
            }
        }

        private void KST_Receive_MSG(string s)
        {
            string msg = s;
            s = s.Replace("(0)", "");
            this.MyCall = Settings.Default.KST_UserName.ToUpper();
            if (s.IndexOf("> ") > 0)
            {
                try
                {
                    if (s.EndsWith("\r\n"))
                    {
                        s = s.Substring(0, s.Length - 2);
                    }
                    if (!s.EndsWith(">"))
                    {
                        MainDlg.Log.WriteMessage("KST message: " + msg);
                        string[] header = s.Substring(0, s.IndexOf("> ")).Split(new char[] { ' ' });
                        DataRow Row = this.MSG.NewRow();
                        string time = header[0].Trim();
                        time = time.Substring(0, 2) + ":" + time.Substring(2, 2);
                        DateTime dt = DateTime.Parse(time);
                        Row["TIME"] = dt;
                        Row["CALL"] = header[1].Trim();
                        Row["NAME"] = header[2].Trim();
                        Row["MSG"] = s.Remove(0, s.IndexOf("> ") + 2).Trim();
                        var msg_upper = Row["MSG"].ToString().ToUpper();
                        if (msg_upper.IndexOf(' ') > 0)
                            msg_upper = msg_upper.Remove(msg_upper.IndexOf(' '));
                        Row["RECIPIENT"] = msg_upper.TrimStart(new char[] { '(' }).TrimEnd(new char[] { ')' });
                        KST_Process_new_message(Row);
                    }
                }
                catch (Exception e)
                {
                    this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
                }
            }
        }

        private void KST_Receive_MSG2(string s)
        {
            string[] msg = s.Split('|'); ;
            this.MyCall = Settings.Default.KST_UserName.ToUpper();
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
                        DataRow Row = this.MSG.NewRow();
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
                        this.msg_latest_first = (s.Substring(0, 2)).Equals("CR");
                        KST_Process_new_message(Row);
                        break;

                        // CK: link check
                        // CK |

                    case "CK": // Link Check
                        this.tw.Send("\r\n"); // need to reply
                        break;

                        // End of CR frames 
                    case "CE":
                        this.latestMessageTimestampSet = true;
                        break;
                }
            }
            catch (Exception e)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, "(" + s + "): " + e.Message);
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
                this.MSG.Rows.Add(Row);

                DateTime dt = (DateTime)Row["TIME"];

                DataRow findrow = this.QRV.Rows.Find(Row["CALL"].ToString().Trim());
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
                    LV.SubItems[i].Name = this.lv_Msg.Columns[i].Text;
                }
                int current_index = 0;
                if (this.lv_Msg.Items.Count == 0)
                {
                    // first entry
                    this.lv_Msg.Items.Insert(0, LV);
                    this.lv_Msg.TopItem = this.lv_Msg.Items[0];
                }
                else
                {
                    ListViewItem topitem = this.lv_Msg.TopItem;
                    Boolean AtBeginningOfList = (this.lv_Msg.TopItem == this.lv_Msg.Items[0]);
                        // display at beginning of list (so newest entry displayed)

                    DateTime dt_top = (DateTime)this.lv_Msg.Items[0].Tag;
                    if (!this.KST_Use_New_Feed && this.msg_latest_first && dt_top < dt)
                    {
                        this.msg_latest_first = false;
                        MainDlg.Log.WriteMessage("msg_oldest_first = false " + dt_top.ToShortTimeString() + " dt " + dt.ToShortTimeString() + " 1 " + Row["CALL"].ToString());
                    }

                    if (this.msg_latest_first)
                    {
                        if (this.KST_Use_New_Feed && this.latestMessageTimestampSet)
                        {
                            // reconnect, sort CR at begining
                            // probably better to re-generate the whole list... alternate line highlighting will not work
                            // not great, but ok for the time being...
                            for (int i=0; i< this.lv_Msg.Items.Count; i++ )
                            {
                                if (dt > (DateTime)this.lv_Msg.Items[i].Tag)
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
                            this.lv_Msg.Items.Insert(this.lv_Msg.Items.Count, LV);
                            current_index = this.lv_Msg.Items.Count - 1;
                        }
                    }
                    else
                    {
                        this.lv_Msg.Items.Insert(0, LV);
                    }
                    if (AtBeginningOfList)
                        this.lv_Msg.TopItem = this.lv_Msg.Items[0];
                    else
                    {
                        if (topitem != null)
                        {
                            this.lv_Msg.TopItem = topitem;
                        }
                        if (!this.ti_Top.Enabled)
                        {
                            this.ti_Top.Start();
                        }
                    }
                }

                if (this.lv_Msg.Items.Count % 2 == 0)
                    this.lv_Msg.Items[current_index].BackColor = Color.FromArgb(16777200);
                else
                    this.lv_Msg.Items[current_index].BackColor = Color.FromArgb(11516905);

                if (Row["RECIPIENT"].ToString().Equals(this.MyCall))
                {
                    this.lv_Msg.Items[current_index].BackColor = Color.FromArgb(16745026);
                }
                // check the recipient of the message
                findrow = this.CALL.Rows.Find(Row["RECIPIENT"].ToString());
                if (findrow != null)
                {
                    findrow["CONTACTED"] = (int)findrow["CONTACTED"] + 1;
                }
                findrow = this.CALL.Rows.Find(Row["CALL"].ToString().Trim());
                if (findrow != null)
                {
                    findrow["CONTACTED"] = 0; // clear counter on activity
                }
                if (Row["MSG"].ToString().ToUpper().StartsWith("(" + this.MyCall + ")") || Row["MSG"].ToString().ToUpper().StartsWith(this.MyCall))
                {
                    ListViewItem MyLV = new ListViewItem();
                    if (Row["MSG"].ToString().ToUpper().StartsWith("(" + this.MyCall + ")"))
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
                        MyLV.SubItems[i].Name = this.lv_MyMsg.Columns[i].Text;
                    }
                    this.lv_MyMsg.Items.Insert(0, MyLV);
                    this.lv_MyMsg.Items[0].EnsureVisible();
                    int hwnd = MainDlg.GetForegroundWindow();
                    if (hwnd != base.Handle.ToInt32())
                    {
                        if (Settings.Default.KST_ShowBalloon)
                        {
                            // TODO: statt MSG war das s... s.Remove(0, s.IndexOf("> ") + 2).Trim();
                            this.ni_Main.ShowBalloonTip(30000, "New MyMessage received", Row["MSG"].ToString(), ToolTipIcon.Info);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        private void KST_Update_USR_Window()
        {
            if (this.KSTState <= KST_STATE.WaitConfig)
                return;
            try
            {
                string topcall = "";
                if (this.lv_Calls.TopItem != null)
                {
                    topcall = this.lv_Calls.TopItem.Text;
                }
                this.lv_Calls.Items.Clear();
                this.lv_Calls.BeginUpdate();
                DataView view = this.CALL.DefaultView;
                if (this.sort_by_dir)
                    view.Sort = "DIR ASC";
                else
                    view.Sort = "CALL ASC";
                DataTable tbl = view.ToTable();

                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    // ignore own call
                    if (tbl.Rows[i]["CALL"].ToString().IndexOf(Settings.Default.KST_UserName.ToUpper()) >= 0)
                        continue;

                    ListViewItem LV = new ListViewItem();

                    if ((bool)tbl.Rows[i]["AWAY"])
                    {
                        if (this.hide_away)
                            continue;
                        LV.Text = "(" + tbl.Rows[i]["CALL"].ToString() + ")";
                        // Italic is too difficult to read and the font gets bigger
                        //LV.Font = new Font(LV.Font, FontStyle.Italic);
                    }
                    else
                        LV.Text = tbl.Rows[i]["CALL"].ToString();
                    // Ignore stations too far away
                    int MaxDist = Convert.ToInt32(Settings.Default.KST_MaxDist);
                    if (MaxDist !=0 && (int)tbl.Rows[i]["QRB"] > MaxDist)
                        continue;

                    // drop calls that are already in log
                    if (this.hide_worked && check_in_log(tbl.Rows[i]))
                        continue;

                    // login time - new calls should be bold
                    DateTime logintime = (DateTime)tbl.Rows[i]["LOGINTIME"];
                    double loggedOnMinutes = (DateTime.UtcNow.Subtract(logintime)).TotalMinutes;
                    if (loggedOnMinutes < 5)
                        LV.Font = new Font(LV.Font, FontStyle.Bold);

                    LV.UseItemStyleForSubItems = false;
                    LV.SubItems.Add(tbl.Rows[i]["NAME"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["LOC"].ToString());

                    // last activity
                    double lastActivityMinutes = (DateTime.UtcNow.Subtract((DateTime)tbl.Rows[i]["TIME"])).TotalMinutes;
                    //MainDlg.Log.WriteMessage("KST Time " + LV.Text + " " + DateTime.UtcNow + " " + (DateTime)tbl.Rows[i]["TIME"] + " " + lastActivityMinutes.ToString("0"));
                    if (lastActivityMinutes < 120.0)
                        LV.SubItems.Add(lastActivityMinutes.ToString("0"));
                    else
                    {
                        if (this.ignore_inactive)
                            continue;
                        if ((int)tbl.Rows[i]["CONTACTED"] < 3)
                            LV.SubItems.Add("---");
                        else
                            LV.SubItems.Add("xxx"); // if contacted 3 times without answer then probably really not available
                    }
                    int qrb = (int)tbl.Rows[i]["QRB"];
                    if (Settings.Default.AS_Active && qrb >= Convert.ToInt32(Settings.Default.AS_MinDist) && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                    {
                        LV.SubItems.Add(this.GetNearestPlanePotential(tbl.Rows[i]["CALL"].ToString()).ToString());
                    }
                    else
                    {
                        LV.SubItems.Add("0");
                    }
                    LV.SubItems.Add(tbl.Rows[i]["144M"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["432M"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["1_2G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["2_3G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["3_4G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["5_7G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["10G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["24G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["47G"].ToString());
                    LV.SubItems.Add(tbl.Rows[i]["76G"].ToString());
                    for (int j = 0; j < this.lv_Calls.Columns.Count; j++)
                    {
                        LV.SubItems[j].Name = this.lv_Calls.Columns[j].Text.Replace(".", "_");
                    }
                    this.lv_Calls.Items.Add(LV);
                }
                this.lv_Calls.EndUpdate();
                ListViewItem toplv = this.lv_Calls.FindItemWithText(topcall);
                if (toplv != null)
                {
                    this.lv_Calls.TopItem = this.lv_Calls.Items[this.lv_Calls.Items.Count - 1];
                    this.lv_Calls.TopItem = toplv;
                }
                if (!this.WinTestLocatorWarning)
                    this.Say("");
                MainDlg.Log.WriteMessage("KST GetUsers finished: " + this.lv_Calls.Items.Count.ToString() + " Calls.");
            }
            catch (Exception e)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, e.Message);
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
                            DataRow row = this.CALL.NewRow();
                            row["CALL"] = bs.Split(new char[] { ' ' })[0].Trim();
                            row["NAME"] = "Beacon";
                            row["LOC"] = bs.Split(new char[] { ' ' })[1].Trim();
                            row["TIME"] = DateTime.MinValue;
                            row["CONTACTED"] = 0;
                            row["144M"] = 0;
                            row["432M"] = 0;
                            row["1_2G"] = 0;
                            row["2_3G"] = 0;
                            row["3_4G"] = 0;
                            row["5_7G"] = 0;
                            row["10G"] = 0;
                            row["24G"] = 0;
                            row["47G"] = 0;
                            row["76G"] = 0;
                            this.CALL.Rows.Add(row);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, e.Message);
            }
        }

        private void KST_Process_QRV(DataRow row, string qrvcall, bool call_new_in_userlist=false)
        {
            DataRow findrow = this.QRV.Rows.Find(qrvcall);
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
                row["144M"] = findrow["144M"];
                row["432M"] = findrow["432M"];
                row["1_2G"] = findrow["1_2G"];
                row["2_3G"] = findrow["2_3G"];
                row["3_4G"] = findrow["3_4G"];
                row["5_7G"] = findrow["5_7G"];
                row["10G"] = findrow["10G"];
                row["24G"] = findrow["24G"];
                row["47G"] = findrow["47G"];
                row["76G"] = findrow["76G"];
            }
            else
            {
                if (call_new_in_userlist)
                    row["TIME"] = row["LOGINTIME"];
                else
                    row["TIME"] = DateTime.MinValue;
                row["144M"] = 0;
                row["432M"] = 0;
                row["1_2G"] = 0;
                row["2_3G"] = 0;
                row["3_4G"] = 0;
                row["5_7G"] = 0;
                row["10G"] = 0;
                row["24G"] = 0;
                row["47G"] = 0;
                row["76G"] = 0;
                DataRow newrow = this.QRV.NewRow();
                newrow["CALL"] = qrvcall;
                if (call_new_in_userlist)
                    newrow["TIME"] = row["LOGINTIME"];
                else
                    newrow["TIME"] = DateTime.MinValue;
                newrow["144M"] = 0;
                newrow["432M"] = 0;
                newrow["1_2G"] = 0;
                newrow["2_3G"] = 0;
                newrow["3_4G"] = 0;
                newrow["5_7G"] = 0;
                newrow["10G"] = 0;
                newrow["24G"] = 0;
                newrow["47G"] = 0;
                newrow["76G"] = 0;
                try
                {
                    this.QRV.Rows.Add(newrow);
                }
                catch (Exception e)
                {
                    this.Error(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        private void KST_Process_MyCallAway()
        {
            DataRow row = this.CALL.Rows.Find(this.MyCall);
            if (row != null)
            {
                if ( ((bool)row["AWAY"] == true && this.UserState == MainDlg.USER_STATE.Here) ||
                     ((bool)row["AWAY"] == false && this.UserState == MainDlg.USER_STATE.Away) )
                {
                    this.UserState = (bool)row["AWAY"] ? MainDlg.USER_STATE.Away : MainDlg.USER_STATE.Here;
                }
            }
        }

        private void KST_Receive_USR(string s)
        {
            if (char.IsDigit(s, 0) && char.IsDigit(s, 1) && char.IsDigit(s, 2) && char.IsDigit(s, 3) && s[4] == 'Z' && s.IndexOf(Settings.Default.KST_UserName.ToUpper()) != 6)
            {
                this.KST_Receive_MSG(s);
            }
            else
            {
                string msg = s;
                this.Say("Getting KST-Users...");
                try
                {
                    if (s.EndsWith("chat>"))
                    {
                        if (Settings.Default.WinTest_Activate)
                        {
                            this.Get_QSOs();
                        }
                        KST_Process_MyCallAway();
                        KST_Update_USR_Window();
                        this.lv_Calls_Updating = false;
                        this.KSTState = MainDlg.KST_STATE.Connected;
                    }
                    else
                    {
                        if (!this.lv_Calls_Updating)
                        {
                            this.oldCALL = this.CALL.Copy(); // keep old CALL
                            this.CALL.Clear();
                            if (Settings.Default.ShowBeacons)
                                KST_Add_Beacons_USR();
                        }
                        this.lv_Calls_Updating = true;
                        s = s.Replace("\r\n", "");
                        string call = s.Substring(0, s.IndexOf(' '));
                        s = s.Remove(0, call.Length);
                        s = s.Trim();
                        string loc = s.Substring(0, s.IndexOf(' '));
                        s = s.Remove(0, loc.Length);
                        s = s.Trim();
                        string name = s;
                        try
                        {
                            DataRow row = this.CALL.NewRow();
                            row["CALL"] = call;
                            row["NAME"] = name;
                            row["LOC"] = loc;
                            if (WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                int qrb = WCCheck.WCCheck.QRB(this.MyLoc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(this.MyLoc, loc);
                                row["QRB"] = qrb;
                                row["DIR"] = qtf;
                            }
                            string qrvcall = call.TrimStart(new char[]{ '(' }).TrimEnd(new char[]{ ')' });
                            if (qrvcall.Equals(call))
                                row["AWAY"] = false;
                            else
                            {
                                row["AWAY"] = true;
                                call = qrvcall;
                                row["CALL"] = qrvcall;
                            }
                            if (qrvcall.IndexOf("-") > 0)
                            {
                                qrvcall = qrvcall.Remove(qrvcall.IndexOf("-"));
                            }
                            bool call_new_in_userlist = false;
                            DataRow oldcallrow = this.oldCALL.Rows.Find(call);
                            if (oldcallrow == null)
                            {
                                if (this.oldCALL.Rows.Count > 0)
                                {
                                    call_new_in_userlist = true;
                                    row["LOGINTIME"] = DateTime.UtcNow; // remember time of login
                                }
                                else
                                {
                                    row["LOGINTIME"] = DateTime.MinValue;
                                }
                                row["CONTACTED"] = 0;
                            }
                            else
                            {
                                row["LOGINTIME"] = oldcallrow["LOGINTIME"];
                                row["CONTACTED"] = oldcallrow["CONTACTED"];
                            }

                            KST_Process_QRV(row, qrvcall, call_new_in_userlist);

                            if (WCCheck.WCCheck.IsCall(WCCheck.WCCheck.Cut(call)) >= 0 && WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                this.CALL.Rows.Add(row);
                            }
                        }
                        catch (Exception e)
                        {
                            this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
                    this.KSTState = MainDlg.KST_STATE.Connected;
                    this.lv_Calls_Updating = false;
                }
            }
        }

        private void KST_Receive_USR2(string s)
        {
            string[] usr = s.Split('|'); ;
            try
            {
                MainDlg.Log.WriteMessage("KST user: " + s);
                DataRow Row = this.MSG.NewRow();
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
                            DataRow row = this.CALL.NewRow();

                            string call = usr[2].Trim();
                            string loc = usr[4].Trim();
                            if (WCCheck.WCCheck.IsCall(WCCheck.WCCheck.Cut(call)) >= 0 && WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                row["CALL"] = call;
                                row["NAME"] = usr[3].Trim();
                                row["LOC"] = loc;

                                int qrb = WCCheck.WCCheck.QRB(this.MyLoc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(this.MyLoc, loc);
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

                                this.CALL.Rows.Add(row);
                            }
                        }
                        break;

                        //User already logged
                        //UM3 | chat id | callsign | firstname | locator | state |

                    case "UM":
                        {
                            string call = usr[2].Trim();
                            DataRow row = this.CALL.Rows.Find(call);
                            string loc = usr[4].Trim();

                            if (WCCheck.WCCheck.IsLoc(loc) >= 0)
                            {
                                row["CALL"] = call;
                                row["NAME"] = usr[3].Trim();
                                row["LOC"] = loc;

                                int qrb = WCCheck.WCCheck.QRB(this.MyLoc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(this.MyLoc, loc);
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
                            DataRow row = this.CALL.Rows.Find(call);
                            Int32 usr_state = Int32.Parse(usr[3]);
                            row["AWAY"] = (usr_state & 1) == 1;
                        }
                        break;

                        // User disconnected (to remove)
                        // UR6 | chat id | callsign |

                    case "UR":
                        {
                            string call = usr[2].Trim();
                            DataRow row = this.CALL.Rows.Find(call);
                            row.Delete();
                        }
                        break;

                        // Users statistics/end of users frames
                        // UE | chat id | nb registered users|
                    case "UE":
                        KST_Process_MyCallAway();
                        if (this.CheckStartUpAway)
                        {
                            if (Settings.Default.KST_StartAsHere && (this.UserState != MainDlg.USER_STATE.Here))
                                KST_Here();
                            else if (!Settings.Default.KST_StartAsHere && (this.UserState == MainDlg.USER_STATE.Here))
                                KST_Away();
                            this.CheckStartUpAway = false;
                        }
                        KST_Update_USR_Window();
                        // TODO wt?
                        break;
                }
            }
            catch (Exception e)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, "(" + s + "): " + e.Message);
            }
        }

        private void KST_Connect()
        {
            if (this.KSTState == MainDlg.KST_STATE.Standby &&
                !string.IsNullOrEmpty(Settings.Default.KST_ServerName) &&
                !string.IsNullOrEmpty(Settings.Default.KST_UserName))
            {
                this.tw = new TelnetWrapper();
                this.tw.DataAvailable += new DataAvailableEventHandler(this.tw_DataAvailable);
                this.tw.Disconnected += new DisconnectedEventHandler(this.tw_Disconnected);
                try
                {
                    this.tw.Connect(Settings.Default.KST_ServerName, Convert.ToInt32(Settings.Default.KST_ServerPort));
                    this.tw.Receive();
                    if (Settings.Default.KST_ServerPort.Equals("23001"))
                    {
                        this.KSTState = MainDlg.KST_STATE.WaitLogin;
                        this.KST_Use_New_Feed = true;
                        this.CALL.Clear();
                        if (Settings.Default.ShowBeacons)
                            KST_Add_Beacons_USR();
                        this.ti_Main.Interval = 5000;
                        if (!this.ti_Main.Enabled)
                        {
                            this.ti_Main.Start();
                        }
                    }
                    else
                    {
                        this.KSTState = MainDlg.KST_STATE.WaitUserName;
                        this.KST_Use_New_Feed = false;
                    }
                    this.Say("Connecting to KST chat..." + Settings.Default.KST_ServerName + " Port "+ Settings.Default.KST_ServerPort);
                }
                catch (Exception e)
                {
                    this.Error(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        private void KST_Disconnect()
        {
            if (this.KSTState >= MainDlg.KST_STATE.Connected)
            {
                this.tw.Send("/q\r");
                this.Say("Disconnected from KST chat...");
                this.KSTState = MainDlg.KST_STATE.Disconnecting;
            }
        }

        private void KST_Here()
        {
            if (this.KSTState >= MainDlg.KST_STATE.Connected)
            {
                if (this.KST_Use_New_Feed)
                    this.tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|/BACK|0|\r");
                else
                    this.tw.Send("/set here\r");
                this.UserState = MainDlg.USER_STATE.Here;
            }
        }

        private void KST_Away()
        {
            if (this.KSTState >= MainDlg.KST_STATE.Connected)
            {
                if (this.KST_Use_New_Feed)
                    this.tw.Send("MSG|" + Settings.Default.KST_Chat.Substring(0, 1)+"|0|/AWAY|0|\r");
                else
                    this.tw.Send("/unset here\r");
                this.UserState = MainDlg.USER_STATE.Away;
            }
        }

        private void KST_Send()
        {
            if (this.tw.Connected && this.cb_Command.Text.Length > 0)
            {
                if (!this.cb_Command.Text.StartsWith("/") || this.cb_Command.Text.ToUpper().StartsWith("/CQ"))
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
                        if (this.KST_Use_New_Feed)
                            to_sent = "MSG|" + Settings.Default.KST_Chat.Substring(0, 1) + "|0|" + to_sent + "|0|\r";
                        else
                            to_sent = to_sent + "\r";
                        tw.Send(to_sent);
                        MainDlg.Log.WriteMessage("KST message send: " + this.cb_Command.Text);
                        if (this.cb_Command.FindStringExact(this.cb_Command.Text) != 0)
                        {
                            this.cb_Command.Items.Insert(0, this.cb_Command.Text);
                        }
                    }
                    catch (Exception e)
                    {
                        this.Error(MethodBase.GetCurrentMethod().Name, e.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Sending commands except /cq is not allowed!", "KST SendCommand");
                }
                this.cb_Command.ResetText();
            }
        }

        private void KST_GetUsers()
        {
            if (this.KSTState == MainDlg.KST_STATE.Connected)
            {
                this.tw.Send("/sh user\r");
                this.KSTState = MainDlg.KST_STATE.WaitUser;
            }
        }

        private void tsi_File_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                this.KST_Disconnect();
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
                this.columnHeader144.Width = COLUMN_WIDTH;
            else
                this.columnHeader144.Width = 0;
            if (Settings.Default.Band_432)
                this.columnHeader432.Width = COLUMN_WIDTH;
            else
                this.columnHeader432.Width = 0;
            if (Settings.Default.Band_1296)
                this.columnHeader1296.Width = COLUMN_WIDTH;
            else
                this.columnHeader1296.Width = 0;
            if (Settings.Default.Band_2320)
                this.columnHeader2320.Width = COLUMN_WIDTH;
            else
                this.columnHeader2320.Width = 0;
            if (Settings.Default.Band_3400)
                this.columnHeader3400.Width = COLUMN_WIDTH;
            else
                this.columnHeader3400.Width = 0;
            if (Settings.Default.Band_5760)
                this.columnHeader5760.Width = COLUMN_WIDTH;
            else
                this.columnHeader5760.Width = 0;
            if (Settings.Default.Band_10368)
                this.columnHeader10368.Width = COLUMN_WIDTH;
            else
                this.columnHeader10368.Width = 0;
            if (Settings.Default.Band_24GHz)
                this.columnHeader24GHz.Width = COLUMN_WIDTH;
            else
                this.columnHeader24GHz.Width = 0;
            if (Settings.Default.Band_47GHz)
                this.columnHeader47GHz.Width = COLUMN_WIDTH;
            else
                this.columnHeader47GHz.Width = 0;
            if (Settings.Default.Band_76GHz)
                this.columnHeader76GHz.Width = COLUMN_WIDTH;
            else
                this.columnHeader76GHz.Width = 0;
        }

        bool check_in_log(DataRow row)
        {
            if (Settings.Default.Band_144 && (int)row["144M"] != 2)
                return false;
            if (Settings.Default.Band_432 && (int)row["432M"] != 2)
                return false;
            if (Settings.Default.Band_1296 && (int)row["1_2G"] != 2)
                return false;
            if (Settings.Default.Band_2320 && (int)row["2_3G"] != 2)
                return false;
            if (Settings.Default.Band_3400 && (int)row["3_4G"] != 2)
                return false;
            if (Settings.Default.Band_5760 && (int)row["5_7G"] != 2)
                return false;
            if (Settings.Default.Band_10368 && (int)row["10G"] != 2)
                return false;
            if (Settings.Default.Band_24GHz && (int)row["24G"] != 2)
                return false;
            if (Settings.Default.Band_47GHz && (int)row["47G"] != 2)
                return false;
            if (Settings.Default.Band_76GHz && (int)row["76G"] != 2)
                return false;
            return true;
        }

        private void tsi_Options_Click(object sender, EventArgs e)
        {
            OptionsDlg Dlg = new OptionsDlg();
            Dlg.cbb_KST_Chat.SelectedIndex = 2;
            if (this.KSTState != MainDlg.KST_STATE.Standby)
            {
                Dlg.tb_KST_Password.Enabled = false;
                Dlg.tb_KST_ServerName.Enabled = false;
                Dlg.tb_KST_ServerPort.Enabled = false;
                Dlg.tb_KST_UserName.Enabled = false;
                Dlg.cbb_KST_Chat.Enabled = false;
            }
            string oldchat = Settings.Default.KST_Chat;
            int KST_MaxDist = Convert.ToInt32(Settings.Default.KST_MaxDist);
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.Save();
                Settings.Default.Reload();
                if (oldchat != Settings.Default.KST_Chat)
                {
                    this.CALL.Clear();
                    this.lv_Calls.Items.Clear();
                    this.lv_Msg.Items.Clear();
                    this.lv_MyMsg.Items.Clear();
                }
                UpdateUserBandsWidth();
                set_KST_Status();
                if (KST_MaxDist != Convert.ToInt32(Settings.Default.KST_MaxDist))
                    KST_Update_USR_Window();
            }
        }

        private void tsi_KST_Connect_Click(object sender, EventArgs e)
        {
            this.KST_Connect();
        }

        private void tsi_KST_Disconnect_Click(object sender, EventArgs e)
        {
            if (Settings.Default.KST_AutoConnect && MessageBox.Show("The KSTAutoConnect function is on. You will be reconnected automatically after 30secs. Do you want to switch this function off?", "KSTAutoConnect", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Settings.Default.KST_AutoConnect = false;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            this.KST_Disconnect();
        }

        private void tsi_KST_Here_Click(object sender, EventArgs e)
        {
            this.KST_Here();
        }

        private void tsi_KST_Away_Click(object sender, EventArgs e)
        {
            this.KST_Away();
        }

        private void btn_KST_Send_Click(object sender, EventArgs e)
        {
            this.KST_Send();
        }

        private void Get_QSOs()
        {
            WinTestLocatorWarning = false;
            string kstcall = WCCheck.WCCheck.Cut(Settings.Default.KST_UserName.ToUpper());
            if (kstcall.IndexOf("DL0GTH") >= 0 || kstcall.IndexOf("DL2ALF") >= 0 || kstcall.IndexOf("DL2ARD") >= 0 || kstcall.IndexOf("DL2AKT") >= 0 || kstcall.IndexOf("DM5CT") >= 0 || kstcall.IndexOf("DL6AUI") >= 0 || kstcall.IndexOf("DR9A") >= 0 || kstcall.IndexOf("DA0FF") >= 0 || kstcall.IndexOf("DL8AAU") >= 0 || kstcall.IndexOf("DL0FTZ") >= 0)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(Settings.Default.WinTest_INI_FileName, Encoding.Default))
                    {
                        string S = sr.ReadToEnd();
                        S = S.Remove(0, S.IndexOf("[Files]") + 7);
                        S = S.Remove(0, S.IndexOf("Path=") + 5);
                        S = S.Remove(S.IndexOf("\r"), S.Length - S.IndexOf("\r"));
                        S = S.Trim();
                        string OldFilename = Settings.Default.WinTest_FileName;
                        if (OldFilename != S)
                        {
                            Settings.Default.WinTest_FileName = S;
                            Settings.Default.Save();
                            Settings.Default.Reload();
                            MainDlg.Log.WriteMessage("Using Win-Test log " + Settings.Default.WinTest_FileName);
                        }
                    }
                    using (Stream stream = File.Open(Settings.Default.WinTest_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        this.QSO.Clear();
                        byte[] bufh = new byte[13944];
                        stream.Read(bufh, 0, bufh.Length);
                        string wtLoc = Encoding.ASCII.GetString(bufh, 24, 6);
                        char[] separator = new char[1];
                        this.MyLoc = wtLoc.Split(separator)[0];
                        if (!this.MyLoc.Equals(Settings.Default.KST_Loc))
                            set_KST_Status();
                        stream.Position = 13944L;
                        while (stream.Position < stream.Length)
                        {
                            byte[] buf = new byte[544];
                            stream.Read(buf, 0, buf.Length);
                            int utime = BitConverter.ToInt32(buf, 24);
                            DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            ts = ts.AddSeconds((double)utime);
                            byte band = buf[4];
                            string call = Encoding.ASCII.GetString(buf, 32, 14);
                            call = call.ToString().Replace("\0", "");

                            DataRow row = this.QSO.NewRow();
                            row["CALL"] = call;
                            switch (band)
                            {
                            case 12:
                                row["BAND"] = "144M";
                                break;
                            case 14:
                                row["BAND"] = "432M";
                                break;
                            case 16:
                                row["BAND"] = "1_2G";
                                break;
                            case 17:
                                row["BAND"] = "2_3G";
                                break;
                            case 18:
                                row["BAND"] = "3_4G";
                                break;
                            case 19:
                                row["BAND"] = "5_7G";
                                break;
                            case 20:
                                row["BAND"] = "10G";
                                break;
                            case 21:
                                row["BAND"] = "24G";
                                break;
                            case 22:
                                row["BAND"] = "47G";
                                break;
                            case 23:
                                row["BAND"] = "76G";
                                break;
                            case 13:
                            case 15:
                            default:
                                row["BAND"] = "";
                                break;
                            }

                            row["TIME"] = ts.ToString("HH:mm");
                            string s = BitConverter.ToInt16(buf, 0).ToString();
                            s = Encoding.ASCII.GetString(buf, 46, 4).Replace("\0", "") + s.PadLeft(3, '0');
                            row["SENT"] = s;
                            switch (buf[8])
                            {
                            case 0:
                                row["RCVD"] = Encoding.ASCII.GetString(buf, 50, 3).Replace("\0", "") + Encoding.ASCII.GetString(buf, 53, 4).Replace("\0", "");
                                break;
                            case 1:
                                row["RCVD"] = Encoding.ASCII.GetString(buf, 50, 2).Replace("\0", "") + Encoding.ASCII.GetString(buf, 52, 4).Replace("\0", "");
                                break;
                            }
                            row["LOC"] = Encoding.ASCII.GetString(buf, 61, 6).Replace("\0", "");
                            try
                            {
                                if (this.QSO.Rows.Find(new string[]
                                {
                                    row["CALL"].ToString(),
                                    row["BAND"].ToString()
                                }) == null)
                                {
                                    this.QSO.Rows.Add(row);
                                }
                            }
                            catch (Exception e)
                            {
                                this.Error(MethodBase.GetCurrentMethod().Name, "(" + row["CALL"].ToString() + "): " + e.Message);
                            }
                            for (int i = 0; i < this.CALL.Rows.Count; i++)
                            {
                                string findcall = this.CALL.Rows[i]["CALL"].ToString();
                                findcall = findcall.TrimStart(new char[] {'('}).TrimEnd(new char[]{')'});
                                if (findcall.IndexOf("-") > 0)
                                {
                                    findcall = findcall.Remove(findcall.IndexOf("-"));
                                }
                                if (findcall == call)
                                {
                                    switch (band)
                                    {
                                    case 12:
                                        this.CALL.Rows[i]["144M"] = 2;
                                        break;
                                    case 14:
                                        this.CALL.Rows[i]["432M"] = 2;
                                        break;
                                    case 16:
                                        this.CALL.Rows[i]["1_2G"] = 2;
                                        break;
                                    case 17:
                                        this.CALL.Rows[i]["2_3G"] = 2;
                                        break;
                                    case 18:
                                        this.CALL.Rows[i]["3_4G"] = 2;
                                        break;
                                    case 19:
                                        this.CALL.Rows[i]["5_7G"] = 2;
                                        break;
                                    case 20:
                                        this.CALL.Rows[i]["10G"] = 2;
                                        break;
                                    case 21:
                                        this.CALL.Rows[i]["24G"] = 2;
                                        break;
                                    case 22:
                                        this.CALL.Rows[i]["47G"] = 2;
                                        break;
                                    case 23:
                                        this.CALL.Rows[i]["76G"] = 2;
                                        break;
                                    }
                                    // check locator
                                    if (CALL.Rows[i]["LOC"].ToString() != row["LOC"].ToString())
                                    {
                                        Say(call + " Locator wrong? Win-Test Log " + row["BAND"] + " " + row["TIME"] + " " + call + " " + row["LOC"] + " KST " + this.CALL.Rows[i]["LOC"].ToString());
                                        WinTestLocatorWarning = true;
                                        MainDlg.Log.WriteMessage("Win-Test log locator mismatch: " + row["BAND"] + " " + row["TIME"] +  " " + call + " Locator wrong? Win-Test Log " + row["LOC"] + " KST " + this.CALL.Rows[i]["LOC"].ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    this.Error(MethodBase.GetCurrentMethod().Name, "(" + Settings.Default.WinTest_FileName + "): " + e.Message);
                }
            }
            else
            {
                MessageBox.Show("Win-Test functionality is for DL0GTH crew only.\nAs " + kstcall + " is not a member, this option is disabled now.", "Function is restricted");
                Settings.Default.WinTest_Activate = false;
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
            if (this.planes.TryGetValue(call, out infolist))
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
            if (this.planes.TryGetValue(call, out infolist))
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
            this.ti_Main.Stop();
            if (this.KST_Use_New_Feed && this.KSTState == MainDlg.KST_STATE.Connected)
            {
                if (Settings.Default.WinTest_Activate)
                {
                    this.Get_QSOs();
                }
                KST_Update_USR_Window();
            }
            else
            {
                MainDlg.Log.WriteMessage("KST GetUsers start.");
                this.KST_GetUsers();
            }
            int interval = Convert.ToInt32(Settings.Default.UpdateInterval) * 1000;
            if (interval > 10000)
            {
                this.ti_Main.Interval = interval;
            }
            this.ti_Main.Start();
        }

        public void Say(string Text)
        {
            this.tsl_Info.Text = Text;
            this.ss_Main.Refresh();
            Application.DoEvents();
        }

        public void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
            this.tsl_Error.Text = "<" + methodname + "> " + Text;
            this.ss_Main.Refresh();
            Application.DoEvents();
            if (!this.ti_Error.Enabled)
            {
                this.ti_Error.Interval = 30000;
                this.ti_Error.Start();
            }
        }

        private void MainDlg_Load(object sender, EventArgs e)
        {
            // Set window location
            if (Settings.Default.WindowLocation != null)
            {
                this.Location = Settings.Default.WindowLocation;
            }

            // Set window size
            if (Settings.Default.WindowSize != null)
            {
                this.Size = Settings.Default.WindowSize;
            }
        }

        private void MainDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.bw_GetPlanes.CancelAsync();
                this.ni_Main.Visible = false; // hide NotificationIcon
                string FileName = Application.LocalUserAppDataPath + "\\" + Settings.Default.WinTest_QRV_Table_FileName;
                this.Say("Saving QRV-Database to " + FileName + "...");
                this.QRV.WriteXml(FileName, XmlWriteMode.IgnoreSchema);
            }
            catch (Exception e2)
            {
                this.Error(MethodBase.GetCurrentMethod().Name, "(qrv.xml): " + e2.Message);
            }
            // Copy window location to app settings
            Settings.Default.WindowLocation = this.Location;

            // Copy window size to app settings
            if (this.WindowState == FormWindowState.Normal)
            {
                Settings.Default.WindowSize = this.Size;
            }
            else
            {
                Settings.Default.WindowSize = this.RestoreBounds.Size;
            }

            // Save settings
            Settings.Default.Save();

            MainDlg.Log.WriteMessage("Closed down.");
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            this.ni_Main.Dispose(); //remove notification icon
        }

        private void ShowToolTip(string text, Control control, Point p)
        {
            int BorderWith = (base.Width - base.ClientSize.Width) / 2;
            int TitleBarHeight = base.Height - base.ClientSize.Height - 2 * BorderWith;
            p = control.PointToScreen(p);
            p = base.PointToClient(p);
            p.X += BorderWith;
            p.Y = p.Y + TitleBarHeight + this.Cursor.Size.Height;
            this.tt_Info.Show(text, this, p, 5000);
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
                if (e.ColumnIndex > 4 && this.hide_worked) // Band
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
                    if ((e.ColumnIndex == 0 && this.hide_away) ||   // CALL
                        (e.ColumnIndex == 2 && this.sort_by_dir) || // LOCATOR
                        (e.ColumnIndex == 3 && this.ignore_inactive)) // ACT
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
                int state = 0;
                try
                {
                    state = Convert.ToInt32(e.SubItem.Text);
                }
                catch
                {
                }
                switch (state)
                {
                case 0:
                    e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                    break;
                case 1:
                    e.Graphics.FillRectangle(Brushes.Red, e.Bounds);
                    break;
                case 2:
                    e.Graphics.FillRectangle(Brushes.Green, e.Bounds);
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
            ListViewHitTestInfo info = this.lv_Calls.HitTest(p);
            string ToolTipText = "";
            if (this.OldMousePos != p && info != null && info.SubItem != null)
            {
                this.OldMousePos = p;
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
                        WCCheck.WCCheck.QTF(this.MyLoc, info.Item.SubItems[2].Text).ToString("000"),
                        "°\nQRB:\t",
                        WCCheck.WCCheck.QRB(this.MyLoc, info.Item.SubItems[2].Text),
                        " km\n\nLeft click to\nSend Message."
                    });
                }
                if (info.SubItem.Name == "AS")
                {
                    string s = this.GetNearestPlanes(info.Item.Text);
                    if (string.IsNullOrEmpty(s))
                    {
                        ToolTipText = "No planes\n\nLeft click for map";
                        DataRow Row = this.CALL.Rows.Find(info.Item.Text);
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
                    if (info.SubItem.Text != "2")
                    {
                        ToolTipText = info.SubItem.Name.Replace("_", ".") + ": Left click to \ntoggle QRV info";
                    }
                    else
                    {
                        DataRow findrow = this.QSO.Rows.Find(new object[]
                        {
                            info.Item.Text,
                            info.SubItem.Name.Replace(".", "_")
                        });
                        if (findrow != null)
                        {
                            ToolTipText = string.Concat(new object[]
                            {
                                info.SubItem.Name.Replace("_", "."),
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
                this.ShowToolTip(ToolTipText, this.lv_Calls, p);
            }
        }

        private void cmi_Calls_SendMessage_Click(object sender, EventArgs e)
        {
            if (this.lv_Calls.SelectedItems != null && this.lv_Calls.SelectedItems[0].Text.Length > 0)
            {
                this.cb_Command.Text = "/cq " + this.lv_Calls.SelectedItems[0].Text + " ";
                this.cb_Command.Focus();
                this.cb_Command.SelectionStart = this.cb_Command.Text.Length;
                this.cb_Command.SelectionLength = 0;
            }
        }


        private void lv_Calls_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // CALL column
            if (e.Column == 0)
            {
                if (this.hide_away)
                    this.hide_away = false;
                else
                    this.hide_away = true;
                KST_Update_USR_Window();
            }

            // LOCATOR column
            if (e.Column == 2)
            {
                if (this.sort_by_dir)
                    this.sort_by_dir = false;
                else
                    this.sort_by_dir = true;
                KST_Update_USR_Window();
            }

            // ACT column
            if (e.Column == 3)
            {
                if (this.ignore_inactive)
                    this.ignore_inactive = false;
                else
                    this.ignore_inactive = true;
                KST_Update_USR_Window();
            }
            // band columns
            if (e.Column > 4)
            {
                if (this.hide_worked)
                    this.hide_worked = false;
                else
                    this.hide_worked = true;
                this.lv_Calls.Invalidate(true);
                KST_Update_USR_Window();
            }
        }

        private void lv_Calls_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = new Point(e.X, e.Y);
                int state = 0;
                DataRow Row = null;
                ListViewHitTestInfo info = this.lv_Calls.HitTest(p);
                if (info != null && info.SubItem != null)
                {
                    if (info.SubItem.Name == "Call" || info.SubItem.Name == "Name" || info.SubItem.Name == "Locator" || info.SubItem.Name == "UTC")
                    {
                        if (info.Item.Text.Length > 0 && this.KSTState == MainDlg.KST_STATE.Connected)
                        {
                            this.cb_Command.Text = "/cq " + info.Item.Text.Replace("(", "").Replace(")", "") + " ";
                            this.cb_Command.SelectionStart = this.cb_Command.Text.Length;
                            this.cb_Command.SelectionLength = 0;
                        }
                    }
                    if (info.SubItem.Name == "AS" && Settings.Default.AS_Active)
                    {
                        string call = WCCheck.WCCheck.Cut(info.Item.Text);
                        string loc = info.Item.SubItems[2].Text;
                        string qrg = AS_qrg_from_settings();
                        wtMessage Msg = new wtMessage(WTMESSAGES.ASSHOWPATH, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, string.Concat(new string[]
                        {
                            qrg,
                            ",",
                            WCCheck.WCCheck.Cut(this.MyCall),
                            ",",
                            this.MyLoc,
                            ",",
                            WCCheck.WCCheck.Cut(call),
                            ",",
                            loc
                        }));
                        UdpClient client = new UdpClient();
                        client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                        client.Client.ReceiveTimeout = 10000;
                        IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, Settings.Default.AS_Port);
                        client.Connect(groupEp);
                        byte[] b = Msg.ToBytes();
                        client.Send(b, b.Length);
                        client.Close();
                    }
                    if (info.SubItem.Name[0] > '0' && info.SubItem.Name[0] < '9')
                    {
                        state = 0;
                        try
                        {
                            state = Convert.ToInt32(info.SubItem.Text);
                        }
                        catch
                        {
                        }
                        switch (state)
                        {
                        case 0:
                            info.SubItem.Text = "1";
                            break;
                        case 1:
                            info.SubItem.Text = "0";
                            break;
                        }
                        Row = this.QRV.Rows.Find(info.Item.Text);
                        if (Row != null)
                        {
                            state = 0;
                            try
                            {
                                state = Convert.ToInt32(Row[info.SubItem.Name].ToString());
                            }
                            catch
                            {
                            }
                            switch (state)
                            {
                            case 0:
                                Row[info.SubItem.Name] = 1;
                                break;
                            case 1:
                                Row[info.SubItem.Name] = 0;
                                break;
                            }
                        }
                        Row = this.CALL.Rows.Find(info.Item.Text);
                        if (Row != null)
                        {
                            state = 0;
                            try
                            {
                                state = Convert.ToInt32(Row[info.SubItem.Name].ToString());
                            }
                            catch
                            {
                            }
                            switch (state)
                            {
                            case 0:
                                Row[info.SubItem.Name] = 1;
                                break;
                            case 1:
                                Row[info.SubItem.Name] = 0;
                                break;
                            }
                        }
                        this.lv_Calls.Refresh();
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                string ToolTipText = "";
                ListViewHitTestInfo info = this.lv_Calls.HitTest(p);
                if (info != null && info.SubItem != null)
                {
                    if (info.SubItem.Name == "AS" && Settings.Default.AS_Active)
                    {
                        string s = this.GetNearestPlanes(info.Item.Text);
                        if (string.IsNullOrEmpty(s))
                        {
                            ToolTipText = "No planes\n\nLeft click for map";
                            DataRow Row = this.CALL.Rows.Find(info.Item.Text);
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
                    this.ShowToolTip(ToolTipText, this.lv_Calls, p);
                }
            }
        }

        private void lv_Msg_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = this.lv_Msg.HitTest(p);
            if (info != null && info.Item != null)
            {
                if (info.Item.SubItems[1].Text.Length > 0 && this.KSTState == MainDlg.KST_STATE.Connected)
                {
                    this.cb_Command.Text = "/cq " + info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "") + " ";
                    this.cb_Command.SelectionStart = this.cb_Command.Text.Length;
                    this.cb_Command.SelectionLength = 0;
                }
            }
        }

        private void lv_MyMsg_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = this.lv_MyMsg.HitTest(p);
            if (info != null && info.Item != null)
            {
                if (info.Item.SubItems[1].Text.Length > 0 && this.KSTState == MainDlg.KST_STATE.Connected)
                {
                    this.cb_Command.Text = "/cq " + info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "") + " ";
                    this.cb_Command.SelectionStart = this.cb_Command.Text.Length;
                    this.cb_Command.SelectionLength = 0;
                }
            }
        }

        private void lv_Msg_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = this.lv_Msg.HitTest(p);
            if (this.OldMousePos != p && info != null && info.SubItem != null)
            {
                this.OldMousePos = p;
                if (info.SubItem.Name == "Messages")
                {
                    this.ShowToolTip(info.Item.SubItems[3].Text, this.lv_Msg, p);
                }
                else
                {
                    this.ShowToolTip("Left click to\nSend Message.", this.lv_Msg, p);
                }
            }
        }

        private void lv_MyMsg_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = this.lv_MyMsg.HitTest(p);
            if (this.OldMousePos != p && info != null && info.SubItem != null)
            {
                this.OldMousePos = p;
                this.ShowToolTip("Left click to\nSend Message.", this.lv_MyMsg, p);
            }
        }

        private void lv_Msg_Resize(object sender, EventArgs e)
        {
            int colwidth = 0;
            for (int i = 0; i < this.lv_Msg.Columns.Count - 1; i++)
            {
                colwidth += this.lv_Msg.Columns[i].Width;
            }
            this.lv_Msg.Columns[this.lv_Msg.Columns.Count - 1].Width = this.lv_Msg.Width - colwidth - 3;
        }

        private void lv_MyMsg_Resize(object sender, EventArgs e)
        {
            int colwidth = 0;
            for (int i = 0; i < this.lv_MyMsg.Columns.Count - 1; i++)
            {
                colwidth += this.lv_MyMsg.Columns[i].Width;
            }
            this.lv_MyMsg.Columns[this.lv_MyMsg.Columns.Count - 1].Width = this.lv_MyMsg.Width - colwidth - 3;
        }

        private void btn_KST_Send_Click_1(object sender, EventArgs e)
        {
            this.KST_Send();
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
            this.ti_Receive.Stop();
            DateTime t = DateTime.Now;
            while (true)
            {
                string s;
                lock(this.MsgQueue)
                {
                    if (this.MsgQueue.Count > 0)
                    {
                        s = this.MsgQueue.Dequeue();
                    }
                    else
                    {
                        s = "";
                    }
                }
                if (s.Length > 0)
                {
                    this.KST_Receive(s);
                }
                else
                    break;
            }
            TimeSpan ts = DateTime.Now - t;
            if (ts.Seconds > 1)
            {
                MainDlg.Log.WriteMessage("ReceiveTimer Overflow: " + ts.ToString());
            }
            this.ti_Receive.Start();
        }

        private void ti_Error_Tick(object sender, EventArgs e)
        {
            this.tsl_Error.Text = "";
            this.ti_Error.Stop();
        }

        private void cb_Command_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string LastCommand = this.cb_Command.Text;
                if (this.cb_Command.Text.ToUpper().StartsWith("/CQ ") && this.cb_Command.SelectedItem != null)
                {
                    LastCommand = LastCommand.Split(new char[] { ' ' })[0] + " " + LastCommand.Split(new char[] { ' ' })[1];
                    string NewCommand = (string)this.cb_Command.SelectedItem;
                    if (NewCommand.ToUpper().StartsWith("/CQ "))
                    {
                        NewCommand = NewCommand.Remove(0, 4);
                        NewCommand = NewCommand.Remove(0, NewCommand.IndexOf(" ") + 1);
                    }
                    NewCommand = LastCommand + " " + NewCommand;
                    if (this.cb_Command.FindStringExact(NewCommand) != 0)
                    {
                        this.cb_Command.Items.Insert(0, NewCommand);
                    }
                    this.cb_Command.SelectedIndex = 0;
                }
            }
            finally
            {
                this.lv_Calls.Focus();
            }
        }

        private void cb_Command_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!this.cb_Command.DroppedDown)
            {
                Point p = new Point(e.X, e.Y);
                p = this.cb_Command.PointToScreen(p);
                Point cp = this.lv_Calls.PointToClient(p);
                ListViewHitTestInfo info = this.lv_Calls.HitTest(cp);
                if (info != null && info.Item != null)
                {
                    int newindex = this.lv_Calls.TopItem.Index - Math.Sign(e.Delta);
                    if (newindex < 0)
                    {
                        newindex = 0;
                    }
                    if (newindex > this.lv_Calls.Items.Count)
                    {
                        newindex = this.lv_Calls.Items.Count;
                    }
                    this.lv_Calls.TopItem = this.lv_Calls.Items[newindex];
                    ((HandledMouseEventArgs)e).Handled = true;
                }
                else
                {
                    cp = this.lv_Msg.PointToClient(p);
                    info = this.lv_Msg.HitTest(cp);
                    if (info != null && info.Item != null)
                    {
                        int newindex = this.lv_Msg.TopItem.Index - Math.Sign(e.Delta);
                        if (newindex < 0)
                        {
                            newindex = 0;
                        }
                        if (newindex > this.lv_Msg.Items.Count)
                        {
                            newindex = this.lv_Msg.Items.Count;
                        }
                        this.lv_Msg.TopItem = this.lv_Msg.Items[newindex];
                        ((HandledMouseEventArgs)e).Handled = true;
                    }
                    else
                    {
                        cp = this.lv_Msg.PointToClient(p);
                        info = this.lv_MyMsg.HitTest(cp);
                        if (info != null && info.Item != null)
                        {
                            int newindex = this.lv_MyMsg.TopItem.Index - Math.Sign(e.Delta);
                            if (newindex < 0)
                            {
                                newindex = 0;
                            }
                            if (newindex > this.lv_MyMsg.Items.Count)
                            {
                                newindex = this.lv_MyMsg.Items.Count;
                            }
                            this.lv_MyMsg.TopItem = this.lv_MyMsg.Items[newindex];
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
            if (this.lv_Msg.Items.Count > 0)
            {
                this.lv_Msg.TopItem = this.lv_Msg.Items[0];
            }
            this.ti_Top.Stop();
        }

        private void ti_Reconnect_Tick(object sender, EventArgs e)
        {
            if (Settings.Default.KST_AutoConnect && this.KSTState == MainDlg.KST_STATE.Standby)
            {
                this.KST_Connect();
            }
        }

        private void ti_Linkcheck_Tick(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (this.KSTState == MainDlg.KST_STATE.Connected)
            {
                tw.Send("\r\n"); // send to check if link is still up
            }
        }

        private string AS_qrg_from_settings()
        {
            string qrg = "1440000";
            if (Settings.Default.AS_QRG == "432M")
            {
                qrg = "4320000";
            }
            else if (Settings.Default.AS_QRG == "1.2G")
            {
                qrg = "12960000";
            }
            else if (Settings.Default.AS_QRG == "2.3G")
            {
                qrg = "23200000";
            }
            else if (Settings.Default.AS_QRG == "3.4G")
            {
                qrg = "34000000";
            }
            else if (Settings.Default.AS_QRG == "5.7G")
            {
                qrg = "57600000";
            }
            else if (Settings.Default.AS_QRG == "10G")
            {
                qrg = "103680000";
            }
            return qrg;
        }

        private void bw_GetPlanes_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.bw_GetPlanes.CancellationPending)
            {
                if (this.KSTState < MainDlg.KST_STATE.Connected || !Settings.Default.AS_Active)
                {
                    Thread.Sleep(200); // TODO: better to use WaitHandles
                    continue;
                }
                int errors = 0;
                for (int i = 0; Settings.Default.AS_Active && i < this.CALL.Rows.Count; i++)
                {
                    try
                    {
                        int qrb = (int)this.CALL.Rows[i]["QRB"];
                        string mycall = WCCheck.WCCheck.Cut(this.MyCall);
                        string myloc = this.MyLoc;
                        string dxcall = WCCheck.WCCheck.Cut(this.CALL.Rows[i]["CALL"].ToString().TrimStart(
                            new char[]{ '(' }).TrimEnd(new char[]{ ')' }));
                        string dxloc = this.CALL.Rows[i]["LOC"].ToString();
                        string rxmycall = "";
                        string rxdxcall = "";
                        string qrg = AS_qrg_from_settings();
                        wtMessage Msg = new wtMessage(WTMESSAGES.ASSETPATH, Settings.Default.AS_My_Name, Settings.Default.AS_Server_Name, string.Concat(new string[]
                        {
                            qrg,
                            ",",
                            mycall,
                            ",",
                            myloc,
                            ",",
                            dxcall,
                            ",",
                            dxloc
                        }));
                        try
                        {
                            if (Settings.Default.AS_Active && qrb >= Convert.ToInt32(Settings.Default.AS_MinDist) && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                            {
                                UdpClient client = new UdpClient();
                                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                                client.Client.ReceiveTimeout = 10000;
                                IPEndPoint groupEp = new IPEndPoint(IPAddress.Broadcast, Settings.Default.AS_Port);
                                client.Connect(groupEp);
                                byte[] b = Msg.ToBytes();
                                client.Send(b, b.Length);
                                client.Close();
                                DateTime start = DateTime.UtcNow;
                                int rcvtimeout = 30;
                                try
                                {
                                    rcvtimeout = Convert.ToInt32(Settings.Default.AS_Timeout);
                                }
                                catch
                                {
                                    rcvtimeout = 30;
                                }
                                int elapsed;
                                bool match;
                                do
                                {
                                    IPEndPoint ep = new IPEndPoint(IPAddress.Any, Settings.Default.AS_Port);
                                    UdpClient u = new UdpClient();
                                    u.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                                    u.Client.ReceiveTimeout = 1000;
                                    u.Client.Bind(ep);
                                    try
                                    {
                                        byte[] data = u.Receive(ref ep);
                                        if (data.Length > 0)
                                        {
                                            string text = Encoding.ASCII.GetString(data);
                                            text = text.Remove(text.Length - 1);
                                            byte sum = 0;
                                            for (int j = 0; j < data.Length - 1; j++)
                                            {
                                                sum += data[j];
                                            }
                                            sum |= 128;
                                            Console.WriteLine(string.Concat(new object[]
                                            {
                                                ep.Address,
                                                " >> ",
                                                text,
                                                "[",
                                                data[data.Length - 1].ToString("X2"),
                                                "<>",
                                                sum.ToString("X2"),
                                                "]"
                                            }));
                                            Msg = new wtMessage(data);
                                            if (Msg.Msg == WTMESSAGES.ASNEAREST)
                                            {
                                                string[] a = Msg.Data.Split(new char[]{ ',' });
                                                rxmycall = a[1];
                                                rxdxcall = a[3];
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                    finally
                                    {
                                        if (u != null)
                                        {
                                            u.Close();
                                        }
                                    }
                                    elapsed = (DateTime.UtcNow - start).Seconds;
                                    match = (Msg.Dst == Settings.Default.AS_My_Name && Msg.Msg == WTMESSAGES.ASNEAREST && mycall == rxmycall && dxcall == rxdxcall);
                                    if (match)
                                    {
                                        this.bw_GetPlanes.ReportProgress(1, Msg);
                                    }
                                    Thread.Sleep(100);
                                }
                                while (!match && elapsed < rcvtimeout);
                                if (!match)
                                {
                                }
                                if (elapsed >= rcvtimeout)
                                {
                                    errors++;
                                    this.bw_GetPlanes.ReportProgress(0, dxcall);
                                    if (errors > 10)
                                    {
                                        this.bw_GetPlanes.ReportProgress(0, null);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e1_596)
                        {
                            this.bw_GetPlanes.ReportProgress(0, null);
                            this.bw_GetPlanes.ReportProgress(-1, Msg);
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
                if (Msg.Msg == WTMESSAGES.ASNEAREST)
                {
                    try
                    {
                        string[] a = Msg.Data.Split(new char[]{ ',' });
                        DateTime utc = Convert.ToDateTime(a[0]).ToUniversalTime();
                        string mycall = a[1];
                        string myloc = a[2];
                        string dxcall = a[3];
                        string dxloc = a[4];
                        int planecount = Convert.ToInt32(a[5]);
                        PlaneInfoList infolist = new PlaneInfoList();
                        infolist.UTC = utc;
                        for (int i = 0; i < planecount; i++)
                        {
                            PlaneInfo info = new PlaneInfo(a[6 + i * 5], a[7 + i * 5], Convert.ToInt32(a[8 + i * 5]), Convert.ToInt32(a[9 + i * 5]), Convert.ToInt32(a[10 + i * 5]));
                            infolist.Add(info);
                        }
                        this.planes.Remove(dxcall);
                        if (infolist.Count > 0)
                        {
                            infolist.Sort(new PlaneInfoComparer());
                            this.planes.Add(dxcall, infolist);
                            for (int i = 0; i < this.lv_Calls.Items.Count; i++)
                            {
                                if (this.lv_Calls.Items[i].Text.IndexOf(dxcall) >= 0)
                                {
                                    string newtext = infolist[0].Potential.ToString();
                                    if (this.lv_Calls.Items[i].SubItems[4].Text != newtext)
                                    {
                                        this.lv_Calls.Items[i].SubItems[4].Text = newtext;
                                        this.lv_Calls.Refresh();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e1_211)
                    {
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
                        this.planes.Clear();
                        for (int i = 0; i < this.lv_Calls.Items.Count; i++)
                        {
                            this.lv_Calls.Items[i].SubItems[4].Text = "";
                        }
                        this.lv_Calls.Refresh();
                    }
                    catch
                    {
                    }
                }
                else
                {
                    this.planes.Remove(dxcall);
                    for (int i = 0; i < this.lv_Calls.Items.Count; i++)
                    {
                        if (this.lv_Calls.Items[i].Text.IndexOf(dxcall) >= 0)
                        {
                            string newtext = "";
                            if (this.lv_Calls.Items[i].SubItems[4].Text != newtext)
                            {
                                this.lv_Calls.Items[i].SubItems[4].Text = newtext;
                                this.lv_Calls.Refresh();
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                wtMessage Msg = (wtMessage)e.UserState;
                if (Msg.Msg == WTMESSAGES.ASSETPATH)
                {
                    try
                    {
                        string[] a = Msg.Data.Split(new char[]{ ',' });
                        string mycall = a[0];
                        string myloc = a[1];
                        string dxcall = a[2];
                        string dxloc = a[3];
                        this.planes.Remove(dxcall);
                    }
                    catch (Exception e1_211)
                    {
                    }
                }
            }
        }

        private void bw_GetPlanes_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

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
            this.lv_Calls = new System.Windows.Forms.ListView();
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
            this.il_Calls = new System.Windows.Forms.ImageList(this.components);
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
            this.il_Planes = new System.Windows.Forms.ImageList(this.components);
            this.tt_Info = new System.Windows.Forms.ToolTip(this.components);
            this.bw_GetPlanes = new System.ComponentModel.BackgroundWorker();
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
            this.lv_Calls.SmallImageList = this.il_Calls;
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
            // il_Calls
            // 
            this.il_Calls.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_Calls.ImageStream")));
            this.il_Calls.TransparentColor = System.Drawing.Color.Transparent;
            this.il_Calls.Images.SetKeyName(0, "JEWEL_GRAY.PNG");
            this.il_Calls.Images.SetKeyName(1, "JEWEL_GREEN.PNG");
            this.il_Calls.Images.SetKeyName(2, "JEWEL_RED.PNG");
            this.il_Calls.Images.SetKeyName(3, "JEWEL_YELLWO.PNG");
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
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
            this.ti_Reconnect.Enabled = true;
            this.ti_Reconnect.Interval = 30000;
            this.ti_Reconnect.Tick += new System.EventHandler(this.ti_Reconnect_Tick);
            // 
            // ti_Linkcheck
            // 
            this.ti_Linkcheck.Enabled = false;
            this.ti_Linkcheck.AutoReset = true;
            this.ti_Linkcheck.Interval = 120000;
            this.ti_Linkcheck.Elapsed += this.ti_Linkcheck_Tick;
            // 
            // il_Planes
            // 
            this.il_Planes.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("il_Planes.ImageStream")));
            this.il_Planes.TransparentColor = System.Drawing.Color.Transparent;
            this.il_Planes.Images.SetKeyName(0, "Green");
            this.il_Planes.Images.SetKeyName(1, "Orange");
            this.il_Planes.Images.SetKeyName(2, "Blue");
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
