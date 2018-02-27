using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bcbp_101.Models.bcbp
{
    public class MemberModel
    {
        [Display(Name = "Id")]
        public int id { get; set; }

        [Display(Name = "Name")]
        public string FirstName { get; set; }

        [Display(Name = "MidName")]
        public string MidName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email Add")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAdd { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Province")]
        public string Province { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Area Code must be numeric")]
        [Display(Name = "Area Code")]
        public int AreaCode { get; set; }

        [Display(Name = "Mobile #")]
        //[RegularExpression(@"^([0]|\+91[\-\s]?)?[789]\d{9}$", ErrorMessage = "Entered Mobile No is not valid.")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Entered Mobile No is not valid")]
        public string contactNum { get; set; }


        [Display(Name = "Group")]
        public int bcbpGroup { get; set; }

        [Display(Name = "Position")]
        public string PositionName { get; set; }
        public int Position { get; set; }

        [Display(Name = "Sponsor")]
        public int SponsorId { get; set; }
        public string SponsorName { get; set; }

        [Display(Name = "Status")]
        public string statusName { get; set; }
        public int status { get; set; }

        [Display(Name = "Member since")]
        public int YearOfMembership { get; set; }

        [Display(Name = "CLP Number")]
        public int CLPNum { get; set; }

        [Display(Name = "Chapter")]
        public int Chapter { get; set; }

        [Display(Name = "Access Type")]
        public int AccessType { get; set; }

        [Display(Name = "image")]
        public string image { get; set; }

        [Display(Name = "Entered By")]
        public int enteredBy { get; set; }

        [Display(Name = "Modified By")]
        public int modifiedBy { get; set; }

        [Display(Name = "Entered Date")]
        public DateTime enteredDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime modifiedDate { get; set; }

        [Display(Name = "Name")]
        public string memName { get; set; }

        public List<int> ListyearOfMembership { get; set; }
        public List<GenericList> StatusList { get; set; }
        public SelectList statList { get; set; }
        public List<GenericList> PositionList { get; set; }
        public List<int> CLPList { get; set; }
        public List<GenericList> ChapList { get; set; }
        public List<GenericList> AccessTypeList { get; set; }
        public List<GenericList> SponsorList { get; set; }
    }

}