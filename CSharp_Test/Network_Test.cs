using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CSharp_Test
{
    /// <summary>
    /// 测试HttpClient
    /// </summary>
    public class UseHttpClient
    {
        private const string url= "http://test2.fastoa.co/api/rank?pageSize=5";

        public static async Task GetDataSimpleAsync()
        {
            Console.WriteLine("\n> 测试HttpClient读取URL返回的String");
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json;odata=verbose");  //响应内容默认为XML
                    ShowHeaders("Request Headers:", client.DefaultRequestHeaders);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Response Status Code:{(int)response.StatusCode}" + $"{response.ReasonPhrase}");
                        string responseBodyAsText = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Received playload of{responseBodyAsText.Length} characters");
                        Console.WriteLine();
                        Console.WriteLine(responseBodyAsText);
                    }
                    ShowHeaders("Response Headers:", response.Headers);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
        }

        public static void ShowHeaders(string title,HttpHeaders headers)
        {
            Console.WriteLine(title);
            foreach(var header in headers)
            {
                string value = string.Join(" ", header.Value);
                Console.WriteLine($"Header : {header.Key} Value : {value}");
            }
            Console.WriteLine();
        }
    }
}
