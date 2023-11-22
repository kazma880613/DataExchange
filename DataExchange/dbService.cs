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
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Metadata;

namespace DataExchange
{
    public class dbService
    {
        OdbcConnection _OdbcConnection;

        OracleConnection connection ;

        public static List<response> _responseList = new List<response>();

        public dbService()
        {

        }

        public void chooseSQL_Open()
        {
            if(InfoLoad.ConnectionType == "ODBC")
            {
                ODBC_SQL_Open();
            }

            if(InfoLoad.ConnectionType == "ORACLE")
            {
                ORACLE_SQL_Open();
            }
            
        }

        public void ODBC_SQL_Open()
        {

            _OdbcConnection = new OdbcConnection(InfoLoad.ConnectionString);

            try
            {
                _OdbcConnection.Open();

                Console.WriteLine("DB Connection is Connect.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Connection Fail, ODBC Error: " + ex.Message);
            }

        }

        public void ORACLE_SQL_Open()
        {
            connection = new OracleConnection(InfoLoad.ConnectionString);

            try
            {
                connection.Open();

                Console.WriteLine("DB Connection is Connect.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Connection Fail, ORACLE Error: " + ex.Message);
            }
        }

        public void chooseSQL_Close()
        {
            if (InfoLoad.ConnectionType == "ODBC")
            {
                ODBC_SQL_Close();
            }

            if (InfoLoad.ConnectionType == "ORACLE")
            {
                ORACLE_SQL_Close();
            }
        }

        public void ODBC_SQL_Close()
        {
            try
            {
                if(_OdbcConnection.State.ToString() == "Open")
                {
                    _OdbcConnection.Close();

                    Console.WriteLine("DB Connection is Closed.");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Connection Close Fail, ODBC Error: " + ex.Message);
            }
        }

        public void ORACLE_SQL_Close()
        {
            try
            {
                if (connection.State.ToString() == "Open")
                {
                    connection.Close();

                    Console.WriteLine("DB Connection is Closed.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("DB Connection Close Fail, ORACLE Error: " + ex.Message);
            }
        }

        public void Collecting_data(List<string> QueryString)
        {
            HandleString _handler = new HandleString();

            string responseBody = string.Empty;

            List<Dictionary<string, object>> respnoseJson = new List<Dictionary<string, object>>();

            Dictionary<string, string[]> keyValuePairs = new Dictionary<string, string[]>();

            Dictionary<int, string> EqualValue = new Dictionary<int, string>();

            for (int i = 0; i < QueryString.Count; i++)
            {
                string tableName = _handler.ExtractTableNameFromQuery(QueryString[i]);

                string[] fieldsName = _handler.ExtractFieldsFromQuery(QueryString[i]);

                string divideString = _handler.equalstring(QueryString[i]);

                keyValuePairs.Add(tableName, fieldsName);

                EqualValue.Add(i, divideString);
            }

            try
            {
                int count = 0;

                for(int c = 0; c < QueryString.Count; c++)
                {
                    QueryString[c] = _handler.DateReplace(QueryString[c]) ;
                }

                //Console.WriteLine(QueryString[0]);

                OdbcCommand command = new OdbcCommand(_handler.sqlQuery(QueryString[0]), _OdbcConnection);

                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Dictionary<string, object> map = new Dictionary<string, object>();

                    for (int i = 0; i < keyValuePairs[_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[0]))].Length; i++)
                    {

                        string fielddata = string.Empty;

                        object valueFromDatabase = reader[keyValuePairs[_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[0]))][i]];

                        fielddata = (valueFromDatabase != DBNull.Value) ? valueFromDatabase.ToString() : string.Empty;

                        map.Add(keyValuePairs[_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[0]))][i], fielddata);
                    }

                    if (keyValuePairs.Count > 1)
                    {

                        for (int j = 1; j < keyValuePairs.Count; j++)
                        {
                            List<Dictionary<string, object>> Jsonobject = new List<Dictionary<string, object>>();

                            List<int> newEqualList = new List<int>();

                            bool _ifEqual = false;

                            newEqualList = _handler.TwoInt(EqualValue[j]);

                            OdbcCommand command2 = new OdbcCommand(_handler.sqlQuery(QueryString[j]), _OdbcConnection);
                            OdbcDataReader reader2 = command2.ExecuteReader();

                            while (reader2.Read())
                            {
                                Dictionary<string, object> map2 = new Dictionary<string, object>();

                                for (int m = 0; m + 2 <= newEqualList.Count ; m += 2)
                                {
                                    if (reader.GetString(newEqualList[m]) == reader2.GetString(newEqualList[m+1]))
                                    {
                                        _ifEqual = true;
                                    }
                                    else
                                    {
                                        _ifEqual = false;

                                        break;
                                    }
                                }

                                if(_ifEqual == true)
                                {
                                    for (int k = 0; k < keyValuePairs[_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[j]))].Length; k++)
                                    {

                                        string field2data = string.Empty;

                                        object valueFromDatabase2 = reader2[keyValuePairs[_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[j]))][k]];

                                        field2data = (valueFromDatabase2 != DBNull.Value) ? valueFromDatabase2.ToString() : string.Empty;

                                        map2.Add(keyValuePairs[_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[j]))][k], field2data);
                                    }

                                    Jsonobject.Add(map2);
                                }
                                
                            }

                            reader2.Close();

                            if (Jsonobject.Count > 0)
                            {
                                map.Add(_handler.ExtractTableNameFromQuery(_handler.sqlQuery(QueryString[j])), Jsonobject);
                            }
                        }

                    }

                    respnoseJson.Add(map);

                    count++;
                }

                reader.Close();

                int batchSize ;

                if (InfoLoad._request.objectItem == "OrdernReturnDetail")
                {
                    batchSize = 50;
                }
                else
                {
                    batchSize = 100;
                }

                
                for (int i = 0; i < respnoseJson.Count; i += batchSize)
                {
                    var batch = respnoseJson.Skip(i).Take(batchSize);

                    string json = JsonConvert.SerializeObject(batch, Formatting.Indented);

                    response _newResponse = new response();

                    _newResponse.objectItem = InfoLoad._request.objectItem;

                    _newResponse.body = json;

                    //Console.WriteLine(json);

                    _responseList.Add(_newResponse);
                }

                //responseBody = JsonConvert.SerializeObject(respnoseJson, Formatting.Indented);

                //InfoLoad._response.body[0] = responseBody;

                //Console.WriteLine(responseBody);

                Console.WriteLine(count);

                //return responseBody;
            }
            catch (Exception ex)
            {
                responseBody = ex.Message;

                Console.WriteLine(responseBody);

                //return responseBody;
            }
        }

    }
}
