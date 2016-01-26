using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;

namespace ArghyaC.Infrastructure.Utilities
{
    public class WebUtilities
    {
        public static string SetAbsoluteUri(string uri, Uri baseUri)
        {
            //if (uri.StartsWith("/"))
            //    uri = string.Format("{0}://{1}{2}", baseUri.Scheme, baseUri.Authority, uri);
            //else if (!uri.ToLower().Contains("http://") && !uri.ToLower().Contains("https://"))
            //    uri = "http://" + uri;
            //return uri;
            var protocolScheme = baseUri.Scheme;
            if (uri.StartsWith("//"))
                return protocolScheme + ":" + uri;
            if (uri.StartsWith("/"))
                return string.Format("{0}://{1}{2}", protocolScheme, baseUri.Authority, uri);
            if (!IsAbsoluteUri(uri)) 
                return protocolScheme + "://" + uri;
            return uri;
        }

        public static string GetWebReaderUri(Uri baseUri)
        {
            return string.Format("{0}://{1}{2}", baseUri.Scheme, baseUri.Authority, baseUri.AbsolutePath);
        }

        public static string GetBaseUriForLinks(string uri, Uri baseUri, bool keepDirectLink = false)
        {
            if (ConfigUtilities.GetAppSettingsBool(Constants.IsSinglePageRendering)) //app setting to show original links
                keepDirectLink = true;
            var uriObj = new Uri(uri);
            var directUri = string.Format("{0}://{1}", uriObj.Scheme, uriObj.Authority);
            if (keepDirectLink)
                return directUri;
            return GetWebReaderUri(baseUri) + "?uri="; //+directUri; //TODO: remove hard coding //9 dec 15
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentHtml"></param>
        /// <param name="divertUri">Uri for WebReader redirection e.g. http://arghya.com/webreader/read?uri=</param>
        /// <param name="directUri">Uri for direct link to original resource e.g. http://www.facebook.com</param>
        /// <returns></returns>
        public static string SetAbsoluteLinks(string contentHtml, string divertUri, string directUri)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(contentHtml);

            var allLinks = htmlDoc.DocumentNode.SelectNodes("//*[@href or @src or @action]");
            if (allLinks == null)
                return contentHtml;

            foreach (HtmlNode link in allLinks)
            {
                HtmlAttribute att = GetAttributeForLink(link);
                if (att != null)
                {
                    string href = att.Value; //TODO: re-evaluate "link"
                    var tagsWithOriginalLink = new[] { "img", "link", "audio", "video" };
                    if (tagsWithOriginalLink.Any(tag => link.OriginalName.ToLower() == tag)) //we are not good at rendering binary. yet.
                    {
                        if (!IsAbsoluteUri(href))
                        {
                            //uri = new Uri(new Uri(baseUri), uri);
                            att.Value = directUri + href; //uri.ToString();
                        }
                        continue;
                    }
                    // ignore javascript on buttons using a tags
                    if (href.StartsWith("javascript", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    //if (!IsAbsoluteUri(href))
                    //{
                    //    //uri = new Uri(new Uri(baseUri), uri);
                    //    att.Value = divertUri + href; //uri.ToString(); //TODO: check fix
                    //}
                    att.Value = IsAbsoluteUri(href) ? divertUri + href : divertUri + directUri + href;
                }
            }
            return htmlDoc.DocumentNode.OuterHtml;
        }

        static bool IsAbsoluteUri(string link)
        {
            Uri uri = new Uri(link, UriKind.RelativeOrAbsolute);
            return link.StartsWith("//") || uri.IsAbsoluteUri;
        }

        //static bool IsAbsoluteUri(string link)
        //{
        //    Uri result;
        //    return link.StartsWith("//") || Uri.TryCreate(link, UriKind.Absolute, out result);
        //}

        static HtmlAttribute GetAttributeForLink(HtmlNode link)
        {
            var linkTypes = new[] { "href", "src", "action" };
            foreach (var type in linkTypes)
            {
                var attribute = link.Attributes[type];
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                    return attribute;
            }
            return null;
        }

        public static string GetWebResponse(string uri, ref string contentType, HttpClient client, Uri baseUri)
        {
            var responseContent = string.Empty;
            var response = client.GetAsync(uri).Result;
            if (response != null)
            {
                contentType = response.Content.Headers.ContentType.MediaType;
                if (!response.IsSuccessStatusCode)
                    responseContent = string.Format("Server responded with [{0}] : {1}", (int)response.StatusCode, response.ReasonPhrase);
                else
                {
                    responseContent = response.Content.ReadAsStringAsync().Result;
                    if (contentType == "text/html")
                    {
                        var webReaderUri = new Uri(WebUtilities.GetWebReaderUri(baseUri));
                        responseContent = WebUtilities.SetAbsoluteLinks(responseContent,
                                                                        WebUtilities.GetBaseUriForLinks(uri, webReaderUri),
                                                                        WebUtilities.GetBaseUriForLinks(uri, webReaderUri, true));
                    }
                }
            }
            else
                responseContent = "Server response was null!";
            return responseContent;
        }
    }
}