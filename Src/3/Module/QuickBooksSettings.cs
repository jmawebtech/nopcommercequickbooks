using System.ComponentModel;
using Nop.Core.Configuration;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class QuickBooksSettings : ISettings
    {
        public string ARAAccount { get; set; }
        public string BillAPAAccount { get; set; }
        public string ClassRef { get; set; }
        public string CompanyID { get; set; }
        public string CompletedOrderStatus { get; set; }
        public string ConnectionString { get; set; }
        public string CustomerID { get; set; }
        public string DataMode { get; set; }
        public string DebugMode { get; set; }
        public string DefaultAccount { get; set; }
        public string DefaultAccountID { get; set; }
        public string DefaultSalesRep { get; set; }
        public bool DeleteQBDuplicates { get; set; }
        public string DefaultTaxCode { get; set; }
        public string DNNUserName { get; set; }
        public string DNNPassword { get; set; }
        public int DNNPortalID { get; set; }
        public string DNNUrl { get; set; }
        public string EBayUserToken { get; set; }
        public DateTime EBayUserTokenDate { get; set; }
        public string ECommerceCustomerTags { get; set; }
        public int HighestOrder { get; set; }
        public bool InsertZeroOrders { get; set; }
        public string InvDiscAcct { get; set; }
        public string InvoiceMode { get; set; }
        public string ItemAssetAcct { get; set; }
        public string ItemCOGSAcct { get; set; }
        public string ItemIncomeAcct { get; set; }
        public DateTime LastDownloadUtc { get; set; }
        public DateTime LastDownloadUtcEnd { get; set; }
        public string Licensekey { get; set; }
        public int LowestOrder { get; set; }
        public bool MarkOrderComplete { get; set; }
        public decimal MerchantFee { get; set; }
        public decimal MerchantPercent { get; set; }
        public string MerchantVendorAcct { get; set; }
        public bool ModifyRecords { get; set; }

        public string MSDynamicsCRMUom { get; set; }
        public string MSDynamicsCRMPriceLevel { get; set; }
        public string MSDynamicsCRMCustomerTypeCode { get; set; }

        public string MSDynamicsCRMOrganizationUri { get; set; }
        public string MSDynamicsCRMUserName { get; set; }
        public string MSDynamicsCRMPassword { get; set; }
        public string PaymentGatewayEmail { get; set; }
        public int OrderLimit { get; set; }
        public string OrderPrefix { get; set; }
        public string OrderPostScript { get; set; }
        public string OrderStatus { get; set; }
        public string QBXMLVersion { get; set; }
        public List<string> QuickBooksCustomFields { get; set; }
        public DateTime QuickBooksTrialStartDate { get; set; }
        public string QuickBooksOnlineEmail { get; set; }
        public string PmtARAccount { get; set; }
        public bool POSSyncImages { get; set; }
        public int POSNumberProducts { get; set; }
        public string SalesTaxVendor { get; set; }
        public bool SetExported { get; set; }

        public string SolutionName { get; set; }
        public string SolutionUserName { get; set; }
        public string SolutionPassword { get; set; }
        public string SolutionWebsite { get; set; }


        public string StandardTerms { get; set; }
        public int StartOrderNumber { get; set; }
        public string StoreName { get; set; }
        public List<string> StringOrders { get; set; }
        public string TestingResult { get; set; }
        public bool ToBePrinted { get; set; }
        public bool UpdateInStockFromQB { get; set; }
        public bool UseMultiCurrency { get; set; }
        public bool UseUnknownItem { get; set; }
        public bool UseUK { get; set; }
        public bool UseTaxServiceItem { get; set; }
        public bool VoidOnly { get; set; }

    }
}