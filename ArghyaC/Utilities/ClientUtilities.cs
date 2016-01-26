using ArghyaC.Infrastructure.Utilities;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace ArghyaC.Utilities
{
    public class ClientUtilities
    {
        internal static ContentResult GetWebResponse(string uri, string accept, Uri baseUri)
        {
            var message = "Failed to load data from server. ";
            var contentType = "text/html";

            if (!string.IsNullOrEmpty(uri))
            {
                uri = WebUtilities.SetAbsoluteUri(uri, baseUri);
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        if (!string.IsNullOrEmpty(accept) && accept.Contains("/"))
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                        message = WebUtilities.GetWebResponse(uri, ref contentType, client, baseUri);
                    }
                }
                catch (WebException wex)
                {
                    if (wex.Response != null)
                    {
                        using (var stream = wex.Response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            var msg = System.Text.RegularExpressions.Regex.Unescape(reader.ReadToEnd());
                            message = string.Format("Server encountered probelm! Server response :  {0}", msg);
                        }
                    }
                    else
                        message += string.Format("Server encountered probelm! Server response :  {0}", wex.Message);
                }
                catch (Exception ex)
                {
                    message += string.Format("Server encountered probelm! Server response :  {0}", ex.Message);
                }
            }
            else
                message = "Invalid url";

            return new ContentResult
            {
                Content = message,
                ContentType = contentType
            };
        }
    }
}