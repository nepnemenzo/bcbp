using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace bcbp_101.Reflection
{
    public class BaseReflection
    {
        /// <summary>
        /// Method to compare two objects of the same class.
        /// You can pass the list of properties that can not be compared through "ignore".
        /// 
        /// Método para comparar 2 objetos de la misma clase.
        /// Se le puede pasar la lista de propiedades que no se va a comparar a través de "ignore".
        /// </summary>
        public static bool InstancePropertiesEqual<T>(T obj1, T obj2, params string[] ignore) where T : class
        {
            try
            {
                if (obj1 != null && obj2 != null)
                {
                    Type type = typeof(T);
                    List<string> ignoreList = new List<string>(ignore);
                    foreach (PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        if (!ignoreList.Contains(pi.Name))
                        {
                            object Obj1Valor = type.GetProperty(pi.Name).GetValue(obj1, null);
                            object Obj2Valor = type.GetProperty(pi.Name).GetValue(obj2, null);

                            if (Obj1Valor != null && !Obj1Valor.Equals(Obj2Valor))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error InstancePropertiesEqual: " + ex.Message);
                Console.ReadLine();
            }
            return false;

        }


        #region Convertidor de DataSet a List<T>
        /// <summary>
        /// Method that converts to DataSet a list Object type T
        /// 
        /// Método que convierte de DataSet a una lista de Objeto de tipo T
        /// </summary>
        public static List<T> DatasetBDToObjectList<T>(DataSet ds) where T : new()
        {
            List<T> listaObjects = new List<T>();
            if (ds == null || ds.Tables.Count == 0)
            {
                return listaObjects;
            }

            DataTable dt = ds.Tables[0];
            listaObjects = BaseReflection.DatatableBDToObjectList<T>(dt);

            return listaObjects;
        }

        public static List<T> DatatableBDToObjectList<T>(DataTable dt) where T : new()
        {
            List<T> listaObjects = new List<T>();
            if (dt == null)
            {
                return listaObjects;
            }

            Type t = typeof(T);
            foreach (DataRow dr in dt.Rows)
            {
                T item = new T();
                listaObjects.Add(item);
                foreach (DataColumn dc in dt.Columns)
                {
                    PropertyInfo pi = t.GetProperty(dc.ColumnName);
                    if (pi != null)
                    {
                        if ((dr[dc.ColumnName]) != DBNull.Value)
                        {
                            // Según el tipo de la columna de la database
                            Type typeDBColumn = dr[dc.ColumnName].GetType();
                            if (typeDBColumn.Name.Equals("Byte[]"))
                            {
                                Byte[] b = (Byte[])dr[dc.ColumnName];
                                if (b.Length > 0)
                                {
                                    if (pi.PropertyType.Name.Equals("Int32")) pi.SetValue(item, Convert.ToInt32(b[0]));
                                    else if (pi.PropertyType.Name.Equals("Int64")) pi.SetValue(item, Convert.ToInt64(b[0]));
                                }
                            }
                            else // Según el tipo de la variable del modelo
                            {
                                var piValue = dr[dc.ColumnName];
                                switch (pi.PropertyType.Name.ToString())
                                {
                                    case "Int32":
                                        pi.SetValue(item, Convert.ToInt32(piValue));
                                        break;
                                    case "Int64":
                                        pi.SetValue(item, Convert.ToInt64(piValue));
                                        break;
                                    case "String":
                                        pi.SetValue(item, Convert.ToString(piValue).Replace("\"", ""));
                                        break;
                                    case "Boolean":
                                        pi.SetValue(item, Convert.ToBoolean(piValue));
                                        break;
                                    case "Double":
                                        pi.SetValue(item, Convert.ToDouble(piValue));
                                        break;
                                    case "Single":
                                        pi.SetValue(item, Convert.ToSingle(piValue));
                                        break;
                                    case "DateTime":
                                        pi.SetValue(item, Convert.ToDateTime(piValue));
                                        break;
                                    case "Char":
                                        pi.SetValue(item, Convert.ToChar(piValue));
                                        break;
                                    case "Decimal":
                                        pi.SetValue(item, Convert.ToDecimal(piValue));
                                        break;
                                    case "Nullable`1":
                                        if (piValue == null)
                                        {
                                            pi.SetValue(item, null);
                                        }
                                        else
                                        {
                                            switch (pi.PropertyType.GenericTypeArguments[0].Name.ToString())
                                            {
                                                case "Int32":
                                                    pi.SetValue(item, Convert.ToInt32(piValue));
                                                    break;
                                                case "Int64":
                                                    pi.SetValue(item, Convert.ToInt64(piValue));
                                                    break;
                                                case "String":
                                                    pi.SetValue(item, Convert.ToString(piValue).Replace("\"", ""));
                                                    break;
                                                case "Boolean":
                                                    pi.SetValue(item, Convert.ToBoolean(piValue));
                                                    break;
                                                case "Double":
                                                    pi.SetValue(item, Convert.ToDouble(piValue));
                                                    break;
                                                case "Single":
                                                    pi.SetValue(item, Convert.ToSingle(piValue));
                                                    break;
                                                case "DateTime":
                                                    pi.SetValue(item, Convert.ToDateTime(piValue));
                                                    break;
                                                case "Char":
                                                    pi.SetValue(item, Convert.ToChar(piValue));
                                                    break;
                                                case "Decimal":
                                                    pi.SetValue(item, Convert.ToDecimal(piValue));
                                                    break;
                                            }
                                        }
                                        break;
                                        //case "Nullable`1":
                                        //    if (pi.PropertyType.FullName.Contains("System.Int32"))
                                        //    {
                                        //        pi.SetValue(item, Convert.ToInt32(dr[dc.ColumnName]));
                                        //    }

                                        //    break;
                                }
                            }
                        }
                    }
                }
            }
            return listaObjects;
        }

        internal static bool TryJoinObects<T, K>(T source, K destiny) where T : new() where K : new()
        {
            Type t = typeof(T);
            Type k = typeof(K);

            bool success = true;

            try
            {
                foreach (var parameter in t.GetProperties())
                {
                    if (parameter.GetValue(source) != null)
                    {
                        if (k.GetProperty(parameter.Name).GetValue(destiny) == null)
                            k.GetProperty(parameter.Name).SetValue(destiny, parameter.GetValue(source));
                        else
                        {
                            int tmp = -1;
                            if (int.TryParse(k.GetProperty(parameter.Name).GetValue(destiny).ToString(), out tmp))
                            {
                                if (tmp == 0 || tmp == -1)
                                {
                                    k.GetProperty(parameter.Name).SetValue(destiny, parameter.GetValue(source));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                success = false;
            }
            return success;
        }
        #endregion

        #region Convertidor de DataSet a Objeto tipo t
        /// <summary>
        /// Method that converts to DataSet a Object type T
        /// 
        /// Método que convierte de DataSet a un Objeto de tipo T
        /// </summary>
        public static T DatasetBDToObject<T>(DataSet ds) where T : new()
        {
            if (ds.Tables.Count <= 0) return default(T);

            DataTable dt = ds.Tables[0];
            Type t = typeof(T);
            object item = new T();

            if (dt.Rows.Count <= 0) return default(T);

            DataRow dr = dt.Rows[0];
            foreach (DataColumn dc in dt.Columns)
            {
                PropertyInfo pi = t.GetProperty(dc.ColumnName);
                if (pi != null)
                {
                    if ((dr[dc.ColumnName]) != DBNull.Value)
                    {
                        //if (!ignoreList.Contains(pi.Name))
                        //{
                        switch (pi.PropertyType.Name.ToString())
                        {
                            case "Int32":
                                pi.SetValue(item, Convert.ToInt32(dr[dc.ColumnName]));
                                break;
                            case "Int64":
                                pi.SetValue(item, Convert.ToInt64(dr[dc.ColumnName]));
                                break;
                            case "String":
                                pi.SetValue(item, dr[dc.ColumnName]);
                                break;
                            case "Boolean":
                                pi.SetValue(item, Convert.ToBoolean(dr[dc.ColumnName]));
                                break;
                            case "Double":
                                pi.SetValue(item, Convert.ToDouble(dr[dc.ColumnName]));
                                break;
                            case "Single":
                                pi.SetValue(item, Convert.ToSingle(dr[dc.ColumnName]));
                                break;
                            case "DateTime":
                                pi.SetValue(item, Convert.ToDateTime(dr[dc.ColumnName]));
                                break;
                            case "Char":
                                pi.SetValue(item, Convert.ToChar(dr[dc.ColumnName]));
                                break;
                            case "Decimal":
                                pi.SetValue(item, Convert.ToDecimal(dr[dc.ColumnName]));
                                break;
                        }
                        //try
                        //{
                        //    pi.SetValue(item, dr[dc.ColumnName], null);
                        //}
                        //catch (Exception ex)
                        //{
                        //    string error = ex.Message;
                        //}
                    }
                    //}
                }
                else //Tipo Primitivo
                {
                    switch (t.Name.ToString())
                    {
                        case "Int32":
                            item = Convert.ToInt32(dr[dc.ColumnName]);
                            break;
                        case "Int64":
                            item = Convert.ToInt64(dr[dc.ColumnName]);
                            break;
                        case "String":
                            item = dr[dc.ColumnName];
                            break;
                        case "Boolean":
                            item = Convert.ToBoolean(dr[dc.ColumnName]);
                            break;
                        case "Double":
                            item = Convert.ToDouble(dr[dc.ColumnName]);
                            break;
                        case "Single":
                            item = Convert.ToSingle(dr[dc.ColumnName]);
                            break;
                        case "DateTime":
                            item = Convert.ToDateTime(dr[dc.ColumnName]);
                            break;
                        case "Char":
                            item = Convert.ToChar(dr[dc.ColumnName]);
                            break;
                        case "Decimal":
                            item = Convert.ToDecimal(dr[dc.ColumnName]);
                            break;
                    }
                }
            }
            return (T)item;
        }
        #endregion

        /// <summary>
        /// Metodo para validar si es decimal
        /// </summary>
        /// <param name="cadena"></param>
        /// <returns></returns>
        bool isDecimal(String cadena)
        {
            bool flag = false;
            for (int i = 0; i < cadena.Length; i++)
            {
                char c = cadena[i];
                if ((c >= '0' && c <= '9'))
                {
                    continue;
                }
                else
                {
                    if ((c == ',') && (i != 0 && i < (cadena.Length - 1)) && (cadena[i - 1] != ','))
                    {
                        flag = true;

                    }
                    else return false;

                }
            }
            return flag;
        }
    }

}