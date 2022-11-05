using System;

namespace SpeckLib
{

    public sealed class Speck
    {

        Guid _id;
        DateTime _stamp;

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

        public Speck()
        {
            _id = Guid.NewGuid();
            _stamp = DateTime.UtcNow;
        }


    }

}
