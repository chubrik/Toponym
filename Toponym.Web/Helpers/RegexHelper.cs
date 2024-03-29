﻿namespace Toponym.Web;

using System.Diagnostics;
using System.Text.RegularExpressions;

public static class RegexHelper
{
    private const string SoftLeft = "[^^ ]";
    private const string SoftRight = "[^$ ]";
    private const string StrictLeft = "(^| |-)";
    private const string StrictRight = "($| |-)";

    public static Regex? GetRegex(string query, Language language)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(query));

        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentNullException(nameof(query));

        query = Regex.Replace(query, "[`‘’\"«»„“”]", "'");
        query = Regex.Replace(query, "[–—−]", "-");

        if (language == Language.Russian || language == Language.Belarusian)
            query = Regex.Replace(query, "ё", "е", RegexOptions.IgnoreCase);

        if (language == Language.Belarusian)
        {
            query = Regex.Replace(query, "[иі]", "i", RegexOptions.IgnoreCase); // Cyrillic "i" to latin
            query = Regex.Replace(query, "ў", "у", RegexOptions.IgnoreCase);
            query = Regex.Replace(query, "щ", "шч", RegexOptions.IgnoreCase);
            query = Regex.Replace(query, "ъ", "'", RegexOptions.IgnoreCase);
        }

        if (Regex.IsMatch(query, @"^[а-яa-z\s/'-]+$", RegexOptions.IgnoreCase))
        {
            // It is simple query, not regex

            var parts = query.Split('/');

            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();

                if (part.Length < 2)
                    continue;

                if (part.StartsWith("'") && part.EndsWith("'"))
                    part = StrictLeft + part[1..^1] + StrictRight;

                else if (part.Contains('-'))
                {
                    // Hyphens in the middle
                    part = Regex.Replace(part, "(?<=[а-яa-z'])-(?=[а-яa-z'])", "[а-яa-z'-]+", RegexOptions.IgnoreCase);

                    if (part.StartsWith("-"))
                        part = SoftLeft + part[1..];
                    else
                        part = StrictLeft + part;

                    if (part.EndsWith("-"))
                        part = part[0..^1] + SoftRight;
                    else
                        part += StrictRight;
                }

                parts[i] = part;
            }

            query = string.Join("|", parts);
        }

        try
        {
            return new Regex(query, RegexOptions.IgnoreCase);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
