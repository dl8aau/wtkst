using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace wtKST
{
    public class ADIFRecord
    {
        public string Call { get; set; }
        public string Band { get; set; }
        public string Time { get; set; }
        public string Sent { get; set; }
        public string Rcvd { get; set; }
        public string Loc  { get; set; }
    }

    public static class ADIFLog
    {
        // Map standard ADIF band names to internal column names used in the CALL table.
        private static readonly Dictionary<string, string> BandMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "6m",     "50M"  },
            { "4m",     "70M"  },
            { "2m",     "144M" },
            { "70cm",   "432M" },
            { "23cm",   "1_2G" },
            { "13cm",   "2_3G" },
            { "9cm",    "3_4G" },
            { "6cm",    "5_7G" },
            { "3cm",    "10G"  },
            { "1.25cm", "24G"  },
            { "6mm",    "47G"  },
            { "4mm",    "76G"  },
        };

        /// <summary>
        /// Parse an ADIF file and return a list of QSO records.
        /// </summary>
        public static List<ADIFRecord> Parse(string filePath)
        {
            var result = new List<ADIFRecord>();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return result;

            string text;
            try { text = File.ReadAllText(filePath); }
            catch { return result; }

            // Skip ADIF header (everything before <EOH>)
            int eoh = text.IndexOf("<EOH>", StringComparison.OrdinalIgnoreCase);
            if (eoh >= 0)
                text = text.Substring(eoh + 5);

            string[] records = Regex.Split(text, @"<EOR>", RegexOptions.IgnoreCase);
            var fieldRegex = new Regex(@"<([A-Za-z_]+):\d+>([^<]*)", RegexOptions.IgnoreCase);

            foreach (var record in records)
            {
                string call = null, band = null, timeOn = "", sent = "", rcvd = "", loc = "";

                foreach (Match m in fieldRegex.Matches(record))
                {
                    string fn = m.Groups[1].Value.ToUpperInvariant();
                    string fv = m.Groups[2].Value.Trim();
                    switch (fn)
                    {
                        case "CALL":       call = fv.ToUpperInvariant(); break;
                        case "BAND":       if (band == null) band = MapBand(fv); break;
                        case "FREQ":       if (band == null) band = FreqMhzToBand(fv); break;
                        case "TIME_ON":    timeOn = fv.Length >= 4 ? fv.Substring(0, 2) + ":" + fv.Substring(2, 2) : fv; break;
                        case "STX":        sent = fv; break;
                        case "SRX":        rcvd = fv; break;
                        case "GRIDSQUARE": loc = fv.Length >= 6 ? fv.Substring(0, 6) : fv; break;
                    }
                }

                if (string.IsNullOrEmpty(call) || string.IsNullOrEmpty(band))
                    continue;

                result.Add(new ADIFRecord { Call = call, Band = band, Time = timeOn, Sent = sent, Rcvd = rcvd, Loc = loc });
            }

            return result;
        }

        private static string MapBand(string adifBand)
        {
            if (BandMap.TryGetValue(adifBand.Trim(), out string b))
                return b;
            return null;
        }

        private static string FreqMhzToBand(string freqStr)
        {
            if (!double.TryParse(freqStr, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out double mhz))
                return null;

            if (mhz >= 50 && mhz < 54)        return "50M";
            if (mhz >= 70 && mhz < 71)        return "70M";
            if (mhz >= 144 && mhz < 148)      return "144M";
            if (mhz >= 420 && mhz < 450)      return "432M";
            if (mhz >= 1240 && mhz < 1300)    return "1_2G";
            if (mhz >= 2300 && mhz < 2450)    return "2_3G";
            if (mhz >= 3300 && mhz < 3500)    return "3_4G";
            if (mhz >= 5650 && mhz < 5850)    return "5_7G";
            if (mhz >= 10000 && mhz < 10500)  return "10G";
            if (mhz >= 24000 && mhz < 24050)  return "24G";
            if (mhz >= 47000 && mhz < 47200)  return "47G";
            if (mhz >= 76000 && mhz < 81000)  return "76G";
            return null;
        }
    }
}
