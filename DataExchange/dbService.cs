using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using System.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections;
using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Relational;

namespace DataExchange
{
    public class dbService
    {
        OdbcConnection _OdbcConnection;

        public dbService()
        {

        }

        public void ODBC_SQL_Open(string connectionString)
        {

            _OdbcConnection = new OdbcConnection(connectionString);

            try
            {
                _OdbcConnection.Open();

                Console.WriteLine("DB Connection is Connect");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ODBC Error: " + ex.Message);
            }

        }

        public void ODBC_SQL_Close()
        {
            try
            {
                if(_OdbcConnection.State.ToString() == "Open")
                {
                    _OdbcConnection.Close();

                    Console.WriteLine("DB Connection is Closed");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void doaction(string requestString)
        {
            string[] lines = requestString.Split('\n');

            if(lines.Length == 1)
            {
                ODBC_SQL_SimpleQuery(requestString);
            }
            else
            {
                ODBC_SQL_ComplexQuery(requestString);
            }
        }

        public void ODBC_SQL_SimpleQuery(string QueryString)
        {
            string[] fields = ExtractFieldsFromQuery(QueryString);

            string responseBody  = string.Empty;

            try
            {
                OdbcCommand command = new OdbcCommand(QueryString, _OdbcConnection);
                OdbcDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();

                    for(int i = 0; i < fields.Length ; i++)
                    {
                        map.Add(fields[i], reader.GetString(i));
                    }

                    string json = JsonConvert.SerializeObject(map, Formatting.Indented);

                    responseBody = responseBody + json + ",\n";
                }

                Console.WriteLine(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public void ODBC_SQL_ComplexQuery(string QueryString)
        {
            string[] lines = QueryString.Split('\n');

            string[] FirstLinefields = ExtractFieldsFromQuery(lines[0]);

            string[] Second_Linefields = ExtractFieldsFromQuery(lines[1]);

            string responseBody = string.Empty;

            //foreach (string word in lines)
            //{
            //     Console.WriteLine(word);
            //}

            try
            {
                OdbcCommand command = new OdbcCommand(lines[0], _OdbcConnection);
                OdbcDataReader reader = command.ExecuteReader();

                OdbcCommand command2 = new OdbcCommand(lines[1], _OdbcConnection);
                OdbcDataReader reader2 = command2.ExecuteReader();

                while (reader.Read())
                {
                    Dictionary<string, string> map = new Dictionary<string, string>();

                    for (int i = 0; i < FirstLinefields.Length; i++)
                    {
                        map.Add(FirstLinefields[i], reader.GetString(i));
                    }

                    string key1ToFind = "country.code";

                    string key2ToFind = "city.CountryCode";

                    while (reader2.Read())
                    {
                        //Dictionary<string, string> map2 = new Dictionary<string, string>();

                        //if (reader2.GetString(1) == reader.GetString(0))
                        //{
                        //    string tableName = ExtractTableNameFromQuery(lines[1]);

                        //    for (int j = 0; j < Second_Linefields.Length; j++)
                        //    {

                        //        map2.Add(Second_Linefields[j], reader2.GetString(j));

                        //        string json2 = JsonConvert.SerializeObject(map2, Formatting.Indented);

                        //        responseBody = responseBody + json2 + ",\n";

                        //    }
                        //}
                    }

                    string json = JsonConvert.SerializeObject(map, Formatting.Indented);

                    responseBody = responseBody + json + ",\n";

                    Console.WriteLine(reader.GetString(0));
                }

                //Console.WriteLine(responseBody);

                reader.Close();

                responseBody = string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }


            //try
            //{
            //    string tablename = ExtractTableNameFromQuery(lines[1]);

            //    responseBody = responseBody + "\"" + tablename + "\":[\n"  ;

            //    OdbcCommand command2 = new OdbcCommand(lines[1], _OdbcConnection);
            //    OdbcDataReader reader2 = command2.ExecuteReader();

            //    while (reader2.Read())
            //    {
            //        Dictionary<string, string> map2 = new Dictionary<string, string>();

            //        for (int j = 0; j < Second_Linefields.Length; j++)
            //        {
            //            map2.Add(Second_Linefields[j], reader2.GetString(j));
            //        }

            //        string json2 = JsonConvert.SerializeObject(map2, Formatting.Indented);

            //        responseBody = responseBody + json2 + ",\n";
            //    }

            //    responseBody = responseBody + "]";

            //    reader2.Close();

            //    Console.WriteLine(responseBody);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Error: " + ex.Message);
            //}

            //foreach (string line in lines)
            //{
            //    string[] words = line.Split(' '); // 根据空格拆分每一行
            //    foreach (string word in words)
            //    {
            //        Console.WriteLine(word);
            //    }
            //    Console.WriteLine("---------------------");
            //}
        }

        public string[] ExtractFieldsFromQuery(string query)
        {
            int startIndex = query.IndexOf("SELECT") + 7;
            int endIndex = query.IndexOf("FROM");

            string fieldsSubstring = query.Substring(startIndex, endIndex - startIndex).Trim();
            string[] fields = fieldsSubstring.Split(',');

            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = fields[i].Trim();
            }

            return fields;
        }

        public string ExtractTableNameFromQuery(string query)
        {
            int fromIndex = query.IndexOf("FROM") + 5; // 5 是 "FROM " 的长度
            int spaceIndex = query.IndexOf(" ", fromIndex);

            if (spaceIndex == -1)
            {
                return query.Substring(fromIndex).Trim();
            }
            else
            {
                return query.Substring(fromIndex, spaceIndex - fromIndex).Trim();
            }
        }
    }
}
