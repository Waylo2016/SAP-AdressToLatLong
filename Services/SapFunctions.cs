using System.Net.Security;
using Newtonsoft.Json;
using SAP_AdresToLatLong.Interfaces;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Services;

public class SapFunctions : ISapFunctions
{
    private readonly HttpClient _httpClient;
    public SapFunctions()
    {
        var handler = new HttpClientHandler
        {
            
            // Doing this yucky ew thing because the server I (waylo) am using 
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                var expectedHost = Environment.GetEnvironmentVariable("SAP_REST_CLIENT");
                var requestHost = message.RequestUri?.Host;

                expectedHost = expectedHost?.Split(':')[0];

                if (errors == SslPolicyErrors.None)
                    return true;

                return string.Equals(requestHost, expectedHost, StringComparison.OrdinalIgnoreCase);
            }
        };
        
        _httpClient = new HttpClient(handler);
    }

    private string? _sapRestApiBaseUrl()
    {
        var baseUrl = Environment.GetEnvironmentVariable("SAP_REST_CLIENT");
        
        string sapRestApiBaseUrl = $"https://{baseUrl}/b1s/v2/";
        return sapRestApiBaseUrl;
    }
    
    
    public List<SapLoginData>? LoginSAPRestApi(string username, string password, string companyDatabase)
    {
        string loginUrl = $"{_sapRestApiBaseUrl()}/Login";
        
        var request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
        var content = new StringContent($"{{\"CompanyDB\": \"{companyDatabase}\", \"UserName\": \"{username}\", \"Password\": \"{password}\"}}",
            null,
            "application/json");
        
        request.Content = content;
        
        var response = _httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();
        
        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        
        // return JsonConvert.DeserializeObject<List<SapLoginData>>(response.Content.ReadAsStringAsync().Result);
        return null;
    }

    public List<SAPData>? GetCustomerAddresses(string username, string password, string companyDatabase, SapLoginData loginData)
    {
        throw new NotImplementedException();
    }

    public void LogoutSAPRestApi(string sessionId)
    {
        throw new NotImplementedException();
    }

}