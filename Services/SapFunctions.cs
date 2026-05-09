using System.Net;
using System.Net.Security;
using Newtonsoft.Json.Linq;
using SAP_AdresToLatLong.Data;
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
            
            // Doing this yucky ew thing because the server I (waylo) am using. It'll be deleted when the company I work for has a proper CA
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

    private string _sapRestApiBaseUrl()
    {
        string? baseUrl = Environment.GetEnvironmentVariable("SAP_REST_CLIENT");
        
        string sapRestApiBaseUrl = $"https://{baseUrl}/b1s/v2/";
        return sapRestApiBaseUrl;
    }
    
    
    public SapCookieData LoginSAPRestApi(SapLoginData loginData)
    {
        try
        {
            string loginUrl = $"{_sapRestApiBaseUrl()}/Login";
        
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, loginUrl);
            StringContent content = new StringContent($"{{\"CompanyDB\": \"{loginData.CompanyDB}\", \"UserName\": \"{loginData.UserName}\", \"Password\": \"{loginData.Password}\"}}",
                null,
                "application/json");
        
            request.Content = content;
        
            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("login:");
            Console.WriteLine(response.StatusCode);
        
            // check the returned cookies from the server and throw them into our cookie jar
            CookieCollection cookies = _httpClientHandler.CookieContainer.GetCookies(new Uri(_sapRestApiBaseUrl()));
            if (cookies == null)        {
                throw new Exception("No cookies received from SAP REST API, login failed.");
            }
            
            SapCookieData cookieData = new SapCookieData
            {
                ROUTEID = cookies["ROUTEID"]!.Value,
                B1SESSION = cookies["B1SESSION"]!.Value
            };

            return cookieData;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public List<SAPData> GetCustomerAddresses(SapLoginData loginData, SapCookieData cookieData)
    {
        string baseUrl = $"{_sapRestApiBaseUrl()}/Orders?$select=DocNum,CardCode,Address,Address2";
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
        request.Headers.Add("Cookie", $"ROUTEID={cookieData.ROUTEID}; B1SESSION={cookieData.B1SESSION}");
        StringContent content = new StringContent($"{{\"CompanyDB\": \"{loginData.CompanyDB}\", \"UserName\": \"{loginData.UserName}\", \"Password\": \"{loginData.Password}\"}}",
            null,
            "application/json");
        
        
        request.Content = content;
        HttpResponseMessage response = _httpClient.SendAsync(request).Result;
        
        
        response.EnsureSuccessStatusCode();
        string responseBody = response.Content.ReadAsStringAsync().Result;

        List<SapApiItem>? items = JObject.Parse(responseBody)["value"]!.ToObject<List<SapApiItem>>();

        List<SAPData>? sapDataList = items?.Select(item => new SAPData
        {
            DocNum = item.DocNum,
            CardCode = item.CardCode,
            BillToAddress = item.Address.Replace("\r", "").Replace("\n", " "),
            SendToAddress = item.Address2?.Replace("\r", "").Replace("\n", " ")
        }).ToList();

        return sapDataList ?? new List<SAPData>();
    }

    public void SaveCustomerAddresses(List<SAPData> sapDataList, ApplicationDbContext context)
    {
        foreach (var sapData in sapDataList)
        {
            // Check if the record already exists in the database
            bool exists = context.SAPData.Any(s => s.DocNum == sapData.DocNum && s.CardCode == sapData.CardCode);

            if (!exists)
            {
                context.SAPData.Add(sapData);
            }
        }

        context.SaveChanges();
    }

    public void LogoutSAPRestApi(SapCookieData cookieData)
    {
        try
        {
            string logoutUrl = $"{_sapRestApiBaseUrl()}/Logout";
        
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, logoutUrl);
            request.Headers.Add("Cookie", $"ROUTEID={cookieData.ROUTEID}; B1SESSION={cookieData.B1SESSION}");
        
            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            Console.WriteLine("logout:");
            Console.WriteLine(response.StatusCode);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}