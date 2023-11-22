using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExchange
{
    public class dataObject
    {

    }

    public class dbConfig
    {
        public string DBType;
        public string Server;
        public int Port;
        public string Database;
        public string Uid;
        public string Pwd;
    }

    public class request
    {
        public string objectItem;
        public List<string> QueryString;

    }

    public class response
    {
        public string objectItem;
        public string body; 
    }

    public class dataInformation
    {
        public static int batchSize = 50;
        public static int delayTime = 40000;
    }
}
