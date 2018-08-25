using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Toponym.Site.Extensions;
using Toponym.Site.Helpers;
using Toponym.Site.Models;
using Toponym.Site.Services;

namespace Toponym.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataService _dataService;

        public HomeController(DataService dataService) =>
            _dataService = dataService;

        [Route("")]
        [Route("{lang:regex(^(ru|be|en)$)}")]
        public IActionResult Index(string q1, EntryCategory? t1, string lang = "ru")
        {
            if (!CheckHost(Request, out IActionResult result))
                return result;

            var firstCategory =
                t1 > 0 && t1 <= Constants.AllEntryCategories
                    ? (EntryCategory)t1
                    : Constants.AllEntryCategories;

            var language = LangHelper.GetByQueryParam(lang);
            ViewBag.FirstQuery = q1;
            ViewBag.FirstCategory = firstCategory;
            ViewBag.MatchCount = 0;
            ViewBag.Language = language;

            if (string.IsNullOrWhiteSpace(q1))
                return View();

            var regex = RegexHelper.GetRegex(q1, language);

            if (regex != null)
                ViewBag.MatchCount = _dataService.GetEntries(regex, firstCategory, language).Count;

            return View();
        }

        public class EntriesRequest
        {
            public string Query { get; set; }
            public EntryCategory Category { get; set; }
            public Language Language { get; set; }
        }

        [HttpPost]
        [Route("xhr/entries")]
        public IActionResult Entries([FromBody]EntriesRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return JsonResult(ResponseStatus.Failure);

            var regex = RegexHelper.GetRegex(request.Query, request.Language);

            if (regex == null)
                return JsonResult(ResponseStatus.SyntaxError);

            var matched = _dataService.GetEntries(regex, request.Category, request.Language);
            var limited = matched.Take(Constants.EntryCountLimit);
            var data = limited.Select(i => i.ToTransport(request.Language)).ToList();
            return JsonResult(data, matched.Count);
        }

        private static bool CheckHost(HttpRequest request, out IActionResult result)
        {
            var host = request.Host.Host;

            if (host == Constants.DefaultHost || host == "localhost")
            {
                result = null;
                return true;
            }

            var builder = new UriBuilder
            {
                Host = Constants.DefaultHost,
                Port = 80,
                Path = request.Path,
                Query = request.QueryString.Value
            };

            result = new RedirectResult(builder.Uri.ToString(), permanent: true);
            return false;
        }

        private ContentResult JsonResult(ResponseStatus status)
        {
            var json = JsonConvert.SerializeObject(
                new ResponseTransport
                {
                    Status = status
                });

            return Content(json, "application/json");
        }

        private ContentResult JsonResult(List<EntryTransport> entries, int matchCount)
        {
            var json = JsonConvert.SerializeObject(
                new ResponseTransport
                {
                    Status = ResponseStatus.Success,
                    Entries = entries,
                    MatchCount = matchCount
                });

            return Content(json, "application/json");
        }
    }
}
