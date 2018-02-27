using bcbp_101.Core;
using bcbp_101.Models.bcbp;
using bcbp_101.Reflection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace bcbp_101.DataModel.bcbp
{
    public class ServantData
    {
        public static SqlConnection _con;
        public static void connection()
        {
            _con = new SqlConnection(Connection.connString);
        }

        public static List<ServantModel> GetServantsList(int? id)
        {
            string query = string.Empty;
            List<ServantModel> servant = new List<ServantModel>();


            query = " SELECT ser.id, ser.memberId, mem.LastName + ', ' + mem.FirstName as name, vc.ServantTask as ServiceName " + 
                    "FROM[dbo].[CLPServants] ser " +
                    "INNER JOIN[dbo].[Member] mem ON ser.memberId = mem.id " +
                    "INNER JOIN[dbo].[CLPService] vc ON ser.ServiceId = vc.id";

            DataSet ds = Connection.connectSQLServerQuery(query);
            servant = BaseReflection.DatasetBDToObjectList<ServantModel>(ds);
            ServantModel model = (servant != null && servant.Count > 0) ? servant[0] : null;
            return servant;
        }





        public ResponseModel SaveServant(ServantModel modl)
        {
            ResponseModel resp = new ResponseModel();
            string sqlQuery = string.Empty;
            try
            {
                using (SqlConnection db = new SqlConnection(Connection.connString))
                {

                    sqlQuery = "Insert Into [dbo].[CLPServants] ([memberId],[ServiceId],[CLPid], [DateEntered],[EnteredBy],[ModifiedDate],[ModifiedBy])" +
                                                           "Values(@memberId, @ServiceId, @CLPid, @DateEntered, @EnteredBy, @ModifiedDate, @ModifiedBy)";

                    var x = db.Execute(sqlQuery, new
                    {
                        memberId = modl.memberId,
                        ServiceId = modl.ServiceId,
                        CLPid = modl.CLPId,
                        DateEntered = modl.DateEntered,
                        EnteredBy = modl.EnteredBy,
                        ModifiedDate = modl.ModifiedDate,
                        ModifiedBy = modl.ModifiedBy

                    });
                }
                resp.status = 1;
                resp.msg = "Item inserted - Success";
            }
            catch (Exception ex)
            {
                resp.status = 0;
                resp.msg = "Item inserted - Failed : " + ex.Message;
            }

            return resp;

        }



        public ServantModel GetServantDetails(int id)
        { 
            List<ServantModel> partLst = new List<ServantModel>();
            ServantModel part = new ServantModel();

            //string query = "Select * from [dbo].[CLPParticipant] WHERE  Memberid = " + id;
            string query = "Select prt.id, prt.memberId, prt.CLPid as CLPId, prt.ServiceId as ServiceId, mem.LastName + ', ' + mem.FirstName as name from [dbo].[CLPServants] prt " +
                            "Inner Join[dbo].[Member] mem ON prt.memberId = mem.id WHERE  prt.memberId = " + id;

            DataSet ds = Connection.connectSQLServerQuery(query);
            partLst = BaseReflection.DatasetBDToObjectList<ServantModel>(ds);
            if (partLst.Count > 0)
            {
                part = new ServantModel
                {
                    id = partLst[0].id,
                    memberId = partLst[0].memberId,
                    name = partLst[0].name,
                    CLPId = partLst[0].CLPId,
                    ServiceId = partLst[0].ServiceId
                };
            }
            //part.MemberList = ParticipantData.GetMemberParticipantListModify();
            //GenericList model = (GroupList != null && GroupList.Count > 0) ? GroupList[0] : null;
            return part;

        }




        public static List<GenericList> GetMemberList()
        {
            List<GenericList> MemberList = new List<GenericList>();
            string query = "Select mem.id,  mem.LastName + ', ' + mem.FirstName as name " +
                       "from[dbo].[Member] mem ";

            DataSet ds = Connection.connectSQLServerQuery(query);
            MemberList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);
            return MemberList;
        }

        public static List<GenericList> GetServiceList()
        {
            List<GenericList> ServiceList = new List<GenericList>();
            string query = "Select id,  ServantTask as name " +
                       "from [dbo].[CLPService] ORDER BY ServantTask ";

            DataSet ds = Connection.connectSQLServerQuery(query);
            ServiceList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);
            return ServiceList;
        }

        public static List<GenericList> GetCLPList()
        {
            List<GenericList> CLPList = new List<GenericList>();
            string query = "Select CLPId as id,  clpName as name " +
                       "from [dbo].[CLP] ORDER BY clpName ";

            DataSet ds = Connection.connectSQLServerQuery(query);
            CLPList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);
            return CLPList;
        }

    }
}