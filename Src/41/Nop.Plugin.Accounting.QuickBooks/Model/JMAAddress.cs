using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ConnexForQuickBooks.Model
{
    [Serializable]
    public class JMAAddress
    {
        public JMAAddress()
        {
            Customer = new JMACustomer();
            FirstName = string.Empty;
            LastName = string.Empty;
            MiddleName = string.Empty;
            Email = string.Empty;
            Company = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            Address3 = string.Empty;
            City = string.Empty;
            RegionName = string.Empty;
            CountryName = string.Empty;
            TwoLetterIsoCode = string.Empty;
            PostalCode = string.Empty;
            FaxNumber = string.Empty;
        }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string AltPhoneNumber { get; set; }
        public string City { get; set; }
        public string Company { get; set; }
        public string CountryName { get; set; }
        public JMACustomer Customer { get; set; }
        public string Email { get; set; }
        public string FaxNumber { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public int Id { get; set; }
        public bool IsBilling { get; set; }
        public string JobTitle { get; set; }
        public string JobName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string OrderNumber { get; set; }
        public string ParentCustomer { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string RegionName { get; set; }
        public string TwoLetterIsoCode { get; set; }
        public string VATNumber { get; set; }
        public bool SubjectToVat { get; set; }

        public string GetFullName()
        {
            if (!String.IsNullOrEmpty(FullName))
                return FullName;
            else if (!String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName) && !String.IsNullOrEmpty(MiddleName))
                return FirstName.Trim() + " " + MiddleName.Trim() + " " + LastName.Trim();
            else if (!String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName))
                return FirstName.Trim() + " " + LastName.Trim();
            else if (!String.IsNullOrEmpty(FirstName))
                return FirstName.Trim() + " " + "Not Supplied";
            else
                return string.Empty;
        }



    }
}
