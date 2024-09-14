using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using fstCopy_Proj5.Models;

namespace fstCopy_Proj5.Controllers
{
    public class GeneralListingsController : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        // GET: GeneralListings
        public ActionResult Index()
        {
            return View(db.GeneralListings.ToList());
        }

        // GET: GeneralListings/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralListing generalListing = db.GeneralListings.Find(id);
            if (generalListing == null)
            {
                return HttpNotFound();
            }
            return View(generalListing);
        }

        // GET: GeneralListings/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult ThankYou()
        {
            return View();
        }

        // POST: GeneralListings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GeneralListingID,Name,Delegate_Name,Delegate_Phone,Delegate_Email,NumberOfVotes,Status")] GeneralListing generalListing)
        {
            // التحقق من أن القائمة لم يتم إنشاؤها مسبقًا بنفس الاسم
            bool isDuplicate = db.GeneralListings.Any(g => g.Name == generalListing.Name);

            if (isDuplicate)
            {
                ModelState.AddModelError("Name", "هذه القائمة تم إنشاؤها مسبقًا بنفس الاسم.");
                return View(generalListing);
            }

            if (ModelState.IsValid)
            {
                db.GeneralListings.Add(generalListing);
                Session["List_Name"] = generalListing.Name;
                db.SaveChanges();
                return RedirectToAction("GeneralListCandidates");
            }

            return View(generalListing);
        }



        // GET: GeneralListings/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralListing generalListing = db.GeneralListings.Find(id);
            if (generalListing == null)
            {
                return HttpNotFound();
            }
            return View(generalListing);
        }

        // POST: GeneralListings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GeneralListingID,Name,Delegate_Name,Delegate_Phone,Delegate_Email,NumberOfVotes,Status")] GeneralListing generalListing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(generalListing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(generalListing);
        }

        // GET: GeneralListings/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GeneralListing generalListing = db.GeneralListings.Find(id);
            if (generalListing == null)
            {
                return HttpNotFound();
            }
            return View(generalListing);
        }

        // POST: GeneralListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            GeneralListing generalListing = db.GeneralListings.Find(id);
            db.GeneralListings.Remove(generalListing);
            db.SaveChanges();
            return RedirectToAction("Index");
        }






        // GET: GeneralListCandidates/Create
        public ActionResult GeneralListCandidates()
        {

            // التحقق مما إذا كانت القائمة قد تم إنشاؤها
            if (Session["List_Name"] == null)
            {
                // إعادة توجيه المستخدم إلى صفحة إنشاء القائمة
                return RedirectToAction("Create");
            }

            var candidateList = Session["CandidateList"] as List<GeneralListCandidate> ?? new List<GeneralListCandidate>();
            ViewBag.CandidateList = candidateList; // عرض القائمة في الـ View
            return View(new GeneralListCandidate());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeneralListCandidates(GeneralListCandidate generalListCandidate)
        {
            if (ModelState.IsValid)
            {
                // استرجاع القائمة من الجلسة أو إنشاء قائمة جديدة إذا لم تكن موجودة
                var candidateList = Session["CandidateList"] as List<GeneralListCandidate> ?? new List<GeneralListCandidate>();

                // التحقق من أن المرشح غير موجود مسبقًا
                bool isDuplicate = candidateList.Any(c => c.NationalNumber == generalListCandidate.NationalNumber);

                if (isDuplicate)
                {
                    ModelState.AddModelError("NationalNumber", "المرشح بالرقم الوطني المدخل موجود بالفعل في القائمة.");
                    ViewBag.CandidateList = candidateList;
                    return View(generalListCandidate);
                }

                // تعيين قيمة GeneralListingName من Session
                generalListCandidate.GeneralListingName = Session["List_Name"]?.ToString();

                // إضافة المرشح إلى القائمة
                candidateList.Add(generalListCandidate);

                // تحديث القائمة في الجلسة
                Session["CandidateList"] = candidateList;

                // مسح الحقول في الـ View بعد الإضافة
                ModelState.Clear();
                return RedirectToAction("GeneralListCandidates");
            }

            ViewBag.CandidateList = Session["CandidateList"] as List<GeneralListCandidate>;
            return View(generalListCandidate);
        }



        /*
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult SubmitAllCandidates()
                {
                    var candidateList = Session["CandidateList"] as List<GeneralListCandidate>;

                    if (candidateList != null && candidateList.Count > 0)
                    {
                        foreach (var candidate in candidateList)
                        {
                            db.GeneralListCandidates.Add(candidate);
                        }

                        db.SaveChanges();

                        // مسح القائمة من الجلسة بعد الحفظ
                        Session["CandidateList"] = null;
                    }

                    return RedirectToAction("Index");
                }*/

        /* // تعديل مرشح من القائمة المؤقتة
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult EditCandidate(int index)
         {
             var candidateList = Session["CandidateList"] as List<GeneralListCandidate>;

             if (candidateList != null && index >= 0 && index < candidateList.Count)
             {
                 var candidateToEdit = candidateList[index];
                 ViewBag.CandidateList = candidateList; // تأكيد عرض القائمة في الـ View
                 return View("GeneralListCandidates", candidateToEdit); // عرض بيانات المرشح في النموذج للتعديل
             }

             return RedirectToAction("GeneralListCandidates");
         }

         // حفظ التعديلات على المرشح
         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult SaveCandidateEdits(GeneralListCandidate updatedCandidate, int index)
         {
             var candidateList = Session["CandidateList"] as List<GeneralListCandidate>;

             if (candidateList != null && index >= 0 && index < candidateList.Count)
             {
                 candidateList[index] = updatedCandidate; // تحديث المرشح في القائمة
                 Session["CandidateList"] = candidateList; // تحديث الجلسة
             }

             return RedirectToAction("GeneralListCandidates");
         }
 */
        // حذف مرشح من القائمة المؤقتة (دون التأثير على قاعدة البيانات)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCandidate(int index)
        {
            var candidateList = Session["CandidateList"] as List<GeneralListCandidate>;

            if (candidateList != null && index >= 0 && index < candidateList.Count)
            {
                candidateList.RemoveAt(index); // حذف المرشح من القائمة
                Session["CandidateList"] = candidateList; // تحديث الجلسة
            }

            return RedirectToAction("GeneralListCandidates");
        }

        // حفظ جميع المرشحين في قاعدة البيانات
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAllCandidates()
        {
            var candidateList = Session["CandidateList"] as List<GeneralListCandidate>;

            if (candidateList != null && candidateList.Count > 0)
            {
                try
                {
                    foreach (var candidate in candidateList)
                    {
                        db.GeneralListCandidates.Add(candidate); // هنا فقط يتم حفظ المرشحين في قاعدة البيانات
                    }

                    db.SaveChanges();

                    // مسح القائمة من الجلسة بعد الحفظ
                    Session["CandidateList"] = null;

                    // إعادة توجيه إلى صفحة النجاح أو الصفحة المطلوبة بعد الحفظ
                    return RedirectToAction("ThankYou");
                }
                catch (DbUpdateException ex)
                {
                    // يمكن تخصيص رسالة الخطأ بناءً على نوع الاستثناء
                    ModelState.AddModelError("", "حدث خطأ أثناء حفظ المرشحين. قد تكون هناك مشكلة في البيانات المدخلة، مثل رقم وطني مكرر.");
                }
                catch (Exception ex)
                {
                    // التعامل مع استثناءات أخرى
                    ModelState.AddModelError("", "حدث خطأ غير متوقع. الرجاء المحاولة مرة أخرى لاحقًا.");
                }
            }

            // إعادة عرض الصفحة مع رسائل الأخطاء
            ViewBag.CandidateList = candidateList;
            return View("GeneralListCandidates");
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