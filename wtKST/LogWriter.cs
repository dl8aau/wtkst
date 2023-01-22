using System;
using System.IO;
using System.ComponentModel;

// taken from https://dotnet-snippets.de/snippet/logwriter-klasse/746
namespace wtKST
{
    /// <summary>
    /// Handles a log file
    /// </summary>
    [Description("Handles a log file")]
    public class LogWriter : Component
    {
        #region delegates
        /// <summary>
        /// Provides a method, wich handles events from written messages
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Event arguments</param>
        [Description("Provides a method, wich handles events from written messages")]
        public delegate void LogWriterEventHandler(object sender, LogWriterEventArgs e);
        #endregion

        #region fields
        /// <summary>
        /// Stores the log save path
        /// </summary>
        private string path = String.Empty;

        /// <summary>
        /// Stores the file name format
        /// </summary>
        private string fileFormat = "log_{0:d}.log";

        /// <summary>
        /// Stores the message format
        /// </summary>
        private string messageFormat = "{0:t}: {1}";
        #endregion

        #region events
        /// <summary>
        /// Fired, when a message was written
        /// </summary>
        [Description("Fired, when a message was written")]
        public event LogWriterEventHandler MessageWritten;

        /// <summary>
        /// Fired, when a new log file was createdFired, when a message was written
        /// </summary>
        [Description("Fired, when a new log file was createdFired, when a message was written")]
        public event EventHandler FileCreated;
        #endregion

        #region properties
        /// <summary>
        /// Specifyes the log save path
        /// </summary>
        [Description("Specifyes the log save path")]
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }

        /// <summary>
        /// Specifyes the file name format
        /// </summary>
        [Description("Specifyes the file name format")]
        [DefaultValue("log_{0:d}.log")]
        public string FileFormat
        {
            get
            {
                return this.fileFormat;
            }
            set
            {
                this.fileFormat = value;
            }
        }

        /// <summary>
        /// Specifyes the message format
        /// </summary>
        [Description("Specifyes the message format")]
        [DefaultValue("{0:t}: {1}")]
        public string MessageFormat
        {
            get
            {
                return this.messageFormat;
            }
            set
            {
                this.messageFormat = value;
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public LogWriter()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Specifyes the log save path</param>
        public LogWriter(string path)
        {
            this.path = path;
        }
        #endregion

        #region public members
        /// <summary>
        /// Writes a message
        /// </summary>
        /// <param name="message">Specifyes the message to be written</param>
        /// <returns>Wheather the writing progress was successful</returns>
        public bool WriteMessage(string message)
        {
            // if directory not exists, cancel
            if (!Directory.Exists(this.path))
                throw new DirectoryNotFoundException("The directory specifyed in Path was not found");

            // holds the actual date and time
            DateTime time = DateTime.Now;

            // creates the file name
            string fileName;
            if (String.IsNullOrEmpty(this.path))
                fileName = System.IO.Path.DirectorySeparatorChar.ToString();
            else
            {
                fileName = this.path;

                if (fileName[fileName.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                    fileName += System.IO.Path.DirectorySeparatorChar;
            }
            try
            {
                fileName += String.Format(this.fileFormat, time);
            }
            catch (FormatException)
            {
                throw new FormatException("FileFormat has the wrong format.");
            }
            catch
            {
                return false;
            }

            // build message line
            string line;
            try
            {
                // creates the line
                line = String.Format(this.messageFormat, new object[] { time, message });
            }
            catch (FormatException)
            {
                throw new FormatException("MessageFormat has the wrong format.");
            }
            catch
            {
                return false;
            }

            try
            {
                StreamWriter writer;

                // open file
                if (!File.Exists(fileName))
                {
                    writer = new StreamWriter(fileName);
                    OnFileCreated(EventArgs.Empty);
                }
                else
                    writer = new StreamWriter(fileName, true);

                // write message line
                writer.WriteLine(line);

                // close file
                writer.Close();

                OnMessageWritten(new LogWriterEventArgs(time, message));

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region protected members
        /// <summary>
        /// Fires the MessageWritten event
        /// </summary>
        /// <param name="e"></param>
        protected void OnMessageWritten(LogWriterEventArgs e)
        {
            if (MessageWritten != null)
                MessageWritten(this, e);
        }

        /// <summary>
        /// Fires the FileCreated event
        /// </summary>
        /// <param name="e"></param>
        protected void OnFileCreated(EventArgs e)
        {
            if (FileCreated != null)
                FileCreated(this, e);
        }
        #endregion
    }

    #region LogWriterEventArgs class
    /// <summary>
    /// LogWriterEventArgs stores information about the sent log message
    /// </summary>
    public class LogWriterEventArgs : EventArgs
    {
        #region fields
        /// <summary>
        /// Stores the message post time
        /// </summary>
        private DateTime time;

        /// <summary>
        /// Stores the message
        /// </summary>
        private string message;
        #endregion

        #region properties
        /// <summary>
        /// Specifyes the message post time
        /// </summary>
        [Description("Specifyes the message post time")]
        public DateTime Time
        {
            get
            {
                return this.time;
            }
        }

        /// <summary>
        /// Specifyes the message
        /// </summary>
        [Description("Specifyes the message")]
        public string Message
        {
            get
            {
                return this.message;
            }
        }
        #endregion

        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="time">Specifyes the message post time</param>
        /// <param name="message">Specifyes the message</param>
        public LogWriterEventArgs(DateTime time, string message)
            : base()
        {
            this.time = time;
            this.message = message;
        }
        #endregion
    }
    #endregion
}