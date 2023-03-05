// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Crash.Geometry;

using Rhino.Geometry;

namespace Crash.Handlers
{

	/// <summary>
	/// Geometry Converter Classes
	/// </summary>
	public static class Geometry
	{

		public static Point3d ToRhino(this CPoint cPoint) => new Point3d(cPoint.X, cPoint.Y, cPoint.Z);
		public static CPoint ToCrash(this Point3d cPoint) => new CPoint(cPoint.X, cPoint.Y, cPoint.Z);

		public static Vector3d ToRhino(this CVector cVector) => new Vector3d(cVector.X, cVector.Y, cVector.Z);
		public static CVector ToCrash(this Vector3d cVector) => new CVector(cVector.X, cVector.Y, cVector.Z);


		public static IEnumerable<Point3d> ToRhino(this IEnumerable<CPoint> cPoints)
		{
			foreach (var cPoint in cPoints)
				yield return cPoint.ToRhino();
		}
		// Reverse


		public static IEnumerable<CPoint> ToRhino(this IEnumerable<Point3d> rhinoPoints)
		{
			foreach (var point3d in rhinoPoints)
				yield return point3d.ToCrash();
		}
		// Reverse


		public static CTransform ToRhino(this Transform transform)
		{
			return new CTransform(transform.M00, transform.M01, transform.M02, transform.M03,
									transform.M10, transform.M11, transform.M12, transform.M13,
									transform.M20, transform.M21, transform.M22, transform.M23,
									transform.M30, transform.M31, transform.M32, transform.M33);
		}
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
