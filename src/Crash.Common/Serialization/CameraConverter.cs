using System.Text.Json;
using System.Text.Json.Serialization;

using Crash.Common.View;
using Crash.Geometry;

namespace Crash.Common.Serialization
{

	/// <summary>
	/// Converts the Camera class to and from JSON.
	/// </summary>
	public class CameraConverter : JsonConverter<Camera>
	{
		/// <inheritdoc/>
		public override Camera Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartArray)
			{
				throw new JsonException();
			}

			if (reader.Read() && reader.TryGetDouble(out double targetX) &&
				reader.Read() && reader.TryGetDouble(out double targetY) &&
				reader.Read() && reader.TryGetDouble(out double targetZ) &&

				reader.Read() && reader.TryGetDouble(out double locationX) &&
				reader.Read() && reader.TryGetDouble(out double locationY) &&
				reader.Read() && reader.TryGetDouble(out double locationZ) &&

				reader.Read() && reader.TryGetInt64(out long ticks) &&

				reader.Read() && reader.TokenType == JsonTokenType.EndArray)
			{
				CPoint location = new CPoint(locationX, locationY, locationZ);
				CPoint target = new CPoint(targetX, targetY, targetZ);

				return new Camera(location, target)
				{
					Time = new DateTime(ticks)
				};
			}
			else
			{
				throw new JsonException();
			}
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Camera value, JsonSerializerOptions options)
		{
			var target = value.Target;
			var location = value.Location;

			writer.WriteStartArray();

			writer.WriteNumberValue(target.X);
			writer.WriteNumberValue(target.Y);
			writer.WriteNumberValue(target.Z);

			writer.WriteNumberValue(location.X);
			writer.WriteNumberValue(location.Y);
			writer.WriteNumberValue(location.Z);

			writer.WriteNumberValue(value.Time.Ticks);

			writer.WriteEndArray();
		}

	}

}
