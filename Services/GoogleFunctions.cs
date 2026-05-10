using System.Text.Json;
using System.Text.RegularExpressions;
using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Helpers;
using SAP_AdresToLatLong.Interfaces;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Services;

public class GoogleFunctions : IGoogleFunctions
{
    private readonly string _googleApiLink = "https://geocode.googleapis.com/v4/geocode/address";
    private readonly string _googleApiKey = SecretParser.GetGeocodingApi();
    private readonly HttpClient _httpClient = new();
    
    public bool CheckIfGeocodeDataExists(int docNum, string cardCode, ApplicationDbContext context)
    {
        bool exists = context.PostGeocodeData.Any(x => x.DocNum == docNum && x.CardCode == cardCode);
        return exists;
    }

    public async Task<PostGeocodeData?> GetGeocodeDataAsync(SAPData sapData, ApplicationDbContext context)
    {
        string url = BuildGoogleApiUrl(sapData.DocNum, context);
        
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Goog-Api-Key", _googleApiKey);
        request.Headers.Add("X-Goog-FieldMask", "results.location");
        
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        string responseBody = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseBody);
        var location = doc.RootElement
            .GetProperty("results")[0]
            .GetProperty("location");

        var postGeocodeData = new PostGeocodeData
        {
            DocNum = sapData.DocNum,
            CardCode = sapData.CardCode,
            Latitude = location.GetProperty("latitude").GetDecimal(),
            Longitude = location.GetProperty("longitude").GetDecimal()
        };
        return postGeocodeData;
    }

    public async Task SaveGeocodeDataAsync(PostGeocodeData postGeocodeData, ApplicationDbContext context)
    {
        if (!CheckIfGeocodeDataExists(postGeocodeData.DocNum, postGeocodeData.CardCode, context))
        {
            context.PostGeocodeData.Add(postGeocodeData);
            await context.SaveChangesAsync();
        }
    }
    
    private string BuildGoogleApiUrl(int docNum, ApplicationDbContext context)
    {
        var result = context.SAPData
            .Where(x => x.DocNum == docNum)
            .AsEnumerable()
            .Select(x => new {
                Source = !string.IsNullOrWhiteSpace(x.SendToAddress) ? "SendTo" : "BillTo",
                Address = Regex.Replace(
                    !string.IsNullOrWhiteSpace(x.SendToAddress) ? x.SendToAddress : x.BillToAddress,
                    @"\s+", "+")
            })
            .First();

        return $"{_googleApiLink}/{result.Address}";
    }
}