using fstCopy_Proj5.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace fstCopy_Proj5.Models
{
    public class LocalListCandidatesController : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        public ActionResult Index()
        {
            try
            {
                var candidates = db.LocalListCandidates
                    .Where(c => c.LocalListingID != null)
                    .ToList();

                return View(candidates);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ أثناء جلب البيانات: " + ex.Message);
                return View(new List<LocalListCandidate>());
            }
        }

        public ActionResult CreateList()
        {
            PopulateDropDowns();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateList(LocalList localList, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageUpload != null && ImageUpload.ContentLength > 0)
                    {
                       
                        string path = Path.Combine(Server.MapPath("~/Images/Lists/"), Path.GetFileName(ImageUpload.FileName));

                      
                        ImageUpload.SaveAs(path);

                    
                        localList.ImageUpload = "/Images/Lists/"+ Path.GetFileName(ImageUpload.FileName);
                    }

                    // إضافة القائمة إلى قاعدة البيانات
                    db.LocalLists.Add(localList);
                    db.SaveChanges();

                    // تخزين معلومات الجلسة
                    Session["LocalListID"] = localList.ID;
                    Session["Governorate"] = localList.Governorate;
                    Session["ElectionArea"] = localList.ElectionArea;

                    return RedirectToAction("Create");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            PopulateDropDowns();
            return View(localList);
        }


        public ActionResult Create()
        {
            ViewBag.LocalListID = Session["LocalListID"];
            ViewBag.Governorate = Session["Governorate"];
            ViewBag.ElectionArea = Session["ElectionArea"];
            PopulateDropDowns();
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "CandidateID,CandidateName,NationalNumber,Gender,Religion,Type_Chair,BirthdateCandidate,Governorate,ElectionArea,NumberOfVotesCandidate,LocalListingID,Email")] LocalListCandidate localListCandidate, HttpPostedFileBase ImageUpload)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // التحقق من الرقم الوطني (10 أرقام)
        //        if (localListCandidate.NationalNumber.Length != 10)
        //        {
        //            ModelState.AddModelError("NationalNumber", "الرقم الوطني يجب أن يكون 10 أرقام.");
        //        }

        //        // التحقق من تاريخ الميلاد (العمر أكبر من 25 سنة)
        //        if (!string.IsNullOrEmpty(localListCandidate.BirthdateCandidate))
        //        {
        //            DateTime birthDate;
        //            if (DateTime.TryParse(localListCandidate.BirthdateCandidate, out birthDate))
        //            {
        //                int age = DateTime.Now.Year - birthDate.Year;
        //                if (birthDate > DateTime.Now.AddYears(-age)) age--;

        //                if (age < 25)
        //                {
        //                    ModelState.AddModelError("BirthdateCandidate", "العمر يجب أن يكون 25 سنة أو أكثر.");
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("BirthdateCandidate", "تنسيق تاريخ غير صحيح.");
        //            }
        //        }

        //        // التحقق من الحقول المطلوبة
        //        if (string.IsNullOrEmpty(localListCandidate.CandidateName))
        //        {
        //            ModelState.AddModelError("CandidateName", "اسم المرشح مطلوب.");
        //        }

        //        // التحقق من نوع الكرسي والترشح لعجلون
        //        if (localListCandidate.Governorate == "عجلون")
        //        {
        //            var currentListCandidates = db.LocalListCandidates
        //                .Where(c => c.LocalListingID == localListCandidate.LocalListingID)
        //                .ToList();

        //            int christianCount = currentListCandidates.Count(c => c.Type_Chair == "مسيحي");
        //            int quotaCount = currentListCandidates.Count(c => c.Type_Chair == "كوتا");
        //            int competitiveCount = currentListCandidates.Count(c => c.Type_Chair == "تنافس");

        //            if (localListCandidate.Type_Chair == "مسيحي" && christianCount >= 1)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح مسيحي واحد.");
        //            }
        //            else if (localListCandidate.Type_Chair == "كوتا" && quotaCount >= 1)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
        //            }
        //            else if (localListCandidate.Type_Chair == "تنافس" && competitiveCount >= 1)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح تنافس واحد.");
        //            }
        //        }
        //        else if (localListCandidate.Governorate == "إربد" && localListCandidate.ElectionArea == "إربد الأولى")
        //        {
        //            var currentListCandidates = db.LocalListCandidates
        //                .Where(c => c.LocalListingID == localListCandidate.LocalListingID)
        //                .ToList();

        //            int quotaCount = currentListCandidates.Count(c => c.Type_Chair == "كوتا");
        //            int competitiveCount = currentListCandidates.Count(c => c.Type_Chair == "تنافس");

        //            if (localListCandidate.Type_Chair == "كوتا" && quotaCount >= 1)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
        //            }
        //            else if (localListCandidate.Type_Chair == "تنافس" && competitiveCount >= 7)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من 7 مرشحين تنافس.");
        //            }
        //        }
        //        else if (localListCandidate.Governorate == "إربد" && localListCandidate.ElectionArea == "إربد الثانية")
        //        {
        //            var currentListCandidates = db.LocalListCandidates
        //                .Where(c => c.LocalListingID == localListCandidate.LocalListingID)
        //                .ToList();

        //            int christianCount = currentListCandidates.Count(c => c.Type_Chair == "مسيحي");
        //            int quotaCount = currentListCandidates.Count(c => c.Type_Chair == "كوتا");
        //            int competitiveCount = currentListCandidates.Count(c => c.Type_Chair == "تنافس");

        //            if (localListCandidate.Type_Chair == "مسيحي" && christianCount >= 1)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح مسيحي واحد.");
        //            }
        //            else if (localListCandidate.Type_Chair == "كوتا" && quotaCount >= 1)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
        //            }
        //            else if (localListCandidate.Type_Chair == "تنافس" && competitiveCount >= 5)
        //            {
        //                ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من 5 مرشحين تنافس.");
        //            }
        //        }

        //        // معالجة تحميل الصورة
        //        if (ImageUpload != null && ImageUpload.ContentLength > 0)
        //        {
        //            string fileName = Path.GetFileName(ImageUpload.FileName);
        //            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        //            string path = Path.Combine(Server.MapPath("~/Images/"), uniqueFileName);

        //            try
        //            {
        //                // Ensure the directory exists
        //                if (!Directory.Exists(Server.MapPath("~/Images/")))
        //                {
        //                    Directory.CreateDirectory(Server.MapPath("~/Images/"));
        //                }

        //                ImageUpload.SaveAs(path);
        //                localListCandidate.ImageUpload = "/Images/Candidates/" + uniqueFileName;
        //            }
        //            catch (Exception ex)
        //            {
        //                ModelState.AddModelError("", "حدث خطأ أثناء رفع الصورة: " + ex.Message);
        //            }
        //        }

        //        // إذا لم يكن هناك أي أخطاء في الفالديشن، احفظ المرشح
        //        if (ModelState.IsValid)
        //        {
        //            db.LocalListCandidates.Add(localListCandidate);
        //            db.SaveChanges();
        //            return RedirectToAction("CreateCandidates", new { listId = localListCandidate.LocalListingID });
        //        }
        //    }

        //    // عرض البيانات المدخلة مع رسائل الخطأ
        //    ViewBag.LocalListID = localListCandidate.LocalListingID;
        //    ViewBag.Governorate = Session["Governorate"];
        //    ViewBag.ElectionArea = Session["ElectionArea"];
        //    ViewBag.Email = Session["Email"];
        //    PopulateDropDowns(localListCandidate);
        //    return View(localListCandidate);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CandidateID,CandidateName,NationalNumber,Gender,Religion,Type_Chair,BirthdateCandidate,Governorate,ElectionArea,NumberOfVotesCandidate,LocalListingID,Email")] LocalListCandidate localListCandidate, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // التحقق من الرقم الوطني (10 أرقام)
                    if (localListCandidate.NationalNumber.Length != 10)
                    {
                        ModelState.AddModelError("NationalNumber", "الرقم الوطني يجب أن يكون 10 أرقام.");
                    }

                    // التحقق من تاريخ الميلاد (العمر أكبر من 25 سنة)
                    if (!string.IsNullOrEmpty(localListCandidate.BirthdateCandidate))
                    {
                        DateTime birthDate;
                        if (DateTime.TryParse(localListCandidate.BirthdateCandidate, out birthDate))
                        {
                            int age = DateTime.Now.Year - birthDate.Year;
                            if (birthDate > DateTime.Now.AddYears(-age)) age--;

                            if (age < 25)
                            {
                                ModelState.AddModelError("BirthdateCandidate", "العمر يجب أن يكون 25 سنة أو أكثر.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("BirthdateCandidate", "تنسيق تاريخ غير صحيح.");
                        }
                    }

                    // التحقق من الحقول المطلوبة
                    if (string.IsNullOrEmpty(localListCandidate.CandidateName))
                    {
                        ModelState.AddModelError("CandidateName", "اسم المرشح مطلوب.");
                    }


                    // التحقق من نوع الكرسي والترشح لعجلون
                    if (localListCandidate.Governorate == "عجلون")
                    {
                        var currentListCandidates = db.LocalListCandidates
                            .Where(c => c.LocalListingID == localListCandidate.LocalListingID)
                            .ToList();

                        int christianCount = currentListCandidates.Count(c => c.Type_Chair == "مسيحي");
                        int quotaCount = currentListCandidates.Count(c => c.Type_Chair == "كوتا");
                        int competitiveCount = currentListCandidates.Count(c => c.Type_Chair == "تنافس");

                        if (localListCandidate.Type_Chair == "مسيحي" && christianCount >= 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح مسيحي واحد.");
                        }
                        else if (localListCandidate.Type_Chair == "كوتا" && quotaCount >= 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
                        }
                        else if (localListCandidate.Type_Chair == "تنافس" && competitiveCount >= 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح تنافس واحد.");
                        }
                    }
                    else if (localListCandidate.Governorate == "إربد" && localListCandidate.ElectionArea == "إربد الأولى")
                    {
                        var currentListCandidates = db.LocalListCandidates
                            .Where(c => c.LocalListingID == localListCandidate.LocalListingID)
                            .ToList();

                        int quotaCount = currentListCandidates.Count(c => c.Type_Chair == "كوتا");
                        int competitiveCount = currentListCandidates.Count(c => c.Type_Chair == "تنافس");

                        if (localListCandidate.Type_Chair == "كوتا" && quotaCount >= 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
                        }
                        else if (localListCandidate.Type_Chair == "تنافس" && competitiveCount >= 7)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من 7 مرشحين تنافس.");
                        }
                    }
                    else if (localListCandidate.Governorate == "إربد" && localListCandidate.ElectionArea == "إربد الثانية")
                    {
                        var currentListCandidates = db.LocalListCandidates
                            .Where(c => c.LocalListingID == localListCandidate.LocalListingID)
                            .ToList();

                        int christianCount = currentListCandidates.Count(c => c.Type_Chair == "مسيحي");
                        int quotaCount = currentListCandidates.Count(c => c.Type_Chair == "كوتا");
                        int competitiveCount = currentListCandidates.Count(c => c.Type_Chair == "تنافس");

                        if (localListCandidate.Type_Chair == "مسيحي" && christianCount >= 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح مسيحي واحد.");
                        }
                        else if (localListCandidate.Type_Chair == "كوتا" && quotaCount >= 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
                        }
                        else if (localListCandidate.Type_Chair == "تنافس" && competitiveCount >= 5)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من 5 مرشحين تنافس.");
                        }
                    }


                    // معالجة تحميل الصورة
                    if (ImageUpload != null && ImageUpload.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(ImageUpload.FileName);
                        var uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                        var path = Path.Combine(Server.MapPath("~/Images/Candidates/"), uniqueFileName);

                        try
                        {
                            // Ensure the directory exists
                            if (!Directory.Exists(Server.MapPath("~/Images/Candidates/")))
                            {
                                Directory.CreateDirectory(Server.MapPath("~/Images/Candidates/"));
                            }

                            ImageUpload.SaveAs(path);

                            // Set the image path in the model
                            localListCandidate.ImageUpload = uniqueFileName;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "حدث خطأ أثناء رفع الصورة: " + ex.Message);
                        }
                    }

                    // إذا لم يكن هناك أي أخطاء في الفالديشن، احفظ المرشح
                    if (ModelState.IsValid)
                    {
                        db.LocalListCandidates.Add(localListCandidate);
                        db.SaveChanges();

                        // Redirect to the appropriate action
                        return RedirectToAction("CreateCandidates", new { listId = localListCandidate.LocalListingID });
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    // Handle validation errors
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // عرض البيانات المدخلة مع رسائل الخطأ
            ViewBag.LocalListID = localListCandidate.LocalListingID;
            ViewBag.Governorate = Session["Governorate"];
            ViewBag.ElectionArea = Session["ElectionArea"];
            ViewBag.Email = Session["Email"];
            PopulateDropDowns(localListCandidate);
            return View(localListCandidate);
        }

        public ActionResult CreateCandidates(int listId)
        {
            Session["LocalListID"] = listId;
            ViewBag.LocalListID = listId;
            var localList = db.LocalLists.Find(listId);
            ViewBag.Governorate = localList.Governorate;
            ViewBag.ElectionArea = localList.ElectionArea;

            int candidateCount = 0;

            if (localList.Governorate == "إربد" && localList.ElectionArea == "إربد الأولى")
            {
                candidateCount = 7;
            }
            else if (localList.Governorate == "إربد" && localList.ElectionArea == "إربد الثانية")
            {
                candidateCount = 6;
            }
            else if (localList.Governorate == "عجلون")
            {
                candidateCount = 3;
            }

            ViewBag.CandidateCount = candidateCount;
            Session["CandidateCount"] = candidateCount;

            // إنشاء قائمة من المرشحين الفارغين بناءً على عدد المرشحين المطلوب
            var candidates = new List<LocalListCandidate>();
            for (int i = 0; i < candidateCount; i++)
            {
                candidates.Add(new LocalListCandidate());
            }

            PopulateDropDowns();
            return View(candidates);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCandidates(List<LocalListCandidate> candidates)
        {
            if (candidates != null && candidates.Any())
            {
                if (Session["LocalListID"] != null)
                {
                    int localListID = (int)Session["LocalListID"];
                    var localList = db.LocalLists.Find(localListID);
                    string governorate = localList.Governorate;
                    string electionArea = localList.ElectionArea;

                    int christianCount = 0;
                    int quotaCount = 0;
                    int competitiveCount = 0;

                    for (int i = 0; i < candidates.Count; i++)
                    {
                        var candidate = candidates[i];
                        candidate.LocalListingID = localListID;

                        if (candidate.Type_Chair == "مسيحي")
                        {
                            christianCount++;
                        }
                        else if (candidate.Type_Chair == "كوتا")
                        {
                            quotaCount++;
                        }
                        else if (candidate.Type_Chair == "تنافس")
                        {
                            competitiveCount++;
                        }

                        if ((governorate == "عجلون" || (governorate == "إربد" && electionArea == "إربد الثانية") || (governorate == "إربد" && electionArea == "إربد الأولى")) && christianCount > 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح مسيحي واحد.");
                        }

                        if ((governorate == "عجلون" || (governorate == "إربد" && electionArea == "إربد الثانية")) && quotaCount > 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
                        }

                        if ((governorate == "إربد" && electionArea == "إربد الأولى") && quotaCount > 1)
                        {
                            ModelState.AddModelError("Type_Chair", "لا يمكن إضافة أكثر من مرشح كوتا واحد.");
                        }

                        var existingCandidate = db.LocalListCandidates
                            .FirstOrDefault(c => c.NationalNumber == candidate.NationalNumber && c.LocalListingID == localListID);

                        if (existingCandidate != null)
                        {
                            ModelState.AddModelError("", "الرقم الوطني موجود بالفعل في القائمة.");
                        }

                        // Handle file upload for each candidate
                        var file = Request.Files[$"ImageUploads[{i}]"];
                        if (file != null && file.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string uniqueFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                            string path = Path.Combine(Server.MapPath("~/Images/Candidates/"), uniqueFileName);

                            try
                            {
                                if (!Directory.Exists(Server.MapPath("~/Images/Candidates/")))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/Images/Candidates/"));
                                }

                                file.SaveAs(path);
                                candidate.ImageUpload = "/Images/Candidates/" + uniqueFileName;
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", "حدث خطأ أثناء رفع الصورة: " + ex.Message);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("ImageUpload", "الرجاء اختيار صورة صالحة للتحميل.");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        ViewBag.Governorate = Session["Governorate"];
                        ViewBag.ElectionArea = Session["ElectionArea"];
                        PopulateDropDowns();
                        return View(candidates);
                    }

                    try
                    {
                        foreach (var candidate in candidates)
                        {
                            db.LocalListCandidates.Add(candidate);
                        }
                        db.SaveChanges();
                        return RedirectToAction("ThankYou");
                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "حدث خطأ: " + ex.InnerException?.Message ?? ex.Message);
                    }

                    ViewBag.Governorate = Session["Governorate"];
                    ViewBag.ElectionArea = Session["ElectionArea"];
                    PopulateDropDowns();
                    return View(candidates);
                }
            }

            ViewBag.Governorate = Session["Governorate"];
            ViewBag.ElectionArea = Session["ElectionArea"];
            PopulateDropDowns();
            return View(candidates);
        }



        private void PopulateDropDowns(LocalListCandidate localListCandidate = null)
        {
            ViewBag.LocalListingID = new SelectList(db.LocalLists, "LocalListingID", "LocalListingName", localListCandidate?.LocalListingID);
            ViewBag.Religions = new SelectList(new List<string> { "مسلم", "مسيحي" }, localListCandidate?.Religion);
            ViewBag.Genders = new SelectList(new List<string> { "ذكر", "أنثى" }, localListCandidate?.Gender);
            ViewBag.Governorates = new SelectList(new List<string> { "إربد", "عجلون" ,"عمان "}, localListCandidate?.Governorate);
            ViewBag.ElectionAreas = new SelectList(new List<string> { "إربد الأولى", "إربد الثانية", "عجلون" ,"عمان الأولى","عمان الثانية","عمان الثالثة"}, localListCandidate?.ElectionArea);
            ViewBag.TypeChairs = new SelectList(new List<string> { "كوتا", "مسيحي", "تنافس" }, localListCandidate?.Type_Chair);
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        // GET: LocalListCandidates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalListCandidate localListCandidate = db.LocalListCandidates.Find(id);
            if (localListCandidate == null)
            {
                return HttpNotFound();
            }
            PopulateDropDowns(localListCandidate);
            return View(localListCandidate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidateID,CandidateName,NationalNumber,Gender,Religion,Type_Chair,BirthdateCandidate,Governorate,ElectionArea,NumberOfVotesCandidate,LocalListingID,Status")] LocalListCandidate localListCandidate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(localListCandidate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDropDowns(localListCandidate);
            return View(localListCandidate);
        }

        // GET: LocalListCandidates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalListCandidate localListCandidate = db.LocalListCandidates.Find(id);
            if (localListCandidate == null)
            {
                return HttpNotFound();
            }
            return View(localListCandidate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocalListCandidate localListCandidate = db.LocalListCandidates.Find(id);
            db.LocalListCandidates.Remove(localListCandidate);
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



        public ActionResult AcceptCandidate(int id)
        {
            var candidate = db.LocalListCandidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }

            // التحقق من العمر
            DateTime birthDate;
            if (DateTime.TryParse(candidate.BirthdateCandidate, out birthDate))
            {
                int age = DateTime.Now.Year - birthDate.Year;
                if (birthDate > DateTime.Now.AddYears(-age)) age--;

                if (age >= 25)
                {
                    // قبول المفوض وإرسال بريد إلكتروني
                    candidate.Status = "Accepted";
                    db.Entry(candidate).State = EntityState.Modified;
                    db.SaveChanges();

                    SendEmailToCommissioner(candidate, "تم قبول طلبك", "مرحبًا، تم قبول طلب ترشحك.");
                }
                else
                {
                    // رفض المفوض وإرسال بريد إلكتروني
                    candidate.Status = "Rejected";
                    db.Entry(candidate).State = EntityState.Modified;
                    db.SaveChanges();

                    SendEmailToCommissioner(candidate, "تم رفض طلبك", "مرحبًا، تم رفض طلب ترشحك بسبب عدم تحقيق شرط العمر.");
                }
            }
            else
            {
                ModelState.AddModelError("", "تنسيق تاريخ الميلاد غير صحيح.");
            }

            return RedirectToAction("Index");
        }

        private void SendEmailToCommissioner(LocalListCandidate candidate, string subject, string body)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("teamorange077@gmail.com", "thxc lkmk weir bxbq"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("teamorange077@gmail.com"),
                Subject = subject,
                Body = $"مرحبًا {candidate.CandidateName}, \n\n{body}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(candidate.Email);

            client.Send(mailMessage);
        }

        public ActionResult RejectCandidate(int id)
        {
            var candidate = db.LocalListCandidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }

            // رفض المفوض وإرسال بريد إلكتروني
            candidate.Status = "Rejected";
            db.Entry(candidate).State = EntityState.Modified;
            db.SaveChanges();

            SendEmailToCommissioner(candidate, "تم رفض طلبك", "مرحبًا، تم رفض طلب ترشحك.");

            return RedirectToAction("Index");
        }


    }



}