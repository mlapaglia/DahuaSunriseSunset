namespace SunriseSunset
{
	public class DahuaCameraDefinition : CameraDefinition
	{
		public DahuaCameraDefinition() : base()
		{
		}

		public DahuaCameraDefinition(string hostAndPort, string user, string pass, bool https) : base (hostAndPort, user, pass, https)
		{
		}

		public DahuaCameraDefinition(string hostAndPort, string user, string pass, bool https, string dayZoom, string dayFocus, string nightZoom, string nightFocus, int lensDelay, Profile sunriseProfile, Profile sunsetProfile) : this(hostAndPort, user, pass, https)
		{
			this.dayZoom = dayZoom;
			this.dayFocus = dayFocus;
			this.nightZoom = nightZoom;
			this.nightFocus = nightFocus;
			this.secondsBetweenLensCommands = lensDelay;
			this.sunriseProfile = sunriseProfile;
			this.sunsetProfile = sunsetProfile;
			this.manufacturer = CameraManufacturer.Dahua;
		}

		public override string ToString()
		{
			return "http" + (https ? "s" : "") + "://" + user + ":" + pass + "@" + hostAndPort + "/";
		}

		public override string GetBaseUrl()
		{
			return "http" + (https ? "s" : "") + "://" + hostAndPort;
		}

		public override string GetNightDayUrl(Profile profile)
		{
			return GetBaseUrl() + "/cgi-bin/configManager.cgi?action=setConfig&VideoInMode[0].Config[0]=" + (int)profile;
		}

		public override string GetNightDayBody(Profile profile)
		{
			return string.Empty;
		}

		public override string GetZoomAndFocusUrl(string zoom, string focus)
		{
			return GetBaseUrl() + "/cgi-bin/devVideoInput.cgi?action=adjustFocus&focus=" + focus + "&zoom=" + zoom;
		}

		public override string GetAutoFocusUrl()
		{
			return GetBaseUrl() + "/cgi-bin/devVideoInput.cgi?action=autoFocus";
		}
	}
}