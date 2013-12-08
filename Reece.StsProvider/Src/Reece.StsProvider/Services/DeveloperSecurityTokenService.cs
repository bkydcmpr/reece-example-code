using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Reece.StsProvider.Services
{
    public class DeveloperSecurityTokenService : SecurityTokenService
    {
        public DeveloperSecurityTokenService(SecurityTokenServiceConfiguration securityTokenServiceConfiguration) : base(securityTokenServiceConfiguration)
        {
        }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            var scope = new Scope(request.AppliesTo.Uri.AbsoluteUri,
                this.SecurityTokenServiceConfiguration.SigningCredentials)
                        {
                            TokenEncryptionRequired = false
                        };
            scope.ReplyToAddress = string.IsNullOrWhiteSpace(request.ReplyTo)
                ? scope.AppliesToAddress
                : scope.ReplyToAddress;
            return scope;
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            var config = ClaimsConfiguration.ConfigurationFactory();
            var realm = request.AppliesTo.Uri.AbsoluteUri;
            var claims = new List<Claim>();
            
            claims.AddRange(GetClaimsForRealm("common", config));
            claims.AddRange(GetClaimsForRealm(realm,config));

            return new ClaimsIdentity(claims);
        }

        private IEnumerable<Claim> GetClaimsForRealm(string realm, ClaimsConfiguration config)
        {
            var claimsSection = config.RelyingParties[realm];
            if (claimsSection != null)
            {
                return claimsSection.Claims.Cast<RelyingPartyClaim>()
                    .Select(claim => new Claim(claim.Claim, claim.Value));
            }
            return new List<Claim>();
        }
    }
}