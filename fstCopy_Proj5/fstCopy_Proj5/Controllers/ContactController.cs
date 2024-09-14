using fstCopy_Proj5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace fstCopy_Proj5.Controllers
{
    public class ContactController : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        // GET: Admin

        // GET: contact_us
        public ActionResult Index(bool? isRead)
        {
            var contacts = db.Contacts.AsQueryable();

            if (isRead.HasValue)
            {
                if (isRead.Value)
                {
                    contacts = contacts.Where(c => (bool)c.IsRead);
                }
                else
                {
                    contacts = contacts.Where(c => !(bool)c.IsRead);
                }
            }

            return View(contacts.ToList());
        }

        // GET: contact_us/Details/5
        public ActionResult DetailsContact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            // Set IsRead to true
            contact.IsRead = true;
            db.Entry(contact).State = EntityState.Modified;
            db.SaveChanges();
            return View(contact);

        }


        // GET: Admin/Create
        public ActionResult CreateContact()
        {
            return View();
        }




        // POST: contact_us/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                db.SaveChanges();
                TempData["SuccessMessage"] = "شكرًا لك على تواصلك معنا. رأيك وملاحظاتك تهمنا ونقدر دعمك. سنقوم بمراجعة رسالتك والرد عليك في أقرب وقت ممكن.";
                return RedirectToAction("CreateContact");
            }

            return View(contact);
        }



    }



}
