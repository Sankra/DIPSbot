using System;
using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    [DebuggerDisplay("{Address} has {FreeBikes} / {AvailableSpace} bikes", Name = "{Name}")]
    public class BikeShareStation {
        public BikeShareStation(string name, string address, int freeBikes, int availableSpace, double latitude, double longitude) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (address == null) {
                throw new ArgumentNullException(nameof(address));
            }

            Name = name;
            Address = address;
            FreeBikes = freeBikes;
            AvailableSpace = availableSpace;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; }
        public string Address { get; }
        public int FreeBikes { get; }
        public int AvailableSpace { get; }
        public double Latitude { get; }
        public double Longitude { get; }
    }
}
