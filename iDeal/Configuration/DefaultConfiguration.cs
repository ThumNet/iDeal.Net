using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace iDeal.Configuration
{
    /// <summary>
    /// Default configuration will load information from .config file
    /// </summary>
    public class DefaultConfiguration : IConfiguration
    {
        public string MerchantId { get; private set; }
        public int MerchantSubId { get; private set; }
        public string AcquirerUrl { get; private set; }
        public X509Certificate2 AcceptantCertificate { get; private set; }
        public X509Certificate2 AcquirerCertificate { get; private set; }

        public DefaultConfiguration(IConfigurationSectionHandler configurationSectionHandler)
        {
            MerchantId = configurationSectionHandler.MerchantId;
            MerchantSubId = configurationSectionHandler.MerchantSubId;
            AcquirerUrl = configurationSectionHandler.AcquirerUrl;

            // Retrieve acceptant's certificate
            if (!string.IsNullOrWhiteSpace(configurationSectionHandler.AcceptantCertificateFilename))
            {
                // Retrieve certificate from file
                if (string.IsNullOrWhiteSpace(configurationSectionHandler.AcceptantCertificatePassword))
                    throw new ConfigurationErrorsException("Password is required when acceptant's certificate is loaded from filesystem");

                AcceptantCertificate = GetCertificateFromFile(configurationSectionHandler.AcceptantCertificateFilename, configurationSectionHandler.AcceptantCertificatePassword);
            }
            else if (configurationSectionHandler.AcceptantCertificateStoreLocation != null)
            {
                // Retrieve certificate from certificate store
                if (string.IsNullOrWhiteSpace(configurationSectionHandler.AcceptantCertificateStoreName))
                    throw new ConfigurationErrorsException("Acceptant's certificate store name is required when loading certificate from the certificate store");

                if (string.IsNullOrWhiteSpace(configurationSectionHandler.AcceptantCertificateThumbprint))
                    throw new ConfigurationErrorsException("Acceptant's certificate thumbprint is required when loading certificate from the certificate store");

                AcceptantCertificate = GetCertificateFromStore(configurationSectionHandler.AcceptantCertificateStoreLocation.Value, configurationSectionHandler.AcceptantCertificateStoreName, configurationSectionHandler.AcceptantCertificateThumbprint);
            }
            else
            {
                // Neither filename nor store location is specified
                throw new ConfigurationErrorsException("You should either specify a filename or a certificate store location to specify the acceptant's certificate.");
            }

            // Retrieve acquirer's certificate
            if (!string.IsNullOrWhiteSpace(configurationSectionHandler.AcquirerCertificateFilename))
            {
                // Retrieve certificate from file
                AcquirerCertificate = GetCertificateFromFile(configurationSectionHandler.AcquirerCertificateFilename, null);
            }
            else if (configurationSectionHandler.AcquirerCertificateStoreLocation != null)
            {
                // Retrieve certificate from certificate store
                if (string.IsNullOrWhiteSpace(configurationSectionHandler.AcquirerCertificateStoreName))
                    throw new ConfigurationErrorsException("Acquirer's certificate store name is required when loading certificate from the certificate store");

                if (string.IsNullOrWhiteSpace(configurationSectionHandler.AcquirerCertificateThumbprint))
                    throw new ConfigurationErrorsException("Acquirer's certificate thumbprint is required when loading certificate from the certificate store");

                AcquirerCertificate = GetCertificateFromStore(configurationSectionHandler.AcquirerCertificateStoreLocation.Value, configurationSectionHandler.AcquirerCertificateStoreName, configurationSectionHandler.AcquirerCertificateThumbprint);
            }
            else
            {
                // Neither filename nor store location is specified
                throw new ConfigurationErrorsException("You should either specify a filename or a certificate store location to specify the acquirer's certificate.");
            }
        }

        private static X509Certificate2 GetCertificateFromFile(string relativePath, string password)
        {
            try
            {
                var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                return password != null ? new X509Certificate2(absolutePath, password, X509KeyStorageFlags.MachineKeySet) : new X509Certificate2(absolutePath);
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException("Could not load certificate file", exception);
            }
        }

        private static X509Certificate2 GetCertificateFromStore(StoreLocation storeLocation, string storeName, string thumbprint)
        {
            try
            {
                var certificateStore = new X509Store(storeName, storeLocation);
                certificateStore.Open(OpenFlags.OpenExistingOnly);

                foreach (var certificate in certificateStore.Certificates)
                {
                    if (certificate.Thumbprint.Trim().ToUpper() == thumbprint.Replace(" ", "").ToUpper())
                    {
                        return certificate;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ConfigurationErrorsException("Could not retrieve certificate from store " + storeName, exception);
            }

            throw new ConfigurationErrorsException("Certificate with thumbprint '" + thumbprint + "' not found");
        }
    }
}
