namespace SAP_AdresToLatLong.Models;

public class SAPData
{
    public int DocNum { get; set; }
    public string CardCode { get; set; }
    public string? BillToAddress { get; set; }
    public string? SendToAddress { get; set; }
    public PostGeocodeData PostGeocodeData { get; set; }
}