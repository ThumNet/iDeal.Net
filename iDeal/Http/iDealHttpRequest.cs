using System.IO;
using System.Net;
using System.Text;
using iDeal.Base;
using iDeal.SignatureProviders;

namespace iDeal.Http
{
    public class iDealHttpRequest : IiDealHttpRequest
    {
        public iDealResponse SendRequest(iDealRequest idealRequest, ISignatureProvider signatureProvider, string url, IiDealHttpResponseHandler iDealHttpResponseHandler)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Create request
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentType = "text/xml";
            request.Method = "POST";            
            
            // Set content
            string xml = idealRequest.CreateAndSignXml(signatureProvider);
            var postBytes = Encoding.ASCII.GetBytes(xml);

            // Send
            var requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            // Return result
            var response = (HttpWebResponse)request.GetResponse();            
            return iDealHttpResponseHandler.HandleResponse(new StreamReader(response.GetResponseStream()).ReadToEnd(), signatureProvider);
        }
    }
}
