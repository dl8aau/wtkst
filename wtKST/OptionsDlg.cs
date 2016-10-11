using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using wtKST.Properties;

namespace wtKST
{
    public class OptionsDlg : Form
    {
        private IContainer components = null;

        private Button btn_OK;

        private Button btn_Cancel;

        private Button btn_Options_WinTest_QRV_Select;

        public TextBox tb_Options_WinTest_QRV;

        private Label label10;

        private Button btn_Options_WinTest_INI_Select;

        public TextBox tb_Options_WinTest_INI;

        private Label label6;

        private GroupBox groupBox3;

        public TextBox tb_Options_UpdateInterval;

        private Label label7;

        private GroupBox groupBox5;

        public CheckBox cb_KST_ShowBalloon;

        public CheckBox cb_ShowBeacons;
        private Label label1;
        public TextBox tb_KST_ServerName;
        private Label label2;
        public TextBox tb_KST_ServerPort;
        private Label label3;
        public TextBox tb_KST_UserName;
        private Label label4;
        public TextBox tb_KST_Password;
        public ComboBox cbb_KST_Chat;
        private Label label5;
        public CheckBox cb_KST_AutoConnect;
        public RadioButton rb_KST_StartAsHere;
        public RadioButton rb_KST_StartAsAway;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private Label label16;
        private ComboBox cb_AS_QRG;
        private TextBox tb_Options_AS_My_Name;
        private Label label15;
        private TextBox tb_Options_AS_Server_Name;
        private Label label14;
        private Label label13;
        private TextBox tb_AS_Timeout;
        private Label label12;
        private Label label11;
        private TextBox tb_AS_MaxDist;
        private Label label9;
        private TextBox tb_AS_MinDist;
        private Label label8;
        public CheckBox cb_AS_Active;
        private GroupBox Bands;
        private CheckBox checkBox1296;
        private CheckBox checkBox432;
        private CheckBox checkBox144;
        private CheckBox checkBox10GHz;
        private CheckBox checkBox24GHz;
        private CheckBox checkBox47GHz;
        private CheckBox checkBox2320;
        private CheckBox checkBox3400;
        private CheckBox checkBox5760;
        private CheckBox checkBox76GHz;
        public TextBox tb_KST_MaxDist;
        private Label label17;
        public CheckBox cb_WinTest_Active;

        public OptionsDlg()
        {
            this.InitializeComponent();
        }

        private void btn_Options_WinTest_QRV_Select_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Filter = "Win-Test Extradatenbanken | *.xdt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.WinTest_QRV_FileName = ofd.FileName;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
        }

        private void btn_Options_WinTest_INI_Select_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Multiselect = false;
            ofd.Filter = "Win-Test Einstellung | *.ini";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Settings.Default.WinTest_INI_FileName = ofd.FileName;
                Settings.Default.Save();
                Settings.Default.Reload();
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
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDlg));
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Options_WinTest_INI_Select = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_Options_WinTest_QRV_Select = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_ShowBeacons = new System.Windows.Forms.CheckBox();
            this.tb_Options_UpdateInterval = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cb_KST_ShowBalloon = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rb_KST_StartAsAway = new System.Windows.Forms.RadioButton();
            this.tb_KST_ServerPort = new System.Windows.Forms.TextBox();
            this.rb_KST_StartAsHere = new System.Windows.Forms.RadioButton();
            this.cb_KST_AutoConnect = new System.Windows.Forms.CheckBox();
            this.tb_KST_ServerName = new System.Windows.Forms.TextBox();
            this.cbb_KST_Chat = new System.Windows.Forms.ComboBox();
            this.tb_KST_Password = new System.Windows.Forms.TextBox();
            this.tb_KST_UserName = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cb_WinTest_Active = new System.Windows.Forms.CheckBox();
            this.tb_Options_WinTest_INI = new System.Windows.Forms.TextBox();
            this.tb_Options_WinTest_QRV = new System.Windows.Forms.TextBox();
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
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.cb_AS_QRG = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cb_AS_Active = new System.Windows.Forms.CheckBox();
            this.tb_Options_AS_My_Name = new System.Windows.Forms.TextBox();
            this.tb_AS_MinDist = new System.Windows.Forms.TextBox();
            this.tb_Options_AS_Server_Name = new System.Windows.Forms.TextBox();
            this.tb_AS_MaxDist = new System.Windows.Forms.TextBox();
            this.tb_AS_Timeout = new System.Windows.Forms.TextBox();
            this.tb_KST_MaxDist = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.Bands.SuspendLayout();
            this.tabPage4.SuspendLayout();
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
            // 
            // btn_Options_WinTest_INI_Select
            // 
            this.btn_Options_WinTest_INI_Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Options_WinTest_INI_Select.Location = new System.Drawing.Point(345, 55);
            this.btn_Options_WinTest_INI_Select.Name = "btn_Options_WinTest_INI_Select";
            this.btn_Options_WinTest_INI_Select.Size = new System.Drawing.Size(75, 23);
            this.btn_Options_WinTest_INI_Select.TabIndex = 13;
            this.btn_Options_WinTest_INI_Select.Text = "Select";
            this.btn_Options_WinTest_INI_Select.UseVisualStyleBackColor = true;
            this.btn_Options_WinTest_INI_Select.Click += new System.EventHandler(this.btn_Options_WinTest_INI_Select_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "wt.ini :";
            // 
            // btn_Options_WinTest_QRV_Select
            // 
            this.btn_Options_WinTest_QRV_Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Options_WinTest_QRV_Select.Location = new System.Drawing.Point(345, 81);
            this.btn_Options_WinTest_QRV_Select.Name = "btn_Options_WinTest_QRV_Select";
            this.btn_Options_WinTest_QRV_Select.Size = new System.Drawing.Size(75, 23);
            this.btn_Options_WinTest_QRV_Select.TabIndex = 10;
            this.btn_Options_WinTest_QRV_Select.Text = "Select";
            this.btn_Options_WinTest_QRV_Select.UseVisualStyleBackColor = true;
            this.btn_Options_WinTest_QRV_Select.Click += new System.EventHandler(this.btn_Options_WinTest_QRV_Select_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(21, 86);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "qrv.xdt :";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_KST_MaxDist);
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.cb_ShowBeacons);
            this.groupBox3.Controls.Add(this.tb_Options_UpdateInterval);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(9, 19);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(445, 62);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Calls";
            // 
            // cb_ShowBeacons
            // 
            this.cb_ShowBeacons.AutoSize = true;
            this.cb_ShowBeacons.Checked = global::wtKST.Properties.Settings.Default.ShowBeacons;
            this.cb_ShowBeacons.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "ShowBeacons", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_ShowBeacons.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_ShowBeacons.Location = new System.Drawing.Point(163, 32);
            this.cb_ShowBeacons.Name = "cb_ShowBeacons";
            this.cb_ShowBeacons.Size = new System.Drawing.Size(98, 17);
            this.cb_ShowBeacons.TabIndex = 13;
            this.cb_ShowBeacons.Text = "Show Beacons";
            this.cb_ShowBeacons.UseVisualStyleBackColor = true;
            // 
            // tb_Options_UpdateInterval
            // 
            this.tb_Options_UpdateInterval.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "UpdateInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_UpdateInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_UpdateInterval.Location = new System.Drawing.Point(111, 29);
            this.tb_Options_UpdateInterval.Name = "tb_Options_UpdateInterval";
            this.tb_Options_UpdateInterval.Size = new System.Drawing.Size(33, 20);
            this.tb_Options_UpdateInterval.TabIndex = 12;
            this.tb_Options_UpdateInterval.Text = global::wtKST.Properties.Settings.Default.UpdateInterval;
            this.tb_Options_UpdateInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_Options_UpdateInterval_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(6, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Update interval [s] :";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cb_KST_ShowBalloon);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(9, 215);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(445, 62);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Notifications";
            // 
            // cb_KST_ShowBalloon
            // 
            this.cb_KST_ShowBalloon.AutoSize = true;
            this.cb_KST_ShowBalloon.Checked = global::wtKST.Properties.Settings.Default.KST_ShowBalloon;
            this.cb_KST_ShowBalloon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_KST_ShowBalloon.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_ShowBalloon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_KST_ShowBalloon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_KST_ShowBalloon.Location = new System.Drawing.Point(9, 29);
            this.cb_KST_ShowBalloon.Name = "cb_KST_ShowBalloon";
            this.cb_KST_ShowBalloon.Size = new System.Drawing.Size(241, 17);
            this.cb_KST_ShowBalloon.TabIndex = 11;
            this.cb_KST_ShowBalloon.Text = "On new MyMessages show ballon notification";
            this.cb_KST_ShowBalloon.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 124);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Chat :";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(479, 316);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rb_KST_StartAsAway);
            this.tabPage1.Controls.Add(this.tb_KST_ServerPort);
            this.tabPage1.Controls.Add(this.rb_KST_StartAsHere);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.cb_KST_AutoConnect);
            this.tabPage1.Controls.Add(this.tb_KST_ServerName);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label2);
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
            // rb_KST_StartAsAway
            // 
            this.rb_KST_StartAsAway.AutoSize = true;
            this.rb_KST_StartAsAway.Checked = global::wtKST.Properties.Settings.Default.KST_StartAsAway;
            this.rb_KST_StartAsAway.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_StartAsAway", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rb_KST_StartAsAway.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb_KST_StartAsAway.Location = new System.Drawing.Point(275, 101);
            this.rb_KST_StartAsAway.Name = "rb_KST_StartAsAway";
            this.rb_KST_StartAsAway.Size = new System.Drawing.Size(96, 17);
            this.rb_KST_StartAsAway.TabIndex = 12;
            this.rb_KST_StartAsAway.TabStop = true;
            this.rb_KST_StartAsAway.Text = "Start as AWAY";
            this.rb_KST_StartAsAway.UseVisualStyleBackColor = true;
            this.rb_KST_StartAsAway.Click += new System.EventHandler(this.rb_KST_StartAsAwayHere_Click);
            // 
            // tb_KST_ServerPort
            // 
            this.tb_KST_ServerPort.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_ServerPort", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_ServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_ServerPort.Location = new System.Drawing.Point(120, 44);
            this.tb_KST_ServerPort.Name = "tb_KST_ServerPort";
            this.tb_KST_ServerPort.Size = new System.Drawing.Size(56, 20);
            this.tb_KST_ServerPort.TabIndex = 3;
            this.tb_KST_ServerPort.Text = global::wtKST.Properties.Settings.Default.KST_ServerPort;
            // 
            // rb_KST_StartAsHere
            // 
            this.rb_KST_StartAsHere.AutoSize = true;
            this.rb_KST_StartAsHere.Checked = global::wtKST.Properties.Settings.Default.KST_StartAsHere;
            this.rb_KST_StartAsHere.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_StartAsHere", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.rb_KST_StartAsHere.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb_KST_StartAsHere.Location = new System.Drawing.Point(275, 70);
            this.rb_KST_StartAsHere.Name = "rb_KST_StartAsHere";
            this.rb_KST_StartAsHere.Size = new System.Drawing.Size(94, 17);
            this.rb_KST_StartAsHere.TabIndex = 11;
            this.rb_KST_StartAsHere.Text = "Start as HERE";
            this.rb_KST_StartAsHere.UseVisualStyleBackColor = true;
            this.rb_KST_StartAsHere.Click += new System.EventHandler(this.rb_KST_StartAsAwayHere_Click);
            // 
            // cb_KST_AutoConnect
            // 
            this.cb_KST_AutoConnect.AutoSize = true;
            this.cb_KST_AutoConnect.Checked = global::wtKST.Properties.Settings.Default.KST_AutoConnect;
            this.cb_KST_AutoConnect.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_AutoConnect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_KST_AutoConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_KST_AutoConnect.Location = new System.Drawing.Point(277, 21);
            this.cb_KST_AutoConnect.Name = "cb_KST_AutoConnect";
            this.cb_KST_AutoConnect.Size = new System.Drawing.Size(90, 17);
            this.cb_KST_AutoConnect.TabIndex = 10;
            this.cb_KST_AutoConnect.Text = "Auto connect";
            this.cb_KST_AutoConnect.UseVisualStyleBackColor = true;
            // 
            // tb_KST_ServerName
            // 
            this.tb_KST_ServerName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_ServerName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_ServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_ServerName.Location = new System.Drawing.Point(120, 18);
            this.tb_KST_ServerName.Name = "tb_KST_ServerName";
            this.tb_KST_ServerName.Size = new System.Drawing.Size(123, 20);
            this.tb_KST_ServerName.TabIndex = 1;
            this.tb_KST_ServerName.Text = global::wtKST.Properties.Settings.Default.KST_ServerName;
            // 
            // cbb_KST_Chat
            // 
            this.cbb_KST_Chat.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Chat", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cbb_KST_Chat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbb_KST_Chat.FormattingEnabled = true;
            this.cbb_KST_Chat.Items.AddRange(new object[] {
            "1 - 50/70 MHz",
            "2 - 144/432 MHz",
            "3 - Microwave",
            "4 - EME/JT65",
            "5 - Low Band"});
            this.cbb_KST_Chat.Location = new System.Drawing.Point(120, 124);
            this.cbb_KST_Chat.Name = "cbb_KST_Chat";
            this.cbb_KST_Chat.Size = new System.Drawing.Size(121, 21);
            this.cbb_KST_Chat.TabIndex = 8;
            this.cbb_KST_Chat.Text = global::wtKST.Properties.Settings.Default.KST_Chat;
            // 
            // tb_KST_Password
            // 
            this.tb_KST_Password.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_Password.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_Password.Location = new System.Drawing.Point(120, 98);
            this.tb_KST_Password.Name = "tb_KST_Password";
            this.tb_KST_Password.PasswordChar = '*';
            this.tb_KST_Password.Size = new System.Drawing.Size(123, 20);
            this.tb_KST_Password.TabIndex = 7;
            this.tb_KST_Password.Text = global::wtKST.Properties.Settings.Default.KST_Password;
            // 
            // tb_KST_UserName
            // 
            this.tb_KST_UserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_UserName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_UserName.Location = new System.Drawing.Point(120, 71);
            this.tb_KST_UserName.Name = "tb_KST_UserName";
            this.tb_KST_UserName.Size = new System.Drawing.Size(123, 20);
            this.tb_KST_UserName.TabIndex = 5;
            this.tb_KST_UserName.Text = global::wtKST.Properties.Settings.Default.KST_UserName;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cb_WinTest_Active);
            this.tabPage2.Controls.Add(this.btn_Options_WinTest_QRV_Select);
            this.tabPage2.Controls.Add(this.btn_Options_WinTest_INI_Select);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.tb_Options_WinTest_INI);
            this.tabPage2.Controls.Add(this.tb_Options_WinTest_QRV);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(471, 290);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Win-Test";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cb_WinTest_Active
            // 
            this.cb_WinTest_Active.AutoSize = true;
            this.cb_WinTest_Active.Checked = global::wtKST.Properties.Settings.Default.WinTest_Activate;
            this.cb_WinTest_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "WinTest_Activate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_WinTest_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_WinTest_Active.Location = new System.Drawing.Point(24, 29);
            this.cb_WinTest_Active.Name = "cb_WinTest_Active";
            this.cb_WinTest_Active.Size = new System.Drawing.Size(297, 17);
            this.cb_WinTest_Active.TabIndex = 14;
            this.cb_WinTest_Active.Text = "Activate (only DL0GTH crew members, not for public use)";
            this.cb_WinTest_Active.UseVisualStyleBackColor = true;
            // 
            // tb_Options_WinTest_INI
            // 
            this.tb_Options_WinTest_INI.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WinTest_INI_Filename", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_WinTest_INI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_WinTest_INI.Location = new System.Drawing.Point(126, 57);
            this.tb_Options_WinTest_INI.Name = "tb_Options_WinTest_INI";
            this.tb_Options_WinTest_INI.Size = new System.Drawing.Size(204, 20);
            this.tb_Options_WinTest_INI.TabIndex = 12;
            this.tb_Options_WinTest_INI.Text = global::wtKST.Properties.Settings.Default.WinTest_INI_FileName;
            // 
            // tb_Options_WinTest_QRV
            // 
            this.tb_Options_WinTest_QRV.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WinTest_QRV_Filename", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_WinTest_QRV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_WinTest_QRV.Location = new System.Drawing.Point(126, 83);
            this.tb_Options_WinTest_QRV.Name = "tb_Options_WinTest_QRV";
            this.tb_Options_WinTest_QRV.Size = new System.Drawing.Size(204, 20);
            this.tb_Options_WinTest_QRV.TabIndex = 1;
            this.tb_Options_WinTest_QRV.Text = global::wtKST.Properties.Settings.Default.WinTest_QRV_FileName;
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
            this.Bands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Bands.Location = new System.Drawing.Point(9, 87);
            this.Bands.Name = "Bands";
            this.Bands.Size = new System.Drawing.Size(445, 122);
            this.Bands.TabIndex = 13;
            this.Bands.TabStop = false;
            this.Bands.Text = "Bands";
            // 
            // checkBox76GHz
            // 
            this.checkBox76GHz.AutoSize = true;
            this.checkBox76GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox76GHz.Checked = global::wtKST.Properties.Settings.Default.Band_76GHz;
            this.checkBox76GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox76GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_76GHz", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox76GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox76GHz.Location = new System.Drawing.Point(46, 96);
            this.checkBox76GHz.Name = "checkBox76GHz";
            this.checkBox76GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox76GHz.TabIndex = 10;
            this.checkBox76GHz.Text = "76GHz";
            this.checkBox76GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox10GHz
            // 
            this.checkBox10GHz.AutoSize = true;
            this.checkBox10GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox10GHz.Checked = global::wtKST.Properties.Settings.Default.Band_10368;
            this.checkBox10GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox10GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_10368", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox10GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox10GHz.Location = new System.Drawing.Point(46, 73);
            this.checkBox10GHz.Name = "checkBox10GHz";
            this.checkBox10GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox10GHz.TabIndex = 7;
            this.checkBox10GHz.Text = "10GHz";
            this.checkBox10GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox24GHz
            // 
            this.checkBox24GHz.AutoSize = true;
            this.checkBox24GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox24GHz.Checked = global::wtKST.Properties.Settings.Default.Band_24GHz;
            this.checkBox24GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox24GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_24GHz", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox24GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox24GHz.Location = new System.Drawing.Point(150, 73);
            this.checkBox24GHz.Name = "checkBox24GHz";
            this.checkBox24GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox24GHz.TabIndex = 8;
            this.checkBox24GHz.Text = "24GHz";
            this.checkBox24GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox47GHz
            // 
            this.checkBox47GHz.AutoSize = true;
            this.checkBox47GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox47GHz.Checked = global::wtKST.Properties.Settings.Default.Band_47GHz;
            this.checkBox47GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox47GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_47GHz", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox47GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox47GHz.Location = new System.Drawing.Point(263, 73);
            this.checkBox47GHz.Name = "checkBox47GHz";
            this.checkBox47GHz.Size = new System.Drawing.Size(59, 17);
            this.checkBox47GHz.TabIndex = 9;
            this.checkBox47GHz.Text = "47GHz";
            this.checkBox47GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox2320
            // 
            this.checkBox2320.AutoSize = true;
            this.checkBox2320.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox2320.Checked = global::wtKST.Properties.Settings.Default.Band_2320;
            this.checkBox2320.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2320.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_2320", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox2320.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2320.Location = new System.Drawing.Point(33, 50);
            this.checkBox2320.Name = "checkBox2320";
            this.checkBox2320.Size = new System.Drawing.Size(72, 17);
            this.checkBox2320.TabIndex = 4;
            this.checkBox2320.Text = "2320MHz";
            this.checkBox2320.UseVisualStyleBackColor = true;
            // 
            // checkBox3400
            // 
            this.checkBox3400.AutoSize = true;
            this.checkBox3400.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox3400.Checked = global::wtKST.Properties.Settings.Default.Band_3400;
            this.checkBox3400.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3400.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_3400", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox3400.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3400.Location = new System.Drawing.Point(137, 50);
            this.checkBox3400.Name = "checkBox3400";
            this.checkBox3400.Size = new System.Drawing.Size(72, 17);
            this.checkBox3400.TabIndex = 5;
            this.checkBox3400.Text = "3400MHz";
            this.checkBox3400.UseVisualStyleBackColor = true;
            // 
            // checkBox5760
            // 
            this.checkBox5760.AutoSize = true;
            this.checkBox5760.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox5760.Checked = global::wtKST.Properties.Settings.Default.Band_5760;
            this.checkBox5760.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox5760.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_5760", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox5760.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox5760.Location = new System.Drawing.Point(250, 50);
            this.checkBox5760.Name = "checkBox5760";
            this.checkBox5760.Size = new System.Drawing.Size(72, 17);
            this.checkBox5760.TabIndex = 6;
            this.checkBox5760.Text = "5760MHz";
            this.checkBox5760.UseVisualStyleBackColor = true;
            // 
            // checkBox1296
            // 
            this.checkBox1296.AutoSize = true;
            this.checkBox1296.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1296.Checked = global::wtKST.Properties.Settings.Default.Band_1296;
            this.checkBox1296.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1296.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_1296", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1296.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1296.Location = new System.Drawing.Point(250, 27);
            this.checkBox1296.Name = "checkBox1296";
            this.checkBox1296.Size = new System.Drawing.Size(72, 17);
            this.checkBox1296.TabIndex = 3;
            this.checkBox1296.Text = "1296MHz";
            this.checkBox1296.UseVisualStyleBackColor = true;
            // 
            // checkBox432
            // 
            this.checkBox432.AutoSize = true;
            this.checkBox432.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox432.Checked = global::wtKST.Properties.Settings.Default.Band_432;
            this.checkBox432.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox432.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_432", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox432.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox432.Location = new System.Drawing.Point(143, 27);
            this.checkBox432.Name = "checkBox432";
            this.checkBox432.Size = new System.Drawing.Size(66, 17);
            this.checkBox432.TabIndex = 2;
            this.checkBox432.Text = "432MHz";
            this.checkBox432.UseVisualStyleBackColor = true;
            // 
            // checkBox144
            // 
            this.checkBox144.AutoSize = true;
            this.checkBox144.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox144.Checked = global::wtKST.Properties.Settings.Default.Band_144;
            this.checkBox144.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox144.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_144", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox144.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox144.Location = new System.Drawing.Point(39, 27);
            this.checkBox144.Name = "checkBox144";
            this.checkBox144.Size = new System.Drawing.Size(66, 17);
            this.checkBox144.TabIndex = 1;
            this.checkBox144.Text = "144MHz";
            this.checkBox144.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label16);
            this.tabPage4.Controls.Add(this.cb_AS_QRG);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.label15);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.label13);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.cb_AS_Active);
            this.tabPage4.Controls.Add(this.tb_Options_AS_My_Name);
            this.tabPage4.Controls.Add(this.tb_AS_MinDist);
            this.tabPage4.Controls.Add(this.tb_Options_AS_Server_Name);
            this.tabPage4.Controls.Add(this.tb_AS_MaxDist);
            this.tabPage4.Controls.Add(this.tb_AS_Timeout);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(471, 290);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Airplane Scatter";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(299, 67);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(60, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "Frequency:";
            // 
            // cb_AS_QRG
            // 
            this.cb_AS_QRG.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_QRG", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_AS_QRG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_AS_QRG.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AS_QRG.FormattingEnabled = true;
            this.cb_AS_QRG.Items.AddRange(new object[] {
            "144M",
            "432M",
            "1.2G",
            "2.3G",
            "3.4G",
            "5.7G",
            "10G"});
            this.cb_AS_QRG.Location = new System.Drawing.Point(368, 63);
            this.cb_AS_QRG.Name = "cb_AS_QRG";
            this.cb_AS_QRG.Size = new System.Drawing.Size(78, 21);
            this.cb_AS_QRG.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 150);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(224, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Get aircraft positions in the range of minumum:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(158, 67);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 13);
            this.label15.TabIndex = 22;
            this.label15.Text = "My Name:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(273, 150);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(102, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "kms up to maximum:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(15, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(70, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "Server name:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(199, 101);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(27, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "sec.";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(417, 150);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "kms.";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(13, 101);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(141, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Timeout for server response:";
            // 
            // cb_AS_Active
            // 
            this.cb_AS_Active.AutoSize = true;
            this.cb_AS_Active.Checked = global::wtKST.Properties.Settings.Default.AS_Active;
            this.cb_AS_Active.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_AS_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "AS_Active", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cb_AS_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_AS_Active.Location = new System.Drawing.Point(16, 29);
            this.cb_AS_Active.Name = "cb_AS_Active";
            this.cb_AS_Active.Size = new System.Drawing.Size(366, 17);
            this.cb_AS_Active.TabIndex = 11;
            this.cb_AS_Active.Text = "Activate (Requires AirScout server functionionaltiy newer than V.0.9.9.5)";
            this.cb_AS_Active.UseVisualStyleBackColor = true;
            // 
            // tb_Options_AS_My_Name
            // 
            this.tb_Options_AS_My_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_My_Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_AS_My_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_AS_My_Name.Location = new System.Drawing.Point(217, 64);
            this.tb_Options_AS_My_Name.Name = "tb_Options_AS_My_Name";
            this.tb_Options_AS_My_Name.Size = new System.Drawing.Size(66, 20);
            this.tb_Options_AS_My_Name.TabIndex = 23;
            this.tb_Options_AS_My_Name.Text = global::wtKST.Properties.Settings.Default.AS_My_Name;
            // 
            // tb_AS_MinDist
            // 
            this.tb_AS_MinDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_MinDist", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_AS_MinDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AS_MinDist.Location = new System.Drawing.Point(234, 147);
            this.tb_AS_MinDist.Name = "tb_AS_MinDist";
            this.tb_AS_MinDist.Size = new System.Drawing.Size(33, 20);
            this.tb_AS_MinDist.TabIndex = 13;
            this.tb_AS_MinDist.Text = global::wtKST.Properties.Settings.Default.AS_MinDist;
            // 
            // tb_Options_AS_Server_Name
            // 
            this.tb_Options_AS_Server_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Server_Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_AS_Server_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_Options_AS_Server_Name.Location = new System.Drawing.Point(89, 64);
            this.tb_Options_AS_Server_Name.Name = "tb_Options_AS_Server_Name";
            this.tb_Options_AS_Server_Name.Size = new System.Drawing.Size(51, 20);
            this.tb_Options_AS_Server_Name.TabIndex = 21;
            this.tb_Options_AS_Server_Name.Text = global::wtKST.Properties.Settings.Default.AS_Server_Name;
            // 
            // tb_AS_MaxDist
            // 
            this.tb_AS_MaxDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_MaxDist", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_AS_MaxDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AS_MaxDist.Location = new System.Drawing.Point(378, 147);
            this.tb_AS_MaxDist.Name = "tb_AS_MaxDist";
            this.tb_AS_MaxDist.Size = new System.Drawing.Size(33, 20);
            this.tb_AS_MaxDist.TabIndex = 15;
            this.tb_AS_MaxDist.Text = global::wtKST.Properties.Settings.Default.AS_MaxDist;
            // 
            // tb_AS_Timeout
            // 
            this.tb_AS_Timeout.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Timeout", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_AS_Timeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AS_Timeout.Location = new System.Drawing.Point(160, 98);
            this.tb_AS_Timeout.Name = "tb_AS_Timeout";
            this.tb_AS_Timeout.Size = new System.Drawing.Size(33, 20);
            this.tb_AS_Timeout.TabIndex = 18;
            this.tb_AS_Timeout.Text = global::wtKST.Properties.Settings.Default.AS_Timeout;
            // 
            // tb_KST_MaxDist
            // 
            this.tb_KST_MaxDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_MaxDist", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_MaxDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_KST_MaxDist.Location = new System.Drawing.Point(384, 29);
            this.tb_KST_MaxDist.Name = "tb_KST_MaxDist";
            this.tb_KST_MaxDist.Size = new System.Drawing.Size(45, 20);
            this.tb_KST_MaxDist.TabIndex = 15;
            this.tb_KST_MaxDist.Text = global::wtKST.Properties.Settings.Default.KST_MaxDist;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(279, 32);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(101, 13);
            this.label17.TabIndex = 14;
            this.label17.Text = "Max. Distance [km]:";
            // 
            // OptionsDlg
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(617, 350);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OptionsDlg";
            this.Text = "Options";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.Bands.ResumeLayout(false);
            this.Bands.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        private void rb_KST_StartAsAwayHere_Click(object sender, EventArgs e)
        {
            // work around an issue with using user settings and data binding
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Checked == false)
                radioButton.Checked = true;
        }
    }
}
