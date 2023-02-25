using System.Text.Json.Serialization;

using Crash.Common.Serialization;
using Crash.Geometry;

namespace Crash.Common.View
{

	/// <summary>
	/// The Camera represents a user view.
	/// </summary>
	[JsonConverter(typeof(CameraConverter))]
	public struct Camera : IEquatable<Camera>
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
			Time > DateTime.MinValue &&
			Time < DateTime.MaxValue;

		public override int GetHashCode() => Location.GetHashCode() ^ Target.GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj is not Camera camera) return false;
			return Equals(camera);
		}

		public bool Equals(Camera other)
			=> this.Location == other.Location &&
			   this.Target == other.Target;

		public override string ToString()
			=> $"Camera {Location}/{Target}";

	}

}
