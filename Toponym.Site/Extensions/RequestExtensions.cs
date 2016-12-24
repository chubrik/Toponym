using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Toponym.Site.Extensions {
    public static class RequestExtensions {

        // https://developers.facebook.com/docs/sharing/webmasters/crawler
        public static bool IsFacebookBot(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("facebookexternalhit") || userAgent.Contains("Facebot");
        }

        // https://vk.com/dev/video_emb
        public static bool IsVkBot(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("vkShare");
        }

        // https://dev.twitter.com/cards/getting-started#crawling
        public static bool IsTwitterBot(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("Twitterbot");
        }

        // http://stackoverflow.com/questions/9038625/detect-if-device-is-ios
        public static bool IsIos(this HttpRequest request) {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iPod");
        }
    }
}
