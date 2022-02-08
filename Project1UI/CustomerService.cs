
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Project1UI
{

  
    public class CustomerService

    {

        private readonly HttpClient _httpClient = new();

        public CustomerService(Uri serverUri)
        {

            _httpClient.BaseAddress = serverUri;
        }

        public async Task<List<Customer>> FindCustomerAsync(string firstName, string lastName)
        {

            Dictionary<string, string> query = new() { ["firstName"] = firstName, ["lastName"] = lastName };
            string requestUri = QueryHelpers.AddQueryString("/api/Customer", query);

            HttpRequestMessage request = new(HttpMethod.Get, requestUri);

            request.Headers.Accept.Add(new(MediaTypeNames.Application.Json));

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw ex.GetBaseException();
            }

            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentType?.MediaType != MediaTypeNames.Application.Json)
            {

            }

            var customers = await response.Content.ReadFromJsonAsync<List<Customer>>();
            if (customers == null)
            {

            }

            return customers;
        }
        public async Task AddNewCustomerAsync(string firstName, string lastName)
        {

            Dictionary<string, string> query = new() { ["firstName"] = firstName, ["lastName"] = lastName };
            string requestUri = QueryHelpers.AddQueryString("/api/Customer", query);

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
