namespace Crash.Geometry
{

	/// <summary>A 3 dimensional representation of a direction.</summary>
	[JsonConverter(typeof(CVectorConverter))]
	public struct CVector
	{

		/// <summary>The X Direction</summary>
		public double X { get; set; } = 0;
		/// <summary>The Y Direction</summary>
		public double Y { get; set; } = 0;
		/// <summary>The Z Direction</summary>
		public double Z { get; set; } = 0;


		/// <summary>Returns a non existant drection.</summary>
		public static CVector Unset => new CVector(double.NaN, double.NaN, double.NaN);

		/// <summary>Returns a CVector at 0,0,0.</summary>
		public static CVector Origin => new CVector(0, 0, 0);

		/// <summary>Empty Constructor</summary>
		public CVector() { }

		/// <summary>Creates a new CVector</summary>
		public CVector(double x = 0, double y = 0, double z = 0)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			if (obj is not CVector cPoint) return false;
			return this == cPoint;
		}

		/// <inheritdoc/>
		public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

		/// <inheritdoc/>
		public override string ToString() => $"{X},{Y},{Z}";

		/// <summary>Returns a Rounded CVector</summary>
		/// <param name="digits">the number</param>
		/// <returns>The number of fractional digits in the return value</returns>
		public CVector Round(int digits = 0) => new(Math.Round(this.X, digits),
												   Math.Round(this.Y, digits),
												   Math.Round(this.Z, digits));

		/// <summary>Conperts a CPoint to a CVector</summary>
		public static implicit operator CVector(CPoint p) => new CVector(p.X, p.Y, p.Z);

		/// <summary>Tests for mathmatic equality</summary>
		public static bool operator ==(CVector v1, CVector v2)
			=> v1.X == v2.X &&
			v1.Y == v2.Y &&
			v1.Z == v2.Z;

		/// <summary>Tests for mathmatic inequality</summary>
		public static bool operator !=(CVector v1, CVector v2) => !(v1 == v2);

		/// <summary>Subtracts p2 from p1</summary>
		public static CVector operator -(CVector v1, CVector v2)
			=> new CVector(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);

		/// <summary>Adds p2 and p1</summary>
		public static CVector operator +(CVector v1, CVector v2)
			=> new CVector(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);

	}

}
