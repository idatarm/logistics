using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logistics.Controllers
{
    public class ServiceController : Controller
    {
        //
        // GET: /Service/

        public ActionResult Overview()
        {
            return View();
        }

        public ActionResult CustomsBrokerage()
        {
            return View();
        }

        public ActionResult LogisticsManagement()
        {
            return View();
        }

        public ActionResult FreightTransportation()
        {
            return View();
        }

        public ActionResult AgencyBusiness()
        {
            return View();
        }
    }
}
