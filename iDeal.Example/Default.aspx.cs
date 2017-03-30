using iDeal.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace iDeal.Example
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            EnableButtons();
        }
        protected void btnDirReq_Click(object sender, EventArgs e)
        {
            try
            {
                tableError.Visible = false;
                logResult.InnerHtml = "";

                var ideal = new iDealService();
                var response = ideal.SendDirectoryRequest();
                foreach (var issuer in response.Issuers)
                {
                    logResult.InnerHtml += string.Format("Issuer: ID={0} Name={1}, Country={2}<br/>", issuer.Id, issuer.Name, issuer.CountryNames);
                }
                Session["issuers"] = response.Issuers;
                EnableButtons();
            }

            catch (iDealException ex)
            {
                tableError.Visible = true;
                lblErrorCode.Text = ex.ErrorCode;
                lblErrorDetail.Text = ex.ErrorDetail;
                lblErrorMessage.Text = ex.ErrorMessage;
                lblConsumerMessage.Text = ex.ConsumerMessage;
            }
        }
        protected void btnTransaction1_Click(object sender, EventArgs e)
        {
            try
            {
                tableError.Visible = false;
                logResult.InnerHtml = "";

                var ideal = new iDealService();

                var issuers = (IList<iDeal.Directory.Issuer>)Session["issuers"];
                int amount = int.Parse(txtAmount.Value);
                var response = ideal.SendTransactionRequest(issuers[0].Id, "http://www.your-url.com", "purchaseId", amount, TimeSpan.FromMinutes(5), "Buy something for " + txtAmount.Value + " euro", "myentrancecode");

                logResult.InnerHtml = string.Format("AcquirerId:{0}<br>", response.AcquirerId);
                logResult.InnerHtml += string.Format("TransactionId:{0}<br>", response.TransactionId);
                logResult.InnerHtml += string.Format("PurchaseId:{0}<br>", response.PurchaseId);
                logResult.InnerHtml += string.Format("<a href='{0}' target=='new'>complete transaction</a><br>", response.IssuerAuthenticationUrl);

                Session["transactionid"] = response.TransactionId;
                EnableButtons();
            }

            catch (iDealException ex)
            {
                tableError.Visible = true;
                lblErrorCode.Text = ex.ErrorCode;
                lblErrorDetail.Text = ex.ErrorDetail;
                lblErrorMessage.Text = ex.ErrorMessage;
                lblConsumerMessage.Text = ex.ConsumerMessage;
            }

        }

        private void EnableButtons()
        {
            btnTransaction1.Enabled = (Session["issuers"] != null);
            btnStatus1.Enabled = (Session["transactionid"] != null);
        }
        protected void btnStatus1_Click(object sender, EventArgs e)
        {
            try
            {
                tableError.Visible = false;
                logResult.InnerHtml = "";

                var ideal = new iDealService();
                var response = ideal.SendStatusRequest((string)Session["transactionid"]);

                logResult.InnerHtml += string.Format("Status:{0}<br>", response.Status);
                logResult.InnerHtml = string.Format("TransactionId:{0}<br>", response.TransactionId);
                logResult.InnerHtml += string.Format("AcquirerId:{0}<br>", response.AcquirerId);
                logResult.InnerHtml += string.Format("ConsumerName:{0}<br>", response.ConsumerName);
                logResult.InnerHtml += string.Format("ConsumerIBAN:{0}<br>", response.ConsumerIBAN);
                logResult.InnerHtml += string.Format("ConsumerBIC:{0}<br>", response.ConsumerBIC);
                logResult.InnerHtml += string.Format("Amount:{0}<br>", response.Amount);
                logResult.InnerHtml += string.Format("Currency:{0}<br>", response.Currency);

                Session["transactionid"] = null;
                EnableButtons();
            }

            catch (iDealException ex)
            {
                tableError.Visible = true;
                lblErrorCode.Text = ex.ErrorCode;
                lblErrorDetail.Text = ex.ErrorDetail;
                lblErrorMessage.Text = ex.ErrorMessage;
                lblConsumerMessage.Text = ex.ConsumerMessage;
            }

        }
    }
}