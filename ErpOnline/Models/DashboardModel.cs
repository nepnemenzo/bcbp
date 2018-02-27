using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bcbp_101.Models
{
    public class DashboardModel
    {
        public int branchID { get; set; }
        public string name { get; set; }
    }

    public class StockResponse
    {
        public string itemname { get; set; }
        public decimal qty { get; set; }
    }
}