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
    public partial class ChatReview : Form
    {
        public ChatReview(DataView table, string call)
        {
            InitializeComponent();
            this.dataGridView1.DataSource = table;
            this.Text = call;
            // Message column should go full width
            this.dataGridView1.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // Time column does not need to display date..
            this.dataGridView1.Columns["Time"].DefaultCellStyle.Format = "HH:mm:ss";
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
            this.dataGridView1.AutoResizeColumns();
            //this.AutoSizeFormToDataGridView();
        }

        private void AutoSizeFormToDataGridView()
        {
            Size contentsSize = GetDataGridViewContentsSize();
            this.ClientSize = contentsSize;
        }

        protected Size GetDataGridViewContentsSize()
        {
            DockStyle dockStyleSave = dataGridView1.Dock;
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.AutoSize = true;

            Size dataContentsSize = dataGridView1.Size;

            dataGridView1.AutoSize = false;
            dataGridView1.Dock = dockStyleSave;
            return dataContentsSize;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // hack to make sure nothing is selected or can be selected...
            if (this.dataGridView1.SelectedCells.Count != 0)
                this.dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dataGridView1.Rows)
            {
                if (Myrow.Cells["From"].Value.ToString().Equals(this.Text)) // to or from Call
                {
                    // green 200
                    Myrow.DefaultCellStyle.BackColor = Color.FromArgb(0xFF, 0xA5, 0xD6, 0xA7);
                }
                else
                {
                    // green 50
                    Myrow.DefaultCellStyle.BackColor = Color.FromArgb(0xFF, 0xE8, 0xF5, 0xE9);
                }
            }
        }
    }
}
