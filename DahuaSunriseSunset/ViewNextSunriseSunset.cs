using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SunriseSunset
{
	public partial class ViewNextSunriseSunset : Form
	{
		public ViewNextSunriseSunset()
		{
			InitializeComponent();
		}

		private void ViewNextSunriseSunset_Load(object sender, EventArgs e)
		{
			SunriseSunsetConfig cfg = new SunriseSunsetConfig();
			cfg.Load();

			TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

			SunHelper.SunHelperCalculation sunCalculation = SunHelper.CalculateDailySunEvents(cfg.Latitude, cfg.Longitude);

			label1.Text = "Lat " + cfg.Latitude + Environment.NewLine
				+ "Lon " + cfg.Longitude + Environment.NewLine
				+ "UTC Offset: " + utcOffset.TotalSeconds + " seconds (" + utcOffset.TotalHours + " hours)" + Environment.NewLine
				+ Environment.NewLine
				+ (sunCalculation.TimeZoneAndLongitudeAreCompatible ? "" : "Your machine's time zone needs to be on the same side " + Environment.NewLine
														  + "of the prime meridian as the longitude you have entered." + Environment.NewLine + Environment.NewLine)
				+ (sunCalculation.NextRise > sunCalculation.NextSet ?
				("Sunset at " + sunCalculation.NextSet + Environment.NewLine
				+ "Sunrise at " + sunCalculation.NextRise)
				:
				("Sunrise at " + sunCalculation.NextRise + Environment.NewLine
				+ "Sunset at " + sunCalculation.NextSet));
		}
	}
}
