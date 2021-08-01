using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

namespace ClientApp
{
    class Program
    {
        // static void Main(string[] args)
        // {
        //     AuthConfig config = AuthConfig.ReadFromJsonFile("appsettings.json");
        //     Console.WriteLine($"Authority: {config.Authority}");
        // }

        static void Main(string[] args)
        {
            //Console.WriteLine("Making the call...");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            //to acquire token

            AuthConfig config = AuthConfig.ReadFromJsonFile("appsettings.json");

            IConfidentialClientApplication app;

            app = ConfidentialClientApplicationBuilder.Create(config.Registered_ClientId)
                .WithClientSecret(config.Scada_ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            string[] Scada_ResourceIds = new string[] { config.Scada_ResourceId };

            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(Scada_ResourceIds).ExecuteAsync();
                
                Console.WriteLine("Token acquired \n");
                Console.WriteLine(result.AccessToken);
                
            }
            catch (MsalClientException ex)
            {
                
                Console.WriteLine(ex.Message);
                
            }

            //if token acquired successfully
            //proceed to get information from scada

            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                var httpClient = new HttpClient();
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

                if (defaultRequestHeaders.Accept == null ||
                   !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/x-www-form-urlencoded"))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new
                      MediaTypeWithQualityHeaderValue("application/json"));
                }
                defaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("bearer", result.AccessToken);

                //HttpResponseMessage response = await httpClient.GetAsync(config.Scada_BaseAddress);
                HttpResponseMessage response = await httpClient.GetAsync("https://scada.pttngd.co.th/api/scada/get_all");
                if (response.IsSuccessStatusCode)
                {
                    
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }
                else
                {
                    
                    Console.WriteLine($"Failed to call the Web Api: {response.StatusCode}");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                }
                Console.ResetColor();
            }

        }

    }
}
