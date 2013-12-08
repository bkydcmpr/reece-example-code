using System;
using System.Configuration;
using System.IdentityModel;
using System.IdentityModel.Configuration;

namespace Reece.StsProvider.Services
{
    public static class StsFactory
    {
        public static SecurityTokenService GetSecurityTokenService()
        {
            var config = new SecurityTokenServiceConfiguration(
                ConfigurationManager.AppSettings["stsName"],
                CertificateFactory.GetSigningCredentials())
                         {
                             DefaultTokenLifetime = new TimeSpan(1, 0, 0, 0)
                         };
            return new DeveloperSecurityTokenService(config);
        }
    }
}