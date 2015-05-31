using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityServer.Core;

namespace MVC.Controllers
{
    public class AjaxController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult AccessToken()
        {
            /* 
             * the authorize attribute here will ensure we are logged in correctly.
             * if the we keep the default idserver settings, access token and auth should
             * expire at the same time so we shouldn't need to refresh manually
             */

            var user = User as ClaimsPrincipal;
            var token = user.FindFirst(Constants.TokenTypes.AccessToken).Value;

            return Json(token, JsonRequestBehavior.AllowGet);
        }
    }
}