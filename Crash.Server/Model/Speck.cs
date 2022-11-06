namespace Crash.Server.Model
{
    /// <summary>
    /// Model.Speck to be stored in SqLite DB. To/From methods convert from SpeckLib.Speck to/from Crash.Server.Model.Speck
    /// </summary>
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

        public static Speck From(SpeckLib.Speck speck)
        {
            var s = new Speck();
            s.Id = speck.Id;
            s.Stamp = speck.Stamp;
            s.Owner = speck.Owner;
            s.Temporary = true;
            s.Payload = speck.Payload;
            return s;
        }

        public SpeckLib.Speck To()
        {
            SpeckLib.Speck s = new SpeckLib.Speck();
            s.Id = this.Id;
            s.Stamp = this.Stamp;
            s.Owner = this.Owner;
            s.Payload = this.Payload;
            return s;
        }

    }

}
