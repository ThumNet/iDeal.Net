using System.Xml.Linq;

namespace iDeal.SignatureProviders
{
    public interface ISignatureProvider
    {
        /// <summary>
        /// Verifies the digital signature used in status responses from the iDeal API (stored in xml field signature value)
        /// </summary>
        /// <param name="xml">The XML response</param>
        bool VerifySignature(string xml);

        /// <summary>
        /// Addes a signature to the XML before sending it to the iDeal API.
        /// </summary>
        /// <param name="xml">The XML request</param>
        string SignXml(XDocument xml);
    }
}