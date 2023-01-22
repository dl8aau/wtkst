using System;

namespace wtKST
{
    partial class WTSkedDlg
    {
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_Call = new System.Windows.Forms.TextBox();
            this.cb_station = new System.Windows.Forms.ComboBox();
            this.bt_save = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_band = new System.Windows.Forms.ComboBox();
            this.cb_mode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.bt_qsyqrg = new System.Windows.Forms.Button();
            this.bt_radioqrg = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_notes = new System.Windows.Forms.TextBox();
            this.num_frequency = new System.Windows.Forms.NumericUpDown();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.mtb_time = new System.Windows.Forms.MaskedTextBox();
            this.bt_kstqrg = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.num_frequency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Callsign:";
            // 
            // tb_Call
            // 
            this.tb_Call.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tb_Call.Location = new System.Drawing.Point(69, 6);
            this.tb_Call.Name = "tb_Call";
            this.tb_Call.Size = new System.Drawing.Size(100, 20);
            this.tb_Call.TabIndex = 1;
            // 
            // cb_station
            // 
            this.cb_station.FormattingEnabled = true;
            this.cb_station.Location = new System.Drawing.Point(285, 80);
            this.cb_station.Name = "cb_station";
            this.cb_station.Size = new System.Drawing.Size(121, 21);
            this.cb_station.TabIndex = 7;
            this.cb_station.SelectedIndexChanged += new System.EventHandler(this.cb_station_SelectedIndexChanged);
            // 
            // bt_save
            // 
            this.bt_save.Location = new System.Drawing.Point(325, 167);
            this.bt_save.Name = "bt_save";
            this.bt_save.Size = new System.Drawing.Size(75, 23);
            this.bt_save.TabIndex = 11;
            this.bt_save.Text = "Save";
            this.bt_save.UseVisualStyleBackColor = true;
            this.bt_save.Click += new System.EventHandler(this.bt_save_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Frequency:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(176, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Time:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Band:";
            // 
            // cb_band
            // 
            this.cb_band.FormattingEnabled = true;
            this.cb_band.Location = new System.Drawing.Point(55, 80);
            this.cb_band.Name = "cb_band";
            this.cb_band.Size = new System.Drawing.Size(70, 21);
            this.cb_band.TabIndex = 5;
            this.cb_band.SelectedIndexChanged += new System.EventHandler(this.cb_band_SelectedIndexChanged);
            // 
            // cb_mode
            // 
            this.cb_mode.FormattingEnabled = true;
            this.cb_mode.Location = new System.Drawing.Point(174, 80);
            this.cb_mode.Name = "cb_mode";
            this.cb_mode.Size = new System.Drawing.Size(56, 21);
            this.cb_mode.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(131, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Mode:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(236, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Station:";
            // 
            // bt_qsyqrg
            // 
            this.bt_qsyqrg.Location = new System.Drawing.Point(16, 123);
            this.bt_qsyqrg.Name = "bt_qsyqrg";
            this.bt_qsyqrg.Size = new System.Drawing.Size(130, 23);
            this.bt_qsyqrg.TabIndex = 8;
            this.bt_qsyqrg.Text = "QSY freq = 432250.0";
            this.bt_qsyqrg.UseVisualStyleBackColor = true;
            this.bt_qsyqrg.Click += new System.EventHandler(this.bt_qsyqrg_Click);
            // 
            // bt_radioqrg
            // 
            this.bt_radioqrg.Location = new System.Drawing.Point(152, 123);
            this.bt_radioqrg.Name = "bt_radioqrg";
            this.bt_radioqrg.Size = new System.Drawing.Size(116, 23);
            this.bt_radioqrg.TabIndex = 9;
            this.bt_radioqrg.Text = "Radio 1 = 432261.0";
            this.bt_radioqrg.UseVisualStyleBackColor = true;
            this.bt_radioqrg.Click += new System.EventHandler(this.bt_radioqrg_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(176, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Notes:";
            // 
            // tb_notes
            // 
            this.tb_notes.Location = new System.Drawing.Point(217, 37);
            this.tb_notes.Name = "tb_notes";
            this.tb_notes.Size = new System.Drawing.Size(189, 20);
            this.tb_notes.TabIndex = 4;
            // 
            // num_frequency
            // 
            this.num_frequency.Location = new System.Drawing.Point(69, 37);
            this.num_frequency.Maximum = new decimal(new int[] {
            500000000,
            0,
            0,
            0});
            this.num_frequency.Name = "num_frequency";
            this.num_frequency.Size = new System.Drawing.Size(101, 20);
            this.num_frequency.TabIndex = 3;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // mtb_time
            // 
            this.mtb_time.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.mtb_time.Location = new System.Drawing.Point(217, 6);
            this.mtb_time.Mask = "90:00";
            this.mtb_time.Name = "mtb_time";
            this.mtb_time.Size = new System.Drawing.Size(33, 20);
            this.mtb_time.TabIndex = 2;
            this.mtb_time.ValidatingType = typeof(System.DateTime);
            this.mtb_time.TypeValidationCompleted += new System.Windows.Forms.TypeValidationEventHandler(this.mtb_time_TypeValidationCompleted);
            // 
            // bt_kstqrg
            // 
            this.bt_kstqrg.Location = new System.Drawing.Point(274, 123);
            this.bt_kstqrg.Name = "bt_kstqrg";
            this.bt_kstqrg.Size = new System.Drawing.Size(126, 23);
            this.bt_kstqrg.TabIndex = 10;
            this.bt_kstqrg.Text = "KST = 432261.0";
            this.bt_kstqrg.UseVisualStyleBackColor = true;
            this.bt_kstqrg.Click += new System.EventHandler(this.bt_kst_qrg_Click);
            // 
            // WTSkedDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 203);
            this.Controls.Add(this.bt_kstqrg);
            this.Controls.Add(this.mtb_time);
            this.Controls.Add(this.num_frequency);
            this.Controls.Add(this.tb_notes);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.bt_radioqrg);
            this.Controls.Add(this.bt_qsyqrg);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cb_mode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cb_band);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bt_save);
            this.Controls.Add(this.cb_station);
            this.Controls.Add(this.tb_Call);
            this.Controls.Add(this.label1);
            this.Name = "WTSkedDlg";
            this.Text = "WTSkedDialog";
            ((System.ComponentModel.ISupportInitialize)(this.num_frequency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_Call;
        private System.Windows.Forms.ComboBox cb_station;
        private System.Windows.Forms.Button bt_save;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_band;
        private System.Windows.Forms.ComboBox cb_mode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button bt_qsyqrg;
        private System.Windows.Forms.Button bt_radioqrg;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_notes;
        private System.Windows.Forms.NumericUpDown num_frequency;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.MaskedTextBox mtb_time;
        private System.Windows.Forms.Button bt_kstqrg;
    }
}