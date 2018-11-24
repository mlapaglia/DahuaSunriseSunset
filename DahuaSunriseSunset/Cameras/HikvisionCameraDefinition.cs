using System;
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
			HostAndPort = hostAndPort;
			Username = user;
			Password = pass;
			UseHttps = https;
		}

		public HikvisionCameraDefinition(string hostAndPort, string user, string pass, bool https, string dayZoom, string dayFocus, string nightZoom, string nightFocus, int lensDelay, Profile sunriseProfile, Profile sunsetProfile) : this(hostAndPort, user, pass, https)
		{
			DayZoom = dayZoom;
			DayFocus = dayFocus;
			NightZoom = nightZoom;
			NightFocus = nightFocus;
			SecondsBetweenLensCommands = lensDelay;
			SunriseProfile = sunriseProfile;
			SunsetProfile = sunsetProfile;
			Manufacturer = CameraManufacturer.Hikvision;
		}

		public override Uri GetBaseUri()
		{
			return new Uri("http" + (UseHttps ? "s" : "") + "://" + HostAndPort + "/ISAPI/Image/channels/1");
		}

		public override Uri GetNightDayUri(Profile profile)
		{
			return GetBaseUri();
		}

		public override string GetNightDayBody(Profile profile)
		{
			var nightOrDay = profile.ToString().ToLower();
			return "<ImageChannel version=\"2.0\" xmlns=\"http://www.hikvision.com/ver20/XMLSchema\"><IrcutFilter version=\"2.0\" xmlns=\"http://www.hikvision.com/ver20/XMLSchema\"><IrcutFilterType>" + nightOrDay + "</IrcutFilterType></IrcutFilter></ImageChannel>";
		}

		public override Uri GetAutoFocusUri()
		{
			return GetBaseUri();
		}

		public override Uri GetZoomAndFocusUri(string zoom, string focus)
		{
			return GetBaseUri();
		}
	}
}