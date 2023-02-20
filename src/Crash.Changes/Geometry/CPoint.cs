using System.Text.Json.Serialization;
using Crash.Changes.Serialization;

namespace Crash.Geometry
{

	/// <summary>
	/// A 3 dimensional representation of a point.
	/// </summary>
	[JsonConverter(typeof(CPointConverter))]
	public struct CPoint
	{

		public double X { get; set; } = 0;
		public double Y { get; set; } = 0;
		public double Z { get; set; } = 0;


		/// <summary>Returns a non existant point.</summary>
		public static CPoint Unset => new CPoint(double.NaN, double.NaN, double.NaN);

		/// <summary>Returns a Point at 0,0,0.</summary>
		public static CPoint Origin => default(CPoint);


		public CPoint() { }

		public CPoint(double x = 0, double y = 0, double z = 0)
		{
			X = x;
			Y = y;
			Z = z;
		}


		public override bool Equals(object obj)
		{
			if (obj is not CPoint cPoint) return false;
			return this == cPoint;
		}

		public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();


		public static implicit operator CPoint(CVector v) => new CPoint(v.X, v.Y, v.Z);

		public static bool operator ==(CPoint v1, CPoint v2)
			=> v1.X == v2.X &&
			v1.Y == v2.Y &&
			v1.Z == v2.Z;
		public static bool operator !=(CPoint v1, CPoint v2) => !(v1 == v2);

		public static CPoint operator -(CPoint v1, CPoint v2)
			=> new CPoint(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);

		public static CPoint operator +(CPoint v1, CPoint v2)
			=> new CPoint(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
	}

}
