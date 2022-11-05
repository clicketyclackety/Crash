using System;

namespace SpeckLib
{

    public sealed class Speck
    {

        Guid _id;
        DateTime _stamp;
        string _owner;

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

        public string Owner 
        {
            get => _owner;
            set => _owner = value;
        }

        public Speck()
        {
            _id = Guid.NewGuid();
            _stamp = DateTime.UtcNow;
        }

        public Speck(Guid id)
        {
            _id = id;
            _stamp = DateTime.UtcNow;
        }


    }

}
