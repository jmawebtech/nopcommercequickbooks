using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnexForQuickBooks.Model
{
    [Serializable]
    public class JMACustomer
    {
        public JMACustomer()
        {
            DefaultCategory = new List<string>();
            JMACustomFields = new List<JMACustomField>();
            Tags = new List<string>();
        }

        public string Id { get; set; }
        public bool IsTaxExempt { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string DefaultCurrencyCode { get; set; }
        public string Website { get; set; }
        public JMAAddress BillingAddress { get; set; }
        public JMAAddress ShippingAddress { get; set; }
        public List<string> DefaultCategory { get; set; }
        public string Notes { get; set; }
        public List<JMACustomField> JMACustomFields { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalRevenue { get; set; }
        public string CustomerType { get; set; }
        public List<string> Tags { get; set; }
        public string GroupName { get; set; }

        public string PriceLevel { get; set; }
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
    }
}
