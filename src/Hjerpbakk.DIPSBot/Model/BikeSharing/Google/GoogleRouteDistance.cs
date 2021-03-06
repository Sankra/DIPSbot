﻿using System.Diagnostics;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Model.BikeSharing.Google {
    public readonly struct GoogleRouteDistance {
        [JsonConstructor]
        public GoogleRouteDistance(Row[] rows, string status) {
            Rows = rows;
            Status = status;
        }

        public Row[] Rows { get; }
        public string Status { get; }
    }

    public readonly struct Row {
        [JsonConstructor]
        public Row(Element[] elements) {
            Elements = elements;
        }

        public Element[] Elements { get; }
    }

    [DebuggerDisplay("{Duration.Text}")]
    public readonly struct Element {
        [JsonConstructor]
        public Element(Duration duration, string status) {
            Duration = duration;
            Status = status;
        }

        public Duration Duration { get; }
        public string Status { get; }
    }

    [DebuggerDisplay("{Text}")]
    public readonly struct Duration {
        [JsonConstructor]
        public Duration(string text, long value) {
            Text = text;
            Value = value;
        }

        public string Text { get; }
        public long Value { get; }
    }
}
