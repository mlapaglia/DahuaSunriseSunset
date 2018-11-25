using System;

namespace SunriseSunset
{
	public class DahuaCameraDefinition : CameraDefinition
	{
		public DahuaCameraDefinition() : base()
		{
		}

		public DahuaCameraDefinition(string hostAndPort,
			string user,
			string pass,
			bool https) : base (hostAndPort,
				user,
				pass,
				https)
		{
		}

		public DahuaCameraDefinition(string hostAndPort,
			string user,
			string pass,
			bool https,
			string dayZoom,
			string dayFocus,
			string nightZoom,
			string nightFocus,
			int lensDelay,
			Profile sunriseProfile,
			Profile sunsetProfile) : base(hostAndPort,
				user,
				pass,
				https,
				dayZoom,
				dayFocus,
				nightZoom,
				nightFocus,
				lensDelay,
				sunriseProfile,
				sunsetProfile)
		{
			Manufacturer = CameraManufacturer.Dahua;
		}

		public override Uri GetBaseUri()
		{
			return new Uri("http" + (UseHttps ? "s" : "") + "://" + HostAndPort);
		}

		public override Uri GetNightDayUri(Profile profile)
		{
			return new Uri(GetBaseUri() + "cgi-bin/configManager.cgi?action=setConfig&VideoInMode[0].Config[0]=" + (int)profile);
		}

		public override string GetNightDayBody(Profile profile)
		{
			return string.Empty;
		}

		public override Uri GetZoomAndFocusUri(string zoom, string focus)
		{
			return new Uri(GetBaseUri() + "cgi-bin/devVideoInput.cgi?action=adjustFocus&focus=" + focus + "&zoom=" + zoom);
		}

		public override Uri GetAutoFocusUri()
		{
			return new Uri(GetBaseUri() + "cgi-bin/devVideoInput.cgi?action=autoFocus");
		}
	}
}