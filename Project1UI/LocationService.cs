
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Project1UI
{
    public class LocationService
    {

        private readonly HttpClient _httpClient = new();

        public LocationService(Uri serverUri)
        {

            _httpClient.BaseAddress = serverUri;
        }

        public async Task AddNewLocationAsync(string storeName)
        {
            Dictionary<string, string> query = new() { ["storeName"] = storeName};
            string requestUri = QueryHelpers.AddQueryString("/api/Location", query);

            HttpRequestMessage request = new(HttpMethod.Put, requestUri);

            request.Headers.Accept.Add(new(MediaTypeNames.Application.Json));

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw;// UnexpectedServerBehaviorException("network error", ex);
            }

            response.EnsureSuccessStatusCode();

        }
    }
}
