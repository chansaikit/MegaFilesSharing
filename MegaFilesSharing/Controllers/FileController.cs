using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MegaFilesSharing.Models;
using MegaFilesSharing.Filters;
using WebMatrix.WebData;
using System.Globalization;
using System.Net;
using StreamReader = System.IO.StreamReader;
using HtmlAgilityPack;
using System.Data.Entity.Validation;
using PagedList;
using System.Collections;


namespace MegaFilesSharing.Controllers
{
    public class FileController : Controller
    {
        private MegaFilesSharingEntities db = new MegaFilesSharingEntities();

        //
        // GET: /File/

        [InitializeSimpleMembership]
        [Authorize]
        public ActionResult Index(int? page)
        {

            //var files = from f in db.Files
            //           select f;

            int currentPageIndex = page.HasValue ? page.Value : 1;

            PagedList<File> list = new PagedList<File>(
                db.Files.Where(i => i.UserID == WebSecurity.CurrentUserId
                ).ToList()
                , currentPageIndex, 10);

            @ViewBag.page = currentPageIndex;

            if (Request.IsAjaxRequest())
                return PartialView("_listoffiles", list);

            return View(list);
        }


        //
        // GET: /File/Details/5
        [InitializeSimpleMembership]
        public ActionResult Details(string title, int id = 0)
        {
            if (id == 0)
            {
                return HttpNotFound();
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            //increment of view
            file.NumOfView++;

            db.Entry(file).State = EntityState.Modified;
            db.SaveChanges();

            ViewBag.like = db.Likes.Count(x => x.FileID == id);

            return View(file);
        }

        //
        //POST: /FILE/Details
        [HttpPost]
        [InitializeSimpleMembership]
        public ActionResult Details(string act, int id, string body)
        {
            if (act == "like")
            {
                int numOfLike = Convert.ToInt32(body);
                //check double like
                var check = db.Likes.Where(x => x.FileID == id
                    && x.UserID == WebSecurity.CurrentUserId);
                if (check.Count() > 0)
                {
                    return Content("" + numOfLike);
                }

                //saving new like
                Like like = new Like();
                like.UserID = WebSecurity.CurrentUserId;
                like.FileID = id;
                db.Likes.Add(like);
                db.SaveChanges();

                //update like
                numOfLike++;
                ViewBag.like = numOfLike;

                return Content("" + numOfLike);
            }
            else if (act == "comment")
            {
                //comment action
                if (string.IsNullOrWhiteSpace(body))
                    return Content("");

                //Create comment 
                Comment cmt = new Comment();
                cmt.FileID = id;
                cmt.CommentContent = body;
                cmt.Date = DateTime.Now;
                cmt.IP = Request.ServerVariables["REMOTE_ADDR"];
                if (User.Identity.IsAuthenticated)
                {
                    cmt.UserID = WebSecurity.CurrentUserId;
                    cmt.Poster = User.Identity.Name;
                }
                else
                    cmt.Poster = "anonymous";
                //saving comment
                db.Comments.Add(cmt);
                db.SaveChanges();

                //getting comments
                var cmts = db.Comments.Where(x => x.FileID == id).OrderByDescending(
                    x => x.CommentID).ToList();

                return PartialView("_comments", cmts);
            }


            return Content("");
        }



        //
        // GET: /File/Create

        public ActionResult Create()
        {
            File file = new File();

            if (User.Identity.IsAuthenticated)
                file.Publisher = User.Identity.Name;
            else
                file.Publisher = "anonymous";

            file.PublishDate = DateTime.Now;
            return View(file);
        }


        //
        // POST: /File/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [InitializeSimpleMembership]
        public ActionResult Create(File file, string type, string linkString, string porn)
        {

            //Check Is porn
            bool isPorn = false;
            if (!string.IsNullOrWhiteSpace(porn))
                isPorn = true;


            if (type == "auto" && !String.IsNullOrWhiteSpace(linkString))
            {
                //counter for lines
                int counter = 0;
                ViewBag.text = linkString;

                file = new File();
                //remove empty space in link
                linkString = linkString.Replace(" ", "");

                //convert links to array
                String[] links = linkString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                //loop for links
                foreach (string link in links)
                {
                    counter++;
                    //check is valid url 
                    if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                    {
                        //check duplicate record in database
                        var linkitem = db.Files.Where(
                        p => p.Url.Equals(link)
                        );
                        if (linkitem.Count() > 0)
                        {
                            ViewBag.message = "Link already exists at Line " + counter;
                            return PartialView("_Auto");
                        }

                        Uri uri = new Uri(link);

                        var title = "";
                        //Youtube link
                        if (uri.Host == "www.youtube.com")
                        {
                            title = getYoutube(link);
                            file.FileType = "Video";
                            file.Source = "youtube";
                        }
                        else if (uri.Host == "mega.co.nz")
                        {
                            MegaCoNz mega = new MegaCoNz();
                            file = mega.getFile(link);
                            title = file.Name; 
                            file.Source = "mega.co.nz";
                        }
                        else
                        {
                            ViewBag.message = "Not Valid link at Line " + counter;
                            return PartialView("_Auto");
                        }
                        //if title not found
                        if (title == "")
                        {
                            ViewBag.message = "Something wrong at Line " + counter;
                            return PartialView("_Auto");
                        }
                        else
                        {
                            try
                            {

                                //setting attributes  
                                file.Name = title;
                                file.PublishDate = DateTime.Now;
                                if (User.Identity.IsAuthenticated)
                                {
                                    file.Publisher = User.Identity.Name;
                                    file.UserID = WebSecurity.CurrentUserId;
                                }
                                else
                                    file.Publisher = "anonymous";
                                file.Url = link;
                                file.Porn = isPorn;
                                //save in database
                                db.Files.Add(file);
                                db.SaveChanges();
                            }
                            catch (DbEntityValidationException e)
                            {
                                foreach (var eve in e.EntityValidationErrors)
                                {
                                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                    foreach (var ve in eve.ValidationErrors)
                                    {
                                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                            ve.PropertyName, ve.ErrorMessage);
                                    }
                                }
                                throw;
                            }

                        }

                    }

                    else
                    {
                        ViewBag.message = "Not Valid link at Line " + counter;
                        return PartialView("_Auto");
                    }
                }

                ViewBag.message = "Created Successfully!";
                return PartialView("_Auto");
            }

            var item = db.Files.Where(
                    p => p.Url.Equals(file.Url)
                );
            if (item.Count() > 0)
            {
                ViewBag.warning = "Link already exists";
                return View(file);
            }


            string host = new Uri(file.Url).Host;
            host = host.Replace("www.", "");
            host = host.Replace(".com", "");
            host = host.Replace(".net", "");
            file.Source = host;
            file.NumOfDownload = 0;
            file.NumOfView = 0;
            file.Porn = isPorn;

            file.PublishDate = DateTime.Now;
            if (User.Identity.IsAuthenticated)
                file.UserID = WebSecurity.CurrentUserId;

            //if (ModelState.IsValid)
            //{
            db.Files.Add(file);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
            //}

            //return View(file);
        }


        //
        // GET YOUTUBE file INFO
        public String getYoutube(String url)
        {
            var title = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            string contents = "";
            try
            {
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;
                contents = reader.ReadToEnd();
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.LoadHtml(contents);

                var node = htmlDoc.DocumentNode.SelectNodes("//meta");
                title = node[0].Attributes["content"].Value;

                return title;
            }
            catch (Exception e)
            {
                return title;
            }
        }

        //
        // GET MEGAUPLOAD File INFO

        public void getMegaInfo(String url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            string contents = "";
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;
                contents = reader.ReadToEnd();
            }
            Console.WriteLine(contents);
        }


        //
        // GET: /File/Edit/5
        [Authorize]
        [InitializeSimpleMembership]
        public ActionResult Edit(int? id)
        {
            try
            {
                File file = db.Files.Find(id);
                if (file.UserID == WebSecurity.CurrentUserId)
                {
                    if (file == null)
                    {
                        return HttpNotFound();
                    }
                    return View(file);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /File/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [InitializeSimpleMembership]
        public ActionResult Edit(File file)
        {
            string host = new Uri(file.Url).Host;
            host = host.Replace("www.", "");
            host = host.Replace(".com", "");
            host = host.Replace(".net", "");
            file.Source = host;


            file.UserID = WebSecurity.CurrentUserId;


            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(file);
        }

        //
        // GET: /File/Delete/5

        [Authorize]
        [InitializeSimpleMembership]
        public ActionResult Delete(int? id)
        {
            try
            {
                File file = db.Files.Find(id);

                if (file == null)
                {
                    return HttpNotFound();
                }

                if (file.UserID == WebSecurity.CurrentUserId)
                {
                    return View(file);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                return RedirectToAction("Index", "Home");
            }

        }

        //
        // POST: /File/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize]
        [InitializeSimpleMembership]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                File file = db.Files.Find(id);
                if (file.UserID == WebSecurity.CurrentUserId)
                {
                    db.Files.Remove(file);
                    db.SaveChanges();
                }
                if (Request.IsAjaxRequest())
                    return Content(Boolean.TrueString);
                else
                    return RedirectToAction("Index");
            }
            catch
            {//TODO: Log error				
                return Content(Boolean.FalseString);
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}