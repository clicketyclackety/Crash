using System.Text.Json;
using System.Text.Json.Serialization;

using Crash.Geometry;

namespace Crash.Common.Tests.Serialization
{
	[TestFixture]
	public sealed class CTransformSerializationTests
	{
		const double MIN = -123456789.123456789;
		const double MAX = MIN * -1;

		internal readonly static JsonSerializerOptions TestOptions;

		static CTransformSerializationTests()
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
		public void TestCTransformSerializationMaximums(double x, double y, double z)
		{
			TestCTransformSerializtion(new CTransform(x, y, z));
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void TestCTransformSerializationRandom(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var x = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				var y = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				var z = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				TestCTransformSerializtion(new CTransform(x, y, z));
			}
		}

		private void TestCTransformSerializtion(CTransform cTransform)
		{
			var json = JsonSerializer.Serialize(cTransform, TestOptions);
			var cTransformOut = JsonSerializer.Deserialize<CTransform>(json, TestOptions);
			Assert.That(cTransform.X, Is.EqualTo(cTransformOut.X));
			Assert.That(cTransform.Y, Is.EqualTo(cTransformOut.Y));
			Assert.That(cTransform.Z, Is.EqualTo(cTransformOut.Z));
		}

	}
}
