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
            InitializeComponent();
        }

        private void btn_Options_WinTest_QRV_Select_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Win-Test\\extras";
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
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Win-Test\\cfg";
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
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDlg));
            btn_OK = new System.Windows.Forms.Button();
            btn_Cancel = new System.Windows.Forms.Button();
            btn_Options_WinTest_INI_Select = new System.Windows.Forms.Button();
            label6 = new System.Windows.Forms.Label();
            btn_Options_WinTest_QRV_Select = new System.Windows.Forms.Button();
            label10 = new System.Windows.Forms.Label();
            groupBox3 = new System.Windows.Forms.GroupBox();
            tb_KST_MaxDist = new System.Windows.Forms.TextBox();
            label17 = new System.Windows.Forms.Label();
            cb_ShowBeacons = new System.Windows.Forms.CheckBox();
            tb_Options_UpdateInterval = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            groupBox5 = new System.Windows.Forms.GroupBox();
            cb_KST_ShowBalloon = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            rb_KST_StartAsAway = new System.Windows.Forms.RadioButton();
            rb_KST_StartAsHere = new System.Windows.Forms.RadioButton();
            cb_KST_AutoConnect = new System.Windows.Forms.CheckBox();
            tb_KST_ServerName = new System.Windows.Forms.TextBox();
            cbb_KST_Chat = new System.Windows.Forms.ComboBox();
            tb_KST_Password = new System.Windows.Forms.TextBox();
            tb_KST_UserName = new System.Windows.Forms.TextBox();
            tabPage2 = new System.Windows.Forms.TabPage();
            cb_WinTest_Active = new System.Windows.Forms.CheckBox();
            tb_Options_WinTest_INI = new System.Windows.Forms.TextBox();
            tb_Options_WinTest_QRV = new System.Windows.Forms.TextBox();
            tabPage3 = new System.Windows.Forms.TabPage();
            Bands = new System.Windows.Forms.GroupBox();
            checkBox76GHz = new System.Windows.Forms.CheckBox();
            checkBox10GHz = new System.Windows.Forms.CheckBox();
            checkBox24GHz = new System.Windows.Forms.CheckBox();
            checkBox47GHz = new System.Windows.Forms.CheckBox();
            checkBox2320 = new System.Windows.Forms.CheckBox();
            checkBox3400 = new System.Windows.Forms.CheckBox();
            checkBox5760 = new System.Windows.Forms.CheckBox();
            checkBox1296 = new System.Windows.Forms.CheckBox();
            checkBox432 = new System.Windows.Forms.CheckBox();
            checkBox144 = new System.Windows.Forms.CheckBox();
            tabPage4 = new System.Windows.Forms.TabPage();
            label16 = new System.Windows.Forms.Label();
            cb_AS_QRG = new System.Windows.Forms.ComboBox();
            label8 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            cb_AS_Active = new System.Windows.Forms.CheckBox();
            tb_Options_AS_My_Name = new System.Windows.Forms.TextBox();
            tb_AS_MinDist = new System.Windows.Forms.TextBox();
            tb_Options_AS_Server_Name = new System.Windows.Forms.TextBox();
            tb_AS_MaxDist = new System.Windows.Forms.TextBox();
            tb_AS_Timeout = new System.Windows.Forms.TextBox();
            groupBox3.SuspendLayout();
            groupBox5.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            Bands.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // btn_OK
            // 
            btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btn_OK.Location = new System.Drawing.Point(521, 36);
            btn_OK.Name = "btn_OK";
            btn_OK.Size = new System.Drawing.Size(75, 23);
            btn_OK.TabIndex = 1;
            btn_OK.Text = "&OK";
            btn_OK.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btn_Cancel.Location = new System.Drawing.Point(521, 68);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new System.Drawing.Size(75, 23);
            btn_Cancel.TabIndex = 2;
            btn_Cancel.Text = "&Cancel";
            btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_Options_WinTest_INI_Select
            // 
            btn_Options_WinTest_INI_Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_Options_WinTest_INI_Select.Location = new System.Drawing.Point(345, 55);
            btn_Options_WinTest_INI_Select.Name = "btn_Options_WinTest_INI_Select";
            btn_Options_WinTest_INI_Select.Size = new System.Drawing.Size(75, 23);
            btn_Options_WinTest_INI_Select.TabIndex = 13;
            btn_Options_WinTest_INI_Select.Text = "Select";
            btn_Options_WinTest_INI_Select.UseVisualStyleBackColor = true;
            btn_Options_WinTest_INI_Select.Click += new System.EventHandler(btn_Options_WinTest_INI_Select_Click);
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label6.Location = new System.Drawing.Point(21, 60);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(37, 13);
            label6.TabIndex = 11;
            label6.Text = "wt.ini :";
            // 
            // btn_Options_WinTest_QRV_Select
            // 
            btn_Options_WinTest_QRV_Select.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btn_Options_WinTest_QRV_Select.Location = new System.Drawing.Point(345, 81);
            btn_Options_WinTest_QRV_Select.Name = "btn_Options_WinTest_QRV_Select";
            btn_Options_WinTest_QRV_Select.Size = new System.Drawing.Size(75, 23);
            btn_Options_WinTest_QRV_Select.TabIndex = 10;
            btn_Options_WinTest_QRV_Select.Text = "Select";
            btn_Options_WinTest_QRV_Select.UseVisualStyleBackColor = true;
            btn_Options_WinTest_QRV_Select.Click += new System.EventHandler(btn_Options_WinTest_QRV_Select_Click);
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label10.Location = new System.Drawing.Point(21, 86);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(45, 13);
            label10.TabIndex = 0;
            label10.Text = "qrv.xdt :";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(tb_KST_MaxDist);
            groupBox3.Controls.Add(label17);
            groupBox3.Controls.Add(cb_ShowBeacons);
            groupBox3.Controls.Add(tb_Options_UpdateInterval);
            groupBox3.Controls.Add(label7);
            groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            groupBox3.Location = new System.Drawing.Point(9, 19);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(445, 62);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Calls";
            // 
            // tb_KST_MaxDist
            // 
            tb_KST_MaxDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_MaxDist", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_KST_MaxDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_KST_MaxDist.Location = new System.Drawing.Point(384, 29);
            tb_KST_MaxDist.Name = "tb_KST_MaxDist";
            tb_KST_MaxDist.Size = new System.Drawing.Size(45, 20);
            tb_KST_MaxDist.TabIndex = 15;
            tb_KST_MaxDist.Text = global::wtKST.Properties.Settings.Default.KST_MaxDist;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label17.Location = new System.Drawing.Point(279, 32);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(101, 13);
            label17.TabIndex = 14;
            label17.Text = "Max. Distance [km]:";
            // 
            // cb_ShowBeacons
            // 
            cb_ShowBeacons.AutoSize = true;
            cb_ShowBeacons.Checked = global::wtKST.Properties.Settings.Default.ShowBeacons;
            cb_ShowBeacons.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "ShowBeacons", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cb_ShowBeacons.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_ShowBeacons.Location = new System.Drawing.Point(163, 32);
            cb_ShowBeacons.Name = "cb_ShowBeacons";
            cb_ShowBeacons.Size = new System.Drawing.Size(98, 17);
            cb_ShowBeacons.TabIndex = 13;
            cb_ShowBeacons.Text = "Show Beacons";
            cb_ShowBeacons.UseVisualStyleBackColor = true;
            // 
            // tb_Options_UpdateInterval
            // 
            tb_Options_UpdateInterval.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "UpdateInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_Options_UpdateInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_Options_UpdateInterval.Location = new System.Drawing.Point(111, 29);
            tb_Options_UpdateInterval.Name = "tb_Options_UpdateInterval";
            tb_Options_UpdateInterval.Size = new System.Drawing.Size(33, 20);
            tb_Options_UpdateInterval.TabIndex = 12;
            tb_Options_UpdateInterval.Text = global::wtKST.Properties.Settings.Default.UpdateInterval;
            tb_Options_UpdateInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tb_Options_UpdateInterval_KeyPress);
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label7.Location = new System.Drawing.Point(6, 32);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(99, 13);
            label7.TabIndex = 11;
            label7.Text = "Update interval [s] :";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(cb_KST_ShowBalloon);
            groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            groupBox5.Location = new System.Drawing.Point(9, 215);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(445, 62);
            groupBox5.TabIndex = 12;
            groupBox5.TabStop = false;
            groupBox5.Text = "Notifications";
            // 
            // cb_KST_ShowBalloon
            // 
            cb_KST_ShowBalloon.AutoSize = true;
            cb_KST_ShowBalloon.Checked = global::wtKST.Properties.Settings.Default.KST_ShowBalloon;
            cb_KST_ShowBalloon.CheckState = System.Windows.Forms.CheckState.Checked;
            cb_KST_ShowBalloon.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_ShowBalloon", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cb_KST_ShowBalloon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_KST_ShowBalloon.Location = new System.Drawing.Point(9, 29);
            cb_KST_ShowBalloon.Name = "cb_KST_ShowBalloon";
            cb_KST_ShowBalloon.Size = new System.Drawing.Size(241, 17);
            cb_KST_ShowBalloon.TabIndex = 11;
            cb_KST_ShowBalloon.Text = "On new MyMessages show ballon notification";
            cb_KST_ShowBalloon.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(15, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(44, 13);
            label1.TabIndex = 0;
            label1.Text = "Server :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label3.Location = new System.Drawing.Point(15, 71);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(35, 13);
            label3.TabIndex = 4;
            label3.Text = "User :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.Location = new System.Drawing.Point(15, 98);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(59, 13);
            label4.TabIndex = 6;
            label4.Text = "Password :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(15, 124);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(35, 13);
            label5.TabIndex = 9;
            label5.Text = "Chat :";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location = new System.Drawing.Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(479, 316);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(rb_KST_StartAsAway);
            tabPage1.Controls.Add(rb_KST_StartAsHere);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(cb_KST_AutoConnect);
            tabPage1.Controls.Add(tb_KST_ServerName);
            tabPage1.Controls.Add(label5);
            tabPage1.Controls.Add(cbb_KST_Chat);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(tb_KST_Password);
            tabPage1.Controls.Add(tb_KST_UserName);
            tabPage1.Controls.Add(label4);
            tabPage1.Location = new System.Drawing.Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Size = new System.Drawing.Size(471, 290);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "KST";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // rb_KST_StartAsAway
            // 
            rb_KST_StartAsAway.AutoSize = true;
            rb_KST_StartAsAway.Checked = global::wtKST.Properties.Settings.Default.KST_StartAsAway;
            rb_KST_StartAsAway.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_StartAsAway", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            rb_KST_StartAsAway.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rb_KST_StartAsAway.Location = new System.Drawing.Point(275, 101);
            rb_KST_StartAsAway.Name = "rb_KST_StartAsAway";
            rb_KST_StartAsAway.Size = new System.Drawing.Size(96, 17);
            rb_KST_StartAsAway.TabIndex = 12;
            rb_KST_StartAsAway.TabStop = true;
            rb_KST_StartAsAway.Text = "Start as AWAY";
            rb_KST_StartAsAway.UseVisualStyleBackColor = true;
            rb_KST_StartAsAway.Click += new System.EventHandler(rb_KST_StartAsAwayHere_Click);
            // 
            // rb_KST_StartAsHere
            // 
            rb_KST_StartAsHere.AutoSize = true;
            rb_KST_StartAsHere.Checked = global::wtKST.Properties.Settings.Default.KST_StartAsHere;
            rb_KST_StartAsHere.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_StartAsHere", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            rb_KST_StartAsHere.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rb_KST_StartAsHere.Location = new System.Drawing.Point(275, 70);
            rb_KST_StartAsHere.Name = "rb_KST_StartAsHere";
            rb_KST_StartAsHere.Size = new System.Drawing.Size(94, 17);
            rb_KST_StartAsHere.TabIndex = 11;
            rb_KST_StartAsHere.Text = "Start as HERE";
            rb_KST_StartAsHere.UseVisualStyleBackColor = true;
            rb_KST_StartAsHere.Click += new System.EventHandler(rb_KST_StartAsAwayHere_Click);
            // 
            // cb_KST_AutoConnect
            // 
            cb_KST_AutoConnect.AutoSize = true;
            cb_KST_AutoConnect.Checked = global::wtKST.Properties.Settings.Default.KST_AutoConnect;
            cb_KST_AutoConnect.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "KST_AutoConnect", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cb_KST_AutoConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_KST_AutoConnect.Location = new System.Drawing.Point(277, 21);
            cb_KST_AutoConnect.Name = "cb_KST_AutoConnect";
            cb_KST_AutoConnect.Size = new System.Drawing.Size(90, 17);
            cb_KST_AutoConnect.TabIndex = 10;
            cb_KST_AutoConnect.Text = "Auto connect";
            cb_KST_AutoConnect.UseVisualStyleBackColor = true;
            // 
            // tb_KST_ServerName
            // 
            tb_KST_ServerName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_ServerName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_KST_ServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_KST_ServerName.Location = new System.Drawing.Point(120, 18);
            tb_KST_ServerName.Name = "tb_KST_ServerName";
            tb_KST_ServerName.Size = new System.Drawing.Size(123, 20);
            tb_KST_ServerName.TabIndex = 1;
            tb_KST_ServerName.Text = global::wtKST.Properties.Settings.Default.KST_ServerName;
            // 
            // cbb_KST_Chat
            // 
            cbb_KST_Chat.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Chat", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cbb_KST_Chat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cbb_KST_Chat.FormattingEnabled = true;
            cbb_KST_Chat.Items.AddRange(new object[] {
            "1 - 50/70 MHz",
            "2 - 144/432 MHz",
            "3 - Microwave",
            "4 - EME/JT65",
            "5 - Low Band"});
            cbb_KST_Chat.Location = new System.Drawing.Point(120, 124);
            cbb_KST_Chat.Name = "cbb_KST_Chat";
            cbb_KST_Chat.Size = new System.Drawing.Size(121, 21);
            cbb_KST_Chat.TabIndex = 8;
            cbb_KST_Chat.Text = global::wtKST.Properties.Settings.Default.KST_Chat;
            // 
            // tb_KST_Password
            // 
            tb_KST_Password.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_KST_Password.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_KST_Password.Location = new System.Drawing.Point(120, 98);
            tb_KST_Password.Name = "tb_KST_Password";
            tb_KST_Password.PasswordChar = '*';
            tb_KST_Password.Size = new System.Drawing.Size(123, 20);
            tb_KST_Password.TabIndex = 7;
            tb_KST_Password.Text = global::wtKST.Properties.Settings.Default.KST_Password;
            // 
            // tb_KST_UserName
            // 
            tb_KST_UserName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "KST_UserName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_KST_UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_KST_UserName.Location = new System.Drawing.Point(120, 71);
            tb_KST_UserName.Name = "tb_KST_UserName";
            tb_KST_UserName.Size = new System.Drawing.Size(123, 20);
            tb_KST_UserName.TabIndex = 5;
            tb_KST_UserName.Text = global::wtKST.Properties.Settings.Default.KST_UserName;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(cb_WinTest_Active);
            tabPage2.Controls.Add(btn_Options_WinTest_QRV_Select);
            tabPage2.Controls.Add(btn_Options_WinTest_INI_Select);
            tabPage2.Controls.Add(label10);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(tb_Options_WinTest_INI);
            tabPage2.Controls.Add(tb_Options_WinTest_QRV);
            tabPage2.Location = new System.Drawing.Point(4, 22);
            tabPage2.Name = "tabPage2";
            tabPage2.Size = new System.Drawing.Size(471, 290);
            tabPage2.TabIndex = 0;
            tabPage2.Text = "Win-Test";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // cb_WinTest_Active
            // 
            cb_WinTest_Active.AutoSize = true;
            cb_WinTest_Active.Checked = global::wtKST.Properties.Settings.Default.WinTest_Activate;
            cb_WinTest_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "WinTest_Activate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cb_WinTest_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_WinTest_Active.Location = new System.Drawing.Point(24, 29);
            cb_WinTest_Active.Name = "cb_WinTest_Active";
            cb_WinTest_Active.Size = new System.Drawing.Size(297, 17);
            cb_WinTest_Active.TabIndex = 14;
            cb_WinTest_Active.Text = "Activate (only DL0GTH crew members, not for public use)";
            cb_WinTest_Active.UseVisualStyleBackColor = true;
            // 
            // tb_Options_WinTest_INI
            // 
            tb_Options_WinTest_INI.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WinTest_INI_Filename", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_Options_WinTest_INI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_Options_WinTest_INI.Location = new System.Drawing.Point(126, 57);
            tb_Options_WinTest_INI.Name = "tb_Options_WinTest_INI";
            tb_Options_WinTest_INI.Size = new System.Drawing.Size(204, 20);
            tb_Options_WinTest_INI.TabIndex = 12;
            tb_Options_WinTest_INI.Text = global::wtKST.Properties.Settings.Default.WinTest_INI_FileName;
            // 
            // tb_Options_WinTest_QRV
            // 
            tb_Options_WinTest_QRV.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "WinTest_QRV_Filename", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_Options_WinTest_QRV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_Options_WinTest_QRV.Location = new System.Drawing.Point(126, 83);
            tb_Options_WinTest_QRV.Name = "tb_Options_WinTest_QRV";
            tb_Options_WinTest_QRV.Size = new System.Drawing.Size(204, 20);
            tb_Options_WinTest_QRV.TabIndex = 1;
            tb_Options_WinTest_QRV.Text = global::wtKST.Properties.Settings.Default.WinTest_QRV_FileName;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(Bands);
            tabPage3.Controls.Add(groupBox3);
            tabPage3.Controls.Add(groupBox5);
            tabPage3.Location = new System.Drawing.Point(4, 22);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new System.Drawing.Size(471, 290);
            tabPage3.TabIndex = 1;
            tabPage3.Text = "Calls";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // Bands
            // 
            Bands.Controls.Add(checkBox76GHz);
            Bands.Controls.Add(checkBox10GHz);
            Bands.Controls.Add(checkBox24GHz);
            Bands.Controls.Add(checkBox47GHz);
            Bands.Controls.Add(checkBox2320);
            Bands.Controls.Add(checkBox3400);
            Bands.Controls.Add(checkBox5760);
            Bands.Controls.Add(checkBox1296);
            Bands.Controls.Add(checkBox432);
            Bands.Controls.Add(checkBox144);
            Bands.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Bands.Location = new System.Drawing.Point(9, 87);
            Bands.Name = "Bands";
            Bands.Size = new System.Drawing.Size(445, 122);
            Bands.TabIndex = 13;
            Bands.TabStop = false;
            Bands.Text = "Bands";
            // 
            // checkBox76GHz
            // 
            checkBox76GHz.AutoSize = true;
            checkBox76GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox76GHz.Checked = global::wtKST.Properties.Settings.Default.Band_76GHz;
            checkBox76GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox76GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_76GHz", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox76GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox76GHz.Location = new System.Drawing.Point(46, 96);
            checkBox76GHz.Name = "checkBox76GHz";
            checkBox76GHz.Size = new System.Drawing.Size(59, 17);
            checkBox76GHz.TabIndex = 10;
            checkBox76GHz.Text = "76GHz";
            checkBox76GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox10GHz
            // 
            checkBox10GHz.AutoSize = true;
            checkBox10GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox10GHz.Checked = global::wtKST.Properties.Settings.Default.Band_10368;
            checkBox10GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox10GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_10368", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox10GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox10GHz.Location = new System.Drawing.Point(46, 73);
            checkBox10GHz.Name = "checkBox10GHz";
            checkBox10GHz.Size = new System.Drawing.Size(59, 17);
            checkBox10GHz.TabIndex = 7;
            checkBox10GHz.Text = "10GHz";
            checkBox10GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox24GHz
            // 
            checkBox24GHz.AutoSize = true;
            checkBox24GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox24GHz.Checked = global::wtKST.Properties.Settings.Default.Band_24GHz;
            checkBox24GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox24GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_24GHz", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox24GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox24GHz.Location = new System.Drawing.Point(150, 73);
            checkBox24GHz.Name = "checkBox24GHz";
            checkBox24GHz.Size = new System.Drawing.Size(59, 17);
            checkBox24GHz.TabIndex = 8;
            checkBox24GHz.Text = "24GHz";
            checkBox24GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox47GHz
            // 
            checkBox47GHz.AutoSize = true;
            checkBox47GHz.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox47GHz.Checked = global::wtKST.Properties.Settings.Default.Band_47GHz;
            checkBox47GHz.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox47GHz.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_47GHz", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox47GHz.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox47GHz.Location = new System.Drawing.Point(263, 73);
            checkBox47GHz.Name = "checkBox47GHz";
            checkBox47GHz.Size = new System.Drawing.Size(59, 17);
            checkBox47GHz.TabIndex = 9;
            checkBox47GHz.Text = "47GHz";
            checkBox47GHz.UseVisualStyleBackColor = true;
            // 
            // checkBox2320
            // 
            checkBox2320.AutoSize = true;
            checkBox2320.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox2320.Checked = global::wtKST.Properties.Settings.Default.Band_2320;
            checkBox2320.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox2320.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_2320", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox2320.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox2320.Location = new System.Drawing.Point(33, 50);
            checkBox2320.Name = "checkBox2320";
            checkBox2320.Size = new System.Drawing.Size(72, 17);
            checkBox2320.TabIndex = 4;
            checkBox2320.Text = "2320MHz";
            checkBox2320.UseVisualStyleBackColor = true;
            // 
            // checkBox3400
            // 
            checkBox3400.AutoSize = true;
            checkBox3400.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox3400.Checked = global::wtKST.Properties.Settings.Default.Band_3400;
            checkBox3400.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox3400.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_3400", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox3400.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox3400.Location = new System.Drawing.Point(137, 50);
            checkBox3400.Name = "checkBox3400";
            checkBox3400.Size = new System.Drawing.Size(72, 17);
            checkBox3400.TabIndex = 5;
            checkBox3400.Text = "3400MHz";
            checkBox3400.UseVisualStyleBackColor = true;
            // 
            // checkBox5760
            // 
            checkBox5760.AutoSize = true;
            checkBox5760.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox5760.Checked = global::wtKST.Properties.Settings.Default.Band_5760;
            checkBox5760.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox5760.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_5760", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox5760.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox5760.Location = new System.Drawing.Point(250, 50);
            checkBox5760.Name = "checkBox5760";
            checkBox5760.Size = new System.Drawing.Size(72, 17);
            checkBox5760.TabIndex = 6;
            checkBox5760.Text = "5760MHz";
            checkBox5760.UseVisualStyleBackColor = true;
            // 
            // checkBox1296
            // 
            checkBox1296.AutoSize = true;
            checkBox1296.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox1296.Checked = global::wtKST.Properties.Settings.Default.Band_1296;
            checkBox1296.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox1296.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_1296", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox1296.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox1296.Location = new System.Drawing.Point(250, 27);
            checkBox1296.Name = "checkBox1296";
            checkBox1296.Size = new System.Drawing.Size(72, 17);
            checkBox1296.TabIndex = 3;
            checkBox1296.Text = "1296MHz";
            checkBox1296.UseVisualStyleBackColor = true;
            // 
            // checkBox432
            // 
            checkBox432.AutoSize = true;
            checkBox432.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox432.Checked = global::wtKST.Properties.Settings.Default.Band_432;
            checkBox432.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox432.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_432", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox432.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox432.Location = new System.Drawing.Point(143, 27);
            checkBox432.Name = "checkBox432";
            checkBox432.Size = new System.Drawing.Size(66, 17);
            checkBox432.TabIndex = 2;
            checkBox432.Text = "432MHz";
            checkBox432.UseVisualStyleBackColor = true;
            // 
            // checkBox144
            // 
            checkBox144.AutoSize = true;
            checkBox144.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            checkBox144.Checked = global::wtKST.Properties.Settings.Default.Band_144;
            checkBox144.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox144.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "Band_144", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            checkBox144.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            checkBox144.Location = new System.Drawing.Point(39, 27);
            checkBox144.Name = "checkBox144";
            checkBox144.Size = new System.Drawing.Size(66, 17);
            checkBox144.TabIndex = 1;
            checkBox144.Text = "144MHz";
            checkBox144.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(label16);
            tabPage4.Controls.Add(cb_AS_QRG);
            tabPage4.Controls.Add(label8);
            tabPage4.Controls.Add(label15);
            tabPage4.Controls.Add(label9);
            tabPage4.Controls.Add(label14);
            tabPage4.Controls.Add(label13);
            tabPage4.Controls.Add(label11);
            tabPage4.Controls.Add(label12);
            tabPage4.Controls.Add(cb_AS_Active);
            tabPage4.Controls.Add(tb_Options_AS_My_Name);
            tabPage4.Controls.Add(tb_AS_MinDist);
            tabPage4.Controls.Add(tb_Options_AS_Server_Name);
            tabPage4.Controls.Add(tb_AS_MaxDist);
            tabPage4.Controls.Add(tb_AS_Timeout);
            tabPage4.Location = new System.Drawing.Point(4, 22);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new System.Drawing.Size(471, 290);
            tabPage4.TabIndex = 2;
            tabPage4.Text = "Airplane Scatter";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label16.Location = new System.Drawing.Point(299, 67);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(60, 13);
            label16.TabIndex = 25;
            label16.Text = "Frequency:";
            // 
            // cb_AS_QRG
            // 
            cb_AS_QRG.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_QRG", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cb_AS_QRG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_AS_QRG.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_AS_QRG.FormattingEnabled = true;
            cb_AS_QRG.Items.AddRange(new object[] {
            "144M",
            "432M",
            "1.2G",
            "2.3G",
            "3.4G",
            "5.7G",
            "10G"});
            cb_AS_QRG.Location = new System.Drawing.Point(368, 63);
            cb_AS_QRG.Name = "cb_AS_QRG";
            cb_AS_QRG.Size = new System.Drawing.Size(78, 21);
            cb_AS_QRG.TabIndex = 24;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label8.Location = new System.Drawing.Point(13, 150);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(224, 13);
            label8.TabIndex = 12;
            label8.Text = "Get aircraft positions in the range of minumum:";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label15.Location = new System.Drawing.Point(158, 67);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(55, 13);
            label15.TabIndex = 22;
            label15.Text = "My Name:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label9.Location = new System.Drawing.Point(273, 150);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(102, 13);
            label9.TabIndex = 14;
            label9.Text = "kms up to maximum:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label14.Location = new System.Drawing.Point(15, 67);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(70, 13);
            label14.TabIndex = 20;
            label14.Text = "Server name:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label13.Location = new System.Drawing.Point(199, 101);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(27, 13);
            label13.TabIndex = 19;
            label13.Text = "sec.";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label11.Location = new System.Drawing.Point(417, 150);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(29, 13);
            label11.TabIndex = 16;
            label11.Text = "kms.";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label12.Location = new System.Drawing.Point(13, 101);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(141, 13);
            label12.TabIndex = 17;
            label12.Text = "Timeout for server response:";
            // 
            // cb_AS_Active
            // 
            cb_AS_Active.AutoSize = true;
            cb_AS_Active.Checked = global::wtKST.Properties.Settings.Default.AS_Active;
            cb_AS_Active.CheckState = System.Windows.Forms.CheckState.Checked;
            cb_AS_Active.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::wtKST.Properties.Settings.Default, "AS_Active", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            cb_AS_Active.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cb_AS_Active.Location = new System.Drawing.Point(16, 29);
            cb_AS_Active.Name = "cb_AS_Active";
            cb_AS_Active.Size = new System.Drawing.Size(366, 17);
            cb_AS_Active.TabIndex = 11;
            cb_AS_Active.Text = "Activate (Requires AirScout server functionionaltiy newer than V.0.9.9.5)";
            cb_AS_Active.UseVisualStyleBackColor = true;
            // 
            // tb_Options_AS_My_Name
            // 
            tb_Options_AS_My_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_My_Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_Options_AS_My_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_Options_AS_My_Name.Location = new System.Drawing.Point(217, 64);
            tb_Options_AS_My_Name.Name = "tb_Options_AS_My_Name";
            tb_Options_AS_My_Name.Size = new System.Drawing.Size(66, 20);
            tb_Options_AS_My_Name.TabIndex = 23;
            tb_Options_AS_My_Name.Text = global::wtKST.Properties.Settings.Default.AS_My_Name;
            // 
            // tb_AS_MinDist
            // 
            tb_AS_MinDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_MinDist", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_AS_MinDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_AS_MinDist.Location = new System.Drawing.Point(234, 147);
            tb_AS_MinDist.Name = "tb_AS_MinDist";
            tb_AS_MinDist.Size = new System.Drawing.Size(33, 20);
            tb_AS_MinDist.TabIndex = 13;
            tb_AS_MinDist.Text = global::wtKST.Properties.Settings.Default.AS_MinDist;
            // 
            // tb_Options_AS_Server_Name
            // 
            tb_Options_AS_Server_Name.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Server_Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_Options_AS_Server_Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_Options_AS_Server_Name.Location = new System.Drawing.Point(89, 64);
            tb_Options_AS_Server_Name.Name = "tb_Options_AS_Server_Name";
            tb_Options_AS_Server_Name.Size = new System.Drawing.Size(51, 20);
            tb_Options_AS_Server_Name.TabIndex = 21;
            tb_Options_AS_Server_Name.Text = global::wtKST.Properties.Settings.Default.AS_Server_Name;
            // 
            // tb_AS_MaxDist
            // 
            tb_AS_MaxDist.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_MaxDist", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_AS_MaxDist.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_AS_MaxDist.Location = new System.Drawing.Point(378, 147);
            tb_AS_MaxDist.Name = "tb_AS_MaxDist";
            tb_AS_MaxDist.Size = new System.Drawing.Size(33, 20);
            tb_AS_MaxDist.TabIndex = 15;
            tb_AS_MaxDist.Text = global::wtKST.Properties.Settings.Default.AS_MaxDist;
            // 
            // tb_AS_Timeout
            // 
            tb_AS_Timeout.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::wtKST.Properties.Settings.Default, "AS_Timeout", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            tb_AS_Timeout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tb_AS_Timeout.Location = new System.Drawing.Point(160, 98);
            tb_AS_Timeout.Name = "tb_AS_Timeout";
            tb_AS_Timeout.Size = new System.Drawing.Size(33, 20);
            tb_AS_Timeout.TabIndex = 18;
            tb_AS_Timeout.Text = global::wtKST.Properties.Settings.Default.AS_Timeout;
            // 
            // OptionsDlg
            // 
            AcceptButton = btn_OK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btn_Cancel;
            ClientSize = new System.Drawing.Size(617, 350);
            Controls.Add(tabControl1);
            Controls.Add(btn_Cancel);
            Controls.Add(btn_OK);
            Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            Name = "OptionsDlg";
            Text = "Options";
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            Bands.ResumeLayout(false);
            Bands.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);

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
