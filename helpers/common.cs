using System.Data;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace helpers
{
    public record generateRandom
    {
        public static string Number(int digitCount)
        {
            string result = "";
            Random random = new Random();
            for (var i = 0; i < digitCount; i++)
            {
                result += random.Next(0, 9).ToString();
            }
            return result;
        }

        public static string Alphanumeric(int count)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string result = "";
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                result += chars[random.Next(chars.Length)];
            }
            return result;
        }
    }
    public record maskString
    {
        public static string PhoneNumber(string unmaskedString)
        {
            return "******" + unmaskedString.Substring(unmaskedString.Length - 4);
        }

        public static string EmailId(string unmaskedString)
        {
            string[] splittedString = unmaskedString.Split('@');
            return splittedString[0].Substring(0, 2) + "******@" + unmaskedString.Split('@')[1];
        }
    }
    public record dataTableConversion
    {
        public List<T> ToList<T>(DataTable dt) where T : class
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }


        public T ToModel<T>(DataTable dt) where T : class
        {
            T data = Activator.CreateInstance<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data = item;
                break; // only loop onces, as this is a model.
            }
            return data;
        }

        private T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == column.ColumnName.ToLower())
                    {
                        //skip the column if the value is null or empty
                        if (dr[column.ColumnName].ToString() == "" || dr[column.ColumnName].ToString() == null)
                            continue;

                        pro.SetValue(obj, dr[column.ColumnName], null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }
    }
    public struct xmlConversion
    {
        public string fromList<T>(List<T> listValue)
        {
            // Serialize the list to XML
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            StringWriter sw = new StringWriter();
            serializer.Serialize(sw, listValue);
            return sw.ToString();
        }
        public string fromDataTable(DataTable dataTable)
        {
            // Create a StringWriter to hold the XML
            using (StringWriter writer = new StringWriter())
            {
                // Set the table name and use the WriteXml method to write the DataTable to the StringWriter
                dataTable.TableName = "records";
                dataTable.WriteXml(writer);

                // Return the XML string
                return writer.ToString();
            }
        }
        public string fromDictionaryList<T>(List<Dictionary<string, T>> dataList)
        {
            var dataListElement = new XElement("DataList");

            foreach (var dataDict in dataList)
            {
                foreach (var entry in dataDict)
                {
                    var dataElement = new XElement("Data");
                    dataElement.SetAttributeValue("key", entry.Key);
                    dataElement.Add(typeof(T).GetProperties().Select(prop =>
                                    new XElement(prop.Name, prop.GetValue(entry.Value, null))));
                    dataListElement.Add(dataElement);
                }
            }
            return dataListElement.ToString();
        }
        public string fromDictionary<T>(Dictionary<string, T> dataList)
        {
            var dataListElement = new XElement("DataList");

            //foreach (var dataDict in dataList)
            {
                foreach (var entry in dataList)
                {
                    var dataElement = new XElement("Data");
                    dataElement.SetAttributeValue("key", entry.Key);
                    dataElement.Add(typeof(T).GetProperties().Select(prop =>
                                    new XElement(prop.Name, prop.GetValue(entry.Value, null))));
                    dataListElement.Add(dataElement);
                }
            }
            return dataListElement.ToString();
        }
    }
}
