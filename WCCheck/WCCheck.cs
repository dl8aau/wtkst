using System;
using System.Collections;

namespace WCCheck
{
	public class WCCheck
	{
		public class PrefixEntry
		{
			public string Prefix = "";

			public string Name = "";

			public string Start = "";

			public string Stop = "";

			public string PREFIX
			{
				get
				{
					return Prefix;
				}
			}

			public string STOP
			{
				get
				{
					return Stop;
				}
			}

			public string START
			{
				get
				{
					return Start;
				}
			}

			public PrefixEntry(string APrefix, string AName, string AStart, string AStop)
			{
                Prefix = APrefix;
                Name = AName;
                Start = AStart;
                Stop = AStop;
			}
		}

		public class PrefixList : ArrayList
		{
			public class LengthComparer : IComparer
			{
				int IComparer.Compare(object x, object y)
				{
					int result;
					if (((WCCheck.PrefixEntry)x).Start.Length < ((WCCheck.PrefixEntry)y).Start.Length)
					{
						result = 1;
					}
					else if (((WCCheck.PrefixEntry)x).Start.Length > ((WCCheck.PrefixEntry)y).Start.Length)
					{
						result = -1;
					}
					else
					{
						result = ((WCCheck.PrefixEntry)x).Start.CompareTo(((WCCheck.PrefixEntry)y).Start);
					}
					return result;
				}
			}

			public PrefixList()
			{
                Add(new WCCheck.PrefixEntry("C3", "Andorra", "C3", "C3"));
                Add(new WCCheck.PrefixEntry("OE", "Austria", "OE", "OE"));
                Add(new WCCheck.PrefixEntry("ON", "Belgium", "ON", "OT"));
                Add(new WCCheck.PrefixEntry("LZ", "Bulgaria", "LZ", "LZ"));
                Add(new WCCheck.PrefixEntry("EA8", "Canary Islands", "EA8", "EA8"));
                Add(new WCCheck.PrefixEntry("EA8", "Canary Islands", "EH8", "EH8"));
                Add(new WCCheck.PrefixEntry("SV9", "Crete", "SV9", "SV9"));
                Add(new WCCheck.PrefixEntry("5B", "Cyprus", "5B", "5B"));
                Add(new WCCheck.PrefixEntry("5B", "Cyprus", "C4", "C4"));
                Add(new WCCheck.PrefixEntry("5B", "Cyprus", "H2", "H2"));
                Add(new WCCheck.PrefixEntry("5B", "Cyprus", "P3", "P4"));
                Add(new WCCheck.PrefixEntry("OK", "Czechia", "OK", "OL"));
                Add(new WCCheck.PrefixEntry("OM", "Slovakia", "OM", "OM"));
                Add(new WCCheck.PrefixEntry("OZ", "Denmark", "OU", "OZ"));
                Add(new WCCheck.PrefixEntry("EA", "Spain", "AM", "AO"));
                Add(new WCCheck.PrefixEntry("EA", "Spain", "EA", "EH"));
                Add(new WCCheck.PrefixEntry("DL", "Germany", "DA", "DR"));
                Add(new WCCheck.PrefixEntry("EI", "Ireland", "EI", "EJ"));
                Add(new WCCheck.PrefixEntry("F", "France", "F", "F"));
                Add(new WCCheck.PrefixEntry("F", "France", "TP", "TP"));
                Add(new WCCheck.PrefixEntry("F", "France", "TM", "TM"));
                Add(new WCCheck.PrefixEntry("G", "England", "G", "G"));
                Add(new WCCheck.PrefixEntry("G", "England", "M", "M"));
                Add(new WCCheck.PrefixEntry("G", "England", "2E", "2E"));
                Add(new WCCheck.PrefixEntry("HA", "Hungaria", "HA", "HA"));
                Add(new WCCheck.PrefixEntry("HA", "Hungaria", "HG", "HG"));
                Add(new WCCheck.PrefixEntry("HB9", "Switzerland", "HB1", "HB9"));
                Add(new WCCheck.PrefixEntry("HB9", "Switzerland", "HE", "HE"));
                Add(new WCCheck.PrefixEntry("HB0", "Liechtenstein", "HB0", "HB0"));
                Add(new WCCheck.PrefixEntry("SP", "Poland", "HF", "HF"));
                Add(new WCCheck.PrefixEntry("SP", "Poland", "SP", "SP"));
                Add(new WCCheck.PrefixEntry("SP", "Poland", "SO", "SO"));
                Add(new WCCheck.PrefixEntry("SP", "Poland", "SQ", "SQ"));
                Add(new WCCheck.PrefixEntry("SP", "Poland", "SN", "SN"));
                Add(new WCCheck.PrefixEntry("SV", "Greece", "J4", "J4"));
                Add(new WCCheck.PrefixEntry("SV", "Greece", "SV", "SV"));
                Add(new WCCheck.PrefixEntry("LA", "Norway", "LA", "LN"));
                Add(new WCCheck.PrefixEntry("LX", "Luxembuorg", "LX", "LX"));
                Add(new WCCheck.PrefixEntry("PA", "Netherlands", "PA", "PI"));
                Add(new WCCheck.PrefixEntry("I", "Italy", "I0", "I9"));
                Add(new WCCheck.PrefixEntry("I", "Italy", "IA", "IZ"));
                Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YU1", "YU1"));
                Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YU6", "YU9"));
                Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YT", "YT"));
                Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YZ", "YZ"));
                Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "4N", "4N"));
                Add(new WCCheck.PrefixEntry("9A", "Croatia", "9A", "9A"));
                Add(new WCCheck.PrefixEntry("9A", "Croatia", "YU2", "YU2"));
                Add(new WCCheck.PrefixEntry("S5", "Slovenia", "S5", "S5"));
                Add(new WCCheck.PrefixEntry("S5", "Slovenia", "YU3", "YU3"));
                Add(new WCCheck.PrefixEntry("T9", "Bosnia-Herzegovina", "T9", "T9"));
                Add(new WCCheck.PrefixEntry("T9", "Bosnia-Herzegovina", "YU4", "YU4"));
                Add(new WCCheck.PrefixEntry("Z3", "Mazedonia", "Z3", "Z3"));
                Add(new WCCheck.PrefixEntry("Z3", "Mazedonia", "YU5", "YU5"));
                Add(new WCCheck.PrefixEntry("SM", "Sweden", "SA", "SM"));
                Add(new WCCheck.PrefixEntry("HV", "Vatikan", "HV", "HV"));
                Add(new WCCheck.PrefixEntry("EA6", "Balearic Islands", "EA6", "EA6"));
                Add(new WCCheck.PrefixEntry("EA6", "Balearic Islands", "EH6", "EH6"));
                Add(new WCCheck.PrefixEntry("GD", "Isle Of Man", "GD", "GD"));
                Add(new WCCheck.PrefixEntry("GI", "Northern Ireland", "GI", "GI"));
                Add(new WCCheck.PrefixEntry("GJ", "Jersey", "GJ", "GJ"));
                Add(new WCCheck.PrefixEntry("GM", "Scotland", "GM", "GM"));
                Add(new WCCheck.PrefixEntry("GW", "Wales", "GW", "GW"));
                Add(new WCCheck.PrefixEntry("IS", "Sardinia", "IS", "IS"));
                Add(new WCCheck.PrefixEntry("TK", "Corsica", "TK", "TK"));
                Add(new WCCheck.PrefixEntry("LY", "Lithuania", "LY", "LY"));
                Add(new WCCheck.PrefixEntry("ER", "Moldavia", "ER", "ER"));
                Add(new WCCheck.PrefixEntry("ES", "Estonia", "ES", "ES"));
                Add(new WCCheck.PrefixEntry("EV", "Byelorussia", "EV", "EV"));
                Add(new WCCheck.PrefixEntry("EV", "Byelorussia", "EW", "EW"));
                Add(new WCCheck.PrefixEntry("UA", "European Russia", "UA", "UA"));
                Add(new WCCheck.PrefixEntry("UA", "European Russia", "UA", "RA"));
                Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "UA2", "UA2"));
                Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "RA2", "RA2"));
                Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "RN2", "RN2"));
                Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "RK2", "RK2"));
                Add(new WCCheck.PrefixEntry("YO", "Romania", "YO", "YR"));
                Add(new WCCheck.PrefixEntry("ZA", "Albania", "ZA", "ZA"));
                Add(new WCCheck.PrefixEntry("ZB4", "Gibraltar", "ZB4", "ZB4"));
                Add(new WCCheck.PrefixEntry("9H", "Malta", "9H", "9H"));
                Add(new WCCheck.PrefixEntry("TA", "Turkey", "TA", "TA"));
                Add(new WCCheck.PrefixEntry("T7", "San Marino", "T7", "T7"));
                Sort(new WCCheck.PrefixList.LengthComparer());
			}
		}

		public static double Radius;

		private static WCCheck.PrefixList Prefixes;

		static WCCheck()
		{
			WCCheck.Radius = 6378.2;
			WCCheck.Prefixes = new WCCheck.PrefixList();
		}

		public static double Lat(string S)
		{
			double StepB = 10.0;
			double StepB2 = StepB / 10.0;
			double StepB3 = StepB2 / 24.0;
			double StartB = -90.0;
			double StartB2 = 0.0;
			double StartB3 = StepB3 / 2.0;
			double result;
			try
			{
				S = S.ToUpper();
				if (S[1] < 'A' || S[1] > 'Z' || S[3] < '0' || S[3] > '9' || S[5] < 'A' || S[5] > 'X')
				{
					result = -360.0;
				}
				else
				{
					result = StartB + StepB * (double)(Convert.ToInt16(S[1]) - 65) + StartB2 + StepB2 * (double)(Convert.ToInt16(S[3]) - 48) + StartB3 + StepB3 * (double)(Convert.ToInt16(S[5]) - 65);
				}
			}
			catch
			{
				result = -360.0;
			}
			return result;
		}

		public static double Lon(string S)
		{
			double result;
			try
			{
				double StepL = 20.0;
				double StepL2 = StepL / 10.0;
				double StepL3 = StepL2 / 24.0;
				double StartL = -180.0;
				double StartL2 = 0.0;
				double StartL3 = StepL3 / 2.0;
				S = S.ToUpper();
				if (S[0] < 'A' || S[0] > 'Z' || S[2] < '0' || S[2] > '9' || S[4] < 'A' || S[4] > 'X')
				{
					result = -360.0;
				}
				else
				{
					result = StartL + StepL * (double)(Convert.ToInt16(S[0]) - 65) + StartL2 + StepL2 * (double)(Convert.ToInt16(S[2]) - 48) + StartL3 + StepL3 * (double)(Convert.ToInt16(S[4]) - 65);
				}
			}
			catch
			{
				result = -360.0;
			}
			return result;
		}

		public static string Loc(double L, double B)
		{
			string result;
			try
			{
				double StepB = 10.0;
				double StepB2 = StepB / 10.0;
				double StepB3 = StepB2 / 24.0;
				double StartB = -90.0;
				double StartB2 = 0.0;
				double StartB3 = StepB3 / 2.0;
				double StepL = 20.0;
				double StepL2 = StepL / 10.0;
				double StepL3 = StepL2 / 24.0;
				double StartL = -180.0;
				double StartL2 = 0.0;
				double StartL3 = StepL3 / 2.0;
				int i0 = Convert.ToInt32(Math.Floor((L - StartL) / StepL));
				char S0 = Convert.ToChar(i0 + 65);
				L = L - (double)i0 * StepL - StartL;
				int i = Convert.ToInt32(Math.Floor((L - StartL2) / StepL2));
				char S = Convert.ToChar(i + 48);
				L = L - (double)i * StepL2 - StartL2;
				int i2 = Convert.ToInt32((L - StartL3) / StepL3);
				char S2 = Convert.ToChar(i2 + 65);
				int i3 = Convert.ToInt32(Math.Floor((B - StartB) / StepB));
				char S3 = Convert.ToChar(i3 + 65);
				B = B - (double)i3 * StepB - StartB;
				int i4 = Convert.ToInt32(Math.Floor((B - StartB2) / StepB2));
				char S4 = Convert.ToChar(i4 + 48);
				B = B - (double)i4 * StepB2 - StartB2;
				int i5 = Convert.ToInt32((B - StartB3) / StepB3);
				char S5 = Convert.ToChar(i5 + 65);
				string S6 = string.Concat(new object[]
				{
					S0,
					S3,
					S,
					S4,
					S2,
					S5
				});
				result = S6;
			}
			catch
			{
				result = "";
			}
			return result;
		}

		public static int QRB(string MyLoc, string Loc)
		{
			return (int)WCCheck.QRB(WCCheck.Lat(MyLoc), WCCheck.Lon(MyLoc), WCCheck.Lat(Loc), WCCheck.Lon(Loc));
		}

        public static double QRB(double mylat, double mylon, double lat, double lon)
        {
            if (mylon < -180.0 || mylon > 180.0 || mylat < -90.0 || mylat > 90.0 || lon < -180.0 || lon > 180.0 || lat < -90.0 || lat > 90.0)
            {
                return -1.0;
            }
            double R = Radius;
            double dLat = mylat - lat;
            double dLon = mylon - lon;
            double a = Math.Sin(dLat / 180.0 * Math.PI / 2.0) * Math.Sin(dLat / 180.0 * Math.PI / 2.0) 
                + Math.Sin(dLon / 180.0 * Math.PI / 2.0) * Math.Sin(dLon / 180.0 * Math.PI / 2.0)
                * Math.Cos(mylat / 180.0 * Math.PI) * Math.Cos(lat / 180.0 * Math.PI);
            return R * 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        }

		public static double QTF(string MyLoc, string Loc)
		{
            return WCCheck.QTF(WCCheck.Lat(MyLoc), WCCheck.Lon(MyLoc), WCCheck.Lat(Loc), WCCheck.Lon(Loc));
        }

        // from Airscout ScoutBase.Core.LatLon
        public static double QTF(double mylat, double mylon, double lat, double lon)
        {
            if (mylon < -180.0 || mylon > 180.0 || mylat < -90.0 || mylat > 90.0 || lon < -180.0 || lon > 180.0 || lat < -90.0 || lat > 90.0)
            {
                return -1.0;
            }
            double dLat = lat - mylat;
            double dLon = lon - mylon;
            double y = Math.Sin(dLon / 180.0 * Math.PI) * Math.Cos(lat / 180.0 * Math.PI);
            double x = Math.Cos(mylat / 180.0 * Math.PI) * Math.Sin(lat / 180.0 * Math.PI) 
                - Math.Sin(mylat / 180.0 * Math.PI) * Math.Cos(lat / 180.0 * Math.PI) * Math.Cos(dLon / 180.0 * Math.PI);
            return (Math.Atan2(y, x) / Math.PI * 180.0 + 360.0) % 360.0;
        }


        /* SanitizeCall() tries to remove anything from .e. the KST user name that does not belong to a call sign (e.g. -2, -7 etc.)
        */
        public static string SanitizeCall(string S)
        {
            string result;
            try
            {
                S.Trim().ToUpper();
                if (S.IndexOf('-') >= 0)
                {
                    S = S.Remove(S.IndexOf('-'));
                }
                result = S;
            }
            catch
            {
                result = "";
            }
            return result;
        }

        /* Cut() tries to remove everything that does not belong to the inner call sign
         */
        public static string Cut(string S)
		{
			string result;
			try
			{
				S.Trim().ToUpper();
				if (S.IndexOf('-') >= 0)
				{
					S = S.Remove(S.IndexOf('-'));
				}
				if (S.IndexOf('/') >= 0)
				{
					if (S.IndexOf('/') >= S.Length - 4)
					{
						S = S.Remove(S.IndexOf('/'), S.Length - S.IndexOf('/'));
					}
					if (S.IndexOf('/') >= 0)
					{
						S = S.Remove(0, S.IndexOf('/') + 1);
					}
					if (S.IndexOf('/') >= S.Length - 4)
					{
						S = S.Remove(S.IndexOf('/'), S.Length - S.IndexOf('/'));
					}
				}
				result = S;
			}
			catch
			{
				result = "";
			}
			return result;
		}

		public static string Prefix(string S)
		{
			S = WCCheck.Cut(S).ToUpper();
			string result;
			for (int i = 0; i < WCCheck.Prefixes.Count; i++)
			{
				try
				{
					string Start = ((WCCheck.PrefixEntry)WCCheck.Prefixes[i]).Start;
					string Stop = ((WCCheck.PrefixEntry)WCCheck.Prefixes[i]).Stop;
					string Prefix = ((WCCheck.PrefixEntry)WCCheck.Prefixes[i]).Prefix;
					S = S.Substring(0, Start.Length);
					if (S.CompareTo(Start) >= 0 && S.CompareTo(Stop) <= 0)
					{
						result = Prefix;
						return result;
					}
				}
				catch
				{
				}
			}
			result = "???";
			return result;
		}

		public static int IsCall(string S)
		{
			int result;
			try
			{
				S = S.Trim();
				S = S.ToUpper();
				for (int i = 0; i < S.Length; i++)
				{
					if ((S[i] < 'A' || S[i] > 'Z') && (S[i] < '0' || S[i] > '9') && S[i] != '/')
					{
						result = -1;
						return result;
					}
				}
				S = WCCheck.Cut(S);
				if (S.Length < 3)
				{
					result = -1;
				}
				else if (char.IsNumber(S, 0))
				{
					if (char.IsLetter(S, 1) && char.IsNumber(S, 2))
					{
						result = 1;
					}
					else
					{
						result = -1;
					}
				}
				else if (char.IsLetter(S, 1))
				{
					if (char.IsNumber(S, 2))
					{
						result = 1;
					}
					else
					{
						result = -1;
					}
				}
				else if (char.IsLetter(S, 2))
				{
					result = 1;
				}
				else if (char.IsLetter(S, 3))
				{
					result = 1;
				}
				else
				{
					result = -1;
				}
			}
			catch
			{
				result = -1;
			}
			return result;
		}

		public static int IsLoc(string S)
		{
			S = S.Trim();
			S = S.ToUpper();
			int result;
			if (S.Length == 6)
			{
				if (S[0] >= 'A' && S[0] <= 'X' && S[1] >= 'A' && S[1] <= 'X' && S[2] >= '0' && S[2] <= '9' && S[3] >= '0' && S[3] <= '9' && S[4] >= 'A' && S[4] <= 'X' && S[5] >= 'A' && S[5] <= 'X')
				{
					result = 1;
					return result;
				}
			}
			result = -1;
			return result;
		}

		public static bool IsNumeric(string S)
		{
			int i = 0;
			while (i < S.Length && S[i] >= '0' && S[i] <= '9')
			{
				i++;
			}
			return i >= S.Length;
		}
	}
}
