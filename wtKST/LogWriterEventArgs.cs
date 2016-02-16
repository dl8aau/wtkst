using System;
using System.ComponentModel;

namespace wtKST
{
	public class LogWriterEventArgs : EventArgs
	{
		private DateTime time;

		private string message;

		[Description("Specifyes the message post time")]
		public DateTime Time
		{
			get
			{
				return this.time;
			}
		}

		[Description("Specifyes the message")]
		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public LogWriterEventArgs(DateTime time, string message)
		{
			this.time = time;
			this.message = message;
		}
	}
}
