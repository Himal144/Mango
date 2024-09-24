namespace Mango.Web.Service.IService
{
    public interface ITokenService
    {
        public void AddToken(string token);
        public void RemoveToken();
        public string GetToken();
    }
}
