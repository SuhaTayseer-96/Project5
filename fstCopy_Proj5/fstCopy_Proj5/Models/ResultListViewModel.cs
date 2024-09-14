using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fstCopy_Proj5.Models
{
    public class CircleViewModel
    {
        public List<User> users_circle_local { get; set; }
        public int localvotecounter { get; set; }
        public int whitelocalcounter { get; set; }
        public double threshold { get; set; }
        public int totalVotes { get; set; }
        public List<LocalList> winning_lists { get; set; }
        public Dictionary<int, List<LocalListCandidate>> winningCandidates { get; set; } // key: list ID, value: list of top candidates



    }
}