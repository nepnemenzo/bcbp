using bcbp_101.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using bcbp_101.Models.bcbp;
using bcbp_101.Models;
using bcbp_101.Reflection;

namespace bcbp_101.DataModel
{
    public class InvUserData
    {
        public static List<UserIfo> UserList(int id)
        {
            string query = string.Empty;
            List<UserIfo> users = new List<UserIfo>();
            if (id != 0)
                query = "SELECT * FROM [User] WHERE UserNr = " + id + "";
            else
                query = "SELECT * FROM [User]";

            DataSet ds = Connection.connectSQLServerQuery(query);
            users = BaseReflection.DatasetBDToObjectList<UserIfo>(ds);
            return users;
        }

        public static UserIfo UserData(int id)
        {
            string query = string.Empty;
            List<UserIfo> user = new List<UserIfo>();
            if (id != 0)
                query = "SELECT * FROM [User] WHERE UserNr = " + id + "";
            else
                query = "SELECT * FROM [User]";

            DataSet ds = Connection.connectSQLServerQuery(query);
            user = BaseReflection.DatasetBDToObjectList<UserIfo>(ds);

            UserIfo model = (user != null && user.Count > 0) ? user[0] : null;
            return model;
        }

        public static ResponseModel SaveUser(UserIfo model)
        {
            int id = 0;
            model.DateEntered = DateTime.Now;
            model.DateModified = DateTime.Now;
            ResponseModel response = new ResponseModel();
            try
            {
                List<string> lgp_fields = new List<string>() { "UserName", "UserPaswr", "EmployeeNumber", "FName", "MIName", "LName", "UserAccount", "UserInactive", "EnteredBy", "DateEntered", "ModifiedBy", "DateModified", "AccessRights", "Branch", "loginStatus", "IsAllow" };
                List<string> skp_fields = new List<string>() { "UserNr" };

                id = PrepSQL.ErpInsertData("User", model, lgp_fields, skp_fields);
                response.status = 1;
                response.msg = "Success";
            }
            catch (Exception ex)
            {
                response.status = 0;
                response.msg = ex.Message;
            }
            return response;
        }

        public static ResponseModel UpdateUser(UserIfo model)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<string> lgp_fields = new List<string>() { "Code", "Description1", "Description2" };
                List<string> skp_fields = new List<string>() { "Number" };

                PrepSQL.ErpUpdateData("Units", model, model.UserNr, "UserNr", lgp_fields, skp_fields);
                response.status = 1;
                response.msg = "Success";
            }
            catch (Exception ex)
            {
                response.status = 0;
                response.msg = ex.Message;
            }
            return response;
        }
    }
}