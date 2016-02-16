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
					return this.Prefix;
				}
			}

			public string STOP
			{
				get
				{
					return this.Stop;
				}
			}

			public string START
			{
				get
				{
					return this.Start;
				}
			}

			public PrefixEntry(string APrefix, string AName, string AStart, string AStop)
			{
				this.Prefix = APrefix;
				this.Name = AName;
				this.Start = AStart;
				this.Stop = AStop;
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
				this.Add(new WCCheck.PrefixEntry("C3", "Andorra", "C3", "C3"));
				this.Add(new WCCheck.PrefixEntry("OE", "Austria", "OE", "OE"));
				this.Add(new WCCheck.PrefixEntry("ON", "Belgium", "ON", "OT"));
				this.Add(new WCCheck.PrefixEntry("LZ", "Bulgaria", "LZ", "LZ"));
				this.Add(new WCCheck.PrefixEntry("EA8", "Canary Islands", "EA8", "EA8"));
				this.Add(new WCCheck.PrefixEntry("EA8", "Canary Islands", "EH8", "EH8"));
				this.Add(new WCCheck.PrefixEntry("SV9", "Crete", "SV9", "SV9"));
				this.Add(new WCCheck.PrefixEntry("5B", "Cyprus", "5B", "5B"));
				this.Add(new WCCheck.PrefixEntry("5B", "Cyprus", "C4", "C4"));
				this.Add(new WCCheck.PrefixEntry("5B", "Cyprus", "H2", "H2"));
				this.Add(new WCCheck.PrefixEntry("5B", "Cyprus", "P3", "P4"));
				this.Add(new WCCheck.PrefixEntry("OK", "Czechia", "OK", "OL"));
				this.Add(new WCCheck.PrefixEntry("OM", "Slovakia", "OM", "OM"));
				this.Add(new WCCheck.PrefixEntry("OZ", "Denmark", "OU", "OZ"));
				this.Add(new WCCheck.PrefixEntry("EA", "Spain", "AM", "AO"));
				this.Add(new WCCheck.PrefixEntry("EA", "Spain", "EA", "EH"));
				this.Add(new WCCheck.PrefixEntry("DL", "Germany", "DA", "DR"));
				this.Add(new WCCheck.PrefixEntry("EI", "Ireland", "EI", "EJ"));
				this.Add(new WCCheck.PrefixEntry("F", "France", "F", "F"));
				this.Add(new WCCheck.PrefixEntry("F", "France", "TP", "TP"));
				this.Add(new WCCheck.PrefixEntry("F", "France", "TM", "TM"));
				this.Add(new WCCheck.PrefixEntry("G", "England", "G", "G"));
				this.Add(new WCCheck.PrefixEntry("G", "England", "M", "M"));
				this.Add(new WCCheck.PrefixEntry("G", "England", "2E", "2E"));
				this.Add(new WCCheck.PrefixEntry("HA", "Hungaria", "HA", "HA"));
				this.Add(new WCCheck.PrefixEntry("HA", "Hungaria", "HG", "HG"));
				this.Add(new WCCheck.PrefixEntry("HB9", "Switzerland", "HB1", "HB9"));
				this.Add(new WCCheck.PrefixEntry("HB9", "Switzerland", "HE", "HE"));
				this.Add(new WCCheck.PrefixEntry("HB0", "Liechtenstein", "HB0", "HB0"));
				this.Add(new WCCheck.PrefixEntry("SP", "Poland", "HF", "HF"));
				this.Add(new WCCheck.PrefixEntry("SP", "Poland", "SP", "SP"));
				this.Add(new WCCheck.PrefixEntry("SP", "Poland", "SO", "SO"));
				this.Add(new WCCheck.PrefixEntry("SP", "Poland", "SQ", "SQ"));
				this.Add(new WCCheck.PrefixEntry("SP", "Poland", "SN", "SN"));
				this.Add(new WCCheck.PrefixEntry("SV", "Greece", "J4", "J4"));
				this.Add(new WCCheck.PrefixEntry("SV", "Greece", "SV", "SV"));
				this.Add(new WCCheck.PrefixEntry("LA", "Norway", "LA", "LN"));
				this.Add(new WCCheck.PrefixEntry("LX", "Luxembuorg", "LX", "LX"));
				this.Add(new WCCheck.PrefixEntry("PA", "Netherlands", "PA", "PI"));
				this.Add(new WCCheck.PrefixEntry("I", "Italy", "I0", "I9"));
				this.Add(new WCCheck.PrefixEntry("I", "Italy", "IA", "IZ"));
				this.Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YU1", "YU1"));
				this.Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YU6", "YU9"));
				this.Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YT", "YT"));
				this.Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "YZ", "YZ"));
				this.Add(new WCCheck.PrefixEntry("YU", "Yugoslavia", "4N", "4N"));
				this.Add(new WCCheck.PrefixEntry("9A", "Croatia", "9A", "9A"));
				this.Add(new WCCheck.PrefixEntry("9A", "Croatia", "YU2", "YU2"));
				this.Add(new WCCheck.PrefixEntry("S5", "Slovenia", "S5", "S5"));
				this.Add(new WCCheck.PrefixEntry("S5", "Slovenia", "YU3", "YU3"));
				this.Add(new WCCheck.PrefixEntry("T9", "Bosnia-Herzegovina", "T9", "T9"));
				this.Add(new WCCheck.PrefixEntry("T9", "Bosnia-Herzegovina", "YU4", "YU4"));
				this.Add(new WCCheck.PrefixEntry("Z3", "Mazedonia", "Z3", "Z3"));
				this.Add(new WCCheck.PrefixEntry("Z3", "Mazedonia", "YU5", "YU5"));
				this.Add(new WCCheck.PrefixEntry("SM", "Sweden", "SA", "SM"));
				this.Add(new WCCheck.PrefixEntry("HV", "Vatikan", "HV", "HV"));
				this.Add(new WCCheck.PrefixEntry("EA6", "Balearic Islands", "EA6", "EA6"));
				this.Add(new WCCheck.PrefixEntry("EA6", "Balearic Islands", "EH6", "EH6"));
				this.Add(new WCCheck.PrefixEntry("GD", "Isle Of Man", "GD", "GD"));
				this.Add(new WCCheck.PrefixEntry("GI", "Northern Ireland", "GI", "GI"));
				this.Add(new WCCheck.PrefixEntry("GJ", "Jersey", "GJ", "GJ"));
				this.Add(new WCCheck.PrefixEntry("GM", "Scotland", "GM", "GM"));
				this.Add(new WCCheck.PrefixEntry("GW", "Wales", "GW", "GW"));
				this.Add(new WCCheck.PrefixEntry("IS", "Sardinia", "IS", "IS"));
				this.Add(new WCCheck.PrefixEntry("TK", "Corsica", "TK", "TK"));
				this.Add(new WCCheck.PrefixEntry("LY", "Lithuania", "LY", "LY"));
				this.Add(new WCCheck.PrefixEntry("ER", "Moldavia", "ER", "ER"));
				this.Add(new WCCheck.PrefixEntry("ES", "Estonia", "ES", "ES"));
				this.Add(new WCCheck.PrefixEntry("EV", "Byelorussia", "EV", "EV"));
				this.Add(new WCCheck.PrefixEntry("EV", "Byelorussia", "EW", "EW"));
				this.Add(new WCCheck.PrefixEntry("UA", "European Russia", "UA", "UA"));
				this.Add(new WCCheck.PrefixEntry("UA", "European Russia", "UA", "RA"));
				this.Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "UA2", "UA2"));
				this.Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "RA2", "RA2"));
				this.Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "RN2", "RN2"));
				this.Add(new WCCheck.PrefixEntry("UA2", "Kaliningrad", "RK2", "RK2"));
				this.Add(new WCCheck.PrefixEntry("YO", "Romania", "YO", "YR"));
				this.Add(new WCCheck.PrefixEntry("ZA", "Albania", "ZA", "ZA"));
				this.Add(new WCCheck.PrefixEntry("ZB4", "Gibraltar", "ZB4", "ZB4"));
				this.Add(new WCCheck.PrefixEntry("9H", "Malta", "9H", "9H"));
				this.Add(new WCCheck.PrefixEntry("TA", "Turkey", "TA", "TA"));
				this.Add(new WCCheck.PrefixEntry("T7", "San Marino", "T7", "T7"));
				this.Sort(new WCCheck.PrefixList.LengthComparer());
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
			return WCCheck.QRB(WCCheck.Lat(MyLoc), WCCheck.Lon(MyLoc), WCCheck.Lat(Loc), WCCheck.Lon(Loc));
		}

		public static int QRB(double MyLat, double MyLon, double Lat, double Lon)
		{
			int result;
			try
			{
				double F = 111.2;
				if (MyLon < -180.0 || MyLon > 180.0 || MyLat < -90.0 || MyLat > 90.0 || Lon < -180.0 || Lon > 180.0 || Lat < -90.0 || Lat > 90.0)
				{
					result = -1;
				}
				else
				{
					double E = Math.Sin(MyLat / 180.0 * 3.1415926535897931) * Math.Sin(Lat / 180.0 * 3.1415926535897931) + Math.Cos(MyLat / 180.0 * 3.1415926535897931) * Math.Cos(Lat / 180.0 * 3.1415926535897931) * Math.Cos((MyLon - Lon) / 180.0 * 3.1415926535897931);
					if (E == 0.0)
					{
						result = 0;
					}
					else
					{
						E = Math.Sqrt(1.0 - E * E) / E;
						E = Math.Atan(E) / 3.1415926535897931 * 180.0 * F + 0.5;
						result = Convert.ToInt32(Math.Round(E, 0));
					}
				}
			}
			catch
			{
				result = -1;
			}
			return result;
		}

		public static double QTF(string MyLoc, string Loc)
		{
			return WCCheck.QTF(WCCheck.Lat(MyLoc), WCCheck.Lon(MyLoc), WCCheck.Lat(Loc), WCCheck.Lon(Loc));
		}

		public static double QTF(double MyLat, double MyLon, double Lat, double Lon)
		{
			double result;
			try
			{
				double F = 6.2831853071795862 * WCCheck.Radius / 360.0;
				if (MyLon < -180.0 || MyLon > 180.0 || MyLat < -90.0 || MyLat > 90.0 || Lon < -180.0 || Lon > 180.0 || Lat < -90.0 || Lat > 90.0)
				{
					result = -1.0;
				}
				else
				{
					double DL = Lon - MyLon;
					double CE = (Math.Tan(Lat / 180.0 * 3.1415926535897931) * Math.Cos(MyLat / 180.0 * 3.1415926535897931) - Math.Sin(MyLat / 180.0 * 3.1415926535897931)) * Math.Cos(DL / 180.0 * 3.1415926535897931);
					if (CE == 0.0)
					{
						if (DL < 0.0)
						{
							result = 270.0;
						}
						else if (DL > 0.0)
						{
							result = 90.0;
						}
						else
						{
							result = 0.0;
						}
					}
					else
					{
						double E = Math.Atan(Math.Sin(DL / 180.0 * 3.1415926535897931) / CE) / 3.1415926535897931 * 180.0;
						if (CE < 0.0)
						{
							E += 180.0;
						}
						if (CE > 0.0 && E < 0.0)
						{
							E += 360.0;
						}
						result = E;
					}
				}
			}
			catch
			{
				result = -1.0;
			}
			return result;
		}

		public static string Cut(string S)
		{
			string result;
			try
			{
				S.Trim().ToUpper();
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
