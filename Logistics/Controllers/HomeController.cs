using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Data;
using System.Xml;
using HtmlAgilityPack;
using System.Globalization;
using System.Resources;
using System.ComponentModel.DataAnnotations;
namespace Logistics.Controllers
{
    public class HomeController : Controller
    {
        Logistics.Models.LogisticsDBContent db = new Logistics.Models.LogisticsDBContent();
        
        public ActionResult Index()
        {
            string url = "http://worldmaritimenews.com/archives/category/uncategorized/news-category/top_stories/";
            GetItemNews(url);
            // Lấy 4 News tiếp theo   
            if (db.News.Count() > 0)
            {
                var news = db.News.OrderByDescending(n => n.DateTime).Skip(4).Take(4).ToList();
                return View(news);
            }            
            
            return View(db.News.ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult About2()
        {
            return View();
        }
        public ActionResult Contacts()
        {            
            return View();
        }


        /// <summary>
        /// Hàm add a contact from user
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Contact(string inputName, string inputEmail, string inputSubject, string inputMessage)
        {
            if (inputName != "" && inputEmail != "" && inputSubject != "" && inputMessage != "")
            {
                Logistics.Models.Contact contact = new Models.Contact();
                // Assign attitude
                contact.Name = inputName;
                contact.Email = inputEmail;
                contact.Subject = inputSubject;
                contact.Message = inputMessage;
                // Save
                db.Contacts.Add(contact);
                db.SaveChanges();
                return View("Success");// OK
            }
            return View("Contacts");// Back
        }
        
        /// <summary>
        /// Hàm send email
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult SendMail(string inputName, string inputEmail, string inputSubject, string inputMessage)
        {
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;

            string emailFrom = "viet.nguyen.nii@gmail.com";
            string password = "khongcanmatkhau";
            string emailTo = inputEmail;
            string subject = inputSubject;
            string body = inputMessage;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                // Can set to false, if you are sending pure text.

                //mail.Attachments.Add(new Attachment("C:\\SomeFile.txt"));
                //mail.Attachments.Add(new Attachment("C:\\SomeZip.zip"));

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
            return View();
        }

        /// <summary>
        /// Get all news from url
        /// </summary>
        /// <param name="url"></param>
        public void GetItemNews(string url)
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
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='post']//h2//a"))
                        {
                            titles.Add(HtmlEntity.DeEntitize(t.InnerText));
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
                        foreach (HtmlNode l in doc.DocumentNode.SelectNodes("//div[@class='post']//h2//a[@href]"))
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
                        foreach (HtmlNode d in doc.DocumentNode.SelectNodes("//div[@class='post']//p"))
                        {
                            descriptions.Add(HtmlEntity.DeEntitize(d.InnerText));
                        }
                    }
                    catch (Exception ex)
                    {
                        //Ex
                    }
                    
                    //
                    // =============================  Date time
                    //                    
                    try
                    {
                        // Datetime of Hot News
                        foreach (HtmlNode d in doc.DocumentNode.SelectNodes("//div[@class='post']//div[@class='postinfo']"))
                        {
                            dates.Add(d.InnerText);
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
            if(titles.Count == links.Count && descriptions.Count == contents.Count && dates.Count == sources.Count 
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
                        foreach (HtmlNode t in doc.DocumentNode.SelectNodes("//div[@class='post']//h2//a"))
                        {
                            content = HtmlEntity.DeEntitize(t.InnerText);
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
