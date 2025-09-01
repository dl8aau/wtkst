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
}