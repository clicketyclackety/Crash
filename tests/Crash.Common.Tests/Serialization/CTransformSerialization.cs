﻿using System.Text.Json;
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

		/*
		[TestCaseSource(typeof(InvalidTransformValues), nameof(InvalidValues.TestCases))]
		public bool TestCTransformSerializationMaximums(double[] values)
		{
			TestCTransformSerialization(new CTransform(values));
			return false;
		}
		*/

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(100)]
		public void TestCTransformSerializationRandom(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var doubleValues = GetRandomTransformValues().ToArray();
				TestCTransformSerialization(new CTransform(doubleValues));
			}
		}

		private IEnumerable<double> GetRandomTransformValues()
		{
			for (double i = 0; i < 16; i++)
			{
				yield return TestContext.CurrentContext.Random.NextDouble(-10_000, 10_000);
			}
		}

		private void TestCTransformSerialization(CTransform cTransform)
		{
			var json = JsonSerializer.Serialize(cTransform, TestOptions);
			var cTransformOut = JsonSerializer.Deserialize<CTransform>(json, TestOptions);

			var cDoublesEnumer = cTransform.GetEnumerator();
			var cDoublesOutEnumer = cTransformOut.GetEnumerator();
			while (cDoublesEnumer.MoveNext() &
				  cDoublesOutEnumer.MoveNext())
			{
				Assert.That(cDoublesEnumer.Current, Is.EqualTo(cDoublesOutEnumer.Current));
			}

		}

	}
}
