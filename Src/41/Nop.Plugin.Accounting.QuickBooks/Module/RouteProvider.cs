using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Plugin.Accounting.QuickBooks.AuthenticateToWebservice", "Plugins/QuickBooks/AuthenticateToWebservice",
                 new { controller = "QuickBooks", action = "AuthenticateToWebservice" });
            routeBuilder.MapRoute("Plugin.Accounting.QuickBooks.Orders", "Plugins/QuickBooks/Orders",
     new { controller = "QuickBooks", action = "Orders" });
            routeBuilder.MapRoute("Plugin.Accounting.QuickBooks.StockUpdate", "Plugins/QuickBooks/Inventory",
new { controller = "QuickBooks", action = "Inventory" });
            routeBuilder.MapRoute("Plugin.Accounting.QuickBooks.Stores", "Plugins/QuickBooks/Stores",
new { controller = "QuickBooks", action = "Stores" });
            routeBuilder.MapRoute("Plugin.Accounting.QuickBooks.ChangeOrderStatus", "Plugins/QuickBooks/ChangeOrderStatus",
new { controller = "QuickBooks", action = "ChangeOrderStatus" });
            routeBuilder.MapRoute("Plugin.Accounting.QuickBooks.Products", "Plugins/QuickBooks/CreateProduct",
new { controller = "QuickBooks", action = "CreateProduct" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority
        {
            get { return -1; }
        }
    }
}
