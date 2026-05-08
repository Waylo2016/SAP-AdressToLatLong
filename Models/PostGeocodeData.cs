using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace SAP_AdresToLatLong.Models;

public class PostGeocodeData
{
    public int DocNum { get; set; }
    public int CardCode { get; set; }
    
    [Decimal (9,6)]
    public decimal Latitude { get; set; }
    [Decimal (10,6)]
    public decimal Longitude { get; set; }

    public SAPData SAPData { get; set; }
}