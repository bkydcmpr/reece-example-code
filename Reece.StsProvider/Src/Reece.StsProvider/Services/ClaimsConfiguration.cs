using System.Configuration;

namespace Reece.StsProvider.Services
{
    public class ClaimsConfiguration : ConfigurationSection
    {
        public static ClaimsConfiguration ConfigurationFactory()
        {
            return ConfigurationManager.GetSection("reece.ClaimsConfiguration") as ClaimsConfiguration;
        }

        [ConfigurationProperty("relyingParties", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof (RelyingPartyCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RelyingPartyCollection RelyingParties
        {
            get { return (RelyingPartyCollection) this["relyingParties"]; }
        }
    }

    public class RelyingPartyCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RelyingParty();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RelyingParty) element).Realm;
        }

        public new RelyingParty this[string index]
        {
            get { return (RelyingParty) base.BaseGet(index); }
        }
    }

    public class RelyingParty : ConfigurationElement
    {
        [ConfigurationProperty("realm")]
        public string Realm
        {
            get { return (string) this["realm"]; }
            set { this["realm"] = value; }
        }

        [ConfigurationProperty("claims")]
        [ConfigurationCollection(typeof (RelyingPartyClaim),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public RelyingPartyClaimsCollection Claims
        {
            get { return (RelyingPartyClaimsCollection) this["claims"]; }
        }
    }

    public class RelyingPartyClaimsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RelyingPartyClaim();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RelyingPartyClaim)element).Key;
        }

        public new RelyingPartyClaim this[string index]
        {
            get { return (RelyingPartyClaim)base.BaseGet(index); }
        }
    }

    public class RelyingPartyClaim : ConfigurationElement
    {
        [ConfigurationProperty("key")]
        public string Key
        {
            get { return (string) this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("claim")]
        public string Claim
        {
            get { return (string)this["claim"]; }
            set { this["claim"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}