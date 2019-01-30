using System;
using System.Web.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Tax.AvalaraTax.Models;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using Nop.Services.Tax;
using Nop.Services.Customers;
using Nop.Plugin.Tax.AvalaraTax.Interfaces;
using System.Collections.Generic;
using Nop.Services.Logging;
using System.Linq;

namespace Nop.Plugin.Tax.AvalaraTax.Controllers
{
    [AdminAuthorize]
    public class TaxAvalaraController : Controller
    {
        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly ISettingService _settingService;
        private IAvalaraTaxProvider _avalaraTaxProvider;
        private JMALogProvider m_TextLogProvider;

        public TaxAvalaraController(AvalaraTaxSettings avalaraTaxSettings, ISettingService settingService, IAvalaraTaxProvider avalaraTaxProvider, ILogger logger)
        {
            this._avalaraTaxSettings = avalaraTaxSettings;
            this._settingService = settingService;
            _avalaraTaxProvider = avalaraTaxProvider;
            m_TextLogProvider = new JMALogProvider(avalaraTaxSettings, logger);

        }

        /// <summary>
        /// sets up values for first time control loads in admin
        /// </summary>
        /// <returns></returns>
        public ActionResult Configure()
        {
            var model = new TaxAvalaraModel();
            model.AvalaraAccount = _avalaraTaxSettings.AvalaraAccount;

            //for trial users...
            if (String.IsNullOrWhiteSpace(_avalaraTaxSettings.AvalaraJMALicenseKey))
            {
                model.AvalaraJMALicenseKey = "TRIAL";
            }
            else
            {
                model.AvalaraJMALicenseKey = _avalaraTaxSettings.AvalaraJMALicenseKey;
            }

            model.AvalaraAddressCountries = _avalaraTaxSettings.AvalaraAddressCountries;
            model.AvalaraCompanyCode = _avalaraTaxSettings.AvalaraCompanyCode;
            model.AvalaraDoAddress = _avalaraTaxSettings.AvalaraDoAddress;
            model.AvalaraDoAddressAll = _avalaraTaxSettings.AvalaraDoAddressAll;
            model.AvalaraDocCodePrefix = _avalaraTaxSettings.AvalaraDocCodePrefix;
            model.AvalaraEventLog = _avalaraTaxSettings.AvalaraEventLog;

            model.AvalaraLicense = _avalaraTaxSettings.AvalaraLicense;
            model.AvalaraLogTransactions = _avalaraTaxSettings.AvalaraLogTransactions;
            model.AvalaraLogTransactionsInNopCommerce = _avalaraTaxSettings.AvalaraLogTransactionsInNopCommerce;
            model.AvalaraTaxStates = _avalaraTaxSettings.AvalaraTaxStates;
            model.AvalaraURL = _avalaraTaxSettings.AvalaraURL;
            model.AvalaraUse = _avalaraTaxSettings.AvalaraUse;
            model.AvalaraUseState = _avalaraTaxSettings.AvalaraUseState;
            model.DoAvalaraReturnInvoice = _avalaraTaxSettings.DoAvalaraReturnInvoice;
            model.HighestOrderNumber = _avalaraTaxSettings.HighestOrderNumber;
            model.LowestOrderNumber = _avalaraTaxSettings.LowestOrderNumber;
            model.AvalaraOrderStatus = _avalaraTaxSettings.AvalaraOrderStatus;
            model.NeverUseInsertTax = _avalaraTaxSettings.NeverUseInsertTax;
            model.AvalaraShippingCode = _avalaraTaxSettings.AvalaraShippingCode;
            model.UseAvalaraDefaultTaxClass = _avalaraTaxSettings.UseAvalaraDefaultTaxClass;
            model.AvalaraCustomerReference = _avalaraTaxSettings.AvalaraCustomerReference;

            model.AvalaraShippingOriginAddress1 = _avalaraTaxSettings.AvalaraShippingOriginAddress1;
            model.AvalaraShippingOriginCity = _avalaraTaxSettings.AvalaraShippingOriginCity;
            model.AvalaraShippingOriginState = _avalaraTaxSettings.AvalaraShippingOriginState;
            model.AvalaraShippingOriginZip = _avalaraTaxSettings.AvalaraShippingOriginZip;
            model.AvalaraShippingOriginCountry = _avalaraTaxSettings.AvalaraShippingOriginCountry;
            model.AvalaraShippingOriginAddress2 = _avalaraTaxSettings.AvalaraShippingOriginAddress2;
            model.AvalaraLogOptions = GetLogMode(model);
            model.AvalaraOrderStatuses = GetOrderStatus(model);
            model.AvalaraCustomerReferences = GetCustomerReference(model);

            return View("~/Plugins/Tax.Avalara/Views/TaxAvalara/Configure.cshtml", model);
        }

        public List<SelectListItem> GetLogMode(TaxAvalaraModel model)
        {
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();
            SelectListItem sl = new SelectListItem();
            sl.Text = "No Logging";

            if (!model.AvalaraLogTransactions && !model.AvalaraLogTransactionsInNopCommerce)
                sl.Selected = true;

            ilSelectList.Add(sl);

            sl = new SelectListItem();
            sl.Text = "In Text File";

            if (model.AvalaraLogTransactions)
                sl.Selected = true;

            ilSelectList.Add(sl);

            sl = new SelectListItem();
            sl.Text = "In Database";

            if (model.AvalaraLogTransactionsInNopCommerce)
                sl.Selected = true;

            ilSelectList.Add(sl);

            return ilSelectList.ToList();

        }


        public List<SelectListItem> GetOrderStatus(TaxAvalaraModel model)
        {
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();

            SelectListItem Complete = new SelectListItem();
            Complete.Text = "Complete";
            Complete.Value = "Complete";
            if (model.AvalaraOrderStatus == "Complete")
            {
                Complete.Selected = true;
            }
            ilSelectList.Add(Complete);


            SelectListItem Canceled = new SelectListItem();
            Canceled.Text = "Cancelled";
            Canceled.Value = "Cancelled";
            if (model.AvalaraOrderStatus == "Cancelled")
            {
                Canceled.Selected = true;
            }
            ilSelectList.Add(Canceled);



            SelectListItem Pending = new SelectListItem();
            Pending.Text = "Pending";
            Pending.Value = "Pending";
            if (model.AvalaraOrderStatus == "Pending")
            {
                Pending.Selected = true;
            }
            ilSelectList.Add(Pending);


            SelectListItem Processing = new SelectListItem();
            Processing.Text = "Processing";
            Processing.Value = "Processing";
            if (model.AvalaraOrderStatus == "Processing")
            {
                Processing.Selected = true;
            }
            ilSelectList.Add(Processing);


            return ilSelectList.ToList();

        }

        public List<SelectListItem> GetCustomerReference(TaxAvalaraModel model)
        {
            IList<SelectListItem> ilSelectList = new List<SelectListItem>();

            SelectListItem customerReferenceEmail = new SelectListItem();
            customerReferenceEmail.Text = "Email";
            customerReferenceEmail.Value = "Email";
            if (!String.IsNullOrEmpty(model.AvalaraCustomerReference) || model.AvalaraCustomerReference == "Email")
            {
                customerReferenceEmail.Selected = true;
            }

            ilSelectList.Add(customerReferenceEmail);

            SelectListItem customerIdReference = new SelectListItem();
            customerIdReference.Text = "Id";
            customerIdReference.Value = "Id";
            if (model.AvalaraCustomerReference == "Id")
            {
                customerIdReference.Selected = true;
            }

            ilSelectList.Add(customerIdReference);

            return ilSelectList.ToList();

        }

        /// <summary>
        /// determines whether user has a valid trial or license key is valid. 
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// determines whether user has a valid trial or license key is valid. 
        /// </summary>
        /// <returns></returns>
        private bool IsTrialOrValidLicenseKey()
        {
            _avalaraTaxSettings.AvalaraTrialStartDate = DateTime.MinValue;
            //trial key, within 30 days
            if (_avalaraTaxSettings.AvalaraJMALicenseKey == "TRIAL")
            {
                //I need to set the start date. First check and see if it's min date

                if (_avalaraTaxSettings.AvalaraTrialStartDate.Year < 2012)
                {
                    _avalaraTaxSettings.AvalaraTrialStartDate = DateTime.MinValue;
                }

                //30 days has expired.
                //If the user has mysteriously typed in a production key and then a trial key, make sure I don't add days to max value
                if (_avalaraTaxSettings.AvalaraTrialStartDate.Year != 9999)
                {
                    if (_avalaraTaxSettings.AvalaraTrialStartDate.AddDays(60) >= DateTime.Now)
                    {

                        m_TextLogProvider.WriteLogFile("Trial license expired. Please purchase a license key from JMA Web Technologies.");
                        return false;
                    }
                }


                m_TextLogProvider.WriteLogFile("Trial license activated. Trial will expire on " + _avalaraTaxSettings.AvalaraTrialStartDate.AddDays(60));

                //for the first install, trial date has a min value. For the trial, make it today's date
                if (_avalaraTaxSettings.AvalaraTrialStartDate == DateTime.MinValue)
                {
                    _avalaraTaxSettings.AvalaraTrialStartDate = DateTime.Now;
                }
                return true;
            }
            else
            {
                if (_avalaraTaxSettings.AvalaraJMALicenseKey.Length == 36)
                {
                    _avalaraTaxSettings.AvalaraTrialStartDate = DateTime.MinValue;
                    return true;
                }
            }

            //no trial key
            _avalaraTaxSettings.AvalaraTrialStartDate = DateTime.MinValue;
            _settingService.SaveSetting(_avalaraTaxSettings);
            return false;
        }
        /// <summary>
        /// saves settings to nopCommerce in admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(TaxAvalaraModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            _avalaraTaxSettings.AvalaraAccount = model.AvalaraAccount;
            _avalaraTaxSettings.AvalaraAddressCountries = model.AvalaraAddressCountries;
            _avalaraTaxSettings.AvalaraCompanyCode = model.AvalaraCompanyCode;
            _avalaraTaxSettings.AvalaraDoAddress = model.AvalaraDoAddress;
            _avalaraTaxSettings.AvalaraDoAddressAll = model.AvalaraDoAddressAll;
            _avalaraTaxSettings.AvalaraDocCodePrefix = model.AvalaraDocCodePrefix;
            _avalaraTaxSettings.AvalaraEventLog = model.AvalaraEventLog;
            _avalaraTaxSettings.AvalaraJMALicenseKey = model.AvalaraJMALicenseKey;
            _avalaraTaxSettings.AvalaraLicense = model.AvalaraLicense;

            if (model.AvalaraLogOptionsResponse.Contains("In Database"))
                model.AvalaraLogTransactionsInNopCommerce = true;
            else if (model.AvalaraLogOptionsResponse.Contains("In Text File"))
                model.AvalaraLogTransactions = true;

            _avalaraTaxSettings.AvalaraLogTransactionsInNopCommerce = model.AvalaraLogTransactionsInNopCommerce;
            _avalaraTaxSettings.AvalaraLogTransactions = model.AvalaraLogTransactions;

            _avalaraTaxSettings.AvalaraOrderStatus = model.AvalaraOrderStatus;
            _avalaraTaxSettings.AvalaraTaxStates = model.AvalaraTaxStates;
            _avalaraTaxSettings.AvalaraURL = model.AvalaraURL;
            _avalaraTaxSettings.AvalaraUse = model.AvalaraUse;
            _avalaraTaxSettings.AvalaraUseState = model.AvalaraUseState;
            _avalaraTaxSettings.LowestOrderNumber = model.LowestOrderNumber;
            _avalaraTaxSettings.HighestOrderNumber = model.HighestOrderNumber;
            _avalaraTaxSettings.DoAvalaraReturnInvoice = model.DoAvalaraReturnInvoice;
            _avalaraTaxSettings.NeverUseInsertTax = model.NeverUseInsertTax;
            _avalaraTaxSettings.AvalaraShippingCode = model.AvalaraShippingCode;
            _avalaraTaxSettings.AvalaraCustomerReference = model.AvalaraCustomerReference;
            _avalaraTaxSettings.AvalaraShippingOriginAddress1 = model.AvalaraShippingOriginAddress1;
            _avalaraTaxSettings.AvalaraShippingOriginAddress2 = model.AvalaraShippingOriginAddress2;
            _avalaraTaxSettings.AvalaraShippingOriginCity = model.AvalaraShippingOriginCity;
            _avalaraTaxSettings.AvalaraShippingOriginState = model.AvalaraShippingOriginState;
            _avalaraTaxSettings.AvalaraShippingOriginZip = model.AvalaraShippingOriginZip;
            _avalaraTaxSettings.AvalaraShippingOriginCountry = model.AvalaraShippingOriginCountry;
            _avalaraTaxSettings.UseAvalaraDefaultTaxClass = model.UseAvalaraDefaultTaxClass;

            if (IsTrialOrValidLicenseKey())
            {
                _settingService.SaveSetting(_avalaraTaxSettings);

                //authenticate to Avalara and save the settings
                try
                {
                    string results = _avalaraTaxProvider.TestConnection(_avalaraTaxSettings.AvalaraAccount, _avalaraTaxSettings.AvalaraLicense, _avalaraTaxSettings.AvalaraURL, _avalaraTaxSettings.AvalaraUseState, _avalaraTaxSettings.AvalaraTaxStates, _avalaraTaxSettings.AvalaraCompanyCode, _avalaraTaxSettings.AvalaraLogTransactions, _avalaraTaxSettings.AvalaraJMALicenseKey, _avalaraTaxSettings.AvalaraEventLog, _avalaraTaxSettings.AvalaraDoAddress, _avalaraTaxSettings.AvalaraAddressCountries, _avalaraTaxSettings.AvalaraDocCodePrefix);

                    model.TestingResult = results;

                    if (_avalaraTaxSettings.AvalaraJMALicenseKey == "TRIAL")
                    {
                        model.TestingResult += " Your trial will expire on: " + DateTime.Now.AddDays(30);
                    }

                }
                catch (Exception exc)
                {
                    model.TestingResult = exc.ToString();
                    m_TextLogProvider.WriteLogFile(exc.ToString());
                }
            }
            else
            {
                string err = "Your JMA License Key is invalid or your trial has expired. Please ensure you are entering the right key from JMA Web Technologies.";
                model.TestingResult = err;
                m_TextLogProvider.WriteLogFile(err);
            }

            model.AvalaraLogOptions = GetLogMode(model);
            model.AvalaraOrderStatuses = GetOrderStatus(model);
            model.AvalaraCustomerReferences = GetCustomerReference(model);
            ViewBag.Message = model.TestingResult;
            return View("~/Plugins/Tax.Avalara/Views/TaxAvalara/Configure.cshtml", model);
        }

        /// <summary>
        /// performs batch processing
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("batch")]
        public ActionResult ConfigureBatch(TaxAvalaraModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            try
            {
                var result = _avalaraTaxProvider.BatchData(model.LowestOrderNumber, model.HighestOrderNumber, model.DoAvalaraReturnInvoice);


                if (!result)
                {
                    model.BatchResponse = "Some errors have occured. Please check your Avalara admin to ensure that this was successful. If it was, then you can ignore this message. For error details, please check the log file: avalaralog.txt";
                }
                else
                {
                    model.BatchResponse = "All transactions have completed successfully!";
                }


            }
            catch (Exception)
            {
                model.BatchResponse = "There are no orders to process.";
                //model.BatchResponse = exc.ToString();
            }

            model.AvalaraLogOptions = GetLogMode(model);
            model.AvalaraOrderStatuses = GetOrderStatus(model);
            model.AvalaraCustomerReferences = GetCustomerReference(model);

            ViewBag.Message = model.BatchResponse;
            return View("~/Plugins/Tax.Avalara/Views/TaxAvalara/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("clear")]
        public ActionResult ClearLog(TaxAvalaraModel model)
        {
            bool result = m_TextLogProvider.ClearLog("~/avalaralog.txt");
            if (result)
            {
                model.TestingResult = "Avalara log cleared successfully!";
            }
            else
            {
                model.TestingResult = "Log does not exist, so there was no log to clear.";
            }

            ViewBag.Message = model.TestingResult;

            return Configure();
        }


    }
}