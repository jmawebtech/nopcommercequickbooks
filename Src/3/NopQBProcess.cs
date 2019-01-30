using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JMA.Plugin.Accounting.QuickBooks;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Web.Framework.Menu;
using System.Web.Routing;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class NopQBProcess : BasePlugin, IAdminMenuPlugin, INopQBProcess
    {

        public override void Install()
        {
            base.Install();
            var _settingService = EngineContext.Current.Resolve<ISettingService>();
            QuickBooksSettings settings = new QuickBooksSettings();
            settings.LastDownloadUtc = DateTime.Now;
            settings.LastDownloadUtcEnd = DateTime.Now.AddDays(1);
            settings.HighestOrder = 0;
            settings.LowestOrder = 0;
            settings.QuickBooksTrialStartDate = DateTime.MinValue;
            _settingService.SaveSetting<QuickBooksSettings>(settings);

        }

        public override void Uninstall()
        {
            base.Uninstall();
            var _settingService = EngineContext.Current.Resolve<ISettingService>();

            //uninstall scheduled task

            string connection = new DataSettingsManager().LoadSettings().DataConnectionString;
            SqlConnection conn = new SqlConnection(connection);
            conn.Open();
            string command = String.Format(@"Delete from ScheduleTask Where Name = '{0}'", "QuickBooks Sync");
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();

            //_settingService.DeleteSetting<QuickBooksSettings>();
        }

        public bool Authenticate()
        {
            return true;
        }


        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Admin.Plugin.QuickBooks.Configure",
                Title = "QuuckBooks",
                ControllerName = "QuickBooks",
                ActionName = "Configure",
                Url = "/Admin/Plugin/QuickBooks/Admin/Configure",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            rootNode.RouteValues.Add("Admin.Plugin.QuickBooks.Configure", "Admin.Plugin.QuickBooks.Configure");

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);

        }
    }
}
