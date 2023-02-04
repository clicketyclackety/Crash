using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Crash.UI
{

    [Serializable]
    public sealed class Camera
    {

        public Point3d Location { get; private set; }

        public Point3d Target { get; private set; }

        [JsonIgnore]
        public User Owner { get; private set; }

        public Camera(Point3d location, Point3d target, User user)
        {
            Location = location;
            Target = target;
            this.Owner = user;
        }
        public string ToJSON()
        {
            var options = new JsonSerializerOptions();
            return JsonSerializer.Serialize(this, options);
        }

        public static Camera? FromJSON(string json)
        {
            return JsonSerializer.Deserialize<Camera>(json);
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
            ISpeck cameraSpeck = new Speck(Guid.NewGuid(), camera.Owner.Name, camera.ToJSON());
            return new CameraSpeck(cameraSpeck);
        }

    }

}
