using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using bcbp_101.Core;
using bcbp_101.Models.bcbp;
using bcbp_101.Reflection;

namespace bcbp_101.DataModel.bcbp
{
    public class CLPAttendanceData
    {
        public static SqlConnection _con;

        public static void connection()
        {
            _con = new SqlConnection(Connection.connString);
        }

        public static List<CLPAttendanceModel> AttendanceList(int? id)
        {

            string query = string.Empty;
            List<CLPAttendanceModel> members = new List<CLPAttendanceModel>();

            query = " Select mem.id, mem.LastName + ', ' + mem.FirstName as memName, mem.status, pos.name as PositionName, st.Name as statusName from[dbo].[Member] mem " +
                                "inner join[dbo].[Position] pos on mem.Position = pos.id " +
                                "inner join[dbo].[StatusGeneric] st on mem.status = st.id";

            DataSet ds = Connection.connectSQLServerQuery(query);
            members = Reflection.BaseReflection.DatasetBDToObjectList<CLPAttendanceModel>(ds);
            CLPAttendanceModel model = (members != null && members.Count > 0) ? members[0] : null;
            return members;
        }






        public static MemberModel MemberDetails(int id)
        {
            string query = string.Empty;
            List<MemberModel> memlist = new List<MemberModel>();
            if (id != 0)
                query = "SELECT * FROM [Member] WHERE id = " + id + "";
            else
                query = "SELECT * FROM [Member] Order by [LastName]";

            DataSet ds = Connection.connectSQLServerQuery(query);
            memlist = BaseReflection.DatasetBDToObjectList<MemberModel>(ds);

            MemberModel model = (memlist != null && memlist.Count > 0) ? memlist[0] : null;
            return model;
        }


        public ResponseModel SaveMember(MemberModel modl)
        {
            ResponseModel resp = new ResponseModel();
            try
            {
                using (SqlConnection db = new SqlConnection(Connection.connString))
                {
                    string sqlQuery = "Insert Into [Member] ([FirstName],[MidName],[LastName],[EmailAdd],[City],[Province],[AreaCode]" +
                                                            ",[contactNum],[bcbpGroup],[Position],[status],[YearOfMembership]" +
                                                            ",[CLPNum], [Chapter],[AccessType]) " +
                                                            "Values(@FirstName, @MidName, @LastName, @EmailAdd, @City, @Province, @AreaCode, " +
                                                                   "@contactNum, @bcbpGroup, @Position, @status, @YearOfMembership, @CLPNum, @Chapter, @AccessType)";

                    var x = db.Execute(sqlQuery, new
                    {
                        FirstName = modl.FirstName,
                        MidName = modl.MidName,
                        LastName = modl.LastName,
                        EmailAdd = modl.EmailAdd,
                        City = modl.City,
                        Province = modl.Province,
                        AreaCode = modl.AreaCode,
                        contactNum = modl.contactNum,
                        bcbpGroup = modl.bcbpGroup,
                        Position = modl.Position,
                        status = modl.status,
                        YearOfMembership = modl.YearOfMembership,
                        CLPNum = modl.CLPNum,
                        Chapter = modl.Chapter,
                        AccessType = modl.AccessType
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








        public List<GenericList> GetPositionList()
        {
            List<GenericList> PositionList = new List<GenericList>();
            string query = "Select id, name from [Position] order by id";

            DataSet ds = Connection.connectSQLServerQuery(query);
            PositionList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);

            GenericList model = (PositionList != null && PositionList.Count > 0) ? PositionList[0] : null;
            return PositionList;

        }

        public List<GenericList> GetChapterList()
        {
            List<GenericList> memlist = new List<GenericList>();
            string sql = "Select id, name from [ChapterList] order by name";

            DataSet ds = Connection.connectSQLServerQuery(sql);
            memlist = BaseReflection.DatasetBDToObjectList<GenericList>(ds);

            GenericList model = (memlist != null && memlist.Count > 0) ? memlist[0] : null;
            return memlist;
        }

        public List<GenericList> GetAcccessTypeList()
        {

            List<GenericList> AcccessTypeList = new List<GenericList>();
            string sql = "Select id, name from [AccessType] order by id desc";

            DataSet ds = Connection.connectSQLServerQuery(sql);
            AcccessTypeList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);

            GenericList model = (AcccessTypeList != null && AcccessTypeList.Count > 0) ? AcccessTypeList[0] : null;
            return AcccessTypeList;
        }


        public List<GenericList> GetStatusList()
        {
            List<GenericList> StatusList = new List<GenericList>();
            string sql = "Select id, name from [statusGeneric] order by id";

            DataSet ds = Connection.connectSQLServerQuery(sql);
            StatusList = BaseReflection.DatasetBDToObjectList<GenericList>(ds);

            GenericList model = (StatusList != null && StatusList.Count > 0) ? StatusList[0] : null;
            return StatusList;
        }



    }
}