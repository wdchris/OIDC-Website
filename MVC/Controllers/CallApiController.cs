using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Client;
using Thinktecture.IdentityServer.Core;

namespace MVC.Controllers
{
    public class CallApiController : Controller
    {
        public async Task<ActionResult> ClientCredentials()
        {
            var response = await GetTokenAsync();
            var result = await CallApi(response.AccessToken);

            return View("ShowCredentials", result);
        }

        [Authorize] // this will force a new access token
        public async Task<ActionResult> UserCredentials()
        {
            var user = User as ClaimsPrincipal;
            var token = user.FindFirst(Constants.TokenTypes.AccessToken).Value;
            var result = await CallApi(token);

            return View("ShowCredentials", result);
        }

        private async Task<TokenResponse> GetTokenAsync()
        {
            var client = new OAuth2Client(
                new Uri("https://localhost:44304/identity/connect/token"),
                "mvc_service", "secret");

            return await client.RequestClientCredentialsAsync("sampleApi");
        }

        private async Task<IEnumerable<KeyValuePair<string,string>>> CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync("https://localhost:44303/identity");

            return JArray.Parse(json).Select(j =>
                {
                    var type = j.Values().ElementAt(0).ToString();
                    var value = j.Values().ElementAt(1).ToString();
                    return new KeyValuePair<string, string>(type, value);
                });                    
        }
    }
}