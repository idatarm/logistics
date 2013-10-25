using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Logistics.Models;
using System.Xml;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace Logistics.Controllers
{
    public class NewsController : Controller
    {
        private LogisticsDBContent db = new LogisticsDBContent();

        //
        // GET: /News/ ===========> Role : ADMIN

        public ActionResult Index(int startId = 0)
        {
            // Admin is who can access
            if (Request.IsAuthenticated)
            {
                // Lưu giá trị lại để dùng bên .cshtml
                ViewBag.StartId = startId;
                ViewBag.SumNews = db.News.Count();

                var news = db.News.OrderByDescending(n => n.DateTime).Skip(startId).Take(8).ToList();
                // Phân trang mỗi lần lấy 4 news.
                return View(news);
            }
            else
            {
                return RedirectToAction("LogOn","User",null);
            }
        }

        //
        // GET: /News/

        public ViewResult Manage()
        {
            return View(db.News.OrderBy(n => n.DateTime).Skip(0).Take(8).ToList());
        }

        //
        // GET: /News/Details/5

        public ViewResult Details(int id)
        {
            News news = db.News.Find(id);
            return View(news);
        }

        //
        // GET: /News/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /News/Create

        [HttpPost]
        public ActionResult Create(News news)
        {
            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        //
        // GET: /News/Edit/5

        public ActionResult Edit(int id)
        {
            News news = db.News.Find(id);
            return View(news);
        }

        //
        // POST: /News/Edit/5

        [HttpPost]
        public ActionResult Edit(News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        //
        // GET: /News/Delete/5

        public ActionResult Delete(int id)
        {
            News news = db.News.Find(id);
            return View(news);
        }

        //
        // POST: /News/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Lấy tin tức từ website khác
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNewsFromUrl()
        {
            string url = "http://www.moit.gov.vn/vn/Pages/chuyenmuctin.aspx?ChuyenmucID=22";
            List<List<string>> newsList = GetNews(url);
            int counts = newsList.Count;
            News news;
            for (int i = 0; i < counts; i++)
            {
                // Add attitude
                news = new News();
                news.Title = newsList[i][0];
                news.Link = newsList[i][1];
                news.Description = newsList[i][2];
                news.Content = newsList[i][3];
                news.DateTime = newsList[i][4];
                news.Source = newsList[i][5];

                // Save
                db.News.Add(news);
                db.SaveChanges();
                
            }
            
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Get all news from url
        /// </summary>
        /// <param name="url"></param>
        public List<List<string>> GetNews(string url)
        {
            Stream stream;
            StreamReader reader;
            String response = null;
            WebClient webClient = new WebClient();

            List<string> titles = new List<string>();          //1
            List<string> links = new List<string>();           //2
            List<string> descriptions = new List<string>();    //3
            List<string> contents = new List<string>();         //4
            List<string> dates = new List<string>();            //5
            List<string> sources = new List<string>();          //6

            List<List<string>> NEWS = new List<List<string>>();

            using (webClient)
            {
                try
                {
                    // Open and read from the supplied URI
                    stream = webClient.OpenRead(url);
                    reader = new StreamReader(stream);
                    response = reader.ReadToEnd();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response);

                    // 
                    // ======================  Title
                    //                    
                    try
                    {
                        // Title of hot News
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='txt_vip']//a"))
                        {
                            titles.Add(t.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }
                    try
                    {
                        // Title of others News
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='list_ti']/a"))
                        {
                            titles.Add(t.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }

                    //
                    // ============================  Link and Contents
                    //                    
                    string format_url = "";
                    try
                    {
                        // Link of Hot News                        
                        foreach (HtmlNode l in doc.DocumentNode.SelectNodes("//div[@class='txt_vip']//a"))
                        {
                            // Add links
                            HtmlAttribute att = l.Attributes["href"];
                            format_url = "http://www.moit.gov.vn/vn/Pages/" + att.Value;
                            links.Add(format_url);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }
                    try
                    {
                        // Others link
                        foreach (HtmlNode l in doc.DocumentNode.SelectNodes("//div[@class='list_ti']/a[@href]"))
                        {
                            // Add links
                            HtmlAttribute att = l.Attributes["href"];
                            format_url = "http://www.moit.gov.vn/vn/Pages/" + att.Value;
                            links.Add(format_url);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }

                    //
                    // =============================  Description
                    //
                    try
                    {
                        // Description of Hot News
                        foreach (HtmlNode d in doc.DocumentNode.SelectNodes("//p[@class='sapo']"))
                        {
                            descriptions.Add(d.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }
                    try
                    {
                        // Description of others News
                        foreach (HtmlNode d in doc.DocumentNode.SelectNodes("//div[@class='list_sp']"))
                        {
                            descriptions.Add(d.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }

                    //
                    // =============================  Contents
                    //    
                    try
                    {
                        // Duyệt qua danh sách các link
                        foreach (var link in links)
                        {

                        }
                    }
                    catch (WebException ex)
                    {

                    }


                    //
                    // =============================  Date time
                    //                    
                    try
                    {
                        string t = "";
                        // Datetime of Hot News
                        foreach (HtmlNode d in doc.DocumentNode.SelectNodes("//div[@class='txt_vip']//span"))
                        {
                            dates.Add(d.InnerText.Replace('(',' ').Replace(')',' ').Trim());
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }
                    try
                    {
                        // Datetime of other news
                        foreach (HtmlNode d in doc.DocumentNode.SelectNodes("//div[@class='list_ti']//span"))
                        {
                            dates.Add(d.InnerText.Replace('(', ' ').Replace(')', ' ').Trim());
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }

                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse)
                    {
                        // Add you own error handling as required
                        switch (((HttpWebResponse)ex.Response).StatusCode)
                        {
                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.Unauthorized:
                                response = null;
                                break;

                            default:
                                throw ex;
                        }
                    }
                }
            }// end using

            // Duyệt qua các link để lấy nội dung và nguồn của News
            foreach (var link in links)
            {
                contents.Add(GetContentOfNews(link)[0]);
                sources.Add(GetContentOfNews(link)[1]);
            }

            List<string> tempList;
            if (titles.Count == links.Count && descriptions.Count == contents.Count && dates.Count == sources.Count
                && titles.Count == descriptions.Count && contents.Count == dates.Count)
            {
                for (int i = 0; i < titles.Count; i++)
                {
                    tempList = new List<string>();

                    // Add a news
                    tempList.Add(titles[i]);
                    tempList.Add(links[i]);
                    tempList.Add(descriptions[i]);
                    tempList.Add(contents[i]);
                    tempList.Add(dates[i]);
                    tempList.Add(sources[i]);

                    NEWS.Add(tempList);
                }
            }

            return NEWS;
        }


        /// <summary>
        /// Get content and source of a News
        /// </summary>
        /// <param name="url"></param>
        /// <returns> List include 2 item: List[0] is content and List[1] is source </returns>
        public List<string> GetContentOfNews(string url)
        {
            string content = "";
            string source = "";
            List<string> content_source_news = new List<string>();

            Stream stream2;
            StreamReader reader2;
            String response2 = null;
            WebClient webClient2 = new WebClient();
            using (webClient2)
            {

                try
                {
                    // Open and read from the supplied URI
                    stream2 = webClient2.OpenRead(url);
                    reader2 = new StreamReader(stream2);
                    response2 = reader2.ReadToEnd();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(response2);

                    // Get content of news
                    try
                    {
                        // Description of news
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='sapo_tin']//p"))
                        {
                            content = HtmlEntity.DeEntitize(t.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }

                    try
                    {
                        // Content of others news
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='list-chitiet']"))
                        {
                            content = content + HtmlEntity.DeEntitize(t.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }


                    // Get Source of News
                    try
                    {
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='people']"))
                        {
                            source = HtmlEntity.DeEntitize(t.InnerText);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }
                }
                catch (WebException ex)
                {

                }
                // Result to return
                content_source_news.Add(content); //[0]
                content_source_news.Add(source);  //[1]
            }

            return content_source_news;
        }
    }
}