using System.Text.Json.Serialization;

using Rhino.Geometry;


namespace Crash.UI
{

    [JsonConverter(typeof(Serialization.CameraConverter))]
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

}
