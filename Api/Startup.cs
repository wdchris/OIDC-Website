using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Thinktecture.IdentityServer.AccessTokenValidation;
using System.Web.Http.Cors;

[assembly: OwinStartup(typeof(Api.Startup))]

namespace Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
                {
                    Authority = "https://localhost:44304/identity",
                    RequiredScopes = new[] { "sampleApi" }
                });

            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // allow requests from the ajax calls
            config.EnableCors(new EnableCorsAttribute("https://localhost:44302, https://localhost:44304, https://localhost:44303, http://localhost:26210, http://localhost:32411, http://localhost:20754", "*", "*"));

            app.UseWebApi(config);
        }
    }
}
