using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Reece.StsProvider.Controllers
{
    public class SecurityController : Controller
    {
        private SecurityTokenService _securityTokenService;
        public SecurityTokenService SecurityTokenService
        {
            get { return _securityTokenService ?? ( _securityTokenService = Services.StsFactory.GetSecurityTokenService()); }
            set { _securityTokenService = value; }
        }

        private const string ActionParameter = "wa";
        private const string SigninAction = "wsignin1.0";
        private const string SignoutAction = "wsignout1.0";
        private const string SignoutCleanupAction = "wsignoutcleanup1.0";
        private const string AuthenticationType = "DevelopmentSTS";

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Authorize()
        {
            if (Request.QueryString[ActionParameter] == SigninAction)
            {
                ActionSignon();
            }
            else if (Request.QueryString[ActionParameter] == SignoutAction ||
                     Request.QueryString[ActionParameter] == SignoutCleanupAction)
            {
                return ActionSignout();
            }
            return null;
        }

        private ActionResult ActionSignout()
        {
            var message = WSFederationMessage.CreateFromUri(Request.Url);
            string reply = null;

            if (message.GetType() == typeof (SignOutCleanupRequestMessage))
            {
                reply = ((SignOutCleanupRequestMessage) message).Reply;
            }
            else if (message.GetType() == typeof(SignOutRequestMessage))
            {
                reply = ((SignOutRequestMessage)message).Reply;
            }
            FederatedPassiveSecurityTokenServiceOperations.ProcessSignOutRequest(
                message,
                (ClaimsPrincipal)User,
                reply,
                System.Web.HttpContext.Current.Response);
            return Redirect(reply ?? "/");
        }

        private void ActionSignon()
        {
            var message = (SignInRequestMessage) WSFederationMessage.CreateFromUri(Request.Url);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), AuthenticationType));
            var responseMessage = FederatedPassiveSecurityTokenServiceOperations.ProcessSignInRequest(
                message,
                user,
                SecurityTokenService);
            FederatedPassiveSecurityTokenServiceOperations.ProcessSignInResponse(
                responseMessage, 
                System.Web.HttpContext.Current.Response);
        }
    }
}