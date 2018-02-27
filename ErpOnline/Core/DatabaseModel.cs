using bcbp_101.Helpers;
using bcbp_101.Reflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace bcbp_101.Core
{
    public class DatabaseModel
    {
        public static string FieldId = "id";
        public static string FieldCreatedDate = "createdDate";
        public static string FieldModifiedDate = "modifiedDate";

        public DatabaseModel()
        {
        }

        public static DatabaseStatusModel GetStatus(DataSet ds)
        {
            List<DatabaseStatusModel> dataList = BaseReflection.DatasetBDToObjectList<DatabaseStatusModel>(ds);
            DatabaseStatusModel model = (dataList != null && dataList.Count > 0) ? dataList[0] : new DatabaseStatusModel();
            return model;
        }

        public class DatabaseStatusModel
        {
            public string code { get; set; }
            public string message { get; set; }

            public DatabaseStatusModel()
            {
                code = String.Empty;
                message = String.Empty;
            }

            public bool IsOK()
            {
                return (String.IsNullOrEmpty(code) || !code.Equals("0")) ? true : false;
            }
        }

        public class MethodStatus
        {
            public bool ok { get; set; }
            public string message { get; set; }
            public int newId { get; set; }

            public MethodStatus()
            {
                ok = true;
                message = String.Empty;
            }
        }

        public static string PrepToSQL(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                value = "''";
            }
            else
            {
                value = value.Replace("'", "\"");
                value = "'" + value + "'";
            }
            return value;
        }

        public static string PrepToSQL(bool value)
        {
            return (value == true) ? "1" : "0";
        }

        public static int BoolToInt(bool value)
        {
            return (value == true) ? 1 : 0;
        }

        public static string SqlInsert<T>(T model, string table, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            List<KeyValuePair<string, string>> list = SqlFields(model, fields, skipFields);
            string sql = String.Empty;
            List<string> sqlFields = new List<string>();
            List<string> sqlValues = new List<string>();
            string temp = null;

            if (list.Count == 0) return sql;

            foreach (KeyValuePair<string, string> obj in list)
            {
                sqlFields.Add("[" + obj.Key + "]");
                sqlValues.Add(obj.Value);
            }

            if (fields == null || fields.Count == 0)
            {
                fields = UtilHelper.GetClassProperties(model);
            }

            temp = "createdDate";
            if (fields.Contains(temp) && !sqlFields.Contains(temp))
            {
                sqlFields.Add("[" + temp + "]");
                sqlValues.Add("[dbo].[Fn.TimeStamp.GetTimeStampDate]()");
            }

            temp = "modifiedDate";
            if (fields.Contains(temp) && !sqlFields.Contains(temp))
            {
                sqlFields.Add("[" + temp + "]");
                sqlValues.Add("[dbo].[Fn.TimeStamp.GetTimeStampDate]()");
            }

            sql = @" Declare @id int INSERT INTO [dbo].[" + table + "](";
            sql += String.Join(",", sqlFields);
            sql += ") VALUES (";
            sql += String.Join(",", sqlValues) + ")";

            return sql;
        }

        public static List<string> PrepSqlUpdateFields<T>(T model, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            // En el update no podemos modificar el campo 'createdDate'
            if (fields.Contains("createdDate"))
            {
                fields.Remove("createdDate");
            }

            List<string> sql = new List<string>();
            List<KeyValuePair<string, string>> list = SqlFields(model, fields, skipFields);
            List<string> keys = new List<string>();
            string sqlField = null;

            foreach (KeyValuePair<string, string> obj in list)
            {
                keys.Add(obj.Key);

                sqlField = " [" + obj.Key + "] = " + obj.Value;
                sql.Add(sqlField);
            }

            if (fields == null || fields.Count == 0)
            {
                fields = UtilHelper.GetClassProperties(model);
            }

            string temp = "modifiedDate";
            if (fields.Contains(temp) && !keys.Contains(temp))
            {
                sqlField = " [" + temp + "] = [dbo].[Fn.TimeStamp.GetTimeStampDate]()";
                sql.Add(sqlField);
            }

            return sql;
        }

        public static List<KeyValuePair<string, string>> SqlFields<T>(T model, List<string> fields = null, List<string> skipFields = null) where T : new()
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            List<string> sql = new List<string>();
            Type type = model.GetType();
            string sqlField = null;
            double valueDouble = 0;
            decimal valueDecimal = 0;
            string propertyName = null;

            if (skipFields == null) skipFields = new List<string>();
            if (!skipFields.Contains(FieldCreatedDate)) skipFields.Add(FieldCreatedDate); // Utilizaremos la fecha de la database
            if (!skipFields.Contains(FieldModifiedDate)) skipFields.Add(FieldModifiedDate); // Utilizaremos la fecha de la database

            if (fields == null || fields.Count == 0)
            {
                fields = UtilHelper.GetClassProperties(model);
            }

            foreach (string field in fields)
            {
                if (skipFields.Contains(field))
                {
                    continue;
                }
                try
                {
                    var property = type.GetProperty(field);
                    propertyName = property.PropertyType.Name.ToLower();
                    var value = property.GetValue(model, null);
                    sqlField = null;

                    if (propertyName.Contains("nullable")) // Nullable
                    {
                        if (value == null)
                        {
                            continue;
                        }
                        else
                        {
                            propertyName = value.GetType().Name.ToLower();

                            if (propertyName.Contains("bool")) // boolean
                            {
                                sqlField = PrepToSQL((bool)value);
                            }
                            else if (propertyName.Contains("decimal")) // decimal
                            {
                                valueDecimal = Convert.ToDecimal(value);
                                sqlField = valueDecimal.ToString(CultureInfo.InvariantCulture);
                            }
                            else if (propertyName.Contains("double") || propertyName.Contains("float") || propertyName.Contains("single")) // float, double
                            {
                                valueDouble = Convert.ToDouble(value);
                                sqlField = valueDouble.ToString(CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                sqlField = value.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (propertyName.Equals("string") && value != null) // string
                        {
                            sqlField = PrepToSQL(value.ToString());
                        }
                        else if (propertyName.Contains("bool")) // boolean
                        {
                            sqlField = PrepToSQL((bool)value);
                        }
                        else if (propertyName.Contains("decimal")) // decimal
                        {
                            valueDecimal = Convert.ToDecimal(value);
                            if (valueDecimal != 0) sqlField = valueDecimal.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (propertyName.Contains("double") || propertyName.Contains("float") || propertyName.Contains("single")) // float, double
                        {
                            valueDouble = Convert.ToDouble(value);
                            if (valueDouble != 0) sqlField = valueDouble.ToString(CultureInfo.InvariantCulture);
                        }
                        else // others
                        {
                            if (value.ToString().Length > 0) sqlField = value.ToString();
                        }
                    }

                    if (sqlField != null)
                    {
                        list.Add(new KeyValuePair<string, string>(property.Name, sqlField));
                    }
                }
                catch (Exception) { }
            }

            return list;
        }
    }
}