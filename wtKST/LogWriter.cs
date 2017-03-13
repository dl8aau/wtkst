using System;
using System.ComponentModel;
using System.IO;

namespace wtKST
{
	[Description("Handles a log file")]
	public class LogWriter : Component
	{
		[Description("Provides a method, wich handles events from written messages")]
		public delegate void LogWriterEventHandler(object sender, LogWriterEventArgs e);

		private string path = string.Empty;

		private string fileFormat = "log_{0:d}.log";

		private string messageFormat = "{0:t}: {1}";

		[Description("Fired, when a message was written")]
		public event LogWriter.LogWriterEventHandler MessageWritten;

		[Description("Fired, when a new log file was createdFired, when a message was written")]
		public event EventHandler FileCreated;

		[Description("Specifyes the log save path")]
		public string Path
		{
			get
			{
				return path;
			}
			set
			{
                path = value;
			}
		}

		[DefaultValue("log_{0:d}.log"), Description("Specifyes the file name format")]
		public string FileFormat
		{
			get
			{
				return fileFormat;
			}
			set
			{
                fileFormat = value;
			}
		}

		[DefaultValue("{0:t}: {1}"), Description("Specifyes the message format")]
		public string MessageFormat
		{
			get
			{
				return messageFormat;
			}
			set
			{
                messageFormat = value;
			}
		}

		public LogWriter()
		{
		}

		public LogWriter(string path)
		{
			this.path = path;
		}

		public bool WriteMessage(string message)
		{
			if (!Directory.Exists(path))
			{
				throw new DirectoryNotFoundException("The directory specifyed in Path was not found");
			}
			DateTime time = DateTime.Now;
			string fileName;
			if (string.IsNullOrEmpty(path))
			{
				fileName = System.IO.Path.DirectorySeparatorChar.ToString();
			}
			else
			{
				fileName = path;
				if (fileName[fileName.Length - 1] != System.IO.Path.DirectorySeparatorChar)
				{
					fileName += System.IO.Path.DirectorySeparatorChar;
				}
			}
			bool result;
			try
			{
				fileName += string.Format(fileFormat, time);
			}
			catch (FormatException)
			{
				throw new FormatException("FileFormat has the wrong format.");
			}
			catch
			{
				result = false;
				return result;
			}
			string line;
			try
			{
				line = string.Format(messageFormat, new object[]
				{
					time,
					message
				});
			}
			catch (FormatException)
			{
				throw new FormatException("MessageFormat has the wrong format.");
			}
			catch
			{
				result = false;
				return result;
			}
			try
			{
				StreamWriter writer;
				if (!File.Exists(fileName))
				{
					writer = new StreamWriter(fileName);
                    OnFileCreated(EventArgs.Empty);
				}
				else
				{
					writer = new StreamWriter(fileName, true);
				}
				writer.WriteLine(line);
				writer.Close();
                OnMessageWritten(new LogWriterEventArgs(time, message));
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		protected void OnMessageWritten(LogWriterEventArgs e)
		{
			if (MessageWritten != null)
			{
                MessageWritten(this, e);
			}
		}

		protected void OnFileCreated(EventArgs e)
		{
			if (FileCreated != null)
			{
                FileCreated(this, e);
			}
		}
	}
}
