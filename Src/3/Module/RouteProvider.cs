using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public partial class RouteProvider : IRouteProvider
    {
        //http://www.nopcommerce.com/boards/t/12699/admin-plugin-route.aspx?p=2
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Admin.Plugin.QuickBooks.Configure",
                 "Admin/Plugin/QuickBooks/Admin/Configure",
                 new { controller = "QuickBooks", action = "Configure" },
                 new[] { "Nop.Plugin.Accounting.QuickBooks.Controllers" }
            ).DataTokens.Add("area", "admin");
        }

        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
