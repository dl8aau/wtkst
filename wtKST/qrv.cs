using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using wtKST.Properties;

namespace wtKST
{
    class QRVdb
    {
        public enum QRV_STATE : int
        {
            unknown = 0, qrv = 1, worked = 2, not_qrv = 3
        }

        private string[] BANDS;

        private DataTable QRV = new DataTable("QRV");

        public void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
        }

        public void Process_QRV(DataRow row, string qrvcall, bool call_new_in_userlist = false)
        {
            DataRow findrow = QRV.Rows.Find(qrvcall);
            if (findrow != null)
            {
                if (call_new_in_userlist)
                {
                    // if we have not just started (oldCALL empty) treat connecting
                    // to KST as activity
                    row["TIME"] = row["LOGINTIME"];
                    findrow["TIME"] = row["LOGINTIME"];
                }
                else
                    row["TIME"] = findrow["TIME"];
                foreach (string band in BANDS)
                    row[band] = findrow[band];
            }
            else
            {
                if (call_new_in_userlist)
                    row["TIME"] = row["LOGINTIME"];
                else
                    row["TIME"] = DateTime.MinValue;
                foreach (string band in BANDS)
                    row[band] = QRV_STATE.unknown;
                DataRow newrow = QRV.NewRow();
                newrow["CALL"] = qrvcall;
                if (call_new_in_userlist)
                    newrow["TIME"] = row["LOGINTIME"];
                else
                    newrow["TIME"] = DateTime.MinValue;
                foreach (string band in BANDS)
                    newrow[band] = QRV_STATE.unknown;
                try
                {
                    QRV.Rows.Add(newrow);
                }
                catch (Exception e)
                {
                    Error(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }
        }

        public void set_qrv_state(string call, string band, QRV_STATE state)
        {
            DataRow Row = QRV.Rows.Find(call);
            if (Row != null)
            {
                Row[band] = state;
            }
        }

        private void InitializeQRV()
        {
            QRV.Clear();
            try
            {
                string QRV_Table_Filename = Path.Combine(Directory.GetParent(Application.LocalUserAppDataPath).ToString(),
                    Settings.Default.WinTest_QRV_Table_FileName);
                TimeSpan ts = DateTime.Now - File.GetLastWriteTime(QRV_Table_Filename);
                if (File.Exists(QRV_Table_Filename) && ts.Hours < 48)
                {
                    try
                    {
                        QRV.BeginLoadData();
                        QRV.ReadXml(QRV_Table_Filename);
                        QRV.EndLoadData();
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml): " + e.Message);
                    }
                }
                // if we cannot read qrv.xml from appdata path, try current directoy (=previous default)
                if (QRV.Rows.Count == 0 && File.Exists(Settings.Default.WinTest_QRV_Table_FileName))
                {
                    try
                    {
                        QRV.BeginLoadData();
                        QRV.ReadXml(Settings.Default.WinTest_QRV_Table_FileName);
                        QRV.EndLoadData();
                    }
                    catch (Exception e)
                    {
                            Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml in program dir): " + e.Message + "\n" + e.StackTrace);
                    }
                }
                if (QRV.Rows.Count == 0)
                {
                    using (StreamReader sr = new StreamReader(Settings.Default.WinTest_QRV_FileName))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            DataRow Row = QRV.NewRow();
                            Row["CALL"] = line.Split(new char[0])[0];
                            Row["TIME"] = DateTime.Parse(line.Split(new char[0])[1].TrimStart(
                                new char[] { '[' }).TrimEnd(new char[] { ']' }) + " 00:00:00");
                            foreach (string band in BANDS)
                            {
                                if (line.Replace(".", "_").IndexOf(band) > 0)
                                {
                                    Row[band] = QRV_STATE.qrv;
                                }
                                else
                                {
                                    Row[band] = QRV_STATE.unknown;
                                }
                            }
                            QRV.Rows.Add(Row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, "(" + Settings.Default.WinTest_QRV_FileName + "): " + e.Message + "\n" + e.StackTrace);
            }
        }

        public void save_db()
        {
            string FileName = Path.Combine(Directory.GetParent(Application.LocalUserAppDataPath).ToString(), 
                Settings.Default.WinTest_QRV_Table_FileName);
            QRV.WriteXml(FileName, XmlWriteMode.IgnoreSchema);
        }

        public void set_time(string call, DateTime dt)
        {
            DataRow findrow = QRV.Rows.Find(call);
            // TODO: what if user is not in user list yet? time not updated then
            // -> count appearing on user list as "activity", too
            if (findrow != null)
            {
                if ((DateTime)findrow["TIME"] < dt)
                    findrow["TIME"] = dt;
            }
        }

        public QRVdb(string[] BANDS)
        {
            QRV.Columns.Add("CALL");
            QRV.Columns.Add("TIME", typeof(DateTime));
            this.BANDS = BANDS;
            foreach (string band in BANDS)
                QRV.Columns.Add(band, typeof(int));
            DataColumn[] QRVkeys = new DataColumn[]
            {
                QRV.Columns["CALL"]
            };
            QRV.PrimaryKey = QRVkeys;
            InitializeQRV();
        }

    }
}
