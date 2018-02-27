using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bcbp_101.Models.bcbp;
using bcbp_101.DataModel.bcbp;
using System.IO;

namespace bcpb_101.Controllers.bcbp
{
    public class CLPAttendanceController : Controller
    {
        #region AttendanceList
        // GET: InvBranch
        public ActionResult Index()
        {
            return View();
        }

        [Route("get/AttendanceList")]
        public ActionResult AttendanceList()
        {
            List<CLPAttendanceModel> model = CLPAttendanceData.AttendanceList(0);
            if (model.Count() == 0)
            {
                return Json(new { status = false, msg = "No data found!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //get supplier name
            }

            return Json(new { status = true, msg = "Success", data = RenderPartialViewToString("_CLPAttendanceListPartial", model) }, JsonRequestBehavior.AllowGet);
        }

        #endregion AttendanceList









        #region ConvertPartialViewToString
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
        #endregion ConvertPartialViewToString
    }
}