using System.Text.Json.Serialization;

using Rhino.Geometry;


namespace Crash.UI
{

    [JsonConverter(typeof(Serialization.CameraConverter))]
    public sealed class Camera
    {

        public Point3d Location { get; set; }

        public Point3d Target { get; set; }

        public DateTime Time { get; internal set; }

        public Camera(Point3d location, Point3d target)
        {
            Location = location;
            Target = target;
            Time = DateTime.UtcNow;
        }

        public string ToJSON()
        {
            // Having lots of weird issues here ...
            /*
            var options = new JsonSerializerOptions();
            return JsonSerializer.Serialize(this, options);
            */
            return $"{(int)Location.X},{(int)Location.Y},{(int)Location.Z}," +
                    $"{(int)Target.X},{(int)Target.Y},{(int)Target.Z}," +
                    $"{Time.Ticks}";
        }

        public static Camera? FromJSON(string json)
        {
            try
            {
                // Having lots of weird issues here ...
                // Camera camera = JsonSerializer.Deserialize<Camera>(json);

                string[] points = json.Split(',');
                long[] intValues = points.Select(p => long.Parse(p)).ToArray();
                Point3d location = new Point3d(intValues[0], intValues[1], intValues[2]);
                Point3d target = new Point3d(intValues[3], intValues[4], intValues[5]);
                DateTime time = new DateTime(intValues[6]);

                Camera camera = new Camera(location, target)
                {
                    Time = time
                };

                return camera;
            }
            catch(Exception ex)
            {
                ;
                return null;
            }
        }

    }

}
