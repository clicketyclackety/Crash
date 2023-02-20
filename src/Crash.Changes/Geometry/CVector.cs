using System.Text.Json.Serialization;

using Crash.Changes.Serialization;

namespace Crash.Geometry
{

	/// <summary>
	/// A 3 dimensional representation of a direction.
	/// </summary>
	[JsonConverter(typeof(CVectorConverter))]
	public struct CVector
	{

		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }


		/// <summary>Returns a non existant drection.</summary>
		public static CVector Unset => new CVector(double.NaN, double.NaN, double.NaN);


		public CVector() { }

		public CVector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}


		public override bool Equals(object obj)
		{
			if (obj is not CVector cPoint) return false;
			return this == cPoint;
		}

		public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

		public override string ToString() => $"{X},{Y},{Z}";

		public static implicit operator CVector(CPoint p) => new CVector(p.X, p.Y, p.Z);

		public static bool operator ==(CVector v1, CVector v2)
			=> v1.X == v2.X &&
			v1.Y == v2.Y &&
			v1.Z == v2.Z;
		public static bool operator !=(CVector v1, CVector v2) => !(v1 == v2);

		public static CVector operator -(CVector v1, CVector v2)
			=> new CVector(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);

		public static CVector operator +(CVector v1, CVector v2)
			=> new CVector(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);

	}

}
