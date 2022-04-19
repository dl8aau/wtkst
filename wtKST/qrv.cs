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
            unknown = 0, qrv = 1, not_qrv = 3,
            worked = 8      // power of 2, so it can be added
        }

        private string[] BANDS;

        private DataTable QRV_local = new DataTable("QRV_local");

        public void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
        }

        public void Process_QRV(DataRow row, string qrvcall, bool call_new_in_userlist = false)
        {
            bool call_in_stationDB = false;
            List<QRVDesignator> QRVlist = StationData.Database.QRVFind(qrvcall, row["LOC"].ToString());
            if (QRVlist != null)
            {
                call_in_stationDB = true;
                foreach (string band in BANDS)
                {
                    row[band] = QRV_STATE.unknown;
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
            }
            // look into our local DB
            DataRow findrow = QRV_local.Rows.Find(new object[] {qrvcall, row["LOC"] });
            if (findrow == null)
            {
                // need a new entry
                DataRow newrow = QRV_local.NewRow();
                newrow["CALL"] = qrvcall;
                newrow["LOC"] = row["LOC"];
                foreach (string band in BANDS)
                    newrow[band] = QRV_STATE.unknown;
                try
                {
                    QRV_local.Rows.Add(newrow);
                    findrow = newrow;
                }
                catch (Exception e)
                {
                    Error(MethodBase.GetCurrentMethod().Name, e.Message);
                }
            }

            if (call_in_stationDB)
            {
                // merge only if set
                foreach (string band in BANDS)
                {
                    if ((QRVdb.QRV_STATE)findrow[band] != QRV_STATE.unknown)
                        row[band] = findrow[band];
                }
            }
            else
            {
                foreach (string band in BANDS)
                    row[band] = findrow[band];
            }

            if (call_new_in_userlist)
            {
                // if we have not just started (oldCALL empty) treat connecting
                // to KST as activity
                row["TIME"] = DateTime.UtcNow;
            }
        }

        public void set_qrv_state(DataRow Row, string band, QRV_STATE state)
        {
            DataRow qrv_row = QRV_local.Rows.Find(new object[] { Row["CALL"], Row["LOC"] });
            if (qrv_row != null)
            {
                if (state == QRV_STATE.worked)
                    qrv_row[band] = QRV_STATE.qrv; // if worked, then obviously qrv
                else
                    qrv_row[band] = state;
            }
        }

        public static bool worked(QRV_STATE qrv_state)
        {
            return ((int)qrv_state & (int)QRV_STATE.worked) == (int)QRV_STATE.worked;
        }

        public static bool not_qrv(QRV_STATE qrv_state)
        {
            return ((int)qrv_state & ~(int)QRV_STATE.worked) == (int)QRV_STATE.not_qrv;
        }

        public static bool worked_or_not_qrv(QRV_STATE qrv_state)
        {
            return worked(qrv_state) && not_qrv(qrv_state);
        }

        public static QRV_STATE set_worked(QRV_STATE qrv_state, bool worked)
        {
            if (worked)
                return qrv_state | QRV_STATE.worked;
            return qrv_state & ~QRV_STATE.worked;
        }
        private void InitializeQRV()
        {
            QRV_local.Clear();
            try
            {
                string QRV_Table_Filename = Path.Combine(Directory.GetParent(Application.LocalUserAppDataPath).ToString(),
                    Settings.Default.QRV_Local_Table_FileName);
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
                        Error(MethodBase.GetCurrentMethod().Name, "(qrv_local.xml): " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Error(MethodBase.GetCurrentMethod().Name, ": " + e.Message + "\n" + e.StackTrace);
            }
        }

        public void save_db()
        {
            string FileName = Path.Combine(Directory.GetParent(Application.LocalUserAppDataPath).ToString(),
                Settings.Default.QRV_Local_Table_FileName);
            DataView qrv_local_dv = new DataView(QRV_local);
            string rowfilter = "";
            foreach (string band in BANDS)
            {
                if (string.IsNullOrEmpty(rowfilter))
                    rowfilter = string.Format("([{0}] = {1}) OR ([{0}] = {2})", band, (int)QRV_STATE.qrv, (int)QRV_STATE.not_qrv);
                else
                    rowfilter += string.Format(" OR ([{0}] = {1}) OR ([{0}] = {2})", band, (int)QRV_STATE.qrv, (int)QRV_STATE.not_qrv);
            }
            qrv_local_dv.RowFilter = rowfilter;
            if (qrv_local_dv.Count > 0)
            {
                var dt = qrv_local_dv.ToTable();
                dt.WriteXml(FileName, XmlWriteMode.WriteSchema);
            }
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
