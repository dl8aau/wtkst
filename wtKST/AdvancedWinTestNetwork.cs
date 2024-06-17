using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wtKST
{
    public class AdvancedWinTestNetwork : Form
    {
        public AdvancedWinTestNetwork()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chb_Advanced_WinTest_Network = new System.Windows.Forms.CheckBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            this.chb_Advanced_WinTest_Network.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(12, 48);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(100, 20);
            this.maskedTextBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Network Boradcast IP";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // maskedTextBox2
            // 
            this.maskedTextBox2.Location = new System.Drawing.Point(12, 121);
            this.maskedTextBox2.Name = "maskedTextBox2";
            this.maskedTextBox2.Size = new System.Drawing.Size(100, 20);
            this.maskedTextBox2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(146, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Network Broadcast UDP Port";
            // 
            // AdvancedWinTestNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 155);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maskedTextBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maskedTextBox1);
            this.Controls.Add(this.chb_Advanced_WinTest_Network);
            this.Name = "AdvancedWinTestNetwork";
            this.Text = "Advanced Win-Test Network Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox chb_Advanced_WinTest_Network;
        private System.Windows.Forms.MaskedTextBox maskedTextBox2;
        private System.Windows.Forms.Label label2;
    }
}
