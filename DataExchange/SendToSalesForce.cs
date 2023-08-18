using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExchange
{
    public class SendToSalesForce
    {
        public static async Task SendData(string data)
        {
            string apiUrl = "https://api.example.com/endpoint";

            using (HttpClient client = new HttpClient())
            {
                try
                {
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
            }
        }
    }
}
