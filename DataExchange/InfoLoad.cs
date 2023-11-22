using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExchange
{
    public class InfoLoad
    {
        public static dbConfig _SQLinfo;

        public static string ConnectionType;

        public static string ConnectionString;

        public static string QueryString;

        public static request _request;

        // static response _response = new response();

        public static string URL;

        public InfoLoad()
        {

        }

        public string SQLInfoLoad()
        {
            string fileName = "SQLInfo.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\clientfile\\", fileName);

            try
            {
                string[] AllInfo = File.ReadAllText(fullPath).Split(",");

                ConnectionType = AllInfo[0];

                ConnectionString = AllInfo[1];

                Console.WriteLine("ConnectionString Loading Is Success");

                return ConnectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                return ex.Message;
            }
        }

        public void requestInfoLoad(string objectName)
        {
            string fileName = objectName + ".txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\clientfile\\", fileName);

            string AllInfo = File.ReadAllText(fullPath);

            if(AllInfo != null || AllInfo != "")
            {
                _request = JsonConvert.DeserializeObject<request>(AllInfo);

                //_response.objectItem = _request.objectItem;

                Console.WriteLine("QueryString is ready.");
            }
        }

        public void scheduleInfo()
        {
            string fileName = "TodoObject.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\clientfile\\", fileName);

            string[] AllInfo = File.ReadAllLines(fullPath);

            string lastLine = File.ReadLines(fullPath).Reverse().FirstOrDefault();

            for (int i = 0; i < AllInfo.Length; i++)
            {
                string[] parts = AllInfo[i].Split(',');

                if (parts.Length == 2)
                {
                    string key = parts[0];

                    string value = parts[1];

                    if(value != "success" )
                    {
                        requestInfoLoad(key);

                        if(AllInfo[i] == lastLine)
                        {
                            resetobjectStatus();
                        }
                        else
                        {
                            try
                            {
                                AllInfo[i] = key + "," + "success";

                                File.WriteAllLines(fullPath, AllInfo);
                            }
                            catch
                            {
                                AllInfo[i] = key + "," + "fail";

                                File.WriteAllLines(fullPath, AllInfo);
                            }
                        }

                        break;
                    }

                }
            }

        }

        public void resetobjectStatus()
        {
            string fileName = "TodoObject.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\clientfile\\", fileName);

            string[] AllInfo = File.ReadAllLines(fullPath);

            for (int i = 0; i < AllInfo.Length; i++)
            {
                string[] parts = AllInfo[i].Split(',');

                if (parts.Length == 2)
                {
                    string key = parts[0];

                    AllInfo[i] = key + "," + "Not yet";

                    File.WriteAllLines(fullPath, AllInfo);

                }
            }
        }

        public void getURL()
        {
            string fileName = "apiURL.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\clientfile\\", fileName);

            string[] AllInfo = File.ReadAllLines(fullPath);

            if (AllInfo.Length > 0)
            {
                URL = AllInfo[0];
            }
        }
    }
}
