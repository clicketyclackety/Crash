using Rhino.Geometry;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crash.Serialization
{
    public class CameraConverter : JsonConverter<Camera>
    {
        public override Camera Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int targetX = reader.GetInt32();
            int targetY = reader.GetInt32();
            int targetZ = reader.GetInt32();

            Point3d target = new Point3d(targetX, targetY, targetZ);

            int locationX = reader.GetInt32();
            int locationY = reader.GetInt32();
            int locationZ = reader.GetInt32();

            Point3d location = new Point3d(locationX, locationY, locationZ);

            return new Camera(location, target);
        }

        public override void Write(Utf8JsonWriter writer, Camera value, JsonSerializerOptions options)
        {
            Point3d target = value.Target;
            Point3d location = value.Location;

            writer.WriteNumberValue((int)target.X);
            writer.WriteNumberValue((int)target.Y);
            writer.WriteNumberValue((int)target.Z);

            writer.WriteNumberValue((int)location.X);
            writer.WriteNumberValue((int)location.Y);
            writer.WriteNumberValue((int)location.Z);
        }
    }

}
