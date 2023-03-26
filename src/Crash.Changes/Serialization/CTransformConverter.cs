using System.Text.Json;
using System.Text.Json.Serialization;

using Crash.Geometry;

namespace Crash.Changes.Serialization
{

	/// <summary>Converts a CTransform into json efficiently</summary>
	public sealed class CTransformConverter : JsonConverter<CTransform>
	{

		private static double GetValue(ref Utf8JsonReader reader)
		{
			if (reader.TokenType != JsonTokenType.String)
			{
				return 0;
			}

			string? numberValue = reader.GetString();
			double number = FloatingDoubleConverter.FromString(numberValue);

			reader.Read();

			return number;
		}

		private static void SetValue(ref Utf8JsonWriter writer, int row, int column, CTransform transform)
		{
			double value = transform[row, column];
			string numberValue = FloatingDoubleConverter.ToString(value);

			writer.WriteStringValue(numberValue);
		}

		/// <inheritdoc/>
		public override CTransform Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartArray || !reader.Read())
				throw new JsonException("Couldn't find Array Start");

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

			if (reader.TokenType != JsonTokenType.EndArray)
				throw new JsonException("Couldn't find Array End");

			return transform;
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, CTransform value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();

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

			writer.WriteEndArray();
		}

	}

}
