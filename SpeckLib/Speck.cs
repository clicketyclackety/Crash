using SpeckLib;
using System;

namespace SpeckLib
{
    /// <summary>
    /// Model.Speck to be stored in SqLite DB. To/From methods convert from SpeckLib.Speck to/from 
    /// Crash.Server.Model.Speck
    /// </summary>
    public class Speck
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

        public static Speck CreateEmpty()
        {
            return new Speck()
            {
                Id = Guid.NewGuid()
            };
        }

        public static Speck From(Speck speck)
        {
           Speck newSpeck = new Speck()
            {
                Stamp = speck.Stamp,
                Id = speck.Id,
                Owner = speck.Owner,
                Payload = speck.Payload,
                LockedBy = speck.Owner,
                Temporary = true
            };

            return newSpeck;
        }

    }

}
