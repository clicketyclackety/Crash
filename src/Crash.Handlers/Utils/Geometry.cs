// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Crash.Geometry;

using Rhino.Geometry;

namespace Crash.Handlers
{

	/// <summary>Crash to RhinoGeometry Converter Classes</summary>
	public static class Geometry
	{
		/// <summary>Converts a CPoint to a Point3d</summary>
		public static Point3d ToRhino(this CPoint cPoint) => new(cPoint.X, cPoint.Y, cPoint.Z);
		/// <summary>Converts a Point3d to a CPoint</summary>
		public static CPoint ToCrash(this Point3d cPoint) => new(cPoint.X, cPoint.Y, cPoint.Z);

		/// <summary>Converts a CVector to a Vector3d</summary>
		public static Vector3d ToRhino(this CVector cVector) => new(cVector.X, cVector.Y, cVector.Z);
		/// <summary>Converts a Vector3d to a CVector</summary>
		public static CVector ToCrash(this Vector3d cVector) => new(cVector.X, cVector.Y, cVector.Z);

		/// <summary>Converts a CTransform to a Transform</summary>
		public static CTransform ToCrash(this Transform transform)
		{
			return new CTransform(transform.M00, transform.M01, transform.M02, transform.M03,
									transform.M10, transform.M11, transform.M12, transform.M13,
									transform.M20, transform.M21, transform.M22, transform.M23,
									transform.M30, transform.M31, transform.M32, transform.M33);
		}
		/// <summary>Converts a Transform to a CTransform</summary>
		public static Transform ToRhino(this CTransform transform)
		{
			return new Transform()
			{
				M00 = transform[0, 0],
				M01 = transform[0, 1],
				M02 = transform[0, 2],
				M03 = transform[0, 3],

				M10 = transform[1, 0],
				M11 = transform[1, 1],
				M12 = transform[1, 2],
				M13 = transform[1, 3],

				M20 = transform[2, 0],
				M21 = transform[2, 1],
				M22 = transform[2, 2],
				M23 = transform[2, 3],

				M30 = transform[3, 0],
				M31 = transform[3, 1],
				M32 = transform[3, 2],
				M33 = transform[3, 3]
			};
		}

	}

}
