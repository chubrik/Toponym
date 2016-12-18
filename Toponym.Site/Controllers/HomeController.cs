﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Toponym.Site.Helpers;
using Toponym.Site.Models;
using Toponym.Site.Services;

namespace Toponym.Site.Controllers {
    public class HomeController : Controller {

        private readonly DataService _dataService;

        public HomeController(DataService dataService) {
            _dataService = dataService;
        }

        [Route("")]
        [Route("{lang:regex(^(ru|be|en)$)}")]
        public IActionResult Index(string q1, Group? type, string lang = "ru") {

            IActionResult result;

            if (!CheckHost(Request, out result))
                return result;

            Language language;

            switch (lang) {
                case "ru":
                    language = Language.Russian;
                    break;
                case "be":
                    language = Language.Belarusian;
                    break;
                case "en":
                    language = Language.English;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            ViewBag.FirstQuery = q1;
            ViewBag.FirstType = type ?? Group.All;
            ViewBag.MatchCount = 0;
            ViewBag.Language = language;

            if (string.IsNullOrWhiteSpace(q1))
                return View();

            var regex = RegexHelper.GetRegex(q1);

            if (regex != null)
                ViewBag.MatchCount = _dataService.GetItems(regex, type ?? Group.All, language).Count;

            return View();
        }

        public class ItemsRequest {
            public string Query { get; set; }
            public Group Type { get; set; }
            public Language Language { get; set; }
        }

        [HttpPost]
        [Route("ajax/items")]
        public IActionResult Items([FromBody]ItemsRequest request) {

            if (string.IsNullOrWhiteSpace(request.Query))
                return JsonResult(ResponseStatus.Failure);

            var regex = RegexHelper.GetRegex(request.Query);

            if (regex == null)
                return JsonResult(ResponseStatus.SyntaxError);

            var matched = _dataService.GetItems(regex, request.Type, request.Language);
            var limited = matched.GetRange(0, Math.Min(matched.Count, SiteConstants.ItemCountLimit));
            var data = limited.Select(i => new ItemData(i, request.Language)).ToList();
            return JsonResult(data, matched.Count);
        }

        private static bool CheckHost(HttpRequest request, out IActionResult result) {
            var host = request.Host.Host;

            if (host == SiteConstants.DefaultHost || host == "localhost") {
                result = null;
                return true;
            }

            var builder = new UriBuilder {
                Host = SiteConstants.DefaultHost,
                Port = 80,
                Path = request.Path,
                Query = request.QueryString.Value
            };

            result = new RedirectResult(builder.Uri.ToString(), permanent: true);
            return false;
        }

        private ContentResult JsonResult(ResponseStatus status) {

            var json = JsonConvert.SerializeObject(
                new ResponseData {
                    Status = status
                });

            return Content(json, "application/json");
        }

        private ContentResult JsonResult(List<ItemData> items, int matchCount) {

            var json = JsonConvert.SerializeObject(
                new ResponseData {
                    Status = ResponseStatus.Success,
                    Items = items,
                    MatchCount = matchCount
                });

            return Content(json, "application/json");
        }
    }
}
