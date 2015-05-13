using System;
using System.Threading.Tasks;

namespace MSAuth
{
    public interface IAuthenticationInfo
    {
        string AccessToken { get; }

        string RefreshToken { get; }

        string TokenType { get; }

        DateTimeOffset TokenExpiration { get; }

        Task<bool> RefreshAccessTokenAsync();

        string AuthorizationHeaderValue { get; }
    }

    //public class MicrosoftAccountAuthenticationInfo : AuthenticationInfo
    //{
    //    public string AccessToken { get; set; }

    //    public DateTime TokenExpiration { get; set; }

    //    public string RefreshToken { get; set; }

    //    public async Task RefreshAccessTokenAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public MicrosoftAccountAuthenticationInfo()
    //    {
    //        TokenExpiration = DateTime.MaxValue;
    //    }
    //}

    //public class AzureActiveDirectoryAuthenticationInfo : AuthenticationInfo
    //{
    //    public string AccessToken { get; set; }

    //    public DateTime TokenExpiration { get; set; }

    //    public string RefreshToken { get; set; }

    //    public async Task RefreshAccessTokenAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public AzureActiveDirectoryAuthenticationInfo()
    //    {
    //        TokenExpiration = DateTime.MaxValue;
    //    }
    //}
}
