using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Api
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ApiController
    {
        public IHttpActionResult Get()
        {
            var user = User as ClaimsPrincipal;
            var claims = user.Claims.Select(c => new
                {
                    type = c.Type,
                    value = c.Value
                });

            return Json(claims);
        }
    }
}