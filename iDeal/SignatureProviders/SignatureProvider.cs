using System.Deployment.Internal.CodeSigning;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;

namespace iDeal.SignatureProviders
{
    public class SignatureProvider : ISignatureProvider
    {
        private readonly X509Certificate2 _privateCertificate;
        private readonly X509Certificate2 _publicCertificate;

        public SignatureProvider(X509Certificate2 privateCertificate, X509Certificate2 publicCertificate)
        {
            _privateCertificate = privateCertificate;
            _publicCertificate = publicCertificate;
        }

        /// <summary>
        /// Verifies the digital signature used in status responses from the iDeal API (stored in xml field signature value)
        /// </summary>
        /// <param name="xml">The XML response</param>
        public bool VerifySignature(string xml)
        {
            using (MemoryStream streamIn = new MemoryStream())
            {
                using (StreamWriter w = new StreamWriter(streamIn))
                {
                    w.Write(xml);
                    w.Flush();
                    streamIn.Position = 0;

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(streamIn);
                    SignedXml signedXml = new SignedXml(xmlDoc);
                    XmlNodeList nodeList = xmlDoc.GetElementsByTagName("Signature");
                    if (nodeList.Count != 1)
                    {
                        // TODO: write to log
                        return false;
                    }

                    signedXml.LoadXml((XmlElement)nodeList[0]);

                    CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
                    bool result = signedXml.CheckSignature(_publicCertificate.PublicKey.Key);
                    return result;
                }
            }
        }

        /// <summary>
        /// Addes a signature to the XML before sending it to the iDeal API.
        /// </summary>
        /// <param name="xml">The XML request</param>
        public string SignXml(XDocument xml)
        {
            using (MemoryStream streamIn = new MemoryStream())
            {
                xml.Save(streamIn);
                streamIn.Position = 0;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(streamIn);

                SignedXml signedXml = new SignedXml(xmlDoc);
                signedXml.SigningKey = _privateCertificate.PrivateKey;

                Reference reference = new Reference();
                reference.Uri = "";
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                signedXml.AddReference(reference);

                KeyInfo keyInfo = new KeyInfo();
                KeyInfoName kin = new KeyInfoName();
                kin.Value = _privateCertificate.Thumbprint;
                keyInfo.AddClause(kin);
                signedXml.KeyInfo = keyInfo;

                signedXml.ComputeSignature();
                XmlElement xmlDigitalSignature = signedXml.GetXml();
                xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

                using (MemoryStream sout = new MemoryStream())
                {
                    xmlDoc.Save(sout);
                    sout.Position = 0;
                    using (StreamReader reader = new StreamReader(sout))
                    {
                        string xmlOut = reader.ReadToEnd();
                        return xmlOut;
                    }
                }
            }
        }
    }
}
