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
using System.Text.RegularExpressions;

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

        public void Collecting_data(List<string> QueryString)
        {
            string responseBody = string.Empty;

            List<Dictionary<string, object>> respnoseJson = new List<Dictionary<string, object>>();

            Dictionary<string, string[]> keyValuePairs = new Dictionary<string, string[]>();

            Dictionary<int, string> EqualValue = new Dictionary<int, string>();

            for (int i = 0; i < QueryString.Count; i++)
            {
                string tableName = ExtractTableNameFromQuery(QueryString[i]);

                string[] fieldsName = ExtractFieldsFromQuery(QueryString[i]);

                string divideString = equalstring(QueryString[i]);

                keyValuePairs.Add(tableName, fieldsName);

                EqualValue.Add(i, divideString);
            }

            try
            {
                OdbcCommand command = new OdbcCommand(sqlQuery(QueryString[0]), _OdbcConnection);
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();

                    for (int i = 0; i < keyValuePairs[ExtractTableNameFromQuery(sqlQuery(QueryString[0]))].Length; i++)
                    {
                        map.Add(keyValuePairs[ExtractTableNameFromQuery(sqlQuery(QueryString[0]))][i], reader.GetString(i));
                    }

                    if (keyValuePairs.Count > 1)
                    {

                        for (int j = 1; j < keyValuePairs.Count; j++)
                        {
                            List<Dictionary<string, object>> Jsonobject = new List<Dictionary<string, object>>();

                            List<int> newEqual = new List<int>();

                            newEqual = TwoInt(EqualValue[j]);

                            OdbcCommand command2 = new OdbcCommand(sqlQuery(QueryString[j]), _OdbcConnection);
                            OdbcDataReader reader2 = command2.ExecuteReader();

                            while (reader2.Read())
                            {
                                Dictionary<string, object> map2 = new Dictionary<string, object>();

                                if (reader.GetString(newEqual[0]) == reader2.GetString(newEqual[1]))
                                {
                                    for (int k = 0; k < keyValuePairs[ExtractTableNameFromQuery(sqlQuery(QueryString[j]))].Length; k++)
                                    {
                                        map2.Add(keyValuePairs[ExtractTableNameFromQuery(sqlQuery(QueryString[j]))][k], reader2.GetString(k));
                                    }

                                    Jsonobject.Add(map2);
                                }
                            }

                            reader2.Close();

                            if (Jsonobject.Count > 0)
                            {
                                map.Add(ExtractTableNameFromQuery(sqlQuery(QueryString[j])), Jsonobject);
                            }
                        }

                    }

                    respnoseJson.Add(map);
                }

                reader.Close();

                responseBody = JsonConvert.SerializeObject(respnoseJson, Formatting.Indented);

                Console.WriteLine(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
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

        public string sqlQuery(string query)
        {
            int selectIndex = query.IndexOf("SELECT");

            string selectPart = query.Substring(selectIndex);

            if (selectIndex >= 0)
            {
                int semicolonIndex = selectPart.IndexOf(';');
                if (semicolonIndex >= 0)
                {
                    selectPart = selectPart.Substring(0, semicolonIndex);
                }
            }

            return selectPart;
        }

        public string equalstring(string query)
        {
            string afterSemicolon = string.Empty;

            int semicolonIndex = query.IndexOf(';');

            if (semicolonIndex >= 0)
            {
                afterSemicolon = query.Substring(semicolonIndex + 1).Trim();
            }

            return afterSemicolon;
        }

        public List<int> TwoInt(string requestString)
        {
            List<int> answerList = new List<int>();

            Match match = Regex.Match(requestString, @"\d+");

            if (match.Success)
            {
                int firstNumber = int.Parse(match.Value);

                match = match.NextMatch();

                if (match.Success)
                {
                    int secondNumber = int.Parse(match.Value);

                    answerList.Add(firstNumber);

                    answerList.Add(secondNumber);
                }
            }

            return answerList;
        }

    }
}
