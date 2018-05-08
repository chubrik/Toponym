using System;
using Toponym.Core.Models;
using Toponym.Site.Models;

namespace Toponym.Site.Extensions
{
    public static class ItemTypeExtensions
    {
        public static Category ToCategory(this ItemType type)
        {
            if (type == ItemType.Unknown)
                return Category.Unknown;

            var code = (int)type;

            if (code >= 100 && code < 200)
                return Category.Populated;

            if (code >= 200 && code < 300)
                return Category.Water;

            if (code >= 300 && code < 400)
                return Category.Locality;

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
