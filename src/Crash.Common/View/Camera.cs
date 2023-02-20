using System.Text.Json.Serialization;
using Crash.Common.Serialization;
using Crash.Geometry;

namespace Crash.Common.View
{

	// TODO : Unit Test serialization
	[JsonConverter(typeof(CameraConverter))]
	public struct Camera
	{

		public CPoint Location { get; set; }

		public CPoint Target { get; set; }

		public DateTime Time { get; internal set; }

		public Camera(CPoint location, CPoint target)
		{
			Location = location;
			Target = target;
			Time = DateTime.UtcNow;
		}

		public bool IsValid()
			=> Location != Target &&
			Time > DateTime.MinValue;

	}

}
