using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class QuickBooksSettings : ISettings
    {
        public DateTime LastDownloadUtc { get; set; }
        public DateTime LastDownloadUtcEnd { get; set; }

        public string SolutionName { get; set; }
        public string SolutionUserName { get; set; }
        public string SolutionPassword { get; set; }
        public string SolutionWebsite { get; set; }
        public string StoreName { get; set; }
        public string StringOrders { get; set; }
        public int LowestOrder { get; set; }
        public int HighestOrder { get; set; }
    }
}