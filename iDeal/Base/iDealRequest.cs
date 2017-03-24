using System;
using iDeal.SignatureProviders;
using System.Xml.Linq;

namespace iDeal.Base
{
    public abstract class iDealRequest
    {
        protected readonly XNamespace XmlNamespace = "http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1";

        protected string _merchantId;
        protected int _subId;

        /// <summary>
        /// Unique identifier of merchant
        /// </summary>
        public string MerchantId
        {
            get { return _merchantId; }
            protected set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidOperationException("MerchantId does not contain a value");
                if (value.Contains(" "))
                    throw new InvalidOperationException("MerchantId cannot contain whitespaces");
                if (value.Length > 9)
                    throw new InvalidOperationException("MerchantId cannot contain more than 9 characters.");
                _merchantId = value;
            }
        }

        /// <summary>
        /// Sub id of merchant, usually 0
        /// </summary>
        public int MerchantSubId
        {
            get { return _subId; }
            protected set
            {
                if (value < 0 || value > 6)
                    throw new InvalidOperationException("SubId must contain a value ranging from 0 to 6");
                _subId = value;
            }
        }

        /// <summary>
        /// Create datetimestamp of request
        /// </summary>
        public string createDateTimestamp { get; private set; }

        protected abstract XDocument CreateXml();

        public string CreateAndSignXml(ISignatureProvider signatureProvider)
        {
            var xDoc = CreateXml();
            return signatureProvider.SignXml(xDoc);
        }

        protected iDealRequest(string merchantId, int? subId)
        {
            MerchantId = merchantId;
            MerchantSubId = subId ?? 0; // If no sub id is specified, sub id should be 0
            createDateTimestamp = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
        }
    }
}
