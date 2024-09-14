using fstCopy_Proj5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fstCopy_Proj5.Controllers
{
    public class ResultController : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        public ActionResult Index()
        {
            return View();
        }

        // GET: LocalResult
        public ActionResult LocalResult()
        {
            List<CircleViewModel> objects_list = new List<CircleViewModel>();

            var availableSeats = new Dictionary<int, int>
            {
                { 1, 7 },
                { 2, 5 },
                { 3, 3 }
            };

            string[] arr = { "اربد الاولى", "اربد الثانية", "عجلون" };
            for (int counter = 1; counter <= 3; counter++)
            {
                
                //string x = $"Area {counter}";
                CircleViewModel circleViewModel = new CircleViewModel();
                string selectedArea = arr[counter - 1];

                var records = db.Users
                                .Where(u => u.ElectionArea == selectedArea && (u.LocalElections == true || u.whitePaperLocalElections == true))
                                .ToList();

                circleViewModel.users_circle_local = records;
                circleViewModel.localvotecounter = records.Count(u => u.LocalElections == true);
                circleViewModel.whitelocalcounter = records.Count(u => u.whitePaperLocalElections == true);
                circleViewModel.totalVotes = records.Count();
                circleViewModel.threshold = circleViewModel.totalVotes * 0.07;

                var local_lists = db.LocalLists.Where(l => l.ElectionArea == selectedArea).ToList();
                var winning_lists = local_lists.Where(l => l.NumberOfVotes  >= circleViewModel.threshold).ToList();

                if (!winning_lists.Any() && local_lists.Any())
                {
                    var maxCounterList = local_lists.OrderByDescending(l => l.NumberOfVotes).FirstOrDefault();
                    if (maxCounterList != null)
                    {
                        winning_lists.Add(maxCounterList);
                    }
                }

                double sumOfCounters = (double)winning_lists.Sum(l => l.NumberOfVotes);

                if (sumOfCounters == 0)
                {
                    sumOfCounters = 1;
                }

                var winningCandidates = new Dictionary<int, List<LocalListCandidate>>();

                foreach (var list in winning_lists)
                {
                    list.ActualSeats = (int)Math.Max(Math.Round((double)(list.NumberOfVotes / sumOfCounters * availableSeats[counter])), 1);

                    var candidates = db.LocalListCandidates
                                       .Where(c => c.LocalListingID == list.ID)
                                       .OrderByDescending(c => c.NumberOfVotesCandidate)
                                       .Take(list.ActualSeats)
                                       .ToList();

                    winningCandidates[list.ID] = candidates;
                }

                circleViewModel.winning_lists = winning_lists.Where(l => l.ActualSeats > 0).ToList();
                circleViewModel.winningCandidates = winningCandidates;

                objects_list.Add(circleViewModel);
            }

            return View(objects_list);
        }

        
    }
}