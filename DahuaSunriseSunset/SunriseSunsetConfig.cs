using System.Collections.Generic;
using System.Xml.Serialization;
using BPUtil;

namespace SunriseSunset
{
	[XmlInclude(typeof(DahuaCameraDefinition))]
	[XmlInclude(typeof(HikvisionCameraDefinition))]
	public class SunriseSunsetConfig : SerializableObjectBase
	{
		public double Latitude = 0;
		public double Longitude = 0;
		public double SunriseOffsetHours = 0;
		public double SunsetOffsetHours = 0;
		public List<CameraDefinition> Cameras = new List<CameraDefinition>();
	}
}
