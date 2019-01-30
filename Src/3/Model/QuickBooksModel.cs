using System.ComponentModel;
using System.Web.Mvc;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Accounting.QuickBooks.Models
{
    public class QuickBooksModel
    {
        [DisplayName("Order Prefix:")]
        public string OrderPrefix { get; set; }

        public string AppID { get; set; }

        public string AppLogin { get; set; }

        /// <summary>
        /// accounts receiveable account
        /// </summary>
        [DisplayName("Accounts Receiveable Account:")]
        public string ARAAccount { get; set; }

        /// <summary>
        /// accounts payable account
        /// </summary>
        [DisplayName("Accounts Payable Account:")]
        public string BillAPAAccount { get; set; }

        /// <summary>
        /// how customer paid for product. qb assigns payment type to invoice
        /// </summary>
        public string CashAccount { get; set; }

        /// <summary>
        /// if you do invoices by class, use this value. Like a category
        /// </summary>
        public string ClassRef { get; set; }

        /// <summary>
        /// user name to connect as, when using connect. Acts as user name during auth with web connector
        /// </summary>
        public string ConnectAs { get; set; }

        public string ConnectionString { get; set; }

        public string ConnectionTicket { get; set; }


        /// <summary>
        /// how customer is entered into quickbooks, like last name first name
        /// </summary>

        [DisplayName("Customer ID:")]
        public string CustomerID { get; set; }

        [DisplayName("Data Mode:")]
        public string DataMode { get; set; }

        public string DebugMode { get; set; }

        /// <summary>
        /// removes duplicate invoices from QB
        /// </summary>
        [DisplayName("Delete QB Duplicates:")]
        public bool DeleteQBDuplicates { get; set; }

        /// <summary>
        /// when deposits are made, this is the account where the money goes. Like business or bank of america account
        /// </summary>

        [DisplayName("Funds Are Deposited (typically Undeposited Funds or a bank account):")]
        public string DefaultAccount { get; set; }


        /// <summary>
        /// whether invoices are generated with payments or without them
        /// </summary>

        [DisplayName("Do Payments:")]
        public bool DoPayments { get; set; }

        /// <summary>
        /// when making qbwc file, part of a unique id
        /// </summary>
        public string FileID { get; set; }

        public bool GenBill { get; set; }

        public bool GenPO { get; set; }

        public string GiftWrapItemId { get; set; }

        public string HandlingItemId { get; set; }

        /// <summary>
        /// highest order number to export into QB. 0 means highest order.
        /// </summary>

        [DisplayName("Highest Order:")]
        public int HighestOrder { get; set; }

        /// <summary>
        /// whether to insert tax codes from avalara into quickbooks tax setting
        /// </summary>
        public bool InsertAvalaraCode { get; set; }


        [DisplayName("Discount Account:")]
        public string InvDiscAcct { get; set; }

        [DisplayName("QuickBooks Transaction Type:")]
        public string InvoiceMode { get; set; }

        [DisplayName("Item Asset Account (Inventory):")]
        public string ItemAssetAcct { get; set; }

        [DisplayName("Cost of Goods Sold Account:")]
        public string ItemCOGSAcct { get; set; }

        [DisplayName("Item Income Account:")]
        public string ItemIncomeAcct { get; set; }

        public bool ItemZeroNonInventory { get; set; }

        /// <summary>
        /// shipping tax name in Kentico. 
        /// </summary>
        public string KenticoShippingTax { get; set; }

        /// <summary>
        /// key assigned by jma web technologies
        /// </summary>

        [DisplayName("JMA License Key:")]
        public string Licensekey { get; set; }

        /// <summary>
        /// lowest order number to export into QB. 
        /// </summary>

        [DisplayName("Lowest Order Number:")]
        public int LowestOrder { get; set; }
        [DisplayName("After exporting, mark order as complete:")]
        public bool MarkOrderComplete { get; set; }

        public string MissingVendor { get; set; }


        /// <summary>
        /// name of the current website
        /// </summary>
        public string MySiteName { get; set; }


        /// <summary>
        /// whether to export orders that have already been exported into QuickBooks
        /// </summary>
        public bool NonExport { get; set; }

        /// <summary>
        /// max number of orders total to export, like max of 10.
        /// </summary>

        [DisplayName("Max Orders To Export:")]
        public int OrderLimit { get; set; }

        [DisplayName("All Orders Must Be:")]
        public string OrderStatus { get; set; }

        public string OwnerID { get; set; }

        [DisplayName("Payment Accounts Receiveable Account:")]
        public string PmtARAccount { get; set; }

        [DisplayName("POS Sync Images:")]
        public bool POSSyncImages { get; set; }

        /// <summary>
        /// wipe out the base product price. only the option has the real price. options takes price from base product
        /// </summary>
        public bool ProductOptionsBasePrice { get; set; }

        /// <summary>
        /// only add product options are inventory items
        /// </summary>
        public bool ProductOptionsInventory { get; set; }

        [DisplayName("QuickBooks Online Email")]
        public string QuickBooksOnlineEmail { get; set; }


        public bool RespectPayShip { get; set; }

        [DisplayName("Default Sales Tax Vendor:")]
        public string SalesTaxVendor { get; set; }

        /// <summary>
        /// whether to set order as exported, when it is exported into QB.
        /// </summary>
        [DisplayName("Export Orders Regardless Of Status:")]
        public bool SetExported { get; set; }

        public string ShippingItemId { get; set; }

        [DisplayName("Starting Order Number")]
        public int StartOrderNumber { get; set; }

        public string StoreName { get; set; }

        public string TestingResult { get; set; }

        [DisplayName("For Canceled Orders, Void Matching QuickBooks Transactions:")]
        public bool VoidOnly { get; set; }

        public bool VoidCheck { get; set; }


        [DisplayName("Website:")]
        public string Website { get; set; }

        [DisplayName("Pull all nopCommerce orders from this date forward:")]
        public DateTime? LastDownloadUtc { get; set; }

        [DisplayName("Pull all nopCommerce orders from this date before:")]
        public DateTime? LastDownloadUtcEnd { get; set; }


        public DateTime QuickBooksTrialStartDate { get; set; }

        [DisplayName("Order Numbers, Separated by Commas")]
        public string StringOrders { get; set; }

        [DisplayName("Update Stock From QB")]
        public bool UpdateInStockFromQB { get; set; }

        [DisplayName("Insert Orders With Zero Total")]
        public bool InsertZeroOrders { get; set; }

        [DisplayName("Mark As To Be Printed")]
        public bool ToBePrinted { get; set; }

        [DisplayName("Use UK Edition")]
        public bool UseUK { get; set; }

        [DisplayName("Use MultiCurrency")]
        public bool UseMultiCurrency { get; set; }


    }
}