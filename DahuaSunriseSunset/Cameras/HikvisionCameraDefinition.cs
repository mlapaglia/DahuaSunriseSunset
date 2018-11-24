using System.Net;

namespace SunriseSunset
{
	public class HikvisionCameraDefinition : CameraDefinition
	{
		public HikvisionCameraDefinition() : base()
		{
		}

		public HikvisionCameraDefinition(string hostAndPort, string user, string pass, bool https) : base(hostAndPort, user, pass, https)
		{
			this.hostAndPort = hostAndPort;
			this.user = user;
			this.pass = pass;
			this.https = https;
		}

		public HikvisionCameraDefinition(string hostAndPort, string user, string pass, bool https, string dayZoom, string dayFocus, string nightZoom, string nightFocus, int lensDelay, Profile sunriseProfile, Profile sunsetProfile) : this(hostAndPort, user, pass, https)
		{
			this.dayZoom = dayZoom;
			this.dayFocus = dayFocus;
			this.nightZoom = nightZoom;
			this.nightFocus = nightFocus;
			this.secondsBetweenLensCommands = lensDelay;
			this.sunriseProfile = sunriseProfile;
			this.sunsetProfile = sunsetProfile;
			this.manufacturer = CameraManufacturer.Hikvision;
		}

		public override string ToString()
		{
			return "http" + (https ? "s" : "") + "://" + user + ":" + pass + "@" + hostAndPort + "/";
		}

		public override string GetBaseUrl()
		{
			return "http" + (https ? "s" : "") + "://" + hostAndPort + "/ISAPI/Image/channels/1";
		}

		public override string GetNightDayUrl(Profile profile)
		{
			return "http" + (https ? "s" : "") + "://" + hostAndPort + "/ISAPI/Image/channels/1";
		}

		public override string GetNightDayBody(Profile profile)
		{
			var nightOrDay = profile.ToString().ToLower();
			return "<ImageChannel version=\"2.0\" xmlns=\"http://www.hikvision.com/ver20/XMLSchema\"><IrcutFilter version=\"2.0\" xmlns=\"http://www.hikvision.com/ver20/XMLSchema\"><IrcutFilterType>" + nightOrDay + "</IrcutFilterType></IrcutFilter></ImageChannel>";
		}

		public override string GetAutoFocusUrl()
		{
			return GetBaseUrl();
		}

		public override string GetZoomAndFocusUrl(string zoom, string focus)
		{
			return GetBaseUrl();
		}
	}
}