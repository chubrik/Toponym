using System;
using Toponym.Core.Models;
using Toponym.Site.Models;

namespace Toponym.Site.Extensions
{
    public static class EntryTypeExtensions
    {
        public static EntryCategory ToCategory(this EntryType type)
        {
            if (type == EntryType.Unknown)
                return EntryCategory.Unknown;

            if (type >= EntryType.Populated && type < EntryType.Water)
                return EntryCategory.Populated;

            if (type >= EntryType.Water && type < EntryType.Locality)
                return EntryCategory.Water;

            if (type >= EntryType.Locality)
                return EntryCategory.Locality;

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
