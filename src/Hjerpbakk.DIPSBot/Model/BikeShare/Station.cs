using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    [DebuggerDisplay("{Address} has {FreeBikes}/{AvailableSpace} bikes", Name = "{Name}")]
    public class Station {
        public Station(string name, string address, int freeBikes, int availableSpace, double latitude, double longitude) {
            Name = name;
            Address = address;
            FreeBikes = freeBikes;
            AvailableSpace = availableSpace;
            // TODO: something about location
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; }
        public string Address { get; }
        public int FreeBikes { get; }
        public int AvailableSpace { get; }
        public double Latitude { get; }
        public double Longitude { get; }

        public override string ToString() => $"{Name}: {FreeBikes} / {AvailableSpace}";
    }
}
