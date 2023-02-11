using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
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

        // https://stackoverflow.com/a/38791349
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value == null) return;

            StringFormat sf = StringFormat.GenericDefault;
            sf.FormatFlags = sf.FormatFlags | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.DisplayFormatControl;
            e.PaintBackground(e.CellBounds, true);

            string text = e.Value.ToString();
            // https://stackoverflow.com/a/11708952 - strange hack... width is width(text+text) - width(text)
            SizeF textSize = e.Graphics.MeasureString(text, e.CellStyle.Font, e.CellBounds.Width, sf);
            float textHeight = textSize.Height;
            float textWidth = e.Graphics.MeasureString(text + text, e.CellStyle.Font, e.CellBounds.Width, sf).Width
                - textSize.Width;
            if (e.ColumnIndex == dataGridView1.Columns["Message"].Index)
            {
                // only deal with the second (message) column
                // find all matches of (nn.)nnn - at least 3 digits
                var match = Regex.Match(text, @"[0-9]*[\.,]*[0-9][0-9][0-9]+");

                if (match.Success)
                {
                    using (
                        Brush br = new SolidBrush(this.dataGridView1.ForeColor),
                              fillBrush = new SolidBrush(Color.Yellow))
                    {
                        int keyPos = match.Index;
                        float textMetricWidth = 0;
                        if (keyPos >= 1)
                        {
                            string textMetric = text.Substring(0, keyPos);
                            textMetricWidth = e.Graphics.MeasureString(textMetric + textMetric, e.CellStyle.Font, e.CellBounds.Width, sf).Width -
                                e.Graphics.MeasureString(textMetric, e.CellStyle.Font, e.CellBounds.Width, sf).Width;
                        }

                        SizeF keySize = e.Graphics.MeasureString(text.Substring(keyPos, match.Length) + text.Substring(keyPos, match.Length), e.CellStyle.Font, e.CellBounds.Width, sf) -
                            e.Graphics.MeasureString(text.Substring(keyPos, match.Length), e.CellStyle.Font, e.CellBounds.Width, sf);
                        float left = e.CellBounds.Left + (keyPos <= 0 ? 0 : textMetricWidth) + 2;
                        RectangleF keyRect = new RectangleF(left, e.CellBounds.Top + 1, keySize.Width, e.CellBounds.Height - 2);


                        e.Graphics.FillRectangle(fillBrush, keyRect);

                        e.Graphics.DrawString(text, e.CellStyle.Font, br, new PointF(e.CellBounds.Left + 2, e.CellBounds.Top + (e.CellBounds.Height - textHeight) / 2), sf);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
