using System;
using System.Net;

namespace SunriseSunset
{
	public abstract class CameraDefinition
	{
		public string HostAndPort;
		public string Username;
		public string Password;
		public bool UseHttps;
		public string DayZoom = "";
		public string DayFocus = "";
		public string NightZoom = "";
		public string NightFocus = "";
		public int SecondsBetweenLensCommands = 4;
		public Profile SunriseProfile = Profile.Day;
		public Profile SunsetProfile = Profile.Night;
		public CameraManufacturer Manufacturer;

		public CameraDefinition()
		{
		}

		public CameraDefinition(string hostAndPort, string user, string pass, bool https)
		{
			HostAndPort = hostAndPort;
			Username = user;
			Password = pass;
			UseHttps = https;
		}

		public CameraDefinition(string hostAndPort, string user, string pass, bool https, string dayZoom, string dayFocus, string nightZoom, string nightFocus, int lensDelay, Profile sunriseProfile, Profile sunsetProfile) : this(hostAndPort, user, pass, https)
		{
			DayZoom = dayZoom;
			DayFocus = dayFocus;
			NightZoom = nightZoom;
			NightFocus = nightFocus;
			SecondsBetweenLensCommands = lensDelay;
			SunriseProfile = sunriseProfile;
			SunsetProfile = sunsetProfile;
		}

		public ICredentials GetCredentials()
		{
			if (!string.IsNullOrEmpty(Username))
			{
				return new NetworkCredential(Username, Password);
			}

			return null;
		}

		public override string ToString()
		{
			return "http" + (UseHttps ? "s" : "") + "://" + Username + ":" + Password + "@" + HostAndPort + "/";
		}

		public abstract Uri GetBaseUri();

		public abstract Uri GetNightDayUri(Profile profile);

		public abstract string GetNightDayBody(Profile profile);

		public abstract Uri GetZoomAndFocusUri(string zoom, string focus);

		public abstract Uri GetAutoFocusUri();
	}

	public enum Profile
	{
		Day = 0,
		Night = 1,
		Normal = 2
	}

	public enum CameraManufacturer
	{
		Dahua = 0,
		Hikvision = 1
	}
}