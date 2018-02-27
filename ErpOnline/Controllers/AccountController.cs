using System.Web.Mvc;
using bcbp_101.Models;
using bcbp_101.DataModel;
using System.Web.Security;

namespace ErpOnline.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Login(UserIfo model)
        {
            UserResponse usrdata = AccountData.AccountAuth(model.UserName.Trim(), model.UserPaswr.Trim());
            if (usrdata.status == 1) {
                AccountData.SetCookie(true, usrdata.Userdata);
                return Json(new { status = true, msg = "Success.", url = Url.Action("Index", "Dashboard") }, JsonRequestBehavior.AllowGet);
            } else if(usrdata.status == 2) {
                return Json(new { status = false, msg = usrdata.msg }, JsonRequestBehavior.AllowGet);
            } else {
                return Json(new { status = false, msg = usrdata.msg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}