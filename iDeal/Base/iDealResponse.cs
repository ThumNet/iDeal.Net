using System.Xml.Linq;

namespace iDeal.Base
{
    public abstract class iDealResponse
    {
        protected readonly XNamespace XmlNamespace = "http://www.idealdesk.com/ideal/messages/mer-acq/3.3.1";
        
        public int AcquirerId { get; protected set; }

        public string createDateTimestamp { get; protected set; }

        protected iDealResponse(XElement xDocument)
        {
            createDateTimestamp = xDocument.Element(XmlNamespace + "createDateTimestamp").Value;
            AcquirerId = (int)xDocument.Element(XmlNamespace + "Acquirer").Element(XmlNamespace + "acquirerID");
        }
    }
}
