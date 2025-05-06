using System;
using System.ComponentModel;
using System.Windows.Forms;
using wtKST.Properties;

namespace wtKST
{
    public class OptionsDlg : Form
    {
        private IContainer components = null;

        private Button btn_OK;

        private Button btn_Cancel;

        private Button btn_Options_WinTest_INI_Select;

        public TextBox tb_Options_WinTest_INI;

        private Label label6;

        private GroupBox groupBox3;

        private GroupBox groupBox5;

        public CheckBox cb_KST_ShowBalloon;

        public CheckBox cb_ShowBeacons;
        private Label label3;
        public TextBox tb_KST_UserName;
        private Label label4;
        public TextBox tb_KST_Password;
        public ComboBox cbb_KST_Chat;
        private Label label5;
        public CheckBox cb_KST_AutoConnect;
        public RadioButton rb_KST_StartAsHere;
        public RadioButton rb_KST_StartAsAway;
        public TabControl tabControl1;
        private TabPage tabPage1;
        public TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private Label label16;
        private ComboBox cb_AS_QRG;
        private TextBox tb_Options_AS_My_Name;
        private Label label15;
        private TextBox tb_Options_AS_Server_Name;
        private Label label14;
        private TextBox tb_AS_Timeout;
        private Label label_timeout;
        private Label label11;
        private TextBox tb_AS_MaxDist;
        private Label label9;
        private TextBox tb_AS_MinDist;
        public CheckBox cb_AS_Active;
        private GroupBox Bands;
        private CheckBox checkBox50;
        private CheckBox checkBox70;
        private CheckBox checkBox144;
        private CheckBox checkBox432;
        private CheckBox checkBox1296;
        private CheckBox checkBox10GHz;
        private CheckBox checkBox24GHz;
        private CheckBox checkBox47GHz;
        private CheckBox checkBox2320;
        private CheckBox checkBox3400;
        private CheckBox checkBox5760;
        private CheckBox checkBox76GHz;
        public TextBox tb_KST_MaxDist;
        private Label label17;
        public CheckBox checkBox_KST_Show_Own_Messages;
        public TextBox tb_KST_Name;
        private Label label2;
        public TextBox tb_KST_Locator;
        private Label label1;
        private Label label_local_server;
        private TextBox tb_Options_AS_Local_Server_Name;
        public CheckBox cb_AS_local;
        private TabPage tabPage5;
        private TextBox tB_macro_2;
        private Label label23;
        private CheckBox cB_macro_2;
        private TextBox tB_macro_5;
        private Label label22;
        private CheckBox cB_macro_5;
        private TextBox tB_macro_6;
        private Label label21;
        private CheckBox cB_macro_6;
        private TextBox tB_macro_3;
        private Label label20;
        private CheckBox cB_macro_3;
        private TextBox tB_macro_4;
        private Label label19;
        private CheckBox cB_macro_4;
        private TextBox tB_macro_1;
        private Label label18;
        private CheckBox cB_macro_1;
        private TextBox textBox2;
        private Label label24;
        private CheckBox checkBox1;
        private TextBox textBox3;
        private Label label25;
        private CheckBox checkBox2;
        private TextBox textBox4;
        private Label label26;
        private CheckBox checkBox3;
        private TextBox textBox5;
        private Label label27;
        private CheckBox checkBox4;
        private GroupBox groupBox2;
        public CheckBox cb_WinTestNet_Active;
        public TextBox tb_Options_WinTest_Station_Name;
        private Label label10;
        private GroupBox groupBox1;
        private GroupBox groupBox4;
        public CheckBox cb_QARTest_Active;
        private GroupBox groupBox6;
        public CheckBox cb_DXLog_Active;
        public TextBox tb_Options_DXLog_Station_Name;
        private Label label28;
        public CheckBox cb_WS_Active;
        private Label label_WS_Password;
        private TextBox tb_WS_Password;
        private Label label_WS_Username;
        private TextBox tb_WS_Username;
        private Label label8;
        private Panel panel_WS;
        private Panel panel2;
        private Panel panel_AS;
        private Label label_timeout2;
        private Label label29;
        private TextBox tb_WS_LoginURL;
        private Label label31;
        private TextBox tb_WS_API_URL;
        public CheckBox cb_WinTest_Active;

        public OptionsDlg()
        {
            InitializeComponent();
            if (this.cb_WinTest_Active.Checked)
            {
                this.cb_WinTest_Active.Enabled = true;
                this.cb_WinTestNet_Active.Checked = false;
                this.cb_WinTestNet_Active.Enabled = false;
                this.cb_QARTest_Active.Checked = false;
                this.cb_QARTest_Active.Enabled = false;
                this.cb_DXLog_Active.Checked = false;
                this.cb_DXLog_Active.Enabled = false;
            }
            else if (this.cb_WinTestNet_Active.Checked)
            {
                this.cb_WinTestNet_Active.Enabled = true;
                this.cb_WinTest_Active.Checked = false;
                this.cb_WinTest_Active.Enabled = false;
                this.cb_QARTest_Active.Checked = false;
                this.cb_QARTest_Active.Enabled = false;
                this.cb_DXLog_Active.Checked = false;
                this.cb_DXLog_Active.Enabled = false;
            }
            else if (this.cb_QARTest_Active.Checked)
            {
                this.cb_QARTest_Active.Enabled = true;
                this.cb_WinTest_Active.Checked = false;
                this.cb_WinTest_Active.Enabled = false;
                this.cb_WinTestNet_Active.Checked = false;
                this.cb_WinTestNet_Active.Enabled = false;
                this.cb_DXLog_Active.Checked = false;
                this.cb_DXLog_Active.Enabled = false;
            }
            else if (this.cb_DXLog_Active.Checked)
            {
                this.cb_DXLog_Active.Enabled = true;
                this.cb_QARTest_Active.Enabled = false;
                this.cb_WinTest_Active.Checked = false;
                this.cb_WinTest_Active.Enabled = false;
                this.cb_WinTestNet_Active.Checked = false;
                this.cb_WinTestNet_Active.Enabled = false;
            }
            else
            {
                // none checked
                this.cb_WinTest_Active.Enabled = true;
                this.cb_WinTestNet_Active.Enabled = true;
                this.cb_QARTest_Active.Enabled = true;
                this.cb_DXLog_Active.Enabled = true;
            }

            // AS handling
            mCBsASCheckedChangedTriggered = false;
            mCBsWSCheckedChangedTriggered = false;
            if (this.cb_AS_Active.Checked)
            {
                CBHandleASActive();
            }
            else
            {
                CBHandleWSActive();
            }
        }

        private void btn_Options_WinTest_INI_Select_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Win-Test\\cfg";
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Filter = "Win-Test Einstellung | *.ini";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.tb_Options_WinTest_INI.Text = ofd.FileName;
            }
        }

        private void tb_Options_UpdateInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDlg));
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Options_WinTest_INI_Select = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tb_KST_MaxDist = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cb_ShowBeacons = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cb_KST_ShowBalloon = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tb_KST_Name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_KST_Locator = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_KST_Show_Own_Messages = new System.Windows.Forms.CheckBox();
            this.rb_KST_StartAsAway = new System.Windows.Forms.RadioButton();
            this.rb_KST_StartAsHere = new System.Windows.Forms.RadioButton();
            this.cb_KST_AutoConnect = new System.Windows.Forms.CheckBox();
            this.cbb_KST_Chat = new System.Windows.Forms.ComboBox();
            this.tb_KST_Password = new System.Windows.Forms.TextBox();
            this.tb_KST_UserName = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cb_DXLog_Active = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cb_QARTest_Active = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_WinTestNet_Active = new System.Windows.Forms.CheckBox();
            this.tb_Options_WinTest_Station_Name = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_WinTest_Active = new System.Windows.Forms.CheckBox();
            this.tb_Options_WinTest_INI = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.Bands = new System.Windows.Forms.GroupBox();
            this.checkBox76GHz = new System.Windows.Forms.CheckBox();
            this.checkBox10GHz = new System.Windows.Forms.CheckBox();
            this.checkBox24GHz = new System.Windows.Forms.CheckBox();
            this.checkBox47GHz = new System.Windows.Forms.CheckBox();
            this.checkBox2320 = new System.Windows.Forms.CheckBox();
            this.checkBox3400 = new System.Windows.Forms.CheckBox();
            this.checkBox5760 = new System.Windows.Forms.CheckBox();
            this.checkBox1296 = new System.Windows.Forms.CheckBox();
            this.checkBox432 = new System.Windows.Forms.CheckBox();
            this.checkBox144 = new System.Windows.Forms.CheckBox();
            this.checkBox70 = new System.Windows.Forms.CheckBox();
            this.checkBox50 = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.cb_AS_QRG = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tb_Options_AS_My_Name = new System.Windows.Forms.TextBox();
            this.tb_AS_MinDist = new System.Windows.Forms.TextBox();
            this.tb_Options_AS_Server_Name = new System.Windows.Forms.TextBox();
            this.tb_AS_MaxDist = new System.Windows.Forms.TextBox();
            this.panel_AS = new System.Windows.Forms.Panel();
            this.label_local_server = new System.Windows.Forms.Label();
            this.cb_AS_local = new System.Windows.Forms.CheckBox();
            this.label_timeout2 = new System.Windows.Forms.Label();
            this.label_timeout = new System.Windows.Forms.Label();
            this.cb_AS_Active = new System.Windows.Forms.CheckBox();
            this.tb_AS_Timeout = new System.Windows.Forms.TextBox();
            this.tb_Options_AS_Local_Server_Name = new System.Windows.Forms.TextBox();
            this.panel_WS = new System.Windows.Forms.Panel();
            this.tb_WS_Password = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.tb_WS_API_URL = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tb_WS_LoginURL = new System.Windows.Forms.TextBox();
            this.label_WS_Password = new System.Windows.Forms.Label();
            this.label_WS_Username = new System.Windows.Forms.Label();
            this.tb_WS_Username = new System.Windows.Forms.TextBox();
            this.cb_WS_Active = new System.Windows.Forms.CheckBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.tB_macro_2 = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.cB_macro_2 = new System.Windows.Forms.CheckBox();
            this.tB_macro_5 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.cB_macro_5 = new System.Windows.Forms.CheckBox();
            this.tB_macro_6 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.cB_macro_6 = new System.Windows.Forms.CheckBox();
            this.tB_macro_3 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.cB_macro_3 = new System.Windows.Forms.CheckBox();
            this.tB_macro_4 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.cB_macro_4 = new System.Windows.Forms.CheckBox();
            this.tB_macro_1 = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cB_macro_1 = new System.Windows.Forms.CheckBox();
            this.tb_Options_DXLog_Station_Name = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.Bands.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_AS.SuspendLayout();
            this.panel_WS.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(521, 36);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 1;
            this.btn_OK.Text = "&OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(521, 68);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "&Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Options_WinTest_INI_Select
            // 
            this.btn_Options_WinTest_INI_Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Options_WinTest_INI_Select.Location = new System.Drawing.Point(329, 36);
            this.btn_Options_WinTest_INI_Select.Name = "btn_Options_WinTest_INI_Select";
            this.btn_Options_WinTest_INI_Select.Size = new System.Drawing.Size(75, 23);
            this.btn_Options_WinTest_INI_Select.TabIndex = 14;
            this.btn_Options_WinTest_INI_Select.Text = "Select";
            this.btn_Options_WinTest_INI_Select.UseVisualStyleBackColor = true;
            this.btn_Options_WinTest_INI_Select.Click += new System.EventHandler(this.btn_Options_WinTest_INI_Select_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(5, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "wt.ini :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_KST_MaxDist);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.cb_ShowBeacons);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(9, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(445, 62);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Calls";
            // 
            // tb_KST_MaxDist
            // 
            this.tb_KST_MaxDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_MaxDist", true));
            this.tb_KST_MaxDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_MaxDist.Location = new System.Drawing.Point(241, 29);
            this.tb_KST_MaxDist.Name = "tb_KST_MaxDist";
            this.tb_KST_MaxDist.Size = new System.Drawing.Size(45, 20);
            this.tb_KST_MaxDist.TabIndex = 7;
            this.tb_KST_MaxDist.Text = global::wtKST.Properties.Settings.Default.KST_MaxDist;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(124, 32);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(101, 13);
            this.label17.TabIndex = 6;
            this.label17.Text = "Max. Distance [km]:";
            // 
            // cb_ShowBeacons
            // 
            this.cb_ShowBeacons.AutoSize = true;
            this.cb_ShowBeacons.Checked = global::wtKST.Properties.Settings.Default.ShowBeacons;
            this.cb_ShowBeacons.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "ShowBeacons", true));
            this.cb_ShowBeacons.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ShowBeacons.Location = new System.Drawing.Point(9, 32);
            this.cb_ShowBeacons.Name = "cb_ShowBeacons";
            this.cb_ShowBeacons.Size = new System.Drawing.Size(98, 17);
            this.cb_ShowBeacons.TabIndex = 5;
            this.cb_ShowBeacons.Text = "Show Beacons";
            this.cb_ShowBeacons.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cb_KST_ShowBalloon);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(9, 215);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(445, 62);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Notifications";
            // 
            // cb_KST_ShowBalloon
            // 
            this.cb_KST_ShowBalloon.AutoSize = true;
            this.cb_KST_ShowBalloon.Checked = global::wtKST.Properties.Settings.Default.KST_ShowBalloon;
            this.cb_KST_ShowBalloon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_KST_ShowBalloon.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_ShowBalloon", true));
            this.cb_KST_ShowBalloon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_KST_ShowBalloon.Location = new System.Drawing.Point(9, 29);
            this.cb_KST_ShowBalloon.Name = "cb_KST_ShowBalloon";
            this.cb_KST_ShowBalloon.Size = new System.Drawing.Size(241, 17);
            this.cb_KST_ShowBalloon.TabIndex = 30;
            this.cb_KST_ShowBalloon.Text = "On new MyMessages show ballon notification";
            this.cb_KST_ShowBalloon.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "User :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Password :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Chat :";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(479, 367);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tb_KST_Name);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.tb_KST_Locator);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.checkBox_KST_Show_Own_Messages);
            this.tabPage1.Controls.Add(this.rb_KST_StartAsAway);
            this.tabPage1.Controls.Add(this.rb_KST_StartAsHere);
            this.tabPage1.Controls.Add(this.cb_KST_AutoConnect);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.cbb_KST_Chat);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.tb_KST_Password);
            this.tabPage1.Controls.Add(this.tb_KST_UserName);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(471, 290);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "KST";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tb_KST_Name
            // 
            this.tb_KST_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Name", true));
            this.tb_KST_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_Name.Location = new System.Drawing.Point(120, 142);
            this.tb_KST_Name.Name = "tb_KST_Name";
            this.tb_KST_Name.Size = new System.Drawing.Size(121, 20);
            this.tb_KST_Name.TabIndex = 12;
            this.tb_KST_Name.Text = global::wtKST.Properties.Settings.Default.KST_Name;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name :";
            // 
            // tb_KST_Locator
            // 
            this.tb_KST_Locator.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tb_KST_Locator.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Loc", true));
            this.tb_KST_Locator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_Locator.Location = new System.Drawing.Point(120, 116);
            this.tb_KST_Locator.Name = "tb_KST_Locator";
            this.tb_KST_Locator.Size = new System.Drawing.Size(121, 20);
            this.tb_KST_Locator.TabIndex = 11;
            this.tb_KST_Locator.Text = global::wtKST.Properties.Settings.Default.KST_Loc;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 116);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Locator :";
            // 
            // checkBox_KST_Show_Own_Messages
            // 
            this.checkBox_KST_Show_Own_Messages.AutoSize = true;
            this.checkBox_KST_Show_Own_Messages.Checked = global::wtKST.Properties.Settings.Default.KST_Show_Own_Messages;
            this.checkBox_KST_Show_Own_Messages.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_Show_Own_Messages", true));
            this.checkBox_KST_Show_Own_Messages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_KST_Show_Own_Messages.Location = new System.Drawing.Point(275, 167);
            this.checkBox_KST_Show_Own_Messages.Name = "checkBox_KST_Show_Own_Messages";
            this.checkBox_KST_Show_Own_Messages.Size = new System.Drawing.Size(126, 17);
            this.checkBox_KST_Show_Own_Messages.TabIndex = 16;
            this.checkBox_KST_Show_Own_Messages.Text = "Show own messages";
            this.checkBox_KST_Show_Own_Messages.UseVisualStyleBackColor = true;
            // 
            // rb_KST_StartAsAway
            // 
            this.rb_KST_StartAsAway.AutoSize = true;
            this.rb_KST_StartAsAway.Checked = global::wtKST.Properties.Settings.Default.KST_StartAsAway;
            this.rb_KST_StartAsAway.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_StartAsAway", true));
            this.rb_KST_StartAsAway.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb_KST_StartAsAway.Location = new System.Drawing.Point(275, 101);
            this.rb_KST_StartAsAway.Name = "rb_KST_StartAsAway";
            this.rb_KST_StartAsAway.Size = new System.Drawing.Size(96, 17);
            this.rb_KST_StartAsAway.TabIndex = 15;
            this.rb_KST_StartAsAway.TabStop = true;
            this.rb_KST_StartAsAway.Text = "Start as AWAY";
            this.rb_KST_StartAsAway.UseVisualStyleBackColor = true;
            this.rb_KST_StartAsAway.Click += new System.EventHandler(this.rb_KST_StartAsAwayHere_Click);
            // 
            // rb_KST_StartAsHere
            // 
            this.rb_KST_StartAsHere.AutoSize = true;
            this.rb_KST_StartAsHere.Checked = global::wtKST.Properties.Settings.Default.KST_StartAsHere;
            this.rb_KST_StartAsHere.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_StartAsHere", true));
            this.rb_KST_StartAsHere.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb_KST_StartAsHere.Location = new System.Drawing.Point(275, 70);
            this.rb_KST_StartAsHere.Name = "rb_KST_StartAsHere";
            this.rb_KST_StartAsHere.Size = new System.Drawing.Size(94, 17);
            this.rb_KST_StartAsHere.TabIndex = 14;
            this.rb_KST_StartAsHere.Text = "Start as HERE";
            this.rb_KST_StartAsHere.UseVisualStyleBackColor = true;
            this.rb_KST_StartAsHere.Click += new System.EventHandler(this.rb_KST_StartAsAwayHere_Click);
            // 
            // cb_KST_AutoConnect
            // 
            this.cb_KST_AutoConnect.AutoSize = true;
            this.cb_KST_AutoConnect.Checked = global::wtKST.Properties.Settings.Default.KST_AutoConnect;
            this.cb_KST_AutoConnect.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_AutoConnect", true));
            this.cb_KST_AutoConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_KST_AutoConnect.Location = new System.Drawing.Point(277, 21);
            this.cb_KST_AutoConnect.Name = "cb_KST_AutoConnect";
            this.cb_KST_AutoConnect.Size = new System.Drawing.Size(90, 17);
            this.cb_KST_AutoConnect.TabIndex = 13;
            this.cb_KST_AutoConnect.Text = "Auto connect";
            this.cb_KST_AutoConnect.UseVisualStyleBackColor = true;
            // 
            // cbb_KST_Chat
            // 
            this.cbb_KST_Chat.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Chat", true));
            this.cbb_KST_Chat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_KST_Chat.FormattingEnabled = true;
            this.cbb_KST_Chat.Items.AddRange(new object[] {
            "1 - 50/70 MHz",
            "2 - 144/432 MHz",
            "3 - Microwave",
            "4 - EME/JT65",
            "5 - Low Band",
            "6 - 50 MHz IARU R 3",
            "7 - 50 MHz IARU R 2",
            "8 - 144/432 MHz R 2",
            "9 - 144/432 MHz R 3"});
            this.cbb_KST_Chat.Location = new System.Drawing.Point(120, 79);
            this.cbb_KST_Chat.Name = "cbb_KST_Chat";
            this.cbb_KST_Chat.Size = new System.Drawing.Size(121, 21);
            this.cbb_KST_Chat.TabIndex = 10;
            this.cbb_KST_Chat.Text = global::wtKST.Properties.Settings.Default.KST_Chat;
            // 
            // tb_KST_Password
            // 
            this.tb_KST_Password.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Password", true));
            this.tb_KST_Password.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_Password.Location = new System.Drawing.Point(120, 53);
            this.tb_KST_Password.Name = "tb_KST_Password";
            this.tb_KST_Password.PasswordChar = '*';
            this.tb_KST_Password.Size = new System.Drawing.Size(121, 20);
            this.tb_KST_Password.TabIndex = 9;
            this.tb_KST_Password.Text = global::wtKST.Properties.Settings.Default.KST_Password;
            // 
            // tb_KST_UserName
            // 
            this.tb_KST_UserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_UserName", true));
            this.tb_KST_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_UserName.Location = new System.Drawing.Point(120, 26);
            this.tb_KST_UserName.Name = "tb_KST_UserName";
            this.tb_KST_UserName.Size = new System.Drawing.Size(121, 20);
            this.tb_KST_UserName.TabIndex = 8;
            this.tb_KST_UserName.Text = global::wtKST.Properties.Settings.Default.KST_UserName;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(471, 341);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Log";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.tb_Options_DXLog_Station_Name);
            this.groupBox6.Controls.Add(this.label28);
            this.groupBox6.Controls.Add(this.cb_DXLog_Active);
            this.groupBox6.Location = new System.Drawing.Point(22, 199);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(422, 64);
            this.groupBox6.TabIndex = 18;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "DXLog Network sync";
            // 
            // cb_DXLog_Active
            // 
            this.cb_DXLog_Active.AutoSize = true;
            this.cb_DXLog_Active.Checked = global::wtKST.Properties.Settings.Default.DXLog_Sync_active;
            this.cb_DXLog_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "DXLog_Sync_active", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_DXLog_Active.Enabled = false;
            this.cb_DXLog_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_DXLog_Active.Location = new System.Drawing.Point(8, 22);
            this.cb_DXLog_Active.Name = "cb_DXLog_Active";
            this.cb_DXLog_Active.Size = new System.Drawing.Size(65, 17);
            this.cb_DXLog_Active.TabIndex = 10;
            this.cb_DXLog_Active.Text = "Activate";
            this.cb_DXLog_Active.UseVisualStyleBackColor = true;
            this.cb_DXLog_Active.CheckedChanged += new System.EventHandler(this.cb_DXLog_Active_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cb_QARTest_Active);
            this.groupBox4.Location = new System.Drawing.Point(22, 281);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(173, 46);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "QARTest Network sync";
            // 
            // cb_QARTest_Active
            // 
            this.cb_QARTest_Active.AutoSize = true;
            this.cb_QARTest_Active.Checked = global::wtKST.Properties.Settings.Default.QARTest_Sync_active;
            this.cb_QARTest_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "QARTest_Sync_active", true));
            this.cb_QARTest_Active.Enabled = false;
            this.cb_QARTest_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_QARTest_Active.Location = new System.Drawing.Point(8, 22);
            this.cb_QARTest_Active.Name = "cb_QARTest_Active";
            this.cb_QARTest_Active.Size = new System.Drawing.Size(65, 17);
            this.cb_QARTest_Active.TabIndex = 10;
            this.cb_QARTest_Active.Text = "Activate";
            this.cb_QARTest_Active.UseVisualStyleBackColor = true;
            this.cb_QARTest_Active.CheckedChanged += new System.EventHandler(this.cb_QARTest_Active_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_WinTestNet_Active);
            this.groupBox2.Controls.Add(this.tb_Options_WinTest_Station_Name);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(22, 114);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(422, 64);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Win-Test Network sync";
            // 
            // cb_WinTestNet_Active
            // 
            this.cb_WinTestNet_Active.AutoSize = true;
            this.cb_WinTestNet_Active.Checked = global::wtKST.Properties.Settings.Default.WinTest_NetworkSync_active;
            this.cb_WinTestNet_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "WinTest_NetworkSync_active", true));
            this.cb_WinTestNet_Active.Enabled = false;
            this.cb_WinTestNet_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_WinTestNet_Active.Location = new System.Drawing.Point(8, 22);
            this.cb_WinTestNet_Active.Name = "cb_WinTestNet_Active";
            this.cb_WinTestNet_Active.Size = new System.Drawing.Size(65, 17);
            this.cb_WinTestNet_Active.TabIndex = 10;
            this.cb_WinTestNet_Active.Text = "Activate";
            this.cb_WinTestNet_Active.UseVisualStyleBackColor = true;
            this.cb_WinTestNet_Active.CheckedChanged += new System.EventHandler(this.cb_WinTestNet_Active_CheckedChanged);
            // 
            // tb_Options_WinTest_Station_Name
            // 
            this.tb_Options_WinTest_Station_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WinTest_StationName", true));
            this.tb_Options_WinTest_Station_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_WinTest_Station_Name.Location = new System.Drawing.Point(112, 40);
            this.tb_Options_WinTest_Station_Name.Name = "tb_Options_WinTest_Station_Name";
            this.tb_Options_WinTest_Station_Name.Size = new System.Drawing.Size(204, 20);
            this.tb_Options_WinTest_Station_Name.TabIndex = 13;
            this.tb_Options_WinTest_Station_Name.Text = global::wtKST.Properties.Settings.Default.WinTest_StationName;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(5, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(74, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Station Name:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Options_WinTest_INI_Select);
            this.groupBox1.Controls.Add(this.cb_WinTest_Active);
            this.groupBox1.Controls.Add(this.tb_Options_WinTest_INI);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(22, 36);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(422, 64);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Win-Test file based";
            // 
            // cb_WinTest_Active
            // 
            this.cb_WinTest_Active.AutoSize = true;
            this.cb_WinTest_Active.Checked = global::wtKST.Properties.Settings.Default.WinTest_Activate;
            this.cb_WinTest_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "WinTest_Activate", true));
            this.cb_WinTest_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_WinTest_Active.Location = new System.Drawing.Point(8, 22);
            this.cb_WinTest_Active.Name = "cb_WinTest_Active";
            this.cb_WinTest_Active.Size = new System.Drawing.Size(65, 17);
            this.cb_WinTest_Active.TabIndex = 10;
            this.cb_WinTest_Active.Text = "Activate";
            this.cb_WinTest_Active.UseVisualStyleBackColor = true;
            this.cb_WinTest_Active.CheckedChanged += new System.EventHandler(this.cb_WinTest_Active_CheckedChanged);
            // 
            // tb_Options_WinTest_INI
            // 
            this.tb_Options_WinTest_INI.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WinTest_INI_Filename", true));
            this.tb_Options_WinTest_INI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_WinTest_INI.Location = new System.Drawing.Point(112, 40);
            this.tb_Options_WinTest_INI.Name = "tb_Options_WinTest_INI";
            this.tb_Options_WinTest_INI.Size = new System.Drawing.Size(204, 20);
            this.tb_Options_WinTest_INI.TabIndex = 13;
            this.tb_Options_WinTest_INI.Text = global::wtKST.Properties.Settings.Default.WinTest_INI_FileName;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Bands);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(471, 290);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Calls";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Bands
            // 
            this.Bands.Controls.Add(this.checkBox76GHz);
            this.Bands.Controls.Add(this.checkBox10GHz);
            this.Bands.Controls.Add(this.checkBox24GHz);
            this.Bands.Controls.Add(this.checkBox47GHz);
            this.Bands.Controls.Add(this.checkBox2320);
            this.Bands.Controls.Add(this.checkBox3400);
            this.Bands.Controls.Add(this.checkBox5760);
            this.Bands.Controls.Add(this.checkBox1296);
            this.Bands.Controls.Add(this.checkBox432);
            this.Bands.Controls.Add(this.checkBox144);
            this.Bands.Controls.Add(this.checkBox70);
            this.Bands.Controls.Add(this.checkBox50);
            this.Bands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bands.Location = new System.Drawing.Point(9, 87);
            this.Bands.Name = "Bands";
            this.Bands.Size = new System.Drawing.Size(445, 122);
            this.Bands.TabIndex = 8;
            this.Bands.TabStop = false;
            this.Bands.Text = "Bands";
            // 
            // checkBox76GHz
            // 
            this.checkBox76GHz.AutoSize = true;
            this.checkBox76GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox76GHz.Checked = global::wtKST.Properties.Settings.Default.Band_76GHz;
            this.checkBox76GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox76GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_76GHz", true));
            this.checkBox76GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox76GHz.Location = new System.Drawing.Point(257, 96);
            this.checkBox76GHz.Name = "checkBox76GHz";
            this.checkBox76GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox76GHz.TabIndex = 26;
            this.checkBox76GHz.Text = "76GHz";
            this.checkBox76GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox10GHz
            // 
            this.checkBox10GHz.AutoSize = true;
            this.checkBox10GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox10GHz.Checked = global::wtKST.Properties.Settings.Default.Band_10368;
            this.checkBox10GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox10GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_10368", true));
            this.checkBox10GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox10GHz.Location = new System.Drawing.Point(257, 73);
            this.checkBox10GHz.Name = "checkBox10GHz";
            this.checkBox10GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox10GHz.TabIndex = 23;
            this.checkBox10GHz.Text = "10GHz";
            this.checkBox10GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox24GHz
            // 
            this.checkBox24GHz.AutoSize = true;
            this.checkBox24GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox24GHz.Checked = global::wtKST.Properties.Settings.Default.Band_24GHz;
            this.checkBox24GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox24GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_24GHz", true));
            this.checkBox24GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox24GHz.Location = new System.Drawing.Point(48, 96);
            this.checkBox24GHz.Name = "checkBox24GHz";
            this.checkBox24GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox24GHz.TabIndex = 24;
            this.checkBox24GHz.Text = "24GHz";
            this.checkBox24GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox47GHz
            // 
            this.checkBox47GHz.AutoSize = true;
            this.checkBox47GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox47GHz.Checked = global::wtKST.Properties.Settings.Default.Band_47GHz;
            this.checkBox47GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox47GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_47GHz", true));
            this.checkBox47GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox47GHz.Location = new System.Drawing.Point(156, 96);
            this.checkBox47GHz.Name = "checkBox47GHz";
            this.checkBox47GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox47GHz.TabIndex = 25;
            this.checkBox47GHz.Text = "47GHz";
            this.checkBox47GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox2320
            // 
            this.checkBox2320.AutoSize = true;
            this.checkBox2320.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox2320.Checked = global::wtKST.Properties.Settings.Default.Band_2320;
            this.checkBox2320.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2320.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_2320", true));
            this.checkBox2320.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2320.Location = new System.Drawing.Point(244, 50);
            this.checkBox2320.Name = "checkBox2320";
            this.checkBox2320.Size = new System.Drawing.Size(72, 17);
            this.checkBox2320.TabIndex = 20;
            this.checkBox2320.Text = "2320MHz";
            this.checkBox2320.UseVisualStyleBackColor = true;
            // 
            // checkBox3400
            // 
            this.checkBox3400.AutoSize = true;
            this.checkBox3400.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox3400.Checked = global::wtKST.Properties.Settings.Default.Band_3400;
            this.checkBox3400.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3400.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_3400", true));
            this.checkBox3400.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3400.Location = new System.Drawing.Point(35, 73);
            this.checkBox3400.Name = "checkBox3400";
            this.checkBox3400.Size = new System.Drawing.Size(72, 17);
            this.checkBox3400.TabIndex = 21;
            this.checkBox3400.Text = "3400MHz";
            this.checkBox3400.UseVisualStyleBackColor = true;
            // 
            // checkBox5760
            // 
            this.checkBox5760.AutoSize = true;
            this.checkBox5760.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox5760.Checked = global::wtKST.Properties.Settings.Default.Band_5760;
            this.checkBox5760.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5760.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_5760", true));
            this.checkBox5760.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox5760.Location = new System.Drawing.Point(143, 73);
            this.checkBox5760.Name = "checkBox5760";
            this.checkBox5760.Size = new System.Drawing.Size(72, 17);
            this.checkBox5760.TabIndex = 22;
            this.checkBox5760.Text = "5760MHz";
            this.checkBox5760.UseVisualStyleBackColor = true;
            // 
            // checkBox1296
            // 
            this.checkBox1296.AutoSize = true;
            this.checkBox1296.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1296.Checked = global::wtKST.Properties.Settings.Default.Band_1296;
            this.checkBox1296.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1296.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_1296", true));
            this.checkBox1296.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1296.Location = new System.Drawing.Point(143, 50);
            this.checkBox1296.Name = "checkBox1296";
            this.checkBox1296.Size = new System.Drawing.Size(72, 17);
            this.checkBox1296.TabIndex = 19;
            this.checkBox1296.Text = "1296MHz";
            this.checkBox1296.UseVisualStyleBackColor = true;
            // 
            // checkBox432
            // 
            this.checkBox432.AutoSize = true;
            this.checkBox432.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox432.Checked = global::wtKST.Properties.Settings.Default.Band_432;
            this.checkBox432.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox432.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_432", true));
            this.checkBox432.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox432.Location = new System.Drawing.Point(41, 50);
            this.checkBox432.Name = "checkBox432";
            this.checkBox432.Size = new System.Drawing.Size(66, 17);
            this.checkBox432.TabIndex = 18;
            this.checkBox432.Text = "432MHz";
            this.checkBox432.UseVisualStyleBackColor = true;
            // 
            // checkBox144
            // 
            this.checkBox144.AutoSize = true;
            this.checkBox144.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox144.Checked = global::wtKST.Properties.Settings.Default.Band_144;
            this.checkBox144.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox144.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_144", true));
            this.checkBox144.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox144.Location = new System.Drawing.Point(250, 27);
            this.checkBox144.Name = "checkBox144";
            this.checkBox144.Size = new System.Drawing.Size(66, 17);
            this.checkBox144.TabIndex = 17;
            this.checkBox144.Text = "144MHz";
            this.checkBox144.UseVisualStyleBackColor = true;
            // 
            // checkBox70
            // 
            this.checkBox70.AutoSize = true;
            this.checkBox70.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox70.Checked = global::wtKST.Properties.Settings.Default.Band_70;
            this.checkBox70.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox70.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_70", true));
            this.checkBox70.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox70.Location = new System.Drawing.Point(155, 27);
            this.checkBox70.Name = "checkBox70";
            this.checkBox70.Size = new System.Drawing.Size(60, 17);
            this.checkBox70.TabIndex = 16;
            this.checkBox70.Text = "70MHz";
            this.checkBox70.UseVisualStyleBackColor = true;
            // 
            // checkBox50
            // 
            this.checkBox50.AutoSize = true;
            this.checkBox50.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox50.Checked = global::wtKST.Properties.Settings.Default.Band_50;
            this.checkBox50.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox50.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_50", true));
            this.checkBox50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox50.Location = new System.Drawing.Point(47, 27);
            this.checkBox50.Name = "checkBox50";
            this.checkBox50.Size = new System.Drawing.Size(60, 17);
            this.checkBox50.TabIndex = 15;
            this.checkBox50.Text = "50MHz";
            this.checkBox50.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel2);
            this.tabPage4.Controls.Add(this.panel_AS);
            this.tabPage4.Controls.Add(this.panel_WS);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(471, 290);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Airplane Scatter";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.cb_AS_QRG);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.tb_Options_AS_My_Name);
            this.panel2.Controls.Add(this.tb_AS_MinDist);
            this.panel2.Controls.Add(this.tb_Options_AS_Server_Name);
            this.panel2.Controls.Add(this.tb_AS_MaxDist);
            this.panel2.Location = new System.Drawing.Point(5, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(446, 75);
            this.panel2.TabIndex = 0;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(302, 22);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 13);
            this.label16.TabIndex = 10;
            this.label16.Text = "Frequency:";
            // 
            // cb_AS_QRG
            // 
            this.cb_AS_QRG.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_QRG", true));
            this.cb_AS_QRG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_AS_QRG.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AS_QRG.FormattingEnabled = true;
            this.cb_AS_QRG.Items.AddRange(new object[] {
            "50M",
            "70M",
            "144M",
            "432M",
            "1.2G",
            "2.3G",
            "3.4G",
            "5.7G",
            "10G"});
            this.cb_AS_QRG.Location = new System.Drawing.Point(377, 18);
            this.cb_AS_QRG.Name = "cb_AS_QRG";
            this.cb_AS_QRG.Size = new System.Drawing.Size(64, 21);
            this.cb_AS_QRG.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(224, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Get aircraft positions in the range of minumum:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(161, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 13);
            this.label15.TabIndex = 8;
            this.label15.Text = "My Name:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(275, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "kms up to maximum:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(12, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(85, 13);
            this.label14.TabIndex = 6;
            this.label14.Text = "Server/Channel:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(416, 48);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "kms.";
            // 
            // tb_Options_AS_My_Name
            // 
            this.tb_Options_AS_My_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_My_Name", true));
            this.tb_Options_AS_My_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_AS_My_Name.Location = new System.Drawing.Point(220, 19);
            this.tb_Options_AS_My_Name.Name = "tb_Options_AS_My_Name";
            this.tb_Options_AS_My_Name.Size = new System.Drawing.Size(66, 20);
            this.tb_Options_AS_My_Name.TabIndex = 9;
            this.tb_Options_AS_My_Name.Text = global::wtKST.Properties.Settings.Default.AS_My_Name;
            // 
            // tb_AS_MinDist
            // 
            this.tb_AS_MinDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_MinDist", true));
            this.tb_AS_MinDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AS_MinDist.Location = new System.Drawing.Point(240, 45);
            this.tb_AS_MinDist.Name = "tb_AS_MinDist";
            this.tb_AS_MinDist.Size = new System.Drawing.Size(32, 20);
            this.tb_AS_MinDist.TabIndex = 13;
            this.tb_AS_MinDist.Text = global::wtKST.Properties.Settings.Default.AS_MinDist;
            // 
            // tb_Options_AS_Server_Name
            // 
            this.tb_Options_AS_Server_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Server_Name", true));
            this.tb_Options_AS_Server_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_AS_Server_Name.Location = new System.Drawing.Point(103, 19);
            this.tb_Options_AS_Server_Name.Name = "tb_Options_AS_Server_Name";
            this.tb_Options_AS_Server_Name.Size = new System.Drawing.Size(51, 20);
            this.tb_Options_AS_Server_Name.TabIndex = 7;
            this.tb_Options_AS_Server_Name.Text = global::wtKST.Properties.Settings.Default.AS_Server_Name;
            // 
            // tb_AS_MaxDist
            // 
            this.tb_AS_MaxDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_MaxDist", true));
            this.tb_AS_MaxDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AS_MaxDist.Location = new System.Drawing.Point(377, 45);
            this.tb_AS_MaxDist.Name = "tb_AS_MaxDist";
            this.tb_AS_MaxDist.Size = new System.Drawing.Size(33, 20);
            this.tb_AS_MaxDist.TabIndex = 15;
            this.tb_AS_MaxDist.Text = global::wtKST.Properties.Settings.Default.AS_MaxDist;
            // 
            // panel_AS
            // 
            this.panel_AS.Controls.Add(this.label_local_server);
            this.panel_AS.Controls.Add(this.cb_AS_local);
            this.panel_AS.Controls.Add(this.label_timeout2);
            this.panel_AS.Controls.Add(this.label_timeout);
            this.panel_AS.Controls.Add(this.cb_AS_Active);
            this.panel_AS.Controls.Add(this.tb_AS_Timeout);
            this.panel_AS.Controls.Add(this.tb_Options_AS_Local_Server_Name);
            this.panel_AS.Location = new System.Drawing.Point(5, 84);
            this.panel_AS.Name = "panel_AS";
            this.panel_AS.Size = new System.Drawing.Size(446, 59);
            this.panel_AS.TabIndex = 35;
            // 
            // label_local_server
            // 
            this.label_local_server.AutoSize = true;
            this.label_local_server.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_local_server.Location = new System.Drawing.Point(231, 37);
            this.label_local_server.Name = "label_local_server";
            this.label_local_server.Size = new System.Drawing.Size(70, 13);
            this.label_local_server.TabIndex = 11;
            this.label_local_server.Text = "Local Server:";
            // 
            // cb_AS_local
            // 
            this.cb_AS_local.AutoSize = true;
            this.cb_AS_local.Checked = global::wtKST.Properties.Settings.Default.AS_Local_Active;
            this.cb_AS_local.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "AS_Local_Active", true));
            this.cb_AS_local.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AS_local.Location = new System.Drawing.Point(33, 35);
            this.cb_AS_local.Name = "cb_AS_local";
            this.cb_AS_local.Size = new System.Drawing.Size(177, 17);
            this.cb_AS_local.TabIndex = 7;
            this.cb_AS_local.Text = "Use local server for double click";
            this.cb_AS_local.UseVisualStyleBackColor = true;
            this.cb_AS_local.CheckedChanged += new System.EventHandler(this.cb_AS_local_CheckedChanged);
            this.cb_AS_local.EnabledChanged += new System.EventHandler(this.cb_AS_local_EnableChanged);
            // 
            // label_timeout2
            // 
            this.label_timeout2.AutoSize = true;
            this.label_timeout2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_timeout2.Location = new System.Drawing.Point(414, 14);
            this.label_timeout2.Name = "label_timeout2";
            this.label_timeout2.Size = new System.Drawing.Size(27, 13);
            this.label_timeout2.TabIndex = 20;
            this.label_timeout2.Text = "sec.";
            // 
            // label_timeout
            // 
            this.label_timeout.AutoSize = true;
            this.label_timeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_timeout.Location = new System.Drawing.Point(272, 14);
            this.label_timeout.Name = "label_timeout";
            this.label_timeout.Size = new System.Drawing.Size(109, 13);
            this.label_timeout.TabIndex = 13;
            this.label_timeout.Text = "Timeout for response:";
            // 
            // cb_AS_Active
            // 
            this.cb_AS_Active.AutoSize = true;
            this.cb_AS_Active.Checked = global::wtKST.Properties.Settings.Default.AS_Active;
            this.cb_AS_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "AS_Active", true));
            this.cb_AS_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AS_Active.Location = new System.Drawing.Point(13, 13);
            this.cb_AS_Active.Name = "cb_AS_Active";
            this.cb_AS_Active.Size = new System.Drawing.Size(243, 17);
            this.cb_AS_Active.TabIndex = 5;
            this.cb_AS_Active.Text = "Activate local Airscout service (same network)";
            this.cb_AS_Active.UseVisualStyleBackColor = true;
            this.cb_AS_Active.CheckedChanged += new System.EventHandler(this.cb_AS_Active_CheckedChanged);
            // 
            // tb_AS_Timeout
            // 
            this.tb_AS_Timeout.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Timeout", true));
            this.tb_AS_Timeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AS_Timeout.Location = new System.Drawing.Point(387, 10);
            this.tb_AS_Timeout.Name = "tb_AS_Timeout";
            this.tb_AS_Timeout.Size = new System.Drawing.Size(23, 20);
            this.tb_AS_Timeout.TabIndex = 6;
            this.tb_AS_Timeout.Text = global::wtKST.Properties.Settings.Default.AS_Timeout;
            // 
            // tb_Options_AS_Local_Server_Name
            // 
            this.tb_Options_AS_Local_Server_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Local_Name", true));
            this.tb_Options_AS_Local_Server_Name.Enabled = global::wtKST.Properties.Settings.Default.AS_Local_Active;
            this.tb_Options_AS_Local_Server_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_AS_Local_Server_Name.Location = new System.Drawing.Point(305, 33);
            this.tb_Options_AS_Local_Server_Name.Name = "tb_Options_AS_Local_Server_Name";
            this.tb_Options_AS_Local_Server_Name.Size = new System.Drawing.Size(51, 20);
            this.tb_Options_AS_Local_Server_Name.TabIndex = 8;
            this.tb_Options_AS_Local_Server_Name.Text = global::wtKST.Properties.Settings.Default.AS_Local_Name;
            // 
            // panel_WS
            // 
            this.panel_WS.Controls.Add(this.tb_WS_Password);
            this.panel_WS.Controls.Add(this.label31);
            this.panel_WS.Controls.Add(this.tb_WS_API_URL);
            this.panel_WS.Controls.Add(this.label29);
            this.panel_WS.Controls.Add(this.tb_WS_LoginURL);
            this.panel_WS.Controls.Add(this.label_WS_Password);
            this.panel_WS.Controls.Add(this.label_WS_Username);
            this.panel_WS.Controls.Add(this.tb_WS_Username);
            this.panel_WS.Controls.Add(this.cb_WS_Active);
            this.panel_WS.Location = new System.Drawing.Point(5, 149);
            this.panel_WS.Name = "panel_WS";
            this.panel_WS.Size = new System.Drawing.Size(446, 138);
            this.panel_WS.TabIndex = 37;
            // 
            // tb_WS_Password
            // 
            this.tb_WS_Password.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WS_Password", true));
            this.tb_WS_Password.Location = new System.Drawing.Point(341, 37);
            this.tb_WS_Password.Name = "tb_WS_Password";
            this.tb_WS_Password.PasswordChar = '*';
            this.tb_WS_Password.Size = new System.Drawing.Size(100, 20);
            this.tb_WS_Password.TabIndex = 31;
            this.tb_WS_Password.Text = global::wtKST.Properties.Settings.Default.WS_Password;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(10, 102);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(49, 13);
            this.label31.TabIndex = 36;
            this.label31.Text = "API URL";
            // 
            // tb_WS_API_URL
            // 
            this.tb_WS_API_URL.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WS_API_URL", true));
            this.tb_WS_API_URL.Location = new System.Drawing.Point(79, 99);
            this.tb_WS_API_URL.Name = "tb_WS_API_URL";
            this.tb_WS_API_URL.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tb_WS_API_URL.Size = new System.Drawing.Size(362, 20);
            this.tb_WS_API_URL.TabIndex = 35;
            this.tb_WS_API_URL.Text = global::wtKST.Properties.Settings.Default.WS_Username;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(10, 76);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(58, 13);
            this.label29.TabIndex = 34;
            this.label29.Text = "Login URL";
            // 
            // tb_WS_LoginURL
            // 
            this.tb_WS_LoginURL.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WS_LoginURL", true));
            this.tb_WS_LoginURL.Location = new System.Drawing.Point(79, 73);
            this.tb_WS_LoginURL.Name = "tb_WS_LoginURL";
            this.tb_WS_LoginURL.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tb_WS_LoginURL.Size = new System.Drawing.Size(362, 20);
            this.tb_WS_LoginURL.TabIndex = 33;
            this.tb_WS_LoginURL.Text = global::wtKST.Properties.Settings.Default.WS_Username;
            // 
            // label_WS_Password
            // 
            this.label_WS_Password.AutoSize = true;
            this.label_WS_Password.Location = new System.Drawing.Point(230, 40);
            this.label_WS_Password.Name = "label_WS_Password";
            this.label_WS_Password.Size = new System.Drawing.Size(99, 13);
            this.label_WS_Password.TabIndex = 32;
            this.label_WS_Password.Text = "AirScout Password:";
            // 
            // label_WS_Username
            // 
            this.label_WS_Username.AutoSize = true;
            this.label_WS_Username.Location = new System.Drawing.Point(10, 40);
            this.label_WS_Username.Name = "label_WS_Username";
            this.label_WS_Username.Size = new System.Drawing.Size(101, 13);
            this.label_WS_Username.TabIndex = 28;
            this.label_WS_Username.Text = "AirScout Username:";
            // 
            // tb_WS_Username
            // 
            this.tb_WS_Username.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WS_Username", true));
            this.tb_WS_Username.Location = new System.Drawing.Point(121, 37);
            this.tb_WS_Username.Name = "tb_WS_Username";
            this.tb_WS_Username.Size = new System.Drawing.Size(87, 20);
            this.tb_WS_Username.TabIndex = 27;
            this.tb_WS_Username.Text = global::wtKST.Properties.Settings.Default.WS_Username;
            // 
            // cb_WS_Active
            // 
            this.cb_WS_Active.AutoSize = true;
            this.cb_WS_Active.Checked = global::wtKST.Properties.Settings.Default.WS_Active;
            this.cb_WS_Active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_WS_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "WS_Active", true));
            this.cb_WS_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_WS_Active.Location = new System.Drawing.Point(13, 14);
            this.cb_WS_Active.Name = "cb_WS_Active";
            this.cb_WS_Active.Size = new System.Drawing.Size(297, 17);
            this.cb_WS_Active.TabIndex = 26;
            this.cb_WS_Active.Text = "Activate web service for browser based AirScout versions";
            this.cb_WS_Active.UseVisualStyleBackColor = true;
            this.cb_WS_Active.CheckedChanged += new System.EventHandler(this.cb_WS_Active_CheckedChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.textBox2);
            this.tabPage5.Controls.Add(this.label24);
            this.tabPage5.Controls.Add(this.checkBox1);
            this.tabPage5.Controls.Add(this.textBox3);
            this.tabPage5.Controls.Add(this.label25);
            this.tabPage5.Controls.Add(this.checkBox2);
            this.tabPage5.Controls.Add(this.textBox4);
            this.tabPage5.Controls.Add(this.label26);
            this.tabPage5.Controls.Add(this.checkBox3);
            this.tabPage5.Controls.Add(this.textBox5);
            this.tabPage5.Controls.Add(this.label27);
            this.tabPage5.Controls.Add(this.checkBox4);
            this.tabPage5.Controls.Add(this.tB_macro_2);
            this.tabPage5.Controls.Add(this.label23);
            this.tabPage5.Controls.Add(this.cB_macro_2);
            this.tabPage5.Controls.Add(this.tB_macro_5);
            this.tabPage5.Controls.Add(this.label22);
            this.tabPage5.Controls.Add(this.cB_macro_5);
            this.tabPage5.Controls.Add(this.tB_macro_6);
            this.tabPage5.Controls.Add(this.label21);
            this.tabPage5.Controls.Add(this.cB_macro_6);
            this.tabPage5.Controls.Add(this.tB_macro_3);
            this.tabPage5.Controls.Add(this.label20);
            this.tabPage5.Controls.Add(this.cB_macro_3);
            this.tabPage5.Controls.Add(this.tB_macro_4);
            this.tabPage5.Controls.Add(this.label19);
            this.tabPage5.Controls.Add(this.cB_macro_4);
            this.tabPage5.Controls.Add(this.tB_macro_1);
            this.tabPage5.Controls.Add(this.label18);
            this.tabPage5.Controls.Add(this.cB_macro_1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(471, 290);
            this.tabPage5.TabIndex = 3;
            this.tabPage5.Text = "KST - Macros";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_9", true));
            this.textBox2.Location = new System.Drawing.Point(90, 210);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(374, 20);
            this.textBox2.TabIndex = 36;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(18, 214);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(46, 13);
            this.label24.TabIndex = 34;
            this.label24.Text = "Macro 9";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M9", true));
            this.checkBox1.Location = new System.Drawing.Point(70, 213);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 35;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_0", true));
            this.textBox3.Location = new System.Drawing.Point(90, 235);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(374, 20);
            this.textBox3.TabIndex = 39;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(18, 239);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(52, 13);
            this.label25.TabIndex = 37;
            this.label25.Text = "Macro 10";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M0", true));
            this.checkBox2.Location = new System.Drawing.Point(70, 238);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 38;
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_7", true));
            this.textBox4.Location = new System.Drawing.Point(90, 160);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(374, 20);
            this.textBox4.TabIndex = 30;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(18, 164);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(46, 13);
            this.label26.TabIndex = 28;
            this.label26.Text = "Macro 7";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M7", true));
            this.checkBox3.Location = new System.Drawing.Point(70, 163);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 29;
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // textBox5
            // 
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_8", true));
            this.textBox5.Location = new System.Drawing.Point(90, 185);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(374, 20);
            this.textBox5.TabIndex = 33;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(18, 189);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(46, 13);
            this.label27.TabIndex = 31;
            this.label27.Text = "Macro 8";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M8", true));
            this.checkBox4.Location = new System.Drawing.Point(70, 188);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 32;
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // tB_macro_2
            // 
            this.tB_macro_2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_2", true));
            this.tB_macro_2.Location = new System.Drawing.Point(90, 35);
            this.tB_macro_2.Name = "tB_macro_2";
            this.tB_macro_2.Size = new System.Drawing.Size(374, 20);
            this.tB_macro_2.TabIndex = 15;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(18, 39);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(46, 13);
            this.label23.TabIndex = 13;
            this.label23.Text = "Macro 2";
            // 
            // cB_macro_2
            // 
            this.cB_macro_2.AutoSize = true;
            this.cB_macro_2.Checked = true;
            this.cB_macro_2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_macro_2.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M2", true));
            this.cB_macro_2.Location = new System.Drawing.Point(70, 38);
            this.cB_macro_2.Name = "cB_macro_2";
            this.cB_macro_2.Size = new System.Drawing.Size(15, 14);
            this.cB_macro_2.TabIndex = 14;
            this.cB_macro_2.UseVisualStyleBackColor = true;
            // 
            // tB_macro_5
            // 
            this.tB_macro_5.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_5", true));
            this.tB_macro_5.Location = new System.Drawing.Point(90, 110);
            this.tB_macro_5.Name = "tB_macro_5";
            this.tB_macro_5.Size = new System.Drawing.Size(374, 20);
            this.tB_macro_5.TabIndex = 24;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(18, 114);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(46, 13);
            this.label22.TabIndex = 22;
            this.label22.Text = "Macro 5";
            // 
            // cB_macro_5
            // 
            this.cB_macro_5.AutoSize = true;
            this.cB_macro_5.Checked = true;
            this.cB_macro_5.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_macro_5.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M5", true));
            this.cB_macro_5.Location = new System.Drawing.Point(70, 113);
            this.cB_macro_5.Name = "cB_macro_5";
            this.cB_macro_5.Size = new System.Drawing.Size(15, 14);
            this.cB_macro_5.TabIndex = 23;
            this.cB_macro_5.UseVisualStyleBackColor = true;
            // 
            // tB_macro_6
            // 
            this.tB_macro_6.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_6", true));
            this.tB_macro_6.Location = new System.Drawing.Point(90, 135);
            this.tB_macro_6.Name = "tB_macro_6";
            this.tB_macro_6.Size = new System.Drawing.Size(374, 20);
            this.tB_macro_6.TabIndex = 27;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(18, 139);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(46, 13);
            this.label21.TabIndex = 25;
            this.label21.Text = "Macro 6";
            // 
            // cB_macro_6
            // 
            this.cB_macro_6.AutoSize = true;
            this.cB_macro_6.Checked = true;
            this.cB_macro_6.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_macro_6.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M6", true));
            this.cB_macro_6.Location = new System.Drawing.Point(70, 138);
            this.cB_macro_6.Name = "cB_macro_6";
            this.cB_macro_6.Size = new System.Drawing.Size(15, 14);
            this.cB_macro_6.TabIndex = 26;
            this.cB_macro_6.UseVisualStyleBackColor = true;
            // 
            // tB_macro_3
            // 
            this.tB_macro_3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_3", true));
            this.tB_macro_3.Location = new System.Drawing.Point(90, 60);
            this.tB_macro_3.Name = "tB_macro_3";
            this.tB_macro_3.Size = new System.Drawing.Size(374, 20);
            this.tB_macro_3.TabIndex = 18;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(18, 64);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(46, 13);
            this.label20.TabIndex = 16;
            this.label20.Text = "Macro 3";
            // 
            // cB_macro_3
            // 
            this.cB_macro_3.AutoSize = true;
            this.cB_macro_3.Checked = true;
            this.cB_macro_3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_macro_3.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M3", true));
            this.cB_macro_3.Location = new System.Drawing.Point(70, 63);
            this.cB_macro_3.Name = "cB_macro_3";
            this.cB_macro_3.Size = new System.Drawing.Size(15, 14);
            this.cB_macro_3.TabIndex = 17;
            this.cB_macro_3.UseVisualStyleBackColor = true;
            // 
            // tB_macro_4
            // 
            this.tB_macro_4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_4", true));
            this.tB_macro_4.Location = new System.Drawing.Point(90, 85);
            this.tB_macro_4.Name = "tB_macro_4";
            this.tB_macro_4.Size = new System.Drawing.Size(374, 20);
            this.tB_macro_4.TabIndex = 21;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(18, 89);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(46, 13);
            this.label19.TabIndex = 19;
            this.label19.Text = "Macro 4";
            // 
            // cB_macro_4
            // 
            this.cB_macro_4.AutoSize = true;
            this.cB_macro_4.Checked = true;
            this.cB_macro_4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_macro_4.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M4", true));
            this.cB_macro_4.Location = new System.Drawing.Point(70, 88);
            this.cB_macro_4.Name = "cB_macro_4";
            this.cB_macro_4.Size = new System.Drawing.Size(15, 14);
            this.cB_macro_4.TabIndex = 20;
            this.cB_macro_4.UseVisualStyleBackColor = true;
            // 
            // tB_macro_1
            // 
            this.tB_macro_1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Macro_1", true));
            this.tB_macro_1.Location = new System.Drawing.Point(90, 10);
            this.tB_macro_1.Name = "tB_macro_1";
            this.tB_macro_1.Size = new System.Drawing.Size(374, 20);
            this.tB_macro_1.TabIndex = 12;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(18, 14);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(46, 13);
            this.label18.TabIndex = 10;
            this.label18.Text = "Macro 1";
            // 
            // cB_macro_1
            // 
            this.cB_macro_1.AutoSize = true;
            this.cB_macro_1.Checked = true;
            this.cB_macro_1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_macro_1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_M1", true));
            this.cB_macro_1.Location = new System.Drawing.Point(70, 13);
            this.cB_macro_1.Name = "cB_macro_1";
            this.cB_macro_1.Size = new System.Drawing.Size(15, 14);
            this.cB_macro_1.TabIndex = 11;
            this.cB_macro_1.UseVisualStyleBackColor = true;
            // 
            // tb_Options_DXLog_Station_Name
            // 
            this.tb_Options_DXLog_Station_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "DXLog_StationName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_DXLog_Station_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_DXLog_Station_Name.Location = new System.Drawing.Point(112, 35);
            this.tb_Options_DXLog_Station_Name.Name = "tb_Options_DXLog_Station_Name";
            this.tb_Options_DXLog_Station_Name.Size = new System.Drawing.Size(204, 20);
            this.tb_Options_DXLog_Station_Name.TabIndex = 15;
            this.tb_Options_DXLog_Station_Name.Text = global::wtKST.Properties.Settings.Default.DXLog_StationName;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(5, 42);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(74, 13);
            this.label28.TabIndex = 14;
            this.label28.Text = "Station Name:";
            // 
            // OptionsDlg
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(617, 394);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.Bands.ResumeLayout(false);
            this.Bands.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_AS.ResumeLayout(false);
            this.panel_AS.PerformLayout();
            this.panel_WS.ResumeLayout(false);
            this.panel_WS.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void rb_KST_StartAsAwayHere_Click(object sender, EventArgs e)
        {
            // work around an issue with using user settings and data binding
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Checked == false)
                radioButton.Checked = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.ValidateChildren();
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Settings.Default.Reload();
            this.Close();
        }

        private void cb_AS_local_CheckedChanged(object sender, EventArgs e)
        {
            this.tb_Options_AS_Local_Server_Name.Enabled = this.cb_AS_local.Checked;
            this.label_local_server.Enabled = this.cb_AS_local.Checked;
        }

        private void cb_AS_local_EnableChanged(object sender, EventArgs e)
        {
            this.tb_Options_AS_Local_Server_Name.Enabled = this.cb_AS_local.Checked;
            this.label_local_server.Enabled = this.cb_AS_local.Checked;
        }

        private void cb_WinTest_Active_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_WinTest_Active.Checked)
            {
                this.cb_WinTestNet_Active.Checked = false;
                this.cb_WinTestNet_Active.Enabled = false;
                this.cb_QARTest_Active.Checked = false;
                this.cb_QARTest_Active.Enabled = false;
                this.cb_DXLog_Active.Checked = false;
                this.cb_DXLog_Active.Enabled = false;
            }
            else
            {
                this.cb_WinTest_Active.Enabled = true;
                this.cb_WinTestNet_Active.Enabled = true;
                this.cb_QARTest_Active.Enabled = true;
                this.cb_DXLog_Active.Enabled = true;
            }
        }
        private void cb_WinTestNet_Active_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_WinTestNet_Active.Checked)
            {
                this.cb_WinTest_Active.Checked = false;
                this.cb_WinTest_Active.Enabled = false;
                this.cb_QARTest_Active.Checked = false;
                this.cb_QARTest_Active.Enabled = false;
                this.cb_DXLog_Active.Checked = false;
                this.cb_DXLog_Active.Enabled = false;
            }
            else
            {
                this.cb_WinTest_Active.Enabled = true;
                this.cb_WinTestNet_Active.Enabled = true;
                this.cb_QARTest_Active.Enabled = true;
                this.cb_DXLog_Active.Enabled = true;
            }
        }
        private void cb_QARTest_Active_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_QARTest_Active.Checked)
            {
                this.cb_WinTest_Active.Checked = false;
                this.cb_WinTest_Active.Enabled = false;
                this.cb_WinTestNet_Active.Checked = false;
                this.cb_WinTestNet_Active.Enabled = false;
                this.cb_DXLog_Active.Checked = false;
                this.cb_DXLog_Active.Enabled = false;
            }
            else
            {
                this.cb_WinTest_Active.Enabled = true;
                this.cb_WinTestNet_Active.Enabled = true;
                this.cb_QARTest_Active.Enabled = true;
                this.cb_DXLog_Active.Enabled = true;
            }
        }

        private void cb_DXLog_Active_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cb_DXLog_Active.Checked)
            {
                this.cb_WinTest_Active.Checked = false;
                this.cb_WinTest_Active.Enabled = false;
                this.cb_WinTestNet_Active.Checked = false;
                this.cb_WinTestNet_Active.Enabled = false;
                this.cb_QARTest_Active.Checked = false;
                this.cb_QARTest_Active.Enabled = false;
            }
            else
            {
                this.cb_WinTest_Active.Enabled = true;
                this.cb_WinTestNet_Active.Enabled = true;
                this.cb_QARTest_Active.Enabled = true;
                this.cb_DXLog_Active.Enabled = true;
            }
        }

        private bool mCBsASCheckedChangedTriggered = false;
        private bool mCBsWSCheckedChangedTriggered = false;

        private void CBHandleWSActive()
        {
            if (this.cb_WS_Active.Checked)
            {
                this.panel_AS.Enabled = false;
                this.cb_AS_Active.Checked = false;
                this.cb_AS_Active.Enabled = false;
                mCBsASCheckedChangedTriggered = true;

                foreach (Control c in this.panel_WS.Controls)
                {
                    c.Enabled = true;
                }
            }
            else
            {
                mCBsASCheckedChangedTriggered = false;
                disable_AS_panels_enable_cbs();
            }
        }

        private void cb_WS_Active_CheckedChanged(object sender, EventArgs e)
        {
            if (mCBsWSCheckedChangedTriggered)
            {
                mCBsWSCheckedChangedTriggered = false;
                return;
            }
            CBHandleWSActive();
        }

        private void CBHandleASActive()
        {
            if (this.cb_AS_Active.Checked)
            {
                this.panel_WS.Enabled = false;
                this.cb_WS_Active.Checked = false;
                this.cb_WS_Active.Enabled = false;
                mCBsWSCheckedChangedTriggered = true;
                foreach (Control c in this.panel_AS.Controls)
                {
                    c.Enabled = true;
                }
            }
            else
            {
                mCBsWSCheckedChangedTriggered = false;
                disable_AS_panels_enable_cbs();
            }
        }

        private void cb_AS_Active_CheckedChanged(object sender, EventArgs e)
        {
            if (mCBsASCheckedChangedTriggered)
            {
                mCBsASCheckedChangedTriggered = false;
                return;
            }
            CBHandleASActive();
        }

        private void disable_AS_panels_enable_cbs()
        {
            this.panel_WS.Enabled = true;
            this.panel_AS.Enabled = true;
            // we cannot disable the panel - otherwise we could not enable cb_AS_Active separately
            foreach (Control c in this.panel_AS.Controls)
            {
                if (c != cb_AS_Active)
                    c.Enabled = false;
            }
            this.cb_AS_Active.Enabled = true;
            foreach (Control c in this.panel_WS.Controls)
            {
                if (c != cb_WS_Active)
                    c.Enabled = false;
            }
            this.cb_WS_Active.Enabled = true;
        }
    }
}
