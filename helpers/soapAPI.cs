using System.Net.Http.Headers;
using System.Text;

namespace helpers
{
    public class soapAPI
    {
        private readonly HttpClient _httpClient;

        public soapAPI()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> sendSoapRequestAsync(HttpMethod http_method,
                                                                    string url,
                                                                    string soap_action,
                                                                    string soap_envelope,
                                                                    Dictionary<string, string>? headers = null,
                                                                    AuthenticationHeaderValue? authentication = null)
        {
            var request = new HttpRequestMessage(http_method, url)
            {
                Content = new StringContent(soap_envelope, Encoding.UTF8, "text/xml")
            };

            // Add SOAPAction header
            if (!string.IsNullOrEmpty(soap_action))
            {
                request.Headers.Add("SOAPAction", soap_action);
            }

            // Add additional headers if provided
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            // Add authentication if provided
            if (authentication != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = authentication;
            }

            // Send the request
            return await _httpClient.SendAsync(request);
        }
    }
}
