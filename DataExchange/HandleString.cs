using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExchange
{
    public class HandleString
    {
        //處理SQL WHERE 後的字串出現時間條件
        public string DateReplace(string QueryString)
        {
            if(QueryString.IndexOf("Today", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                QueryString = QueryString.Replace("Today", DateTime.Today.ToString("yyyy-MM-dd"));
            }
            else if(QueryString.IndexOf("Tomorrow", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DateTime today = DateTime.Today;

                QueryString = QueryString.Replace("Tomorrow", today.AddDays(1).ToString("yyyy-MM-dd"));
            }

            return QueryString;
        }

        //取得SQL語句 欄位名稱
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

        //取得SQL語句 資料表名稱
        public string ExtractTableNameFromQuery(string query)
        {
            int fromIndex = query.IndexOf("FROM") + 5;
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

        //取得SQL語句 完整語句
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

        //取得SQL語句 後方加註的等式(判斷相等欄位用)
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

        //取得SQL語句 後方加註的等式的數字(判斷欄位於第幾個位置)
        public List<int> TwoInt(string requestString)
        {
            List<int> answerList = new List<int>();

            string[] parts2 = requestString.Split(",");

            for (int i = 0; i < parts2.Length; i++)
            {
                string[] answer = parts2[i].Split("=");

                answerList.Add(int.Parse(answer[0]));

                answerList.Add(int.Parse(answer[1]));
            }

            return answerList;
        }
    }
}
