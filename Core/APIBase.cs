using MMC.Utils;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using TechTalk.SpecFlow;

namespace MMC.CoreInterfaces
{
    [Binding]
    public class APIBase :StepBase
    {

        public RestClient restClient;
        public RestRequest restRequest;
        public IRestResponse restResponse;
        public string baseUrl;

        public void SetbaseUrl(string AppName)
        {
            baseUrl =  Helper.GetAppURLBasedonEnv(AppName);
        }

        public RestClient SetUrl(string endPoint)
        {
            var url = Path.Combine(baseUrl, endPoint);
            var restClient = new RestClient(url);
            return restClient;
        }
        public RestRequest CreateGetRequest()
        {
            var restRequest = new  RestRequest(Method.GET);
            restRequest.AddHeader("Accept", "application/json");
            return restRequest;
        }

        public IRestResponse GetResponse(RestClient client, RestRequest request)
        {
            return client.Execute(request);
        }

        public DeserializeJson GetContent<DeserializeJson>(IRestResponse response)
        {
            var content = response.Content;
            DeserializeJson deserializeJsonObject = JsonConvert.DeserializeObject<DeserializeJson>(content);

            return deserializeJsonObject;

        }

    }
}
