using ScoutBase.Stations;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using wtKST.Properties;

namespace wtKST
{
    public class QRVdb
    {
        public enum QRV_STATE : int
        {
            unknown = 0, qrv = 1, not_qrv = 3,
            worked = 8      // power of 2, so it can be added
        }

        private string[] BANDS;

        private DataTable QRV_local = new DataTable("QRV_local");
        private bool QRVlocalChanged = false;

        public void Error(string methodname, string Text)
        {
            MainDlg.Log.WriteMessage("Error - <" + methodname + "> " + Text);
        }

        /// <summary>
        /// Complete the information in row from station database, nameInfo string or local database
        /// </summary>
        /// <param name="row">DataRow that contains entries for BANDS</param>
        /// <param name="qrvcall">Callsign used for database queries</param>
        /// <param name="nameInfo">nameInfo string from user list</param>
        /// <param name="call_new_in_userlist">true if user is a new entry in the user list</param>
        public void Process_QRV(DataRow row, string qrvcall, string nameInfo, bool call_new_in_userlist = false)
        {
            string qrvcallBase = qrvcall;
            if (qrvcallBase.IndexOf("-") > 0)
            {
                qrvcallBase = qrvcallBase.Remove(qrvcallBase.IndexOf("-"));
            }
            bool call_in_stationDB = false;
            List<QRVDesignator> QRVlist = null;
            try
            {
                QRVlist = StationData.Database.QRVFind(qrvcallBase, row["LOC"].ToString());
            }
            catch (Exception ex)
            {
                Error(MethodBase.GetCurrentMethod().Name, "Scoutbase " + ex.Message);
            }
            if (QRVlist != null)
            {
                call_in_stationDB = true;
                foreach (string band in BANDS)
                {
                    row[band] = QRV_STATE.unknown;
                }
                foreach (var QRV in QRVlist)
                {
                    // match the bands in "our" BANDS property
                    var matchingBand = BANDS
                        .Where(element => element.Equals(ScoutBase.Core.Bands.GetStringValue(QRV.Band).Replace(".", "_")));
                    if (matchingBand.Count()>0)
                    {
                        row[matchingBand.First()] = QRV_STATE.qrv;
                    }
                }
            }
            // look into our local DB
            DataRow findrow = QRV_local.Rows.Find(new object[] { qrvcall, row["LOC"] });
            if (findrow == null)
            {
                // check if we have a hit for the base entry
                findrow = QRV_local.Rows.Find(new object[] { qrvcallBase, row["LOC"] });
                DataRow newrow = QRV_local.NewRow();
                if (findrow != null)
                {
                    // propagate base to specific one
                    // copy entry
                    newrow.ItemArray = findrow.ItemArray.Clone() as object[];
                    newrow["CALL"] = qrvcall;
                }
                else
                {
                    // need a new entry
                    newrow["CALL"] = qrvcall;
                    newrow["LOC"] = row["LOC"];
                    foreach (string band in BANDS)
                        newrow[band] = QRV_STATE.unknown;
                }
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
                    Object f = findrow[band];
                    if (f != null && f.GetType() != typeof(DBNull) && (QRVdb.QRV_STATE)f != QRV_STATE.unknown)
                        row[band] = findrow[band];
                }
            }
            else
            {
                foreach (string band in BANDS)
                {
                    if (!findrow.IsNull(band))
                        row[band] = findrow[band];
                    else
                        row[band] = QRV_STATE.unknown;
                }
            }

            if (call_new_in_userlist)
            {
                // if we have not just started (oldCALL empty) treat connecting
                // to KST as activity
                row["TIME"] = DateTime.UtcNow;
            }
        }
        /*
         * Methods to process the "nameInfo" part of the user list
         * 
         * try to deduce the bands the user is qrv
         */
        /// <summary>
        /// check "nameInfo" string for not QRV information 
        /// </summary>
        /// <param name="nameInfo"></param>
        /// <returns>return true if user signals not to be qrv</returns>
        private static bool NameInfoNotQrv(string nameInfo)
        {
            return (nameInfo.ToLower().Contains("not qrv") || nameInfo.ToLower().Contains("swl"));
        }

        /// <summary>
        /// Generate a list of bands given first and last
        /// </summary>
        /// <param name="bandFrom"></param>
        /// <param name="bandTo"></param>
        /// <param name="bandList"></param>
        private static void BandListRange(BAND bandFrom, BAND bandTo, ref List<BAND> bandList)
        {
            var bandValues = Enum.GetValues(typeof(BAND)).Cast<BAND>();
            foreach (var b in bandValues.Where(b => b >= bandFrom && b <= bandTo))
            {
                bandList.Add(b);
            }
        }

        /// <summary>
        /// Tries to estimate a range of bands indicated as being qrv
        /// </summary>
        /// <param name="nameInfo"></param>
        /// <param name="bandFromText"></param>
        /// <param name="bandToText"></param>
        /// <returns>true if range could be found</returns>
        private static bool NameInfoRangeToBands(string nameInfo, out List<BAND> bandList)
        {
            bandList = new List<BAND>(); 
            
            Regex r = new Regex("^[^0-9.,]*(\\d*[.,]?\\d+)+(?:c?m?|M?G?Hz) ?(?:[->]{1,2}|to) *(\\d+[.,]?\\d*|[.,]?\\d+)+\\D*$");

            Match m = r.Match(nameInfo);
            if (m.Success)
            {
                if (m.Groups.Count == 3)
                {
                    string bandFromText = m.Groups[1].Value;
                    string bandToText = m.Groups[2].Value;
                    var bandFrom = BandMatchTypeFromText(bandFromText);
                    var bandTo = BandMatchTypeFromText(bandToText);

                    // a few strings can only be matched with more context...
                    if (bandFrom.Item2 == BAND_MATCH_TYPE.UNCLEAR)
                    {
                        if (bandTo.Item2 == BAND_MATCH_TYPE.FREQUENCY_GHZ)
                        {
                            if (bandFromText == "1.2" || bandFromText == "1,2")
                            {
                                // probably 23cm, otherwise it would need to be WAVELENGTH
                                BandListRange(BAND.B1_2G, bandTo.Item1, ref bandList);
                                return true;
                            }
                        }
                    }

                    // sanity check
                    if (bandFrom.Item1 < bandTo.Item1)
                    {
                        BandListRange(bandFrom.Item1, bandTo.Item1, ref bandList);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tries to match one single band
        /// </summary>
        /// <param name="nameInfo"></param>
        /// <param name="bandText"></param>
        /// <returns>true if a single band is found</returns>
        private static bool NameInfoSingleBand(string nameInfo, out BAND band)
        {
            // https://regex101.com/r/a6h8Pg/2
            // don't allow "," - otherwise too mane false hits
            Regex r = new Regex("^[^0-9.,]*(\\d*[.]?\\d+)(?:c?m?|M?G?Hz)+(?(?=(?:\\D*\\d+W|\\D{5,})).*|\\D*)$");

            Match m = r.Match(nameInfo);
            if (m.Success  && m.Groups.Count==2)
            {
                band = BandFromText(m.Groups[1].Value);
                return true;
            }
            band = BAND.BNONE;
            return false;
        }

        /// <summary>
        /// given nameInfo try to match a list of bands user is qrv
        /// </summary>
        /// <param name="nameInfo"></param>
        /// <param name="bandList"></param>
        /// <returns></returns>
        private static bool NameInfoMultipleBands(string nameInfo, out List<BAND> bandList)
        {
            // https://regex101.com/r/Iw9Ylu/3
            Regex r = new Regex("^[^\\d.,]*(?(?=.*\\d[/ +-]+|and)((\\d+[,.]?\\d*)(?:c?m?|M?G?Hz)*(?:[/ +-]+|and)+)+(\\d+[,.]?\\d*)|((\\d+[.]?\\d*)(?:c?m?|M?G?Hz)*(?:[,/ +-]+|and)+)+(\\d+[.]?\\d*))(?:c?m?|M?G?Hz)*");

            Match m = r.Match(nameInfo);
            bandList = new List<BAND>();
            if (m.Success && m.Groups.Count == 7)
            {
                (BAND, BAND_MATCH_TYPE) bandTuple;
                int firstIndex;
                if (m.Groups[1].Captures.Count > 0)
                    firstIndex = 1;
                else
                    firstIndex = 4;

                for (int i = 0; i < m.Groups[firstIndex].Captures.Count; i++)
                    {
                        bandTuple = BandMatchTypeFromText(m.Groups[firstIndex].Captures[i].Value);
                        if (bandTuple.Item1 != BAND.BNONE)
                            bandList.Add(bandTuple.Item1);
                    }
                bandTuple = BandMatchTypeFromText(m.Groups[firstIndex+1].Value);
                if (bandTuple.Item1 != BAND.BNONE)
                    bandList.Add(bandTuple.Item1);

                return true;
            }
            bandList.Add(BAND.BNONE);
            return false;
        }

        /// <summary>
        /// Matches a band from a string including the type of the band information (e.g. MHz or meter or unclear if ambigious)
        /// </summary>
        /// <param name="bandtext"></param>
        /// <returns>tuple BAND and BAND_MATCH_TYPE</returns>
        private static (BAND, BAND_MATCH_TYPE) BandMatchTypeFromText(string bandtext)
        {
            (BAND, BAND_MATCH_TYPE) band = (BAND.BNONE, BAND_MATCH_TYPE.UNCLEAR);
            bandNameToBand.TryGetValue(bandtext, out band);
            return band;
        }

        /// <summary>
        /// Matches a band from a string
        /// </summary>
        /// <param name="bandtext"></param>
        /// <returns></returns>
        private static BAND BandFromText(string bandtext)
        {
            (BAND, BAND_MATCH_TYPE) band = (BAND.BNONE, BAND_MATCH_TYPE.UNCLEAR);
            if (!bandNameToBand.TryGetValue(bandtext, out band))
            {
                if (bandtext.Contains("."))
                    bandNameToBand.TryGetValue(bandtext.Split('.')[0], out band);
            }
            return band.Item1;
        }

        private enum BAND_MATCH_TYPE { UNCLEAR, WAVELENGTH, FREQUENCY_GHZ, FREQUENCY_MHZ };

        private static Dictionary<string, (BAND, BAND_MATCH_TYPE) > bandNameToBand = new Dictionary<string, (BAND, BAND_MATCH_TYPE) >
        {
            { "50", (BAND.B50M, BAND_MATCH_TYPE.FREQUENCY_MHZ) },
            { "144", (BAND.B144M, BAND_MATCH_TYPE.FREQUENCY_MHZ) }, { "2", (BAND.B144M, BAND_MATCH_TYPE.WAVELENGTH) },
            { ".4", (BAND.B432M, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, {"432", (BAND.B432M, BAND_MATCH_TYPE.FREQUENCY_MHZ) }, {"70", (BAND.B432M, BAND_MATCH_TYPE.WAVELENGTH) },
            /* { "1.2", BAND.B1_2G }, { "1,2", BAND.B1_2G },  1.2 / 1,2 without "cm" unclear, also used for 24 GHz */ 
            { "1296", (BAND.B1_2G, BAND_MATCH_TYPE.FREQUENCY_MHZ) }, { "1.3", (BAND.B1_2G, BAND_MATCH_TYPE.FREQUENCY_GHZ) },
                {"1,3", (BAND.B1_2G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, {"23", (BAND.B1_2G, BAND_MATCH_TYPE.WAVELENGTH)},
                { "1", (BAND.B1_2G, BAND_MATCH_TYPE.FREQUENCY_GHZ) },
            { "2.3", (BAND.B2_3G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, { "2,3", (BAND.B2_3G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, 
                { "2320", (BAND.B2_3G, BAND_MATCH_TYPE.FREQUENCY_MHZ) }, { "13", (BAND.B2_3G, BAND_MATCH_TYPE.WAVELENGTH) },
            { "3.4", (BAND.B3_4G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, { "3,4", (BAND.B3_4G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, 
                { "3400", (BAND.B3_4G, BAND_MATCH_TYPE.FREQUENCY_MHZ) }, { "9", (BAND.B3_4G, BAND_MATCH_TYPE.WAVELENGTH) },
            { "5", (BAND.B5_7G, BAND_MATCH_TYPE.UNCLEAR) }, { "5.7", (BAND.B5_7G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, 
                { "5,7", (BAND.B5_7G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, { "5760", (BAND.B5_7G, BAND_MATCH_TYPE.FREQUENCY_MHZ) },
                { "6", (BAND.B5_7G, BAND_MATCH_TYPE.WAVELENGTH) },
            { "10", (BAND.B10G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, { "10.3", (BAND.B10G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, 
                { "10,3", (BAND.B10G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, { "10368", (BAND.B10G, BAND_MATCH_TYPE.FREQUENCY_MHZ) }, 
                { "3", (BAND.B10G, BAND_MATCH_TYPE.WAVELENGTH) },
            { "24", (BAND.B24G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, { "1.2", (BAND.B24G, BAND_MATCH_TYPE.UNCLEAR) }, 
                { "1,2", (BAND.B24G, BAND_MATCH_TYPE.UNCLEAR) }, { "1.5", (BAND.B24G, BAND_MATCH_TYPE.WAVELENGTH) },
            { "47", (BAND.B47G, BAND_MATCH_TYPE.FREQUENCY_GHZ) },
            { "76", (BAND.B76G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }, 
            { "122", (BAND.B122G, BAND_MATCH_TYPE.FREQUENCY_GHZ) },
            { "134", (BAND.B134G, BAND_MATCH_TYPE.FREQUENCY_GHZ) },
            { "241", (BAND.B241G, BAND_MATCH_TYPE.FREQUENCY_GHZ) }
        };

        private void MarkQRVFromBand(BAND QRV, ref DataRow row)
        {
            // match the bands in "our" BANDS property
            var matchingBand = BANDS
                .Where(element => element.Equals(Bands.GetStringValue(QRV).Replace(".", "_")));
            if (matchingBand.Count() > 0)
            {
                row[matchingBand.First()] = QRV_STATE.qrv;
            }
        }

        /// <summary>
        /// EstimateBandsSupported given the string nameInfo estimate information on bands the user is qrv
        /// Put into row
        /// </summary>
        /// <param name="nameInfo"></param>
        /// <param name="row"></param>
        private bool EstimateBandsSupported(string nameInfo, ref DataRow row)
        {
            if (NameInfoNotQrv(nameInfo))
            {
                foreach (string band in BANDS)
                    row[band] = QRV_STATE.not_qrv;
                return true;
            }
            else if (nameInfo.Any(char.IsDigit))
            {
                List<BAND> bandList;
                BAND band;
                if (NameInfoRangeToBands(nameInfo, out bandList))
                {
                    Console.WriteLine("range: " + nameInfo + " " + string.Join(", ", bandList.ToArray()));
                    foreach (var QRV in bandList)
                        MarkQRVFromBand( QRV, ref row);
                    return true;
                }
                else if (NameInfoSingleBand(nameInfo, out band))
                {
                    Console.WriteLine("single: " + nameInfo + " " + band.ToString());
                    MarkQRVFromBand(band, ref row);
                    return true;
                }
                else if (NameInfoMultipleBands(nameInfo, out bandList))
                { 
                    Console.WriteLine("match: " + nameInfo + " " + string.Join(", ", bandList.ToArray()));
                    foreach (var QRV in bandList)
                        MarkQRVFromBand(QRV, ref row);
                    return true;
                }
            }
            return false;
        }


        public void set_qrv_state(string call, string loc, string band, QRV_STATE state)
        {
            DataRow qrv_row = QRV_local.Rows.Find(new object[] { call, loc });
            if (qrv_row != null)
            {
                if (state == QRV_STATE.worked)
                    qrv_row[band] = QRV_STATE.qrv; // if worked, then obviously qrv
                else
                    qrv_row[band] = state;
                QRVlocalChanged = true;
            }
        }

        public DataRow match_call_loc_local(string call, string loc)
        {
            return QRV_local.Rows.Find(new object[] { call, loc });
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
            return worked(qrv_state) || not_qrv(qrv_state);
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
                if (File.Exists(QRV_Table_Filename) && ts.CompareTo(TimeSpan.FromHours(48.0)) < 0)
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
            if (!QRVlocalChanged)
                return; // no need to save
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
