using System;

namespace SphericalEarth
{
	public class LatLon
	{
		public class GPoint
		{
			public double Lat;

			public double Lon;

			public GPoint()
			{
				this.Lat = double.NaN;
				this.Lon = double.NaN;
			}

			public GPoint(double lat, double lon)
			{
				this.Lat = lat;
				this.Lon = lon;
			}
		}

		public static double Distance(double mylat, double mylon, double lat, double lon)
		{
			double R = 6371.0;
			double dLat = mylat - lat;
			double dLon = mylon - lon;
			double a = Math.Sin(dLat / 180.0 * 3.1415926535897931 / 2.0) * Math.Sin(dLat / 180.0 * 3.1415926535897931 / 2.0) + Math.Sin(dLon / 180.0 * 3.1415926535897931 / 2.0) * Math.Sin(dLon / 180.0 * 3.1415926535897931 / 2.0) * Math.Cos(mylat / 180.0 * 3.1415926535897931) * Math.Cos(lat / 180.0 * 3.1415926535897931);
			return R * 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
		}

		public static double Bearing(double mylat, double mylon, double lat, double lon)
		{
			double dLat = lat - mylat;
			double dLon = lon - mylon;
			double y = Math.Sin(dLon / 180.0 * 3.1415926535897931) * Math.Cos(lat / 180.0 * 3.1415926535897931);
			double x = Math.Cos(mylat / 180.0 * 3.1415926535897931) * Math.Sin(lat / 180.0 * 3.1415926535897931) - Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Cos(lat / 180.0 * 3.1415926535897931) * Math.Cos(dLon / 180.0 * 3.1415926535897931);
			return (Math.Atan2(y, x) / 3.1415926535897931 * 180.0 + 360.0) % 360.0;
		}

		public static LatLon.GPoint MidPoint(double mylat, double mylon, double lat, double lon)
		{
			double aslatdiff = mylat - lat;
			double aslondiff = mylon - lon;
			double bx = Math.Cos(lat / 180.0 * 3.1415926535897931) * Math.Cos(-aslondiff / 180.0 * 3.1415926535897931);
			double by = Math.Cos(lat / 180.0 * 3.1415926535897931) * Math.Sin(-aslondiff / 180.0 * 3.1415926535897931);
			double latm = Math.Atan2(Math.Sin(mylat / 180.0 * 3.1415926535897931) + Math.Sin(lat / 180.0 * 3.1415926535897931), Math.Sqrt((Math.Cos(mylat / 180.0 * 3.1415926535897931) + bx) * (Math.Cos(mylat / 180.0 * 3.1415926535897931) + bx) + by * by)) / 3.1415926535897931 * 180.0;
			double lonm = mylon + Math.Atan2(by, Math.Cos(mylat / 180.0 * 3.1415926535897931) + bx) / 3.1415926535897931 * 180.0;
			return new LatLon.GPoint(latm, lonm);
		}

		public static LatLon.GPoint DestinationPoint(double mylat, double mylon, double bearing, double distance)
		{
			double R = 6371.0;
			double lat = Math.Asin(Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Cos(distance / R) + Math.Cos(mylat / 180.0 * 3.1415926535897931) * Math.Sin(distance / R) * Math.Cos(bearing / 180.0 * 3.1415926535897931));
			double lon = mylon / 180.0 * 3.1415926535897931 + Math.Atan2(Math.Sin(bearing / 180.0 * 3.1415926535897931) * Math.Sin(distance / R) * Math.Cos(mylat / 180.0 * 3.1415926535897931), Math.Cos(distance / R) - Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Sin(lat));
			return new LatLon.GPoint(lat / 3.1415926535897931 * 180.0, lon / 3.1415926535897931 * 180.0);
		}

		public static LatLon.GPoint IntersectionPoint(double mylat, double mylon, double mybearing, double lat, double lon, double bearing)
		{
			double dLat = lat - mylat;
			double dLon = lon - mylon;
			double brng13 = mybearing / 180.0 * 3.1415926535897931;
			double brng14 = bearing / 180.0 * 3.1415926535897931;
			double dist12 = 2.0 * Math.Asin(Math.Sqrt(Math.Sin(dLat / 2.0 / 180.0 * 3.1415926535897931) * Math.Sin(dLat / 2.0 / 180.0 * 3.1415926535897931) + Math.Cos(mylat / 180.0 * 3.1415926535897931) * Math.Cos(lat / 180.0 * 3.1415926535897931) * Math.Sin(dLon / 2.0 / 180.0 * 3.1415926535897931) * Math.Sin(dLon / 2.0 / 180.0 * 3.1415926535897931)));
			LatLon.GPoint result;
			if (dist12 == 0.0)
			{
				result = null;
			}
			else
			{
				double brngA = Math.Acos((Math.Sin(lat / 180.0 * 3.1415926535897931) - Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Cos(dist12)) / (Math.Sin(dist12) * Math.Cos(mylat / 180.0 * 3.1415926535897931)));
				if (double.IsNaN(brngA))
				{
					brngA = 0.0;
				}
				double brngB = Math.Acos((Math.Sin(mylat / 180.0 * 3.1415926535897931) - Math.Sin(lat / 180.0 * 3.1415926535897931) * Math.Cos(dist12)) / (Math.Sin(dist12) * Math.Cos(lat / 180.0 * 3.1415926535897931)));
				double brng15;
				double brng16;
				if (Math.Sin(lon / 180.0 * 3.1415926535897931 - mylon / 180.0 * 3.1415926535897931) > 0.0)
				{
					brng15 = brngA;
					brng16 = 6.2831853071795862 - brngB;
				}
				else
				{
					brng15 = 6.2831853071795862 - brngA;
					brng16 = brngB;
				}
				double alpha = (brng13 - brng15 + 3.1415926535897931) % 6.2831853071795862 - 3.1415926535897931;
				double alpha2 = (brng16 - brng14 + 3.1415926535897931) % 6.2831853071795862 - 3.1415926535897931;
				if (Math.Sin(alpha) == 0.0 && Math.Sin(alpha2) == 0.0)
				{
					result = null;
				}
				else if (Math.Sin(alpha) * Math.Sin(alpha2) < 0.0)
				{
					result = null;
				}
				else
				{
					double alpha3 = Math.Acos(-Math.Cos(alpha) * Math.Cos(alpha2) + Math.Sin(alpha) * Math.Sin(alpha2) * Math.Cos(dist12));
					double dist13 = Math.Atan2(Math.Sin(dist12) * Math.Sin(alpha) * Math.Sin(alpha2), Math.Cos(alpha2) + Math.Cos(alpha) * Math.Cos(alpha3));
					double lat2 = Math.Asin(Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Cos(dist13) + Math.Cos(mylat / 180.0 * 3.1415926535897931) * Math.Sin(dist13) * Math.Cos(brng13));
					double dLon2 = Math.Atan2(Math.Sin(brng13) * Math.Sin(dist13) * Math.Cos(mylat / 180.0 * 3.1415926535897931), Math.Cos(dist13) - Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Sin(lat2));
					double lon2 = mylon / 180.0 * 3.1415926535897931 + dLon2;
					lon2 = (lon2 + 9.42477796076938) % 6.2831853071795862 - 3.1415926535897931;
					result = new LatLon.GPoint(lat2 / 3.1415926535897931 * 180.0, lon2 / 3.1415926535897931 * 180.0);
				}
			}
			return result;
		}

		public static double Lat(string loc)
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
				loc = loc.ToUpper();
				if (loc[1] < 'A' || loc[1] > 'Z' || loc[3] < '0' || loc[3] > '9' || loc[5] < 'A' || loc[5] > 'X')
				{
					result = -360.0;
				}
				else
				{
					result = StartB + StepB * (double)(Convert.ToInt16(loc[1]) - 65) + StartB2 + StepB2 * (double)(Convert.ToInt16(loc[3]) - 48) + StartB3 + StepB3 * (double)(Convert.ToInt16(loc[5]) - 65);
				}
			}
			catch
			{
				result = -360.0;
			}
			return result;
		}

		public static double Lon(string loc)
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
				loc = loc.ToUpper();
				if (loc[0] < 'A' || loc[0] > 'Z' || loc[2] < '0' || loc[2] > '9' || loc[4] < 'A' || loc[4] > 'X')
				{
					result = -360.0;
				}
				else
				{
					result = StartL + StepL * (double)(Convert.ToInt16(loc[0]) - 65) + StartL2 + StepL2 * (double)(Convert.ToInt16(loc[2]) - 48) + StartL3 + StepL3 * (double)(Convert.ToInt16(loc[4]) - 65);
				}
			}
			catch
			{
				result = -360.0;
			}
			return result;
		}

		public static string Loc(double lat, double lon)
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
				int i0 = Convert.ToInt32(Math.Floor((lon - StartL) / StepL));
				char S0 = Convert.ToChar(i0 + 65);
				lon = lon - (double)i0 * StepL - StartL;
				int i = Convert.ToInt32(Math.Floor((lon - StartL2) / StepL2));
				char S = Convert.ToChar(i + 48);
				lon = lon - (double)i * StepL2 - StartL2;
				int i2 = Convert.ToInt32((lon - StartL3) / StepL3);
				char S2 = Convert.ToChar(i2 + 65);
				int i3 = Convert.ToInt32(Math.Floor((lat - StartB) / StepB));
				char S3 = Convert.ToChar(i3 + 65);
				lat = lat - (double)i3 * StepB - StartB;
				int i4 = Convert.ToInt32(Math.Floor((lat - StartB2) / StepB2));
				char S4 = Convert.ToChar(i4 + 48);
				lat = lat - (double)i4 * StepB2 - StartB2;
				int i5 = Convert.ToInt32((lat - StartB3) / StepB3);
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

		public static int QRB(string myloc, string loc)
		{
			return LatLon.QRB(LatLon.Lat(myloc), LatLon.Lon(myloc), LatLon.Lat(loc), LatLon.Lon(loc));
		}

		public static int QRB(double mylat, double mylon, double lat, double lon)
		{
			int result;
			try
			{
				double F = 111.2;
				if (mylon < -180.0 || mylon > 180.0 || mylat < -90.0 || mylat > 90.0 || lon < -180.0 || lon > 180.0 || lat < -90.0 || lat > 90.0)
				{
					result = -1;
				}
				else
				{
					double E = Math.Sin(mylat / 180.0 * 3.1415926535897931) * Math.Sin(lat / 180.0 * 3.1415926535897931) + Math.Cos(mylat / 180.0 * 3.1415926535897931) * Math.Cos(lat / 180.0 * 3.1415926535897931) * Math.Cos((mylon - lon) / 180.0 * 3.1415926535897931);
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

		public static double QTF(string myloc, string loc)
		{
			return LatLon.QTF(LatLon.Lat(myloc), LatLon.Lon(myloc), LatLon.Lat(loc), LatLon.Lon(loc));
		}

		public static double QTF(double mylat, double mylon, double lat, double lon)
		{
			double result;
			try
			{
				double R = 3671.0;
				double F = 6.2831853071795862 * R / 360.0;
				if (mylon < -180.0 || mylon > 180.0 || mylat < -90.0 || mylat > 90.0 || lon < -180.0 || lon > 180.0 || lat < -90.0 || lat > 90.0)
				{
					result = -1.0;
				}
				else
				{
					double DL = lon - mylon;
					double CE = (Math.Tan(lat / 180.0 * 3.1415926535897931) * Math.Cos(mylat / 180.0 * 3.1415926535897931) - Math.Sin(mylat / 180.0 * 3.1415926535897931)) * Math.Cos(DL / 180.0 * 3.1415926535897931);
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
	}
}
