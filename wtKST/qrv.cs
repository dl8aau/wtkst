using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ScoutBase.Core;
using ScoutBase.Stations;
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

        private DataTable QRV_local = new DataTable("QRV_local");

        public void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
        }

        public void Process_QRV(DataRow row, string qrvcall, bool call_new_in_userlist = false)
        {
            List<QRVDesignator> QRVlist = StationData.Database.QRVFind(qrvcall, row["LOC"].ToString());
            if (QRVlist != null)
            {
                foreach (string band in BANDS)
                {
                    row[band] = QRV_STATE.not_qrv;
                }
                foreach (var QRV in QRVlist)
                {
                    // "144M", "432M", "1_2G", "2_3G", "3_4G", "5_7G", "10G", "24G", "47G", "76G"
                    switch (QRV.Band)
                    {
                        case BAND.B144M:
                            row["144M"] = QRV_STATE.qrv;
                            break;
                        case BAND.B432M:
                            row["432M"] = QRV_STATE.qrv;
                            break;
                        case BAND.B1_2G:
                            row["1_2G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B2_3G:
                            row["2_3G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B3_4G:
                            row["3_4G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B5_7G:
                            row["5_7G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B10G:
                            row["10G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B24G:
                            row["24G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B47G:
                            row["47G"] = QRV_STATE.qrv;
                            break;
                        case BAND.B76G:
                            row["76G"] = QRV_STATE.qrv;
                            break;
                    }
                }
                if (call_new_in_userlist)
                {
                    // if we have not just started (oldCALL empty) treat connecting
                    // to KST as activity
                    row["TIME"] = row["LOGINTIME"];
                    //FIXME                    findrow["TIME"] = row["LOGINTIME"];
                }
                else
                    row["TIME"] = row["LOGINTIME"]; // FIXME wrong!!! Should be last time they wrote anything   StationData.Database.QRVFindLastUpdated(QRV);
            }
            else
            {
                // we have to look into our local DB, as it is not in the global one
                DataRow findrow = QRV_local.Rows.Find(new object[] { row["CALL"], row["LOC"] });
                if (findrow != null)
                {
                    if (call_new_in_userlist)
                    {
                        // if we have not just started (oldCALL empty) treat connecting
                        // to KST as activity
                        row["TIME"] = row["LOGINTIME"];
                    } else
                        row["TIME"] = DateTime.MinValue; // FIXME!

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

                    DataRow newrow = QRV_local.NewRow();
                    newrow["CALL"] = qrvcall;
                    newrow["LOC"] = row["LOC"];
                    foreach (string band in BANDS)
                        newrow[band] = QRV_STATE.unknown;
                    try
                    {
                        QRV_local.Rows.Add(newrow);
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, e.Message);
                    }
                }
            }
        }

        public void set_qrv_state(DataRow Row, string band, QRV_STATE state)
        {
            DataRow qrv_row = QRV_local.Rows.Find(new object[] { Row["CALL"], Row["LOC"] });
            if (qrv_row != null)
            {
                qrv_row[band] = state;
            }
        }

        private void InitializeQRV()
        {
            QRV_local.Clear();
            try
            {
                string QRV_Table_Filename = Path.Combine(Directory.GetParent(Application.LocalUserAppDataPath).ToString(),
                    "qrv_local.xml");
                TimeSpan ts = DateTime.Now - File.GetLastWriteTime(QRV_Table_Filename);
                if (File.Exists(QRV_Table_Filename) && ts.Hours < 48)
                {
                    try
                    {
                        QRV_local.BeginLoadData();
                        QRV_local.ReadXml(QRV_Table_Filename);
                        QRV_local.EndLoadData();
                    }
                    catch (Exception e)
                    {
                        Error(MethodBase.GetCurrentMethod().Name, "(QRV.xml): " + e.Message);
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
                "qrv_local.xml");
            QRV_local.WriteXml(FileName, XmlWriteMode.WriteSchema);
        }

        public QRVdb(string[] BANDS)
        {
            QRV_local.Columns.Add("CALL");
            QRV_local.Columns.Add("LOC");
            this.BANDS = BANDS;
            foreach (string band in BANDS)
                QRV_local.Columns.Add(band, typeof(int));
            DataColumn[] QRVkeys = new DataColumn[]
            {
                QRV_local.Columns["CALL"],
                QRV_local.Columns["LOC"]
            };
            QRV_local.PrimaryKey = QRVkeys;
            InitializeQRV();
        }

    }
}
