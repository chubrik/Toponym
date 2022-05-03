namespace Toponym.Web
{
    public class ResponseTransport
    {
        public ResponseStatus Status { get; }
        public IReadOnlyList<EntryTransport>? Entries { get; }
        public int? MatchCount { get; }

        public ResponseTransport(ResponseStatus status)
        {
            Status = status;
        }

        public ResponseTransport(IReadOnlyList<EntryTransport> entries, int? matchCount)
        {
            Status = ResponseStatus.Success;
            Entries = entries;
            MatchCount = matchCount;
        }
    }
}
