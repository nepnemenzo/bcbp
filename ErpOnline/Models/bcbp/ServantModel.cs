using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bcbp_101.Models.bcbp
{
    public class ServantModel
    {
        public int id { get; set; }

        [Display(Name = "Member")]
        public int memberId { get; set; }


        [Display(Name = "Name")]
        public string name { get; set; }


        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Display(Name = "Service")]
        public string ServiceName { get; set; }

        [Display(Name = "CLP")]
        public int CLPId { get; set; }
        [Display(Name = "CLP")]
        public string clpName { get; set; }


        [Display(Name = "Entered By")]
        public int EnteredBy { get; set; }

        [Display(Name = "Modified By")]
        public int ModifiedBy { get; set; }

        [Display(Name = "Entered Date")]
        public DateTime DateEntered { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }


        //list
        public List<GenericList> MemberList { get; set; }
        public List<GenericList> ServiceList { get; set; }
        public List<GenericList> CLPList { get; set; }


    }
}