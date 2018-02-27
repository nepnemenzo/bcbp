using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bcbp_101.Models.bcbp
{
    public class CLPAttendanceModel
    {
        public int id { get; set; }

        [Display (Name = "ID")]
        public int participantId { get; set; }

        [Display (Name = "Participant")]
        public string participantName { get; set; }

        [Display (Name = "1St Talk")]
        public bool talka { get; set; }


        [Display (Name = "2nd Talk")]
        public bool talkb { get; set; }

        [Display (Name = "3rd Talk")]
        public bool talkc { get; set; }

        [Display (Name = "4th Talk")]
        public bool talkd { get; set; }

        [Display (Name = "Group")]
        public string groupName { get; set; }

        [Display (Name = "Sponsor")]
        public string sponsor { get; set; }
        public DateTime attDate { get; set; }
    }
}