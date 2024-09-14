using fstCopy_Proj5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fstCopy_Proj5.Controllers
{
    public class HomeController : Controller
    {
        public ElectionEntities db = new ElectionEntities();    
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult CotactUs()
        {
            return View();
        }
        public ActionResult Faq()
        {
            return View();
        }public ActionResult Map()
        {
            return View();
        }
        public ActionResult button1()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Search()
        {
            var user = new fstCopy_Proj5.Models.User();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(User user)
        {
            if (ModelState.IsValid)
            {
                var Voter = db.Users
                                  .FirstOrDefault(c => c.NationalNumber == user.NationalNumber);

                if (Voter != null)
                {
                    user.FullName = Voter.FullName;

                    user.NationalNumber = Voter.NationalNumber;
                    user.Governorate = Voter.Governorate;
                    user.ElectionArea = Voter.ElectionArea;
                    user.NameOftheStation = Voter.NameOftheStation;
                    user.FundNumber = Voter.FundNumber;
                    user.PlaceOfResidence = Voter.PlaceOfResidence;
                }
                else
                {
                    ModelState.AddModelError("", "لا يوجد مرشح بالرقم الوطني المدخل.");
                }
            }

            return View(user);
        }

    }

}