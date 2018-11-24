﻿using System;
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

			DateTime rise, set;
			TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
			bool timeZoneAndLongitudeAreCompatible;
			SunHelper.Calc(cfg.latitude, cfg.longitude, out rise, out set, out timeZoneAndLongitudeAreCompatible);
			label1.Text = "Lat " + cfg.latitude + Environment.NewLine
				+ "Lon " + cfg.longitude + Environment.NewLine
				+ "UTC Offset: " + utcOffset.TotalSeconds + " seconds (" + utcOffset.TotalHours + " hours)" + Environment.NewLine
				+ Environment.NewLine
				+ (timeZoneAndLongitudeAreCompatible ? "" : "Your machine's time zone needs to be on the same side " + Environment.NewLine
														  + "of the prime meridian as the longitude you have entered." + Environment.NewLine + Environment.NewLine)
				+ (rise > set ?
				("Sunset at " + set + Environment.NewLine
				+ "Sunrise at " + rise)
				:
				("Sunrise at " + rise + Environment.NewLine
				+ "Sunset at " + set));
		}
	}
}
