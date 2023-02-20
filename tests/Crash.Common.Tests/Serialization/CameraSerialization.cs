using System.Text.Json;
using System.Text.Json.Serialization;

using Crash.Common.View;

namespace Crash.Changes.Tests.Serialization
{
	[TestFixture]
	public sealed class CameraSerialization
	{
		const double MIN = -123456789.123456789;
		const double MAX = MIN * -1;

		internal readonly static JsonSerializerOptions TestOptions;

		static CameraSerialization()
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
		public void TestCameraSerializationMaximums(double x, double y, double z)
		{
			TestCameraSerializtion(new Camera(x, y, z));
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void TestCameraSerializationRandom(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var x = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				var y = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				var z = TestContext.CurrentContext.Random.NextDouble(MIN, MAX);
				TestCameraSerializtion(new Camera(x, y, z));
			}
		}

		private void TestCameraSerializtion(Camera Camera)
		{
			var json = JsonSerializer.Serialize(Camera, TestOptions);
			var CameraOut = JsonSerializer.Deserialize<Camera>(json, TestOptions);
			Assert.That(Camera.X, Is.EqualTo(CameraOut.X));
			Assert.That(Camera.Y, Is.EqualTo(CameraOut.Y));
			Assert.That(Camera.Z, Is.EqualTo(CameraOut.Z));
		}

	}
}
