using System;
using System.Collections.Generic;


namespace ConnexForQuickBooks.Model
{
    [Serializable]
    public class JMAOrderDetail
    {
        public JMAOrderDetail()
        {
            JMACustomFields = new List<JMACustomField>();
            Product = new JMAProduct();
            Name = string.Empty;
            OrderNumbersForItemSummary = new List<string>();
        }

        public string ASIN { get; set; }
        public bool BuildInventoryItemAssembly { get; set; }
        public decimal DiscountExclTax { get; set; }
        public decimal DiscountInclTax { get; set; }
        public int Id { get; set; }
        public bool IsShipping { get; set; }
        public string FulFillmentChannel { get; set; }
        public bool HasTax { get; set; }
        public string InventorySiteLocation { get; set; }
        public string ItemNumber { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int OrderProductId { get; set; }
        public JMAProduct Product { get; set; }
        public string Name { get; set; }
        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string ParentItem { get; set; }
        public decimal PriceExclTax { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityReceived { get; set; }
        public string Sku { get; set; }
        public string UnitOfMeasure { get; set; }
        public string ClassRef { get; set; }
        public DateTime? ShipDate { get; set; }
        public string Source { get; set; }
        public string StoreName { get; set; }
        public string UPC { get; set; }
        public string LotNumber { get; set; }
        public string TaxCode { get; set; }
        public List<JMACustomField> JMACustomFields { get; set; }
        public decimal PriceExclTaxCustomerCurrency { get; set; }
        public decimal DiscountExclTaxCustomerCurrency { get; set; }
        public decimal PriceInclTaxCustomerCurrency { get; set; }
        public decimal DiscountInclTaxCustomerCurrency { get; set; }
        public string ISBN { get; set; }
        public List<string> OrderNumbersForItemSummary { get; set; }
        public DateTime ServiceDate { get; set; }
    }
}
