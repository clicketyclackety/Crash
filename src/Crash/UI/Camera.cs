using Crash.Tables;
using Eto.Drawing;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Crash.UI
{

    public class PointConverter : JsonConverter<Point3d>
    {
        public override Point3d Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int x = reader.GetInt32();
            int y = reader.GetInt32();
            int z = reader.GetInt32();

            return new Point3d(x, y, z);
        }

        public override void Write(Utf8JsonWriter writer, Point3d value, JsonSerializerOptions options)
        {
            // ints are smaller, and accuracy is non-critical
            writer.WriteNumberValue((int)value.X);
            writer.WriteNumberValue((int)value.Y);
            writer.WriteNumberValue((int)value.Z);
        }
    }

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


    // [Serializable]
    [JsonConverter(typeof(CameraConverter))]
    public sealed class Camera
    {

        public Point3d Location { get; set; }

        public Point3d Target { get; set; }

        public Camera(Point3d location, Point3d target)
        {
            Location = location;
            Target = target;
        }

        public string ToJSON()
        {
            // Having lots of weird issues here ...
            /*
            var options = new JsonSerializerOptions();
            return JsonSerializer.Serialize(this, options);
            */
            return $"{(int)Location.X},{(int)Location.Y},{(int)Location.Z}," +
                    $"{(int)Target.X},{(int)Target.Y},{(int)Target.Z}";
        }

        public static Camera? FromJSON(string json)
        {
            try
            {
                // Having lots of weird issues here ...
                // Camera camera = JsonSerializer.Deserialize<Camera>(json);

                string[] points = json.Split(',');
                int[] pointValues = points.Select(p => int.Parse(p)).ToArray();
                Point3d location = new Point3d(pointValues[0], pointValues[1], pointValues[2]);
                Point3d target = new Point3d(pointValues[3], pointValues[4], pointValues[5]);

                Camera camera = new Camera(location, target);

                return camera;
            }
            catch(Exception ex)
            {
                ;
                return null;
            }
        }

    }

    public sealed class CameraSpeck : ISpeck
    {
        ISpeck Speck { get; set; }

        public Camera Camera { get; private set; }

        public DateTime Stamp => Speck.Stamp;

        public Guid Id => Speck.Id;

        public string Owner => Speck.Owner;

        public bool Temporary => Speck.Temporary;

        public string? LockedBy => Speck.LockedBy;

        public string? Payload => Speck.Payload;

        public CameraSpeck(ISpeck speck)
        {
            this.Speck = speck;
        }

        public static CameraSpeck CreateNew(Camera camera)
        {
            ISpeck cameraSpeck = new Speck(Guid.NewGuid(), UserTable.CurrentUser?.Name, camera.ToJSON());
            return new CameraSpeck(cameraSpeck);
        }

    }

}
