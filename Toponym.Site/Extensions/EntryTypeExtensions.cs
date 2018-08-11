using System;
using Toponym.Core.Models;
using Toponym.Site.Models;

namespace Toponym.Site.Extensions
{
    public static class EntryTypeExtensions
    {
        public static Category ToCategory(this EntryType type)
        {
            if (type == EntryType.Unknown)
                return Category.Unknown;

            if (type >= EntryType.Populated && type < EntryType.Water)
                return Category.Populated;

            if (type >= EntryType.Water && (int)type < 30)
                return Category.Water;

            //if (type >= 30)
            //    return Category.Locality;

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
