using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace WinTest
{
    public class WinTestLog : WinTestLogBase
    {
        private string WinTest_FileName = "";

        public WinTestLog(LogWriteMessageDelegate mylog) : base (mylog)
        {
            DataColumn[] QSOkeys = new DataColumn[]
            {
                QSO.Columns["CALL"],
                QSO.Columns["BAND"]
            };
            QSO.PrimaryKey = QSOkeys;
        }

        public override void Dispose()
        {
        }

        public override string getStatus()
        {
            return WinTest_FileName + " " + QSO.Rows.Count;
        }
        
        public override void Get_QSOs(string WinTest_INI_FileName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(WinTest_INI_FileName, Encoding.Default))
                {
                    string S = sr.ReadToEnd();
                    S = S.Remove(0, S.IndexOf("[Files]") + 7);
                    S = S.Remove(0, S.IndexOf("Path=") + 5);
                    S = S.Remove(S.IndexOf("\r"), S.Length - S.IndexOf("\r"));
                    S = S.Trim();
                    WinTest_FileName = S;
                }
                using (Stream stream = File.Open(WinTest_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    QSO.Clear();
                    byte[] bufh = new byte[13944];
                    stream.Read(bufh, 0, bufh.Length);
                    string wtLoc = Encoding.ASCII.GetString(bufh, 24, 6);
                    char[] separator = new char[1];
                    MyLoc = wtLoc.Split(separator)[0];

                    stream.Position = 13944L;
                    while (stream.Position < stream.Length)
                    {
                        byte[] buf = new byte[544];
                        stream.Read(buf, 0, buf.Length);
                        int utime = BitConverter.ToInt32(buf, 24);
                        DateTime ts = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        ts = ts.AddSeconds((double)utime);
                        byte band = buf[4];
                        string call = Encoding.ASCII.GetString(buf, 32, 14);
                        call = call.ToString().Replace("\0", "");

                        DataRow row = QSO.NewRow();
                        row["CALL"] = call;
                        switch (band)
                        {
                            case 12:
                                row["BAND"] = "144M";
                                break;
                            case 14:
                                row["BAND"] = "432M";
                                break;
                            case 16:
                                row["BAND"] = "1_2G";
                                break;
                            case 17:
                                row["BAND"] = "2_3G";
                                break;
                            case 18:
                                row["BAND"] = "3_4G";
                                break;
                            case 19:
                                row["BAND"] = "5_7G";
                                break;
                            case 20:
                                row["BAND"] = "10G";
                                break;
                            case 21:
                                row["BAND"] = "24G";
                                break;
                            case 22:
                                row["BAND"] = "47G";
                                break;
                            case 23:
                                row["BAND"] = "76G";
                                break;
                            case 13:
                            case 15:
                            default:
                                row["BAND"] = "";
                                break;
                        }

                        row["TIME"] = ts.ToString("HH:mm");
                        string s = BitConverter.ToInt16(buf, 0).ToString();
                        s = Encoding.ASCII.GetString(buf, 46, 4).Replace("\0", "") + s.PadLeft(3, '0');
                        row["SENT"] = s;
                        switch (buf[8])
                        {
                            case 0:
                                row["RCVD"] = Encoding.ASCII.GetString(buf, 50, 3).Replace("\0", "") + Encoding.ASCII.GetString(buf, 53, 4).Replace("\0", "");
                                break;
                            case 1:
                                row["RCVD"] = Encoding.ASCII.GetString(buf, 50, 2).Replace("\0", "") + Encoding.ASCII.GetString(buf, 52, 4).Replace("\0", "");
                                break;
                        }
                        row["LOC"] = Encoding.ASCII.GetString(buf, 61, 6).Replace("\0", "");
                        try
                        {
                            if (QSO.Rows.Find(new string[]
                            {
                                row["CALL"].ToString(),
                                row["BAND"].ToString()
                            }) == null)
                            {
                                QSO.Rows.Add(row);
                            }
                        }
                        catch (Exception e)
                        {
                            Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "(" + row["CALL"].ToString() + "): " + e.Message);
                        }

                    } /* while */
                }
            }
            catch (Exception e)
            {
                Error(System.Reflection.MethodBase.GetCurrentMethod().Name, "(" + WinTest_INI_FileName + "): " + e.Message);
                throw e;
            }
        }

    }
}
