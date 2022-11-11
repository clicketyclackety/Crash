using SpeckLib;
using System;

namespace SpeckLib
{
    /// <summary>
    /// Model.Speck to be stored in SqLite DB. To/From methods convert from SpeckLib.Speck to/from 
    /// Crash.Server.Model.Speck
    /// </summary>
    public class ServerSpeck : ISpeck
    {

        public DateTime Stamp { get; private set; }

        public Guid Id { get; private set; }

        public string Owner { get; private set; }

        public bool Temporary { get; set; }

        public string? LockedBy { get; set; }

        public string? Payload { get; set; }

        private ServerSpeck()
        {
            Stamp = DateTime.UtcNow;
        }

        public static ISpeck CreateEmpty()
        {
            return new ServerSpeck()
            {
                Id = Guid.NewGuid()
            };
        }

        public static ServerSpeck From(ISpeck speck)
        {
            return new ServerSpeck()
            {
                Stamp = speck.Stamp,
                Id = speck.Id,
                Owner = speck.Owner,
                Payload = speck.Payload,
                LockedBy = speck.Owner,
                Temporary = true
            };
        }

    }

}
