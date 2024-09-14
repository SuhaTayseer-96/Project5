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
    public class Debates1Controller : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        // GET: Debates1
        public ActionResult Index()
        {
            return View(db.Debates.ToList());
        }
        // GET: Debates1
        public ActionResult IndexDebatsesHome()
        {
            return View(db.Debates.ToList());
        }
        // GET: Debates1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Debate debate = db.Debates.Find(id);
            if (debate == null)
            {
                return HttpNotFound();
            }
            return View(debate);
        }

        // GET: Debates1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Debates1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Circle_ID,Date_Of_Debate,Topic,First_Candidate,First_Candidate_List,Second_Candidate,Second_Candidate_List,Status,Zoom_link")] Debate debate)
        {
            if (ModelState.IsValid)
            {
                db.Debates.Add(debate);
                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Debates1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Debate debate = db.Debates.Find(id);
            if (debate == null)
            {
                return HttpNotFound();
            }

            // Populate the dropdown options
            ViewBag.StatusOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Accept", Text = "Accept" },
                new SelectListItem { Value = "Reject", Text = "Reject" }
            }, "Value", "Text", debate.Status);

            return View(debate);
        }

        // POST: Debates1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Circle_ID,Date_Of_Debate,Topic,First_Candidate,First_Candidate_List,Second_Candidate,Second_Candidate_List,Status,Zoom_link")] Debate debate)
        {
            if (ModelState.IsValid)
            {
                if (debate.Status == "Accept")
                {
                    var existingDebate = db.Debates.Find(debate.ID);
                    if (existingDebate == null)
                    {
                        return HttpNotFound();
                    }

                    // Update the fields
                    existingDebate.Circle_ID = debate.Circle_ID;
                    existingDebate.Date_Of_Debate = debate.Date_Of_Debate;
                    existingDebate.Topic = debate.Topic;
                    existingDebate.First_Candidate = debate.First_Candidate;
                    existingDebate.First_Candidate_List = debate.First_Candidate_List;
                    existingDebate.Second_Candidate = debate.Second_Candidate;
                    existingDebate.Second_Candidate_List = debate.Second_Candidate_List;
                    existingDebate.Status = debate.Status;
                    existingDebate.Zoom_link = debate.Zoom_link;

                    db.Entry(existingDebate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    // If status is not "Accept", do not save changes
                    ModelState.AddModelError("", "Only records with 'Accept' status can be saved.");
                }
            }

            // Repopulate dropdown options in case of validation failure
            ViewBag.StatusOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Accept", Text = "Accept" },
                new SelectListItem { Value = "Reject", Text = "Reject" }
            }, "Value", "Text", debate.Status);

            return View(debate);
        }

        // GET: Debates1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Debate debate = db.Debates.Find(id);
            if (debate == null)
            {
                return HttpNotFound();
            }
            return View(debate);
        }

        // POST: Debates1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Debate debate = db.Debates.Find(id);
            db.Debates.Remove(debate);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}