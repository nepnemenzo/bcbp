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
    public class ParticipantController : Controller
    {
        // GET: Participant
        public ActionResult Index()
        {
            #region List
            ParticipantModel model = new ParticipantModel();
            model.MemberList = ParticipantData.GetMemberParticipantList();
            model.GroupList = ParticipantData.GetGroupList();
            model.StatusList = ParticipantData.GetStatusList();

            return View(model);
        }

        [Route("get/Participantdata")]
        public ActionResult ParticipantList()
        {
            List<ParticipantModel> model = ParticipantData.ParticipantList(0);
            if (model.Count() == 0)
            {
                return Json(new { status = false, msg = "No data found!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //get supplier name
            }

            return Json(new { status = true, msg = "Success", data = RenderPartialViewToString("_ParticipantListPartial", model) }, JsonRequestBehavior.AllowGet);
        }
        #endregion List



        #region Save
        public ActionResult SaveParticipant(ParticipantModel model)
        {
            ParticipantData MemRepo = new ParticipantData();
            model.CLPid = 1;
            model.DateEntered = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            model.EnteredBy = 1;        //user
            model.ModifiedBy = 1;       //user

            if (ModelState.IsValid)
            {
                ResponseModel isSave = MemRepo.SaveParticipant(model);
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

        public ActionResult ModifyParticipant(int id)
        {
            ParticipantData partRepo = new ParticipantData();
            ParticipantModel model = partRepo.GetParticipant(id);

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

        //*******------------------------
    }
}