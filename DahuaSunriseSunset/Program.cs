using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using BPUtil;
using BPUtil.Forms;

namespace SunriseSunset
{
	static class Program
	{
		static ServiceManager sm;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			string exePath = Assembly.GetExecutingAssembly().Location;
			Globals.Initialize(exePath);
			Directory.SetCurrentDirectory(Globals.ApplicationDirectoryBase);

			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

			if (Environment.UserInteractive)
			{
				string Title = "SunriseSunset " + Assembly.GetEntryAssembly().GetName().Version.ToString() + " Service Manager";
				DahuaSunriseSunset.Utilities.Logger.LogMessage(Title + " Startup", System.Diagnostics.EventLogEntryType.Information);
				string ServiceName = "SunriseSunset";
				ButtonDefinition btnConfigure = new ButtonDefinition("Configure Service", btnConfigure_Click);
				ButtonDefinition btnSimulateSunrise = new ButtonDefinition("Simulate Sunrise", btnSimulateSunrise_Click);
				ButtonDefinition btnViewNextTimes = new ButtonDefinition("View rise/set", btnViewNextTimes_Click);
				ButtonDefinition btnSimulateSunset = new ButtonDefinition("Simulate Sunset", btnSimulateSunset_Click);
				ButtonDefinition[] customButtons = new ButtonDefinition[] { btnConfigure, btnSimulateSunrise, btnViewNextTimes, btnSimulateSunset };

				Application.Run(sm = new ServiceManager(Title, ServiceName, customButtons));
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
				new SunriseSunsetService()
				};
				ServiceBase.Run(ServicesToRun);
			}
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Logger.Debug(e.Exception, "Application_ThreadException");
		}

		static Form cfgForm = null;

		private static void btnConfigure_Click(object sender, EventArgs e)
		{
			if (cfgForm == null)
			{
				cfgForm = new ConfigurationForm();
				cfgForm.StartPosition = FormStartPosition.CenterParent;
				cfgForm.FormClosed += CfgForm_FormClosed;
				cfgForm.Show(sm);
			}
			else
				cfgForm.Focus();
		}

		private static void CfgForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			cfgForm = null;
		}

		private static void btnViewNextTimes_Click(object sender, EventArgs e)
		{
			ViewNextSunriseSunset f = new ViewNextSunriseSunset();
			f.StartPosition = FormStartPosition.CenterParent;
			f.Show(sm);
		}
		private static void btnSimulateSunrise_Click(object sender, EventArgs e)
		{
			ServiceWrapper.TriggerSunActions(DateTime.Now.AddMinutes(5), Profile.Day);
		}
		private static void btnSimulateSunset_Click(object sender, EventArgs e)
		{
			ServiceWrapper.TriggerSunActions(DateTime.Now.AddMinutes(5), Profile.Night);
		}
	}
}
