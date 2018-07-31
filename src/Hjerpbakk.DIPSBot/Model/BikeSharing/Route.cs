using System;
using System.Diagnostics;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{EncodedTransportationMode}", Name = "{From} - {To}")]
    public readonly struct Route {
        public Route(in Address from, in Address to, in TransportationMode transportationMode) {
            From = from;
            To = to;
            TransportationMode = transportationMode;
        }

        public Address From { get; }
        public Address To { get; }
        public TransportationMode TransportationMode { get; }
        public string EncodedTransportationMode => Enum.GetName(typeof(TransportationMode), TransportationMode).ToLower();
    }
}
