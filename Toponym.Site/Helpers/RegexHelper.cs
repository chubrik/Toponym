using System;
using System.Text.RegularExpressions;
using Toponym.Core.Extensions;

namespace Toponym.Site.Helpers {
    public class RegexHelper {

        public static Regex GetRegex(string query) {

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException(nameof(query));

            query = query.ToSimple();

            // простой синтаксис
            if (Regex.IsMatch(query, @"^[а-яa-z\s/'-]+$", RegexOptions.IgnoreCase)) {
                var parts = query.Split('/');

                for (var x = 0; x < parts.Length; x++) {
                    var part = parts[x].Trim();
                    var needEdges = false;

                    if (part.Length >= 2) {

                        // в кавычках - "минск"
                        if (part.StartsWith("'") && part.EndsWith("'")) {
                            part = part.Substring(1, Math.Max(part.Length - 2, 0));
                            needEdges = true;
                        }
                        else {
                            // в середине дефис - гор-ки
                            if (Regex.IsMatch(part, @"[а-яa-z']-[а-яa-z']", RegexOptions.IgnoreCase)) {
                                part = Regex.Replace(part, @"(?<=[а-яa-z'])-(?=[а-яa-z'])", @"[а-яa-z'-]+", RegexOptions.IgnoreCase);
                                needEdges = true;
                            }

                            // по краям дефисы - -оло-
                            if (part.StartsWith("-") && part.EndsWith("-")) {
                                part = "[^^ -]" + part.Substring(1, Math.Max(part.Length - 2, 0)) + "[^$ -]";
                                needEdges = false;
                            }

                            // в начале дефис - -ки
                            else if (part.StartsWith("-")) {
                                part = "[^^ -]" + part.Substring(1) + "($|[ -])";
                                needEdges = false;
                            }

                            // на конце дефис - гор-
                            else if (part.EndsWith("-")) {
                                part = "(^|[ -])" + part.Substring(0, part.Length - 1) + "[^$ -]";
                                needEdges = false;
                            }
                        }
                    }

                    // строгие края
                    if (needEdges)
                        part = "(^|[ -])" + part + "($|[ -])";

                    parts[x] = part;
                }

                query = string.Join("|", parts);
            }

            try {
                return new Regex(query, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException) {
                return null;
            }
        }
    }
}
