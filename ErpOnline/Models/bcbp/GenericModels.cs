using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bcbp_101.Models.bcbp
{

    public class ListStatus
    {
        public int value { get; set; }

        public string text { get; set; }

    }

    public class GenericList
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class ResponseModel
    {
        public int status { get; set; }
        public string msg { get; set; }
    }
}