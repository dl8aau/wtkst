using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WinTest;
using wtKST.Properties;
using Application = System.Windows.Forms.Application;

namespace wtKST
{
    public class MainDlg : Form
    {

        private static readonly string[] BANDS = new string[] { "50M", "70M", "144M", "432M", "1_2G", "2_3G", "3_4G", "5_7G", "10G", "24G", "47G", "76G" };

        private KSTcom KST;

        public DataTable MYMSG = new DataTable("MYMSG");

        private QRVdb qrv = new QRVdb(BANDS);

        public DataTable CALL = new DataTable("CALL");

        private WinTest.WinTestLogBase wtQSO = null;

        private wtKST.AirScoutInterface AS_if = null;

        public static LogWriter Log = new LogWriter(Directory.GetParent(Application.LocalUserAppDataPath).ToString());

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

        private System.Windows.Forms.ListView lv_Msg;

        private ColumnHeader lvh_Time;

        private ColumnHeader lvh_Call;

        private ColumnHeader lvh_Name;

        private ColumnHeader lvh_Msg;

        private Label lbl_KST_Msg;

        private System.Windows.Forms.ListView lv_MyMsg;

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
        private DataGridView lv_Calls;

        private const int COLUMN_WIDTH = 20;

        private Label lbl_KST_Status;

        private ToolStripStatusLabel tsl_Info;

        private System.Windows.Forms.Button btn_KST_Send;

        private ToolStripStatusLabel tsl_Error;

        private NotifyIcon ni_Main;

        private ContextMenuStrip cmn_Notify;

        private ToolStripMenuItem cmi_Notify_Restore;

        private ToolStripSeparator toolStripSeparator3;

        private ToolStripMenuItem cmi_Notify_Quit;

        private System.Windows.Forms.Timer ti_Error;

        private System.Windows.Forms.ComboBox cb_Command;

        private System.Windows.Forms.Timer ti_Top;

        private System.Windows.Forms.Timer ti_Reconnect;

        private System.Windows.Forms.ToolTip tt_Info;
        private System.Windows.Forms.ToolTip tt_ASInfo;

        private System.Windows.Forms.Timer ti_UpdateFilter;

        private bool WinTestLocatorWarning = false;
        private bool hide_away = false;
        private bool sort_by_dir = false;
        private bool ignore_inactive = false;
        private bool hide_worked = false;
        private ContextMenuStrip cmn_userlist;
        private ContextMenuStrip cmn_msglist;
        private ToolStripMenuItem cmn_userlist_wtsked;
        private ToolStripMenuItem cmn_userlist_chatReview;
        private ToolStripMenuItem cmn_msglist_wtsked;
        private ToolStripMenuItem cmn_msglist_chatReview;
        private ToolStripMenuItem cmn_msglist_openURL;
        private ToolStripMenuItem cmn_msglist_AS_details;
        private ToolStripTextBox cmn_msglist_toolStripTextBox_DirQRB;
        private ToolStripLabel cmn_msglist_AS_status;
        private Label cmn_msglist_Label;

        private WinTest.wtStatus wts;
        private WTSkedDlg wtskdlg;
        private uint last_sked_qrg;
        private uint kst_sked_qrg;
        private string kst_sked_mode;
        private uint kst_sked_band_freq;
        private string last_cq_call;

        private ToolStripMenuItem macroToolStripMenuItem;
        private ToolStripMenuItem menu_btn_macro_1;
        private ToolStripMenuItem menu_btn_macro_2;
        private ToolStripMenuItem menu_btn_macro_3;
        private ToolStripMenuItem menu_btn_macro_4;
        private ToolStripMenuItem menu_btn_macro_5;
        private ToolStripMenuItem menu_btn_macro_6;
        private ToolStripMenuItem menu_btn_macro_7;
        private ToolStripMenuItem menu_btn_macro_8;
        private ToolStripMenuItem menu_btn_macro_9;
        private ToolStripMenuItem menu_btn_macro_0;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem macro_default_Station;
        private ToolStripStatusLabel tsl_LED_AS_Status;
        private ToolStripStatusLabel tsl_LED_Log_Status;
        private ToolStripStatusLabel tsl_LED_KST_Status;
        private string AS_watchlist = "";

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
            MainDlg.Log.WriteMessage("Startup Version " + aboutBox1.AssemblyVersion.ToString());
            Application.Idle += new EventHandler(OnIdle);
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            cb_Command.MouseWheel += new MouseEventHandler(cb_Command_MouseWheel);

            KST = new KSTcom();

            KST.process_new_message += KST_Process_new_message;
            KST.process_user_update += KST_Process_user_update;
            KST.update_user_state += onUserStateChanged;
            KST.dispText += KST_dispText;
            KST.KSTStateChanged += KST_StateChanged;

            CALL.Columns.Add("CALL");
            CALL.Columns.Add("NAME");
            CALL.Columns.Add("LOC");
            CALL.Columns.Add("TIME", typeof(DateTime));
            CALL.Columns.Add("CONTACTED", typeof(int));
            CALL.Columns.Add("RECENTLOGIN", typeof(bool));
            CALL.Columns.Add("ASLAT", typeof(double));
            CALL.Columns.Add("ASLON", typeof(double));
            CALL.Columns.Add("QRB", typeof(int));
            CALL.Columns.Add("DIR", typeof(int));
            CALL.Columns.Add("AWAY", typeof(bool));
            CALL.Columns.Add("AS");
            foreach (string band in BANDS)
                CALL.Columns.Add(band, typeof(int));
            CALL.Columns.Add("COLOR", typeof(int));
            DataColumn[] CALLkeys = new DataColumn[]
            {
                CALL.Columns["CALL"]
            };
            CALL.PrimaryKey = CALLkeys;
            lv_Calls.DataSource = CALL;
            lv_Calls.Columns["CALL"].HeaderCell.Value = "Call";
            lv_Calls.Columns["CALL"].Width = 80;
            lv_Calls.Columns["NAME"].HeaderCell.Value = "Name";
            lv_Calls.Columns["NAME"].Width = 100;
            lv_Calls.Columns["LOC"].HeaderCell.Value = "Locator";
            lv_Calls.Columns["LOC"].Width = 50;
            lv_Calls.Columns["AS"].Width = 30;
            lv_Calls.Columns["AS"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // fix the width
            lv_Calls.Columns["AS"].Resizable = DataGridViewTriState.False;

            lv_Calls.Columns["TIME"].Visible = false;

            lv_Calls.Columns["CONTACTED"].HeaderCell.Value = "ACT";
            lv_Calls.Columns["CONTACTED"].Width = 30;

            lv_Calls.Columns["RECENTLOGIN"].Visible = false;
            lv_Calls.Columns["ASLAT"].Visible = false;
            lv_Calls.Columns["ASLON"].Visible = false;
            lv_Calls.Columns["QRB"].Visible = false;
            lv_Calls.Columns["DIR"].Visible = false;
            lv_Calls.Columns["AWAY"].Visible = false;
            lv_Calls.Columns["COLOR"].Visible = false;
            lv_Calls.Columns["50M"].Width = 20;
            lv_Calls.Columns["70M"].Width = 20;
            lv_Calls.Columns["144M"].Width = 20;
            lv_Calls.Columns["432M"].Width = 20;
            lv_Calls.Columns["1_2G"].Width = 20;
            lv_Calls.Columns["2_3G"].Width = 20;
            lv_Calls.Columns["3_4G"].Width = 20;
            lv_Calls.Columns["5_7G"].Width = 20;
            lv_Calls.Columns["10G"].Width = 20;
            lv_Calls.Columns["24G"].Width = 20;
            lv_Calls.Columns["47G"].Width = 20;
            lv_Calls.Columns["76G"].Width = 20;

            for (int i = 0; i < lv_Calls.ColumnCount; i++)
            {
                lv_Calls.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // https://stackoverflow.com/a/28617333
            VScrollBar scrollBar = lv_Calls.Controls.OfType<VScrollBar>().First();
            scrollBar.Scroll += lv_Calls_scroll;

            string kstcall = WCCheck.WCCheck.Cut(Settings.Default.KST_UserName);

            // handle Log interface
            init_wtQSO();

            UpdateUserBandsWidth();
            AS_if = new wtKST.AirScoutInterface();
            AS_if.UpdateASStatusEvent += AS_UpdateASStatus;
            AS_if.Connect(); // TODO
            AS_if.ASStateChanged += AS_StateChanged;
            AS_if.ASStateChanged += AS_StateChanged;
            if (Settings.Default.KST_AutoConnect)
            {
                KST.Connect();
            }
        }

        private void onUserStateChanged(object sender, KSTcom.userStateEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<KSTcom.userStateEventArgs>(onUserStateChanged), new object[] { sender, args });
                return;
            }
            if (args.state == KSTcom.USER_STATE.Here)
            {
                lbl_Call.Text = Settings.Default.KST_UserName.ToUpper();
            }
            else
            {
                lbl_Call.Text = "(" + Settings.Default.KST_UserName.ToUpper() + ")";
            }

        }

        private void KST_dispText(object sender, KSTcom.newdispTextEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<KSTcom.newdispTextEventArgs>(KST_dispText), new object[] { sender, args });
                return;
            }
            if (!String.IsNullOrEmpty(args.text))
                Say(args.text);
        }

        private void KST_StateChanged(object sender, KSTcom.KSTStateEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<KSTcom.KSTStateEventArgs>(KST_StateChanged), new object[] { sender, args });
                return;
            }
            // from set_KST_Status
            if (args.KSTState < KSTcom.KST_STATE.Connected)
            {
                lbl_KST_Status.BackColor = Color.LightGray;
                lbl_KST_Status.Text = "Status: Disconnected ";
            }
            else
            {
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
            // "LED" handling
            switch (args.KSTState)
            {
                default:
                    if (tsl_LED_KST_Status.BackColor != Color.Red)
                        tsl_LED_KST_Status.BackColor = Color.Red;
                    break;
                case KSTcom.KST_STATE.WaitLogin:
                case KSTcom.KST_STATE.WaitLogstat:
                case KSTcom.KST_STATE.Disconnecting:
                    if (tsl_LED_KST_Status.BackColor != Color.Yellow)
                        tsl_LED_KST_Status.BackColor = Color.Yellow;
                    break;
                case KSTcom.KST_STATE.Connected:
                    if (tsl_LED_KST_Status.BackColor != Color.Green)
                        tsl_LED_KST_Status.BackColor = Color.Green;
                    break;
            }
        }


        private void AS_StateChanged(object sender, AirScoutInterface.ASStateEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<AirScoutInterface.ASStateEventArgs>(AS_StateChanged), new object[] { sender, args });
                return;
            }
            switch (args.ASState)
            {
                default:
                    if (tsl_LED_AS_Status.BackColor != Color.Red)
                        tsl_LED_AS_Status.BackColor = Color.Red;
                    break;
                case AirScoutInterface.AS_STATE.AS_UPDATING:
                    if (tsl_LED_AS_Status.BackColor != Color.Yellow)
                        tsl_LED_AS_Status.BackColor = Color.Yellow;
                    break;
                case AirScoutInterface.AS_STATE.AS_IN_SYNC:
                    if (tsl_LED_AS_Status.BackColor != Color.Green)
                        tsl_LED_AS_Status.BackColor = Color.Green;
                    break;
            }
        }

        private void Log_StateChanged(object sender, WinTestLogBase.LogStateEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<WinTestLogBase.LogStateEventArgs>(Log_StateChanged), new object[] { sender, args });
                return;
            }
            switch (args.LogState)
            {
                default:
                    if (tsl_LED_Log_Status.BackColor != Color.Red)
                        tsl_LED_Log_Status.BackColor = Color.Red;
                    break;
                case WinTestLogBase.LOG_STATE.LOG_SYNCING:
                    if (tsl_LED_Log_Status.BackColor != Color.Yellow)
                        tsl_LED_Log_Status.BackColor = Color.Yellow;
                    break;
                case WinTestLogBase.LOG_STATE.LOG_IN_SYNC:
                    if (tsl_LED_Log_Status.BackColor != Color.Green)
                        tsl_LED_Log_Status.BackColor = Color.Green;
                    break;
            }
        }

        private DateTime KST_Update_User_Filter_last_called = DateTime.Now;

        private void OnIdle(object sender, EventArgs args)
        {
            KST.KSTStateMachine();

            if (KST.State == KSTcom.KST_STATE.Disconnected)
            {
                lock (CALL)
                {
                    CALL.Clear();
                }
                // FIXME DV needed? lv_Calls.Items.Clear();
                tsi_KST_Connect.Enabled = true;
                tsi_KST_Disconnect.Enabled = false;
                tsi_KST_Here.Enabled = false;
                tsi_KST_Away.Enabled = false;
                cb_Command.Text = "";
                cb_Command.Enabled = false;
                btn_KST_Send.Enabled = false;
                lbl_Call.Enabled = false;
                lock (AS_if.planes)
                {
                    AS_if.planes.Clear();
                }
                if (Settings.Default.KST_AutoConnect && !ti_Reconnect.Enabled)
                {
                    ti_Reconnect.Start();
                }

                KST.SetStateStandby();
                KST_USR_RowFilter = "";
            }
            if (KST.State >= KSTcom.KST_STATE.Connected)
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
                if (KST_USR_RowFilter.Equals(""))
                    KST_Update_Usr_Filter(); // initial filter

                // rate limit - ideally calling filter update should not be needed here
                // should be triggered appropriately
                if (KST_Update_User_Filter_last_called.AddSeconds(10) <= DateTime.Now)
                {
                    KST_Update_Usr_Filter(true); // only check filter here! it should not have changed
                    KST_Update_User_Filter_last_called = DateTime.Now;
            }
            }

            //fill_AS_list();

            string KST_Calls_Text = lbl_KST_Calls.Text;
            if (lv_Calls.RowCount > 0)
            {
                /* show number of calls in list and total number of users (-1 for own call) */
                KST_Calls_Text = "Calls [" + lv_Calls.RowCount.ToString() + " / " + (CALL.Rows.Count - 1) + "]";
                if (wtQSO != null)
                    KST_Calls_Text += " - " + Path.GetFileName(wtQSO.getStatus());
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

            ni_Main.Text = "wtKST\nLeft click to activate";

            if (!aboutBox1.Visible && !cb_Command.IsDisposed && !cb_Command.Focused && !btn_KST_Send.Capture
                 && (wtskdlg == null || !wtskdlg.Visible))
            {
                cb_Command.Focus();
                cb_Command.SelectionLength = 0;
                cb_Command.SelectionStart = cb_Command.Text.Length;
            }
        }

        private int next_color = 1;

        private void KST_Process_new_message(object sender, KSTcom.newMSGEventArgs arg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<KSTcom.newMSGEventArgs>(KST_Process_new_message), new object[] { sender, arg });
                return;
            }

            DataRow Row = arg.msg;

            try
            {

                DateTime dt = (DateTime)Row["TIME"];

                ListViewItem LV = new ListViewItem();
                LV.Text = ((DateTime)Row["TIME"]).ToString("HH:mm");
                LV.Tag = dt; // store the timestamp in the tag field
                LV.SubItems.Add(Row["CALL"].ToString());
                LV.SubItems.Add(Row["NAME"].ToString());

                // filter <a href="http://dr9a.de" target="_blank"><b>http_link</b></a> -> http://dr9a.de
                // changed to <a href=`http://www.heise.de` target=`_blank`...> Jan-2024
                Regex regex_filter_msg_http_link = new Regex(@"(.*)<a href=(?:""|`)(https?://.*)(?:""|`) target=(?:""|`)_blank(?:""|`)><b>https?_link</b></a>(.*)", RegexOptions.Compiled);

                string msg = regex_filter_msg_http_link.Replace(Row["MSG"].ToString(), "$1$2 $3");
                LV.SubItems.Add(msg);
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

                    lv_Msg.Items.Insert(0, LV);

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
                string MyCall = Settings.Default.KST_UserName.ToUpper();
                if (Row["RECIPIENT"].ToString().Equals(MyCall))
                {
                    lv_Msg.Items[current_index].BackColor = Color.Coral;
                }

                bool fromMe = Row["CALL"].ToString().ToUpper().StartsWith(MyCall);

                if (Row["MSG"].ToString().ToUpper().StartsWith("(" + MyCall + ")") || Row["MSG"].ToString().ToUpper().StartsWith(MyCall)
                    || (fromMe && Settings.Default.KST_Show_Own_Messages))
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
                        switch (color_index)
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
                    MyLV.SubItems.Add(msg); // filtered: <a href="http://dr9a.de" target="_blank"><b>http_link</b>
                    for (int i = 0; i < MyLV.SubItems.Count; i++)
                    {
                        MyLV.SubItems[i].Name = lv_MyMsg.Columns[i].Text;
                    }
                    lv_MyMsg.Items.Insert(0, MyLV);
                    lv_MyMsg.Items[0].EnsureVisible();
                    int hwnd = MainDlg.GetForegroundWindow();
                    if (hwnd != base.Handle.ToInt32())
                    {
                        if (Settings.Default.KST_ShowBalloon && !fromMe)
                        {
                            ni_Main.ShowBalloonTip(30000, "New MyMessage received", msg, ToolTipIcon.Info);
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

        private void KST_Process_user_update(object sender, KSTcom.UserUpdateEventArgs arg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<KSTcom.UserUpdateEventArgs>(KST_Process_user_update), new object[] { sender, arg });
                return;
            }

            DataRow USER = arg.user;

            switch (arg.user_op)
            {
                case KSTcom.USER_OP.USER_NEW:
                    {
                        if (USER.RowState == DataRowState.Detached)
                            return; // seems this happens from time to time... FIXME

                        DataRow row = CALL.NewRow();

                        row["CALL"] = USER["CALL"];
                        row["NAME"] = USER["NAME"];
                        row["LOC"] = USER["LOC"];
                        string loc = USER["LOC"].ToString();

                        int qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc);
                        int qtf = (int)WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc);
                        row["QRB"] = qrb;
                        row["DIR"] = qtf;

                        row["COLOR"] = 0;
                        row["TIME"] = USER["TIME"];
                        row["CONTACTED"] = USER["CONTACTED"];
                        row["AWAY"] = USER["AWAY"];
                        row["RECENTLOGIN"] = USER["RECENTLOGIN"];

                        string qrvcall = row["CALL"].ToString();
                        bool call_new_in_userlist = (bool)USER["RECENTLOGIN"];
                        qrv.Process_QRV(row, qrvcall, call_new_in_userlist);

                        if (call_new_in_userlist && wtQSO != null && wtQSO.QSO.Rows.Count > 0)
                            Check_QSO(row);

                        lock (CALL)
                        {
                            CALL.Rows.Add(row);
                        }
                    }
                    break;

                case KSTcom.USER_OP.USER_DELETE:
                    {
                        DataRow row = CALL.Rows.Find(USER["CALL"]);
                        if (row != null)
                            row.Delete();
                    }
                    break;
                case KSTcom.USER_OP.USER_MODIFY:
                    {
                        if (USER.RowState == DataRowState.Detached)
                            return; // seems this happens from time to time... FIXME

                        DataRow row = CALL.Rows.Find(USER["CALL"]);
                        if (row != null)
                        {
                            if (!row["NAME"].Equals(USER["NAME"]))
                                row["NAME"] = USER["NAME"];
                            if (!row["LOC"].Equals(USER["LOC"]))
                            {
                                row["LOC"] = USER["LOC"];
                                string loc = USER["LOC"].ToString();

                                int qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc);
                                int qtf = (int)WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc);
                                row["QRB"] = qrb;
                                row["DIR"] = qtf;
                            }
                            if ( !row["TIME"].Equals(USER["TIME"]) || !row["CONTACTED"].Equals(USER["CONTACTED"]) )
                            {
                                row["TIME"] = USER["TIME"];
                                row["CONTACTED"] = USER["CONTACTED"]; // need to touch "CONTACTED" to trigger a change
                            }
                            if (!row["AWAY"].Equals(USER["AWAY"]))
                                row["AWAY"] = USER["AWAY"];
                            if (!row["RECENTLOGIN"].Equals(USER["RECENTLOGIN"]))
                                row["RECENTLOGIN"] = USER["RECENTLOGIN"];
                        }
                    }
                    break;
                case KSTcom.USER_OP.USER_MODIFY_STATE:
                    {
                        if (USER.RowState == DataRowState.Detached)
                            return; // seems this happens from time to time... FIXME

                        DataRow row = CALL.Rows.Find(USER["CALL"]);
                        if (row != null)
                        {
                            if (!row["AWAY"].Equals(USER["AWAY"]))
                                row["AWAY"] = USER["AWAY"];
                            if (!row["RECENTLOGIN"].Equals(USER["RECENTLOGIN"]))
                                row["RECENTLOGIN"] = USER["RECENTLOGIN"];
                        }
                    }
                    break;
                case KSTcom.USER_OP.USER_DONE:
                    AS_send_ASWATCHLIST();
                    break;
            }
        }

        private string KST_USR_RowFilter = "";

        private void KSTUsrFilterSetTimer(int setTimer, bool force_wait)
        {

            if (force_wait)
            {
                ti_UpdateFilter.Stop();
            }
            else if (ti_UpdateFilter.Enabled)
            {
                return; // timer is already triggered, don't reprogram
            }
            ti_UpdateFilter.Interval = setTimer;
            ti_UpdateFilter.Enabled = true;
        }

        private void ti_UpdateFilter_Tick(object sender, EventArgs e)
        {
            KST_Update_Usr_Filter(false);
            ti_UpdateFilter.Stop();
        }

        private void KST_Update_Usr_Filter(bool check = false)
        {
            string RowFilter = string.Format("(CALL <> '{0}')", Settings.Default.KST_UserName.ToUpper());
            if (hide_away)
                RowFilter += string.Format(" AND (AWAY = 0)");

            if (ignore_inactive)
            {
                var last_activity_max = DateTime.UtcNow.AddMinutes(-120);
                RowFilter += string.Format(" AND (CONTACTED < 3)");
                RowFilter += string.Format(" AND (TIME > #{0}#)", last_activity_max.ToString("g", CultureInfo.InvariantCulture.DateTimeFormat));
            }

            int MaxDist = Convert.ToInt32(Settings.Default.KST_MaxDist);
            if (MaxDist != 0)
                RowFilter += string.Format(" AND (QRB < {0}", MaxDist);

            // drop calls that are already in log
            if (hide_worked)
            {
                string add_rowfilter = " AND (CALL NOT IN (";
                bool calls_to_hide = false;
                foreach (DataRow row in CALL.Rows)
                {
                    if (check_in_log(row))
                    {
                        calls_to_hide = true;
                        add_rowfilter += string.Format("'{0}',", row["CALL"]);
                    }
                }
                if (calls_to_hide)
                    RowFilter += add_rowfilter + "))";
            }

            RowFilter += ")";

            if (!RowFilter.Equals(KST_USR_RowFilter))
            {
                if (check)
                {
                    // just check if a change would have an effect
                    Console.WriteLine("RowFilter is " + KST_USR_RowFilter);
                    Console.WriteLine("should be " + RowFilter);
                    MainDlg.Log.WriteMessage("RowFilter is " + KST_USR_RowFilter);
                    MainDlg.Log.WriteMessage("should be " + RowFilter);
                    return;
                }
                // remember the vertical scroll position as the rowfilter will screw it up
                // it is reset in OnIdle
                var Keep_FirstDisplayedScrollingRowIndex = lv_Calls.FirstDisplayedScrollingRowIndex;
                (lv_Calls.DataSource as DataTable).DefaultView.RowFilter = RowFilter;
                KST_USR_RowFilter = RowFilter;
                // now restore the vertical position of the first entry
                if (lv_Calls.FirstDisplayedScrollingRowIndex != Keep_FirstDisplayedScrollingRowIndex)
                {
                    // if we try to force the first line to something that is not visible anymore , then just ask to scroll down as far as possible
                    // see https://stackoverflow.com/a/9823873
                    if (Keep_FirstDisplayedScrollingRowIndex + lv_Calls.DisplayedRowCount(false) < lv_Calls.Rows.Count)
                    {
                        lv_Calls.FirstDisplayedScrollingRowIndex = Keep_FirstDisplayedScrollingRowIndex;
                    }
                    else
                    {
                        lv_Calls.FirstDisplayedScrollingRowIndex = lv_Calls.Rows.Count - 1;
                    }
                }
            }
        }

        //FIXME re-add
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
                            string locator = bs.Split(new char[] { ' ' })[1].Trim();
                            row["LOC"] = locator;
                            row["TIME"] = DateTime.MinValue;
                            row["CONTACTED"] = 0;
                            row["AWAY"] = true;

                            int qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, locator);
                            int qtf = (int)WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, locator);
                            row["QRB"] = qrb;
                            row["DIR"] = qtf;

                            row["RECENTLOGIN"] = false;

                            foreach (string band in BANDS)
                                row[band] = QRVdb.QRV_STATE.not_qrv;
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

        private void tsi_File_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                KST.Disconnect();
            }
            catch
            {
            }
            base.Close();
        }

        public class bandinfo
        {
            public string band_name { get; set; }
            public string band_wavelength_name { get; set; }
            public uint band_start { get; set; }
            public uint band_stop { get; set; }
            public uint band_center_activity { get; set; }
            public bool in_band(int freq)
            {
                return (freq >= band_start && freq <= band_stop);
            }

            public bandinfo(string n, string wavelength, uint start, uint stop, uint center)
            {
                band_name = n;
                band_wavelength_name = wavelength;
                band_start = start;
                band_stop = stop;
                band_center_activity = center;
            }
        }

        private List<bandinfo> selected_bands()
        {
            List<bandinfo> b = new List<bandinfo>();
            if (Settings.Default.Band_50)
                b.Add(new bandinfo("50MHz", "6 m", 50000, 52000, 50000));
            if (Settings.Default.Band_70)
                b.Add(new bandinfo("70MHz", "4 m", 70000, 70500, 70000));
            if (Settings.Default.Band_144)
                b.Add(new bandinfo("144MHz", "2 m", 144000, 146000, 144000));
            if (Settings.Default.Band_432)
                b.Add(new bandinfo("432MHz", "70 cm", 430000, 440000, 432000));
            if (Settings.Default.Band_1296)
                b.Add(new bandinfo("1296MHz", "23 cm", 1240000, 1300000, 1296000));
            if (Settings.Default.Band_2320)
                b.Add(new bandinfo("2320MHz", "13 cm", 2320000, 2450000, 2320000));
            if (Settings.Default.Band_3400)
                b.Add(new bandinfo("3400MHz", "9 cm", 3400000, 3475000, 3400000));
            if (Settings.Default.Band_5760)
                b.Add(new bandinfo("5760MHz", "6 cm", 5650000, 5850000, 5760000));
            if (Settings.Default.Band_10368)
                b.Add(new bandinfo("10GHz", "3 cm", 10000000, 10500000, 10368000));
            if (Settings.Default.Band_24GHz)
                b.Add(new bandinfo("24GHz", "1.2 cm", 24000000, 24250000, 24048000));
            if (Settings.Default.Band_47GHz)
                b.Add(new bandinfo("47GHz", "6 mm", 47000000, 47200000, 47088000));
            if (Settings.Default.Band_76GHz)
                b.Add(new bandinfo("76GHz", "4 mm", 75500000, 81500000, 76032000));
            return b;
        }

        // hide bands that should not be displayed by making their width = 0
        private void UpdateUserBandsWidth()
        {
            lv_Calls.Columns["50M"].Visible = Settings.Default.Band_50;
            lv_Calls.Columns["70M"].Visible = Settings.Default.Band_70;
            lv_Calls.Columns["144M"].Visible = Settings.Default.Band_144;
            lv_Calls.Columns["432M"].Visible = Settings.Default.Band_432;
            lv_Calls.Columns["1_2G"].Visible = Settings.Default.Band_1296;
            lv_Calls.Columns["2_3G"].Visible = Settings.Default.Band_2320;
            lv_Calls.Columns["3_4G"].Visible = Settings.Default.Band_3400;
            lv_Calls.Columns["5_7G"].Visible = Settings.Default.Band_5760;
            lv_Calls.Columns["10G"].Visible = Settings.Default.Band_10368;
            lv_Calls.Columns["24G"].Visible = Settings.Default.Band_24GHz;
            lv_Calls.Columns["47G"].Visible = Settings.Default.Band_47GHz;
            lv_Calls.Columns["76G"].Visible = Settings.Default.Band_76GHz;
        }

        bool check_in_log(DataRow row)
        {
            // FIXME use selected_bands()
            if (Settings.Default.Band_50 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["50M"]))
                return false;
            if (Settings.Default.Band_70 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["70M"]))
                return false;
            if (Settings.Default.Band_144 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["144M"]))
                return false;
            if (Settings.Default.Band_432 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["432M"]))
                return false;
            if (Settings.Default.Band_1296 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["1_2G"]))
                return false;
            if (Settings.Default.Band_2320 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["2_3G"]))
                return false;
            if (Settings.Default.Band_3400 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["3_4G"]))
                return false;
            if (Settings.Default.Band_5760 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["5_7G"]))
                return false;
            if (Settings.Default.Band_10368 &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["10G"]))
                return false;
            if (Settings.Default.Band_24GHz &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["24G"]))
                return false;
            if (Settings.Default.Band_47GHz &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["47G"]))
                return false;
            if (Settings.Default.Band_76GHz &&
                !QRVdb.worked_or_not_qrv((QRVdb.QRV_STATE)row["76G"]))
                return false;
            return true;
        }

        private void tsi_Options_Click(object sender, EventArgs e)
        {
            OptionsDlg Dlg = new OptionsDlg();
            Dlg.cbb_KST_Chat.SelectedIndex = 2;
            if (KST.State != KSTcom.KST_STATE.Standby)
            {
                Dlg.tb_KST_Password.Enabled = false;
                Dlg.tb_KST_UserName.Enabled = false;
                Dlg.cbb_KST_Chat.Enabled = false;
                Dlg.tb_KST_Locator.Enabled = false;
                Dlg.tb_KST_Name.Enabled = false;
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
                    KST.MSG_clear();
                    // FIXME DV needed ? lv_Calls.Items.Clear();
                    lv_Msg.Items.Clear();
                    lv_MyMsg.Items.Clear();
                }
                UpdateUserBandsWidth();
                if (KST_MaxDist != Convert.ToInt32(Settings.Default.KST_MaxDist))
                    KST_Update_Usr_Filter();
                macro_RefreshMacroText();
                init_wtQSO();
                AS_if.Connect(); //TODO!
            }
        }
        private void tsi_KST_Connect_Click(object sender, EventArgs e)
        {
            KST.Connect();
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
            KST.Disconnect();
        }

        private void tsi_KST_Here_Click(object sender, EventArgs e)
        {
            KST.Here();
        }

        private void tsi_KST_Away_Click(object sender, EventArgs e)
        {
            KST.Away();
        }

        private void init_wtQSO()
        {
            if (Settings.Default.WinTest_Activate)
            {
                // problem: what if wtQSO is still in use somewhere?
                if (wtQSO != null)
                {
                    if (wtQSO.GetType() == typeof(WinTest.WinTestLog))
                    {
                        Console.WriteLine("wtQSO already WinTestLog");
                        return;
                    }
                    else
                    {

                        Console.WriteLine("wtQSO active " + wtQSO.GetType().ToString());
                        ((IDisposable)wtQSO).Dispose();
                        wtQSO = null; // TODO: dispose?
                    }
                }
                // check if we are running on Windows, otherwise Win-Test will not run
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    wtQSO = new WinTest.WinTestLog(MainDlg.Log.WriteMessage);
                    wtQSO.LogStateChanged += Log_StateChanged;
                }
                if (wts == null)
                    wts = new WinTest.wtStatus();
            }
            else if (Settings.Default.WinTest_NetworkSync_active)
            {
                if (wtQSO != null)
                {
                    if (wtQSO.GetType() == typeof(WinTest.WtLogSync))
                    {
                        Console.WriteLine("wtQSO already WtLogSync");
                        return;
                    }
                    else
                    {

                        Console.WriteLine("wtQSO active " + wtQSO.GetType().ToString());
                        ((IDisposable)wtQSO).Dispose();
                        wtQSO = null;
                    }
                }
                wtQSO = new WtLogSync(MainDlg.Log.WriteMessage);
                if (wts == null)
                    wts = new WinTest.wtStatus();
                wtQSO.LogStateChanged += Log_StateChanged;
            }
            else if (Settings.Default.QARTest_Sync_active)
            {
                if (wtQSO != null)
                {
                    if (wtQSO.GetType() == typeof(WinTest.QARTestLogSync))
                    {
                        Console.WriteLine("wtQSO already QARTestLogSync");
                        return;
                    }
                    else
                    {

                        Console.WriteLine("wtQSO active " + wtQSO.GetType().ToString());
                        ((IDisposable)wtQSO).Dispose();
                        wtQSO = null;
                    }
                }
                wtQSO = new QARTestLogSync(MainDlg.Log.WriteMessage);
                if (wts != null)
                {
                    Console.WriteLine("wts active - turn off");
                    wts = null;
                }
                wtQSO.LogStateChanged += Log_StateChanged;
            }
            else
            {
                if (wtQSO != null)
                {

                    Console.WriteLine("wtQSO active " + wtQSO.GetType().ToString());
                    wtQSO = null; // TODO: dispose?
                }
                // turn off wintest status support
                if (wts != null)
                {
                    Console.WriteLine("wts active - turn off");
                    wts = null;
                }
                Log_StateChanged(this, new WinTest.WinTestLogBase.LogStateEventArgs(WinTestLogBase.LOG_STATE.LOG_INACTIVE));
            }
        }

        private bool wtQSO_local_lock = false;

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
            bool[] found = new bool[BANDS.Length]; // defaults to false

            lock (wtQSO)
            {
                if (wtQSO_local_lock)
                {
                    Console.WriteLine("wtQSO locked");
                    return;
                }
                wtQSO_local_lock = true;
                DataRow[] selectRow = wtQSO.QSO.Select(findCall);

                foreach (var qso_row in selectRow)
                {
                    var band = qso_row["BAND"].ToString();

                    if (WCCheck.WCCheck.Cut(qso_row["CALL"].ToString()).Equals(wcall))
                    {
                        if (!QRVdb.worked((QRVdb.QRV_STATE)call_row[band]))
                            call_row[band] = QRVdb.set_worked((QRVdb.QRV_STATE)call_row[band], true);

                        found[Array.IndexOf(BANDS, band)] = true;
                        // check locator
                        if (call_row["LOC"].ToString() != qso_row["LOC"].ToString())
                        {
                            // TODO: this most probably leads to
                            // Check_QSOs(1055): Die Sammlung wurde geändert; möglicherweise wurde die Enumeration nicht ausgeführt.
                            // so comment out for now
                            // Say(call + " Locator wrong? Win-Test Log " + band + " " + qso_row["TIME"] + " " + call + " " + qso_row["LOC"] + " KST " + call_row["LOC"].ToString());
                            WinTestLocatorWarning = true;
                            Log.WriteMessage("Win-Test log locator mismatch: " + qso_row["BAND"] + " " + qso_row["TIME"] + " " + call + " Locator wrong? Win-Test Log " + qso_row["LOC"] + " KST " + call_row["LOC"].ToString());
                        }
                    }
                }
                wtQSO_local_lock = false;
                // important: I cannot know if a call is *not* in the log until I scanned the log fully, so this code to clean entries that are wrongly marked as "worked" has to be done
                // *after* the loop
                foreach (string band in BANDS)
                {
                    // if marked as worked - but not in the log anymore, just leave it as "qrv"
                    // check if worked - only then remove the flag. If we touch everything, we may trigger unneeded redraws
                    if (!found[Array.IndexOf(BANDS, band)] && QRVdb.worked((QRVdb.QRV_STATE)call_row[band]))
                        call_row[band] = QRVdb.set_worked((QRVdb.QRV_STATE)call_row[band], false);
                }
            }
        }

        private bool check_QSO_CALL_lock = false;

        private void Check_QSOs()
        {
            WinTestLocatorWarning = false;
            try
            {
                lock (CALL)
                {
                    if (check_QSO_CALL_lock)
                    {
                        Console.WriteLine("Check_QSOs can't check...");
                        return;
                    }
                    check_QSO_CALL_lock = true;
                    // lock helps against other threads changing CALL, but not same (UI)
                    foreach (DataRow call_row in CALL.Rows) // FIXME: CALL muss gelockt werden, während foreach drüber läuft
                    {
                        Check_QSO(call_row);
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + wtQSO.getStatus() + "): " + e.Message);
                MainDlg.Log.WriteMessage(MethodBase.GetCurrentMethod().Name + "(" + wtQSO.getStatus() + "): " + e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                check_QSO_CALL_lock = false;
            }
        }

        private void ti_Main_Tick(object sender, EventArgs e)
        {
            ti_Main.Stop();
            if (KST.State == KSTcom.KST_STATE.Connected)
            {
                if (lv_Calls != null)
                    lv_Calls.InvalidateColumn(lv_Calls.Columns["CONTACTED"].DisplayIndex); // trigger a redraw of the activity column to update the time

                if (wtQSO != null)
                {
                    lock (wtQSO)
                    {
                        if (wtQSO_local_lock)
                        {
                            Console.WriteLine("ti_Main_Tick - skip Check_QSOs()");
                        }
                        else
                        {
                            wtQSO_local_lock = true;
                            try
                            {
                                MainDlg.Log.WriteMessage("KST wt Get_QSOs start.");

                                if (wtQSO.GetType() == typeof(WinTestLog))
                                    wtQSO.Get_QSOs(Settings.Default.WinTest_INI_FileName);
                                else if (wtQSO.GetType() == typeof(WtLogSync))
                                    wtQSO.Get_QSOs(Settings.Default.WinTest_StationName);
                                else if (wtQSO.GetType() == typeof(QARTestLogSync))
                                    wtQSO.Get_QSOs("");
                                if (!String.IsNullOrEmpty(wtQSO.MyLoc) && WCCheck.WCCheck.IsLoc(wtQSO.MyLoc) > 0 && !wtQSO.MyLoc.Equals(Settings.Default.KST_Loc))
                                {
                                    MessageBox.Show("KST locator " + Settings.Default.KST_Loc + " does not match locator in Win-Test " + wtQSO.MyLoc + " !!!", "Win-Test Log",
                                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                                wtQSO_local_lock = false;

                                Check_QSOs();
                            }
                            catch
                            {

                            }
                            finally
                            {
                                wtQSO_local_lock = false;
                            }
                        }
                    }
                }
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

        private bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }

            return false;
        }

        private void MainDlg_Load(object sender, EventArgs e)
        {
            // https://stackoverflow.com/a/942069
            // https://learn.microsoft.com/en-us/answers/questions/1370989/function-to-save-restore-last-used-form-size-posit

            // Set window location and size
            if (Settings.Default.WindowLocation != null &&
                Settings.Default.WindowSize != null &&
                IsVisibleOnAnyScreen(new Rectangle(Settings.Default.WindowLocation, Settings.Default.WindowSize)))
            {
                Location = Settings.Default.WindowLocation;
                Size = Settings.Default.WindowSize;
            }
            else
            {
                if (Settings.Default.WindowSize != null)
                    Size = Settings.Default.WindowSize;
                StartPosition = FormStartPosition.Manual;
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width,
                                     Screen.PrimaryScreen.WorkingArea.Height - this.Height);
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
                AS_if.OnFormClosing();
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

        private void ToolTipPos(string text, Control control, ref Point p)
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
            if (p.Y + TitleBarHeight - Cursor.Size.Height + TextLineHeight - TextBlockHeight > 0)// move text above the mouse pointer
            {
                p.Y += TitleBarHeight - Cursor.Size.Height + TextLineHeight - TextBlockHeight;
            }
            else
            {
                p.Y = 0;
            }
        }

        private System.Windows.Forms.Timer ti_ToolTip_active = new System.Windows.Forms.Timer();
        private bool ToolTip_AS_active = false;

        private void ShowToolTip(string text, Control control, Point p)
        {
            ToolTipPos(text, control, ref p);
            tt_Info.Show(text, this, p, 5000);
        }

        private void ShowASToolTip(string text, Control control, Point p)
        {
            ToolTipPos(text, control, ref p);
            tt_ASInfo.Show(text, this, p, 10000);
            ti_ToolTip_active.Start();
            ToolTip_AS_active = true;
        }

        private void ti_ToolTip_active_Tick(object sender, EventArgs e)
        {
            ToolTip_AS_active = false;
            ti_ToolTip_active.Stop();
        }

        private void ToolTipp_AS_Popup(System.Object sender, System.Windows.Forms.PopupEventArgs e)
        {
            Console.WriteLine("Popup " + e.ToolTipSize.Width + " " + e.ToolTipSize.Height);
            System.Windows.Forms.ToolTip tt = (sender as System.Windows.Forms.ToolTip);
            string toolTipText = tt.GetToolTip(e.AssociatedControl);

            using (Font f = new Font("Tahoma", 9))
            {
                Size ttsize = TextRenderer.MeasureText(toolTipText, f);
                ttsize.Height += f.Height / 2;
                e.ToolTipSize = ttsize;
            }
            Console.WriteLine("now " + e.ToolTipSize.Width + " " + e.ToolTipSize.Height + " " + (float)e.ToolTipSize.Height/((float)(toolTipText.Split('\n').Length-1)));
        }

        private void ToolTipp_AS_Draw(System.Object sender,
            System.Windows.Forms.DrawToolTipEventArgs e)
        {
            // Draw the standard background.
            e.DrawBackground();
            e.DrawBorder();
            Console.WriteLine("ttdraw " + e.ToolTipText + "\n" + e.ToolTipText.Split(new string[] { "\n" }, StringSplitOptions.None).Count() + " " + e.Bounds.Height );

            using (StringFormat sf = new StringFormat())
            using (Font f = new Font("Tahoma", 9))
            {
                int line = 0;
                Regex pattern = new Regex(@"(?<pot>\d+) : \[?(?<ID>\w+)\]?\s*\[(?<cat>\w)\] --> (?<dist>\w+) \[(?<min>\w+)mins\]");
                sf.FormatFlags = StringFormatFlags.NoWrap;
                sf.SetTabStops(0.0f, new float[] { 40.0f });

                foreach ( var s in e.ToolTipText.Split(new string[] { "\n" }, StringSplitOptions.None))
                {
                    // skip the first line, any empty line and lines that do not match
                    if (line >1 && s.Length > 0 && pattern.IsMatch(s))
                    {
                        try
                        {
                            Match match = pattern.Match(s);
                            int pot = int.Parse(match.Groups["pot"].Value);
                            string ID = match.Groups["ID"].Value;
                            string cat = match.Groups["cat"].Value;
                            string dist = match.Groups["dist"].Value;
                            string min = match.Groups["min"].Value;
                            int Mins = int.Parse(min);

                            paintAScell(pot, cat, Mins, new Rectangle(0, line * f.Height-3, 33, f.Height+4), e.Graphics);
                            // for the text we have to use DrawString - textrenderer does not support tabs it seems
                            e.Graphics.DrawString(( ID.Length<7 ? ID.PadRight(7) : ID) + "\t" + dist + " " + (pot<100 ? min + "mins" : ""), f,
                                    SystemBrushes.ActiveCaptionText, new Point(45, line * f.Height), sf);
                        }
                        catch (Exception ex)
                        {
                            MainDlg.Log.WriteMessage(MethodBase.GetCurrentMethod().Name + " " + s + "\n" + ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                    else
                    {
                        e.Graphics.DrawString(s, e.Font, SystemBrushes.ActiveCaptionText, new Point(0, line * e.Font.Height), sf);
                    }
                    line++;
                }
            }
            e.Graphics.Dispose();
        }

        /// <summary>
        /// Paint airplane scatter status - visualize status by an elipse with different size and color
        /// </summary>
        /// <param name="pot">"Potential" of a plane - 100=on the path, 75=soon on the path, 50=soon on the path, but too low</param>
        /// <param name="cat">"Category" of the plane ([S]uper heavy, [H]eavy, [M]edium, [L]ight</param>
        /// <param name="Mins">minutes until the plane is on the path</param>
        /// <param name="CellBounds">Bounds of the cell to paint</param>
        /// <param name="gfx">Graphics context to draw to</param>
        /// 
        private void paintAScell(int pot, string cat, int Mins, Rectangle CellBounds, Graphics gfx)
        {
            Rectangle newRect = new Rectangle(CellBounds.X + 2,
                CellBounds.Y + 2, CellBounds.Width - 4,
                CellBounds.Height - 4);
            Rectangle b = newRect;
            b.Inflate(-5, 0); // width -10 for the text
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
            b.X = newRect.X + 1;  // FIXME needed?
            if (pot == 100)
            {
                // 100
                gfx.FillEllipse(new SolidBrush(Color.Magenta), b);
            }
            else if (pot > 50)
            {
                // 51..99
                if (Mins > 15)
                    gfx.FillEllipse(new SolidBrush(Color.FromArgb(0xff, 0xff, 0xdb, 0xdb)), b); // Pale Pink
                else if (Mins > 5)
                    gfx.FillEllipse(new SolidBrush(Color.FromArgb(0xff, 0xb7, 0x2d, 0x2d)), b); // Golden Gate Bridge Orange
                else
                    gfx.FillEllipse(new SolidBrush(Color.Red), b);

                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    Rectangle tb = newRect;
                    tb.X = newRect.Width - 15 + tb.X;
                    tb.Width = 15;
                    using (Font font = new Font("Tahoma", 6.6F, FontStyle.Regular))
                    {
                        gfx.DrawString(Mins.ToString(), font, Brushes.Black, tb, sf);
                    }
                }
            }
            else
            {
                // 0..50
                gfx.FillEllipse(new SolidBrush(Color.Orange), b);
            }
        }

        private void lv_Calls_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0)
                return;
            DataGridView dgv = sender as DataGridView;

            // check that we are in a header cell!
            if (e.RowIndex == -1)
            {
                if (e.ColumnIndex > dgv.Columns["AS"].DisplayIndex  /* 3  */  /* Header.Text[0] > '0' && e.Header.Text[0] < '9') || e.Header.Text == "AS" */)
                {
                    Rectangle rect = e.CellBounds;
                    if (e.ColumnIndex > 4 && hide_worked) // Band
                        e.Graphics.FillRectangle(Brushes.LightGray, rect);
                    else
                        e.PaintBackground(e.ClipBounds, false);
                    using (Font headerfont = new Font(e.CellStyle.Font.OriginalFontName, 6f))
                    {
                        Size titlesize = TextRenderer.MeasureText(e.FormattedValue.ToString(), headerfont);
                        e.Graphics.TranslateTransform(0f, (float)titlesize.Width);
                        e.Graphics.RotateTransform(-90f);
                        e.Graphics.DrawString(e.FormattedValue.ToString(), headerfont, Brushes.Black, new PointF((float)rect.Y, (float)(rect.X + 3)));
                        e.Graphics.RotateTransform(90f);
                        e.Graphics.TranslateTransform(0f, (float)(-(float)titlesize.Width));
                    }
                    e.Handled = true;
                }
                else if (e.ColumnIndex == dgv.Columns["CALL"].DisplayIndex || e.ColumnIndex == dgv.Columns["LOC"].DisplayIndex
                      || e.ColumnIndex == dgv.Columns["CONTACTED"].DisplayIndex)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    // columns with filter function (CALL, ACT column - Band handled above)
                    if ((e.ColumnIndex == dgv.Columns["CALL"].DisplayIndex && hide_away) ||
                        (e.ColumnIndex == dgv.Columns["CONTACTED"].DisplayIndex && ignore_inactive))
                    {
                        e.Graphics.FillRectangle(Brushes.LightGray, e.CellBounds);
                    }

                    // mark sorting column - draw text + icon
                    if ((e.ColumnIndex == dgv.Columns["CALL"].DisplayIndex) ||
                        (e.ColumnIndex == dgv.Columns["LOC"].DisplayIndex))
                    {
                        //Draw Text Custom
                        TextRenderer.DrawText(e.Graphics, string.Format("{0}", e.FormattedValue),
                        e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                        //Draw Sort Icon
                        var sortIcon = ((e.ColumnIndex == dgv.Columns["LOC"].DisplayIndex && sort_by_dir) ||
                            (e.ColumnIndex == dgv.Columns["CALL"].DisplayIndex && !sort_by_dir)) ? "▼" : " ";
                        //Or draw an icon here.
                        TextRenderer.DrawText(e.Graphics, sortIcon,
                            e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor,
                            TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
                    }
                    else
                    {
                        e.PaintContent(e.CellBounds);
                    }
                    e.Handled = true;
                }
            }
            else
            {
                if (e.ColumnIndex == dgv.Columns["CALL"].DisplayIndex && e.Value != null)
                {
                    string call = e.Value.ToString();
                    if (bool.TryParse(dgv.Rows[e.RowIndex].Cells["AWAY"].Value.ToString(), out bool away) && away == true)
                    {
                        call = "(" + call + ")";
                        // Italic is too difficult to read and the font gets bigger
                        //LV.Font = new Font(LV.Font, FontStyle.Italic);
                    }
                    e.PaintBackground(e.ClipBounds, false);

                    if (bool.TryParse(dgv.Rows[e.RowIndex].Cells["RECENTLOGIN"].Value.ToString(), out bool recentlogin) && recentlogin == true)
                    {
                        using (var font = new Font(e.CellStyle.Font, FontStyle.Bold))
                        {
                            TextRenderer.DrawText(e.Graphics, call,
                                             font, e.CellBounds, e.CellStyle.ForeColor,
                                             TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);
                        }
                    }
                    else
                        TextRenderer.DrawText(e.Graphics, call,
                                         e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor,
                                         TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter);

                    e.Handled = true;
                }
                else if (e.ColumnIndex > dgv.Columns["AS"].DisplayIndex)
                {
                    if (e.Value == null)
                        return;
                    QRVdb.QRV_STATE state = QRVdb.QRV_STATE.unknown;
                    try
                    {
                        Enum.TryParse<QRVdb.QRV_STATE>(e.Value.ToString(), out state);
                    }
                    catch
                    {
                    }

                    Rectangle newRect = new Rectangle(e.CellBounds.X + 2,
                        e.CellBounds.Y + 2, e.CellBounds.Width - 4,
                        e.CellBounds.Height - 4);
                    e.PaintBackground(e.CellBounds, false);
                    if (QRVdb.worked(state))
                    {
                        e.Graphics.FillRectangle(Brushes.Green, newRect);
                        e.Handled = true;
                    }
                    else
                    {
                        switch (state)
                        {
                            case QRVdb.QRV_STATE.unknown:
                                e.Graphics.FillRectangle(Brushes.LightGray, newRect);
                                e.Handled = true;
                                break;
                            case QRVdb.QRV_STATE.qrv:
                                e.Graphics.FillRectangle(Brushes.Red, newRect);
                                e.Handled = true;
                                break;
                            case QRVdb.QRV_STATE.not_qrv:
                                e.Graphics.FillRectangle(Brushes.Black, newRect);
                                e.Handled = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (e.ColumnIndex == dgv.Columns["CONTACTED"].DisplayIndex)
                {

                    // last activity
                    var row = dgv.Rows[e.RowIndex];
                    var timeval = row.Cells["TIME"].Value;
                    e.PaintBackground(e.CellBounds, false);

                    if (timeval != null && timeval != DBNull.Value)
                    {
                        double lastActivityMinutes = (DateTime.UtcNow.Subtract((DateTime)timeval)).TotalMinutes;
                        string act_string;
                        if (lastActivityMinutes < 120.0)
                            act_string = lastActivityMinutes.ToString("0");
                        else
                        {
                            if ((int)row.Cells["CONTACTED"].Value < 3)
                                act_string = "---";
                            else
                                act_string = "xxx"; // if contacted 3 times without answer then probably really not available
                        }
                        StringFormat sf = new StringFormat
                        {
                            LineAlignment = StringAlignment.Center,
                            Alignment = StringAlignment.Center
                        };
                        e.Graphics.DrawString(act_string, e.CellStyle.Font, Brushes.Black, e.CellBounds.X + e.CellBounds.Width / 2,
                            e.CellBounds.Y + e.CellBounds.Height / 2, sf);
                    }
                    e.Handled = true;
                }
                else if (e.ColumnIndex == dgv.Columns["AS"].DisplayIndex)
                {
                    try
                    {
                        var row = dgv.Rows[e.RowIndex];
                        if (row.Cells["AS"].Value == null)
                            return;
                        string as_string = e.Value.ToString();

                        if (as_string.Length > 0)
                        {
                            if (as_string.Equals("<") || as_string.Equals(">"))
                            {
                                return;
                            }
                            e.PaintBackground(e.CellBounds, false);
                            string[] a = as_string.Split(new char[] { ',' });
                            if (a.Length < 3)
                            {
                                // draw nothing, just the background
                                return;
                            }
                            int pot = Convert.ToInt32(a[0]);
                            int Mins = Convert.ToInt32(a[2]);
                            if (Mins > 99)
                                Mins = 99;
                            if (pot > 0)
                            {
                                if (a.Length < 2)
                                    Console.WriteLine("ups " + a.Length + " " + as_string);
                                var cat = a[1];
                                paintAScell(pot, cat, Mins, e.CellBounds, e.Graphics);
                            }
                            e.Handled = true;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void lv_Calls_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0)
            {
                DataGridView dgv = sender as DataGridView;
                if (sort_by_dir && dgv.Columns[e.ColumnIndex].Name == "DIR")
                {
                    dgv.Sort(dgv.Columns["DIR"], ListSortDirection.Ascending);
                }
                else if (!sort_by_dir && dgv.Columns[e.ColumnIndex].Name == "CALL")
                {
                    dgv.Sort(dgv.Columns["CALL"], ListSortDirection.Ascending);
                }
                else if (e.ColumnIndex > dgv.Columns["AS"].DisplayIndex)
                    KSTUsrFilterSetTimer(3000, true); // when one changes the band status, wait 3s to give some time to make further changes until the row hides
            }
        }

        private Point OldMousePos = new Point(0, 0);

        private void lv_Calls_MouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            string ToolTipText = "";
            DataGridView dgv = sender as DataGridView;
            if (OldMousePos != Cursor.Position && e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                OldMousePos = Cursor.Position;
                var column = dgv.Columns[e.ColumnIndex];
                var row = dgv.Rows[e.RowIndex];

                if (row.Cells["CALL"].Value == null)
                    return;
                string call = row.Cells["CALL"].Value.ToString();
                string name = row.Cells["NAME"].Value.ToString();
                string loc = row.Cells["LOC"].Value.ToString();
                if (column.Name == "CALL" || column.Name == "NAME" || column.Name == "LOC" || column.Name == "CONTACTED")
                {
                    ToolTipText = string.Concat(new object[]
                    {
                        "Call:\t",
                        call,
                        "\nName:\t",
                        name,
                        "\nLoc:\t",
                        loc,
                        "\nQTF:\t",
                        WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc).ToString("000"),
                        "°\nQRB:\t",
                        WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc),
                        " km\n\nLeft click to\nSend Message."
                    });
                }
                if (column.Name == "AS")
                {
                    if (AS_if.IsActive())
                    {
                        string ascall = WCCheck.WCCheck.SanitizeCall(call.Replace("(", "").Replace(")", ""));
                        string s = AS_if.GetNearestPlanes(ascall);
                        if (string.IsNullOrEmpty(s))
                        {
                            ToolTipText = "No planes\n\nLeft click for map";
                            lock (CALL)
                            {
                                DataRow Row = CALL.Rows.Find(ascall);
                                if (Row != null )
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
                }
                if (!String.IsNullOrEmpty(column.Name) && column.Name[0] > '0' && column.Name[0] < '9')
                {
                    QRVdb.QRV_STATE state = QRVdb.QRV_STATE.unknown;
                    try
                    {
                        Enum.TryParse<QRVdb.QRV_STATE>(row.Cells[e.ColumnIndex].Value.ToString(), out state);
                    }
                    catch
                    {
                    }
                    if (!QRVdb.worked(state))
                    {
                        ToolTipText = column.Name.Replace("_", ".") + ": Left click to \ntoggle QRV info";
                    }
                    else
                    {
                        if (wtQSO != null)
                        {
                            string band = column.Name;
                            DataRow[] findrow = wtQSO.QSO.Select(string.Format("[CALL] LIKE '*{0}*' AND [BAND] = '{1}'", WCCheck.WCCheck.Cut(call), band.Replace(".", "_")));
                            if (findrow != null && findrow.Count() > 0)
                            {
                                ToolTipText = string.Concat(new object[]
                                {
                                    band.Replace("_", "."),
                                    ": ",
                                    findrow[0]["CALL"],
                                    "  ",
                                    findrow[0]["TIME"],
                                    "  ",
                                    findrow[0]["SENT"],
                                    "  ",
                                    findrow[0]["RCVD"],
                                    "  ",
                                    findrow[0]["LOC"]
                                });
                            }
                        }
                    }
                }
                ShowToolTip(ToolTipText, dgv, dgv.PointToClient(Cursor.Position));
            }
        }

        private void lv_Calls_scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                fill_AS_list();
            }
        }

        private void lv_Calls_clientSizeChanged(object sender, EventArgs e)
        {
            fill_AS_list();
        }

        private void lv_Calls_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            fill_AS_list();
        }

        private void lv_Calls_mousewheel_event(object sender, EventArgs e)
        {
            fill_AS_list();
        }

        // Disable cell selection
        private void lv_Calls_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
                e.Cell.Selected = false;
        }

        // Disable focus cues
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }


        private void fill_AS_list()
        {
            if (AS_if==null || !AS_if.IsActive() || lv_Calls == null || lv_Calls.RowCount == 0)
                return;

            string watchlist = "";
            List<AirScoutInterface.AS_Calls> AS_list = new List<AirScoutInterface.AS_Calls>();
            List<AirScoutInterface.AS_Calls> tmp_AS_list = new List<AirScoutInterface.AS_Calls>();

            string mycall = WCCheck.WCCheck.SanitizeCall(Settings.Default.KST_UserName);

            // find visible users
            for (int i = 0; i < lv_Calls.RowCount; i++)
            {
                try
                {
                    if (lv_Calls.Rows[i].Cells["LOC"].Value == null)
                        continue;
                    string dxloc = lv_Calls.Rows[i].Cells["LOC"].Value.ToString();
                    string dxcall = WCCheck.WCCheck.SanitizeCall(lv_Calls.Rows[i].Cells["CALL"].Value.ToString().TrimStart(
                        new char[] { '(' }).TrimEnd(new char[] { ')' }));

                    if (!mycall.Equals(dxcall))
                    {
                        int qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, dxloc);
                        var row = lv_Calls.Rows[i].Cells;//  DataBoundItem.GetType();
                        if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                        {
                            if (!row["AS"].Value.Equals("<"))
                                row["AS"].Value = "<";
                        }
                        else if (qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
                        {
                            watchlist += string.Concat(new string[] { ",", dxcall, ",", dxloc });
                            if (i >= lv_Calls.FirstDisplayedCell.RowIndex && i < lv_Calls.FirstDisplayedCell.RowIndex + lv_Calls.DisplayedRowCount(true))
                            {
                                AS_list.Add(new AirScoutInterface.AS_Calls(dxcall, dxloc));
                            }
                            else
                            {
                                tmp_AS_list.Add(new AirScoutInterface.AS_Calls(dxcall, dxloc));
                            }
                        }
                        else
                        {
                            if (!row["AS"].Value.Equals(">"))
                                row["AS"].Value = ">";
                        }
                    }
                }
                catch
                {
                }
            }
            // concat the currently not displayed calls to the end of AS_list
            AS_list.AddRange(tmp_AS_list);
            AS_if.update_AS_list(AS_list);

            lock (AS_watchlist)
            {
                AS_watchlist = watchlist;
            }
        }

        private void cmi_Calls_SendMessage_Click(object sender, EventArgs e)
        {
            if (lv_Calls.SelectedRows != null && lv_Calls.SelectedRows[0].HeaderCell.Value.ToString().Length > 0)
            {
                cb_Command.Text = "/cq " + lv_Calls.SelectedRows[0].HeaderCell.Value.ToString().Replace("(", "").Replace(")", "") + " ";
                cb_Command.Focus();
                cb_Command.SelectionStart = cb_Command.Text.Length;
                cb_Command.SelectionLength = 0;
            }
        }


        private void lv_Calls_ColumnClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            var column = dgv.Columns[e.ColumnIndex];

            // CALL column
            if (column.Name == "CALL")
            {
                if (hide_away)
                    hide_away = false;
                else
                    hide_away = true;
                KST_Update_Usr_Filter();
                return;
            }

            // LOCATOR column
            if (column.Name == "LOC")
            {
                if (sort_by_dir)
                {
                    sort_by_dir = false;
                    dgv.Sort(dgv.Columns["CALL"], ListSortDirection.Ascending);
                }
                else
                {
                    sort_by_dir = true;
                    dgv.Sort(dgv.Columns["DIR"], ListSortDirection.Ascending);
                }
                KST_Update_Usr_Filter();
                return;
            }

            // ACT column
            if (column.Name == "CONTACTED")
            {
                if (ignore_inactive)
                    ignore_inactive = false;
                else
                    ignore_inactive = true;
                KST_Update_Usr_Filter();
                return;
            }
            // band columns
            if (e.ColumnIndex > dgv.Columns["AS"].DisplayIndex)
            {
                if (hide_worked)
                    hide_worked = false;
                else
                    hide_worked = true;
                KST_Update_Usr_Filter();
                return;
            }
        }

        private string lv_Calls_control_shown_from_Call = "";

        private void lv_Calls_MouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                QRVdb.QRV_STATE state = QRVdb.QRV_STATE.unknown;
                DataGridView dgv = sender as DataGridView;

                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    var column = dgv.Columns[e.ColumnIndex];
                    var row = dgv.Rows[e.RowIndex];

                    string username = row.Cells["CALL"].Value.ToString().Replace("(", "").Replace(")", "");
                    string call = WCCheck.WCCheck.SanitizeCall(username);

                    if (column.Name == "CALL" || column.Name == "NAME" || column.Name == "LOC" || column.Name == "CONTACTED")
                    {
                        if (username.Length > 0 && KST.State == KSTcom.KST_STATE.Connected)
                        {
                            cb_Command.Text = "/cq " + username + " ";
                            cb_Command.SelectionStart = cb_Command.Text.Length;
                            cb_Command.SelectionLength = 0;
                        }
                    }
                    string loc = row.Cells["LOC"].Value.ToString();

                    if (column.Name == "AS")
                    {
                        AS_if.ShowPath(call, loc, Settings.Default.KST_UserName.ToUpper(), Settings.Default.KST_Loc);
                    }
                    if (column.Name[0] > '0' && column.Name[0] < '9')
                    {
                        lock (CALL)
                        {
                            // band columns

                            // look for base call - can yield several hits
                            DataRow[] call_rows = CALL.Select(string.Format("[CALL] LIKE '*{0}*'", call));
                            // do we have a matching local qrv entry?
                            var localQRVRow = qrv.match_call_loc_local(username, loc);

                            string band = column.Name;
                            state = QRVdb.QRV_STATE.unknown;
                            try
                            {
                                Enum.TryParse<QRVdb.QRV_STATE>(row.Cells[e.ColumnIndex].Value.ToString(), out state);
                            }
                            catch
                            {
                            }
                            if (!QRVdb.worked(state))
                            {
                                switch (state)
                                {
                                    case QRVdb.QRV_STATE.unknown:
                                        if (localQRVRow!=null)
                                        {
                                            foreach (var CallsRow in call_rows)
                                            {
                                                if (CallsRow["CALL"].Equals(localQRVRow["CALL"]))
                                                {
                                                    CallsRow[band] = QRVdb.QRV_STATE.qrv;
                                                    qrv.set_qrv_state(CallsRow["CALL"].ToString().Replace("(", "").Replace(")", ""),
                                                        loc, band, QRVdb.QRV_STATE.qrv);
                                                }
                                                else
                                                {
                                                    CallsRow[band] = QRVdb.QRV_STATE.not_qrv;
                                                    qrv.set_qrv_state(CallsRow["CALL"].ToString().Replace("(", "").Replace(")", ""),
                                                        loc, band, QRVdb.QRV_STATE.not_qrv);
                                                }
                                            }
                                        }
                                        else
                                            foreach (var CallsRow in call_rows)
                                                CallsRow[band] = QRVdb.QRV_STATE.qrv;
                                        break;
                                    case QRVdb.QRV_STATE.qrv:
                                        if (localQRVRow != null)
                                        {
                                            foreach (var CallsRow in call_rows)
                                            {
                                                CallsRow[band] = QRVdb.QRV_STATE.not_qrv;
                                                qrv.set_qrv_state(CallsRow["CALL"].ToString().Replace("(", "").Replace(")", ""),
                                                            loc, band, QRVdb.QRV_STATE.not_qrv);
                                            }
                                        }
                                        else
                                            foreach (var CallsRow in call_rows)
                                                CallsRow[band] = QRVdb.QRV_STATE.not_qrv;
                                        break;
                                    case QRVdb.QRV_STATE.not_qrv:
                                        foreach (var CallsRow in call_rows)
                                        {
                                            CallsRow[band] = QRVdb.QRV_STATE.unknown;
                                            qrv.set_qrv_state(CallsRow["CALL"].ToString().Replace("(", "").Replace(")", ""),
                                                        loc, band, QRVdb.QRV_STATE.unknown);
                                        }
                                        break;
                                }
                                dgv.Refresh();
                            }
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {

                string ToolTipText = "";
                DataGridView dgv = sender as DataGridView;

                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    var column = dgv.Columns[e.ColumnIndex];
                    var row = dgv.Rows[e.RowIndex];

                    if (column.Name == "CALL")
                    {
                        string call = WCCheck.WCCheck.SanitizeCall(row.Cells["CALL"].Value.ToString().Replace("(", "").Replace(")", ""));
                        DataRow[] selectRow = KST.MSG_findcall(call);

                        this.cmn_userlist_chatReview.Visible = (selectRow.Length > 0);
                        this.cmn_userlist_wtsked.Visible = (wts != null && wts.wtStatusList.Count > 0);

                        lv_Calls_control_shown_from_Call = call;

                        this.cmn_userlist.Show(lv_Calls, dgv.PointToClient(Cursor.Position));
                    }
                    else if (column.Name == "AS" && AS_if.IsActive())
                    {
                        string call = WCCheck.WCCheck.SanitizeCall(row.Cells["CALL"].Value.ToString().Replace("(", "").Replace(")", ""));
                        string s = AS_if.GetNearestPlanes(call);
                        if (string.IsNullOrEmpty(s))
                        {
                            ToolTipText = "No planes\n\nLeft click for map";
                            lock (CALL)
                            {
                                DataRow Row = CALL.Rows.Find(call);
                                if (Row != null)
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
                            ToolTipText = s + "\nLeft click for map";
                            ShowASToolTip(ToolTipText, dgv, dgv.PointToClient(Cursor.Position));
                            return;
                        }
                    }
                    ShowToolTip(ToolTipText, dgv, dgv.PointToClient(Cursor.Position));
                }
            }
        }

        private void lv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            // TODO we get this event when AS column changes - could be ignored, but no idea how to find out which column changed
            KSTUsrFilterSetTimer(100, false);
        }

        private string lv_msg_control_url_shown_from_Call = "";

        private void lv_Msg_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_Msg.HitTest(p);
            if (info != null && info.Item != null)
            {
                string call = info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "");
                if (call.Equals(Settings.Default.KST_UserName.ToUpper()))
                {
                    // if we would be sending to ourselves, use recipient call instead
                    call = info.Item.SubItems[3].Text.Split(new char[] { ' ' })[0].Replace("(", "").Replace(")", "");
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (call.Length > 0 && KST.State == KSTcom.KST_STATE.Connected)
                    {
                        cb_Command.Text = "/cq " + call + " ";
                        cb_Command.SelectionStart = cb_Command.Text.Length;
                        cb_Command.SelectionLength = 0;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    DataRow[] selectRow = KST.MSG_findcall(call);

                    DataRow userRow = CALL.Rows.Find(call);
                    int qrb = 0;
                    double qtf = 0.0;

                    if (userRow != null) 
                    {
                       string loc = userRow["LOC"].ToString();
                        qtf = WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc);
                        qrb = WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc);
                       this.cmn_msglist_toolStripTextBox_DirQRB.Text = string.Concat(new object[]
                       {
                            qtf.ToString("000"), "° ",
                            qrb, " km"
                       });
                       this.cmn_msglist_toolStripTextBox_DirQRB.Visible = true;
                    }
                    else
                        this.cmn_msglist_toolStripTextBox_DirQRB.Visible = false;

                    this.cmn_msglist_chatReview.Visible = (selectRow.Length > 0);
                    this.cmn_msglist_wtsked.Visible = (wts != null && wts.wtStatusList.Count > 0);

                    string msg = info.Item.SubItems[3].Text;
                    var rx = new Regex(@".*(https?://.*) .*", RegexOptions.Compiled);
                    if (rx.IsMatch(msg))
                    {
                        lv_msg_control_url_shown_from_Call = rx.Replace(msg, "$1"); // we only take the first match...

                        this.cmn_msglist_openURL.Visible = true;
                    }
                    else
                    {
                        this.cmn_msglist_openURL.Visible = false;
                    }
                    // we only display Airscout information if available and the user is the in the user list (otherwise we probably do not have enough data)
                    this.cmn_msglist_AS_details.Visible = AS_if.IsActive() && userRow != null;
                    if (AS_if.IsActive() && !String.IsNullOrEmpty(call) && userRow != null)
                    {
                        this.cmn_msglist_AS_status.Visible = true;
                        using (Graphics graphic = Graphics.FromImage(this.cmn_msglist_AS_status.Image))
                        using (SolidBrush brush = new SolidBrush(Control.DefaultBackColor))
                        {
                            graphic.FillRectangle(brush, new RectangleF(0, 0,
                                cmn_msglist_AS_status.Image.Width, cmn_msglist_AS_status.Image.Height));

                            if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                            {
                                TextRenderer.DrawText(graphic, "   < ", cmn_msglist_AS_status.Font, new Point(0,0), cmn_msglist_AS_status.ForeColor);
                            }
                            else
                            {
                                string ascall = WCCheck.WCCheck.SanitizeCall(call.Replace("(", "").Replace(")", ""));
                                string as_string = AS_if.GetNearestPlanePotential(ascall);

                                if (!string.IsNullOrEmpty(as_string) && as_string != "0")
                                {
                                    string[] a = as_string.Split(new char[] { ',' });
                                    if (a.Length == 3)
                                    {
                                        int pot = Convert.ToInt32(a[0]);
                                        int Mins = Convert.ToInt32(a[2]);
                                        if (Mins > 99)
                                            Mins = 99;
                                        if (pot > 0)
                                        {
                                            var cat = a[1];

                                                paintAScell(pot, cat, Mins, new Rectangle(0, 0, cmn_msglist_AS_status.Image.Width, cmn_msglist_AS_status.Image.Height), graphic);
                                        }
                                    }
                                }
                            }
                        }
                        this.cmn_msglist_AS_status.Text = "Click for map";
                    }
                    else
                        this.cmn_msglist_AS_status.Visible = false;

                    this.cmn_msglist.Show(lv_Msg, p);
                }
            }
        }

        private void cmn_msglist_toolStripLabel_AS_clicked(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = sender as ToolStripItem;
            if (AS_if.IsActive())
            {
                string call = cmn_userlist_get_call_from_contextMenu(clickedItem.Owner as ContextMenuStrip);

                if (!String.IsNullOrEmpty(call))
                {
                    DataRow userRow = CALL.Rows.Find(call);

                    if (userRow != null)
                    {
                        string loc = userRow["LOC"].ToString();

                        string ascall = WCCheck.WCCheck.SanitizeCall(call.Replace("(", "").Replace(")", ""));
                        AS_if.ShowPath(ascall, loc, Settings.Default.KST_UserName.ToUpper(), Settings.Default.KST_Loc);
                    }
                }
            }
        }

        private void cmn_item_AS_Click(object sender, MouseEventArgs e)
        {
            ToolStripItem clickedItem = sender as ToolStripItem;
            string call = cmn_userlist_get_call_from_contextMenu(clickedItem.Owner as ContextMenuStrip);

            if (!String.IsNullOrEmpty(call))
            {
                string ToolTipText;
                string ascall = WCCheck.WCCheck.SanitizeCall(call.Replace("(", "").Replace(")", ""));
                string s = AS_if.GetNearestPlanes(ascall);
                if (string.IsNullOrEmpty(s))
                {
                    ToolTipText = "No planes";
                    lock (CALL)
                    {
                        DataRow Row = CALL.Rows.Find(ascall);
                        if (Row != null && AS_if.IsActive())
                        {
                            int qrb = (int)Row["QRB"];
                            if (qrb < Convert.ToInt32(Settings.Default.AS_MinDist))
                                ToolTipText = "Too close for planes";
                            else if (qrb > Convert.ToInt32(Settings.Default.AS_MaxDist))
                                ToolTipText = "Too far away for planes";
                        }
                    }
                }
                else
                {
                    ToolTipText = s;
                }
                ShowASToolTip(ToolTipText, lv_Msg, lv_Msg.PointToClient(Cursor.Position));
                this.cmn_msglist_Label.Text = ToolTipText;
            }
        }

        private void lv_MyMsg_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_MyMsg.HitTest(p);
            if (info != null && info.Item != null)
            {
                string call = info.Item.SubItems[1].Text.Replace("(", "").Replace(")", "");
                if (call.Equals(Settings.Default.KST_UserName.ToUpper()))
                {
                    // if we would be sending to ourselves, use recipient call instead
                    call = info.Item.SubItems[3].Text.Split(new char[] { ' ' })[0].Replace("(", "").Replace(")", "");
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (call.Length > 0 && KST.State == KSTcom.KST_STATE.Connected)
                    {
                        cb_Command.Text = "/cq " + call + " ";
                        cb_Command.SelectionStart = cb_Command.Text.Length;
                        cb_Command.SelectionLength = 0;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    DataRow[] selectRow = KST.MSG_findcall(call);

                    this.cmn_msglist_chatReview.Visible = (selectRow.Length > 0);
                    this.cmn_msglist_wtsked.Visible = (wts != null && wts.wtStatusList.Count > 0);

                    string msg = info.Item.SubItems[3].Text;
                    var rx = new Regex(@".*(https?://.*) .*", RegexOptions.Compiled);
                    if (rx.IsMatch(msg))
                    {
                        lv_msg_control_url_shown_from_Call = rx.Replace(msg, "$1"); // we only take the first match...

                        this.cmn_msglist_openURL.Visible = true;
                    }
                    else
                    {
                        this.cmn_msglist_openURL.Visible = false;
                    }
                    DataRow userRow = CALL.Rows.Find(call);

                    if (userRow != null)
                    {
                        string loc = userRow["LOC"].ToString();
                        this.cmn_msglist_toolStripTextBox_DirQRB.Text = string.Concat(new object[]
                        {
                            WCCheck.WCCheck.QTF(Settings.Default.KST_Loc, loc).ToString("000"), "° ",
                            WCCheck.WCCheck.QRB(Settings.Default.KST_Loc, loc), " km"
                        });
                        this.cmn_msglist_toolStripTextBox_DirQRB.Visible = true;
                    }
                    else
                        this.cmn_msglist_toolStripTextBox_DirQRB.Visible = false;

                    this.cmn_msglist.Show(lv_MyMsg, p);
                }
            }
        }

        private void lv_Msg_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_Msg.HitTest(p);
            if (OldMousePos != p && info != null && info.SubItem != null && !ToolTip_AS_active)
            {
                OldMousePos = p;
                if (info.SubItem.Name == "Messages")
                {
                    ShowToolTip(info.Item.SubItems[3].Text, lv_Msg, p);
                }
                else
                {
                    ShowToolTip("Left click to\nSend Message.\nRight click for more", lv_Msg, p);
                }
            }
        }

        private void lv_MyMsg_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            ListViewHitTestInfo info = lv_MyMsg.HitTest(p);
            if (OldMousePos != p && info != null && info.SubItem != null && !ToolTip_AS_active)
            {
                OldMousePos = p;
                if (info.SubItem.Name == "Messages")
                {
                    ShowToolTip(info.Item.SubItems[3].Text, lv_MyMsg, p);
                }
                else
                {
                    ShowToolTip("Left click to\nSend Message.\nRight click for more", lv_MyMsg, p);
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


        /// <summary>
        /// return frequency of the band named by wavelength "band"
        /// </summary>
        /// <param name="band">wavelength in the form nn m/cm/mm like "23 cm"</param>
        /// <returns>frequency in MHz</returns>
        private uint get_sked_band_qrg(string band)
        {
            foreach (var b in new List<bandinfo>(selected_bands()))
            {
                if (b.band_wavelength_name.Equals(band))
                    return b.band_center_activity / 1000;
            }
            return 0;
        }

        private string cmn_userlist_get_call_from_contextMenu(ContextMenuStrip contextMenu)
        {
            var Control = contextMenu.SourceControl as Control;
            kst_sked_mode = "SSB";

            if (Control.Name.Equals("lv_Calls"))
            {
                Console.WriteLine(Control.GetType().ToString());
                kst_sked_qrg = 0;
                // FIXME DV das geht so nicht, keine Ahnung...
                //var yourControl = contextMenu.SourceControl as DataGridView;
                //if (yourControl.SelectedRows.Count > 0)
                //{
                //    string call = yourControl.SelectedRows[0].Cells["CALL"].Value.ToString().Replace("(", "").Replace(")", "");
                //    Console.WriteLine("clicked " + call);
                //    return call;
                //}

                if (!String.IsNullOrEmpty(lv_Calls_control_shown_from_Call))
                    return lv_Calls_control_shown_from_Call;
            }
            else if (Control.Name.Equals("lv_Msg") || Control.Name.Equals("lv_MyMsg"))
            {
                Console.WriteLine(Control.GetType().ToString());
                var yourControl = contextMenu.SourceControl as System.Windows.Forms.ListView;
                if (yourControl.SelectedItems.Count > 0)
                {
                    string call = yourControl.SelectedItems[0].SubItems[1].Text.Replace("(", "").Replace(")", "");
                    if (call.Equals(Settings.Default.KST_UserName.ToUpper()))
                    {
                        // if we clicked on our own call use recipient call instead (in SubItems[3] column)
                        call = yourControl.SelectedItems[0].SubItems[3].Text.Split(new char[] { ' ' })[0].Replace("(", "").Replace(")", "");
                    }
                    kst_sked_band_freq = 0;
                    //string pattern = @"\.?(\d{2,3})";
                    string pattern = @"\.?(\d{2,3})(?!(\s*cm))";
                    string note = yourControl.SelectedItems[0].SubItems[3].Text;
                    Match m = Regex.Match(note, pattern);
                    if (m.Success)
                    {
                        Console.WriteLine(m.Value);
                        if (!uint.TryParse(m.Value.Replace(".", ""), out kst_sked_qrg))
                        {
                            kst_sked_qrg = 0;
                        }
                    }
                    pattern = @"(\d{1,2})\s?(c?m)";
                    m = Regex.Match(note, pattern);
                    if (m.Success)
                    {
                        kst_sked_band_freq = get_sked_band_qrg(m.Groups[1].Value + " " + m.Groups[2].Value);
                    }
                    pattern = @"(\d{3,4})\.(\d{2,3})";
                    m = Regex.Match(note, pattern);
                    if (m.Success)
                    {
                        Console.WriteLine(m.Value);
                        uint.TryParse(m.Groups[2].Value, out kst_sked_qrg);
                        uint.TryParse(m.Groups[1].Value, out kst_sked_band_freq);
                    }
                    pattern = @"\bcw\b";
                    m = Regex.Match(note, pattern, RegexOptions.IgnoreCase);
                    if (m.Success)
                        kst_sked_mode = "CW";
                    return call;
                }
            }
            return "";
        }


        private void cmn_item_wtsked_Click(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = sender as ToolStripItem;
            string call = cmn_userlist_get_call_from_contextMenu(clickedItem.Owner as ContextMenuStrip);

            if (!String.IsNullOrEmpty(call))
            {
                string notes = "KST - Offline";
                DataRow findrow = CALL.Rows.Find(call);
                // [JO02OB - 113\\260] AP in 2min
                if (findrow != null)
                    notes = String.Format("[{0} - {1}°]", findrow["LOC"].ToString(), findrow["DIR"].ToString());
                wtskdlg = new WTSkedDlg(WCCheck.WCCheck.SanitizeCall(call), wts.wtStatusList, new BindingList<bandinfo>(selected_bands()),
                                        notes, last_sked_qrg, kst_sked_qrg, kst_sked_mode, kst_sked_band_freq);
                if (wtskdlg.ShowDialog() == DialogResult.OK)
                {
                    WinTest.wtSked wtsked = new WinTest.wtSked();

                    wtsked.send_locksked(wtskdlg.target_wt);
                    last_sked_qrg = wtskdlg.qrg;
                    wtsked.send_addsked(wtskdlg.target_wt, wtskdlg.sked_time, wtskdlg.qrg, wtskdlg.band, wtskdlg.mode,
                        wtskdlg.call, wtskdlg.notes);
                    wtsked.send_unlocksked(wtskdlg.target_wt);
                }
            }
        }

        private void cmn_item_chatReview_Click(object sender, EventArgs e)
        {
            ToolStripItem clickedItem = sender as ToolStripItem;
            string call = cmn_userlist_get_call_from_contextMenu(clickedItem.Owner as ContextMenuStrip);

            if (!String.IsNullOrEmpty(call))
            {
                DataTable chat_review_table = new DataTable("ChatReviewTable");
                chat_review_table.Columns.Add("Time", typeof(DateTime));
                chat_review_table.Columns.Add("From");
                chat_review_table.Columns.Add("Message");

                string call_cut = WCCheck.WCCheck.SanitizeCall(call);
                DataRow[] selectRow = KST.MSG_findcall(call_cut);
                foreach (var msg_row in selectRow) // potential issue: collection may change while this executes
                {
                    //Console.WriteLine(msg_row["TIME"].ToString() + " " + msg_row["CALL"].ToString() + " -> " + msg_row["RECIPIENT"].ToString() + " " + msg_row["MSG"].ToString());
                    chat_review_table.Rows.Add(msg_row["TIME"], msg_row["CALL"], msg_row["MSG"]);
                }
                // we need to sort by time - so that the newest entry is the first
                DataView view = chat_review_table.DefaultView;
                view.Sort = "TIME DESC";

                ChatReview cr = new ChatReview(view, call);
                cr.ShowDialog();
            }
        }

        private void cmn_item_openURL_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(lv_msg_control_url_shown_from_Call) &&
                Uri.IsWellFormedUriString(lv_msg_control_url_shown_from_Call, UriKind.Absolute))
            {
                System.Diagnostics.Process.Start(lv_msg_control_url_shown_from_Call);
            }
        }

        private void btn_KST_Send_Click(object sender, EventArgs e)
        {
            if (!KST.Send(cb_Command.Text))
            {
                MessageBox.Show("Sending commands except \"/cq\", \"/shloc\" and \"/shuser\" is not allowed!", "KST SendCommand");
            }
            else
            {
                try
                {
                    last_cq_call = cb_Command.Text.Split(' ')[1];
                    MainDlg.Log.WriteMessage("KST message send: " + cb_Command.Text);
                    if (cb_Command.FindStringExact(cb_Command.Text) != 0)
                    {
                        cb_Command.Items.Insert(0, cb_Command.Text);
                    }
                }
                catch
                { }
            }
            cb_Command.ResetText();
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
                {
                    cp = lv_Msg.PointToClient(p);
                    ListViewHitTestInfo info = lv_Msg.HitTest(cp);
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
            if (Settings.Default.KST_AutoConnect && KST.State == KSTcom.KST_STATE.Standby)
            {
                KST.Connect();
            }
        }

        /* send the current list of CALLS to AS server - allows AS to batch process
         */
        private void AS_send_ASWATCHLIST()
        {
            if (AS_watchlist.Equals(""))
                return;

            lock (AS_watchlist)
            {
                string mycall = WCCheck.WCCheck.SanitizeCall(Settings.Default.KST_UserName);
                AS_if.SendWatchlist(AS_watchlist, mycall, Settings.Default.KST_Loc);
            }
        }

        private void AS_UpdateASStatus(object sender, AirScoutInterface.UpdateASStatusEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<AirScoutInterface.UpdateASStatusEventArgs>(AS_UpdateASStatus), new object[] { sender, args });
                return;
            }
            if (args.dxcall == null)
            {
                // remove all AS information from CALL
                try
                {
                    lv_Calls.SuspendLayout();
                    for (int i = 0; i < CALL.Rows.Count; i++)
                    {
                        CALL.Rows[i]["AS"] = "";
                    }
                    lv_Calls.ResumeLayout();
                }
                catch
                {
                }

            }
            else
            {
                // search the view for matching call - note that dxcall is the bare callsign, whereas
                // the list contains () for users that are away and may contain things like /p
                // so this is safer...
                DataRow[] call_rows = CALL.Select(string.Format("[CALL] LIKE '*{0}*'", args.dxcall));

                foreach (var c in call_rows)
                {
                    if (c["AS"].ToString() != args.newtext)
                    {
                        c["AS"] = args.newtext;
                    }
                }
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
            this.tsl_LED_AS_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsl_LED_Log_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsl_LED_KST_Status = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.lv_Calls = new System.Windows.Forms.DataGridView();
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
            this.macroToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_3 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_4 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_5 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_6 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_7 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_8 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_9 = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_btn_macro_0 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.macro_default_Station = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ti_Main = new System.Windows.Forms.Timer(this.components);
            this.ni_Main = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmn_Notify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmi_Notify_Restore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmi_Notify_Quit = new System.Windows.Forms.ToolStripMenuItem();
            this.ti_Error = new System.Windows.Forms.Timer(this.components);
            this.ti_Top = new System.Windows.Forms.Timer(this.components);
            this.ti_Reconnect = new System.Windows.Forms.Timer(this.components);
            this.tt_Info = new System.Windows.Forms.ToolTip(this.components);
            this.tt_ASInfo = new System.Windows.Forms.ToolTip(this.components);
            this.ti_UpdateFilter = new System.Windows.Forms.Timer(this.components);
            this.cmn_userlist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmn_userlist_wtsked = new System.Windows.Forms.ToolStripMenuItem();
            this.cmn_userlist_chatReview = new System.Windows.Forms.ToolStripMenuItem();
            this.cmn_msglist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmn_msglist_toolStripTextBox_DirQRB = new System.Windows.Forms.ToolStripTextBox();
            this.cmn_msglist_AS_status = new System.Windows.Forms.ToolStripLabel();
            this.cmn_msglist_Label = new System.Windows.Forms.Label();
            this.cmn_msglist_wtsked = new System.Windows.Forms.ToolStripMenuItem();
            this.cmn_msglist_chatReview = new System.Windows.Forms.ToolStripMenuItem();
            this.cmn_msglist_openURL = new System.Windows.Forms.ToolStripMenuItem();
            this.cmn_msglist_AS_details = new System.Windows.Forms.ToolStripMenuItem();
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
            ((System.ComponentModel.ISupportInitialize)(this.lv_Calls)).BeginInit();
            this.mn_Main.SuspendLayout();
            this.cmn_Notify.SuspendLayout();
            this.cmn_userlist.SuspendLayout();
            this.cmn_msglist.SuspendLayout();
            this.SuspendLayout();
            // 
            // ss_Main
            // 
            this.ss_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsl_Info,
            this.tsl_Error,
            this.tsl_LED_KST_Status,
            this.tsl_LED_AS_Status,
            this.tsl_LED_Log_Status});
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
            this.tsl_Error.Text = "";
            this.tsl_Error.Size = new System.Drawing.Size(1108, 17);
            this.tsl_Error.Spring = true;
            this.tsl_Error.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tsl_LED_KST_Status
            // 
            this.tsl_LED_KST_Status.AutoSize = false;
            this.tsl_LED_KST_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.tsl_LED_KST_Status.Margin = new System.Windows.Forms.Padding(7, 5, 1, 5);
            this.tsl_LED_KST_Status.Name = "tsl_LED_KST_Status";
            this.tsl_LED_KST_Status.Size = new System.Drawing.Size(12, 12);
            this.tsl_LED_KST_Status.Text = "ON4KST Chat Status LED";
            this.tsl_LED_KST_Status.ToolTipText = "ON4KST Chat Status LED";
            this.tsl_LED_KST_Status.BackColor = Color.Red;
            // 
            // tsl_LED_AS_Status
            // 
            this.tsl_LED_AS_Status.AutoSize = false;
            this.tsl_LED_AS_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.tsl_LED_AS_Status.Margin = new System.Windows.Forms.Padding(7, 5, 1, 5);
            this.tsl_LED_AS_Status.Name = "tsl_LED_AS_Status";
            this.tsl_LED_AS_Status.Size = new System.Drawing.Size(12, 12);
            this.tsl_LED_AS_Status.Text = "Airscout Status LED";
            this.tsl_LED_AS_Status.ToolTipText = "Airscout Status LED";
            this.tsl_LED_AS_Status.BackColor = Color.Red;
            // 
            // tsl_LED_Log_Status
            // 
            this.tsl_LED_Log_Status.AutoSize = false;
            this.tsl_LED_Log_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.tsl_LED_Log_Status.Margin = new System.Windows.Forms.Padding(7, 5, 1, 5);
            this.tsl_LED_Log_Status.Name = "tsl_LED_AS_Status";
            this.tsl_LED_Log_Status.Size = new System.Drawing.Size(12, 12);
            this.tsl_LED_Log_Status.Text = "Log sync Status LED";
            this.tsl_LED_Log_Status.ToolTipText = "Log sync Status LED";
            this.tsl_LED_Log_Status.BackColor = Color.Red;
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
            this.cb_Command.TabIndex = 1;
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
            this.btn_KST_Send.TabIndex = 2;
            this.btn_KST_Send.Text = "Send";
            this.btn_KST_Send.UseVisualStyleBackColor = true;
            this.btn_KST_Send.Click += new System.EventHandler(this.btn_KST_Send_Click);
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
            this.lbl_Call.Location = new System.Drawing.Point(587, 41);
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
            this.lv_Msg.HideSelection = false;
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
            this.lv_MyMsg.HideSelection = false;
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
            this.lv_Calls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_Calls.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_Calls.Location = new System.Drawing.Point(0, 24);
            this.lv_Calls.MultiSelect = false;
            this.lv_Calls.Name = "lv_Calls";
            this.lv_Calls.ReadOnly = true;
            this.lv_Calls.RowHeadersVisible = false;
            this.lv_Calls.RowTemplate.Height = 17;
            this.lv_Calls.ShowCellToolTips = false;
            this.lv_Calls.Size = new System.Drawing.Size(353, 658);
            this.lv_Calls.TabIndex = 14;
            this.lv_Calls.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lv_Calls_MouseDown);
            this.lv_Calls.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lv_Calls_MouseMove);
            this.lv_Calls.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.lv_Calls_CellPainting);
            this.lv_Calls.CellStateChanged += new System.Windows.Forms.DataGridViewCellStateChangedEventHandler(this.lv_Calls_CellStateChanged);
            this.lv_Calls.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.lv_Calls_CellValueChanged);
            this.lv_Calls.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lv_Calls_ColumnClick);
            this.lv_Calls.ClientSizeChanged += new System.EventHandler(this.lv_Calls_clientSizeChanged);
            this.lv_Calls.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.lv_Calls_DataBindingComplete);
            this.lv_Calls.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.lv_Calls_mousewheel_event);
            this.lv_Calls.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.lv_DataBindingComplete);
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
            this.macroToolStripMenuItem,
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
            this.toolStripSeparator1.Size = new System.Drawing.Size(90, 6);
            // 
            // tsi_File_Exit
            // 
            this.tsi_File_Exit.Name = "tsi_File_Exit";
            this.tsi_File_Exit.Size = new System.Drawing.Size(93, 22);
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
            this.tsm_KST.Size = new System.Drawing.Size(38, 20);
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
            // macroToolStripMenuItem
            // 
            this.macroToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_btn_macro_1,
            this.menu_btn_macro_2,
            this.menu_btn_macro_3,
            this.menu_btn_macro_4,
            this.menu_btn_macro_5,
            this.menu_btn_macro_6,
            this.menu_btn_macro_7,
            this.menu_btn_macro_8,
            this.menu_btn_macro_9,
            this.menu_btn_macro_0,
            this.toolStripSeparator4,
            this.macro_default_Station});
            this.macroToolStripMenuItem.Name = "macroToolStripMenuItem";
            this.macroToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.macroToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.macroToolStripMenuItem.Text = "&Macros";
            // 
            // menu_btn_macro_1
            // 
            this.menu_btn_macro_1.Name = "menu_btn_macro_1";
            this.menu_btn_macro_1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
            this.menu_btn_macro_1.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_1.Text = global::wtKST.Properties.Settings.Default.KST_Macro_1;
            this.menu_btn_macro_1.Visible = global::wtKST.Properties.Settings.Default.KST_M1;
            this.menu_btn_macro_1.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_2
            // 
            this.menu_btn_macro_2.Name = "menu_btn_macro_2";
            this.menu_btn_macro_2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
            this.menu_btn_macro_2.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_2.Text = global::wtKST.Properties.Settings.Default.KST_Macro_2;
            this.menu_btn_macro_2.Visible = global::wtKST.Properties.Settings.Default.KST_M2;
            this.menu_btn_macro_2.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_3
            // 
            this.menu_btn_macro_3.Name = "menu_btn_macro_3";
            this.menu_btn_macro_3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
            this.menu_btn_macro_3.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_3.Text = global::wtKST.Properties.Settings.Default.KST_Macro_3;
            this.menu_btn_macro_3.Visible = global::wtKST.Properties.Settings.Default.KST_M3;
            this.menu_btn_macro_3.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_4
            // 
            this.menu_btn_macro_4.Name = "menu_btn_macro_4";
            this.menu_btn_macro_4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
            this.menu_btn_macro_4.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_4.Text = global::wtKST.Properties.Settings.Default.KST_Macro_4;
            this.menu_btn_macro_4.Visible = global::wtKST.Properties.Settings.Default.KST_M4;
            this.menu_btn_macro_4.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_5
            // 
            this.menu_btn_macro_5.Name = "menu_btn_macro_5";
            this.menu_btn_macro_5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
            this.menu_btn_macro_5.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_5.Text = global::wtKST.Properties.Settings.Default.KST_Macro_5;
            this.menu_btn_macro_5.Visible = global::wtKST.Properties.Settings.Default.KST_M5;
            this.menu_btn_macro_5.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_6
            // 
            this.menu_btn_macro_6.Name = "menu_btn_macro_6";
            this.menu_btn_macro_6.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D6)));
            this.menu_btn_macro_6.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_6.Text = global::wtKST.Properties.Settings.Default.KST_Macro_6;
            this.menu_btn_macro_6.Visible = global::wtKST.Properties.Settings.Default.KST_M6;
            this.menu_btn_macro_6.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_7
            // 
            this.menu_btn_macro_7.Name = "menu_btn_macro_7";
            this.menu_btn_macro_7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D7)));
            this.menu_btn_macro_7.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_7.Text = global::wtKST.Properties.Settings.Default.KST_Macro_7;
            this.menu_btn_macro_7.Visible = global::wtKST.Properties.Settings.Default.KST_M7;
            this.menu_btn_macro_7.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_8
            // 
            this.menu_btn_macro_8.Name = "menu_btn_macro_8";
            this.menu_btn_macro_8.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D8)));
            this.menu_btn_macro_8.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_8.Text = global::wtKST.Properties.Settings.Default.KST_Macro_8;
            this.menu_btn_macro_8.Visible = global::wtKST.Properties.Settings.Default.KST_M8;
            this.menu_btn_macro_8.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_9
            // 
            this.menu_btn_macro_9.Name = "menu_btn_macro_9";
            this.menu_btn_macro_9.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D9)));
            this.menu_btn_macro_9.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_9.Text = global::wtKST.Properties.Settings.Default.KST_Macro_9;
            this.menu_btn_macro_9.Visible = global::wtKST.Properties.Settings.Default.KST_M9;
            this.menu_btn_macro_9.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // menu_btn_macro_0
            // 
            this.menu_btn_macro_0.Name = "menu_btn_macro_0";
            this.menu_btn_macro_0.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.menu_btn_macro_0.Size = new System.Drawing.Size(337, 22);
            this.menu_btn_macro_0.Text = global::wtKST.Properties.Settings.Default.KST_Macro_0;
            this.menu_btn_macro_0.Visible = global::wtKST.Properties.Settings.Default.KST_M0;
            this.menu_btn_macro_0.Click += new System.EventHandler(this.kst_macro_btn_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(334, 6);
            // 
            // macro_default_Station
            // 
            this.macro_default_Station.Name = "macro_default_Station";
            this.macro_default_Station.Size = new System.Drawing.Size(337, 22);
            this.macro_default_Station.Text = "Default Station ";
            this.macro_default_Station.DropDownOpening += new System.EventHandler(this.macro_default_Station_DropDownOpening);
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
            // ti_ToolTip_active
            //
            this.ti_ToolTip_active.Interval = 10000;
            this.ti_ToolTip_active.Tick += new System.EventHandler(ti_ToolTip_active_Tick);
            // 
            // tt_Info
            // 
            this.tt_Info.ShowAlways = true;
            // 
            // ti_UpdateFilter
            // 
            this.ti_UpdateFilter.Tick += new System.EventHandler(this.ti_UpdateFilter_Tick);
            // 
            // tt_ASInfo
            // 
            this.tt_ASInfo.ShowAlways = true;
            this.tt_ASInfo.OwnerDraw = true;
            this.tt_ASInfo.Popup += new PopupEventHandler(this.ToolTipp_AS_Popup);
            this.tt_ASInfo.Draw += new DrawToolTipEventHandler(this.ToolTipp_AS_Draw);
            // 
            // cmn_userlist
            // 
            this.cmn_userlist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmn_userlist_wtsked,
            this.cmn_userlist_chatReview});
            this.cmn_userlist.Name = "cmn_userlist";
            this.cmn_userlist.Size = new System.Drawing.Size(149, 48);
            // 
            // cmn_userlist_wtsked
            // 
            this.cmn_userlist_wtsked.Name = "cmn_userlist_wtsked";
            this.cmn_userlist_wtsked.Size = new System.Drawing.Size(148, 22);
            this.cmn_userlist_wtsked.Text = "Win-Test &Sked";
            this.cmn_userlist_wtsked.Click += new System.EventHandler(this.cmn_item_wtsked_Click);
            // 
            // cmn_userlist_chatReview
            // 
            this.cmn_userlist_chatReview.Name = "cmn_userlist_chatReview";
            this.cmn_userlist_chatReview.Size = new System.Drawing.Size(148, 22);
            this.cmn_userlist_chatReview.Text = "Chat &Review";
            this.cmn_userlist_chatReview.Click += new System.EventHandler(this.cmn_item_chatReview_Click);
            // 
            // cmn_msglist
            // 
            this.cmn_msglist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmn_msglist_toolStripTextBox_DirQRB,
            this.cmn_msglist_wtsked,
            this.cmn_msglist_chatReview,
            this.cmn_msglist_openURL,
            this.cmn_msglist_AS_status,
            this.cmn_msglist_AS_details
            });
            this.cmn_msglist.Name = "cmn_msglist";
            this.cmn_msglist.Size = new System.Drawing.Size(181, 117);
            this.cmn_msglist.ImageScalingSize = new System.Drawing.Size(40, 20);

            // 
            // cmn_msglist_Label
            // 
            this.cmn_msglist_Label.AutoSize = true;
            // 
            // cmn_msglist_toolStripTextBox_DirQRB
            // 
            this.cmn_msglist_toolStripTextBox_DirQRB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cmn_msglist_toolStripTextBox_DirQRB.Name = "toolStripTextBox_CallQRBDir";
            this.cmn_msglist_toolStripTextBox_DirQRB.ReadOnly = true;
            this.cmn_msglist_toolStripTextBox_DirQRB.Size = new System.Drawing.Size(100, 23);
            this.cmn_msglist_toolStripTextBox_DirQRB.Text = "Call 1000km 350°";
            // 
            // 
            // cmn_msglist_AS_status
            // 
            this.cmn_msglist_AS_status.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.cmn_msglist_AS_status.Name = "cmn_msglist_toolStripTextBox_AS";
            this.cmn_msglist_AS_status.Size = new System.Drawing.Size(206, 23);
            this.cmn_msglist_AS_status.Text = "";
            this.cmn_msglist_AS_status.Image = new Bitmap(40, 20);
            this.cmn_msglist_AS_status.Click += new EventHandler(cmn_msglist_toolStripLabel_AS_clicked);
            // 
            // cmn_msglist_wtsked
            // 
            this.cmn_msglist_wtsked.Name = "cmn_msglist_wtsked";
            this.cmn_msglist_wtsked.Size = new System.Drawing.Size(148, 22);
            this.cmn_msglist_wtsked.Text = "Win-Test &Sked";
            this.cmn_msglist_wtsked.Click += new System.EventHandler(this.cmn_item_wtsked_Click);
            // 
            // cmn_msglist_chatReview
            // 
            this.cmn_msglist_chatReview.Size = new System.Drawing.Size(148, 22);
            this.cmn_msglist_chatReview.Name = "cmn_msglist_chatReview";
            this.cmn_msglist_chatReview.Text = "Chat &Review";
            this.cmn_msglist_chatReview.Click += new System.EventHandler(this.cmn_item_chatReview_Click);
            // 
            // cmn_msglist_openURL
            // 
            this.cmn_msglist_openURL.Size = new System.Drawing.Size(148, 22);
            this.cmn_msglist_openURL.Name = "cmn_msglist_openURL";
            this.cmn_msglist_openURL.Text = "&Open URL";
            this.cmn_msglist_openURL.Click += new System.EventHandler(this.cmn_item_openURL_Click);
            // 
            // cmn_msglist_AS_details
            // 
            this.cmn_msglist_AS_details.Size = new System.Drawing.Size(200, 22);
            this.cmn_msglist_AS_details.Name = "cmn_msglist_AS";
            this.cmn_msglist_AS_details.Text = "&Airscout Details";
            this.cmn_msglist_AS_details.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cmn_item_AS_Click);
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
            ((System.ComponentModel.ISupportInitialize)(this.lv_Calls)).EndInit();
            this.mn_Main.ResumeLayout(false);
            this.mn_Main.PerformLayout();
            this.cmn_Notify.ResumeLayout(false);
            this.cmn_userlist.ResumeLayout(false);
            this.cmn_msglist.ResumeLayout(false);
            this.cmn_msglist.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox1.ShowDialog();
        }

        private void KST_Macro(object sender)
        {
            ulong passfreq = 0;
            ulong runfreq = last_sked_qrg;
            string band = " ";


            for (int i = 0; i < macro_default_Station.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = macro_default_Station.DropDownItems[i] as ToolStripMenuItem;
                if (item.Checked)
                {
                    var wtElem = wts.wtStatusList.SingleOrDefault(x => x.from == item.Text);

                    if (wtElem != null)
                    {
                        passfreq = wtElem.passfreq;
                        band = wtElem.band; // TODO 2_3GHz -> 2.3GHz or 13cm
                        runfreq = wtElem.freq;
                    }
                }
            }


            string call;
            if (!string.IsNullOrEmpty(cb_Command.Text) && this.cb_Command.Text.Contains(' '))
                call = this.cb_Command.Text.Split(' ')[1];
            else if (!string.IsNullOrEmpty(last_cq_call))
                call = last_cq_call;
            else
                call = Settings.Default.KST_UserName.ToUpper();

            string cmdLine = "";

            if (sender.GetType().Name == "ToolStripMenuItem")
            {
                ToolStripMenuItem tmi = sender as ToolStripMenuItem;
                cmdLine = tmi.Text;
            }

            cmdLine = cmdLine.Replace("$CALL", call);

            String freqSTR = (runfreq / 1000).ToString();
            if (freqSTR.Length > 3)
            {
                freqSTR = "." + freqSTR.Substring(freqSTR.Length - 3);
                cmdLine = cmdLine.Replace("$FREQ", freqSTR);
            }
            String passfreqSTR = (passfreq / 1000).ToString();
            if (passfreqSTR.Length > 3)
            {
                passfreqSTR = "." + passfreqSTR.Substring(passfreqSTR.Length - 3);
                cmdLine = cmdLine.Replace("$PASSFREQ", passfreqSTR);
            }
            cmdLine = cmdLine.Replace("$BAND", band);

            DataRow row = CALL.Rows.Find(call);
            string name = "";
            if (row != null)
                name = parseNameFromNameInfo(row["NAME"].ToString());
            cmdLine = cmdLine.Replace("$NAME", name);

            cb_Command.Text = cmdLine;
            cb_Command.SelectionStart = cmdLine.Length;
            cb_Command.SelectionLength = 0;
        }

        private static string parseNameFromNameInfo(string nameInfo)
        {
            Regex r = new Regex("^(\\p{L}+([/ -]\\p{L}+)*)");
            nameInfo = Regex.Replace(nameInfo, "(?:not qrv)|(?:(?:only )?SWL)", "", RegexOptions.IgnoreCase);
            Match m = r.Match(nameInfo);
            if (m.Success)
                return m.Value.Trim();
            else
                return "";
        }

        private void kst_macro_btn_Click(object sender, EventArgs e)
        {
            KST_Macro(sender);
        }

        private void update_station_selection_list()
        {
            string selItem = " ";
            for (int i = 0; i < macro_default_Station.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = macro_default_Station.DropDownItems[i] as ToolStripMenuItem;
                if (item.Checked)
                    selItem = macro_default_Station.DropDownItems[i].Text;
            }
            macro_default_Station.DropDownItems.Clear();
            if (wts != null)
            {
                for (int i = 0; i < wts.wtStatusList.Count; i++)
                {
                    macro_default_Station.DropDownItems.Add(wts.wtStatusList[i].from);
                    ToolStripMenuItem item = macro_default_Station.DropDownItems[i] as ToolStripMenuItem;
                    //item.CheckOnClick = true;
                    item.Click += new System.EventHandler(this.macro_default_Station_CheckedChanged);
                    if (item.Text == selItem)
                        item.Checked = true;
                }
            }
        }

        private void macro_default_Station_DropDownOpening(object sender, EventArgs e)
        {
            update_station_selection_list();
        }

        private void macro_default_Station_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem selItem = sender as ToolStripMenuItem;
            bool selItem_State = selItem.Checked;

            for (int i = 0; i < macro_default_Station.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item = macro_default_Station.DropDownItems[i] as ToolStripMenuItem;
                item.Checked = false;
            }

            selItem.Checked = !selItem_State;

        }

        private void macro_RefreshMacroText()
        {
            menu_btn_macro_1.Enabled = Settings.Default.KST_M1;
            menu_btn_macro_1.Visible = Settings.Default.KST_M1;
            if (!Settings.Default.KST_Macro_1.Equals(menu_btn_macro_1.Text))
                menu_btn_macro_1.Text = Settings.Default.KST_Macro_1;
            menu_btn_macro_2.Enabled = Settings.Default.KST_M2;
            menu_btn_macro_2.Visible = Settings.Default.KST_M2;
            if (!Settings.Default.KST_Macro_2.Equals(menu_btn_macro_2.Text))
                menu_btn_macro_2.Text = Settings.Default.KST_Macro_2;
            menu_btn_macro_3.Enabled = Settings.Default.KST_M3;
            menu_btn_macro_3.Visible = Settings.Default.KST_M3;
            if (!Settings.Default.KST_Macro_3.Equals(menu_btn_macro_3.Text))
                menu_btn_macro_3.Text = Settings.Default.KST_Macro_3;
            menu_btn_macro_4.Enabled = Settings.Default.KST_M4;
            menu_btn_macro_4.Visible = Settings.Default.KST_M4;
            if (!Settings.Default.KST_Macro_4.Equals(menu_btn_macro_4.Text))
                menu_btn_macro_4.Text = Settings.Default.KST_Macro_4;
            menu_btn_macro_5.Enabled = Settings.Default.KST_M5;
            menu_btn_macro_5.Visible = Settings.Default.KST_M5;
            if (!Settings.Default.KST_Macro_5.Equals(menu_btn_macro_5.Text))
                menu_btn_macro_5.Text = Settings.Default.KST_Macro_5;
            menu_btn_macro_6.Enabled = Settings.Default.KST_M6;
            menu_btn_macro_6.Visible = Settings.Default.KST_M6;
            if (!Settings.Default.KST_Macro_6.Equals(menu_btn_macro_6.Text))
                menu_btn_macro_6.Text = Settings.Default.KST_Macro_6;
            menu_btn_macro_7.Enabled = Settings.Default.KST_M7;
            menu_btn_macro_7.Visible = Settings.Default.KST_M7;
            if (!Settings.Default.KST_Macro_7.Equals(menu_btn_macro_7.Text))
                menu_btn_macro_7.Text = Settings.Default.KST_Macro_7;
            menu_btn_macro_8.Enabled = Settings.Default.KST_M8;
            menu_btn_macro_8.Visible = Settings.Default.KST_M8;
            if (!Settings.Default.KST_Macro_8.Equals(menu_btn_macro_8.Text))
                menu_btn_macro_8.Text = Settings.Default.KST_Macro_8;
            menu_btn_macro_9.Enabled = Settings.Default.KST_M9;
            menu_btn_macro_9.Visible = Settings.Default.KST_M9;
            if (!Settings.Default.KST_Macro_9.Equals(menu_btn_macro_9.Text))
                menu_btn_macro_9.Text = Settings.Default.KST_Macro_9;
            menu_btn_macro_0.Enabled = Settings.Default.KST_M0;
            menu_btn_macro_0.Visible = Settings.Default.KST_M0;
            if (!Settings.Default.KST_Macro_9.Equals(menu_btn_macro_0.Text))
                menu_btn_macro_0.Text = Settings.Default.KST_Macro_0;
        }
    }
}
