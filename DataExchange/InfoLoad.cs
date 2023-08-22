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

        public static string ConnectionString;

        public static string QueryString;

        public static request _request;

        public InfoLoad()
        {

        }

        public string SQLInfoLoad()
        {
            string fileName = "SQLInfo.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            try
            {
                ConnectionString = File.ReadAllText(fullPath);

                //_SQLinfo = JsonConvert.DeserializeObject<dbConfig>(ConnectionString);

                Console.WriteLine("ConnectionString Loading Is Success");

                return ConnectionString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                return ex.Message;
            }
        }

        public string QueryInfoLoad()
        {
            string fileName = "QueryString.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            try
            {
                QueryString = File.ReadAllText(fullPath);

                return QueryString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                return ex.Message;
            }
        }

        public void requestInfoLoad()
        {
            string fileName = "requestInfo.txt";

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            string AllInfo = File.ReadAllText(fullPath);

            if(AllInfo != null || AllInfo != "")
            {
                _request = JsonConvert.DeserializeObject<request>(AllInfo);
            }
        }
    }
}
