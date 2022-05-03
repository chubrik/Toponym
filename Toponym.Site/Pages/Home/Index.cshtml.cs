using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Toponym.Site.Pages
{
    public sealed class IndexModel : PageModel
    {
        private readonly DataService _dataService;

        public IndexModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public string? FirstQuery { get; set; }
        public EntryCategory? FirstCategory { get; set; }
        public int? MatchCount { get; set; }
        public Language? Language { get; set; }

        public IActionResult OnGet([FromQuery] string q1, [FromQuery] EntryCategory? t1, string lang = "ru")
        {
            if (!CheckHost(Request, out var redirectUrl))
                return RedirectPermanent(redirectUrl);

            var firstCategory =
                t1 > 0 && t1 <= Constants.AllEntryCategories
                    ? (EntryCategory)t1
                    : Constants.AllEntryCategories;

            var language = LangHelper.GetByQueryParam(lang);
            FirstQuery = q1;
            FirstCategory = firstCategory;
            MatchCount = 0;
            Language = language;

            if (!string.IsNullOrWhiteSpace(q1))
            {
                var regex = RegexHelper.GetRegex(q1, language);

                if (regex != null)
                    MatchCount = _dataService.GetEntries(regex, firstCategory, language).Count;
            }

            return Page();
        }

        private static bool CheckHost(HttpRequest request, out string redirectUrl)
        {
            var host = request.Host.Host;

            if (host == Constants.DefaultHost || host == "localhost")
            {
                redirectUrl = null;
                return true;
            }

            var builder = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttps,
                Host = Constants.DefaultHost,
                Path = request.Path,
                Query = request.QueryString.Value
            };

            redirectUrl = builder.Uri.ToString();
            return false;
        }
    }
}
