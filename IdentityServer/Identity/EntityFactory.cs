using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services.InMemory;
using Thinktecture.IdentityServer.EntityFramework;

namespace IdentityServer.Identity
{
    public class EntityFactory
    {
        public static void ConfigureClients(IEnumerable<Client> clients, EntityFrameworkServiceOptions options)
        {
            using (var db = new ClientConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Clients.Any())
                {
                    foreach (var c in clients)
                    {
                        var e = c.ToEntity();
                        db.Clients.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void ConfigureScopes(IEnumerable<Scope> scopes, EntityFrameworkServiceOptions options)
        {
            using (var db = new ScopeConfigurationDbContext(options.ConnectionString, options.Schema))
            {
                if (!db.Scopes.Any())
                {
                    foreach (var s in scopes)
                    {
                        var e = s.ToEntity();
                        db.Scopes.Add(e);
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void ConfigureUsers(IEnumerable<InMemoryUser> users, EntityFrameworkServiceOptions options)
        {
            using (var db = new UserContext(options.ConnectionString))
            {
                using(var store = new UserStore(db))
                {
                    using(var manager = new UserManager(store))
                    {
                        if (!db.Users.Any())
                        {
                            foreach (var u in users)
                            {
                                var result = manager.CreateAsync(u.ToUser(), Users.DefaultPassword).Result;
                                if(result.Succeeded)
                                {
                                    var user = manager.FindByNameAsync(u.Username).Result;
                                    foreach(var claim in u.Claims)
                                    {
                                        result = manager.AddClaimAsync(user.Id, claim).Result;
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}