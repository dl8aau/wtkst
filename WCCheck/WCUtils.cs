using System;

namespace WCCheck
{
	public class WCUtils
	{
		public static void Debug(string S, int priority)
		{
			Console.WriteLine(string.Concat(new object[]
			{
				DateTime.UtcNow,
				"(",
				priority,
				") - ",
				S
			}));
		}
	}
}
