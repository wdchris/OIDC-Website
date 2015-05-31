using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security.Google;
using Thinktecture.IdentityServer.Core.Services.Default;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.EntityFramework;
using IdentityServer.Identity;
using System.Configuration;
using IdentityManager.Configuration;
using IdentityServer.Identity;

[assembly: OwinStartup(typeof(IdentityServer.Startup))]

namespace IdentityServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // for the identity server itself
            app.Map("/identity", idsrvApp =>
            {
                var factory = ConfigureEFStores();
                //var factory = ConfigureInMemoryStores();


                // if we need to authenticate via CORS
                //factory.CorsPolicyService = new Registration<ICorsPolicyService>(new DefaultCorsPolicyService
                //{
                //    AllowedOrigins = new[] { "https://localhost:44302", "https://localhost:44304", "https://localhost:44303", "http://localhost:26210", "http://localhost:32411", "http://localhost:20754" }
                //});


                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = LoadCertificate(),

                    Factory = factory,

                    AuthenticationOptions = new Thinktecture.IdentityServer.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders
                    }
                });
            });


            // this bit is for all the identity manager stuff......
            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureIdentityManagerService(ConfigurationManager.ConnectionStrings["IdSvr3Config"].ConnectionString);

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory
                });
            });
        }

        private IdentityServerServiceFactory ConfigureInMemoryStores()
        {
            return InMemoryFactory.Create(
                    users: Users.Get(),
                    clients: Clients.Get(),
                    scopes: Scopes.Get());
        }

        private IdentityServerServiceFactory ConfigureEFStores()
        {
            var connString = ConfigurationManager.ConnectionStrings["IdSvr3Config"].ConnectionString;

            var efConfig = new EntityFrameworkServiceOptions
            {
                ConnectionString = connString
            };

            // client and scope stores...
            EntityFactory.ConfigureClients(Clients.Get(), efConfig);
            EntityFactory.ConfigureScopes(Scopes.Get(), efConfig);

            var factory = new IdentityServerServiceFactory();
            factory.RegisterConfigurationServices(efConfig);
            factory.RegisterOperationalServices(efConfig);

            // identity user service
            EntityFactory.ConfigureUsers(Users.Get(), efConfig);

            factory.UserService = new Thinktecture.IdentityServer.Core.Configuration.Registration<IUserService, UserService>();
            factory.Register(new Thinktecture.IdentityServer.Core.Configuration.Registration<UserManager>());
            factory.Register(new Thinktecture.IdentityServer.Core.Configuration.Registration<UserStore>());
            factory.Register(new Thinktecture.IdentityServer.Core.Configuration.Registration<UserContext>(resolver => new UserContext(connString)));

            return factory;
        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "myid",
                ClientSecret = "mysecret"
            });
        }

        private X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
