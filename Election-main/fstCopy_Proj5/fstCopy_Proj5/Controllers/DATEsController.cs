using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using fstCopy_Proj5.Models;

namespace fstCopy_Proj5.Controllers
{
    public class DATEsController : Controller
    {
        private ElectionEntities db = new ElectionEntities();


        // GET: Dates
        public ActionResult Index()
        {
            return View(db.DATES.ToList());
        }

        public ActionResult IndexAdmin()
        {
            return View(db.DATES.ToList());
        }

        // GET: Dates/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,startDateNominate,EndDateNominate,startDateOfElection,EndDateOfElection")] DATE dateModel)
        {
            if (ModelState.IsValid)
            {
                db.DATES.Add(dateModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dateModel);
        }

        // GET: Dates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATE dateModel = db.DATES.Find(id);
            if (dateModel == null)
            {
                return HttpNotFound();
            }
            return View(dateModel);
        }

        // POST: Dates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,startDateOfElection,EndDateOfElection")] DATE dateModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dateModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dateModel);
        }

        // GET: Dates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DATE dateModel = db.DATES.Find(id);
            if (dateModel == null)
            {
                return HttpNotFound();
            }
            return View(dateModel);
        }

        // POST: Dates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DATE dateModel = db.DATES.Find(id);
            db.DATES.Remove(dateModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
