using System.Collections.Generic;

namespace Toponym.Site
{
    public class ResponseTransport
    {
        public ResponseStatus Status { get; set; }
        public List<EntryTransport> Entries { get; set; }
        public int? MatchCount { get; set; }
    }
}
