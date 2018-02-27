using bcbp_101.Models;
using bcbp_101.Reflection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace bcbp_101.DataModel
{
    public class AccountData
    {
        public static UserResponse AccountAuth(string username, string password)
        {
            List<UserIfo> UserList = new List<UserIfo>();
            UserResponse model = new UserResponse();
            try
            {
                DataSet ds = new DataSet();
                string sql = "SELECT * FROM [dbo].[User] WHERE UserName = @uname AND UserPaswr = @passwrd";
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPconn"].ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@uname", username);
                        command.Parameters.AddWithValue("@passwrd", password);
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;
                        command.Connection.Open();
                        da.Fill(ds);
                        command.Connection.Close();
                    }
                }
                UserList = BaseReflection.DatasetBDToObjectList<UserIfo>(ds);
                model.Userdata = (UserList != null && UserList.Count > 0) ? UserList[0] : null;

                if (UserList.Count != 0)
                {
                    model.status = 1;
                    model.msg = "Success";
                }
                else
                {
                    model.status = 2;
                    model.msg = "Invalid username/password";
                }
            }
            catch (Exception ex)
            {
                model.status = 0;
                model.msg = "Failed : " + ex.Message;
            }

            return model;
        }

        public static void SetCookie(bool isPersistent, UserIfo usrdata)
        {
            DateTime dateExpire = isPersistent == true ? DateTime.Now.AddDays(10) : DateTime.Now.AddMinutes(30);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string userData = serializer.Serialize(usrdata);
            
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1,
                usrdata.UserName,
                DateTime.Now,
                dateExpire,
                isPersistent,
                userData, FormsAuthentication.FormsCookiePath);


            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie userCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            userCookie.Path = FormsAuthentication.FormsCookiePath;
            FormsAuthentication.SetAuthCookie(usrdata.UserName, isPersistent);
            userCookie.Expires = dateExpire;
            HttpContext.Current.Response.Cookies.Add(userCookie);
        }

    }
}