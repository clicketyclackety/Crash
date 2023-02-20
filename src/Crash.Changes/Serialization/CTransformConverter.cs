using System.Text.Json;
using System.Text.Json.Serialization;
using Crash.Geometry;

namespace Crash.Changes.Serialization
{
	public sealed class CTransformConverter : JsonConverter<CTransform>
	{
		/*
		const string m00_KEY = "00";
		const string m01_KEY = "01";
		const string m02_KEY = "02";
		const string m03_KEY = "03";

		const string m10_KEY = "10";
		const string m11_KEY = "11";
		const string m12_KEY = "12";
		const string m13_KEY = "13";

		const string m20_KEY = "20";
		const string m21_KEY = "21";
		const string m22_KEY = "22";
		const string m23_KEY = "23";

		const string m30_KEY = "30";
		const string m31_KEY = "31";
		const string m32_KEY = "32";
		const string m33_KEY = "33";
		*/

		private static double GetValue(ref Utf8JsonReader reader)
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				throw new JsonException("Couldn't find property Name");
			}
			reader.Read();

			if (reader.TokenType != JsonTokenType.Number)
			{
				throw new JsonException("Couldn't find Number Value");
			}
			double value = reader.GetDouble();

			reader.Read();

			return value;
		}

		private static void SetValue(ref Utf8JsonWriter writer, int row, int column, CTransform transform)
		{
			writer.WriteNumberValue(transform[row, column]);
		}

		public override CTransform Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
		{
			CTransform transform = new CTransform(
				GetValue(ref reader), // [0,0]
				GetValue(ref reader), // [0,1]
				GetValue(ref reader), // [0,2]
				GetValue(ref reader), // [0,3]

				GetValue(ref reader), // [1,0]
				GetValue(ref reader), // [1,1]
				GetValue(ref reader), // [1,2]
				GetValue(ref reader), // [1,3]

				GetValue(ref reader), // [2,0]
				GetValue(ref reader), // [2,1]
				GetValue(ref reader), // [2,2]
				GetValue(ref reader), // [2,3]

				GetValue(ref reader), // [3,0]
				GetValue(ref reader), // [3,1]
				GetValue(ref reader), // [3,2]
				GetValue(ref reader)  // [3,3]
			);

			return transform;
		}

		public override void Write(Utf8JsonWriter writer, CTransform value, JsonSerializerOptions options)
		{
			SetValue(ref writer, 0, 0, value);
			SetValue(ref writer, 0, 1, value);
			SetValue(ref writer, 0, 2, value);
			SetValue(ref writer, 0, 3, value);

			SetValue(ref writer, 1, 0, value);
			SetValue(ref writer, 1, 1, value);
			SetValue(ref writer, 1, 2, value);
			SetValue(ref writer, 1, 3, value);

			SetValue(ref writer, 2, 0, value);
			SetValue(ref writer, 2, 1, value);
			SetValue(ref writer, 2, 2, value);
			SetValue(ref writer, 2, 3, value);

			SetValue(ref writer, 3, 0, value);
			SetValue(ref writer, 3, 1, value);
			SetValue(ref writer, 3, 2, value);
			SetValue(ref writer, 3, 3, value);
		}

	}
}
