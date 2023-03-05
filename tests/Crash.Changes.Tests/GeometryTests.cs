using Crash.Geometry;

namespace Crash.Changes.Tests
{
	[TestFixture]
	public sealed class GeometryTests
	{

		[TestCase(0, 1, 2)]
		[TestCase(double.NaN, double.NaN, double.NaN)]
		[TestCase(double.MaxValue, double.MinValue, double.NaN)]
		public void CreateCPoint(double x, double y, double z)
		{
			CPoint point = new CPoint(x, y, z);
			Assert.That(x, Is.EqualTo(point.X));
			Assert.That(y, Is.EqualTo(point.Y));
			Assert.That(z, Is.EqualTo(point.Z));
		}

		[Test]
		public void CPointConsts()
		{
			var unset = CPoint.None;
			Assert.That(unset.X, Is.EqualTo(double.NaN));
			Assert.That(unset.Y, Is.EqualTo(double.NaN));
			Assert.That(unset.Z, Is.EqualTo(double.NaN));

			unset.X = 200;
			unset.Y = 300;
			unset.Z = 500;

			var newUnset = CPoint.None;
			Assert.That(newUnset.X, Is.EqualTo(double.NaN));
			Assert.That(newUnset.Y, Is.EqualTo(double.NaN));
			Assert.That(newUnset.Z, Is.EqualTo(double.NaN));
		}

		[TestCase(0, 1, 2)]
		[TestCase(double.NaN, double.NaN, double.NaN)]
		[TestCase(double.MaxValue, double.MinValue, double.NaN)]
		public void ImplicitConvertCPointToCVector(double x, double y, double z)
		{
			CVector cPoint = new CPoint(x, y, z);
			Assert.That(x, Is.EqualTo(cPoint.X));
			Assert.That(y, Is.EqualTo(cPoint.Y));
			Assert.That(z, Is.EqualTo(cPoint.Z));
		}

		[TestCase(0, 1, 2)]
		[TestCase(double.NaN, double.NaN, double.NaN)]
		[TestCase(double.MaxValue, double.MinValue, double.NaN)]
		public void CreateCVector(double x, double y, double z)
		{
			CVector point = new CVector(x, y, z);
			Assert.That(x, Is.EqualTo(point.X));
			Assert.That(y, Is.EqualTo(point.Y));
			Assert.That(z, Is.EqualTo(point.Z));
		}

		[Test]
		public void CVectorConsts()
		{
			var unset = CVector.Unset;
			Assert.That(unset.X, Is.EqualTo(double.NaN));
			Assert.That(unset.Y, Is.EqualTo(double.NaN));
			Assert.That(unset.Z, Is.EqualTo(double.NaN));

			unset.X = 200;
			unset.Y = 300;
			unset.Z = 500;

			var newUnset = CVector.Unset;
			Assert.That(newUnset.X, Is.EqualTo(double.NaN));
			Assert.That(newUnset.Y, Is.EqualTo(double.NaN));
			Assert.That(newUnset.Z, Is.EqualTo(double.NaN));
		}

		[TestCase(0, 1, 2)]
		[TestCase(double.NaN, double.NaN, double.NaN)]
		[TestCase(double.MaxValue, double.MinValue, double.NaN)]
		public void ImplicitConvertCVectorToCPoint(double x, double y, double z)
		{
			CPoint cPoint = new CVector(x, y, z);
			Assert.That(x, Is.EqualTo(cPoint.X));
			Assert.That(y, Is.EqualTo(cPoint.Y));
			Assert.That(z, Is.EqualTo(cPoint.Z));
		}

	}
}
