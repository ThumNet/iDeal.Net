using System.Xml.Linq;
using iDeal.Base;

namespace iDeal.Directory
{
    public class DirectoryRequest : iDealRequest
    {
        public DirectoryRequest(string merchantId, int? subId)
            : base(merchantId, subId)
        {
        }

        /// <summary>
        /// Creates xml representation of directory request
        /// </summary>
        protected override XDocument CreateXml()
        {
            return new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement(XmlNamespace + "DirectoryReq",
                        new XAttribute("version", "3.3.1"),
                        new XElement(XmlNamespace + "createDateTimestamp", createDateTimestamp),
                        new XElement(XmlNamespace + "Merchant",
                            new XElement(XmlNamespace + "merchantID", MerchantId.PadLeft(9, '0')),
                            new XElement(XmlNamespace + "subID", "0")
                        )
                    )
                );
        }
    }
}