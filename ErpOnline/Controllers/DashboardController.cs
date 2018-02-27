using bcbp_101.DataModel;
using bcbp_101.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ErpOnline.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            List<DashboardModel> Listofbranches = DashboardData.LisstOfBranches();
            ViewBag.ListofBranch = Listofbranches;
            return View();
        }
    }
}