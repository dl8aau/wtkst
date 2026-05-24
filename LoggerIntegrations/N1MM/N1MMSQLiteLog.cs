using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using WinTest;

namespace wtKST
{
    public class N1MMSQLiteLog : WinTestLogBase
    {
        public int ContestNR { get; set; } = -1;

        private string _dbPath = "";

        public N1MMSQLiteLog(WinTestLogBase.LogWriteMessageDelegate mylog) : base(mylog)
        {
            DataColumn[] keys = { QSO.Columns["CALL"], QSO.Columns["BAND"] };
            QSO.PrimaryKey = keys;
        }

        public override void Dispose() { }

        public override string getStatus()
        {
            return _dbPath + " ContestNR=" + ContestNR + " QSOs=" + QSO.Rows.Count;
        }

        public override void Get_QSOs(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath) || ContestNR < 0)
                return;

            _dbPath = dbPath;

            try
            {
                string connectionString = "Data Source=" + dbPath + ";Version=3;Read Only=True;";
                using (var conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    LogState = LOG_STATE.LOG_SYNCING;

                    using (var cmd = new SQLiteCommand(
                        "SELECT Call, Band, TS, SentNr, NR, GridSquare FROM DXLOG WHERE ContestNR = @nr",
                        conn))
                    {
                        cmd.Parameters.AddWithValue("@nr", ContestNR);
                        using (var reader = cmd.ExecuteReader())
                        {
                            lock (QSOlock)
                            {
                                QSO.Clear();
                            }
                            while (reader.Read())
                            {
                                string call = reader["Call"].ToString().Trim();
                                double bandMhz = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                                string band = BandFromMhz(bandMhz);
                                string ts = "";
                                if (!reader.IsDBNull(2))
                                {
                                    DateTime dt;
                                    if (DateTime.TryParse(reader["TS"].ToString(), out dt))
                                        ts = dt.ToString("HH:mm");
                                }
                                string sent = reader.IsDBNull(3) ? "" : reader.GetInt32(3).ToString();
                                string rcvd = reader.IsDBNull(4) ? "" : reader.GetInt32(4).ToString();
                                string loc = reader.IsDBNull(5) ? "" : reader["GridSquare"].ToString().Trim();

                                if (string.IsNullOrEmpty(call) || string.IsNullOrEmpty(band))
                                    continue;

                                DataRow row = QSO.NewRow();
                                row["CALL"] = call;
                                row["BAND"] = band;
                                row["TIME"] = ts;
                                row["SENT"] = sent;
                                row["RCVD"] = rcvd;
                                row["LOC"] = loc;

                                try
                                {
                                    lock (QSOlock)
                                    {
                                        if (QSO.Rows.Find(new object[] { call, band }) == null)
                                            QSO.Rows.Add(row);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Error("(" + call + "): " + ex.Message);
                                }
                            }
                        }
                    }
                    LogState = LOG_STATE.LOG_IN_SYNC;
                }
            }
            catch (Exception ex)
            {
                Error("(" + dbPath + "): " + ex.Message);
                LogState = LOG_STATE.LOG_INACTIVE;
            }
        }

        private static string BandFromMhz(double mhz)
        {
            if (mhz == 50.0) return "50M";
            if (mhz == 70.0) return "70M";
            if (mhz == 144.0) return "144M";
            if (mhz == 420.0) return "432M";
            if (mhz == 1240.0) return "1_2G";
            if (mhz == 2300.0) return "2_3G";
            if (mhz == 3300.0) return "3_4G";
            if (mhz == 5650.0) return "5_7G";
            if (mhz == 10000.0) return "10G";
            if (mhz == 24000.0) return "24G";
            if (mhz == 47000.0) return "47G";
            if (mhz == 76000.0) return "76G";
            return "";
        }

        public static List<ContestEntry> LoadContests(string dbPath)
        {
            var result = new List<ContestEntry>();
            string connectionString = "Data Source=" + dbPath + ";Version=3;Read Only=True;";
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT ContestNR, ContestName, StartDate FROM ContestInstance ORDER BY StartDate DESC",
                    conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int nr = reader.IsDBNull(0) ? -1 : reader.GetInt32(0);
                            string name = reader.IsDBNull(1) ? "" : reader["ContestName"].ToString();
                            string dateStr = reader.IsDBNull(2) ? "" : reader["StartDate"].ToString();
                            result.Add(new ContestEntry { ContestNR = nr, ContestName = name, StartDate = dateStr });
                        }
                    }
                }
            }
            return result;
        }

        public class ContestEntry
        {
            public int ContestNR;
            public string ContestName;
            public string StartDate;
            public override string ToString() => ContestName + " \u2014 " + StartDate;
        }
    }
}
