using System;
using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{FreeBikes} free bikes / {AvailableSpace} free locks", Name = "{Name}")]
    public readonly struct BikeSharingStation {
        public BikeSharingStation(in string name, in int freeBikes, in int availableSpace, in double latitude, in double longitude) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = new Address(name, latitude, longitude);
            FreeBikes = freeBikes;
            AvailableSpace = availableSpace;
        }

        public string Name { get; }
        public Address Address { get; }
        public int FreeBikes { get; }
        public int AvailableSpace { get; }

    }
}
