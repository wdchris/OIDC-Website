using IdentityManager;
using IdentityManager.AspNetIdentity;
using IdentityManager.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityServer.Identity
{
    public static class IdentityManagerServiceExtensions
    {
        public static void ConfigureIdentityManagerService(this IdentityManagerServiceFactory factory, string connectionString)
        {
            factory.Register(new Registration<UserContext>(resolver => new UserContext(connectionString)));
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<RoleStore>());
            factory.Register(new Registration<UserManager>());
            factory.Register(new Registration<RoleManager>());
            factory.IdentityManagerService = new Registration<IIdentityManagerService, IdentityManagerService>();
        }
    }

    public class IdentityManagerService : AspNetIdentityManagerService<User, string, Role, string>
    {
        public IdentityManagerService(UserManager userMgr, RoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }
    }
}