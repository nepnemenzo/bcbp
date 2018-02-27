using bcbp_101.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace bcbp_101.Core
{
    public class PrepSQL
    {
        private string _debugLastQuery;

        public List<KeyValuePair<String, Object>> ColumnValueList { get; set; }
        public string Query { get; set; }
        public string TempSql { get; set; }
        public string DebugLastQuery { get { return _debugLastQuery; } }

        //Insert -------------------------------------------------------------------------------------------------------------------------------------------------

        public static int ErpInsertData<T>(string tableName, T model, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            return Insert(tableName, model, true, fields, skipFields, true);
        }

        protected static int Insert<T>(string tableName, T model, bool addNullValues, List<string> fields = null, List<string> skipFields = null, bool skipId = true) where T : new()
        {
            if (skipId)
            {
                if (skipFields == null) skipFields = new List<string>();
                if (!skipFields.Contains(DatabaseModel.FieldId)) skipFields.Add(DatabaseModel.FieldId);
            }

            PrepSQL sql = new PrepSQL();
            sql.ConfigInsert(model, addNullValues, fields, skipFields);

            sql.Query = "INSERT INTO [dbo].[" + tableName + "] " + sql.TempSql;

            int id = sql.Insert(tableName);
            return id;
        }

        public void ConfigInsert<T>(T model, bool addNullValues, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            List<string> sqlFields = new List<string>();
            List<string> sqlValues = new List<string>();
            List<object> remove = new List<object>();

            ColumnValueList = GenerateColumnValueList(model, addNullValues, fields, skipFields);

            foreach (KeyValuePair<string, object> obj in ColumnValueList)
            {
                if (obj.Key == DatabaseModel.FieldCreatedDate || obj.Key == DatabaseModel.FieldModifiedDate)
                {
                    remove.Add(obj);
                }
                else
                {
                    sqlFields.Add("[" + obj.Key + "]");
                    sqlValues.Add("@" + obj.Key);
                }
            }

            //sqlFields.Add("[" + DatabaseModel.FieldCreatedDate + "]");
            //sqlValues.Add("[dbo].[Fn.TimeStamp.GetTimeStampDate]()");

            //sqlFields.Add("[" + DatabaseModel.FieldModifiedDate + "]");
            //sqlValues.Add("[dbo].[Fn.TimeStamp.GetTimeStampDate]()");

            // Eliminamos para no añadirlos posteriormente como SqlParameter
            foreach (KeyValuePair<string, object> obj in remove)
                ColumnValueList.Remove(obj);

            TempSql = "(" + String.Join(",", sqlFields);
            TempSql += ") VALUES (";
            TempSql += String.Join(",", sqlValues) + ")";
        }


        public int Insert(string tablename)
        {
            int id = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPconn"].ConnectionString))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {

                        if (Query.Contains("[id]"))
                        {
                            command.CommandText = Query + " SELECT ID FROM [DBO].[" + tablename + "] WHERE ID = @id";
                        }
                        else
                        {
                            command.CommandText = Query + " SELECT SCOPE_IDENTITY()";
                        }

                        command.CommandType = CommandType.Text;

                        foreach (var pair in ColumnValueList)
                        {
                            command.Parameters.Add(CreateSqlParameter("@" + pair.Key, pair.Value));
                        }

                        connection.Open();

                        _debugLastQuery = QueryCommandToString(command);

                        object result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            id = Convert.ToInt32(result);
                        }
                        else
                        {
                            id = 0;
                        }


                        connection.Close();

                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return 0;
        }

        //Update -------------------------------------------------------------------------------------------------------------------------------------------------

        public static bool ErpUpdateData<T>(string tableName, T model, int whereId, string idname, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            bool addNullValues = true;
            return Update(tableName, model, whereId, idname, addNullValues, fields, skipFields);
        }

        protected static bool Update<T>(string tableName, T model, int whereId, string idname, bool addNullValues, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            if (skipFields == null) skipFields = new List<string>();
            if (!skipFields.Contains(DatabaseModel.FieldId)) skipFields.Add(DatabaseModel.FieldId);

            PrepSQL sql = new PrepSQL();
            sql.ConfigUpdate(model, addNullValues, fields, skipFields);

            sql.Query = "UPDATE [dbo].[" + tableName + "] SET " + sql.TempSql + " WHERE " + idname + " = @" + idname;
            sql.ColumnValueList.Add(new KeyValuePair<String, Object>(idname, whereId));

            bool ok = sql.Update();
            return ok;
        }

        public void ConfigUpdate<T>(T model, bool addNullValues, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            List<string> sql = new List<string>();
            string sqlField = null;
            List<object> remove = new List<object>();

            ColumnValueList = GenerateColumnValueList(model, addNullValues, fields, skipFields);

            foreach (KeyValuePair<string, object> obj in ColumnValueList)
            {
                if (obj.Key == DatabaseModel.FieldCreatedDate || obj.Key == DatabaseModel.FieldModifiedDate)
                {
                    remove.Add(obj);
                }
                else
                {
                    sqlField = "[" + obj.Key + "] = @" + obj.Key;
                    sql.Add(sqlField);
                }
            }

            //sqlField = "[" + DatabaseModel.FieldModifiedDate + "] = [dbo].[Fn.TimeStamp.GetTimeStampDate]()";
            //sql.Add(sqlField);

            // Eliminamos para no añadirlos posteriormente como SqlParameter
            foreach (KeyValuePair<string, object> obj in remove)
                ColumnValueList.Remove(obj);

            TempSql = String.Join(",", sql);
        }

        public bool Update()
        {
            bool ok = true;
            DataSet result = null;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPconn"].ConnectionString))
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = Query;

                        foreach (var pair in ColumnValueList)
                        {
                            cmd.Parameters.Add(CreateSqlParameter("@" + pair.Key, pair.Value));
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            result = new DataSet();
                            da.Fill(result);

                            _debugLastQuery = QueryCommandToString(cmd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message;
                ok = false;
            }

            return ok;
        }

        public DataSet Select()
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPconn"].ConnectionString))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = Query;
                        command.CommandType = CommandType.Text;

                        foreach (var pair in ColumnValueList)
                        {
                            command.Parameters.Add(CreateSqlParameter("@" + pair.Key, pair.Value));
                        }

                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = command;

                        command.Connection.Open();

                        da.Fill(ds);

                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                ds = null;
            }
            return ds;
        }

        // Utilizado en el debug
        public static string QueryCommandToString(SqlCommand cmd)
        {
            string query = cmd.CommandText;
            string value;

            foreach (SqlParameter p in cmd.Parameters)
            {
                if (p.Value == DBNull.Value) value = "null";
                else if (IsTypeString(p.SqlDbType)) value = "'" + p.Value.ToString() + "'";
                else value = p.Value.ToString();

                query = query.Replace(p.ParameterName, value);
            }

            return query;
        }

        public static List<string> GetModelFieldsUsingHttpRequest<T>(T model, List<string> fields = null) where T : new()
        {
            if (fields == null || fields.Count == 0)
            {
                fields = UtilHelper.GetClassProperties(model);
            }

            List<string> requestFields = GetRequestFields();
            List<string> newFields = new List<string>();

            foreach (string fld in requestFields)
            {
                if (fields.Contains(fld)) newFields.Add(fld);
            }

            return newFields;
        }

        public static List<string> GetRequestFields()
        {
            List<string> fields = new List<string>();

            try
            {
                var form = System.Web.HttpContext.Current.Request.Form;

                if (form != null)
                {
                    foreach (string key in form.AllKeys)
                    {
                        fields.Add(key);
                        //var value = form[key];
                    }
                }
            }
            catch (Exception)
            {
                fields = new List<string>();
            }

            return fields;
        }

        //Generate Culumn Values -------------------------------------------------------------------------------------------------------------------------------------------------

        public static List<KeyValuePair<String, Object>> GenerateColumnValueList<T>(T model, bool addNullValues, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            List<KeyValuePair<String, Object>> list = new List<KeyValuePair<String, Object>>();

            List<string> sql = new List<string>();
            Type type = model.GetType();
            Object sqlField = null;
            string propertyName = null;

            if (fields == null || fields.Count == 0) fields = UtilHelper.GetClassProperties(model);
            if (skipFields == null) skipFields = new List<string>();

            foreach (string field in fields)
            {
                if (skipFields.Contains(field))
                {
                    continue;
                }

                try
                {
                    var property = type.GetProperty(field);
                    var value = property.GetValue(model, null);

                    if (value == null && !addNullValues)
                    {
                        continue;
                    }

                    propertyName = property.PropertyType.Name.ToLower();
                    sqlField = null;

                    if (propertyName.Contains("nullable")) // Nullable
                    {
                        if (value == null)
                        {
                            sqlField = value;
                        }
                        else
                        {
                            propertyName = value.GetType().Name.ToLower();

                            if (propertyName.Contains("bool")) // boolean to int
                            {
                                sqlField = BoolToInt((bool)value);
                            }
                            else
                            {
                                sqlField = value;
                            }
                        }
                    }
                    else
                    {
                        if (propertyName.Contains("bool")) // boolean to int
                        {
                            sqlField = (value == null) ? 0 : BoolToInt((bool)value);
                        }
                        else
                        {
                            sqlField = value;
                        }
                    }

                    list.Add(new KeyValuePair<String, Object>(property.Name, sqlField));
                }
                catch (Exception e)
                {
                    string message = e.Message;
                }
            }

            return list;
        }

        public static int BoolToInt(bool value)
        {
            return (value == true) ? 1 : 0;
        }

        public static SqlParameter CreateSqlParameter(string parameterName, object value)
        {
            if (value == null)
            {
                return new SqlParameter(parameterName, DBNull.Value);
            }
            else
            {
                return new SqlParameter(parameterName, value);
            }
        }

        /*
            http://stackoverflow.com/questions/1594711/how-to-retrieve-net-type-of-given-storedprocedures-parameter-in-sql
         
            string valueToSet = "1234";
            SqlParameter p = new SqlParameter();
            p.SqlDbType = System.Data.SqlDbType.Int;
            p.Value = TypeMapper[p.SqlDbType](valueToSet);
        */

        public static bool IsTypeString(SqlDbType type)
        {
            if (type == SqlDbType.Char || type == SqlDbType.NChar || type == SqlDbType.NText
                || type == SqlDbType.NVarChar || type == SqlDbType.Text || type == SqlDbType.VarChar)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Dictionary<SqlDbType, Type> TypeMap = new Dictionary<SqlDbType, Type>
        {
            { SqlDbType.BigInt, typeof(Int64) },
            { SqlDbType.Binary, typeof(Byte[]) },
            { SqlDbType.Bit, typeof(Boolean) },
            { SqlDbType.Char, typeof(String) },
            { SqlDbType.Date, typeof(DateTime) },
            { SqlDbType.DateTime, typeof(DateTime) },
            { SqlDbType.DateTime2, typeof(DateTime) },
            { SqlDbType.DateTimeOffset, typeof(DateTimeOffset) },
            { SqlDbType.Decimal, typeof(Decimal) },
            { SqlDbType.Float, typeof(Double) },
            { SqlDbType.Int, typeof(Int32) },
            { SqlDbType.Money, typeof(Decimal) },
            { SqlDbType.NChar, typeof(String) },
            { SqlDbType.NText, typeof(String) },
            { SqlDbType.NVarChar, typeof(String) },
            { SqlDbType.Real, typeof(Single) },
            { SqlDbType.SmallInt, typeof(Int16) },
            { SqlDbType.SmallMoney, typeof(Decimal) },
            { SqlDbType.Structured, typeof(Object) }, // might not be best mapping...
            { SqlDbType.Text, typeof(String) },
            { SqlDbType.Time, typeof(TimeSpan) },
            { SqlDbType.Timestamp, typeof(Byte[]) },
            { SqlDbType.TinyInt, typeof(Byte) },
            { SqlDbType.Udt, typeof(Object) },  // might not be best mapping...
            { SqlDbType.UniqueIdentifier, typeof(Guid) },
            { SqlDbType.VarBinary, typeof(Byte[]) },
            { SqlDbType.VarChar, typeof(String) },
            { SqlDbType.Variant, typeof(Object) },
            { SqlDbType.Xml, typeof(SqlXml) },
        };

        public static Dictionary<SqlDbType, Func<string, object>> TypeMapper = new Dictionary<SqlDbType, Func<string, object>>
        {
            { SqlDbType.BigInt, s => Int64.Parse(s)},
            { SqlDbType.Binary, s => null },  // TODO: what parser?
            { SqlDbType.Bit, s => Boolean.Parse(s) },
            { SqlDbType.Char, s => s },
            { SqlDbType.Date, s => DateTime.Parse(s) },
            { SqlDbType.DateTime, s => DateTime.Parse(s) },
            { SqlDbType.DateTime2, s => DateTime.Parse(s) },
            { SqlDbType.DateTimeOffset, s => DateTimeOffset.Parse(s) },
            { SqlDbType.Decimal, s => Decimal.Parse(s) },
            { SqlDbType.Float, s => Double.Parse(s) },
            { SqlDbType.Int, s => Int32.Parse(s) },
            { SqlDbType.Money, s => Decimal.Parse(s) },
            { SqlDbType.NChar, s => s },
            { SqlDbType.NText, s => s },
            { SqlDbType.NVarChar, s => s },
            { SqlDbType.Real, s => Single.Parse(s) },
            { SqlDbType.SmallInt, s => Int16.Parse(s) },
            { SqlDbType.SmallMoney, s => Decimal.Parse(s) },
            { SqlDbType.Structured, s => null }, // TODO: what parser?
            { SqlDbType.Text, s => s },
            { SqlDbType.Time, s => TimeSpan.Parse(s) },
            { SqlDbType.Timestamp, s => null },  // TODO: what parser?
            { SqlDbType.TinyInt, s => Byte.Parse(s) },
            { SqlDbType.Udt, s => null },  // consider exception instead
            { SqlDbType.UniqueIdentifier, s => new Guid(s) },
            { SqlDbType.VarBinary, s => null },  // TODO: what parser?
            { SqlDbType.VarChar, s => s },
            { SqlDbType.Variant, s => null }, // TODO: what parser?
            { SqlDbType.Xml, s => s },
        };
    }
}