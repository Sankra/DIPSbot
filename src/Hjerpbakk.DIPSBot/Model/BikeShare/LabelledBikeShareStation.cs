namespace Hjerpbakk.DIPSBot.Model.BikeShare {
    public readonly struct LabelledBikeShareStation {
        public LabelledBikeShareStation(char label, BikeShareStation bikeShareStation) {
            Label = label;
            BikeShareStation = bikeShareStation;
        }

        public char Label { get; }
        public BikeShareStation BikeShareStation { get; }
    }
}
