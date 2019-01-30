using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JMA.Plugin.Accounting.QuickBooks;
using JMA.Plugin.Accounting.QuickBooks.LogProvider;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class NopCommerceLogProvider : IJMALogProvider
    {

        public string WriteToLog(ErrorMessage message)
        {
            ILogger log = EngineContext.Current.Resolve<ILogger>();
            log.InsertLog(Core.Domain.Logging.LogLevel.Information, message.ApplicationName, message.Message);
            return "OK";
        }

        public string ClearLog()
        {
            ILogger log = EngineContext.Current.Resolve<ILogger>();
            log.ClearLog();
            return "OK";
        }


        public string ClearOldLog()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> ErrorList
        {
            get { throw new NotImplementedException(); }
        }
    }
}
