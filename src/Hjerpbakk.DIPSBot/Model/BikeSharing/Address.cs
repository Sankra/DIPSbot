using System;
using System.Diagnostics;
using System.Web;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing {
    [DebuggerDisplay("{Value}", Name = "{Name}")]
    public readonly struct Address {
        public Address(in string name, in double latitude, in double longitude) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = latitude + "," + longitude;
        }

        public Address(in string name, in string value) {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Address(in string value) {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Name = value;
        }

        public string Name { get; }
        public string Value { get; }
        public string UrlEncodedValue => HttpUtility.UrlEncode(Value);

        public override string ToString() => Name;

        public static implicit operator Address(string location) => new Address(location);
    }
}
