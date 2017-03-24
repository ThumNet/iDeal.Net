﻿using System;
using System.Xml.Linq;
using iDeal.Base;

namespace iDeal.Status
{
    public class StatusResponse : iDealResponse
    {
        /// <summary>
        /// Acquirer id (first four digits) + unique id generated by acquirer (last 12 digits)
        /// </summary>
        public string TransactionId { get; private set; }

        /// <summary>
        /// Status: success, cancelled, expired, failure or open
        /// </summary>
        public Status Status { get; private set; }

        /// <summary>
        /// Datetime when Status was set and verified (only set when Status= Success, Cancelled, Expired or Failure)
        /// </summary>
        public string StatusDateTimestamp { get; private set; }

        /// <summary>
        /// Consumer name (only set when Status= Success)
        /// </summary>
        public string ConsumerName { get; private set; }

        /// <summary>
        /// Accountnumber of consumer (only set when Status= Success)
        /// </summary>
        public string ConsumerIBAN { get; private set; }

        /// <summary>
        /// Consumer city (only set when Status= Success)
        /// </summary>
        public string ConsumerBIC { get; private set; }

        /// <summary>
        /// The amount garanteed by the Acquirer to the Merchant (only set when Status= Success)
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// The currency of the garanteed amount (ISO 4217) (only set when Status= Success)
        /// </summary>
        public string Currency { get; private set; }

        public StatusResponse(XElement xDocument)
            : base(xDocument)
        {
            TransactionId = xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "transactionID").Value;

            switch (xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "status").Value)
            {
                case "Success":
                    Status = Status.Success;
                    break;
                case "Cancelled":
                    Status = Status.Cancelled;
                    break;
                case "Expired":
                    Status = Status.Expired;
                    break;
                case "Failure":
                    Status = Status.Failure;
                    break;
                case "Open":
                    Status = Status.Open;
                    break;
                default:
                    throw new InvalidOperationException("Received unknown status");
            }

            if (Status != Status.Open)
            {
                StatusDateTimestamp = xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "statusDateTimestamp").Value;
            }
            
            if (Status == Status.Success)
            {
                ConsumerName = xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "consumerName").Value;
                ConsumerIBAN = xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "consumerIBAN").Value;
                ConsumerBIC = xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "consumerBIC").Value;
                Amount = (decimal)xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "amount");
                Currency = xDocument.Element(XmlNamespace + "Transaction").Element(XmlNamespace + "currency").Value;
            }
        }
    }
}
