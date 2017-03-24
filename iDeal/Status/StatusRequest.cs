using System;
using System.Xml.Linq;
using iDeal.Base;

namespace iDeal.Status
{
    public class StatusRequest : iDealRequest
    {
        private string _transactionId;

        /// <summary>
        /// Unique 16 digits number, assigned by the acquirer to the transaction
        /// </summary>
        public string TransactionId
        {
            get { return _transactionId; }
            private set
            {
                if (string.IsNullOrEmpty(value) || value.Length != 16)
                    throw new InvalidOperationException("TransactionId must contain exactly 16 characters");
                _transactionId = value;
            }
        }

        public StatusRequest(string merchantId, int? subId, string transactionId)
            : base (merchantId, subId)
        {
            TransactionId = transactionId;
        }

        protected override XDocument CreateXml()
        {
            return new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement(XmlNamespace + "AcquirerStatusReq",
                        new XAttribute("version", "3.3.1"),
                        new XElement(XmlNamespace + "createDateTimestamp", createDateTimestamp),
                        new XElement(XmlNamespace + "Merchant",
                            new XElement(XmlNamespace + "merchantID", MerchantId.PadLeft(9, '0')),
                            new XElement(XmlNamespace + "subID", MerchantSubId)
                        ),
                        new XElement(XmlNamespace + "Transaction",
                            new XElement(XmlNamespace + "transactionID", TransactionId)
                        )
                    )
                );
        }
    }
}
