using fstCopy_Proj5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fstCopy_Proj5.Controllers
{
    public class PartyResultController : Controller
    {
        private ElectionEntities db = new ElectionEntities();

        public ActionResult Index()
        {
            ViewBag.TotalUsers = usersNumber();
            ViewBag.PartyPercentage = partyPercentage();
            ViewBag.PartyVotes = partyVotersCount();
            ViewBag.PartyThreshold = partyThreshold();
            ViewBag.PartiesAboveThreshold = partyListsAboveThreshold();
            ViewBag.WinningListsVotesSum = partyWinningListsVotesSum();
            ViewBag.PartyListsSeats = partyListsSeats();
            ViewBag.PartyWinningCandidates = partyListsCanditates();
            ViewBag.PartyListsAndCandidates = partyListsAndCanditates();

            return View();
        }

        public long usersNumber()
        {
            long countUsers = 0;
            var allUsers = db.Users.ToList();

            foreach (var row in allUsers)
            {
                countUsers++;
            }
            return countUsers;
        }

        public double partyPercentage()
        {
            double percentage = (double)partyVotersCount() / (double)usersNumber();
            return (Math.Round(percentage, 2)) * 100;
        }

        public long partyVotersCount()
        {
            long partyVotes = 0;
            var allUsers = db.Users.ToList();

            foreach (var row in allUsers)
            {
                partyVotes += Convert.ToInt64(row.PartyElections);
            }

            return partyVotes;
        }

        public double partyThreshold()
        {
            double partyThreshold = partyVotersCount() * 0.025;
            return partyThreshold;
        }

        public List<string> partyListsAboveThreshold()
        {
            List<string> thresholdParty = new List<string>();
            var allPartyLists = db.GeneralListings.ToList();

            foreach (var row in allPartyLists)
            {
                if (row.NumberOfVotes > partyThreshold())
                {
                    string addedList = $"{row.GeneralListingID}, {row.Name}, {row.NumberOfVotes}";
                    thresholdParty.Add(addedList);
                }
            }

            return thresholdParty;
        }

        public long partyWinningListsVotesSum()
        {
            long winningListsVotesSum = 0;
            var winningLists = partyListsAboveThreshold().ToList();

            foreach (var row in winningLists)
            {
                var winningListsRows = String.Join(", ", row);
                string[] listsArray = winningListsRows.Split(',');
                long listVotes = long.Parse(listsArray.Last());

                winningListsVotesSum += listVotes;
            }

            return winningListsVotesSum;
        }

        public List<string> partyListsSeats()
        {
            var Lists = partyListsAboveThreshold().ToList();
            var winningLists = new List<string>();

            foreach (var row in Lists)
            {
                string listRow = row;

                var winningListsRows = String.Join(", ", row);
                string[] listsArray = winningListsRows.Split(',');

                long listVotes = long.Parse(listsArray.Last());
                long sum = partyWinningListsVotesSum();

                decimal percentage = (decimal)listVotes / (decimal)sum;
                decimal seatsWon = percentage * (decimal)41;

                listRow += $", {Math.Round(seatsWon)}";
                winningLists.Add(listRow);
            }

            return winningLists;
        }

        public List<string> partyListsCanditates()
        {
            var partyCan = partyListsAboveThreshold().ToList();
            var partyCanditates = db.GeneralListCandidates.ToList();

            List<string> partyWinningCanditates = new List<string>();

            foreach (var row in partyCan)
            {
                var winningListsRows = String.Join(", ", row);
                string[] listsArray = winningListsRows.Split(',');

                string listName = listsArray[1].Trim();

                foreach (var can in partyCanditates)
                {
                    if (can.GeneralListingName == listName)
                    {
                        string candidateNameAndId = $"{can.CandidateID}, {can.CandidateName}, {can.GeneralListingName}";
                        partyWinningCanditates.Add(candidateNameAndId);
                    }
                }
            }

            try
            {
                partyWinningCanditates = partyWinningCanditates
                    .OrderBy(can => long.Parse(can.Split(',').Last().Trim()))
                    .ToList();
            }
            catch (FormatException ex)
            {
                // Handle the format exception (e.g., log the error, ignore the entry, etc.)
                // Optionally, you might want to log which specific candidate caused the error:
                System.Diagnostics.Debug.WriteLine("Error parsing candidate data: " + ex.Message);
            }

            return partyWinningCanditates;
        }


        public List<string> partyListsAndCanditates()
        {
            var partyListsWinners = partyListsSeats().ToList();
            var partyCanditates = partyListsCanditates().ToList();

            List<string> partyWinners = new List<string>();

            foreach (var row in partyListsWinners)
            {
                string listRow = row;

                var winningListsRows = String.Join(", ", row);
                string[] listsArray = winningListsRows.Split(',');

                long listsSeats = long.Parse(listsArray.Last());

                string listName = listsArray[1];

                long seatsCount = 0;

                foreach (var can in partyCanditates)
                {
                    string canRow = can;

                    var canRowsList = string.Join(", ", canRow);
                    string[] canArray = canRowsList.Split(',');

                    if (canArray[2] == listName)
                    {
                        string canditateNameAndId = $"{listsArray[0]}, {listsArray[1]}, {listsArray[2]}, {canArray[0]}, {canArray[1]}";

                        partyWinners.Add(canditateNameAndId);

                        seatsCount++;

                        if (seatsCount == listsSeats)
                        {
                            break;
                        }
                    }
                }
            }
            return partyWinners;
        }
    }
}
