namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    public readonly struct BikeSharingStationsInformation {
        public BikeSharingStationsInformation(string[] response, LabelledBikeSharingStation[] labelledBikeSharingStations) {
            Response = response;
            LabelledBikeSharingStations = labelledBikeSharingStations;
        }

        public string[] Response { get; }
        public LabelledBikeSharingStation[] LabelledBikeSharingStations { get; }
    }
}
