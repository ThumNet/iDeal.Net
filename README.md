# iDeal.NET
iDeal is the leading online payment platform in the Netherlands. 
iDeal.NET provides an API to easily communicate with your iDeal provider and integrate iDeal payments into your .NET (web)applications.
The project contains a sample application which gives a basic example of the usage of iDeal.NET.

## iDeal versions
iDeal.NET is aimed at the latest version of iDeal v3.3.1 and works with iDeal Professional (Rabobank), iDeal Zelfbouw (ABN Amro), iDeal Integrated and iDeal Advanced (ING Bank). These versions allow for real-time feedback on transactions. 
iDeal.NET does not yet support iDeal Basic (ING Bank), iDeal Hosted , iDeal Lite (Rabobank) and iDeal Zakelijk which are easily implemented in applications but do not allow for real-time feedback on transactions.



## Configuration
By default iDeal.NET is configured through the web.config or app.config.

First declare the configuration section

	<configSections>
      <section name="iDeal" type="iDeal.Configuration.ConfigurationSectionHandler, iDeal" allowLocation="true" allowDefinition="Everywhere" />
    </configSections>
	
Second implement the iDeal section

	<iDeal>
        <merchant id="123456789" subId="0" />
        <acquirer url="https://abnamro-test.ideal-payment.de/ideal/iDEALv3/" />
        <acceptantCertificate filename="App_Data\yourprivate.pfx" password="your private password" />
        <acquirerCertificate filename="App_Data\ideal_public.cer" />
    </iDeal>

The merchant id is the unique identifier you received from your iDeal provider(acquirer). The merchant subId is usually 0, or otherwise specified by the acquirer. The url points to the url of your acquirer which handles all iDeal requests.

The acceptant certicate is the private certificate you created (or bought), the related public key has to be uploaded to your ideal dashboard. See below how to create a new self signed certificate. 

The acquirer certificate is the certificate you receive from your ideal provider, with this certificate responses from the acquirer are verified.

The filenames of the certificates specify the relative path to the files containing the certificates. Only for the private acceptant certificate a password is needed.

## Directory request
In order for customers to make a payment, they first have to choose their bank. To retrieve a list of banks (issuers) which consumers can choose from, you have to send a directory request to your iDeal provider (acquirer). This is how you send a directory request with iDeal.NET:

	var iDealService = new iDealService();
	var directoryResponse = iDealService.SendDirectoryRequest();
	var issuers = directoryResponse.Issuers;

The response from a directory request holds a list of issuers. De issuer object holds all the relevant information of an issuer like id and name. Check the iDeal Merchant Integration Guide from your acquirer for exact details on rendering the dropdown.
In order the minimize the calls to the acquirer it's recommended (and often required) you cache the result of a directory request and refresh the cache every 24 hours.

## Transaction request
When a customer has choosen an issuer you can send a transaction request to the selected issuer:

	// Send transaction request to selected issuer
	var transactionResponse = _iDealService.SendTransactionRequest(
		issuerId: "ABNAMROSIM", 
		merchantReturnUrl: "http://www.yourwebsite.nl/landingpage", 
		purchaseId: "12345", 
		amount: 500, 
		expirationPeriod: TimeSpan.FromMinutes(5), 
		description: "Some description",
		entranceCode: "67890";

	// Redirect user to transaction page of issuer
	Response.Redirect(transactionResponse.IssuerAuthenticationUrl);

The following parameters have to be specified to perform a transaction request
 
 - issuerId: Unique identifier of the selected issuer
 - merchantReturnUrl: Url to which the customers is redirected after the payment process finishes
 - purchaseId: Unique identifier generated by merchant/acceptant
 - amount: The amount in cents (euro)
 - expirationPeriod: Period consumer has to finish the tranaction before it is marked as expired by the issuer
 - description: Description of the payment, will be shown on customer's bank statement.
 - entranceCode: Unique code to identify consumer when returning to webshop, generated by merchant/acceptant
 	
The response of a transaction request holds the following information:

 - TransactionId: Uniquely identifies the transaction, used to retrieve the status of a transaction
 - IssuerAuthenticationUrl: Url of the selected issuer to which you have to redirect the customer to make the payment
 - PurchaseId: Unique identifier generated by merchant/acceptant in the transaction request
 
When the response from your acquirer is received you can redirect the customer to the issuer who will be performing the iDeal payment (IssuerAuthenticationUrl). It's important to store the transaction id, you will need this to retrieve the status on the landing page you specified to which the customer will be redirected when finishing the payment.

## Status request
When a customer has finished the payment, the customer is redirected back to the url you specified when sending the transaction request. The issuer will add two query parameters to the url 'trxid' and 'ec'. Something like: http://your-url.nl/Status?trxid=0000000000078401&ec=ce6462a2-ce87-46
Parameter 'trxid' holds the transaction id, and 'ec' holds the entrance code you specified in the transaction request. With the transaction id you can retrieve the status of the payment:

	var iDealService = new iDealService();
	var statusResponse = iDealService.SendStatusRequest(transactionId);
	
The response contains the status, which can be Success, Failure, Cancelled, Open or Expired. The response also contains the account number, name and city of the customer.

## Certificates
In order to use iDeal you need to create (or buy) a certificate. The public key needs to be uploaded to your iDeal dashboard so they are able to verify your messages/requests. To create a self-signed certifcate you can use openSSL
Download and install openSSL from http://www.openssl.org/ or https://slproweb.com/products/Win32OpenSSL.html
then from the command prompt run the following commands:

  * openssl genrsa -aes128 -out private.pem -passout pass:[YOUR PRIVATE PASSWORD] 2048
  * openssl req -x509 -sha256 -new -key private.pem -passin pass:[YOUR PRIVATE PASSWORD] -days 1825 -out certificate.cer
  * openssl pkcs12 -export -in certificate.cer -inkey privateKey.pem -out yourprivate.pfx

After this:
 * upload the certificate.cer to your iDeal dashboard 
 * put 'yourprivate.pfx'in your App_Data\ folder on your website and make sure the web.config/app.config specifies the correct path and password
 * download the public certificate provided by your iDeal provider and place it in the App_Data\ folder and make sure the web.config/app.config specifies the correct path 
 

## License
All source code is licensed under the [GNU Lesser General Public License](http://www.gnu.org/licenses/lgpl.html)
[bruidsfotograaf](https://www.erwinbeckers.nl)
