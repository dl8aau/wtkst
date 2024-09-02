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
            LogState = LOG_STATE.LOG_INACTIVE;
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

        public enum LOG_STATE
        {
            LOG_INACTIVE,
            LOG_SYNCING,
            LOG_IN_SYNC,
        };

        private LOG_STATE logState = LOG_STATE.LOG_INACTIVE;

        public LOG_STATE LogState {
            get { return logState; }
            protected set
            {
                logState = value;
                if (LogStateChanged != null)
                {
                    LogStateChanged(this, new LogStateEventArgs(logState));
                }
            }
        }

        public event EventHandler<LogStateEventArgs> LogStateChanged;
        public class LogStateEventArgs : EventArgs
        {
            /// <summary>
            /// called when LogState changes
            /// </summary>
            public LogStateEventArgs(LOG_STATE LogState)
            {
                this.LogState = LogState;
            }
            public LOG_STATE LogState { get; private set; }
        }

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
