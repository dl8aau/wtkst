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

        private GroupBox groupBox1;

        private Label label1;

        private Label label2;

        private Label label5;

        private Label label4;

        private Label label3;

        private Button btn_OK;

        private Button btn_Cancel;

        public TextBox tb_KST_ServerPort;

        public TextBox tb_KST_ServerName;

        public RadioButton rb_KST_StartAsAway;

        public RadioButton rb_KST_StartAsHere;

        public CheckBox cb_KST_AutoConnect;

        public ComboBox cbb_KST_Chat;

        public TextBox tb_KST_Password;

        public TextBox tb_KST_UserName;

        private GroupBox groupBox2;

        private Button btn_Options_WinTest_QRV_Select;

        public TextBox tb_Options_WinTest_QRV;

        private Label label10;

        private Button btn_Options_WinTest_INI_Select;

        public TextBox tb_Options_WinTest_INI;

        private Label label6;

        private GroupBox groupBox3;

        public TextBox tb_Options_UpdateInterval;

        private Label label7;

        private GroupBox groupBox4;

        public CheckBox cb_AS_Active;

        private GroupBox groupBox5;

        public CheckBox cb_KST_ShowBalloon;

        public CheckBox cb_ShowBeacons;

        public CheckBox cb_WinTest_Active;

        private Label label11;

        private TextBox tb_AS_MaxDist;

        private Label label9;

        private TextBox tb_AS_MinDist;

        private Label label8;

        private Label label13;

        private TextBox tb_AS_Timeout;

        private Label label12;

        private TextBox tb_Options_AS_Server_Name;

        private Label label14;

        private TextBox tb_Options_AS_My_Name;

        private Label label15;

        private Label label16;

        private ComboBox cb_AS_QRG;

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
            this.groupBox1 = new GroupBox();
            this.label5 = new Label();
            this.label4 = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.btn_OK = new Button();
            this.btn_Cancel = new Button();
            this.groupBox2 = new GroupBox();
            this.btn_Options_WinTest_INI_Select = new Button();
            this.label6 = new Label();
            this.btn_Options_WinTest_QRV_Select = new Button();
            this.label10 = new Label();
            this.groupBox3 = new GroupBox();
            this.label7 = new Label();
            this.groupBox4 = new GroupBox();
            this.label15 = new Label();
            this.label14 = new Label();
            this.label13 = new Label();
            this.label12 = new Label();
            this.label11 = new Label();
            this.label9 = new Label();
            this.label8 = new Label();
            this.groupBox5 = new GroupBox();
            this.label16 = new Label();
            this.cb_KST_ShowBalloon = new CheckBox();
            this.cb_AS_QRG = new ComboBox();
            this.tb_Options_AS_My_Name = new TextBox();
            this.tb_Options_AS_Server_Name = new TextBox();
            this.tb_AS_Timeout = new TextBox();
            this.tb_AS_MaxDist = new TextBox();
            this.tb_AS_MinDist = new TextBox();
            this.cb_AS_Active = new CheckBox();
            this.cb_ShowBeacons = new CheckBox();
            this.tb_Options_UpdateInterval = new TextBox();
            this.cb_WinTest_Active = new CheckBox();
            this.tb_Options_WinTest_INI = new TextBox();
            this.tb_Options_WinTest_QRV = new TextBox();
            this.rb_KST_StartAsAway = new RadioButton();
            this.rb_KST_StartAsHere = new RadioButton();
            this.cb_KST_AutoConnect = new CheckBox();
            this.cbb_KST_Chat = new ComboBox();
            this.tb_KST_Password = new TextBox();
            this.tb_KST_UserName = new TextBox();
            this.tb_KST_ServerPort = new TextBox();
            this.tb_KST_ServerName = new TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            base.SuspendLayout();
            this.groupBox1.Controls.Add(this.rb_KST_StartAsAway);
            this.groupBox1.Controls.Add(this.rb_KST_StartAsHere);
            this.groupBox1.Controls.Add(this.cb_KST_AutoConnect);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cbb_KST_Chat);
            this.groupBox1.Controls.Add(this.tb_KST_Password);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tb_KST_UserName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tb_KST_ServerPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tb_KST_ServerName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            this.groupBox1.Location = new Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(488, 173);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "KST";
            this.label5.AutoSize = true;
            this.label5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label5.Location = new Point(6, 136);
            this.label5.Name = "label5";
            this.label5.Size = new Size(35, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Chat :";
            this.label4.AutoSize = true;
            this.label4.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label4.Location = new Point(6, 110);
            this.label4.Name = "label4";
            this.label4.Size = new Size(59, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Password :";
            this.label3.AutoSize = true;
            this.label3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label3.Location = new Point(6, 83);
            this.label3.Name = "label3";
            this.label3.Size = new Size(35, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User :";
            this.label2.AutoSize = true;
            this.label2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label2.Location = new Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port :";
            this.label1.AutoSize = true;
            this.label1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server :";
            this.btn_OK.DialogResult = DialogResult.OK;
            this.btn_OK.Location = new Point(521, 36);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new Size(75, 23);
            this.btn_OK.TabIndex = 1;
            this.btn_OK.Text = "&OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_Cancel.DialogResult = DialogResult.Cancel;
            this.btn_Cancel.Location = new Point(521, 68);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new Size(75, 23);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "&Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.groupBox2.Controls.Add(this.cb_WinTest_Active);
            this.groupBox2.Controls.Add(this.btn_Options_WinTest_INI_Select);
            this.groupBox2.Controls.Add(this.tb_Options_WinTest_INI);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btn_Options_WinTest_QRV_Select);
            this.groupBox2.Controls.Add(this.tb_Options_WinTest_QRV);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            this.groupBox2.Location = new Point(12, 191);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(488, 137);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Win-Test";
            this.btn_Options_WinTest_INI_Select.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btn_Options_WinTest_INI_Select.Location = new Point(330, 55);
            this.btn_Options_WinTest_INI_Select.Name = "btn_Options_WinTest_INI_Select";
            this.btn_Options_WinTest_INI_Select.Size = new Size(75, 23);
            this.btn_Options_WinTest_INI_Select.TabIndex = 13;
            this.btn_Options_WinTest_INI_Select.Text = "Auswählen";
            this.btn_Options_WinTest_INI_Select.UseVisualStyleBackColor = true;
            this.btn_Options_WinTest_INI_Select.Click += new EventHandler(this.btn_Options_WinTest_INI_Select_Click);
            this.label6.AutoSize = true;
            this.label6.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label6.Location = new Point(6, 60);
            this.label6.Name = "label6";
            this.label6.Size = new Size(37, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "wt.ini :";
            this.btn_Options_WinTest_QRV_Select.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.btn_Options_WinTest_QRV_Select.Location = new Point(330, 81);
            this.btn_Options_WinTest_QRV_Select.Name = "btn_Options_WinTest_QRV_Select";
            this.btn_Options_WinTest_QRV_Select.Size = new Size(75, 23);
            this.btn_Options_WinTest_QRV_Select.TabIndex = 10;
            this.btn_Options_WinTest_QRV_Select.Text = "Auswählen";
            this.btn_Options_WinTest_QRV_Select.UseVisualStyleBackColor = true;
            this.btn_Options_WinTest_QRV_Select.Click += new EventHandler(this.btn_Options_WinTest_QRV_Select_Click);
            this.label10.AutoSize = true;
            this.label10.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label10.Location = new Point(6, 86);
            this.label10.Name = "label10";
            this.label10.Size = new Size(45, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "qrv.xdt :";
            this.groupBox3.Controls.Add(this.cb_ShowBeacons);
            this.groupBox3.Controls.Add(this.tb_Options_UpdateInterval);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            this.groupBox3.Location = new Point(12, 334);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new Size(488, 62);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Calls";
            this.label7.AutoSize = true;
            this.label7.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label7.Location = new Point(6, 32);
            this.label7.Name = "label7";
            this.label7.Size = new Size(99, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Update interval [s] :";
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.cb_AS_QRG);
            this.groupBox4.Controls.Add(this.tb_Options_AS_My_Name);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.tb_Options_AS_Server_Name);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.tb_AS_Timeout);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.tb_AS_MaxDist);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.tb_AS_MinDist);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.cb_AS_Active);
            this.groupBox4.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            this.groupBox4.Location = new Point(12, 402);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new Size(488, 177);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Airplane Scatter Prediction";
            this.label15.AutoSize = true;
            this.label15.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label15.Location = new Point(151, 67);
            this.label15.Name = "label15";
            this.label15.Size = new Size(55, 13);
            this.label15.TabIndex = 22;
            this.label15.Text = "My Name:";
            this.label14.AutoSize = true;
            this.label14.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label14.Location = new Point(8, 67);
            this.label14.Name = "label14";
            this.label14.Size = new Size(70, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "Server name:";
            this.label13.AutoSize = true;
            this.label13.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label13.Location = new Point(192, 101);
            this.label13.Name = "label13";
            this.label13.Size = new Size(27, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "sec.";
            this.label12.AutoSize = true;
            this.label12.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label12.Location = new Point(6, 101);
            this.label12.Name = "label12";
            this.label12.Size = new Size(141, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Timeout for server response:";
            this.label11.AutoSize = true;
            this.label11.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label11.Location = new Point(410, 150);
            this.label11.Name = "label11";
            this.label11.Size = new Size(29, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "kms.";
            this.label9.AutoSize = true;
            this.label9.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label9.Location = new Point(266, 150);
            this.label9.Name = "label9";
            this.label9.Size = new Size(102, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "kms up to maximum:";
            this.label8.AutoSize = true;
            this.label8.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label8.Location = new Point(6, 150);
            this.label8.Name = "label8";
            this.label8.Size = new Size(224, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Get aircraft positions in the range of minumum:";
            this.groupBox5.Controls.Add(this.cb_KST_ShowBalloon);
            this.groupBox5.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            this.groupBox5.Location = new Point(12, 585);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new Size(488, 62);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Notifications";
            this.label16.AutoSize = true;
            this.label16.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label16.Location = new Point(292, 67);
            this.label16.Name = "label16";
            this.label16.Size = new Size(60, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "Frequency:";
            this.cb_KST_ShowBalloon.AutoSize = true;
            this.cb_KST_ShowBalloon.Checked = Settings.Default.KST_ShowBalloon;
            this.cb_KST_ShowBalloon.CheckState = CheckState.Checked;
            this.cb_KST_ShowBalloon.DataBindings.Add(new Binding("Checked", Settings.Default, "KST_ShowBalloon", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cb_KST_ShowBalloon.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cb_KST_ShowBalloon.Location = new Point(9, 29);
            this.cb_KST_ShowBalloon.Name = "cb_KST_ShowBalloon";
            this.cb_KST_ShowBalloon.Size = new Size(241, 17);
            this.cb_KST_ShowBalloon.TabIndex = 11;
            this.cb_KST_ShowBalloon.Text = "On new MyMessages show ballon notification";
            this.cb_KST_ShowBalloon.UseVisualStyleBackColor = true;
            this.cb_AS_QRG.DataBindings.Add(new Binding("Text", Settings.Default, "AS_QRG", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cb_AS_QRG.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cb_AS_QRG.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cb_AS_QRG.FormattingEnabled = true;
            this.cb_AS_QRG.Items.AddRange(new object[]
            {
                "144M",
                "432M",
                "1.2G",
                "2.3G",
                "3.4G",
                "5.7G",
                "10G"
            });
            this.cb_AS_QRG.Location = new Point(361, 63);
            this.cb_AS_QRG.Name = "cb_AS_QRG";
            this.cb_AS_QRG.Size = new Size(78, 21);
            this.cb_AS_QRG.TabIndex = 24;
            this.cb_AS_QRG.Text = Settings.Default.AS_QRG;
            this.tb_Options_AS_My_Name.DataBindings.Add(new Binding("Text", Settings.Default, "AS_My_Name", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_AS_My_Name.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_Options_AS_My_Name.Location = new Point(210, 64);
            this.tb_Options_AS_My_Name.Name = "tb_Options_AS_My_Name";
            this.tb_Options_AS_My_Name.Size = new Size(66, 20);
            this.tb_Options_AS_My_Name.TabIndex = 23;
            this.tb_Options_AS_My_Name.Text = Settings.Default.AS_My_Name;
            this.tb_Options_AS_Server_Name.DataBindings.Add(new Binding("Text", Settings.Default, "AS_Server_Name", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_AS_Server_Name.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_Options_AS_Server_Name.Location = new Point(82, 64);
            this.tb_Options_AS_Server_Name.Name = "tb_Options_AS_Server_Name";
            this.tb_Options_AS_Server_Name.Size = new Size(51, 20);
            this.tb_Options_AS_Server_Name.TabIndex = 21;
            this.tb_Options_AS_Server_Name.Text = Settings.Default.AS_Server_Name;
            this.tb_AS_Timeout.DataBindings.Add(new Binding("Text", Settings.Default, "AS_Timeout", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_AS_Timeout.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_AS_Timeout.Location = new Point(153, 98);
            this.tb_AS_Timeout.Name = "tb_AS_Timeout";
            this.tb_AS_Timeout.Size = new Size(33, 20);
            this.tb_AS_Timeout.TabIndex = 18;
            this.tb_AS_Timeout.Text = Settings.Default.AS_Timeout;
            this.tb_AS_MaxDist.DataBindings.Add(new Binding("Text", Settings.Default, "AS_MaxDist", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_AS_MaxDist.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_AS_MaxDist.Location = new Point(371, 147);
            this.tb_AS_MaxDist.Name = "tb_AS_MaxDist";
            this.tb_AS_MaxDist.Size = new Size(33, 20);
            this.tb_AS_MaxDist.TabIndex = 15;
            this.tb_AS_MaxDist.Text = Settings.Default.AS_MaxDist;
            this.tb_AS_MinDist.DataBindings.Add(new Binding("Text", Settings.Default, "AS_MinDist", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_AS_MinDist.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_AS_MinDist.Location = new Point(227, 147);
            this.tb_AS_MinDist.Name = "tb_AS_MinDist";
            this.tb_AS_MinDist.Size = new Size(33, 20);
            this.tb_AS_MinDist.TabIndex = 13;
            this.tb_AS_MinDist.Text = Settings.Default.AS_MinDist;
            this.cb_AS_Active.AutoSize = true;
            this.cb_AS_Active.Checked = Settings.Default.AS_Active;
            this.cb_AS_Active.CheckState = CheckState.Checked;
            this.cb_AS_Active.DataBindings.Add(new Binding("Checked", Settings.Default, "AS_Active", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cb_AS_Active.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cb_AS_Active.Location = new Point(9, 29);
            this.cb_AS_Active.Name = "cb_AS_Active";
            this.cb_AS_Active.Size = new Size(366, 17);
            this.cb_AS_Active.TabIndex = 11;
            this.cb_AS_Active.Text = "Activate (Requires AirScout server functionionaltiy newer than V.0.9.9.5)";
            this.cb_AS_Active.UseVisualStyleBackColor = true;
            this.cb_ShowBeacons.AutoSize = true;
            this.cb_ShowBeacons.Checked = Settings.Default.ShowBeacons;
            this.cb_ShowBeacons.DataBindings.Add(new Binding("Checked", Settings.Default, "ShowBeacons", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cb_ShowBeacons.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cb_ShowBeacons.Location = new Point(218, 31);
            this.cb_ShowBeacons.Name = "cb_ShowBeacons";
            this.cb_ShowBeacons.Size = new Size(98, 17);
            this.cb_ShowBeacons.TabIndex = 13;
            this.cb_ShowBeacons.Text = "Show Beacons";
            this.cb_ShowBeacons.UseVisualStyleBackColor = true;
            this.tb_Options_UpdateInterval.DataBindings.Add(new Binding("Text", Settings.Default, "UpdateInterval", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_UpdateInterval.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_Options_UpdateInterval.Location = new Point(111, 29);
            this.tb_Options_UpdateInterval.Name = "tb_Options_UpdateInterval";
            this.tb_Options_UpdateInterval.Size = new Size(71, 20);
            this.tb_Options_UpdateInterval.TabIndex = 12;
            this.tb_Options_UpdateInterval.Text = Settings.Default.UpdateInterval;
            this.tb_Options_UpdateInterval.KeyPress += new KeyPressEventHandler(this.tb_Options_UpdateInterval_KeyPress);
            this.cb_WinTest_Active.AutoSize = true;
            this.cb_WinTest_Active.Checked = Settings.Default.WinTest_Activate;
            this.cb_WinTest_Active.DataBindings.Add(new Binding("Checked", Settings.Default, "WinTest_Activate", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cb_WinTest_Active.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cb_WinTest_Active.Location = new Point(9, 29);
            this.cb_WinTest_Active.Name = "cb_WinTest_Active";
            this.cb_WinTest_Active.Size = new Size(297, 17);
            this.cb_WinTest_Active.TabIndex = 14;
            this.cb_WinTest_Active.Text = "Activate (only DL0GTH crew members, not for public use)";
            this.cb_WinTest_Active.UseVisualStyleBackColor = true;
            this.tb_Options_WinTest_INI.DataBindings.Add(new Binding("Text", Settings.Default, "WinTest_INI_Filename", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_WinTest_INI.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_Options_WinTest_INI.Location = new Point(111, 57);
            this.tb_Options_WinTest_INI.Name = "tb_Options_WinTest_INI";
            this.tb_Options_WinTest_INI.Size = new Size(204, 20);
            this.tb_Options_WinTest_INI.TabIndex = 12;
            this.tb_Options_WinTest_INI.Text = Settings.Default.WinTest_INI_FileName;
            this.tb_Options_WinTest_QRV.DataBindings.Add(new Binding("Text", Settings.Default, "WinTest_QRV_Filename", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_Options_WinTest_QRV.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_Options_WinTest_QRV.Location = new Point(111, 83);
            this.tb_Options_WinTest_QRV.Name = "tb_Options_WinTest_QRV";
            this.tb_Options_WinTest_QRV.Size = new Size(204, 20);
            this.tb_Options_WinTest_QRV.TabIndex = 1;
            this.tb_Options_WinTest_QRV.Text = Settings.Default.WinTest_QRV_FileName;
            this.rb_KST_StartAsAway.AutoSize = true;
            this.rb_KST_StartAsAway.Checked = Settings.Default.KST_StartAsAway;
            this.rb_KST_StartAsAway.DataBindings.Add(new Binding("Checked", Settings.Default, "KST_StartAsAway", true, DataSourceUpdateMode.OnPropertyChanged));
            this.rb_KST_StartAsAway.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.rb_KST_StartAsAway.Location = new Point(266, 113);
            this.rb_KST_StartAsAway.Name = "rb_KST_StartAsAway";
            this.rb_KST_StartAsAway.Size = new Size(96, 17);
            this.rb_KST_StartAsAway.TabIndex = 12;
            this.rb_KST_StartAsAway.TabStop = true;
            this.rb_KST_StartAsAway.Text = "Start as AWAY";
            this.rb_KST_StartAsAway.UseVisualStyleBackColor = true;
            this.rb_KST_StartAsHere.AutoSize = true;
            this.rb_KST_StartAsHere.Checked = Settings.Default.KST_StartAsHere;
            this.rb_KST_StartAsHere.DataBindings.Add(new Binding("Checked", Settings.Default, "KST_StartAsHere", true, DataSourceUpdateMode.OnPropertyChanged));
            this.rb_KST_StartAsHere.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.rb_KST_StartAsHere.Location = new Point(266, 82);
            this.rb_KST_StartAsHere.Name = "rb_KST_StartAsHere";
            this.rb_KST_StartAsHere.Size = new Size(94, 17);
            this.rb_KST_StartAsHere.TabIndex = 11;
            this.rb_KST_StartAsHere.Text = "Start as HERE";
            this.rb_KST_StartAsHere.UseVisualStyleBackColor = true;
            this.cb_KST_AutoConnect.AutoSize = true;
            this.cb_KST_AutoConnect.Checked = Settings.Default.KST_AutoConnect;
            this.cb_KST_AutoConnect.DataBindings.Add(new Binding("Checked", Settings.Default, "KST_AutoConnect", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cb_KST_AutoConnect.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cb_KST_AutoConnect.Location = new Point(268, 33);
            this.cb_KST_AutoConnect.Name = "cb_KST_AutoConnect";
            this.cb_KST_AutoConnect.Size = new Size(90, 17);
            this.cb_KST_AutoConnect.TabIndex = 10;
            this.cb_KST_AutoConnect.Text = "Auto connect";
            this.cb_KST_AutoConnect.UseVisualStyleBackColor = true;
            this.cbb_KST_Chat.DataBindings.Add(new Binding("Text", Settings.Default, "KST_Chat", true, DataSourceUpdateMode.OnPropertyChanged));
            this.cbb_KST_Chat.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.cbb_KST_Chat.FormattingEnabled = true;
            this.cbb_KST_Chat.Items.AddRange(new object[]
            {
                "1 - 50/70 MHz",
                "2 - 144/432 MHz",
                "3 - Microwave",
                "4 - EME/JT65",
                "5 - Low Band"
            });
            this.cbb_KST_Chat.Location = new Point(111, 136);
            this.cbb_KST_Chat.Name = "cbb_KST_Chat";
            this.cbb_KST_Chat.Size = new Size(121, 21);
            this.cbb_KST_Chat.TabIndex = 8;
            this.cbb_KST_Chat.Text = Settings.Default.KST_Chat;
            this.tb_KST_Password.DataBindings.Add(new Binding("Text", Settings.Default, "KST_Password", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_Password.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_KST_Password.Location = new Point(111, 110);
            this.tb_KST_Password.Name = "tb_KST_Password";
            this.tb_KST_Password.PasswordChar = '*';
            this.tb_KST_Password.Size = new Size(123, 20);
            this.tb_KST_Password.TabIndex = 7;
            this.tb_KST_Password.Text = Settings.Default.KST_Password;
            this.tb_KST_UserName.DataBindings.Add(new Binding("Text", Settings.Default, "KST_UserName", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_UserName.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_KST_UserName.Location = new Point(111, 83);
            this.tb_KST_UserName.Name = "tb_KST_UserName";
            this.tb_KST_UserName.Size = new Size(123, 20);
            this.tb_KST_UserName.TabIndex = 5;
            this.tb_KST_UserName.Text = Settings.Default.KST_UserName;
            this.tb_KST_ServerPort.DataBindings.Add(new Binding("Text", Settings.Default, "KST_ServerPort", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_ServerPort.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_KST_ServerPort.Location = new Point(111, 56);
            this.tb_KST_ServerPort.Name = "tb_KST_ServerPort";
            this.tb_KST_ServerPort.Size = new Size(56, 20);
            this.tb_KST_ServerPort.TabIndex = 3;
            this.tb_KST_ServerPort.Text = Settings.Default.KST_ServerPort;
            this.tb_KST_ServerName.DataBindings.Add(new Binding("Text", Settings.Default, "KST_ServerName", true, DataSourceUpdateMode.OnPropertyChanged));
            this.tb_KST_ServerName.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.tb_KST_ServerName.Location = new Point(111, 30);
            this.tb_KST_ServerName.Name = "tb_KST_ServerName";
            this.tb_KST_ServerName.Size = new Size(123, 20);
            this.tb_KST_ServerName.TabIndex = 1;
            this.tb_KST_ServerName.Text = Settings.Default.KST_ServerName;
            base.AcceptButton = this.btn_OK;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.CancelButton = this.btn_Cancel;
            base.ClientSize = new Size(606, 659);
            base.Controls.Add(this.groupBox5);
            base.Controls.Add(this.groupBox4);
            base.Controls.Add(this.groupBox3);
            base.Controls.Add(this.groupBox2);
            base.Controls.Add(this.btn_Cancel);
            base.Controls.Add(this.btn_OK);
            base.Controls.Add(this.groupBox1);
            base.Name = "OptionsDlg";
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            base.ResumeLayout(false);
        }
    }
}
