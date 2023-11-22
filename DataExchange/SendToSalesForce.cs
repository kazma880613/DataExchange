using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataExchange
{
    public class SendToSalesForce
    {

        public async Task SendData()
        {
            string apiUrl = InfoLoad.URL;

            for(int i = 0; i < dbService._responseList.Count; i++)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        if (i + 1 == dbService._responseList.Count)
                        {
                            client.DefaultRequestHeaders.Add("IfLastBatch", "LastBatch");
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Add("IfLastBatch", "NotLast");
                        }

                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        string data = Newtonsoft.Json.JsonConvert.SerializeObject(dbService._responseList[i]);

                        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("API Response: " + responseContent);
                        }
                        else
                        {
                            Console.WriteLine("API request failed. Status code: " + response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    //if(dbService._responseList[i].objectItem == "Account")
                    //{
                    //    Thread.Sleep(dataInformation.delayTime);
                    //}
                }
            }

        }
    }
}
