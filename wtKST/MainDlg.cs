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
			WaitUserName,
			WaitPassword,
			WaitChat,
			WaitAway,
			WaitConfig,
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

		public DataTable QSO = new DataTable("QSO");

		private bool lv_Calls_Updating = false;

		private bool DLLNotLoaded = false;

		private Point OldMousePos = new Point(0, 0);

		public static LogWriter Log = new LogWriter(Application.StartupPath);

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

		private ColumnHeader ch_UTC;

		private ColumnHeader ch_Loc;

		private ColumnHeader columnHeader9;

		private ColumnHeader columnHeader10;

		private ImageList il_Calls;

		private ColumnHeader columnHeader11;

		private ColumnHeader columnHeader12;

		private ColumnHeader columnHeader13;

		private ColumnHeader columnHeader14;

		private ColumnHeader columnHeader15;

		private ColumnHeader columnHeader16;

		private ColumnHeader columnHeader17;

		private ColumnHeader columnHeader18;

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

		private ColumnHeader ch_AS;

		public ImageList il_Planes;

		private ToolTip tt_Info;

		private BackgroundWorker bw_GetPlanes;

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
			this.cb_Command.MouseWheel += new MouseEventHandler(this.cb_Command_MouseWheel);
			this.MSG.Columns.Add("TIME", typeof(DateTime));
			this.MSG.Columns.Add("CALL");
			this.MSG.Columns.Add("NAME");
			this.MSG.Columns.Add("MSG");
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
			this.CALL.Columns.Add("ASLAT", typeof(double));
			this.CALL.Columns.Add("ASLON", typeof(double));
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
				TimeSpan ts = DateTime.Now - File.GetLastWriteTime(Settings.Default.WinTest_QRV_Table_FileName);
				if (!ForceReload && File.Exists(Settings.Default.WinTest_QRV_Table_FileName) && ts.Hours < 48)
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
							Row["TIME"] = DateTime.Parse(line.Split(new char[0])[1].TrimStart(new char[]
							{
								'['
							}).TrimEnd(new char[]
							{
								']'
							}) + " 00:00:00");
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
			MainDlg.Log.WriteMessage("Diconnected from: " + Settings.Default.KST_Chat);
			try
			{
				this.tw.Dispose();
			}
			catch
			{
			}
			this.KSTState = MainDlg.KST_STATE.Disconnected;
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
					string esc = Encoding.ASCII.GetString(bytes, 0, 1);
					t = t.Replace(esc, "{" + b.ToString("X2") + "}");
				}
			}
			catch
			{
			}
			Console.WriteLine(t);
			string s = e.Data.Replace("\0", "");
			this.KSTBuffer += s;
			if (this.KSTBuffer.EndsWith("\r\n"))
			{
				string[] buffer = this.KSTBuffer.Split(new char[]
				{
					'\r',
					'\n'
				});
				lock (this.MsgQueue)
				{
					string[] array = buffer;
					for (int i = 0; i < array.Length; i++)
					{
						string buf = array[i];
						this.MsgQueue.Enqueue(buf);
					}
				}
				this.KSTBuffer = "";
			}
		}

		private void OnIdle(object sender, EventArgs args)
		{
			if (this.DLLNotLoaded)
			{
				MessageBox.Show("Die Datei telnet.dll konnte im Programmverzeichnis nicht gefunden werden.", "Fehler");
				base.Close();
			}
			try
			{
				if (this.tw != null && !this.tw.Connected)
				{
					this.KSTState = MainDlg.KST_STATE.Disconnected;
				}
			}
			catch
			{
				this.KSTState = MainDlg.KST_STATE.Disconnected;
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
				if (Settings.Default.KST_AutoConnect && !this.ti_Reconnect.Enabled)
				{
					this.ti_Reconnect.Start();
				}
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
			if (this.UserState == MainDlg.USER_STATE.Here)
			{
				this.lbl_Call.Text = Settings.Default.KST_UserName.ToUpper();
			}
			else
			{
				this.lbl_Call.Text = "(" + Settings.Default.KST_UserName.ToUpper() + ")";
			}
			if (this.lv_Calls.Items.Count > 0)
			{
				this.lbl_KST_Calls.Text = "Calls [" + this.lv_Calls.Items.Count.ToString() + "] - " + Path.GetFileName(Settings.Default.WinTest_FileName);
			}
			else
			{
				this.lbl_KST_Calls.Text = "Calls";
			}
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
			if (this.KSTState >= MainDlg.KST_STATE.Connected)
			{
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
					Settings.Default.KST_Loc,
					"]"
				});
			}
			else
			{
				this.lbl_KST_Status.BackColor = Color.LightGray;
				this.lbl_KST_Status.Text = "Status: Disconnected ";
			}
			this.ni_Main.Text = "wtKST\nLeft click to activate";
			if (!this.cb_Command.Focused && !this.btn_KST_Send.Capture)
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
				}
				break;
			case MainDlg.KST_STATE.WaitPassword:
				if (s.IndexOf("Password:") >= 0)
				{
					Thread.Sleep(100);
					this.tw.Send(Settings.Default.KST_Password + "\r");
					this.KSTState = MainDlg.KST_STATE.WaitChat;
				}
				break;
			case MainDlg.KST_STATE.WaitChat:
				if (s.IndexOf(':') > 0)
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
					this.ti_Main.Interval = 5000;
					if (!this.ti_Main.Enabled)
					{
						this.ti_Main.Start();
					}
				}
				break;
			default:
				switch (kSTState)
				{
				case MainDlg.KST_STATE.Connected:
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
						string[] header = s.Substring(0, s.IndexOf("> ")).Split(new char[]
						{
							' '
						});
						DataRow Row = this.MSG.NewRow();
						string time = header[0].Trim();
						time = time.Substring(0, 2) + ":" + time.Substring(2, 2);
						DateTime dt = DateTime.Parse(time);
						Row["TIME"] = dt;
						Row["CALL"] = header[1].Trim();
						Row["NAME"] = header[2].Trim();
						Row["MSG"] = s.Remove(0, s.IndexOf("> ") + 2).Trim();
						this.MSG.Rows.Add(Row);
						DataRow findrow = this.QRV.Rows.Find(Row["CALL"].ToString().Trim());
						if (findrow != null)
						{
							findrow["TIME"] = Row["TIME"];
						}
						ListViewItem LV = new ListViewItem();
						LV.Text = ((DateTime)Row["TIME"]).ToString("HH:mm");
						LV.SubItems.Add(Row["CALL"].ToString());
						LV.SubItems.Add(Row["NAME"].ToString());
						LV.SubItems.Add(Row["MSG"].ToString());
						for (int i = 0; i < LV.SubItems.Count; i++)
						{
							LV.SubItems[i].Name = this.lv_Msg.Columns[i].Text;
						}
						if (this.lv_Msg.Items.Count == 0 || this.lv_Msg.TopItem == this.lv_Msg.Items[0])
						{
							this.lv_Msg.Items.Insert(0, LV);
							this.lv_Msg.TopItem = this.lv_Msg.Items[0];
						}
						else
						{
							ListViewItem topitem = this.lv_Msg.TopItem;
							this.lv_Msg.Items.Insert(0, LV);
							if (topitem != null)
							{
								this.lv_Msg.TopItem = topitem;
							}
							if (!this.ti_Top.Enabled)
							{
								this.ti_Top.Start();
							}
						}
						if (this.lv_Msg.Items.Count % 2 == 0)
						{
							this.lv_Msg.Items[0].BackColor = Color.FromArgb(16777200);
						}
						else
						{
							this.lv_Msg.Items[0].BackColor = Color.FromArgb(11516905);
						}
						if (Row["MSG"].ToString().Contains("(" + this.MyCall + ")"))
						{
							this.lv_Msg.Items[0].BackColor = Color.FromArgb(16745026);
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
									this.ni_Main.ShowBalloonTip(30000, "New MyMessage received", s, ToolTipIcon.Info);
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
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
						try
						{
							if (Settings.Default.WinTest_Activate)
							{
								this.Get_QSOs();
							}
							string topcall = "";
							if (this.lv_Calls.TopItem != null)
							{
								topcall = this.lv_Calls.TopItem.Text;
							}
							this.lv_Calls.Items.Clear();
							this.lv_Calls.BeginUpdate();
							for (int i = 0; i < this.CALL.Rows.Count; i++)
							{
								if (this.CALL.Rows[i]["CALL"].ToString().IndexOf(Settings.Default.KST_UserName.ToUpper()) < 0)
								{
									ListViewItem LV = new ListViewItem();
									LV.Text = this.CALL.Rows[i]["CALL"].ToString();
									LV.UseItemStyleForSubItems = false;
									LV.SubItems.Add(this.CALL.Rows[i]["NAME"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["LOC"].ToString());
									LV.SubItems.Add(((DateTime)this.CALL.Rows[i]["TIME"]).ToString("HH:mm"));
									int qrb = WCCheck.WCCheck.QRB(this.MyLoc, this.CALL.Rows[i]["LOC"].ToString());
									if (Settings.Default.AS_Active && qrb >= Convert.ToInt32(Settings.Default.AS_MinDist) && qrb <= Convert.ToInt32(Settings.Default.AS_MaxDist))
									{
										LV.SubItems.Add(this.GetNearestPlanePotential(this.CALL.Rows[i]["CALL"].ToString()).ToString());
									}
									else
									{
										LV.SubItems.Add("0");
									}
									LV.SubItems.Add(this.CALL.Rows[i]["144M"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["432M"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["1_2G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["2_3G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["3_4G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["5_7G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["10G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["24G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["47G"].ToString());
									LV.SubItems.Add(this.CALL.Rows[i]["76G"].ToString());
									for (int j = 0; j < this.lv_Calls.Columns.Count; j++)
									{
										LV.SubItems[j].Name = this.lv_Calls.Columns[j].Text.Replace(".", "_");
									}
									this.lv_Calls.Items.Add(LV);
								}
							}
							this.lv_Calls.EndUpdate();
							ListViewItem toplv = this.lv_Calls.FindItemWithText(topcall);
							if (toplv != null)
							{
								this.lv_Calls.TopItem = this.lv_Calls.Items[this.lv_Calls.Items.Count - 1];
								this.lv_Calls.TopItem = toplv;
							}
							this.lv_Calls_Updating = false;
							this.KSTState = MainDlg.KST_STATE.Connected;
							this.Say("");
							MainDlg.Log.WriteMessage("KST GetUsers finished: " + this.lv_Calls.Items.Count.ToString() + " Calls.");
						}
						catch (Exception e)
						{
							this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
						}
					}
					else
					{
						if (!this.lv_Calls_Updating)
						{
							this.CALL.Clear();
							if (Settings.Default.ShowBeacons)
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
												row["CALL"] = bs.Split(new char[]
												{
													' '
												})[0].Trim();
												row["NAME"] = "Beacon";
												row["LOC"] = bs.Split(new char[]
												{
													' '
												})[1].Trim();
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
									this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
								}
							}
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
							string qrvcall = call.TrimStart(new char[]
							{
								'('
							}).TrimEnd(new char[]
							{
								')'
							});
							if (qrvcall.IndexOf("-") > 0)
							{
								qrvcall = qrvcall.Remove(qrvcall.IndexOf("-"));
							}
							DataRow findrow = this.QRV.Rows.Find(qrvcall);
							if (findrow != null)
							{
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
									this.Error(MethodBase.GetCurrentMethod().Name, "(" + msg + "): " + e.Message);
								}
							}
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

		private void KST_Connect()
		{
			if (this.KSTState == MainDlg.KST_STATE.Disconnected)
			{
				this.tw = new TelnetWrapper();
				this.tw.DataAvailable += new DataAvailableEventHandler(this.tw_DataAvailable);
				this.tw.Disconnected += new DisconnectedEventHandler(this.tw_Disconnected);
				try
				{
					this.tw.Connect(Settings.Default.KST_ServerName, Convert.ToInt32(Settings.Default.KST_ServerPort));
					this.tw.Receive();
					this.KSTState = MainDlg.KST_STATE.WaitUserName;
					this.Say("Connecting to KST chat...");
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
				this.KSTState = MainDlg.KST_STATE.Disconnected;
			}
		}

		private void KST_Here()
		{
			if (this.KSTState >= MainDlg.KST_STATE.Connected)
			{
				this.tw.Send("/set here\r");
				this.UserState = MainDlg.USER_STATE.Here;
			}
		}

		private void KST_Away()
		{
			if (this.KSTState >= MainDlg.KST_STATE.Connected)
			{
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
						this.tw.Send(this.cb_Command.Text + "\r");
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

		private void tsi_Options_Click(object sender, EventArgs e)
		{
			OptionsDlg Dlg = new OptionsDlg();
			Dlg.cbb_KST_Chat.SelectedIndex = 2;
			if (this.KSTState != MainDlg.KST_STATE.Disconnected)
			{
				Dlg.tb_KST_Password.Enabled = false;
				Dlg.tb_KST_ServerName.Enabled = false;
				Dlg.tb_KST_ServerPort.Enabled = false;
				Dlg.tb_KST_UserName.Enabled = false;
				Dlg.cbb_KST_Chat.Enabled = false;
			}
			string oldchat = Settings.Default.KST_Chat;
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
			string kstcall = WCCheck.WCCheck.Cut(Settings.Default.KST_UserName.ToUpper());
			if (kstcall.IndexOf("DL0GTH") >= 0 || kstcall.IndexOf("DL2ALF") >= 0 || kstcall.IndexOf("DL2ARD") >= 0 || kstcall.IndexOf("DL2AKT") >= 0 || kstcall.IndexOf("DM5CT") >= 0 || kstcall.IndexOf("DL6AUI") >= 0 || kstcall.IndexOf("DR9A") >= 0 || kstcall.IndexOf("DA0FF") >= 0 || kstcall.IndexOf("DL0FTZ") >= 0)
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
						}
					}
					using (Stream stream = File.Open(Settings.Default.WinTest_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						this.QSO.Clear();
						byte[] bufh = new byte[13944];
						stream.Read(bufh, 0, bufh.Length);
						string arg_196_0 = Encoding.ASCII.GetString(bufh, 24, 6);
						char[] separator = new char[1];
						this.MyLoc = arg_196_0.Split(separator)[0];
						stream.Position = 13944L;
						while (stream.Position < stream.Length)
						{
							byte[] buf = new byte[544];
							stream.Read(buf, 0, buf.Length);
							int utime = BitConverter.ToInt32(buf, 24);
							DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
							ts = ts.AddSeconds((double)utime);
							string call = Encoding.ASCII.GetString(buf, 32, 14);
							call = call.ToString().Replace("\0", "");
							DataRow row = this.QSO.NewRow();
							row["CALL"] = call;
							switch (buf[4])
							{
							case 12:
								row["BAND"] = "144M";
								break;
							case 13:
							case 15:
								goto IL_35A;
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
							default:
								goto IL_35A;
							}
							IL_36E:
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
								findcall = findcall.TrimStart(new char[]
								{
									'('
								}).TrimEnd(new char[]
								{
									')'
								});
								if (findcall.IndexOf("-") > 0)
								{
									findcall = findcall.Remove(findcall.IndexOf("-"));
								}
								if (findcall == call)
								{
									switch (buf[4])
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
								}
							}
							continue;
							IL_35A:
							row["BAND"] = "";
							goto IL_36E;
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
			call = call.TrimStart(new char[]
			{
				'('
			}).TrimEnd(new char[]
			{
				')'
			});
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
			call = call.TrimStart(new char[]
			{
				'('
			}).TrimEnd(new char[]
			{
				')'
			});
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
			if (Settings.Default.AS_Active)
			{
			}
			MainDlg.Log.WriteMessage("KST GetUsers start.");
			this.KST_GetUsers();
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

		private void MainDlg_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				this.bw_GetPlanes.CancelAsync();
				string FileName = Settings.Default.WinTest_QRV_Table_FileName;
				this.Say("Saving QRV-Database to " + FileName + "...");
				this.QRV.WriteXml(FileName, XmlWriteMode.IgnoreSchema);
			}
			catch (Exception e2)
			{
				this.Error(MethodBase.GetCurrentMethod().Name, "(qrv.xml): " + e2.Message);
			}
			MainDlg.Log.WriteMessage("Closed down.");
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
					if (info.SubItem.Name == "AS")
					{
						string call = WCCheck.WCCheck.Cut(info.Item.Text);
						string loc = info.Item.SubItems[2].Text;
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
					if (info.SubItem.Name == "AS")
					{
						string s = this.GetNearestPlanes(info.Item.Text);
						if (string.IsNullOrEmpty(s))
						{
							ToolTipText = "No planes\n\nLeft click for map";
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
			lock (this.MsgQueue)
			{
				while (this.MsgQueue.Count > 0)
				{
					string s = this.MsgQueue.Dequeue();
					if (s.Length > 0)
					{
						this.KST_Receive(s);
					}
				}
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
					LastCommand = LastCommand.Split(new char[]
					{
						' '
					})[0] + " " + LastCommand.Split(new char[]
					{
						' '
					})[1];
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
			if (Settings.Default.KST_AutoConnect && this.KSTState == MainDlg.KST_STATE.Disconnected)
			{
				this.KST_Connect();
			}
		}

		private void bw_GetPlanes_DoWork(object sender, DoWorkEventArgs e)
		{
			while (!this.bw_GetPlanes.CancellationPending)
			{
				int errors = 0;
				for (int i = 0; i < this.CALL.Rows.Count; i++)
				{
					try
					{
						int qrb = WCCheck.WCCheck.QRB(this.MyLoc, this.CALL.Rows[i]["LOC"].ToString());
						string mycall = WCCheck.WCCheck.Cut(this.MyCall);
						string myloc = this.MyLoc;
						string dxcall = WCCheck.WCCheck.Cut(this.CALL.Rows[i]["CALL"].ToString().TrimStart(new char[]
						{
							'('
						}).TrimEnd(new char[]
						{
							')'
						}));
						string dxloc = this.CALL.Rows[i]["LOC"].ToString();
						string rxmycall = "";
						string rxdxcall = "";
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
												string[] a = Msg.Data.Split(new char[]
												{
													','
												});
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
						string[] a = Msg.Data.Split(new char[]
						{
							','
						});
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
						string[] a = Msg.Data.Split(new char[]
						{
							','
						});
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
			this.components = new Container();
			ComponentResourceManager resources = new ComponentResourceManager(typeof(MainDlg));
			this.ss_Main = new StatusStrip();
			this.tsl_Info = new ToolStripStatusLabel();
			this.tsl_Error = new ToolStripStatusLabel();
			this.splitContainer1 = new SplitContainer();
			this.splitContainer2 = new SplitContainer();
			this.splitContainer3 = new SplitContainer();
			this.cb_Command = new ComboBox();
			this.btn_KST_Send = new Button();
			this.lbl_KST_Status = new Label();
			this.lbl_Call = new Label();
			this.lv_Msg = new ListView();
			this.lvh_Time = new ColumnHeader();
			this.lvh_Call = new ColumnHeader();
			this.lvh_Name = new ColumnHeader();
			this.lvh_Msg = new ColumnHeader();
			this.lbl_KST_Msg = new Label();
			this.lv_MyMsg = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.columnHeader2 = new ColumnHeader();
			this.columnHeader3 = new ColumnHeader();
			this.columnHeader4 = new ColumnHeader();
			this.lbl_KST_MyMsg = new Label();
			this.lv_Calls = new ListView();
			this.ch_Call = new ColumnHeader();
			this.ch_Name = new ColumnHeader();
			this.ch_Loc = new ColumnHeader();
			this.ch_UTC = new ColumnHeader();
			this.ch_AS = new ColumnHeader();
			this.columnHeader9 = new ColumnHeader();
			this.columnHeader10 = new ColumnHeader();
			this.columnHeader11 = new ColumnHeader();
			this.columnHeader12 = new ColumnHeader();
			this.columnHeader13 = new ColumnHeader();
			this.columnHeader14 = new ColumnHeader();
			this.columnHeader15 = new ColumnHeader();
			this.columnHeader16 = new ColumnHeader();
			this.columnHeader17 = new ColumnHeader();
			this.columnHeader18 = new ColumnHeader();
			this.il_Calls = new ImageList(this.components);
			this.lbl_KST_Calls = new Label();
			this.mn_Main = new MenuStrip();
			this.tsm_File = new ToolStripMenuItem();
			this.toolStripSeparator1 = new ToolStripSeparator();
			this.tsi_File_Exit = new ToolStripMenuItem();
			this.tsm_KST = new ToolStripMenuItem();
			this.tsi_KST_Connect = new ToolStripMenuItem();
			this.tsi_KST_Disconnect = new ToolStripMenuItem();
			this.toolStripSeparator2 = new ToolStripSeparator();
			this.tsi_KST_Here = new ToolStripMenuItem();
			this.tsi_KST_Away = new ToolStripMenuItem();
			this.tsi_Options = new ToolStripMenuItem();
			this.toolStripMenuItem1 = new ToolStripMenuItem();
			this.aboutToolStripMenuItem = new ToolStripMenuItem();
			this.ti_Main = new System.Windows.Forms.Timer(this.components);
			this.ni_Main = new NotifyIcon(this.components);
			this.cmn_Notify = new ContextMenuStrip(this.components);
			this.cmi_Notify_Restore = new ToolStripMenuItem();
			this.toolStripSeparator3 = new ToolStripSeparator();
			this.cmi_Notify_Quit = new ToolStripMenuItem();
			this.ti_Receive = new System.Windows.Forms.Timer(this.components);
			this.ti_Error = new System.Windows.Forms.Timer(this.components);
			this.ti_Top = new System.Windows.Forms.Timer(this.components);
			this.ti_Reconnect = new System.Windows.Forms.Timer(this.components);
			this.il_Planes = new ImageList(this.components);
			this.tt_Info = new ToolTip(this.components);
			this.bw_GetPlanes = new BackgroundWorker();
			this.ss_Main.SuspendLayout();
			((ISupportInitialize)this.splitContainer1).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((ISupportInitialize)this.splitContainer2).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((ISupportInitialize)this.splitContainer3).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.mn_Main.SuspendLayout();
			this.cmn_Notify.SuspendLayout();
			base.SuspendLayout();
			this.ss_Main.Items.AddRange(new ToolStripItem[]
			{
				this.tsl_Info,
				this.tsl_Error
			});
			this.ss_Main.Location = new Point(0, 708);
			this.ss_Main.Name = "ss_Main";
			this.ss_Main.Size = new Size(1008, 22);
			this.ss_Main.TabIndex = 9;
			this.ss_Main.Text = "statusStrip1";
			this.tsl_Info.Name = "tsl_Info";
			this.tsl_Info.Size = new Size(28, 17);
			this.tsl_Info.Text = "Info";
			this.tsl_Error.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.tsl_Error.ForeColor = Color.Red;
			this.tsl_Error.Name = "tsl_Error";
			this.tsl_Error.RightToLeft = RightToLeft.No;
			this.tsl_Error.Size = new Size(965, 17);
			this.tsl_Error.Spring = true;
			this.tsl_Error.TextAlign = ContentAlignment.MiddleRight;
			this.splitContainer1.BorderStyle = BorderStyle.FixedSingle;
			this.splitContainer1.Dock = DockStyle.Fill;
			this.splitContainer1.Location = new Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel2.Controls.Add(this.lv_Calls);
			this.splitContainer1.Panel2.Controls.Add(this.lbl_KST_Calls);
			this.splitContainer1.Size = new Size(1008, 684);
			this.splitContainer1.SplitterDistance = 707;
			this.splitContainer1.TabIndex = 10;
			this.splitContainer2.BorderStyle = BorderStyle.FixedSingle;
			this.splitContainer2.Dock = DockStyle.Fill;
			this.splitContainer2.Location = new Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = Orientation.Horizontal;
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
			this.splitContainer2.Panel1MinSize = 75;
			this.splitContainer2.Panel2.Controls.Add(this.lv_MyMsg);
			this.splitContainer2.Panel2.Controls.Add(this.lbl_KST_MyMsg);
			this.splitContainer2.Panel2MinSize = 75;
			this.splitContainer2.Size = new Size(707, 684);
			this.splitContainer2.SplitterDistance = 346;
			this.splitContainer2.TabIndex = 0;
			this.splitContainer3.BorderStyle = BorderStyle.FixedSingle;
			this.splitContainer3.Dock = DockStyle.Fill;
			this.splitContainer3.Location = new Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = Orientation.Horizontal;
			this.splitContainer3.Panel1.Controls.Add(this.cb_Command);
			this.splitContainer3.Panel1.Controls.Add(this.btn_KST_Send);
			this.splitContainer3.Panel1.Controls.Add(this.lbl_KST_Status);
			this.splitContainer3.Panel1.Controls.Add(this.lbl_Call);
			this.splitContainer3.Panel1MinSize = 80;
			this.splitContainer3.Panel2.Controls.Add(this.lv_Msg);
			this.splitContainer3.Panel2.Controls.Add(this.lbl_KST_Msg);
			this.splitContainer3.Size = new Size(707, 346);
			this.splitContainer3.SplitterDistance = 80;
			this.splitContainer3.TabIndex = 0;
			this.cb_Command.Font = new Font("Tahoma", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.cb_Command.FormattingEnabled = true;
			this.cb_Command.Location = new Point(11, 41);
			this.cb_Command.MaxDropDownItems = 5;
			this.cb_Command.Name = "cb_Command";
			this.cb_Command.Size = new Size(469, 24);
			this.cb_Command.TabIndex = 17;
			this.cb_Command.TextUpdate += new EventHandler(this.cb_Command_TextUpdate);
			this.cb_Command.DropDownClosed += new EventHandler(this.cb_Command_DropDownClosed);
			this.cb_Command.TextChanged += new EventHandler(this.cb_Command_TextChanged);
			this.btn_KST_Send.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.btn_KST_Send.Location = new Point(508, 41);
			this.btn_KST_Send.Name = "btn_KST_Send";
			this.btn_KST_Send.Size = new Size(75, 23);
			this.btn_KST_Send.TabIndex = 16;
			this.btn_KST_Send.Text = "Send";
			this.btn_KST_Send.UseVisualStyleBackColor = true;
			this.btn_KST_Send.Click += new EventHandler(this.btn_KST_Send_Click_1);
			this.lbl_KST_Status.BackColor = SystemColors.ScrollBar;
			this.lbl_KST_Status.Dock = DockStyle.Top;
			this.lbl_KST_Status.FlatStyle = FlatStyle.Popup;
			this.lbl_KST_Status.Font = new Font("Tahoma", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.lbl_KST_Status.Location = new Point(0, 0);
			this.lbl_KST_Status.Name = "lbl_KST_Status";
			this.lbl_KST_Status.Padding = new Padding(4);
			this.lbl_KST_Status.Size = new Size(705, 26);
			this.lbl_KST_Status.TabIndex = 15;
			this.lbl_KST_Status.Text = "Status";
			this.lbl_KST_Status.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_Call.Font = new Font("Tahoma", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.lbl_Call.Location = new Point(589, 41);
			this.lbl_Call.Name = "lbl_Call";
			this.lbl_Call.Size = new Size(100, 23);
			this.lbl_Call.TabIndex = 14;
			this.lbl_Call.Text = "Call";
			this.lbl_Call.TextAlign = ContentAlignment.MiddleCenter;
			this.lv_Msg.Columns.AddRange(new ColumnHeader[]
			{
				this.lvh_Time,
				this.lvh_Call,
				this.lvh_Name,
				this.lvh_Msg
			});
			this.lv_Msg.Dock = DockStyle.Fill;
			this.lv_Msg.Font = new Font("Tahoma", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lv_Msg.FullRowSelect = true;
			this.lv_Msg.GridLines = true;
			this.lv_Msg.Location = new Point(0, 26);
			this.lv_Msg.MultiSelect = false;
			this.lv_Msg.Name = "lv_Msg";
			this.lv_Msg.Size = new Size(705, 234);
			this.lv_Msg.TabIndex = 12;
			this.lv_Msg.UseCompatibleStateImageBehavior = false;
			this.lv_Msg.View = View.Details;
			this.lv_Msg.MouseDown += new MouseEventHandler(this.lv_Msg_MouseDown);
			this.lv_Msg.MouseMove += new MouseEventHandler(this.lv_Msg_MouseMove);
			this.lv_Msg.Resize += new EventHandler(this.lv_Msg_Resize);
			this.lvh_Time.Text = "Time";
			this.lvh_Time.Width = 50;
			this.lvh_Call.Text = "Call";
			this.lvh_Call.Width = 100;
			this.lvh_Name.Text = "Name";
			this.lvh_Name.Width = 150;
			this.lvh_Msg.Text = "Messages";
			this.lvh_Msg.Width = 600;
			this.lbl_KST_Msg.BackColor = SystemColors.ScrollBar;
			this.lbl_KST_Msg.Dock = DockStyle.Top;
			this.lbl_KST_Msg.FlatStyle = FlatStyle.Popup;
			this.lbl_KST_Msg.Font = new Font("Tahoma", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.lbl_KST_Msg.Location = new Point(0, 0);
			this.lbl_KST_Msg.Name = "lbl_KST_Msg";
			this.lbl_KST_Msg.Padding = new Padding(4);
			this.lbl_KST_Msg.Size = new Size(705, 26);
			this.lbl_KST_Msg.TabIndex = 11;
			this.lbl_KST_Msg.Text = "Messages";
			this.lbl_KST_Msg.TextAlign = ContentAlignment.MiddleLeft;
			this.lv_MyMsg.Columns.AddRange(new ColumnHeader[]
			{
				this.columnHeader1,
				this.columnHeader2,
				this.columnHeader3,
				this.columnHeader4
			});
			this.lv_MyMsg.Dock = DockStyle.Fill;
			this.lv_MyMsg.Font = new Font("Tahoma", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lv_MyMsg.FullRowSelect = true;
			this.lv_MyMsg.GridLines = true;
			this.lv_MyMsg.Location = new Point(0, 26);
			this.lv_MyMsg.MultiSelect = false;
			this.lv_MyMsg.Name = "lv_MyMsg";
			this.lv_MyMsg.Size = new Size(705, 306);
			this.lv_MyMsg.TabIndex = 13;
			this.lv_MyMsg.UseCompatibleStateImageBehavior = false;
			this.lv_MyMsg.View = View.Details;
			this.lv_MyMsg.MouseDown += new MouseEventHandler(this.lv_MyMsg_MouseDown);
			this.lv_MyMsg.MouseMove += new MouseEventHandler(this.lv_MyMsg_MouseMove);
			this.lv_MyMsg.Resize += new EventHandler(this.lv_MyMsg_Resize);
			this.columnHeader1.Text = "Time";
			this.columnHeader1.Width = 50;
			this.columnHeader2.Text = "Call";
			this.columnHeader2.Width = 100;
			this.columnHeader3.Text = "Name";
			this.columnHeader3.Width = 150;
			this.columnHeader4.Text = "Messages";
			this.columnHeader4.Width = 600;
			this.lbl_KST_MyMsg.BackColor = Color.BlanchedAlmond;
			this.lbl_KST_MyMsg.Dock = DockStyle.Top;
			this.lbl_KST_MyMsg.FlatStyle = FlatStyle.Popup;
			this.lbl_KST_MyMsg.Font = new Font("Tahoma", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.lbl_KST_MyMsg.Location = new Point(0, 0);
			this.lbl_KST_MyMsg.Name = "lbl_KST_MyMsg";
			this.lbl_KST_MyMsg.Padding = new Padding(4);
			this.lbl_KST_MyMsg.Size = new Size(705, 26);
			this.lbl_KST_MyMsg.TabIndex = 10;
			this.lbl_KST_MyMsg.Text = "My Messages";
			this.lbl_KST_MyMsg.TextAlign = ContentAlignment.MiddleLeft;
			this.lv_Calls.Columns.AddRange(new ColumnHeader[]
			{
				this.ch_Call,
				this.ch_Name,
				this.ch_Loc,
				this.ch_UTC,
				this.ch_AS,
				this.columnHeader9,
				this.columnHeader10,
				this.columnHeader11,
				this.columnHeader12,
				this.columnHeader13,
				this.columnHeader14,
				this.columnHeader15,
				this.columnHeader16,
				this.columnHeader17,
				this.columnHeader18
			});
			this.lv_Calls.Dock = DockStyle.Fill;
			this.lv_Calls.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.lv_Calls.GridLines = true;
			this.lv_Calls.Location = new Point(0, 24);
			this.lv_Calls.MultiSelect = false;
			this.lv_Calls.Name = "lv_Calls";
			this.lv_Calls.OwnerDraw = true;
			this.lv_Calls.Size = new Size(295, 658);
			this.lv_Calls.SmallImageList = this.il_Calls;
			this.lv_Calls.TabIndex = 14;
			this.lv_Calls.UseCompatibleStateImageBehavior = false;
			this.lv_Calls.View = View.Details;
			this.lv_Calls.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(this.lv_Calls_DrawColumnHeader);
			this.lv_Calls.DrawItem += new DrawListViewItemEventHandler(this.lv_Calls_DrawItem);
			this.lv_Calls.DrawSubItem += new DrawListViewSubItemEventHandler(this.lv_Calls_DrawSubItem);
			this.lv_Calls.MouseDown += new MouseEventHandler(this.lv_Calls_MouseDown);
			this.lv_Calls.MouseMove += new MouseEventHandler(this.lv_Calls_MouseMove);
			this.ch_Call.Text = "Call";
			this.ch_Call.Width = 80;
			this.ch_Name.Text = "Name";
			this.ch_Name.Width = 100;
			this.ch_Loc.Text = "Locator";
			this.ch_UTC.Text = "UTC";
			this.ch_UTC.Width = 40;
			this.ch_AS.Text = "AS";
			this.ch_AS.Width = 20;
			this.columnHeader9.Text = "144M";
			this.columnHeader9.Width = 20;
			this.columnHeader10.Text = "432M";
			this.columnHeader10.Width = 20;
			this.columnHeader11.Text = "1.2G";
			this.columnHeader11.Width = 20;
			this.columnHeader12.Text = "2.3G";
			this.columnHeader12.Width = 20;
			this.columnHeader13.Text = "3.4G";
			this.columnHeader13.Width = 20;
			this.columnHeader14.Text = "5.7G";
			this.columnHeader14.Width = 20;
			this.columnHeader15.Text = "10G";
			this.columnHeader15.Width = 20;
			this.columnHeader16.Text = "24G";
			this.columnHeader16.Width = 20;
			this.columnHeader17.Text = "47G";
			this.columnHeader17.Width = 20;
			this.columnHeader18.Text = "76G";
			this.columnHeader18.Width = 20;
			this.il_Calls.ImageStream = (ImageListStreamer)resources.GetObject("il_Calls.ImageStream");
			this.il_Calls.TransparentColor = Color.Transparent;
			this.il_Calls.Images.SetKeyName(0, "JEWEL_GRAY.PNG");
			this.il_Calls.Images.SetKeyName(1, "JEWEL_GREEN.PNG");
			this.il_Calls.Images.SetKeyName(2, "JEWEL_RED.PNG");
			this.il_Calls.Images.SetKeyName(3, "JEWEL_YELLWO.PNG");
			this.lbl_KST_Calls.BackColor = SystemColors.ScrollBar;
			this.lbl_KST_Calls.Dock = DockStyle.Top;
			this.lbl_KST_Calls.FlatStyle = FlatStyle.Popup;
			this.lbl_KST_Calls.Font = new Font("Tahoma", 9.75f, FontStyle.Italic, GraphicsUnit.Point, 0);
			this.lbl_KST_Calls.Location = new Point(0, 0);
			this.lbl_KST_Calls.Name = "lbl_KST_Calls";
			this.lbl_KST_Calls.Padding = new Padding(4);
			this.lbl_KST_Calls.Size = new Size(295, 24);
			this.lbl_KST_Calls.TabIndex = 0;
			this.lbl_KST_Calls.Text = "Calls";
			this.lbl_KST_Calls.TextAlign = ContentAlignment.MiddleLeft;
			this.lbl_KST_Calls.UseMnemonic = false;
			this.mn_Main.Items.AddRange(new ToolStripItem[]
			{
				this.tsm_File,
				this.tsm_KST,
				this.tsi_Options,
				this.toolStripMenuItem1
			});
			this.mn_Main.Location = new Point(0, 0);
			this.mn_Main.Name = "mn_Main";
			this.mn_Main.Size = new Size(1008, 24);
			this.mn_Main.TabIndex = 11;
			this.mn_Main.Text = "menuStrip1";
			this.tsm_File.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.toolStripSeparator1,
				this.tsi_File_Exit
			});
			this.tsm_File.Name = "tsm_File";
			this.tsm_File.Size = new Size(37, 20);
			this.tsm_File.Text = "&File";
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new Size(89, 6);
			this.tsi_File_Exit.Name = "tsi_File_Exit";
			this.tsi_File_Exit.Size = new Size(92, 22);
			this.tsi_File_Exit.Text = "E&xit";
			this.tsi_File_Exit.Click += new EventHandler(this.tsi_File_Exit_Click);
			this.tsm_KST.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.tsi_KST_Connect,
				this.tsi_KST_Disconnect,
				this.toolStripSeparator2,
				this.tsi_KST_Here,
				this.tsi_KST_Away
			});
			this.tsm_KST.Name = "tsm_KST";
			this.tsm_KST.Size = new Size(39, 20);
			this.tsm_KST.Text = "&KST";
			this.tsi_KST_Connect.Name = "tsi_KST_Connect";
			this.tsi_KST_Connect.Size = new Size(160, 22);
			this.tsi_KST_Connect.Text = "&Connect";
			this.tsi_KST_Connect.Click += new EventHandler(this.tsi_KST_Connect_Click);
			this.tsi_KST_Disconnect.Enabled = false;
			this.tsi_KST_Disconnect.Name = "tsi_KST_Disconnect";
			this.tsi_KST_Disconnect.Size = new Size(160, 22);
			this.tsi_KST_Disconnect.Text = "&Disconnect";
			this.tsi_KST_Disconnect.Click += new EventHandler(this.tsi_KST_Disconnect_Click);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new Size(157, 6);
			this.tsi_KST_Here.Name = "tsi_KST_Here";
			this.tsi_KST_Here.Size = new Size(160, 22);
			this.tsi_KST_Here.Text = "&I am on the chat";
			this.tsi_KST_Here.Click += new EventHandler(this.tsi_KST_Here_Click);
			this.tsi_KST_Away.Name = "tsi_KST_Away";
			this.tsi_KST_Away.Size = new Size(160, 22);
			this.tsi_KST_Away.Text = "I am &away";
			this.tsi_KST_Away.Click += new EventHandler(this.tsi_KST_Away_Click);
			this.tsi_Options.Name = "tsi_Options";
			this.tsi_Options.Size = new Size(61, 20);
			this.tsi_Options.Text = "&Options";
			this.tsi_Options.Click += new EventHandler(this.tsi_Options_Click);
			this.toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[]
			{
				this.aboutToolStripMenuItem
			});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new Size(24, 20);
			this.toolStripMenuItem1.Text = "&?";
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new Size(107, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			this.ti_Main.Enabled = true;
			this.ti_Main.Interval = 30000;
			this.ti_Main.Tick += new EventHandler(this.ti_Main_Tick);
			this.ni_Main.ContextMenuStrip = this.cmn_Notify;
			this.ni_Main.Icon = (Icon)resources.GetObject("ni_Main.Icon");
			this.ni_Main.Text = "wtKST";
			this.ni_Main.Visible = true;
			this.ni_Main.BalloonTipClicked += new EventHandler(this.ni_Main_BalloonTipClicked);
			this.ni_Main.MouseClick += new MouseEventHandler(this.ni_Main_MouseClick);
			this.cmn_Notify.Items.AddRange(new ToolStripItem[]
			{
				this.cmi_Notify_Restore,
				this.toolStripSeparator3,
				this.cmi_Notify_Quit
			});
			this.cmn_Notify.Name = "cmn_Notify";
			this.cmn_Notify.Size = new Size(119, 54);
			this.cmi_Notify_Restore.Font = new Font("Segoe UI", 9f, FontStyle.Bold, GraphicsUnit.Point, 0);
			this.cmi_Notify_Restore.Name = "cmi_Notify_Restore";
			this.cmi_Notify_Restore.Size = new Size(118, 22);
			this.cmi_Notify_Restore.Text = "&Restore";
			this.cmi_Notify_Restore.Click += new EventHandler(this.cmi_Notify_Restore_Click);
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new Size(115, 6);
			this.cmi_Notify_Quit.Name = "cmi_Notify_Quit";
			this.cmi_Notify_Quit.Size = new Size(118, 22);
			this.cmi_Notify_Quit.Text = "&Quit";
			this.cmi_Notify_Quit.Click += new EventHandler(this.cmi_Notify_Quit_Click);
			this.ti_Receive.Enabled = true;
			this.ti_Receive.Interval = 1000;
			this.ti_Receive.Tick += new EventHandler(this.ti_Receive_Tick);
			this.ti_Error.Interval = 10000;
			this.ti_Error.Tick += new EventHandler(this.ti_Error_Tick);
			this.ti_Top.Interval = 60000;
			this.ti_Top.Tick += new EventHandler(this.ti_Top_Tick);
			this.ti_Reconnect.Enabled = true;
			this.ti_Reconnect.Interval = 30000;
			this.ti_Reconnect.Tick += new EventHandler(this.ti_Reconnect_Tick);
			this.il_Planes.ImageStream = (ImageListStreamer)resources.GetObject("il_Planes.ImageStream");
			this.il_Planes.TransparentColor = Color.Transparent;
			this.il_Planes.Images.SetKeyName(0, "Green");
			this.il_Planes.Images.SetKeyName(1, "Orange");
			this.il_Planes.Images.SetKeyName(2, "Blue");
			this.tt_Info.ShowAlways = true;
			this.bw_GetPlanes.WorkerReportsProgress = true;
			this.bw_GetPlanes.WorkerSupportsCancellation = true;
			this.bw_GetPlanes.DoWork += new DoWorkEventHandler(this.bw_GetPlanes_DoWork);
			this.bw_GetPlanes.ProgressChanged += new ProgressChangedEventHandler(this.bw_GetPlanes_ProgressChanged);
			this.bw_GetPlanes.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.bw_GetPlanes_RunWorkerCompleted);
			base.AcceptButton = this.btn_KST_Send;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			this.AutoValidate = AutoValidate.EnableAllowFocusChange;
			base.ClientSize = new Size(1008, 730);
			base.Controls.Add(this.splitContainer1);
			base.Controls.Add(this.ss_Main);
			base.Controls.Add(this.mn_Main);
			base.Icon = (Icon)resources.GetObject("$this.Icon");
			base.Name = "MainDlg";
			this.Text = "wtKST";
			base.FormClosing += new FormClosingEventHandler(this.MainDlg_FormClosing);
			this.ss_Main.ResumeLayout(false);
			this.ss_Main.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer1).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer2).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((ISupportInitialize)this.splitContainer3).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.mn_Main.ResumeLayout(false);
			this.mn_Main.PerformLayout();
			this.cmn_Notify.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
