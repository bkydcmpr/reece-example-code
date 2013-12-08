using System;
using System.Configuration;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Reece.StsProvider.Services;

namespace Reece.StsProvider.Controllers
{
    public class ConfigurationController : Controller
    {
        public ActionResult FederationMetadata()
        {
            var endpoint = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            var entityDescriptor = new EntityDescriptor(new EntityId(ConfigurationManager.AppSettings["stsName"]))
                                   {
                                       SigningCredentials = CertificateFactory.GetSigningCredentials()
                                   };

            var roleDescriptor = new SecurityTokenServiceDescriptor();
            roleDescriptor.Contacts.Add(new ContactPerson(ContactType.Administrative));

            var clause = new X509RawDataKeyIdentifierClause(CertificateFactory.GetCertificate());
            var securityKeyIdentifier = new SecurityKeyIdentifier(clause);
            var signingKey = new KeyDescriptor(securityKeyIdentifier) {Use = KeyType.Signing};
            roleDescriptor.Keys.Add(signingKey);

            var endpointAddress =
                new System.IdentityModel.Protocols.WSTrust.EndpointReference(endpoint + "/Security/Authorize");

            roleDescriptor.PassiveRequestorEndpoints.Add(endpointAddress);
            roleDescriptor.SecurityTokenServiceEndpoints.Add(endpointAddress);

            roleDescriptor.ProtocolsSupported.Add(new Uri("http://docs.oasis-open.org/wsfed/federation/200706"));

            entityDescriptor.RoleDescriptors.Add(roleDescriptor);

            var serializer = new MetadataSerializer();
            var settings = new XmlWriterSettings {Encoding = Encoding.UTF8};

            var memoryStream = new MemoryStream();
            var writer = XmlWriter.Create(memoryStream, settings);
            serializer.WriteMetadata(writer,entityDescriptor);
            writer.Flush();

            var content = Content(Encoding.UTF8.GetString(memoryStream.GetBuffer()), "text/xml");
            writer.Dispose();

            return content;
        }
	}
}