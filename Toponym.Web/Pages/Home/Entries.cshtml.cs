namespace Toponym.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EntriesModel : PageModel
{
    private readonly DataService _dataService;

    public EntriesModel(DataService dataService)
    {
        _dataService = dataService;
    }

    public JsonResult OnPost(string query, int category, Language language)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new(new ResponseTransport(ResponseStatus.Failure));

        var regex = RegexHelper.GetRegex(query, language);

        if (regex == null)
            return new(new ResponseTransport(ResponseStatus.SyntaxError));

        var matched = _dataService.GetEntries(regex, (EntryCategory)category, language);
        var limited = matched.Take(Constants.EntryCountLimit);
        var data = limited.Select(i => i.ToTransport(language)).ToList();
        return new(new ResponseTransport(entries: data, matchCount: matched.Count));
    }
}
