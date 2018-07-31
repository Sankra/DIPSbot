using System;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    public readonly struct DirectionsAndImage {
        public DirectionsAndImage(in string directions, in string imageUrl) {
            Directions = directions ?? throw new ArgumentNullException(nameof(directions));
            ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
        }

        public string Directions { get; }
        public string ImageUrl { get; }
    }
}
