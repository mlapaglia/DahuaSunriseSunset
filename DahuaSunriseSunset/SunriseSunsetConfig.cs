using System.Collections.Generic;
using System.Xml.Serialization;
using BPUtil;

namespace SunriseSunset
{
	[XmlInclude(typeof(DahuaCameraDefinition))]
	[XmlInclude(typeof(HikvisionCameraDefinition))]
	public class SunriseSunsetConfig : SerializableObjectBase
	{
		public double latitude = 0;
		public double longitude = 0;
		public double sunriseOffsetHours = 0;
		public double sunsetOffsetHours = 0;
		public List<CameraDefinition> Cameras = new List<CameraDefinition>();
	}
}
