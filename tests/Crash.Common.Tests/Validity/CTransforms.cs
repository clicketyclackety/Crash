using System.Collections;

using Crash.Geometry;

namespace Crash.Common.Tests.Validity
{

	[TestFixture]
	public sealed class CTransformValidity
	{

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void IsValidExplicit(int count)
		{
			for (int i = 0; i < count; i++)
			{
				double m00 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m01 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m02 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m03 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);

				double m10 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m11 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m12 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m13 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);

				double m20 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m21 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m22 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m23 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);

				double m30 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m31 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m32 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				double m33 = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);

				CTransform transform = new CTransform(m00, m01, m02, m03,
														m10, m11, m12, m13,
														m20, m21, m22, m23,
														m30, m31, m32, m33);


				Assert.That(transform.IsValid(), Is.True);
			}
		}

		[TestCaseSource(typeof(InvalidValues), nameof(InvalidValues.TestCases))]
		public bool IsNotValid(double[] mValues)
		{
			CTransform transform = new CTransform(mValues);
			return transform.IsValid();
		}

	}

	public sealed class InvalidTransformValues
	{
		public static IEnumerable TestCases
		{
			get
			{
				yield return new TestCaseData(new CPoint(0, 0, 0), new CPoint(0, 0, 0), DateTime.Now).Returns(false);
				yield return new TestCaseData(CPoint.Unset, CPoint.Unset, DateTime.MaxValue).Returns(false);
				yield return new TestCaseData(CPoint.Unset, CPoint.Unset, DateTime.MinValue).Returns(false);
				yield return new TestCaseData(CPoint.Origin, new CPoint(1, 2, 3), DateTime.MinValue).Returns(false);
			}
		}
	}

}
