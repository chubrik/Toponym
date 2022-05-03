using System.Text.Json.Serialization;

namespace Toponym.Web
{
    public class EntryTransport
    {
        public string Title { get; }

        public EntryType Type { get; }

        [JsonPropertyName("geo")]
        public float[] GeoPoint { get; }

        [JsonPropertyName("screen")]
        public IReadOnlyList<float[]> ScreenPoints { get; }

        public EntryTransport(
            string title, EntryType type, float[] geoPoint, IReadOnlyList<float[]> screenPoints)
        {
            Title = title;
            Type = type;
            GeoPoint = geoPoint;
            ScreenPoints = screenPoints;
        }
    }
}
