using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading;

namespace Toponym.Site.Pages
{
    [IgnoreAntiforgeryToken]
    public class EntriesModel : PageModel
    {
        private readonly DataService _dataService;

        public EntriesModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public JsonResult OnPost([FromBody] EntriesRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return new(new ResponseTransport
                {
                    Status = ResponseStatus.Failure
                });

            var regex = RegexHelper.GetRegex(request.Query, request.Language);

            if (regex == null)
                return new(new ResponseTransport
                {
                    Status = ResponseStatus.SyntaxError
                });

            var matched = _dataService.GetEntries(regex, request.Category, request.Language);
            var limited = matched.Take(Constants.EntryCountLimit);
            var data = limited.Select(i => i.ToTransport(request.Language)).ToList();

            return new(new ResponseTransport
            {
                Status = ResponseStatus.Success,
                Entries = data,
                MatchCount = matched.Count
            });
        }

        public class EntriesRequest
        {
            public string Query { get; set; }
            public EntryCategory Category { get; set; }
            public Language Language { get; set; }
        }
    }
}
