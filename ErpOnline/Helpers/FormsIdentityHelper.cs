using bcbp_101.Models;
using System;
using System.Web.Security;

namespace bcbp_101.Helpers
{
    public class FormsIdentityHelper
    {
        private string _CookiePath = string.Empty;
        private string _Expiration = string.Empty;
        private string _Expired = string.Empty;
        private string _IsPersistent = string.Empty;
        private string _IssueDate = string.Empty;
        private string _name = string.Empty;
        private string _version = string.Empty;

        private int _UserNr = 0;
        private string _UserName = string.Empty;
        private string _UserPaswr = string.Empty;
        private int _EmployeeNumber = 0;
        private string _FName = string.Empty;
        private string _MIName = string.Empty;
        private string _LName = string.Empty;
        private int _UserAccount = 0;
        private int _UserInactive = 0;
        private int _AccessRights = 0;
        private int _Branch = 0;
        private int _loginStatus = 0;
        private int _IsAllow = 0;

        public FormsIdentityHelper()
        {
            FormsIdentity id = (FormsIdentity)System.Web.HttpContext.Current.User.Identity;
            FormsAuthenticationTicket ticket = id.Ticket;

            _CookiePath = ticket.CookiePath;
            _Expiration = ticket.Expiration.ToString();
            _Expired = ticket.Expired.ToString();
            _IsPersistent = ticket.IsPersistent.ToString();
            _IssueDate = ticket.IssueDate.ToString();
            _name = ticket.Name;
            _version = ticket.Version.ToString();

            var x = Newtonsoft.Json.JsonConvert.DeserializeObject<UserIfo>(ticket.UserData);

            _UserNr = x.UserNr;
            _UserName = x.UserName;
            _UserPaswr = x.UserPaswr;
            _EmployeeNumber = x.EmployeeNumber;
            _FName = x.FName;
            _MIName = x.MIName;
            _LName = x.LName;
            _UserAccount = x.UserAccount;
            _UserInactive = x.UserInactive;
            _AccessRights = x.AccessRights;
            _Branch = x.Branch;
            _loginStatus = x.loginStatus;
            _IsAllow = x.IsAllow;
        }

        public string CookiePath { get { return _CookiePath; } set { value = _CookiePath; } }
        public string Expiration { get { return _Expiration; } set { value = _Expiration; } }
        public string Expired { get { return _Expired; } set { value = _Expired; } }
        public string IsPersistent { get { return _IsPersistent; } set { value = _IsPersistent; } }
        public string IssueDate { get { return _IssueDate; } set { value = _IssueDate; } }

        public string name { get { return _name; } set { value = _name; } }
        public string version { get { return _version; } set { value = _version; } }
        public int UserNr { get { return _UserNr; } set { value = _UserNr; } }
        public string UserName { get { return _UserName; } set { value = _UserName; } }
        public string UserPaswr { get { return _UserPaswr; } set { value = _UserPaswr; } }
        public int EmployeeNumber { get { return _EmployeeNumber; } set { value = _EmployeeNumber; } }
        public string FName { get { return _FName; } set { value = _FName; } }
        public string MIName { get { return _MIName; } set { value = _MIName; } }
        public string LName { get { return _LName; } set { value = _LName; } }
        public int UserAccount { get { return _UserAccount; } set { value = _UserAccount; } }
        public int UserInactive { get { return _UserInactive; } set { value = _UserInactive; } }
        public int AccessRights { get { return _AccessRights; } set { value = _AccessRights; } }
        public int Branch { get { return _Branch; } set { value = _Branch; } }
        public int loginStatus { get { return _loginStatus; } set { value = _loginStatus; } }
        public int IsAllow { get { return _IsAllow; } set { value = _IsAllow; } }
    }
}