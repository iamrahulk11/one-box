using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace helpers
{
    public class restApi
    {
        private readonly HttpClient _httpClient;

        public restApi()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> SendRequestAsync(
            string url,
            HttpMethod method,
            object? body = null,
            string? body_type = null, // "none", "json", "form-data", "x-www-form-urlencoded"
            bool json_converion_needed = false,
            Dictionary<string, string>? headers = null,
            AuthenticationHeaderValue? authentication = null)
        {
            var request = new HttpRequestMessage(method, url);

            // Add body if provided and body type is specified
            if (body != null && body_type != null)
            {
                switch (body_type.ToLower())
                {
                    case "json":
                        if (json_converion_needed)
                        {
                            string jsonBody = JsonSerializer.Serialize(body);
                            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                        }
                        else
                        {
                            request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/json");
                        }
                        break;

                    case "form-data":
                        if (body is Dictionary<string, string> formData)
                        {
                            var multipartContent = new MultipartFormDataContent();
                            foreach (var kvp in formData)
                            {
                                multipartContent.Add(new StringContent(kvp.Value), kvp.Key);
                            }
                            request.Content = multipartContent;
                        }
                        else
                        {
                            throw new ArgumentException("Form-data body must be a Dictionary<string, string>.");
                        }
                        break;

                    case "x-www-form-urlencoded":
                        if (body is Dictionary<string, string> urlEncodedData)
                        {
                            var formContent = new FormUrlEncodedContent(urlEncodedData);
                            request.Content = formContent;
                        }
                        else
                        {
                            throw new ArgumentException("x-www-form-urlencoded body must be a Dictionary<string, string>.");
                        }
                        break;

                    case "none":
                        // No body to add
                        break;

                    default:
                        throw new ArgumentException("Unsupported body type.");
                }
            }

            // Add headers if provided
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