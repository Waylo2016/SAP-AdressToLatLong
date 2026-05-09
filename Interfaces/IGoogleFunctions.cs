using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Interfaces;

public interface IGoogleFunctions
{
        
        Task<PostGeocodeData?> GetGeocodeDataAsync(SAPData sapData, ApplicationDbContext context);
        Task SaveGeocodeDataAsync(PostGeocodeData postGeocodeData, ApplicationDbContext context);
        bool CheckIfGeocodeDataExists(int docNum, string cardCode, ApplicationDbContext context);
}