using bcbp_101.DataModel;
using bcbp_101.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bcbp_101.Models.bcbp;
using bcbp_101.Core;

namespace bcbp_101.Controllers
{
    [Authorize]
    public class InvUserController : Controller
    {
        public ActionResult Index()
        {
            UserIfo model = new UserIfo();
            //branch 
            //model.listOfBranch = InvBranchData.branchList(0);
            return View(model);
        }

        public ActionResult GetUserList()
        {
            List<UserIfo> model = InvUserData.UserList(0);
            if (model.Count() == 0)
            {
                return Json(new { status = false, msg = "No data found!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = true, msg = "Success", data = RenderPartialViewToString("_UserPartial", model) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(UserIfo usr)
        {
            if (usr.UserNr == 0)
            {
                ResponseModel model = InvUserData.SaveUser(usr);
                if (model.status == 1)
                    return Json(new { status = true, msg = "Successfuly saved." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, msg = "Someething went wrong." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ResponseModel model = InvUserData.UpdateUser(usr);
                if (model.status == 1)
                    return Json(new { status = true, msg = "Successfuly updated." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, msg = "Someething went wrong." }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetUserInfo(int id)
        {
            UserIfo model = InvUserData.UserData(id);
            if (model != null)
            {
                return Json(new { status = true, data = model }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, msg = "No data found!" }, JsonRequestBehavior.AllowGet);
        }

        #region ConvertPartialViewToString
        //Convert Partial view to string
        protected string RenderPartialViewToString()
        {
            return RenderPartialViewToString(null, null);
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
        }

        protected string RenderPartialViewToString(object model)
        {
            return RenderPartialViewToString(null, model);
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        // End
        #endregion ConvertPartialViewToString
    }
}