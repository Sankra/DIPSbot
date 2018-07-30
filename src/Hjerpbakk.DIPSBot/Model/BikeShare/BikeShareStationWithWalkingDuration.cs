namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public class BikeShareStationWithWalkingDuration : BikeShareStation {
        // TODO: use BikeShareStation as property instead
        public BikeShareStationWithWalkingDuration(string name, string address, int freeBikes, int availableSpace, double latitude, double longitude, long walkingDuration) :
            base(name, address, freeBikes, availableSpace, latitude, longitude) {
            WalkingDuration = walkingDuration;
        }

        public long WalkingDuration { get; }
    }
}
