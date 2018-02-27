using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using bcbp_101.Models.bcbp;
using bcbp_101.Core;
using bcbp_101.Reflection;

namespace bcbp_101.DataModel.bcbp
{
    public class MembersData
    {
        public static SqlConnection _con;

        public static void connection()
        {
            _con = new SqlConnection(Connection.connString);
        }

        public static List<MemberModel> MemberList(int? id)
        {
            string query = string.Empty;
            List<MemberModel> members = new List<MemberModel>();
     
            query = " Select mem.id, mem.LastName + ', ' + mem.FirstName as memName, mem.address, mem.status, pos.name as PositionName, st.Name as statusName, sp.LastName + ', ' + sp.FirstName as SponsorName  " + 
                    "from[dbo].[Member] mem " +
                                "inner join[dbo].[Position] pos on mem.Position = pos.id " +
                                "inner join[dbo].[StatusGeneric] st on mem.status = st.id " +
                                "left join[dbo].[SponsorList] sp on mem.SponsorId = sp.id";

            DataSet ds = Connection.connectSQLServerQuery(query);
            members = BaseReflection.DatasetBDToObjectList<MemberModel>(ds);
            MemberModel model = (members != null && members.Count > 0) ? members[0] : null;
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
            string sqlQuery = string.Empty;
            try
            {
                using (SqlConnection db = new SqlConnection(Connection.connString))
                {

                    sqlQuery = "Insert Into [Member] ([FirstName],[MidName],[LastName], [EmailAdd],[Address],[City],[Province],[AreaCode]" +
                                                           ",[contactNum],[bcbpGroup],[Position],[status], [YearOfMembership]" +
                                                           ",[CLPid], [Chapter],[AccessType], [SponsorId]) " +
                                                           "Values(@FirstName, @MidName, @LastName, @EmailAdd, @Address, @City, @Province, @AreaCode, " +
                                                                  "@contactNum, @bcbpGroup, @Position, @status, @YearOfMembership, @CLPNum, @Chapter, @AccessType, @SponsorId)";


                    var x = db.Execute(sqlQuery, new
                    {
                        FirstName = modl.FirstName,
                        MidName = modl.MidName,
                        LastName = modl.LastName,
                        EmailAdd = modl.EmailAdd,
                        Address = modl.address,
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
                        AccessType = modl.AccessType,
                        SponsorId = modl.SponsorId
                    });
                }
                resp.status = 1;
                resp.msg = "Item inserted - Success";
                SaveSponsor(modl);
            }
            catch (Exception ex)
            {
                resp.status = 0;
                resp.msg = "Item inserted - Failed : " + ex.Message;
            }

            return resp;
          
        }



        public ResponseModel UpdateMember(MemberModel modl, int id)
        {
            ResponseModel resp = new ResponseModel();
            string sqlQuery = string.Empty;
            try
            {
                using (SqlConnection db = new SqlConnection(Connection.connString))
                {
                   
                        sqlQuery = "Update [dbo].[Member] Set [FirstName]=@FirstName, [MidName] = @MidName, [LastName] = @LastName,[EmailAdd] =  @EmailAdd, [Address] = @Address, [City] = @City," +
                                                            "[Province] = @Province,[AreaCode] = @AreaCode, [contactNum] = @contactNum ,[bcbpGroup] = @bcbpGroup,[Position] = @Position, " +
                                                            "[status] = @status, [SponsorId] = @SponsorId, [YearOfMembership] = @YearOfMembership, [CLPid] =@CLPNum, [Chapter] = @Chapter, [AccessType] = @AccessType " +
                                                            "WHERE id = @id";

                    var x = db.Execute(sqlQuery, new
                    {
                        id =modl.id,
                        FirstName = modl.FirstName,
                        MidName = modl.MidName,
                        LastName = modl.LastName,
                        EmailAdd = modl.EmailAdd,
                        Address = modl.address,
                        City = modl.City,
                        Province = modl.Province,
                        AreaCode = modl.AreaCode,
                        contactNum = modl.contactNum,
                        bcbpGroup = modl.bcbpGroup,
                        Position = modl.Position,
                        status = modl.status,
                        SponsorId = modl.SponsorId,
                        YearOfMembership = modl.YearOfMembership,
                        CLPNum = modl.CLPNum,
                        Chapter = modl.Chapter,
                        AccessType = modl.AccessType
                    });
                }
                resp.status = 1;
                resp.msg = "Item updated - Success";
                SaveSponsor(modl);
            }
            catch (Exception ex)
            {
                resp.status = 0;
                resp.msg = "Item updated - Failed : " + ex.Message;
            }

            return resp;

        }



        public ResponseModel SaveSponsor(MemberModel modl)
        {
            ResponseModel ans = new ResponseModel();
            string sqlQuery = string.Empty;
            try
            {
                if (!CheckSponsorIfExist(modl.SponsorId))
                {
                    if (modl.SponsorId != 0)
                    {
                        string query = string.Empty;
                        List<MemberModel> member = new List<MemberModel>();
                        query = "SELECT * FROM [Member] WHERE id = " + modl.SponsorId + "";

                        DataSet ds = Connection.connectSQLServerQuery(query);
                        member = BaseReflection.DatasetBDToObjectList<MemberModel>(ds);


                        using (SqlConnection db = new SqlConnection(Connection.connString))
                        {

                            sqlQuery = "Insert Into [SponsorList] ([id], [FirstName], [LastName]) " +
                                                                   "Values(@id, @FirstName, @LastName)";


                            var x = db.Execute(sqlQuery, new
                            {
                                id = member[0].id,
                                FirstName = member[0].FirstName,
                                LastName = member[0].LastName
                            });
                        }
                        ans.status = 1;
                    }
                    ans.status = 0;

                }
                else
                    ans.status = 0;


            }
            catch (Exception ex)
            {
                ans.status = 0;
            }

            return ans;

        }

        private bool CheckSponsorIfExist(int id)
        {
            string query = string.Empty;
            List<MemberModel> memlist = new List<MemberModel>();
            if (id != 0)
                query = "SELECT * FROM [SponsorList] WHERE id = " + id + "";

            DataSet ds = Connection.connectSQLServerQuery(query);
            memlist = BaseReflection.DatasetBDToObjectList<MemberModel>(ds);
            return memlist.Count > 0;

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

        public List<GenericList> GetSponsorList()
        {
            List<GenericList> sponsors = new List<GenericList>();

            string query = "Select id, LastName + ', ' + FirstName as name FROM [dbo].[Member] ORDER BY LastName";

            DataSet ds = Connection.connectSQLServerQuery(query);
            sponsors = BaseReflection.DatasetBDToObjectList<GenericList>(ds);
            GenericList model = (sponsors != null && sponsors.Count > 0) ? sponsors[0] : null;

            return sponsors;
        }

    }
}