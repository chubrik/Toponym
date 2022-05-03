namespace Toponym.Web
{
    public static class RequestExtensions
    {
        // https://developers.facebook.com/docs/sharing/webmasters/crawler
        public static bool IsFacebookBot(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("facebookexternalhit") || userAgent.Contains("Facebot");
        }

        // https://vk.com/dev/video_emb
        public static bool IsVkBot(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("vkShare");
        }

        // https://dev.twitter.com/cards/getting-started#crawling
        public static bool IsTwitterBot(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("Twitterbot");
        }

        // https://stackoverflow.com/questions/9038625/detect-if-device-is-ios
        public static bool IsIos(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].Single();
            return userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iPod");
        }

        // https://stackoverflow.com/questions/9847580/how-to-detect-safari-chrome-ie-firefox-and-opera-browser/#26358856
        public static string BrowserName(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"].Single();

            if (userAgent.Contains("Opera") || userAgent.Contains("OPR"))
                return "opera";

            if (userAgent.Contains("Chrome"))
                return "chrome";

            if (userAgent.Contains("Firefox"))
                return "firefox";

            if (userAgent.Contains("Safari"))
                return "safari";

            if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
                return "msie";

            return "unknown";
        }
    }
}
