using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using bcbp_101.Models.bcbp;
using bcbp_101.Core;

namespace bcbp_101.Helpers
{
    public class CommonHelper
    {
        public static datestring GetServerDate()
        {
            SqlConnection _con = new SqlConnection(Connection.connString);
            var res = _con.Query<datestring>("SELECT GETDATE() AS serverdate").ToArray();
            return res.Count() != 0 ? res[0] : null;
        }

        public static ResponseModel moveImage(HttpPostedFileBase file)
        {
            ResponseModel resp = new ResponseModel();
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/Items"), Path.GetFileName(file.FileName));
                    if (System.IO.File.Exists(path))
                    {
                        resp.status = 0;
                        resp.msg = "Already Exist";
                    }
                    else
                    {
                        file.SaveAs(path);
                        resp.status = 1;
                        resp.msg = "Success";
                    }
                } else {
                    resp.status = 0;
                    resp.msg = "No file";
                }
            }
            catch (Exception ex)
            {
                resp.status = 0;
                resp.msg = ex.Message;
            }
            return resp;
        }


    }

    public class datestring
    {
        public string serverdate { get; set; }
    }
}