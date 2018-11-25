using DahuaSunriseSunset.Utilities;
using System.Reflection;
using System.ServiceProcess;

namespace SunriseSunset
{
	public partial class SunriseSunsetService : ServiceBase
	{
		public SunriseSunsetService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Logger.LogMessage("SunriseSunset " + Assembly.GetEntryAssembly().GetName().Version.ToString() + " Service OnStart", System.Diagnostics.EventLogEntryType.Information);
			ServiceWrapper.Start();
		}

		protected override void OnStop()
		{
			Logger.LogMessage("SunriseSunset " + Assembly.GetEntryAssembly().GetName().Version.ToString() + " Service OnStop", System.Diagnostics.EventLogEntryType.Information);
			ServiceWrapper.Stop();
		}
	}
}
