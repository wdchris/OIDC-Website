using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Models;

namespace IdentityServer
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
            {
                new Scope
                {
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(Constants.ClaimTypes.Role)
                    }
                },
                new Scope
                {
                    Enabled = true,
                    Name = "sampleApi",
                    Description = "Access to a sample API",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(Constants.ClaimTypes.Role)
                    }
                }
            };

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}