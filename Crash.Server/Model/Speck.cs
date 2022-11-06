namespace Crash.Server.Model
{
    public class Speck
    {
        Guid _id;
        DateTime _stamp;
        string? _owner;
        bool _temporary;
        string? _lockedBy;
        string? _payload;

        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        public DateTime Stamp
        {
            get => _stamp;
            set => _stamp = value;
        }

        public string? Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public bool Temporary
        {
            get => _temporary;
            set => _temporary = value;
        }

        public string? LockedBy
        {
            get => _lockedBy;
            set => _lockedBy = value;
        }

        public string? Payload
        {
            get => _payload;
            set => _payload = value;
        }

        public Speck()
        {
            _id = Guid.NewGuid();
            _stamp = DateTime.UtcNow;
        }

        internal static Speck From(SpeckLib.Speck speck)
        {
            var s = new Speck();
            s.Id = speck.Id;
            s.Stamp = speck.Stamp;
            s.Owner = speck.Owner;
            s.Temporary = true;
            s.Payload = speck.Payload;
            return s;
        }

        internal static SpeckLib.Speck To(Speck speck)
        {
            SpeckLib.Speck s = new SpeckLib.Speck();
            s.Id = speck.Id;
            s.Stamp = speck.Stamp;
            s.Owner = speck.Owner;
            s.Payload = speck.Payload;
            return s;
        }
    }
}
