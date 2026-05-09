using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Interfaces;

public interface IGoogleFunctions
{
        
        public bool CheckIfGeocodeDataExists (int docNum, int cardCode);
        
        public List<PostGeocodeData> GetGeocodeData (string address, int docNum, int cardCode);
        
        public void SaveGeocodeData (PostGeocodeData postGeocodeData, ApplicationDbContext context);
}