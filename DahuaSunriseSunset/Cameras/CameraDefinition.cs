using System.Net;

namespace SunriseSunset
{
	public abstract class CameraDefinition
	{
		public string hostAndPort;
		public string user;
		public string pass;
		public bool https;
		public string dayZoom = "";
		public string dayFocus = "";
		public string nightZoom = "";
		public string nightFocus = "";
		public int secondsBetweenLensCommands = 4;
		public Profile sunriseProfile = Profile.Day;
		public Profile sunsetProfile = Profile.Night;
		public CameraManufacturer manufacturer;

		public CameraDefinition()
		{
		}

		public CameraDefinition(string hostAndPort, string user, string pass, bool https)
		{
			this.hostAndPort = hostAndPort;
			this.user = user;
			this.pass = pass;
			this.https = https;
		}

		public CameraDefinition(string hostAndPort, string user, string pass, bool https, string dayZoom, string dayFocus, string nightZoom, string nightFocus, int lensDelay, Profile sunriseProfile, Profile sunsetProfile) : this(hostAndPort, user, pass, https)
		{
			this.dayZoom = dayZoom;
			this.dayFocus = dayFocus;
			this.nightZoom = nightZoom;
			this.nightFocus = nightFocus;
			this.secondsBetweenLensCommands = lensDelay;
			this.sunriseProfile = sunriseProfile;
			this.sunsetProfile = sunsetProfile;
		}

		public override string ToString()
		{
			return "http" + (https ? "s" : "") + "://" + user + ":" + pass + "@" + hostAndPort + "/";
		}

		public ICredentials GetCredentials()
		{
			if (!string.IsNullOrEmpty(user))
				return new NetworkCredential(user, pass);
			return null;
		}

		public abstract string GetBaseUrl();

		public abstract string GetNightDayUrl(Profile profile);

		public abstract string GetNightDayBody(Profile profile);

		public abstract string GetZoomAndFocusUrl(string zoom, string focus);

		public abstract string GetAutoFocusUrl();
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