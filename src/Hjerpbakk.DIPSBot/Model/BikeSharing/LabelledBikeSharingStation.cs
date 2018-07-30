namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    public readonly struct LabelledBikeSharingStation {
        public LabelledBikeSharingStation(char label, BikeSharingStation bikeSharingStation) {
            Label = label;
            BikeSharingStation = bikeSharingStation;
        }

        public char Label { get; }
        public BikeSharingStation BikeSharingStation { get; }
    }
}
