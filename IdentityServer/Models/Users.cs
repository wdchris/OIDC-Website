using IdentityServer.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Services.InMemory;

namespace IdentityServer
{
    public static class Users
    {
        public const string DefaultPassword = "P@ssword1";

        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "bob",
                    Password = "bob",
                    Subject = "1",

                    Claims = new[]
                    {
                        new Claim(Constants.ClaimTypes.GivenName, "Bob"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Smith"),
                        new Claim(Constants.ClaimTypes.Role, "Geek"),
                        new Claim(Constants.ClaimTypes.Role, "Foo")
                    }
                }
            };
        }

        public static User ToUser(this InMemoryUser user)
        {
            var givenName = user.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.GivenName);
            var familyName = user.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.FamilyName);

            return new User 
            {
                UserName = user.Username,
                FirstName = givenName != null ? givenName.Value : null,
                LastName = familyName != null ? familyName.Value : null
            };
        }
    }
}