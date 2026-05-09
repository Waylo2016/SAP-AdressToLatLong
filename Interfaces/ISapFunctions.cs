using SAP_AdresToLatLong.Models;

namespace SAP_AdresToLatLong.Interfaces;

public interface ISapFunctions
{
    public List<SapLoginData>? LoginSAPRestApi(string username, string password, string companyDatabase);
    
    public List<SAPData>? GetCustomerAddresses(string username, string password, string companyDatabase, SapLoginData loginData);
    
    public void LogoutSAPRestApi(string sessionId);
}