using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Logistics.Models;

namespace Logistics.Controllers
{ 
    public class UserController : Controller
    {
        private LogisticsDBContent db = new LogisticsDBContent();

        //
        // GET: /User/

        public ActionResult Index()
        {          
            // Admin is who can access
            if (Request.IsAuthenticated)
            {
                // Giá trị cho Layout
                ViewBag.Post_News = db.News.OrderBy(n => n.NewsID).Skip(0).Take(4).ToList();
                return View(db.Users.ToList());
            }
            else
            {
                return RedirectToAction("LogOn", "User", null);
            }
        }

        //
        // GET: /User/Details/5

        public ViewResult Details(string id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = Logistics.Models.User.EncryptorMD5Hash(user.Password);
                user.ConfirmPassword = Logistics.Models.User.EncryptorMD5Hash(user.ConfirmPassword);
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(user);
        }
        
        //
        // GET: /User/Edit/5
 
        public ActionResult Edit(string id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5
 
        public ActionResult Delete(string id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {            
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }


        /// <summary>
        /// Log On to system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (CheckRole(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Admin", "User");
                    }
                }
                else
                {
                    return RedirectToAction("LogOn", "User");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        /// <summary>
        /// Method check user when login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool CheckRole(string username, string password)
        {
            try
            {
                var users = db.Users.ToList();
                foreach (var item in users)
                {
                    if (username.Trim() == item.UserID && Logistics.Models.User.EncryptorMD5Hash(password) == item.Password)
                    {
                        // Tồn tại
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            
            return false;
        }


        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Admin()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}