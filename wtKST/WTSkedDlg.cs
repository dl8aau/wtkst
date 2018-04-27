using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wtKST
{
    public partial class WTSkedDlg : Form
    {
        public string target_wt { get { return (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).from; } }
        public DateTime sked_time { get { return validated_sked_time; } }
        public int qrg { get { return (int)this.num_frequency.Value; } }
        public WinTest.WTBANDS band { get { return convert_band(this.cb_band.SelectedItem as MainDlg.bandinfo); }  }
        public WinTest.WTMODE mode { get { return this.cb_mode.SelectedItem.Equals("SSB") ? WinTest.WTMODE.ModeSSB : WinTest.WTMODE.ModeCW; } }
        public string call { get { return this.tb_Call.Text; } }
        public string notes { get { return this.tb_notes.Text; } }

        private DateTime current_time, validated_sked_time;

        public WTSkedDlg(String call, IList<WinTest.wtStatus.wtStat> wts,
            IList<MainDlg.bandinfo> band_info, string notes, int last_freq = 0)
        {
            InitializeComponent();
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);

            this.tb_Call.Text = call;

            this.cb_station.DataSource = wts;
            this.cb_station.DisplayMember = "from";
            this.cb_station.ValueMember = "from";

            this.cb_band.DataSource = band_info;
            this.cb_band.DisplayMember = "band_name";
            this.cb_band.ValueMember = "band_name";

            this.tb_notes.Text = notes;

            List<string> wtmode = new List<string>();

            wtmode.Add("SSB");
            wtmode.Add("CW");

            this.cb_mode.DataSource = wtmode;

            // use a good starting point for frequency - if not passed in constructor, use first station radio frequency
            if(last_freq == 0)
                this.num_frequency.Value = (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).freq / 1000;
            else
                this.num_frequency.Value = last_freq;

            DateTime dt = DateTime.UtcNow;
            TimeSpan d = TimeSpan.FromMinutes(1);
            // round to next full minute
            current_time = new DateTime(((dt.Ticks + d.Ticks - 1) / d.Ticks) * d.Ticks, DateTimeKind.Utc); ;

            this.mtb_time.Text = current_time.ToString("HH:mm");
            validated_sked_time = current_time;
        }

        private WinTest.WTBANDS convert_band(MainDlg.bandinfo band )
        {
            switch (band.band_name)
            {
                case "144MHz":  return WinTest.WTBANDS.Band144MHz;
                case "432MHz":  return WinTest.WTBANDS.Band432MHz;
                case "1296MHz": return WinTest.WTBANDS.Band1_2GHz;
                case "2320MHz": return WinTest.WTBANDS.Band2_3GHz;
                case "3400MHz": return WinTest.WTBANDS.Band3_4GHz;
                case "5760MHz": return WinTest.WTBANDS.Band5_7GHz;
                case "10GHz":   return WinTest.WTBANDS.Band10GHz;
                case "24GHz":   return WinTest.WTBANDS.Band24GHz;
                case "47GHz":   return WinTest.WTBANDS.Band47GHz;
                case "76GHz":   return WinTest.WTBANDS.Band76GHz;
            }
            return WinTest.WTBANDS.Band144MHz;
        }

        private void check_qsy_radio_qrg_selected()
        {
            // check if qsy and radio frequency are inside the selected band
            this.bt_qsyqrg.Enabled = ((this.cb_band.SelectedItem as MainDlg.bandinfo).in_band(
                (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).passfreq / 1000));
            this.bt_radioqrg.Enabled = ((this.cb_band.SelectedItem as MainDlg.bandinfo).in_band(
                (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).freq / 1000));
        }

        private void cb_band_SelectedIndexChanged(object sender, EventArgs e)
        {
            check_qsy_radio_qrg_selected();
            // check if no frequency selected, then try to find a better station
            if (!this.bt_radioqrg.Enabled && !this.bt_qsyqrg.Enabled)
            {
                // find the first matching station
                foreach (var cb_stat in cb_station.Items)
                {
                    if ( ((this.cb_band.SelectedItem as MainDlg.bandinfo).in_band(
                          (cb_stat as WinTest.wtStatus.wtStat).freq / 1000)) ||
                         ((this.cb_band.SelectedItem as MainDlg.bandinfo).in_band(
                          (cb_stat as WinTest.wtStatus.wtStat).passfreq / 1000)) )
                    {
                        cb_station.SelectedItem = cb_stat;
                        break;
                    }
                }
            }
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            object t = this.mtb_time.ValidateText();
            if (t == null)
                return;
            validated_sked_time = DateTime.SpecifyKind((DateTime)t, DateTimeKind.Utc);
            if (DateTime.Compare(current_time, validated_sked_time) > 0) // if before current local time, add one day
            {
                validated_sked_time = validated_sked_time.AddDays(1);
            }
            //Console.WriteLine("skedtime " + validated_sked_time);          
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cb_station_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.bt_qsyqrg.Text = "QSY freq " + (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).passfreq / 1000;
            this.bt_radioqrg.Text = "Radio freq " + (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).freq / 1000;
            check_qsy_radio_qrg_selected();
        }

        private void bt_qsyqrg_Click(object sender, EventArgs e)
        {
            this.num_frequency.Value = (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).passfreq / 1000;
        }

        private void bt_radioqrg_Click(object sender, EventArgs e)
        {
            this.num_frequency.Value = (this.cb_station.SelectedItem as WinTest.wtStatus.wtStat).freq / 1000;
        }

         private void mtb_time_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            // FIXME: more validation?
            this.bt_save.Enabled = e.IsValidInput;
        }
    }
}
