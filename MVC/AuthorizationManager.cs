using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;
using Thinktecture.IdentityServer.Core;

namespace MVC
{
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            switch(context.Resource.First().Value)
            {
                case "ContactDetails":
                    return AuthorizeContactDetails(context);
                default:
                    return Nok();
            }
        }

        private Task<bool> AuthorizeContactDetails(ResourceAuthorizationContext context)
        {
            switch(context.Action.First().Value)
            {
                case "Read":
                    return Eval(context.Principal.HasClaim(Constants.ClaimTypes.Role, "Geek"));
                case "Write":
                    return Eval(context.Principal.HasClaim(Constants.ClaimTypes.Role, "Operator"));
                default:
                    return Nok();
            }
        }
    }
}