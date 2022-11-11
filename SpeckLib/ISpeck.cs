using System;
using System.Collections.Generic;
using System.Text;

namespace SpeckLib
{

    public interface ISpeck
    {
        public DateTime Stamp { get; }

        public Guid Id { get; }

        public string Owner { get; }

        public string Payload { get; }
    }

}
