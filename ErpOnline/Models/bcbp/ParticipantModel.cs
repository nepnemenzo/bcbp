using bcbp_101.Models.bcbp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bcbp_101.Models.bcbp
{
    public class ParticipantModel
    {
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "Participant")]
        public int memberId { get; set; }

        [Display(Name = "Name")]
        public string name { get; set; }


        public int GroupId { get; set; }

        [Display(Name = "Group")]
        public string GroupName { get; set; }


        [Display(Name = "Sponsor")]
        public string SponsorName { get; set; }


        public int CLPid { get; set; }
        [Display(Name = "CLP")]
        public string CLPname { get; set; }


        public int statusID { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }


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
        public List<GenericList> GroupList { get; set; }
        public List<GenericList> StatusList { get; set; }


    }
}