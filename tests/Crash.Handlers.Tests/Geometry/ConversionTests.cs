using System.Collections;
using System.Collections.Generic;

using Crash.Geometry;

using Rhino.Geometry;

namespace Crash.Handlers.Tests.Geometry
{

	[TestFixture]
	public sealed class ConversionTests
	{

		[TestCaseSource(nameof(Valid_PointPairs))]
		public void CPoint_ToPoint3d_Successful(CPoint cpoint, Point3d point)
		{
			Point3d converted = Crash.Handlers.Geometry.ToRhino(cpoint);
			Assert.That(converted, Is.EqualTo(point));
		}


		[TestCaseSource(nameof(Valid_PointPairs))]
		public void Point3d_ToCPoint_Successful(CPoint cpoint, Point3d point)
		{
			CPoint converted = Crash.Handlers.Geometry.ToCrash(point);
			Assert.That(converted, Is.EqualTo(cpoint));
		}

		[TestCaseSource(nameof(Valid_VectorPairs))]
		public void CVector_ToVector3d_Successful(CVector cvec, Vector3d vec)
		{
			Vector3d converted = Crash.Handlers.Geometry.ToRhino(cvec);
			Assert.That(converted, Is.EqualTo(vec));
		}

		[TestCaseSource(nameof(Valid_VectorPairs))]
		public void Vector3d_ToCVector_Successful(CVector cvec, Vector3d vec)
		{
			CVector converted = Crash.Handlers.Geometry.ToCrash(vec);
			Assert.That(converted, Is.EqualTo(cvec));
		}

		[TestCaseSource(nameof(TransformData))]
		public void Transform_ToCTransform(IEnumerable<double> transformDoubles)
		{
			Transform transform = new Transform()
			{

			};
		}

		[TestCaseSource(nameof(TransformData))]
		public void Transform_ToCTransform(double[] transformDoubles)
		{
			CTransform cTransform = new CTransform(transformDoubles);
			Transform transform = Crash.Handlers.Geometry.ToRhino(cTransform);
		}

		public static IEnumerable Valid_VectorPairs
		{
			get
			{
				for (int i = 0; i < 100; i++)
				{
					Vector3d vec = NRhino.Random.Geometry.NVector3d.Any();
					CVector cVec = new CVector(vec.X, vec.Y, vec.Z);
					yield return new TestCaseData(cVec, vec);
				}
			}
		}

		public static IEnumerable Valid_PointPairs
		{
			get
			{
				for (int i = 0; i < 100; i++)
				{
					Point3d point = NRhino.Random.Geometry.NPoint3d.Any();
					CVector cPoint = new CVector(point.X, point.Y, point.Z);
					yield return new TestCaseData(cPoint, point);
				}
			}
		}

		public static IEnumerable InvalidPointPairs
		{
			get
			{
				foreach (var coord in InvalidCoordinates)
				{
					var x = coord.Item1;
					var y = coord.Item1;
					var z = coord.Item1;

					yield return new TestCaseData(new CVector(x, y, z), new Vector3d(x, y, z));
				}
			}
		}

		public static IEnumerable InvalidVectorPairs
		{
			get
			{
				foreach (var coord in InvalidCoordinates)
				{
					var x = coord.Item1;
					var y = coord.Item1;
					var z = coord.Item1;

					yield return new TestCaseData(new CVector(x, y, z), new Vector3d(x, y, z));
				}
			}
		}

		private static IEnumerable<(double, double, double)> InvalidCoordinates
		{
			get
			{
				yield return (double.NaN, double.NaN, double.NaN);
				yield return (double.NaN, 0, 0);
				yield return (0, double.NaN, 0);
				yield return (0, 0, double.NaN);

				yield return (double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
				yield return (double.NegativeInfinity, 0, 0);
				yield return (0, double.NegativeInfinity, 0);
				yield return (0, 0, double.NegativeInfinity);

				yield return (double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
				yield return (double.PositiveInfinity, 0, 0);
				yield return (0, double.PositiveInfinity, 0);
				yield return (0, 0, double.PositiveInfinity);
			}
		}

		public static IEnumerable TransformData
		{
			get
			{
				for (int i = 0; i < 100; i++)
				{
					yield return GetTransformDoubles();
				}
			}
		}

		public static double[] GetTransformDoubles()
		{
			double[] doubles = new double[16];
			for (int i = 0; i < 16; i++)
			{
				doubles[i] = TestContext.CurrentContext.Random.NextDouble(short.MinValue, short.MaxValue);
			}

			return doubles;
		}

	}

}
