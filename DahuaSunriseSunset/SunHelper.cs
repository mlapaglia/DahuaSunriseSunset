using System;
using CoordinateSharp;

namespace SunriseSunset
{
	public static class SunHelper
	{
		public class SunHelperCalculation
		{
			public DateTime NextRise { get; set; }

			public DateTime NextSet { get; set; }

			public bool TimeZoneAndLongitudeAreCompatible { get; set; }

			public SunHelperCalculation(DateTime nextRise, DateTime nextSet, bool timeZoneAndLongitudeAreCompatible)
			{
				NextRise = nextRise;
				NextSet = nextSet;
				TimeZoneAndLongitudeAreCompatible = timeZoneAndLongitudeAreCompatible;
			}
		}

		/// <summary>
		/// Calculates the next sunrise and sunset that occur AFTER this moment in local time.
		/// If providing offset hours arguments, the rise and set times will be adjusted by those offsets
		/// (e.g. a rise time of 5 AM with an offset of -0.5 hours will result in the rise time being considered as 4:30 AM)
		/// </summary>
		public static SunHelperCalculation CalculateDailySunEvents(double latitude, double longitude, double sunriseOffsetHours = 0, double sunsetOffsetHours = 0)
		{
			SunHelperCalculation calculation = new SunHelperCalculation(DateTime.MinValue, DateTime.MinValue, true);

			DateTime now = DateTime.UtcNow;

			for (int offsetDays = 0; offsetDays < 366; offsetDays++)
			{
				DateTime calcDay = now.AddDays(offsetDays);
				Coordinate coordinate = new Coordinate(latitude, longitude, calcDay);

				if (!calculation.TimeZoneAndLongitudeAreCompatible)
				{
					return calculation;
				}

				if (calculation.NextRise == DateTime.MinValue
					&& coordinate.CelestialInfo.SunRise != null
					&& coordinate.CelestialInfo.SunRise.Value.AddHours(sunriseOffsetHours) > now)
				{
					calculation.NextRise = coordinate.CelestialInfo.SunRise.Value.AddHours(sunriseOffsetHours).ToLocalTime();
				}

				if (calculation.NextSet == DateTime.MinValue
					&& coordinate.CelestialInfo.SunSet != null
					&& coordinate.CelestialInfo.SunSet.Value.AddHours(sunsetOffsetHours) > now)
				{
					calculation.NextSet = coordinate.CelestialInfo.SunSet.Value.AddHours(sunsetOffsetHours).ToLocalTime();
				}

				if (calculation.NextRise != DateTime.MinValue
					&& calculation.NextSet != DateTime.MinValue)
				{
					return calculation;
				}
			}

			return calculation;
		}
	}
}
