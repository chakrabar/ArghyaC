using ArghyaC.Utilities;
using System;
using System.Web.Mvc;

namespace ArghyaC.Controllers
{
    public class SimpleProxyController : Controller
    {
        public ActionResult Index(string uri, string accept)
        {
            Uri baseUri = Request.Url;
            return ClientUtilities.GetWebResponse(uri, accept, baseUri);
        }  
    }
}