namespace iDeal.Base
{
    public class InvalidSignatureException : iDealException
    {
        public InvalidSignatureException()
        {

            ErrorCode = "SIG0001";
            ErrorMessage = "Invalid signature";
            ErrorDetail = "Unable to verify signature from iDeal provider";
            ConsumerMessage = "Betaling met iDeal is momenteel niet mogelijk, probeer het later nog eens";
        }
    }
}
