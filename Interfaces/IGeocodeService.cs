using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Interfaces;

public interface IGeocodeService
{
    public Task<List<PostGeocodeData?>> GetGeocodeDataBatchAsync(IEnumerable<SAPData> sapDataList, ApplicationDbContext context);
}