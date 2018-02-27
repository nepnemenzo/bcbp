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
    public class ParticipantData
    {
        public static SqlConnection _con;
        public static void connection()
        {
            _con = new SqlConnection(Connection.connString);
        }

        public static List<ParticipantModel> ParticipantList(int? id)
        {
            string query = string.Empty;
            List<ParticipantModel> members = new List<ParticipantModel>();


            query = "SELECT prt.id, prt.memberId, prt.GroupId, prt.CLPid, mem.LastName + ', ' + mem.FirstName as name, grp.Name as GroupName, sp.LastName + ', ' + sp.FirstName as SponsorName, st.Name as status " +
                    "FROM [dbo].[CLPParticipant] prt " +
                    "INNER JOIN [dbo].[Member] mem ON prt.memberId = mem.id " +
                    "INNER JOIN [dbo].[CLPGroupings] grp ON prt.GroupId = grp.id " +
                    "INNER JOIN[dbo].[SponsorList] sp ON mem.SponsorId = sp.id " +
                    "INNER JOIN[dbo].[StatusGeneric] st ON prt.statusID = st.id";

            DataSet ds = Connection.connectSQLServerQuery(query);
            members = BaseReflection.DatasetBDToObjectList<ParticipantModel>(ds);
            ParticipantModel model = (members != null && members.Count > 0) ? members[0] : null;
            return members;
        }


        public ResponseModel SaveParticipant(ParticipantModel modl)
        {
            ResponseModel resp = new ResponseModel();
            string sqlQuery = string.Empty;
            try
            {
                using (SqlConnection db = new SqlConnection(Connection.connString))
                {

                    sqlQuery = "Insert Into [CLPParticipant] ([memberId],[GroupId],[CLPid], [DateEntered],[EnteredBy],[ModifiedDate],[ModifiedBy],[statusID])" +
                                                           "Values(@memberId, @GroupId, @CLPid, @DateEntered, @EnteredBy, @ModifiedDate, @ModifiedBy, @statusID)";


                    var x = db.Execute(sqlQuery, new
                    {
                        memberId = modl.memberId,
                        GroupId = modl.GroupId,
                        CLPid = modl.CLPid,
                        DateEntered = modl.DateEntered,
                        EnteredBy = modl.EnteredBy,
                        ModifiedDate = modl.ModifiedDate,
                        ModifiedBy = modl.ModifiedBy,
                        statusID = modl.statusID
                        
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



        public static List<GenericList> GetMemberParticipantList()
        {
            string CLPongoing = System.Configuration.ConfigurationManager.AppSettings["CLPongoing"];
            string CLPupcoming = System.Configuration.ConfigurationManager.AppSettings["CLPupcoming"];
            int CLPparticipantID = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CLPparticipantID"]);

            List<GenericList> MemberParticipantList = new List<GenericList>();
            string query = "Select mem.id,  mem.LastName + ', ' + mem.FirstName as name " +
                       "from[dbo].[Member] mem " +
                        "INNER JOIN (SELECT * FROM[dbo].[CLP] clp INNER JOIN[dbo].[CLPStatus] st ON clp.clpStatus = st.id WHERE st.Name LIKE '%" + CLPongoing + "%' OR st.Name LIKE '%"+ CLPupcoming  + "%') clp ON mem.CLPid = clp.CLPId " +
                        "WHERE mem.Position = " + CLPparticipantID + " AND mem.id NOT IN (SELECT memberId FROM [dbo].[CLPParticipant])";

            DataSet ds = Connection.connectSQLServerQuery(query);
            MemberParticipantList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);
            return MemberParticipantList;
        }


        public ParticipantModel GetParticipant(int id)
        {
            List<ParticipantModel> partLst = new List<ParticipantModel>();
            ParticipantModel part = new ParticipantModel();

            //string query = "Select * from [dbo].[CLPParticipant] WHERE  Memberid = " + id;
            string query = "Select prt.id, prt.memberId, prt.CLPid, prt.GroupId, prt.statusID, mem.LastName + ', ' + mem.FirstName as name from [dbo].[CLPParticipant] prt " +
                            "Inner Join[dbo].[Member] mem ON prt.memberId = mem.id WHERE  Memberid = " + id;

            DataSet ds = Connection.connectSQLServerQuery(query);
            partLst = BaseReflection.DatasetBDToObjectList<ParticipantModel>(ds);
            if(partLst.Count > 0) { 
                part = new ParticipantModel
                {
                    id = partLst[0].id,
                    memberId = partLst[0].memberId,
                    name = partLst[0].name,
                    GroupId = partLst[0].GroupId,
                    CLPid = partLst[0].CLPid,
                    statusID = partLst[0].statusID
                };
            }
            //part.MemberList = ParticipantData.GetMemberParticipantListModify();
            //GenericList model = (GroupList != null && GroupList.Count > 0) ? GroupList[0] : null;
            return part;

        }


        //public static List<GenericList> GetMemberParticipantListModify()
        //{
        //    List<GenericList> MemberParticipantList = new List<GenericList>();
        //    string query = "Select mem.id,  mem.LastName + ', ' + mem.FirstName as name " +
        //               "from[dbo].[Member] mem " +
        //                "INNER JOIN (SELECT * FROM[dbo].[CLP] clp INNER JOIN[dbo].[CLPStatus] st ON clp.clpStatus = st.id WHERE st.Name LIKE '%On Going%' OR st.Name LIKE '%to be%') clp ON mem.CLPid = clp.CLPId " +
        //                "WHERE mem.Position = 5 AND mem.id NOT IN (SELECT memberId FROM [dbo].[CLPParticipant])";

        //    query = "";

        //    DataSet ds = Connection.connectSQLServerQuery(query);
        //    MemberParticipantList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);
        //    return MemberParticipantList;
        //}



        public static List<GenericList> GetGroupList()
        {
            List<GenericList> GroupList = new List<GenericList>();
            string query = "Select id, name from [dbo].[CLPGroupings] order by id";

            DataSet ds = Connection.connectSQLServerQuery(query);
            GroupList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);

            //GenericList model = (GroupList != null && GroupList.Count > 0) ? GroupList[0] : null;
            return GroupList;

        }


        public static List<GenericList> GetStatusList()
        {
            List<GenericList> StatusList = new List<GenericList>();
            string sql = "Select id, name from [statusGeneric] order by id";

            DataSet ds = Connection.connectSQLServerQuery(sql);
            StatusList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);

            GenericList model = (StatusList != null && StatusList.Count > 0) ? StatusList[0] : null;
            return StatusList;
        }



        //*****-------------------------------------------------
    }

}