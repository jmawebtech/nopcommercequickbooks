using ConnexForQuickBooks.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Accounting.QuickBooks.Connection;
using Nop.Plugin.Accounting.QuickBooks.Helper;
using Nop.Web.Framework.Controllers;
using System.IO;
using System.Text;

namespace Nop.Plugin.Accounting.QuickBooks.Controllers
{
    public class QuickBooksController: BasePluginController
    {
        AuthenticateHelper helper;
        JMAWebServiceCommon jmaWebServiceCommon;

        public QuickBooksController()
        {
            helper = new AuthenticateHelper(this);
            jmaWebServiceCommon = new JMAWebServiceCommon();
        }

        /// <summary>
        /// Tests whether the user name and password is correct and the user belongs to the admin role.
        /// </summary>
        /// <returns></returns>
        public IActionResult AuthenticateToWebservice()
        {
            bool result = helper.AuthenticateToWebservice();
            return new ObjectResult(result);
        }

        /// <summary>
        /// Change an order status to complete, adds tracking details, and marks an order as paid.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeOrderStatus()
        {
            JMAOrder order = GetOrder();

            bool result = helper.AuthenticateToWebservice();

            if (!result)
                return new ObjectResult("Authentication failed. Please ensure your nopCommerce user name and password is correct and the user is in the admin role.");

            string updateResult = jmaWebServiceCommon.ChangeOrderStatus(order);
            return new ObjectResult(updateResult);
        }

        private JMAOrder GetOrder()
        {
            JMAOrder order = null;

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string content = reader.ReadToEnd();
                order = JsonConvert.DeserializeObject<JMAOrder>(content);
            }

            return order;
        }

        [HttpPost]
        public IActionResult CreateProduct()
        {
            JMAProduct product = GetProduct();

            string result = jmaWebServiceCommon.MakeProduct(product);
            return new ObjectResult(result);
        }

        private JMAProduct GetProduct()
        {
            JMAProduct product = null;

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                string content = reader.ReadToEnd();
                product = JsonConvert.DeserializeObject<JMAProduct>(content);
            }

            return product;
        }

        public IActionResult Stores()
        {
            return new ObjectResult(jmaWebServiceCommon.GetAllStores());
        }

        /// <summary>
        /// Pulls a list of orders from nopCommerce by order number or creation date.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public IActionResult Orders(QuickBooksSettings settings)
        {
            bool result = helper.AuthenticateToWebservice();

            if (!result)
                return new ObjectResult("Authentication failed. Please ensure your nopCommerce user name and password is correct and the user is in the admin role.");

            JMAUser theUser = new JMAUser();
            OrderConnection connection = new OrderConnection(settings);
            theUser.Orders = connection.Search();
            return new ObjectResult(theUser);
        }

        /// <summary>
        /// Update pricing and stock levels of nopCommerce products.
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="stockQuantity"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public IActionResult Inventory(string sku, decimal? stockQuantity, decimal? price)
        {
            bool result = helper.AuthenticateToWebservice();

            if (!result)
                return new ObjectResult("Authentication failed. Please ensure your nopCommerce user name and password is correct and the user is in the admin role.");

            string updateResult = jmaWebServiceCommon.UpdateInventory(sku, stockQuantity, price);

            return new ObjectResult(updateResult);
        }
    }
}