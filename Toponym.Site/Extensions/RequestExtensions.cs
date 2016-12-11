using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Toponym.Site.Extensions {
    public static class RequestExtensions {

        // https://goo.gl/nkgsV8
        public static bool IsFacebookBot(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("facebookexternalhit") || userAgent.Contains("Facebot");
        }

        // http://goo.gl/cEVwvQ
        public static bool IsVkBot(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("vkShare");
        }

        // https://goo.gl/rp1ZLZ
        public static bool IsTwitterBot(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("Twitterbot");
        }

        // http://goo.gl/rK1lR
        public static bool IsIos(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iPod");
        }
    }
}
