using bcbp_101.Core;
using bcbp_101.Models;
using bcbp_101.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace bcbp_101.DataModel
{
    public class DashboardData
    {
        public static List<DashboardModel> LisstOfBranches()
        {
            string query = "SELECT branchID, name FROM [dbo].[Branch]";
            DataSet ds = Connection.connectSQLServerQuery(query);
            return BaseReflection.DatasetBDToObjectList<DashboardModel>(ds);
        }

        public static List<StockResponse> LisstOfStockIn(int itemNum)
        {
            string query = "SELECT TOP 5 b.name AS itemname,SUM(a.TranInwards) as qty  FROM [dbo].[Transactions] a " +
                            " JOIN [dbo].[Products] b ON b.id = a.TranItemNumber " +
                            " WHERE b.branch = " + itemNum + " AND a.TranMode = 1  GROUP BY b.name,b.sysModified ORDER BY qty DESC";
            DataSet ds = Connection.connectSQLServerQuery(query);
            return BaseReflection.DatasetBDToObjectList<StockResponse>(ds);
        }

        public static List<StockResponse> LisstOfStockOut(int itemNum)
        {
            string query = "SELECT TOP 5 b.name AS itemname,SUM(a.TranOutwards) as qty  FROM [dbo].[Transactions] a " +
                            " JOIN [dbo].[Products] b ON b.id = a.TranItemNumber " +
                            " WHERE b.branch = " + itemNum + " AND a.TranMode = 2 GROUP BY b.name,b.sysModified ORDER BY qty DESC";
            DataSet ds = Connection.connectSQLServerQuery(query);
            return BaseReflection.DatasetBDToObjectList<StockResponse>(ds);
        }
    }
}