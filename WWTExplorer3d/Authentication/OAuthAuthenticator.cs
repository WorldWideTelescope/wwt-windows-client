using System.Threading.Tasks;
using MicrosoftAccount.WindowsForms;

namespace TerraViewer.Authentication
{
    public static class OAuthAuthenticator
    {
        //staging - wwtstaging.azurewebsites.net
        //private const string msa_client_id = "000000004813DA7C";
        //private const string msa_client_secret = "-l6FXTdT1K8xLkBM8mVPed9jJdMy-wLz";

        //prod - openwwt.org
        private const string msa_client_id = "000000004015657B";
        private const string msa_client_secret = "Ewb5SNjRBfzBDy5Ityx-nO4-kPAIwLrH";


        public static async Task<OAuthTicket> SignInToMicrosoftAccount(System.Windows.Forms.IWin32Window parentWindow)
        {
            var oldRefreshToken = Properties.Settings.Default.RefreshToken;
            AppTokenResult appToken = null;
            if (!string.IsNullOrEmpty(oldRefreshToken))
            {
                appToken = await MicrosoftAccountOAuth.RedeemRefreshTokenAsync(msa_client_id, msa_client_secret, oldRefreshToken);
                
            }

            if (null == appToken)
            {
                appToken = await MicrosoftAccountOAuth.LoginAuthorizationCodeFlowAsync(msa_client_id,
                    msa_client_secret,
                    new[] { /*"wl.offline_access", "wl.basic", */"wl.signin", "wl.emails"/*, "onedrive.readwrite"*/ });
            }                       

            if (null != appToken)
            {
                SaveRefreshToken(appToken.RefreshToken);

                return new OAuthTicket(appToken);
            }

            return null;
        }

        private static void SaveRefreshToken(string refreshToken)
        {
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var settings = Properties.Settings.Default;
                settings.RefreshToken = refreshToken;
                settings.Save();
            }
        }

        public static async Task<AppTokenResult> RenewAccessTokenAsync(OAuthTicket ticket)
        {
            var oldRefreshToken = ticket.RefreshToken;
            AppTokenResult appToken = null;

            if (!string.IsNullOrEmpty(oldRefreshToken))
            {
                appToken = await MicrosoftAccountOAuth.RedeemRefreshTokenAsync(msa_client_id, msa_client_secret, oldRefreshToken);
                SaveRefreshToken(appToken.RefreshToken);
            }

            return appToken;
        }
    }
}
