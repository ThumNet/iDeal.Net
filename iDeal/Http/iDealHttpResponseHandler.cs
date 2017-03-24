using System.IO;
using System.Xml.Linq;
using iDeal.Base;
using iDeal.Directory;
using iDeal.SignatureProviders;
using iDeal.Status;
using iDeal.Transaction;

namespace iDeal.Http
{
    public class iDealHttpResponseHandler : IiDealHttpResponseHandler
    {
        public iDealResponse HandleResponse(string response, ISignatureProvider signatureProvider)
        {
            var xDocument = XElement.Parse(response);

            if (!signatureProvider.VerifySignature(response))
            {
              throw new InvalidSignatureException();
            }
            switch (xDocument.Name.LocalName)
            {
                case "DirectoryRes":
                    return new DirectoryResponse(xDocument);
                
                case "AcquirerTrxRes":
                    return new TransactionResponse(xDocument);
                
                case "AcquirerStatusRes":
                    return new StatusResponse(xDocument);

                case "AcquirerErrorRes":
                    throw new iDealException(xDocument);

                default:
                    throw new InvalidDataException("Unknown response");
            }
        }
    }
}
