using SAP_AdresToLatLong.Data;
using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Interfaces;

public interface ISapApiClient
{
    public SapCookieData LoginSAPRestApi(SapLoginData loginData);
    
    public List<SAPData> GetCustomerAddresses(SapCookieData cookieData);
    
    public void SaveCustomerAddresses(List<SAPData> sapDataList, ApplicationDbContext context);
    
    public void LogoutSAPRestApi(SapCookieData cookieData);
    
    public List<SAPData> GetCustomerAddressesOfToday(SapCookieData cookieData);
}