﻿using System.Web.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC BrowserID demo!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
