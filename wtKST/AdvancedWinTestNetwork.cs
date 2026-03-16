using System;
using System.Net;
using System.Windows.Forms;
using WinTest;
using wtKST.Properties;

namespace wtKST
{
    public class AdvancedWinTestNetwork : Form
    {
        public AdvancedWinTestNetwork()
        {
            InitializeComponent();
            InitializeBindings();
            SetDefaultValues();
            UpdateTextboxState();
            chb_Advanced_WinTest_Network.CheckedChanged += Chb_Advanced_WinTest_Network_CheckedChanged;
        }

        private void InitializeBindings()
        {
            this.chb_Advanced_WinTest_Network.DataBindings.Add(new Binding("Checked", global::wtKST.Properties.Settings.Default, "AdvancedWinTestNetwork_Activate", true, DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.DataBindings.Add(new Binding("Text", global::wtKST.Properties.Settings.Default, "AdvancedWinTestNetwork_BroadcastIP", true, DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.DataBindings.Add(new Binding("Text", global::wtKST.Properties.Settings.Default, "AdvancedWinTestNetwork_UDPPort", true, DataSourceUpdateMode.OnPropertyChanged));
        }

        private void SetDefaultValues()
        {
            if (string.IsNullOrEmpty(global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_BroadcastIP))
            {
                IPAddress defaultBroadcastIP = WinTest.WinTest.GetIpIFBroadcastAddress();
                global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_BroadcastIP = defaultBroadcastIP.ToString();
            }
            if (string.IsNullOrEmpty(global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_UDPPort))
            {
                int defaultUDPPort = WinTest.WinTest.WinTestDefaultPort;
                global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_UDPPort = defaultUDPPort.ToString();
            }
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (ValidateIPAddress(this.textBox1.Text) && ValidateUDPPort(this.textBox2.Text))
            {
                global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_Activate = this.chb_Advanced_WinTest_Network.Checked;
                global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_BroadcastIP = this.textBox1.Text;
                global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_UDPPort = this.textBox2.Text;
                global::wtKST.Properties.Settings.Default.Save();
                WinTest.WinTest.advancedWinTestPort = int.Parse(Settings.Default.AdvancedWinTestNetwork_UDPPort);
                WinTest.WinTest.advancedNetActivated = Settings.Default.AdvancedWinTestNetwork_Activate;
                WinTest.WinTest.advancedWinTestBroadcastAddress = IPAddress.Parse(Settings.Default.AdvancedWinTestNetwork_BroadcastIP);
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid IP address or UDP port.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateIPAddress(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

        private bool ValidateUDPPort(string port)
        {
            return int.TryParse(port, out int portNumber) && portNumber >= 0 && portNumber <= 65535;
        }

        private Button btn_reset;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.chb_Advanced_WinTest_Network = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_reset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Location = new System.Drawing.Point(187, 12);
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
            this.btn_Cancel.Location = new System.Drawing.Point(187, 41);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "&Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // chb_Advanced_WinTest_Network
            // 
            this.chb_Advanced_WinTest_Network.AutoSize = true;
            this.chb_Advanced_WinTest_Network.Location = new System.Drawing.Point(12, 12);
            this.chb_Advanced_WinTest_Network.Name = "chb_Advanced_WinTest_Network";
            this.chb_Advanced_WinTest_Network.Size = new System.Drawing.Size(65, 17);
            this.chb_Advanced_WinTest_Network.TabIndex = 0;
            this.chb_Advanced_WinTest_Network.Text = "Activate";
            this.chb_Advanced_WinTest_Network.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 48);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Win-Test Network Broadcast IP";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 90);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Win-Test UDP Port";
            // 
            // btn_reset
            // 
            this.btn_reset.Location = new System.Drawing.Point(187, 71);
            this.btn_reset.Name = "btn_reset";
            this.btn_reset.Size = new System.Drawing.Size(75, 23);
            this.btn_reset.TabIndex = 5;
            this.btn_reset.Text = "Default";
            this.btn_reset.UseVisualStyleBackColor = true;
            this.btn_reset.Click += new System.EventHandler(this.btn_reset_Click);
            // 
            // AdvancedWinTestNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 118);
            this.Controls.Add(this.btn_reset);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.chb_Advanced_WinTest_Network);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Name = "AdvancedWinTestNetwork";
            this.Text = "Advanced Win-Test Network Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox chb_Advanced_WinTest_Network;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;

        private void btn_reset_Click(object sender, EventArgs e)
        {
            IPAddress defaultBroadcastIP = WinTest.WinTest.GetIpIFBroadcastAddress();
            int defaultUDPPort = WinTest.WinTest.WinTestDefaultPort;
            global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_Activate = false;
            global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_BroadcastIP = defaultBroadcastIP.ToString();
            global::wtKST.Properties.Settings.Default.AdvancedWinTestNetwork_UDPPort = defaultUDPPort.ToString();
            global::wtKST.Properties.Settings.Default.Save();
            WinTest.WinTest.advancedWinTestPort = int.Parse(Settings.Default.AdvancedWinTestNetwork_UDPPort);
            WinTest.WinTest.advancedNetActivated = Settings.Default.AdvancedWinTestNetwork_Activate;
            WinTest.WinTest.advancedWinTestBroadcastAddress = IPAddress.Parse(Settings.Default.AdvancedWinTestNetwork_BroadcastIP);
            UpdateTextboxState();
        }

        private void Chb_Advanced_WinTest_Network_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTextboxState();
        }

        private void UpdateTextboxState()
        {
            bool isActive = chb_Advanced_WinTest_Network.Checked;
            textBox1.Enabled = isActive;
            textBox2.Enabled = isActive;
        }
    }
}
