using System.Data;
using System.IO;
using WinTest;

namespace wtKST
{
    public class ADIFContestLog : WinTestLogBase
    {
        public ADIFContestLog(LogWriteMessageDelegate mylog) : base(mylog)
        {
            DataColumn[] keys = { QSO.Columns["CALL"], QSO.Columns["BAND"] };
            QSO.PrimaryKey = keys;
        }

        public override void Dispose() { }

        public override string getStatus()
        {
            return "ADIF contest: " + QSO.Rows.Count + " QSOs";
        }

        public override void Get_QSOs(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                LogState = LOG_STATE.LOG_INACTIVE;
                return;
            }

            LogState = LOG_STATE.LOG_SYNCING;

            var records = ADIFLog.Parse(filePath);

            lock (QSOlock)
            {
                QSO.Clear();
                foreach (var rec in records)
                {
                    DataRow row = QSO.NewRow();
                    row["CALL"] = rec.Call;
                    row["BAND"] = rec.Band;
                    row["TIME"] = rec.Time;
                    row["SENT"] = rec.Sent;
                    row["RCVD"] = rec.Rcvd;
                    row["LOC"]  = rec.Loc;
                    try { QSO.Rows.Add(row); }
                    catch { } // skip duplicate CALL+BAND
                }
            }

            LogState = LOG_STATE.LOG_IN_SYNC;
        }
    }
}
