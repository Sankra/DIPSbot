namespace Hjerpbakk.DIPSBot.Model {
    public class BikeShareStation {
        public BikeShareStation(string name, string address, int freeBikes, int availableSpace) {
            Name = name;
            Address = address;
            FreeBikes = freeBikes;
            AvailableSpace = availableSpace;
            // TODO: something about location
        }

        public string Name { get; }
        public string Address { get; }
        public int FreeBikes { get; }
        public int AvailableSpace { get; }

        public override string ToString() => $"{Name}: {FreeBikes} / {AvailableSpace}";
    }
}
