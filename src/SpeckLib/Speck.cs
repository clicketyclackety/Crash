using SpeckLib;
using System;

namespace SpeckLib
{

    /// <summary>
    /// Model.Speck that is stored in SqLite DB.
    /// </summary>
    public sealed class Speck : ISpeck
    {

        public DateTime Stamp { get; set; }

        public Guid Id { get; set; }

        public string Owner { get; set; }

        public bool Temporary { get; set; }

        public string? LockedBy { get; set; }

        public string? Payload { get; set; }


        public Speck() { }

        public Speck(Guid id, string owner, string? payload)
        {
            Id = id;
            Owner = owner;
            Payload = payload;
            Stamp = DateTime.UtcNow;
        }

        public Speck(ISpeck speck)
        {
            Stamp = speck.Stamp;
            Id = speck.Id;
            Owner = speck.Owner;
            Payload = speck.Payload;
            LockedBy = speck.Owner;
            Temporary = true;
        }

        public static Speck CreateEmpty()
        {
            return new Speck()
            {
                Id = Guid.NewGuid()
            };
        }

    }

}
