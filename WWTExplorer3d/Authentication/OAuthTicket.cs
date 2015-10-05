using System;
using System.Threading.Tasks;
using MicrosoftAccount.WindowsForms;

namespace TerraViewer.Authentication
{
    public class OAuthTicket : IAuthenticationInfo
    {
        public OAuthTicket(string accessToken, DateTimeOffset expirationTime, string refreshToken = null)
        {
            AccessToken = accessToken;
            TokenExpiration = expirationTime;
            TokenType = "Bearer";
            RefreshToken = refreshToken;
        }

        public OAuthTicket(AppTokenResult ticket)
        {
            PopulateOAuthTicket(ticket);
        }

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset TokenExpiration { get; set; }
        public string RefreshToken { get; set; }

        public async Task<bool> RefreshAccessTokenAsync()
        {
            var newTicket = await OAuthAuthenticator.RenewAccessTokenAsync(this);
            PopulateOAuthTicket(newTicket);

            return (newTicket != null);
        }

        private void PopulateOAuthTicket(AppTokenResult newTicket)
        {
            if (null != newTicket)
            {
                AccessToken = newTicket.AccessToken;
                TokenExpiration = DateTimeOffset.Now.AddSeconds(newTicket.AccessTokenExpirationDuration);
                TokenType = newTicket.TokenType;
                RefreshToken = newTicket.RefreshToken;
            }
        }

        public string AuthorizationHeaderValue
        {
            get { return string.Concat(TokenType, " ", AccessToken); }
        }
    }
}
