using MegaFilesSharing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using PagedList; 

namespace MegaFilesSharing.Controllers
{
    public class HomeController : Controller
    {
        private MegaFilesSharingEntities db = new MegaFilesSharingEntities();
        public ActionResult Index(string searchString, string type, string porn, int? page)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                File v = null;
                return View(v);
            }

            string str1 = " " + searchString + " ";
            string str2 = " " + searchString  ;
            string str3 =  searchString + " ";

            //If user wants porn 
            bool IsPorn = false;
            if (!string.IsNullOrWhiteSpace(porn))
                IsPorn = true;

            int currentPageIndex = page.HasValue ? page.Value : 1;

            IEnumerable<File> files;

            if (type == "All")
            {

                  files = from f in db.Files
                          where (f.Name.Contains(str1) ||
                                   f.Name.StartsWith(str3) ||
                                   f.Name.EndsWith(str2)) &&
                                   f.Porn == IsPorn
                            orderby f.PublishDate descending
                            select f;
            } 
            else
            {
                  files = from f in db.Files
                          where (f.Name.Contains(str1) ||
                                 f.Name.StartsWith(str3) ||
                                 f.Name.EndsWith(str2) )&&
                                  f.FileType == type &&
                                   f.Porn == IsPorn
                            orderby f.PublishDate descending
                            select f;
            }
            //List<File> fileList = files.ToList();

            PagedList<File> result = new PagedList<File>(files.ToList(), currentPageIndex, 15);

            ViewBag.numOfResults = files.ToList().Count;
            ViewBag.searchString = searchString;
            ViewBag.type = type;
            ViewBag.page = currentPageIndex;
            ViewBag.porn = porn;


            if (Request.IsAjaxRequest())
                return PartialView("_Search", result);
            return View(result);
        }

        //Paging method
        public IEnumerable<File> GetFilePage(int pageNumber, string sort, bool Dir)
        {
            int pageSize = 10;

            if (pageNumber < 1)
                pageNumber = 1;

            else if (sort == "name")
                return db.Files.OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            else if (sort == "type")
                return db.Files.OrderBy(x => x.FileType)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            else  
                return db.Files.OrderBy(x => x.PublishDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return null;
        }


        
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}
