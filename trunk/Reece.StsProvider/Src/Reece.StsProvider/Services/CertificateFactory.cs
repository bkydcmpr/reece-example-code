using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Reece.StsProvider.Services
{
    public static class CertificateFactory
    {
        public static X509Certificate2 GetCertificate()
        {
            var storeLocationString =  ConfigurationManager.AppSettings["storeLocation"];
            StoreLocation storeLocation;
            if (! Enum.TryParse(storeLocationString, out storeLocation))
            {
                throw new ArgumentException("Invalid store location.");
            }
            var storeName = ConfigurationManager.AppSettings["storeName"];
            var thumbprint = ConfigurationManager.AppSettings["thumbprint"];

            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var certificate = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certificate.Count == 0)
                {
                    throw new Exception("No certificate found.");
                }
                return certificate[0];
            }
            finally 
            {
                store.Close();
            }
        }

        public static X509SigningCredentials GetSigningCredentials()
        {
            return new X509SigningCredentials(GetCertificate());
        }
    }
}