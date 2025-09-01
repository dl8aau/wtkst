using wtKST;

namespace TestClass.kstTest
{

    [TestClass]
    public sealed class UserlistTests
    {
        [TestMethod]
        [DataRow("GHz-Team", "GHz-Team")]
        [DataRow("Dani Test 23/13/6/3", "Dani Test")]
        [DataRow("Phil 1296.255", "Phil")]
        [DataRow("Mek23cm", "Mek")]
        [DataRow("Günther", "Günther")]
        [DataRow("Dare/1296.200", "Dare")]
        [DataRow("Jens/Basti/Alex", "Jens/Basti/Alex")]
        [DataRow("Arvydas SWL", "Arvydas")]
        [DataRow("Dare-only SWL", "Dare")]
        [DataRow("Nick 2m SWL", "Nick")]
        [DataRow("Matjaz (not qrv)", "Matjaz")]
        [DataRow("Marc.(not qrv)", "Marc")]
        [DataRow("Peter not qrv", "Peter")]
        [DataRow("Pavel SWL only", "Pavel")]
        public void TestparseNameFromNameInfo(string nameInfo, string nameExpected)
        {
            PrivateType maindlg = new PrivateType(typeof(wtKST.MainDlg));

            // private static string parseNameFromNameInfo(string nameInfo)
            var actual = maindlg.InvokeStatic("parseNameFromNameInfo", nameInfo);

            Assert.IsTrue(actual.Equals(nameExpected));
        }
    }
    [TestClass]
    public sealed class Userlistqrg
    {
        [TestMethod]
        // evaluated from the left, if one is true, the next ones are don't care
        [DataRow("Pavel - 2m SWL only", true, false, true, false, new BAND[] { BAND.BNONE } )]
        [DataRow("Hans (not qrv)", true, false, false, false, new BAND[] { BAND.BNONE } )]
        [DataRow("Marc.(not qrv)", true, false, false, false, new BAND[] { BAND.BNONE } )]
        [DataRow("70-13cm", false, true, false, false, new BAND[] { BAND.B432M, BAND.B1_2G, BAND.B2_3G } )]
        [DataRow("Ronny 2m-9cm", false, true, false, false, new BAND[] { BAND.B144M, BAND.B432M, BAND.B1_2G, BAND.B2_3G, BAND.B3_4G } )]
        [DataRow("Rene 2m-1.5cm", false, true, false, false, new BAND[] { BAND.B144M, BAND.B432M,
            BAND.B1_2G, BAND.B2_3G, BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G } )]
        [DataRow("Michel 23>3cm", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G, BAND.B3_4G, BAND.B5_7G, BAND.B10G } )]
        [DataRow("Emil 1.3-24GHz", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G } )]
        [DataRow("Rudi .4 - 47 GHz", false, true, false, false, new BAND[] { BAND.B432M, BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G, BAND.B47G } )]
        [DataRow("team 70cm-76GHz", false, true, false, false, new BAND[] { BAND.B432M, BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G, BAND.B47G, BAND.B76G } )]
        [DataRow("Kjeld 1,3-47G", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G, BAND.B47G } )]
        [DataRow("13-1,2", false, true, false, false, new BAND[] { BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G, } )]
        [DataRow("Staszek 5.7-24", false, true, false, false, new BAND[] { BAND.B5_7G, BAND.B10G, BAND.B24G } )]
        [DataRow("David 1.2 - 134", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G, BAND.B47G, BAND.B76G, BAND.B122G, BAND.B134G } )]
        [DataRow("Klaus-23cm", false, false, true, false, new BAND[] { BAND.B1_2G } )]
        [DataRow("Micha 1-10G", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G } )]
        [DataRow("Daniel 1-24GHz", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G } )]
        [DataRow("GHz-Team", false, false, false, false, new BAND[] { BAND.BNONE } )]
        [DataRow("Dani Test 23/13/6/3", false, false, false, true, new BAND[] { BAND.B1_2G, BAND.B2_3G, BAND.B5_7G, BAND.B10G } )]
        [DataRow("Phil 1296.255", false, false, true, false, new BAND[] { BAND.B1_2G } )]
        [DataRow("23and3", false, false, false, true, new BAND[] { BAND.B1_2G, BAND.B10G } )]
        [DataRow("9-6-3-1,2", false, false, false, true, new BAND[] { BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G } )]
        [DataRow("23-13 cm", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G } )]
        [DataRow("144MHz-10GHz", false, true, false, false, new BAND[] { BAND.B144M, BAND.B432M,
            BAND.B1_2G, BAND.B2_3G, BAND.B3_4G, BAND.B5_7G, BAND.B10G } )]
        [DataRow("yoann 1to47 /P", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G, BAND.B47G } )]
        [DataRow("Ben 13cm>>1.2cm", false, true, false, false, new BAND[] { BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G, BAND.B24G } )]
        [DataRow("Dominik_only23cm", false, false, true, false, new BAND[] { BAND.B1_2G } )]
        [DataRow("Philipp 13,9", false, false, false, true, new BAND[] { BAND.B2_3G, BAND.B3_4G } )]
        [DataRow("Tzetzo-23-13", false, true, false, false, new BAND[] { BAND.B1_2G, BAND.B2_3G } )]
        [DataRow("Mike 70 - 3", false, true, false, false, new BAND[] { BAND.B432M, BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G } )]
        [DataRow("Slawek 2m-3cm", false, true, false, false, new BAND[] { BAND.B144M, BAND.B432M, BAND.B1_2G, BAND.B2_3G,
            BAND.B3_4G, BAND.B5_7G, BAND.B10G } )]
        [DataRow("Joe23-QRO-2,4Dsh", false, false, true, false, new BAND[] { BAND.B1_2G })] // ok, stop before text
        [DataRow("23cm 80W@44 Ele", false, false, true, false, new BAND[] { BAND.B1_2G })] // ok, because W makes it clear
        [DataRow("Harry 23/13 CW", false, false, false, true, new BAND[] { BAND.B1_2G, BAND.B2_3G })]
        [DataRow("Oto 50-2320 MHz", false, true, false, false, new BAND[] { BAND.B50M, BAND.B70M, BAND.B144M, BAND.B432M, BAND.B1_2G, BAND.B2_3G })]


        public void TestBandFromNameInfo(string nameInfo, bool notQRV, bool hasRange, bool hasSingle, bool hasBands, BAND[] b)
        {
            PrivateType qrv = new PrivateType(typeof(wtKST.QRVdb));

            // see https://instance-factory.com/?p=738 for "out"
            {
                //private static bool NameInfoNotQrv(string nameInfo)
                object[] argsNotQRV = new object[] { nameInfo };

                var actualNotQrv = qrv.InvokeStatic("NameInfoNotQrv", argsNotQRV);
                Assert.IsTrue((bool)actualNotQrv == notQRV);

                if ((bool)actualNotQrv == true)
                {
                    return;
                }
                // private static bool NameInfoRangeToBands(string nameInfo, out List<BAND> bandList)

                object[] args = new object[] { nameInfo, null! /* placeholders for out param */ };

                var actual = qrv.InvokeStatic("NameInfoRangeToBands", args);

                List<BAND> bandList = (List<BAND>)args[1];
                Assert.IsTrue((bool)actual == hasRange);

                if ((bool)actual == true)
                {
                    Assert.IsTrue(bandList.SequenceEqual(b));
                    return;
                }
            }
            {
                // private static bool NameInfoSingleBand(string nameInfo, out BAND band)

                object[] args = new object[] { nameInfo, null! /* placeholders for out param */ };

                var actual = qrv.InvokeStatic("NameInfoSingleBand", args);

                var band = (BAND)args[1];
                Assert.IsTrue((bool)actual == hasSingle);
                if ((bool)actual == true)
                {
                    // private static BAND BandFromText(string bandtext)
                    Assert.IsTrue(band == b[0]);
                    return;
                }
            }
            {
                // private static bool NameInfoMultipleBands(string nameInfo, out List<BAND> bandList)

                object[] args = new object[] { nameInfo, null! /* placeholders for out param */ };

                var actual = qrv.InvokeStatic("NameInfoMultipleBands", args);

                List<BAND> bandList = (List<BAND>) args[1];
                Assert.IsTrue((bool)actual == hasBands);
                if ((bool)actual == true)
                {
                    Assert.IsTrue(bandList.SequenceEqual(b));
                    return;
                }
            }
        }
    }
}