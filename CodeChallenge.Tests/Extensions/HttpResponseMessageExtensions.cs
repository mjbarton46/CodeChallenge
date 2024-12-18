using Newtonsoft.Json;
using System.IO;
using System.Net.Http;

namespace CodeCodeChallenge.Tests.Integration.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static T DeserializeContent<T>(this HttpResponseMessage message)
        {
            T responseObject = default(T);
            if(message != null)
            {
                var responseJson = message.Content.ReadAsStringAsync().Result;
                responseObject = JsonSerializer.CreateDefault().Deserialize<T>(
                    new JsonTextReader(new StringReader(responseJson)));
            }

            return responseObject;
        }
    }
}
