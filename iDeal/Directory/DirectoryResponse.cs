using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using iDeal.Base;

namespace iDeal.Directory
{
    public class DirectoryResponse : iDealResponse
    {
        private readonly IList<Issuer> _issuers = new List<Issuer>();

        public string DirectoryDateTimeStamp { get; private set; }

        public DateTime DirectoryDateTimeStampLocalTime { get { return DateTime.Parse(DirectoryDateTimeStamp); } }

        public IList<Issuer> Issuers
        {
            get { return new ReadOnlyCollection<Issuer>(_issuers); }
        }

        public DirectoryResponse(XElement xDocument)
            : base(xDocument)
        {
            DirectoryDateTimeStamp = xDocument.Element(XmlNamespace + "Directory").Element(XmlNamespace + "directoryDateTimestamp").Value;
          
            // Get list of countries
            foreach (var country in xDocument.Element(XmlNamespace + "Directory").Elements(XmlNamespace + "Country"))
            {
              // Get list of issuers
              foreach (var issuer in country.Elements(XmlNamespace + "Issuer"))
              {
                  _issuers.Add(
                          new Issuer(
                                  issuer.Element(XmlNamespace + "issuerID").Value,
                                  issuer.Element(XmlNamespace + "issuerName").Value
                              )
                      );
              }
            }
        }
    }
}