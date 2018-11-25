using System.Diagnostics;

namespace DahuaSunriseSunset.Utilities
{
	public static class Logger
	{
		public static void LogMessage(string message, EventLogEntryType entryType)
		{
			using (EventLog eventLog = new EventLog("Application"))
			{
				eventLog.Source = "SunriseSunset";
				eventLog.WriteEntry(message, entryType);
			}
		}
	}
}
