using System.Text.Json;
using System.Text.Json.Serialization;

using Crash.Geometry;

namespace Crash.Changes.Tests.Serialization
{
	[TestFixture]
	public sealed class CPointSerializationTests
	{

		internal readonly static JsonSerializerOptions TestOptions;

		static CPointSerializationTests()
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
		public void TestCPointSerializationMaximums(double x, double y, double z)
		{
			TestCPointSerializtion(new CPoint(x, y, z));
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void TestCPointSerializationRandom(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var x = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				var y = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				var z = TestContext.CurrentContext.Random.NextDouble(Int16.MinValue, Int16.MaxValue);
				TestCPointSerializtion(new CPoint(x, y, z));
			}
		}

		private void TestCPointSerializtion(CPoint cpoint)
		{
			var json = JsonSerializer.Serialize(cpoint, TestOptions);
			var cPointOut = JsonSerializer.Deserialize<CPoint>(json, TestOptions);
			Assert.That(cpoint.X, Is.EqualTo(cPointOut.X));
			Assert.That(cpoint.Y, Is.EqualTo(cPointOut.Y));
			Assert.That(cpoint.Z, Is.EqualTo(cPointOut.Z));
		}

	}
}
