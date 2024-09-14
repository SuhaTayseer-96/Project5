using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fstCopy_Proj5.Models
{
    public class LocalListViewModel
    {
        public int? SelectedListId { get; set; }
        public List<int> SelectedCandidateIds { get; set; }
        public List<LocalList> LocalLists { get; set; }
        public Dictionary<int, List<LocalListCandidate>> CandidatesByList { get; set; }
    }
}