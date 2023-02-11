using System;
using System.Data;

namespace WinTest
{
    public abstract class WtLogEventArgs : EventArgs
    {
        public DataRow QSO { get; }
    }
    public abstract class WinTestLogBase : IDisposable
    {
        public DataTable QSO { get; protected set; }
        public string MyLoc { get; protected set; }

        public delegate bool LogWriteMessageDelegate(string s);
        protected LogWriteMessageDelegate LogWrite;

        protected void Error(string methodname, string Text)
        {
            if (LogWrite != null)
                LogWrite("Error - <" + methodname + "> " + Text);
        }
        public void Clear_QSOs()
        {
            QSO.Clear();
        }

        public abstract string getStatus();

        public abstract void Get_QSOs(string tmp);

        public abstract void Dispose();

        class WtLogEventArgs : EventArgs
        {
            /// <summary>
            /// Win-Test message
            /// </summary>
            public WtLogEventArgs(DataRow QSO)
            {
                this.QSO = QSO;
            }
            public DataRow QSO { get; private set; }
        }

        public event EventHandler AddQSO;


        public WinTestLogBase(LogWriteMessageDelegate mylog)
        {
            QSO = new DataTable("QSO");
            QSO.Columns.Add("CALL");
            QSO.Columns.Add("BAND");
            QSO.Columns.Add("TIME");
            QSO.Columns.Add("SENT");
            QSO.Columns.Add("RCVD");
            QSO.Columns.Add("LOC");
            LogWrite = mylog;
        }
    }
}
