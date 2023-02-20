using System.Text.Json;
using System.Text.Json.Serialization;
using Crash.Geometry;

namespace Crash.Changes.Serialization
{

	/// <summary>
	/// Converts a CPoint into json efficiently
	/// </summary>
	public sealed class CPointConverter : JsonConverter<CPoint>
	{

		public override CPoint Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			if (reader.Read() && reader.TryGetDouble(out double x) &&
				reader.Read() && reader.TryGetDouble(out double y) &&
				reader.Read() && reader.TryGetDouble(out double z) &&
				reader.Read() && reader.TokenType == JsonTokenType.EndArray)
			{
				return new CPoint(x, y, z);
			}
			else
			{
				throw new JsonException();
			}
		}

		public override void Write(Utf8JsonWriter writer, CPoint value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			writer.WriteNumberValue(value.X);
			writer.WriteNumberValue(value.Y);
			writer.WriteNumberValue(value.Z);
			writer.WriteEndArray();
		}
	}
}
