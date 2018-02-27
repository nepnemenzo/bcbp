using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace bcbp_101.Helpers
{
    public static class UtilHelper
    {
        public const string HttpContext = "MS_HttpContext";
        public const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        // string DisplayName = UtilHelper.GetDisplayName<CustomerModel>(m => m.FinancialCardNum);
        public static string GetDisplayName<TModel>(Expression<Func<TModel, object>> expression)
        {

            Type type = typeof(TModel);

            string propertyName = null;
            string[] properties = null;
            IEnumerable<string> propertyList;
            //unless it's a root property the expression NodeType will always be Convert
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expression.Body as UnaryExpression;
                    propertyList = (ue != null ? ue.Operand : null).ToString().Split(".".ToCharArray()).Skip(1); //don't use the root property
                    break;
                default:
                    propertyList = expression.Body.ToString().Split(".".ToCharArray()).Skip(1);
                    break;
            }

            //the propert name is what we're after
            propertyName = propertyList.Last();
            //list of properties - the last property name
            properties = propertyList.Take(propertyList.Count() - 1).ToArray(); //grab all the parent properties

            Expression expr = null;
            foreach (string property in properties)
            {
                PropertyInfo propertyInfo = type.GetProperty(property);
                expr = Expression.Property(expr, type.GetProperty(property));
                type = propertyInfo.PropertyType;
            }

            DisplayAttribute attr;
            attr = (DisplayAttribute)type.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class
            if (attr == null)
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(propertyName);
                    if (property != null)
                    {
                        attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }

            //To support translations call attr.GetName() instead of attr.Name
            return (attr != null) ? attr.GetName() : propertyName;
            //return (attr != null) ? attr.GetName() : String.Empty;
        }
       
        // amount: El valor de amount será un número sin decimales
        // Ejemplo : 2199 se convertirá a 21.99
        public static double AmountTo2Decimals(float amount)
        {
            double result = amount / 100;
            return result;
        }

        public static decimal FixedPointTo2Decimals(float amount)
        {
            // double result = amount / 100;             
            return Convert.ToDecimal(amount);
            //return decimal.Parse(amount.ToString("N"));
        }

        public static decimal? ToNullableDecimal(object val)
        {
            if (val is DBNull ||
                val == null)
            {
                return null;
            }
            if (val == null)
            {
                return null;
            }
            if (val is string &&
                ((string)val).Length == 0)
            {
                return null;
            }
            return Convert.ToDecimal(val);
        }

        public static string AmountTrimConvert(string amount)
        {
            if (amount == null || amount.Length < 3)
                return amount;

            try
            {
                string value = amount.Insert(amount.Length - 2, ".");
                value = value.TrimStart(new char[] { '0' });

                if (value[0].Equals('.')) value = "0" + value;

                return value;
            }
            catch (Exception)
            {
                return amount;
            }
        }

        public static string Truncate(string text, int maxLength, string suffix = "...")
        {
            if (text == null)
                return null;

            string str = text;
            if (maxLength > 0)
            {
                int length = maxLength - suffix.Length;
                if (length <= 0)
                {
                    return str;
                }
                if ((text != null) && (text.Length > maxLength))
                {
                    return (text.Substring(0, length).TrimEnd(new char[0]) + suffix);
                }
            }
            return str;
        }

        internal static string GetBaseUrl(object request)
        {
            throw new NotImplementedException();
        }

        public static Object ReflectionGetProperty(Object obj, string property)
        {
            Type t = obj.GetType();
            PropertyInfo prop = t.GetProperty(property);
            return prop.GetValue(obj);
        }

        // string path = UtilHelper.HostingFilePath("~/Images/transaction/pdf/receipt_test_" + formTransaction.id + ".pdf");
        public static string HostingFilePath(string file)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath(file);
            return path;
        }

        public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        public static string GetSHA1(string str)
        {
            SHA1 sha = SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public static string GetSHA256(string str)
        {
            SHA256 sha = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public static string GetHMACSHA256(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return ArrayToHexString(hashmessage);
            }
        }

        public static string ArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static List<string> GetClassProperties<T>(T item) where T : new()
        {
            var type = item.GetType();
            var properties = type.GetProperties();
            List<string> fields = new List<string>();
            foreach (var property in properties) fields.Add(property.Name);

            return fields;
        }

        public static string classPropertiesToString<T>(T item) where T : new()
        {
            List<string> list = GetClassProperties(item);
            String result = String.Join(", ", list);
            return result;
        }

        public static void CopyValues<TO, TD>(TO origin, ref TD destiny)
            where TO : new()
            where TD : new()
        {
            if (origin == null || destiny == null)
            {
                return;
            }

            Type typeOrigin = origin.GetType();
            var propsOrigin = typeOrigin.GetProperties();

            Type typeDestiny = destiny.GetType();
            var propsDestiny = typeDestiny.GetProperties();

            foreach (var pOrigin in propsOrigin)
            {
                try
                {
                    var pDestiny = typeDestiny.GetProperty(pOrigin.Name);
                    if (pDestiny != null)
                    {
                        pDestiny.SetValue(destiny, pOrigin.GetValue(origin, null));
                    }
                }
                catch (Exception) { }
            }
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string LowercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToLower(s[0]) + s.Substring(1);
        }

        // ========================================

        public static bool DateTimeIsNull(DateTime date)
        {
            //return (date == null || (date.Day == 1 && date.Month == 1 && date.Year == 1)) ? true : false;
            //return (date == null || date == DateTime.MinValue) ? true : false;
            //return (date == null || date == default(DateTime)) ? true : false;
            return (date == default(DateTime)) ? true : false;
        }

        public static DateTime TimestampToDatetime(double unixTimeStamp)
        {
            try
            {
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
                return dtDateTime;
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }

        public static DateTime TimestampToDatetime(String dateTimeString, CultureInfo culture, String format = "")
        {
            if (String.IsNullOrEmpty(dateTimeString))
                return default(DateTime);

            try
            {
                DateTime dateTime = (!String.IsNullOrEmpty(format)) ?
                                        DateTime.ParseExact(dateTimeString, format, culture.DateTimeFormat)
                                        : DateTime.Parse(dateTimeString, culture);
                return dateTime;
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }

        public static long DatetimeToTimestamp(DateTime dateTime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timestamp = (long)dateTime.Subtract(sTime).TotalSeconds;
            return timestamp;
        }

        public static string DatetimeToTimestamp(String dateTimeString, CultureInfo culture, String format = "")
        {
            DateTime date = TimestampToDatetime(dateTimeString, culture, format);
            if (DateTimeIsNull(date))
            {
                return null;
            }
            else
            {
                string timestamp = DatetimeToTimestamp(date).ToString();
                return timestamp;
            }
        }

        // Used with html DatetimePicker
        //public static long DatepickerDateToTimestamp(String dateTimeString)
        //{
        //    string timestamp = DatetimeToTimestamp(dateTimeString, CultureHelper.CultureFormat, CultureHelper.FormatDate);
        //    long result = (!String.IsNullOrEmpty(timestamp)) ? Convert.ToInt64(timestamp) : 0;
        //    return result;
        //}

        // Used with html DatetimePicker
        //public static string DatepickerDateToTimestampString(String dateTimeString)
        //{
        //    string timestamp = DatetimeToTimestamp(dateTimeString, CultureHelper.CultureFormat, CultureHelper.FormatDate);
        //    return timestamp;
        //}
        //convert string to fixedpoint
        public static string AmountTrim(string amount)
        {
            try
            {
                string newAmount = amount.Replace(".", "");
                int strLength = newAmount.Length - 1;
                newAmount = newAmount.Substring(strLength, 1) == "0" ? newAmount + "0" : newAmount;
                strLength--;
                newAmount = newAmount.Substring(strLength, 1) == "0" ? newAmount + "0" : newAmount;

                return newAmount.PadLeft(12, '0');

            }
            catch (Exception)
            {
                return amount;
            }
        }

        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static string ConvertHexToString(string HexValue)
        {
            string StrValue = "";
            try
            {
                while (HexValue.Length > 0)
                {
                    StrValue += System.Convert.ToChar(System.Convert.ToUInt32(HexValue.Substring(0, 2), 16)).ToString();
                    HexValue = HexValue.Substring(2, HexValue.Length - 2);
                }
            }
            catch (Exception) { StrValue = HexValue; }
            return StrValue;
        }

        public static string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            return "IP Address Unavailable";
        }

        public static string GetBaseUrl(HttpRequestBase request)
        {
            string url = request.Url.GetLeftPart(UriPartial.Authority);
            return url;
        }
        public static string GetBaseUrl(HttpRequest request)
        {
            string url = request.Url.GetLeftPart(UriPartial.Authority);
            return url;
        }

        public static string CreateUniqueToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string concat = Convert.ToBase64String(time.Concat(key).ToArray());
            string token = UtilHelper.GetSHA1(concat);

            return token;
        }


        public static string ConvertListToXml(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            XmlSerializerNamespaces xmlSerializerNameSpaces = new XmlSerializerNamespaces();
            xmlSerializerNameSpaces.Add("", "");

            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o, xmlSerializerNameSpaces);
            }
            catch (Exception)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }

            return sw.ToString();
        }
    }
}