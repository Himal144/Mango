namespace Mango.Web.Service.IService
{
    public interface ITokenService
    {
        public void AddToken(string token);
        public void RemoveToken(string token);
        public string GetToken();
    }
}
