//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace fstCopy_Proj5.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class GeneralListCandidate
    {
        public int CandidateID { get; set; }
        public string GeneralListingName { get; set; }
        public string CandidateName { get; set; }
        public int NationalNumber { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Religion { get; set; }
        public string Status { get; set; }
    
        public virtual GeneralListing GeneralListing { get; set; }
    }
}