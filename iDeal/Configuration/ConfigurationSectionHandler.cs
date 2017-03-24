using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace iDeal.Configuration
{
    public class ConfigurationSectionHandler : ConfigurationSection, IConfigurationSectionHandler
    {
        public string MerchantId
        {
            get { return Merchant.Id; }
        }

        public int MerchantSubId
        {
            get { return Merchant.SubId; }
        }

        public string AcquirerUrl
        {
            get { return Acquirer.Url; }
        }

        public string AcceptantCertificateThumbprint
        {
            get { return AcceptantCertificate.Thumbprint; }
        }

        public StoreLocation? AcceptantCertificateStoreLocation
        {
            get { return AcceptantCertificate.StoreLocation; }
        }

        public string AcceptantCertificateStoreName
        {
            get { return AcceptantCertificate.StoreName; }
        }

        public string AcceptantCertificateFilename
        {
            get { return AcceptantCertificate.Filename; }
        }

        public string AcceptantCertificatePassword
        {
            get { return AcceptantCertificate.Password; }
        }

        public string AcquirerCertificateThumbprint
        {
            get { return AcquirerCertificate.Thumbprint; }
        }

        public StoreLocation? AcquirerCertificateStoreLocation
        {
            get { return AcquirerCertificate.StoreLocation; }
        }

        public string AcquirerCertificateStoreName
        {
            get { return AcquirerCertificate.StoreName; }
        }

        public string AcquirerCertificateFilename
        {
            get { return AcquirerCertificate.Filename; }
        }

       [ConfigurationProperty("merchant")]
        public MerchantElement Merchant
        {
            get
            {
                return (MerchantElement)this["merchant"];
            }
        }

        [ConfigurationProperty("acquirer")]
        public AcquirerElement Acquirer
        {
            get
            {
                return (AcquirerElement)this["acquirer"];
            }
        }

        [ConfigurationProperty("acceptantCertificate")]
        public AcceptantCertificateElement AcceptantCertificate
        {
            get
            {
                return (AcceptantCertificateElement)this["acceptantCertificate"];
            }
        }

        [ConfigurationProperty("acquirerCertificate")]
        public AcquirerCertificateElement AcquirerCertificate
        {
            get
            {
                return (AcquirerCertificateElement)this["acquirerCertificate"];
            }
        }
    }

    public class MerchantElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get
            {
                return (string)this["id"];
            }
        }

        [ConfigurationProperty("subId", IsRequired = false)]
        public int SubId
        {
            get { return (int) this["subId"]; }
        }
    }

    public class AcquirerElement : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get
            {
                return (string)this["url"];
            }
        }
    }

    public class AcceptantCertificateElement : ConfigurationElement
    {
        [ConfigurationProperty("thumbprint", IsRequired = false)]
        public string Thumbprint
        {
            get
            {
                return (string)this["thumbprint"];
            }
        }

        [ConfigurationProperty("storeLocation", IsRequired = false)]
        public StoreLocation? StoreLocation
        {
            get
            {
                return (StoreLocation?)this["storeLocation"];
            }
        }

        [ConfigurationProperty("storeName", IsRequired = false)]
        public string StoreName
        {
            get
            {
                return (string)this["storeName"];
            }
        }

        [ConfigurationProperty("filename", IsRequired = false)]
        public string Filename
        {
            get
            {
                return (string)this["filename"];
            }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
        }
    }

    public class AcquirerCertificateElement : ConfigurationElement
    {
        [ConfigurationProperty("thumbprint", IsRequired = false)]
        public string Thumbprint
        {
            get
            {
                return (string)this["thumbprint"];
            }
        }

        [ConfigurationProperty("storeLocation", IsRequired = false)]
        public StoreLocation? StoreLocation
        {
            get
            {
                return (StoreLocation?)this["storeLocation"];
            }
        }

        [ConfigurationProperty("storeName", IsRequired = false)]
        public string StoreName
        {
            get
            {
                return (string)this["storeName"];
            }
        }

        [ConfigurationProperty("filename", IsRequired = false)]
        public string Filename
        {
            get
            {
                return (string)this["filename"];
            }
        }
    }
}