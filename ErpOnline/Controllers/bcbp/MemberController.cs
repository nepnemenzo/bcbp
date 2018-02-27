using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bcbp_101.DataModel.bcbp;
using System.IO;
using bcbp_101.Core;
using bcbp_101.Models.bcbp;
//using bcbpcebuCentral.Repository;

namespace bcbp_101.Controllers.bcbp
{
    //http://www.c-sharpcorner.com/UploadFile/0c1bb2/crud-operations-in-Asp-Net-mvc-5-using-dapper-orm/
    //https://code.msdn.microsoft.com/How-to-use-Apache-log4net-0d969339

    public class MemberController : Controller
    {
        #region MemberList
        // GET: InvBranch
        public ActionResult Index()
        {
            return View();
        }

        [Route("get/Memberdata")]
        public ActionResult MemberList()
        {
            List<MemberModel> model = MembersData.MemberList(0);
            if (model.Count() == 0)
            {
                return Json(new { status = false, msg = "No data found!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //get supplier name
            }

            return Json(new { status = true, msg = "Success", data = RenderPartialViewToString("_MemberListPartial", model) }, JsonRequestBehavior.AllowGet);
        }

        #endregion BranchList

        #region Create

        public ActionResult Create()
        {
            MembersData MemRepo = new MembersData();
            MemberModel mem = new MemberModel();
            mem.ListyearOfMembership = GetYearofMembership();
            mem.StatusList = MemRepo.GetStatusList();           
            mem.PositionList = MemRepo.GetPositionList();
            mem.CLPList = GetCLPNumList();
            mem.ChapList = MemRepo.GetChapterList();
            mem.AccessTypeList = MemRepo.GetAcccessTypeList();
            mem.SponsorList = MemRepo.GetSponsorList();
            return View(mem);
        }
        //save
        [HttpPost]
        public ActionResult Create(MemberModel membr)
        {
            MembersData MemRepo = new MembersData();

            //if (ModelState.IsValid)
            //{
                ResponseModel isSave = MemRepo.SaveMember(membr);
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
            //}

            MemberModel mem = new MemberModel();
            mem.ListyearOfMembership = GetYearofMembership();
            mem.StatusList = MemRepo.GetStatusList();       //MemRepo.GetStatusList_1();
            mem.PositionList = MemRepo.GetPositionList();
            mem.CLPList = GetCLPNumList();
            mem.ChapList = MemRepo.GetChapterList();
            mem.AccessTypeList = MemRepo.GetAcccessTypeList();
            mem.SponsorList = MemRepo.GetSponsorList();
            return Json(new { status = false, code = 0, msg = "Something went wrong." }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");
        }

        #endregion Create


        #region Edit
       
        public ActionResult Edit(int id)
        {
            MembersData MemRepo = new MembersData();
            MemberModel mem = MembersData.MemberDetails(id);
            if (mem != null)
            {
                mem.ListyearOfMembership = GetYearofMembership();
                mem.StatusList = MemRepo.GetStatusList();
                mem.PositionList = MemRepo.GetPositionList();
                mem.CLPList = GetCLPNumList();
                mem.ChapList = MemRepo.GetChapterList();
                mem.AccessTypeList = MemRepo.GetAcccessTypeList();
                mem.SponsorList = MemRepo.GetSponsorList();
            }
            return View(mem);

            //return Json(new { status = true, msg = "Success", data = mem }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(MemberModel membr)
        {
            MembersData MemRepo = new MembersData();

            ResponseModel isSave = MemRepo.UpdateMember(membr, membr.id);
            if (isSave.status == 1)
            {
                Connection.CommitTransaction();
                Connection.CloseConnection();
                return Json(new { status = true, code = 2, msg = "Successfuly Updated." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Connection.RollbackTransaction();
                Connection.CloseConnection();
                return Json(new { status = false, code = 0, msg = "Something went wrong." }, JsonRequestBehavior.AllowGet);
            }

            MemberModel mem = new MemberModel();
            mem.ListyearOfMembership = GetYearofMembership();
            mem.StatusList = MemRepo.GetStatusList();       //MemRepo.GetStatusList_1();
            mem.PositionList = MemRepo.GetPositionList();
            mem.CLPList = GetCLPNumList();
            mem.ChapList = MemRepo.GetChapterList();
            mem.AccessTypeList = MemRepo.GetAcccessTypeList();
            mem.SponsorList = MemRepo.GetSponsorList();
            return RedirectToAction("index");
        }

        #endregion Edit


        #region misc
        private List<int> GetYearofMembership()
        {
            int yearStart = 1990;
            int yearEnd = DateTime.Now.Year;
            List<int> nYear = new List<int>();
            for (int i = yearEnd; i >= yearStart; i--)
            {
                nYear.Add(i);
            }
            return nYear;
        }
        private List<int> GetCLPNumList()
        {
            int CLPstart = 1;
            int CLPEnd = 11;
            List<int> clp = new List<int>();
            for (int i = CLPEnd; i >= CLPstart; i--)
            {
                clp.Add(i);
            }
            return clp;
        }
        #endregion misc

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