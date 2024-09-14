using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using fstCopy_Proj5.Models;

namespace E_Voting.Controllers
{
    public class USERController : Controller
    {
        private ElectionEntities DB = new ElectionEntities();

        // GET: USER
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Logout()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Login(User user)
        {
            try
            {
                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return View();
                }

                var existingUser = DB.Users.FirstOrDefault(u => u.NationalNumber == user.NationalNumber);

                if (existingUser == null)
                {
                    ModelState.AddModelError("", "");
                    return View();
                }

                if (existingUser.Password == "password")
                {
                    string newPassword = GenerateRandomPassword();
                    existingUser.Password = newPassword;
                    DB.SaveChanges();

                    SendConfirmationEmail(existingUser.Email, newPassword);

                    ViewBag.Emailsent = "The code has been sent to your Email";
                }

                // تخزين المستخدم في الجلسة وإعادة التوجيه إلى LoginUser
                Session["LoggedUser"] = JsonConvert.SerializeObject(existingUser);
                return RedirectToAction("LoginUser", new { ID = user.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                Console.WriteLine("Exception message: " + ex.Message);
            }

            return View();
        }

        public ActionResult LoginUser(string nationalNumber)
        {
            var user = DB.Users.FirstOrDefault(u => u.NationalNumber == nationalNumber);

            if (user == null)
            {
                ModelState.AddModelError("", "");
                return View();
            }

            // Save in session
            Session["LoggedUser"] = JsonConvert.SerializeObject(user);
            ViewBag.NationalNumber = nationalNumber;
            ViewBag.Email = user.Email;

            // Redirect to the new page with buttons
            return RedirectToAction("ElectionDistricts");
        }


        //Generate password
        private string GenerateRandomPassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            int length = 8;
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        // Send Email
        private void SendConfirmationEmail(string toEmail, string confirmationCode)
        {
            string fromEmail = "teamorange077@gmail.com";
            string smtpUsername = "teamorange077@gmail.com";
            string smtpPassword = "thxc lkmk weir bxbq";
            //Credentials = new NetworkCredential("teamorange077@gmail.com", "thxc lkmk weir bxbq"),


            string subjectText = "Your Confirmation Code";
            string messageText = $"Your confirmation code is {confirmationCode}";

            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subjectText;
                mailMessage.Body = messageText;
                mailMessage.IsBodyHtml = false;

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;

                    smtpClient.Send(mailMessage);
                }
            }
        }

        public ActionResult TypeOfElection(int id)
        {
            var user = DB.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Save the user to the session
            Session["LoggedUser"] = JsonConvert.SerializeObject(user);
            ViewBag.NationalNumber = user.NationalNumber;

            // Determine the paths for elections
            ViewBag.LocalElectionsPath = (bool)user.LocalElections ? null : "LocalElections";
            ViewBag.PartyElectionsPath = (bool)user.PartyElections ? null : "PartyElections";

            return View();
        }



        public ActionResult LocalElections()
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login");
            }

            var userJson = Session["LoggedUser"].ToString();
            var user = JsonConvert.DeserializeObject<User>(userJson);

            ViewBag.UserId = user.ID;

            var localLists = DB.LocalLists.Where(l => l.ElectionArea == user.ElectionArea).ToList();
            var candidates = DB.LocalListCandidates.ToList();

            ViewBag.LocalLists = localLists;
            ViewBag.Candidates = candidates;

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocalElections(int selectedListId, int[] selectedCandidateIds)
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login");
            }

            var userJson = Session["LoggedUser"].ToString();
            var user = JsonConvert.DeserializeObject<User>(userJson);

            var selectedList = DB.LocalLists.Find(selectedListId);

            if (selectedList != null && selectedCandidateIds != null)
            {
                if (selectedList.NumberOfVotes == null)
                {
                    selectedList.NumberOfVotes = 0;
                }
                // Update the number of votes for the selected list
                selectedList.NumberOfVotes += 1;
                DB.Entry(selectedList).State = System.Data.Entity.EntityState.Modified;

                // Update the number of votes for the selected candidates
                foreach (var candidateId in selectedCandidateIds)
                {
                    var selectedCandidate = DB.LocalListCandidates.FirstOrDefault(c => c.CandidateID == candidateId && c.LocalListingID == selectedListId);
                    if (selectedCandidate != null)
                    {
                        if (selectedCandidate.NumberOfVotesCandidate == null)
                        {
                            selectedCandidate.NumberOfVotesCandidate = 0;
                        }
                        selectedCandidate.NumberOfVotesCandidate += 1;
                        DB.Entry(selectedCandidate).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                // Update user table to reflect that the user has voted in local elections
                user.LocalElections = true;
                DB.Entry(user).State = System.Data.Entity.EntityState.Modified;

                DB.SaveChanges();
            }

            return RedirectToAction("TypeOfElection", new { id = user.ID });
        }


        public ActionResult PartyElections()
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login");
            }

            var userJson = Session["LoggedUser"].ToString();
            var user = JsonConvert.DeserializeObject<User>(userJson);

            ViewBag.UserId = user.ID;

            // Retrieve all party lists to display in the view
            var partyLists = DB.GeneralListings.ToList();
            return View(partyLists);
        }

        [HttpPost]
        public ActionResult PartyElections(int selectedPartyListId)
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login");
            }

            var userJson = Session["LoggedUser"].ToString();
            var user = JsonConvert.DeserializeObject<User>(userJson);

            var selectedPartyList = DB.GeneralListings.Where(x=> x.GeneralListingID  == selectedPartyListId).FirstOrDefault();
            if (selectedPartyList != null)
            {
                // Update user table to reflect that the user has voted in party elections
                user.PartyElections = true;
                //user.LocalElections = true; // Optionally, you can set LocalElections to true here if needed
                if (selectedPartyList.NumberOfVotes == null)
                {
                    selectedPartyList.NumberOfVotes = 0;
                }
                // Update the party list to increment the number of votes
                selectedPartyList.NumberOfVotes += 1;
                DB.Entry(selectedPartyList).State = System.Data.Entity.EntityState.Modified;
                DB.Entry(user).State = System.Data.Entity.EntityState.Modified; // Ensure user entity is being tracked
                DB.SaveChanges();
            }

            return RedirectToAction("TypeOfElection", new { id = user.ID });
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        public ActionResult ElectionDistricts()
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login");
            }

            var userJson = Session["LoggedUser"].ToString();
            var user = JsonConvert.DeserializeObject<User>(userJson);

            ViewBag.ElectionArea = user.ElectionArea;

            return View();
        }


        public ActionResult IrbedFirstDistrict()
        {
            try
            {
                if (Session["LoggedUser"] == null)
                {
                    return RedirectToAction("Login");
                }

                var userJson = Session["LoggedUser"].ToString();
                var user = JsonConvert.DeserializeObject<User>(userJson);

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return RedirectToAction("Login");
                }

                user.ElectionArea = "اربد الأولى"; // Setting election area to IrbedFirstDistrict
                DB.Entry(user).State = System.Data.Entity.EntityState.Modified;
                DB.SaveChanges();

                // Redirect to TypeOfElection with the user's ID
                return RedirectToAction("TypeOfElection", new { id = user.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                Console.WriteLine("Exception message: " + ex.Message);
                return RedirectToAction("Login");
            }
        }


        public ActionResult IrbedSecondDistrict()
        {
            try
            {
                if (Session["LoggedUser"] == null)
                {
                    return RedirectToAction("Login");
                }

                var userJson = Session["LoggedUser"].ToString();
                var user = JsonConvert.DeserializeObject<User>(userJson);

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return RedirectToAction("Login");
                }

                user.ElectionArea = "اربد الثانية"; // Setting election area to IrbedFirstDistrict
                DB.Entry(user).State = System.Data.Entity.EntityState.Modified;
                DB.SaveChanges();

                // Redirect to TypeOfElection with the user's ID
                return RedirectToAction("TypeOfElection", new { id = user.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                Console.WriteLine("Exception message: " + ex.Message);
                return RedirectToAction("Login");
            }
        }

        public ActionResult AjlounDistrict()
        {
            try
            {
                if (Session["LoggedUser"] == null)
                {
                    return RedirectToAction("Login");
                }

                var userJson = Session["LoggedUser"].ToString();
                var user = JsonConvert.DeserializeObject<User>(userJson);

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    return RedirectToAction("Login");
                }

                user.ElectionArea = "عجلون"; // Setting election area to IrbedFirstDistrict
                DB.Entry(user).State = System.Data.Entity.EntityState.Modified;
                DB.SaveChanges();

                // Redirect to TypeOfElection with the user's ID
                return RedirectToAction("TypeOfElection", new { id = user.ID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                Console.WriteLine("Exception message: " + ex.Message);
                return RedirectToAction("Login");
            }
        }
    }


}

