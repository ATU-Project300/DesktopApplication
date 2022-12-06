using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace API
{
    public static class ApiHelper
    {
        //Static because HttpClient is thread safe :)
        public static HttpClient ApiClient { get; set; }

        public static void InitClient()
        {
            //Initialise
            ApiClient = new HttpClient();

            //Set base address to localhost (until API is moved online)
            //TODO: Direct to hosted API
            ApiClient.BaseAddress = new Uri("http://localhost:3000/"); //We will always have the same base address (until we move the API online)

            //Clear default headers
            ApiClient.DefaultRequestHeaders.Accept.Clear(); //Clear default headers

            //Show that we only want JSON in return for this client
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //JSON header

            //Write address and port to console for debugging
            //TODO: Remove this once not needed
            Console.WriteLine(ApiClient.BaseAddress);
            Console.WriteLine(ApiClient.BaseAddress.Port);
        }
    }
}
