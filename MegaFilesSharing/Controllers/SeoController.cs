using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MegaFilesSharing.Controllers
{
    public class SeoController : Controller
    {
        //
        // GET: /Seo/

        public ActionResult Robots()
        {
            var robotsFile = "~/robots.txt";
            //switch (Request.Url.Host.ToLower())
            //{
            //    case "stackoverflow.com":
            //        robotsFile = "~/robots-so.txt";
            //        break;
            //    case "meta.stackoverflow.com":
            //        robotsFile = "~/robots-meta.txt";
            //        break;
            //}
            return File(robotsFile, "text/plain");
        }
    }
}
