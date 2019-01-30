using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using JMA.Plugin.Accounting.QuickBooks;
using JMA.Plugin.Accounting.QuickBooks.DTO;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Accounting.QuickBooks;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;

/// <summary>
/// Summary description for QBOWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//[System.Web.Script.Services.ScriptService]
public class QBOWebService : System.Web.Services.WebService
{
    #region ctor

    public QBOWebService()
    {
        _orderService = EngineContext.Current.Resolve<IOrderService>();
        _productService = EngineContext.Current.Resolve<IProductService>();
        _discountService = EngineContext.Current.Resolve<IDiscountService>();
        _settingService = EngineContext.Current.Resolve<ISettingService>();
        _encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
        _checkoutAttributeService = EngineContext.Current.Resolve<ICheckoutAttributeService>();
        _checkoutAttributeParser = EngineContext.Current.Resolve<ICheckoutAttributeParser>();
        _paymentMethodService = EngineContext.Current.Resolve<IPaymentService>();
        _manufacturerService = EngineContext.Current.Resolve<IManufacturerService>();
        _dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
        _authenticationService = EngineContext.Current.Resolve<IAuthenticationService>();
        _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
        _customerService = EngineContext.Current.Resolve<ICustomerService>();
        _customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
        _settingService.ClearCache();
        _quickBooksSettings = EngineContext.Current.Resolve<QuickBooksSettings>();
        jmaWebServiceCommon = new JMAWebServiceCommon();
    }

    #endregion

    [WebMethod]
    public bool Authenticate(string username, string password)
    {
        Customer cust = new Customer();
        CustomerRole cr = new CustomerRole();
        if (!_customerSettings.UsernamesEnabled)
        {
            cust = _customerService.GetCustomerByEmail(username);
        }
        else
        {
            cust = _customerService.GetCustomerByUsername(username);
        }

        if (cust != null)
        {
            foreach (CustomerRole cuRole in cust.CustomerRoles)
            {
                if (cuRole.Name.ToLower() == "administrators")
                {
                    CustomerLoginResults results = _customerRegistrationService.ValidateCustomer(username, password);
                    if (results == CustomerLoginResults.Successful)
                    {
                        return true;
                    }
                }
            }
        }



        return false;
    }

    [WebMethod]
    public JMAUser GetOrders(string userName, string password)
    {
        //if (!Authenticate(userName, password))
        //{
        //    return null;
        //}
        JMAUser theUser = new JMAUser();
        theUser.Orders = jmaWebServiceCommon.GetOrders();
        theUser.Settings = GetSettings(userName, password);
        return theUser;
    }

    [WebMethod]
    public List<JMAProduct> GetUpdatedProducts(string userName, string password, DateTime beginDate, DateTime endDate)
    {
        if (!Authenticate(userName, password))
        {
            return null;
        }

        List<Product> products = new List<Product>();

        List<JMAProduct> jmaProducts = new List<JMAProduct>();

        if (products == null)
            return jmaProducts;

        foreach (Product pr in products)
        {
            JMAProduct product = new JMAProduct();
            product.Sku = pr.Sku;
            product.Name = pr.Name;
            product.Amount = pr.Price;
            product.PurchaseCost = pr.ProductCost;
            product.StockQuantity = pr.StockQuantity;
            product.Weight = pr.Weight;
            product.IsTaxable = !pr.IsTaxExempt;
            product.IsDownload = pr.IsDownload;
            product.Description = pr.ShortDescription;
            jmaProducts.Add(product);
        }

        return jmaProducts;


    }


    [WebMethod]
    public JMASettings GetSettings(string userName, string password)
    {
        if (!Authenticate(userName, password))
        {
            return null;
        }
        JMASettings theUser = new JMASettings();
        theUser.ARAAccount = _quickBooksSettings.ARAAccount;
        theUser.BillAPAAccount = _quickBooksSettings.BillAPAAccount;
        theUser.CustomerID = _quickBooksSettings.CustomerID;
        theUser.DataMode = _quickBooksSettings.DataMode;
        theUser.DefaultAccount = _quickBooksSettings.DefaultAccount;
        theUser.DeleteQBDuplicates = _quickBooksSettings.DeleteQBDuplicates;
        theUser.HighestOrder = _quickBooksSettings.HighestOrder;
        theUser.InvoiceMode = _quickBooksSettings.InvoiceMode;
        theUser.InsertZeroOrders = _quickBooksSettings.InsertZeroOrders;
        theUser.InvDiscAcct = _quickBooksSettings.InvDiscAcct;
        theUser.ItemAssetAcct = _quickBooksSettings.ItemAssetAcct;
        theUser.ItemCOGSAcct = _quickBooksSettings.ItemCOGSAcct;
        theUser.ItemIncomeAcct = _quickBooksSettings.ItemIncomeAcct;
        theUser.Licensekey = _quickBooksSettings.Licensekey;
        theUser.MarkOrderComplete = _quickBooksSettings.MarkOrderComplete;
        theUser.PmtARAccount = _quickBooksSettings.PmtARAccount;
        theUser.StoreName = _quickBooksSettings.StoreName;
        theUser.StringOrders = _quickBooksSettings.StringOrders;


        if (_quickBooksSettings.LastDownloadUtc == null)
        {
            theUser.LastDownloadUtc = DateTime.Now;
        }
        else
        {
            theUser.LastDownloadUtc = Convert.ToDateTime(_quickBooksSettings.LastDownloadUtc);
        }

        theUser.LowestOrder = _quickBooksSettings.LowestOrder;
        theUser.OrderLimit = _quickBooksSettings.OrderLimit;
        theUser.OrderPrefix = _quickBooksSettings.OrderPrefix;
        theUser.OrderStatus = _quickBooksSettings.OrderStatus;
        theUser.SalesTaxVendor = _quickBooksSettings.SalesTaxVendor;
        theUser.SetExported = _quickBooksSettings.SetExported;
        theUser.StartOrderNumber = _quickBooksSettings.StartOrderNumber;
        theUser.StringOrders = _quickBooksSettings.StringOrders;
        theUser.ToBePrinted = _quickBooksSettings.ToBePrinted;
        theUser.QuickBooksOnlineEmail = _quickBooksSettings.QuickBooksOnlineEmail;
        return theUser;
    }

    [WebMethod]
    public string SaveSettings(JMASettings model, string userName, string password)
    {
        if (!Authenticate(userName, password))
        {
            return null;
        }

        QuickBooksSettings _quickBooksSettings = new QuickBooksSettings();

        _quickBooksSettings.ARAAccount = model.ARAAccount;
        _quickBooksSettings.BillAPAAccount = model.BillAPAAccount;
        _quickBooksSettings.CustomerID = model.CustomerID;
        _quickBooksSettings.DataMode = model.DataMode;
        _quickBooksSettings.DefaultAccount = model.DefaultAccount;
        _quickBooksSettings.DeleteQBDuplicates = model.DeleteQBDuplicates;
        _quickBooksSettings.InvDiscAcct = model.InvDiscAcct;
        _quickBooksSettings.ItemAssetAcct = model.ItemAssetAcct;
        _quickBooksSettings.ItemCOGSAcct = model.ItemCOGSAcct;
        _quickBooksSettings.ItemIncomeAcct = model.ItemIncomeAcct;
        _quickBooksSettings.InvoiceMode = model.InvoiceMode;
        _quickBooksSettings.InsertZeroOrders = model.InsertZeroOrders;
        _quickBooksSettings.LastDownloadUtc = model.LastDownloadUtc;
        _quickBooksSettings.LastDownloadUtcEnd = model.LastDownloadUtcEnd;
        _quickBooksSettings.Licensekey = model.Licensekey.Trim();
        _quickBooksSettings.MarkOrderComplete = model.MarkOrderComplete;
        _quickBooksSettings.OrderLimit = model.OrderLimit;
        _quickBooksSettings.OrderPrefix = model.OrderPrefix;
        _quickBooksSettings.OrderStatus = model.OrderStatus;
        _quickBooksSettings.PmtARAccount = model.PmtARAccount;
        _quickBooksSettings.QuickBooksOnlineEmail = model.QuickBooksOnlineEmail;
        _quickBooksSettings.SetExported = model.SetExported;
        _quickBooksSettings.StartOrderNumber = model.StartOrderNumber;
        _quickBooksSettings.StringOrders = model.StringOrders;
        //if string order is being user, make the high and low numbers 0
        _quickBooksSettings.LowestOrder = model.LowestOrder;
        _quickBooksSettings.HighestOrder = model.HighestOrder;
        _quickBooksSettings.SalesTaxVendor = model.SalesTaxVendor;
        _quickBooksSettings.ToBePrinted = model.ToBePrinted;
        try
        {
            _settingService.SaveSetting(_quickBooksSettings);
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    [WebMethod]
    public string ChangeOrderStatus(string orderID, string status, string trackingNumber)
    {
        try
        {
            Order order = _orderService.GetOrderById(int.Parse(orderID));

            if (String.IsNullOrEmpty(status))
            {
                order.OrderStatus = OrderStatus.Complete;
            }
            else
            {
                OrderStatus orderStatus;
                if (!String.IsNullOrEmpty(status))
                {
                    bool parseResult = Enum.TryParse(status, out orderStatus);
                    order.OrderStatus = orderStatus;
                }
            }

            var sh = new Nop.Core.Domain.Shipping.Shipment();
            sh.CreatedOnUtc = DateTime.Now;
            sh.DeliveryDateUtc = DateTime.Now;
            sh.Order = order;
            sh.ShippedDateUtc = DateTime.Now;
            sh.TrackingNumber = trackingNumber;

            order.PaidDateUtc = DateTime.Now;
            order.OrderStatus = OrderStatus.Complete;
            order.OrderStatusId = 30;
            order.ShippingStatusId = 30;
            order.ShippingStatus = Nop.Core.Domain.Shipping.ShippingStatus.Shipped;
            order.PaymentStatus = Nop.Core.Domain.Payments.PaymentStatus.Paid;
           
            order.PaymentStatusId = 30;

            sh.OrderId = order.Id;
            order.Shipments.Add(sh);
            order.PaidDateUtc = DateTime.Now;
            _orderService.UpdateOrder(order);
            return "OK";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    
    [WebMethod]
    public string StockUpdate(string sku, decimal qbOnHand, string userName, string password)
    {
        if (!Authenticate(userName, password))
        {
            return null;
        }

        try
        {
            IProductService productService = EngineContext.Current.Resolve<IProductService>();

            Product pv = productService.GetProductBySku(sku);

            if (pv == null)
            {
                return "Unable to find product variant for " + sku;
            }

            //skip shipping and non inventory items
            if (pv.ManageInventoryMethod == ManageInventoryMethod.DontManageStock)
            {
                return "OK";
            }

            pv.StockQuantity = Convert.ToInt16(qbOnHand);
            productService.UpdateProduct(pv);
            return "OK";

        }
        catch (Exception ex)
        {
            return String.Format("An error has occured with updating stock. {0} {1}", ex.Message + " " + ex.StackTrace);
        }

    }

    #region private variables

    IDiscountService _discountService;
    IProductService _productService;
    IOrderService _orderService;
    ISettingService _settingService;
    IEncryptionService _encryptionService;
    ICheckoutAttributeService _checkoutAttributeService;
    ICheckoutAttributeParser _checkoutAttributeParser;
    IPaymentService _paymentMethodService;
    IManufacturerService _manufacturerService;
    IDateTimeHelper _dateTimeHelper;
    IAuthenticationService _authenticationService;
    CustomerSettings _customerSettings;
    ICustomerRegistrationService _customerRegistrationService;
    ICustomerService _customerService;
    QuickBooksSettings _quickBooksSettings;
    JMAWebServiceCommon jmaWebServiceCommon;

    #endregion

}
