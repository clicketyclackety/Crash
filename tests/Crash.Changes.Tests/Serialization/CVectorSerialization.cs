using System.Text.Json;
using System.Text.Json.Serialization;

using Crash.Geometry;

namespace Crash.Changes.Tests.Serialization
{
	[TestFixture]
	public sealed class CVectorSerializationTests
	{
		const double MIN = -123456789.123456789;
		const double MAX = MIN * -1;

		internal readonly static JsonSerializerOptions TestOptions;

		static CVectorSerializationTests()
		{
			TestOptions = new JsonSerializerOptions()
			{
				IgnoreReadOnlyFields = true,
				IgnoreReadOnlyProperties = true,
				IncludeFields = true,
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
				ReadCommentHandling = JsonCommentHandling.Skip,
				WriteIndented = true, // TODO : Should this be avoided? Does it add extra memory?
			};
		}

		[TestCase(double.NaN, double.NaN, double.NaN)]
		[TestCase(double.MaxValue, double.MinValue, double.NaN)]
		[TestCase(double.NegativeInfinity, double.PositiveInfinity, double.NaN)]
		public void TestCVectorSerializationMaximums(double x, double y, double z)
		{
			TestCVectorSerializtion(new CVector(x, y, z));
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void TestCVectorSerializationRandom(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var x = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				var y = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				var z = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				TestCVectorSerializtion(new CVector(x, y, z));
			}
		}

		private void TestCVectorSerializtion(CVector CVector)
		{
			var json = JsonSerializer.Serialize(CVector, TestOptions);
			var CVectorOut = JsonSerializer.Deserialize<CVector>(json, TestOptions);
			Assert.That(CVector.X, Is.EqualTo(CVectorOut.X));
			Assert.That(CVector.Y, Is.EqualTo(CVectorOut.Y));
			Assert.That(CVector.Z, Is.EqualTo(CVectorOut.Z));
		}

	}
}
