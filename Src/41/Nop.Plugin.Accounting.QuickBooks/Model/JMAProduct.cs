using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnexForQuickBooks.Model
{
    [Serializable]
    public class JMAProduct
    {
        public JMAProduct()
        {
            AssemblyItems = new List<JMAProduct>();
            JMACustomFields = new List<JMACustomField>();
            Categories = new List<string>();
        }

        public string AlternateId { get; set; }
        public string Condition { get; set; }
        public List<JMAProduct> AssemblyItems { get; set; }
        public string Barcode { get; set; }
        public int Id { get; set; }
        public string Color { get; set; }
        public bool DoNotSync { get; set; }
        public List<JMACustomField> JMACustomFields { get; set; }
        public string ParentItem { get; set; }
        public object ManagingStock { get; set; }
        public Guid ProductGuid { get; set; }
        public string Sku { get; set; }
        public string OriginalSku { get; set; }
        public int HandlingTime { get; set; }
        public decimal Amount { get; set; }
        public decimal MSRP { get; set; }
        public string Name { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal StockQuantity { get; set; }
        public bool IsDownload { get; set; }
        public bool IsService { get; set; }
        public string Vendor { get; set; }
        public string Department { get; set; }
        public string DepartmentCode { get; set; }
        public string InventoryStatus { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string MetaDescription { get; set; }
        public string Description { get; set; }
        public decimal Weight { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsEnabled { get; set; }
        public List<string> Categories { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateInserted { get; set; }
        public string Warehouse { get; set; }
        public string UoM { get; set; }
        public string ParentProduct { get; set; }
        public string Size { get; set; }
        public bool? Visible { get; set; }
        public bool IsInventory 
        {
            get
            {
                if (IsDownload || IsService)
                    return false;

                return true;
            }
        }


        public string ASIN { get; set; }
        public string UPC { get; set; }
        public string Brand { get; set; }
        public string ALU { get; set; }
    }
}
