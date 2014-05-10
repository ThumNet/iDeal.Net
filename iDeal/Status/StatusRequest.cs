﻿using System;
using System.Xml.Linq;
using iDeal.Base;
using iDeal.SignatureProviders;

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
                if (value.IsNullEmptyOrWhiteSpace() || value.Length != 16)
                    throw new InvalidOperationException("TransactionId must contain exactly 16 characters");
                _transactionId = value;
            }
        }

        public override string MessageDigest
        {
            get
            {
                return createDateTimestamp +
                       MerchantId.PadLeft(9, '0') +
                       MerchantSubId +
                       TransactionId;
            }
        }

        public StatusRequest(string merchantId, int? subId, string transactionId)
        {
            MerchantId = merchantId;
            MerchantSubId = subId ?? 0; // If no sub id is specified, sub id should be 0
            TransactionId = transactionId;
        }

        public override string ToXml(ISignatureProvider signatureProvider)
        {
            XNamespace xmlNamespace = "http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1";

            var directoryRequestXmlMessage =
                new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    new XElement(xmlNamespace + "AcquirerStatusReq",
                        new XAttribute("version", "3.3.1"),
                        new XElement(xmlNamespace + "createDateTimestamp", createDateTimestamp),
                        new XElement(xmlNamespace + "Merchant",
                            new XElement(xmlNamespace + "merchantID", MerchantId.PadLeft(9, '0')),
                            new XElement(xmlNamespace + "subID", MerchantSubId)
                        ),
                        new XElement(xmlNamespace + "Transaction",
                            new XElement(xmlNamespace + "transactionID", TransactionId)
                        )
                    )
                );

            return signatureProvider.SignXml(directoryRequestXmlMessage);
        }
    }
}
