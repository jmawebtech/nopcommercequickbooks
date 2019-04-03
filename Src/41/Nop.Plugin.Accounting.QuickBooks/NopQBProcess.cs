using Microsoft.AspNetCore.Routing;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Web.Framework.Menu;
using System;
using System.Linq;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class NopQBProcess : BasePlugin, IAdminMenuPlugin
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
            _settingService.SaveSetting<QuickBooksSettings>(settings);

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
                Title = "QuickBooks",
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
