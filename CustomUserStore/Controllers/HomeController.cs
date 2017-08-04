using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CustomUserStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var tenant = prinicpal.Claims.Where(c => c.Type == "tenant").Select(c => c.Value).SingleOrDefault();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}