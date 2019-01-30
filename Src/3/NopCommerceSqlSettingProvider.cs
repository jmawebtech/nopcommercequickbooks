using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JMA.Plugin.Accounting.QuickBooks.DTO;
using JMA.Plugin.Accounting.QuickBooks.Encrypt;
using JMA.Plugin.Accounting.QuickBooks.SettingProvider;
using Nop.Services.Configuration;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class NopCommerceSqlSettingProvider : ISqlSettingProvider
    {
        QuickBooksSettings _qboSettings;
        ISettingService _settingService;

        public NopCommerceSqlSettingProvider(QuickBooksSettings quickBooksSettings, ISettingService settingService)
        {
            _qboSettings = quickBooksSettings;
            _settingService = settingService;
        }

        public string ClearSettings()
        {
            return "OK";
        }

        public static DateTime GetQBTrialStartDate(string date)
        {
            string v = CryptographyHelper.DecryptData(date, string.Empty);
            if (String.IsNullOrEmpty(v))
            {
                return DateTime.Now;
            }

            return DateTime.Parse(v);
        }

        public string Install()
        {
            return "OK";
        }

        public bool IsTrialOrValidLicensekey(string text, JMASettings qboSettings)
        {
            return true;
        }

        public string Uninstall()
        {
            return "OK";
        }

        public string Update_BulkSetting(JMASettings qboSettings, string userName = "admin", string eComSystem = "")
        {
            return "OK";
        }


        public JMASettings GetSettings(string userName = "admin", string password = "admin", string eComSystem = "")
        {
            JMASettings theUser = new JMASettings();
            theUser.ARAAccount = _qboSettings.ARAAccount;
            theUser.BillAPAAccount = _qboSettings.BillAPAAccount;
            theUser.ConnectionString = _qboSettings.ConnectionString;
            theUser.CustomerID = _qboSettings.CustomerID;
            theUser.DataMode = _qboSettings.DataMode;
            theUser.DefaultAccount = _qboSettings.DefaultAccount;
            theUser.DeleteQBDuplicates = _qboSettings.DeleteQBDuplicates;
            theUser.HighestOrder = _qboSettings.HighestOrder;
            theUser.InvoiceMode = _qboSettings.InvoiceMode;
            theUser.InsertZeroOrders = _qboSettings.InsertZeroOrders;
            theUser.InvDiscAcct = _qboSettings.InvDiscAcct;
            theUser.ItemAssetAcct = _qboSettings.ItemAssetAcct;
            theUser.ItemCOGSAcct = _qboSettings.ItemCOGSAcct;
            theUser.ItemIncomeAcct = _qboSettings.ItemIncomeAcct;
            theUser.Licensekey = _qboSettings.Licensekey;
            theUser.MarkOrderComplete = _qboSettings.MarkOrderComplete;
            theUser.PmtARAccount = _qboSettings.PmtARAccount;
            theUser.StringOrders = _qboSettings.StringOrders;


            if (_qboSettings.LastDownloadUtc == null)
            {
                theUser.LastDownloadUtc = DateTime.Now;
            }
            else
            {
                theUser.LastDownloadUtc = Convert.ToDateTime(_qboSettings.LastDownloadUtc);
            }

            theUser.LowestOrder = _qboSettings.LowestOrder;
            theUser.OrderLimit = _qboSettings.OrderLimit;
            theUser.OrderPrefix = _qboSettings.OrderPrefix;
            theUser.OrderStatus = _qboSettings.OrderStatus;
            theUser.SalesTaxVendor = _qboSettings.SalesTaxVendor;
            theUser.SetExported = _qboSettings.SetExported;
            theUser.StartOrderNumber = _qboSettings.StartOrderNumber;
            theUser.StringOrders = _qboSettings.StringOrders;
            theUser.ToBePrinted = _qboSettings.ToBePrinted;
            theUser.UseUK = _qboSettings.UseUK;
            theUser.UpdateInStockFromQB = _qboSettings.UpdateInStockFromQB;
            theUser.QuickBooksTrialStartDate = _qboSettings.QuickBooksTrialStartDate;
            theUser.QuickBooksOnlineEmail = _qboSettings.QuickBooksOnlineEmail;
            theUser.UseMultiCurrency = _qboSettings.UseMultiCurrency;
            return theUser;

        }

        public string Update_LastDownloadDate(string userName = "admin", string eComSystem = "")
        {
            return "OK";
        }

        public string WriteSetting(string name, string value, string userName = "admin", string password = "admin", string eComSystem = "")
        {
            name = name.Replace("QB:", "quickbookssettings.");
            value = CryptographyHelper.DecryptData(value, string.Empty);
            _settingService.SetSetting(name, value);
            return "OK";

        }


        public string GetPaymentProcessorQBAccount(string email)
        {
            throw new NotImplementedException();
        }
    }
}
