using System;
using System.Web.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Web.Controllers;
using Nop.Plugin.Accounting.QuickBooks.Models;
using System.Collections.Generic;
using System.Xml;
using JMA.Plugin.Accounting.QuickBooks;
using JMA.Plugin.Accounting.QuickBooks.LogProvider;
using JMA.Plugin.Accounting.QuickBooks.UI;
using JMA.Plugin.Accounting.QuickBooks.DTO;
using System.IO;
using System.Data.SqlClient;
using Nop.Core.Data;
using JMA.Plugin.Accounting.QuickBooks.LicenseProvider;

namespace Nop.Plugin.Accounting.QuickBooks.Controllers
{
    [AdminAuthorize]
    public class QuickBooksController: Controller
    {
        #region private

        readonly QuickBooksSettings _quickbooksSettings;
        readonly ISettingService _settingService;
        QuickBooksModel qbModel;
        IJMALogProvider errorMessageDataSource;
        UISettings uiSettings;

        #endregion

        #region ctor

        public QuickBooksController(QuickBooksSettings quickbooksSettings, ISettingService settingService)
        {
            _quickbooksSettings = quickbooksSettings;
            _settingService = settingService;
            this.errorMessageDataSource = JMALogProviderFactory.Get();
            uiSettings = new UISettings(GetSettings(), new NopCommerceSqlSettingProvider(_quickbooksSettings, _settingService), errorMessageDataSource, string.Empty);
        }

        #endregion


        public JMASettings GetSettings()
        {
            JMASettings theUser = new JMASettings();
            theUser.ARAAccount = _quickbooksSettings.ARAAccount;
            theUser.BillAPAAccount = _quickbooksSettings.BillAPAAccount;
            theUser.ConnectionString = _quickbooksSettings.ConnectionString;
            theUser.CustomerID = _quickbooksSettings.CustomerID;
            theUser.DataMode = _quickbooksSettings.DataMode;
            theUser.DefaultAccount = _quickbooksSettings.DefaultAccount;
            theUser.DeleteQBDuplicates = _quickbooksSettings.DeleteQBDuplicates;
            theUser.HighestOrder = _quickbooksSettings.HighestOrder;
            theUser.InvoiceMode = _quickbooksSettings.InvoiceMode;
            theUser.InsertZeroOrders = _quickbooksSettings.InsertZeroOrders;
            theUser.InvDiscAcct = _quickbooksSettings.InvDiscAcct;
            theUser.ItemAssetAcct = _quickbooksSettings.ItemAssetAcct;
            theUser.ItemCOGSAcct = _quickbooksSettings.ItemCOGSAcct;
            theUser.ItemIncomeAcct = _quickbooksSettings.ItemIncomeAcct;
            theUser.Licensekey = _quickbooksSettings.Licensekey;
            theUser.MarkOrderComplete = _quickbooksSettings.MarkOrderComplete;
            theUser.PmtARAccount = _quickbooksSettings.PmtARAccount;
            theUser.StringOrders = _quickbooksSettings.StringOrders;


            if (_quickbooksSettings.LastDownloadUtc == null)
            {
                theUser.LastDownloadUtc = DateTime.Now;
            }
            else
            {
                theUser.LastDownloadUtc = Convert.ToDateTime(_quickbooksSettings.LastDownloadUtc);
            }

            theUser.LowestOrder = _quickbooksSettings.LowestOrder;
            theUser.OrderLimit = _quickbooksSettings.OrderLimit;
            theUser.OrderPrefix = _quickbooksSettings.OrderPrefix;
            theUser.OrderStatus = _quickbooksSettings.OrderStatus;
            theUser.SalesTaxVendor = _quickbooksSettings.SalesTaxVendor;
            theUser.SetExported = _quickbooksSettings.SetExported;
            theUser.StartOrderNumber = _quickbooksSettings.StartOrderNumber;
            theUser.StringOrders = _quickbooksSettings.StringOrders;
            theUser.ToBePrinted = _quickbooksSettings.ToBePrinted;
            theUser.QuickBooksOnlineEmail = _quickbooksSettings.QuickBooksOnlineEmail;
            return theUser;
        }


        public void GetCustomerID(QuickBooksModel model)
        {

            IList<SelectListItem> ilSelectList = new List<SelectListItem>();
            SelectListItem LNFN = new SelectListItem();
            LNFN.Text = "Last Name, First Name";
            LNFN.Value = "LNFN";
            ilSelectList.Add(LNFN);

            SelectListItem Company = new SelectListItem();
            Company.Text = "Company Name";
            Company.Value = "Company Name";
            ilSelectList.Add(Company);

            SelectListItem FNLN = new SelectListItem();
            FNLN.Text = "First Name Last Name";
            FNLN.Value = "FNLN";
            ilSelectList.Add(FNLN);

            SelectListItem SingleName = new SelectListItem();
            SingleName.Text = "Single Customer 'Web Store'";
            SingleName.Value = "SingleName";
            ilSelectList.Add(SingleName);

            foreach (var a in ilSelectList)
            {
                if (model.CustomerID == a.Value)
                {
                    a.Selected = true;
                }
            }

            ViewData["CustomerID"] = ilSelectList;
        }

        public void GetDataMode(QuickBooksModel model)
        {
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();
            SelectListItem Push = new SelectListItem();
            Push.Text = "Push";
            Push.Value = "Push";
            if (model.DataMode == "Push")
            {
                Push.Selected = true;
            }
            ilSelectList.Add(Push);

            SelectListItem Pull = new SelectListItem();
            Pull.Text = "Pull";
            Pull.Value = "Pull";
            if (model.DataMode == "Pull")
            {
                Pull.Selected = true;
            }
            ilSelectList.Add(Pull);


            ViewData["DataMode"] = ilSelectList;

        }

        public void GetOrderStatus(QuickBooksModel model)
        {
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();

            SelectListItem Complete = new SelectListItem();
            Complete.Text = "Complete";
            Complete.Value = "Complete";
            if (model.OrderStatus == "Complete")
            {
                Complete.Selected = true;
            }
            ilSelectList.Add(Complete);


            SelectListItem Canceled = new SelectListItem();
            Canceled.Text = "Cancelled";
            Canceled.Value = "Cancelled";
            if (model.OrderStatus == "Cancelled")
            {
                Canceled.Selected = true;
            }
            ilSelectList.Add(Canceled);



            SelectListItem Pending = new SelectListItem();
            Pending.Text = "Pending";
            Pending.Value = "Pending";
            if (model.OrderStatus == "Pending")
            {
                Pending.Selected = true;
            }
            ilSelectList.Add(Pending);


            SelectListItem Processing = new SelectListItem();
            Processing.Text = "Processing";
            Processing.Value = "Processing";
            if (model.OrderStatus == "Processing")
            {
                Processing.Selected = true;
            }
            ilSelectList.Add(Processing);


            ViewData["OrderStatus"] = ilSelectList;

        }


        public void GetInvoiceMode(QuickBooksModel model)
        {
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();
            SelectListItem Invoice = new SelectListItem();
            Invoice.Text = "Invoices";
            Invoice.Value = "Invoices";
            if (model.InvoiceMode == "Invoices")
            {
                Invoice.Selected = true;
            }
            ilSelectList.Add(Invoice);

            SelectListItem InvoicePayments = new SelectListItem();
            InvoicePayments.Text = "Invoices/Payments";
            InvoicePayments.Value = "Invoices/Payments";
            if (model.InvoiceMode == "Invoices/Payments")
            {
                InvoicePayments.Selected = true;
            }
            ilSelectList.Add(InvoicePayments);


            SelectListItem SalesReceipts = new SelectListItem();
            SalesReceipts.Text = "Sales Receipts";
            SalesReceipts.Value = "Sales Receipts";
            if (model.InvoiceMode == "Sales Receipts")
            {
                SalesReceipts.Selected = true;
            }
            ilSelectList.Add(SalesReceipts);

            SelectListItem SalesOrders = new SelectListItem();
            SalesOrders.Text = "Sales Orders";
            SalesOrders.Value = "Sales Orders";
            if (model.InvoiceMode == "Sales Orders")
            {
                SalesOrders.Selected = true;
            }

            ilSelectList.Add(SalesOrders);

            SelectListItem PurchaseOrders = new SelectListItem();
            PurchaseOrders.Text = "Purchase Orders";
            PurchaseOrders.Value = "Purchase Orders";
            if (model.InvoiceMode == "Purchase Orders")
            {
                PurchaseOrders.Selected = true;
            }

            ilSelectList.Add(PurchaseOrders);

            SelectListItem Estimates = new SelectListItem();
            Estimates.Text = "Estimates";
            Estimates.Value = "Estimates";
            if (model.InvoiceMode == "Estimates")
            {
                Estimates.Selected = true;
            }

            ilSelectList.Add(Estimates);



            ViewData["InvoiceMode"] = ilSelectList;

        }

        protected void LoadGLAccts(string _AcctType, string DropDownToBind, string valueSelected)
        {
            string _QBListXMLFile = Server.MapPath("~/App_Data/QuickBooksDesktop/" + "JMAQB-AccountQueryRs.xml");
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();
            //add undeposited funds account for bank
            if (_AcctType == "Bank")
            {
                SelectListItem sl = new SelectListItem();
                sl.Text = "Undeposited Funds";
                sl.Value = "Undeposited Funds";
                ilSelectList.Add(sl);
            }
            if (System.IO.File.Exists(_QBListXMLFile))
            {
                XmlDocument _InDoc = new XmlDocument();
                _InDoc.Load(_QBListXMLFile);

                // Get GL Accounts
                XmlNodeList _GLAccts = _InDoc.SelectNodes("//AccountRet");
                foreach (XmlNode _Node in _GLAccts)
                {
                    string _GLType = _Node.SelectSingleNode("AccountType").InnerText;
                    string _GLAcctName = _Node.SelectSingleNode("FullName").InnerText;

                    if (!String.IsNullOrEmpty(_GLType) && _GLType == _AcctType)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = _GLAcctName;
                        item.Value = _GLAcctName;
                        if (valueSelected == _GLAcctName)
                        {
                            item.Selected = true;
                        }
                        ilSelectList.Add(item);
                    }
                    else
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = _GLAcctName;
                        item.Value = _GLAcctName;
                        if (valueSelected == _GLAcctName)
                        {
                            item.Selected = true;
                        }
                        ilSelectList.Add(item);
                    }
                }
            }


            ViewData[DropDownToBind] = ilSelectList;


        }

        protected void LoadVendors(string DropDownToBind, string valueSelected)
        {

            string _QBListXMLFile = Server.MapPath("~/App_Data/QuickBooksDesktop/" + "JMAQB-VendorQueryRs.xml");
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();
            if (System.IO.File.Exists(_QBListXMLFile))
            {
                XmlDocument _InDoc = new XmlDocument();
                _InDoc.Load(_QBListXMLFile);

                // Get GL Accounts
                XmlNodeList _GLAccts = _InDoc.SelectNodes("//VendorRet");
                foreach (XmlNode _Node in _GLAccts)
                {
                    string _GLAcctName = _Node.SelectSingleNode("Name").InnerText;

                    SelectListItem item = new SelectListItem();
                    item.Text = _GLAcctName;
                    item.Value = _GLAcctName;
                    if (valueSelected == _GLAcctName)
                    {
                        item.Selected = true;
                    }
                    ilSelectList.Add(item);
                }
            }
            ViewData[DropDownToBind] = ilSelectList;
        }

        private void BindData(QuickBooksModel model)
        {
            GetDataMode(model);
            //assign drop downs
            GetCustomerID(model);
            GetInvoiceMode(model);
            GetOrderStatus(model);
            LoadGLAccts("OtherCurrentAsset", "ItemAssetAcct", model.ItemAssetAcct);
            LoadVendors("SalesTaxVendor", model.SalesTaxVendor);
            //LoadGLAccts("OtherAsset", ref list_ItemsAssetAcct);
            //LoadGLAccts("OtherCurrentAsset", ref list_ItemsAssetAcct);
            LoadGLAccts("CostOfGoodsSold", "ItemCOGSAcct", model.ItemCOGSAcct);
            LoadGLAccts("Income", "ItemIncomeAcct", model.ItemIncomeAcct);
            LoadGLAccts("", "InvDiscAcct", model.InvDiscAcct);
            LoadGLAccts("AccountsReceivable", "ARAAccount", model.ARAAccount);
            LoadGLAccts("AccountsReceivable", "PmtARAccount", model.PmtARAccount);
            LoadGLAccts("AccountsPayable", "BillAPAAccount", model.BillAPAAccount);
            LoadGLAccts("Bank", "DefaultAccount", model.DefaultAccount);


        }


        /// <summary>
        /// sets up values for first time control loads in admin
        /// </summary>
        /// <returns></returns>
        public ActionResult Configure()
        {
            var model = new QuickBooksModel();

            model.ARAAccount = _quickbooksSettings.ARAAccount;
            model.BillAPAAccount = _quickbooksSettings.BillAPAAccount;
            model.ClassRef = _quickbooksSettings.ClassRef;
            model.ConnectionString = _quickbooksSettings.ConnectionString;
            model.CustomerID = _quickbooksSettings.CustomerID;
            model.DataMode = _quickbooksSettings.DataMode;
            model.DebugMode = _quickbooksSettings.DebugMode;
            model.DefaultAccount = _quickbooksSettings.DefaultAccount;
            model.DeleteQBDuplicates = _quickbooksSettings.DeleteQBDuplicates;
            model.HighestOrder = _quickbooksSettings.HighestOrder;
            model.InvDiscAcct = _quickbooksSettings.InvDiscAcct;
            model.InvoiceMode = _quickbooksSettings.InvoiceMode;
            model.InsertZeroOrders = _quickbooksSettings.InsertZeroOrders;

            model.ItemAssetAcct = _quickbooksSettings.ItemAssetAcct;
            model.ItemCOGSAcct = _quickbooksSettings.ItemCOGSAcct;
            model.ItemIncomeAcct = _quickbooksSettings.ItemIncomeAcct;
            model.Licensekey = _quickbooksSettings.Licensekey;
            model.OrderPrefix = _quickbooksSettings.OrderPrefix;
            model.POSSyncImages = _quickbooksSettings.POSSyncImages;
            model.QuickBooksOnlineEmail = _quickbooksSettings.QuickBooksOnlineEmail;
            //for trial users...
            if (String.IsNullOrWhiteSpace(_quickbooksSettings.Licensekey))
            {
                model.Licensekey = "TRIAL";
            }
            else
            {
                model.Licensekey = _quickbooksSettings.Licensekey;
            }



            if (_quickbooksSettings.LastDownloadUtc == null)
            {
                model.LastDownloadUtc = DateTime.Now;
            }
            else
            {
                model.LastDownloadUtc = _quickbooksSettings.LastDownloadUtc;
            }
            model.LowestOrder = _quickbooksSettings.LowestOrder;
            model.OrderLimit = _quickbooksSettings.OrderLimit;
            model.OrderStatus = _quickbooksSettings.OrderStatus;
            model.PmtARAccount = _quickbooksSettings.PmtARAccount;
            model.SalesTaxVendor = _quickbooksSettings.SalesTaxVendor;
            model.SetExported = _quickbooksSettings.SetExported;
            model.StartOrderNumber = _quickbooksSettings.StartOrderNumber;
            model.VoidOnly = _quickbooksSettings.VoidOnly;
            model.UpdateInStockFromQB = _quickbooksSettings.UpdateInStockFromQB;
            model.ToBePrinted = _quickbooksSettings.ToBePrinted;
            model.MarkOrderComplete = _quickbooksSettings.MarkOrderComplete;
            model.UseMultiCurrency = _quickbooksSettings.UseMultiCurrency;
            model.UseUK = _quickbooksSettings.UseUK;
            qbModel = model;
            BindData(qbModel);

            return View("~/Plugins/Accounting.QuickBooks/Views/QuickBooks/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("installautosync")]
        public ActionResult InstallAutoSync(QuickBooksModel model)
        {
            using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/Settings.txt")))
            {
                string connection = new DataSettingsManager().LoadSettings().DataConnectionString;
                //string connection = sr.ReadToEnd().Replace("DataProvider: sqlserver\r\nDataConnectionString: ", "");
                SqlConnection conn = new SqlConnection(connection);
                conn.Open();
                string command = String.Format(@"Select * from ScheduleTask Where Name = 'QuickBooks Sync' If @@RowCount = 0 INSERT INTO [ScheduleTask] ([Name],[Seconds],[Type],[Enabled],[StopOnError],[LastStartUtc], [LastEndUtc]) VALUES ('QuickBooks Sync',86400,'Nop.Plugin.Accounting.QuickBooks.QBTask, Nop.Plugin.Accounting.QuickBooks',1,1,'{0}','{1}')", DateTime.UtcNow, DateTime.UtcNow);
                errorMessageDataSource.WriteToLog(new ErrorMessage(MessageSeverity.Info, "QBFS", "Installing auto sync: " + command, string.Empty));
                SqlCommand comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
                conn.Close();
            }

            return RedirectToAction("List", "ScheduleTask");
        }

        /// <summary>
        /// saves settings to nopCommerce in admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("build")]
        public ActionResult BuildQWC(QuickBooksModel model)
        {
            model.TestingResult = uiSettings.BuildQWC();
            BindData(model);
            return View("~/Plugins/Accounting.QuickBooks/Views/QuickBooks/Configure.cshtml", model);

        }


        /// <summary>
        /// saves settings to nopCommerce in admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(QuickBooksModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }
            _quickbooksSettings.ARAAccount = model.ARAAccount;
            _quickbooksSettings.BillAPAAccount = model.BillAPAAccount;
            _quickbooksSettings.ClassRef = model.ClassRef;
            _quickbooksSettings.ConnectionString = model.ConnectionString;
            _quickbooksSettings.CustomerID = model.CustomerID;
            _quickbooksSettings.DataMode = model.DataMode;
            _quickbooksSettings.DebugMode = model.DefaultAccount;
            _quickbooksSettings.DefaultAccount = model.DefaultAccount;
            _quickbooksSettings.DeleteQBDuplicates = model.DeleteQBDuplicates;
            _quickbooksSettings.InvDiscAcct = model.InvDiscAcct;
            _quickbooksSettings.InvoiceMode = model.InvoiceMode;
            _quickbooksSettings.InsertZeroOrders = model.InsertZeroOrders;


            _quickbooksSettings.ItemAssetAcct = model.ItemAssetAcct;
            _quickbooksSettings.ItemCOGSAcct = model.ItemCOGSAcct;
            _quickbooksSettings.ItemIncomeAcct = model.ItemIncomeAcct;
            _quickbooksSettings.Licensekey = model.Licensekey.Trim();
            _quickbooksSettings.OrderLimit = model.OrderLimit;
            _quickbooksSettings.OrderPrefix = model.OrderPrefix;
            _quickbooksSettings.OrderStatus = model.OrderStatus;
            _quickbooksSettings.PmtARAccount = model.PmtARAccount;
            _quickbooksSettings.POSSyncImages = model.POSSyncImages;
            _quickbooksSettings.QuickBooksOnlineEmail = model.QuickBooksOnlineEmail;
            _quickbooksSettings.SalesTaxVendor = model.SalesTaxVendor;
            _quickbooksSettings.SetExported = model.SetExported;
            _quickbooksSettings.StartOrderNumber = model.StartOrderNumber;
            _quickbooksSettings.VoidOnly = model.VoidOnly;
            //if string order is being user, make the high and low numbers 0
            if (!String.IsNullOrEmpty(model.StringOrders))
            {
                model.LowestOrder = 0;
                model.HighestOrder = 0;
            }

            _quickbooksSettings.LowestOrder = model.LowestOrder;
            _quickbooksSettings.HighestOrder = model.HighestOrder;
            _quickbooksSettings.UpdateInStockFromQB = model.UpdateInStockFromQB;
            _quickbooksSettings.ToBePrinted = model.ToBePrinted;
            _quickbooksSettings.MarkOrderComplete = model.MarkOrderComplete;
            _quickbooksSettings.UseUK = model.UseUK;
            _quickbooksSettings.UseMultiCurrency = model.UseMultiCurrency;
            _settingService.SaveSetting(_quickbooksSettings);



            model.TestingResult = uiSettings.MakeError(_quickbooksSettings.Licensekey, _quickbooksSettings.QuickBooksTrialStartDate);
            errorMessageDataSource.WriteToLog(new ErrorMessage(MessageSeverity.Info, "QBFS", model.TestingResult, string.Empty));

            BindData(model);
            return View("~/Plugins/Accounting.QuickBooks/Views/QuickBooks/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("clear")]
        public ActionResult ClearLog(QuickBooksModel model)
        {
            model.TestingResult = uiSettings.ClearLog();
            BindData(model);
            return View("~/Plugins/Accounting.QuickBooks/Views/QuickBooks/Configure.cshtml", model);
        }
    }
}