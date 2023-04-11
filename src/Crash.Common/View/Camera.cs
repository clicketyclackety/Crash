using System.Text.Json.Serialization;

using Crash.Common.Serialization;
using Crash.Geometry;

namespace Crash.Common.View
{

	/// <summary>The Camera represents a user view with two points.</summary>
	[JsonConverter(typeof(CameraConverter))]
	public struct Camera : IEquatable<Camera>
	{

		/// <summary>The location of the viewpont of the camera</summary>
		public CPoint Location { get; set; }

		/// <summary>The target viewpoint of the camera</summary>
		public CPoint Target { get; set; }

		/// <summary>A datetime stamp for the Camera</summary>
		public DateTime Stamp { get; internal set; }

		/// <summary>Creates a Camera</summary>
		public Camera(CPoint location, CPoint target)
		{
			Location = location;
			Target = target;
			Stamp = DateTime.UtcNow;
		}

		/// <summary>A non-existant Camera</summary>
		public static Camera None => new(CPoint.None, CPoint.None);

		/// <summary>Checks for Validity</summary>
		public bool IsValid()
			=> Location != Target &&
			   Location != CPoint.None &&
			   Target != CPoint.None &&
			Stamp > DateTime.MinValue &&
			Stamp < DateTime.MaxValue;

		/// <inheritdoc/>
		public override int GetHashCode() => Location.GetHashCode() ^ Target.GetHashCode();

		/// <summary>Equality Comparison</summary>
		public override bool Equals(object? obj)
		{
			if (obj is not Camera camera) return false;
			return Equals(camera);
		}

		/// <summary>Equality Comparison</summary>
		public bool Equals(Camera other) => this == other;

		/// <summary>Equality Comparison</summary>
		public static bool operator ==(Camera c1, Camera c2)
			=> c1.Location == c2.Location && c1.Target == c2.Target;

		/// <summary>Inqquality Comparison</summary>
		public static bool operator !=(Camera c1, Camera c2) => !(c1 == c2);

		/// <inheritdoc/>
		public override string ToString() => $"Camera {Location}/{Target}";

	}

}
