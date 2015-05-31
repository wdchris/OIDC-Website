using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "MVC Client",
                    ClientId = "mvc",
                    Flow = Flows.Implicit,
                    
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44302/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:44302/"
                    }

                    // this will affect how long access to api will last                   
                    //AccessTokenLifetime = 300

                    // this will affect how long mvc cookie/auth will last before getting claims etc.. again
                    //IdentityTokenLifetime = 10
                },
                new Client
                {
                    Enabled = true,
                    ClientName = "MVC Client (service communication)",
                    ClientId = "mvc_service",
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret("secret".Sha256())
                    },
                    Flow = Flows.ClientCredentials
                }
            };
        }
    }
}