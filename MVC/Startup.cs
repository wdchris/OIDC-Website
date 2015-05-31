using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using Thinktecture.IdentityModel.Client;
using Thinktecture.IdentityServer.Core;

[assembly: OwinStartup(typeof(MVC.Startup))]

namespace MVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // for the mvc app
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationType = "Cookies"
                });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    Authority = "https://localhost:44304/identity",

                    ClientId = "mvc",
                    RedirectUri = "https://localhost:44302/",
                    ResponseType = "id_token token",
                    Scope = "openid profile roles sampleApi",

                    SignInAsAuthenticationType = "Cookies",
                   // UseTokenLifetime = false,

                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = async n =>
                            {
                                var id = n.AuthenticationTicket.Identity;                               

                                var nid = new ClaimsIdentity(id.AuthenticationType, Constants.ClaimTypes.GivenName, Constants.ClaimTypes.Role);

                                // claims to keep
                                //var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                                //var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                                //var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                                //var roles = id.FindAll(Constants.ClaimTypes.Role);

                                //nid.AddClaim(givenName);
                                //nid.AddClaim(familyName);
                                //nid.AddClaim(sub);
                                //nid.AddClaims(roles);                                

                                var userInfoClient = new UserInfoClient(
                                    new Uri(n.Options.Authority + "/connect/userinfo"),
                                    n.ProtocolMessage.AccessToken);

                                var userInfo = await userInfoClient.GetAsync();
                                userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                                nid.AddClaim(new Claim(Constants.TokenTypes.IdentityToken, n.ProtocolMessage.IdToken));
                                nid.AddClaim(new Claim(Constants.TokenTypes.AccessToken, n.ProtocolMessage.AccessToken));
                                nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                                n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
                            },
                        RedirectToIdentityProvider = async n =>
                            {
                                if(n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                                {
                                    var idTokenHint = n.OwinContext.Authentication.User.FindFirst(Constants.TokenTypes.IdentityToken).Value;
                                    n.ProtocolMessage.IdTokenHint = idTokenHint;
                                }
                            }
                    }
                });

            app.UseResourceAuthorization(new AuthorizationManager());


            //app.Map("/api", apiReq =>
            //    {
            //        apiReq.Use(typeof(CallApiMiddleware));
            //    });
        }
    }

    /// <summary>
    /// A class to intercept calls to an 'API' and transmit them with the bearer token
    /// </summary>
    public class CallApiMiddleware : OwinMiddleware
    {
        public CallApiMiddleware(OwinMiddleware next)
            :base(next)
        { }

        public async override Task Invoke(IOwinContext context)
        {
            var user = context.Authentication.User as ClaimsPrincipal;
            var token = user.FindFirst(Constants.TokenTypes.AccessToken).Value;

            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync("https://localhost:44303/identity");

            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }
    }
}
