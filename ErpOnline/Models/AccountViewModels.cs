using bcbp_101.Models.bcbp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace bcbp_101.Models
{
    public class UserIfo
    {
        public int UserNr { get; set; }
        public string UserName { get; set; }
        public string UserPaswr { get; set; }
        public int EmployeeNumber { get; set; }
        public string FName { get; set; }
        public string MIName { get; set; }
        public string LName { get; set; }
        public int UserAccount { get; set; }
        public int UserInactive { get; set; }
        public int EnteredBy { get; set; }
        public DateTime DateEntered { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime DateModified { get; set; }
        public int AccessRights { get; set; }
        public int Branch { get; set; }
        public int loginStatus { get; set; }
        public int IsAllow { get; set; }
        public List<ChapterModels> listOfBranch { get; set; }

    }

    public class UserResponse
    {
        public UserIfo Userdata { get; set; }
        public int status { get; set; }
        public string msg { get; set; }
    }
}
