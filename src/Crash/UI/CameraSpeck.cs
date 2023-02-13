using Crash.Document;
using Crash.Tables;

namespace Crash.UI
{
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

        public static CameraSpeck CreateNew(Camera camera, string userName)
        {
            ISpeck cameraSpeck = new Speck(Guid.NewGuid(), userName, camera.ToJSON());
            return new CameraSpeck(cameraSpeck);
        }

    }

}
