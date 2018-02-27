using bcbp_101.Models.bcbp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bcbp_101.DataModel.bcbp;
using System.IO;
using bcbp_101.Core;

namespace bcbp_101.Controllers.bcbp
{
    public class ServantController : Controller
    {
        // GET: Servant
        public ActionResult Index()
        {
            ServantModel model = new ServantModel();
            model.MemberList = ServantData.GetMemberList();
            model.ServiceList = ServantData.GetServiceList();
            model.CLPList = ServantData.GetCLPList();

            return View(model);
        }

        [Route("get/ServantList")]
        public ActionResult ServantList()
        {
            List<ServantModel> model = ServantData.GetServantsList(0);
            if (model.Count() == 0)
            {
                return Json(new { status = false, msg = "No data found!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //get supplier name
            }

            return Json(new { status = true, msg = "Success", data = RenderPartialViewToString("_ServantListPartial", model) }, JsonRequestBehavior.AllowGet);
        }




        #region Save
        public ActionResult SaveServant(ServantModel model)
        {
            ServantData MemRepo = new ServantData();
            model.DateEntered = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            model.EnteredBy = 1;        //user
            model.ModifiedBy = 1;       //user

            if (ModelState.IsValid)
            {
                ResponseModel isSave = MemRepo.SaveServant(model);
                if (isSave.status == 1)
                {
                    Connection.CommitTransaction();
                    Connection.CloseConnection();
                    return Json(new { status = true, code = 2, msg = "Successfuly Created." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Connection.RollbackTransaction();
                    Connection.CloseConnection();
                    return Json(new { status = false, code = 0, msg = "Something went wrong." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(new { status = false, code = 0, msg = "Something went wrong." }, JsonRequestBehavior.AllowGet);

        }
        #endregion Save




        

        #region Edit

        public ActionResult ModifyServant(int id)
        {
            ServantData partRepo = new ServantData();
            ServantModel model = partRepo.GetServantDetails(id);

            if (model.id > 0)
            {
                return Json(new { status = true, result = model, msg = "Successfuly Created." }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new { status = false, code = 0, msg = "Something went wrong." }, JsonRequestBehavior.AllowGet);

        }


        #endregion Edit





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