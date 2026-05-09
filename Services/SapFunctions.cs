using System.Net;
using System.Net.Security;
using Newtonsoft.Json;
using SAP_AdresToLatLong.Interfaces;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Services;

public class SapFunctions : ISapFunctions
{
    private readonly HttpClient _httpClient;
    private readonly HttpClientHandler _httpClientHandler;
    public SapFunctions()
    {
        _httpClientHandler = new HttpClientHandler
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
        
        _httpClient = new HttpClient(_httpClientHandler);
    }

    private string? _sapRestApiBaseUrl()
    {
        var baseUrl = Environment.GetEnvironmentVariable("SAP_REST_CLIENT");
        
        string sapRestApiBaseUrl = $"https://{baseUrl}/b1s/v2/";
        return sapRestApiBaseUrl;
    }
    
    
    public SapLoginData LoginSAPRestApi(string username, string password, string companyDatabase)
    {
        string loginUrl = $"{_sapRestApiBaseUrl()}/Login";
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
        StringContent content = new StringContent($"{{\"CompanyDB\": \"{companyDatabase}\", \"UserName\": \"{username}\", \"Password\": \"{password}\"}}",
            null,
            "application/json");
        
        request.Content = content;
        
        HttpResponseMessage response = _httpClient.SendAsync(request).Result;
        response.EnsureSuccessStatusCode();

        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        Console.WriteLine(response.StatusCode);
        // check the returned cookies from the server
        CookieCollection cookies = _httpClientHandler.CookieContainer.GetCookies(new Uri(_sapRestApiBaseUrl()));
        SapLoginData loginData = new SapLoginData
        {
            ROUTEID = cookies["ROUTEID"]?.Value,
            B1SESSION = cookies["B1SESSION"]?.Value
        };

        return loginData;
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