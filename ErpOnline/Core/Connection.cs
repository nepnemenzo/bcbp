using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace bcbp_101.Core
{
    public class Connection
    {
        public static int TIMEOUT = 45;
        public static SqlTransaction Sqltrans;
        public static string connString = ConfigurationManager.ConnectionStrings["ERPconn"].ConnectionString;
        public static SqlConnection Sqlconn = new SqlConnection(connString);
        public static SqlCommand Sqlcomm = new SqlCommand();

        public static void OpenConnection()
        {
            Sqlconn.Open();
            Sqltrans = Sqlconn.BeginTransaction();
            Sqlcomm.Connection = Sqlconn;
            Sqlcomm.Transaction = Sqltrans;
        }

        public static int CommandConnectionInsert(string query)
        {

            Sqlcomm.CommandText = query;
            Sqlcomm.CommandType = CommandType.Text;
            Sqlcomm.CommandTimeout = TIMEOUT;
            Sqlcomm.ExecuteNonQuery();

            Sqlcomm.CommandText = "SELECT SCOPE_IDENTITY()";
            int id = Convert.ToInt32(Sqlcomm.ExecuteScalar());

            return id;
        }

        public static int CommandConnectionUpdate(string query)
        {
            Sqlcomm.CommandText = query;
            Sqlcomm.CommandType = CommandType.Text;
            Sqlcomm.CommandTimeout = TIMEOUT;
            int id = Sqlcomm.ExecuteNonQuery();
            return id;
        }


        public static DataSet CommandConnectionSearch(string query)
        {
            DataSet ds = new DataSet();

            Sqlcomm.CommandText = query;
            Sqlcomm.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter();
            Sqlcomm.CommandTimeout = TIMEOUT;
            da.SelectCommand = Sqlcomm;
            da.Fill(ds);

            return ds;
        }



        public static void CommitTransaction()
        {
            //Sqltrans.Commit();
        }

        public static void RollbackTransaction()
        {
            if (Sqltrans != null)
            {
                Sqltrans.Rollback();
            }
        }

        public static void CloseConnection()
        {
            if (Sqlconn.State.HasFlag(ConnectionState.Open))
            {
                Sqlconn.Close();
            }
        }


        public static int performSQLSelectCountOperation(string sqlQuery)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand selectCommand = new SqlCommand(sqlQuery, connection);
            connection.Open();
            int num = Convert.ToInt32(selectCommand.ExecuteScalar());
            connection.Close();
            return num;
        }

        public static int ExecuteScalar(string sqlQuery)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        int num = Convert.ToInt32(command.ExecuteScalar());
                        connection.Close();
                        return num;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet connectSQLServerQuery(string query)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    command.Connection.Open();
                    da.Fill(ds);
                    command.Connection.Close();

                    return ds;
                }
            }
        }


        public static DataTable SQLServerQuery(string query)
        {
            DataTable ds = new DataTable();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = command;
                    command.Connection.Open();
                    da.Fill(ds);
                    command.Connection.Close();

                    return ds;
                }
            }
        }
        public static int DeleteQuery(string query)
        {
            int resultOne = 0;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    using (SqlCommand command2 = connection.CreateCommand())
                    {
                        try
                        {
                            command.CommandText = query;
                            command.CommandType = CommandType.Text;

                            connection.Open();
                            resultOne = command.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch(Exception ex)
                        {
                            resultOne = 0;
                        }
 
                    };
                    return resultOne;
                }
            }

        }

        public static int connectSQLServerQueryOutId(string query)
        {
            int resultOne = 0;
            int resultTwo = 0;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    using (SqlCommand command2 = connection.CreateCommand())
                    {
                        string query2 = "SELECT SCOPE_IDENTITY()";

                        command.CommandText = query;
                        command.CommandType = CommandType.Text;

                        command2.CommandText = query2;
                        command2.CommandType = CommandType.Text;

                        connection.Open();
                        resultOne = command.ExecuteNonQuery();
                        resultTwo = Convert.ToInt32(command2.ExecuteScalar());
                        connection.Close();
                    };
                    return resultTwo;
                }
            }
        }


        public static float performSQLSelectSumOperation(string sqlQuery)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand selectCommand = new SqlCommand(sqlQuery, connection);
            connection.Open();
            int num = Convert.ToInt32(selectCommand.ExecuteScalar());
            connection.Close();
            return num;
        }

        public static int getRelation(string sqlQuery)
        {
            SqlConnection connection = new SqlConnection(connString);
            SqlCommand selectCommand = new SqlCommand(sqlQuery, connection);
            connection.Open();
            int num = Convert.ToInt32(selectCommand.ExecuteScalar());
            connection.Close();
            return num;
        }
    }
}