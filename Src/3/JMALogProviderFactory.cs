using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JMA.Plugin.Accounting.QuickBooks.LogProvider;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class JMALogProviderFactory
    {
        public static IJMALogProvider Get()
        {
            IJMALogProvider errorMessageDataSource;
            if (ConfigurationManager.AppSettings["LogProvider"] != null && ConfigurationManager.AppSettings["LogProvider"] == "Host")
            {
                errorMessageDataSource = new NopCommerceLogProvider() as IJMALogProvider;
            }
            else
            {
                errorMessageDataSource = new TextLogProvider() as IJMALogProvider;
            }

            return errorMessageDataSource;
        }
    }
}
