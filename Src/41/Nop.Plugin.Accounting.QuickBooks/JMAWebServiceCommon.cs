using ConnexForQuickBooks.Model;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Plugin.Accounting.QuickBooks.Utility;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class JMAWebServiceCommon
    {
        public JMAWebServiceCommon()
        {
            _orderService = EngineContext.Current.Resolve<IOrderService>();
            _discountService = EngineContext.Current.Resolve<IDiscountService>();
            _settingService = EngineContext.Current.Resolve<ISettingService>();
            _encryptionService = EngineContext.Current.Resolve<IEncryptionService>();
            _checkoutAttributeService = EngineContext.Current.Resolve<ICheckoutAttributeService>();
            _checkoutAttributeParser = EngineContext.Current.Resolve<ICheckoutAttributeParser>();
            _paymentMethodService = EngineContext.Current.Resolve<IPaymentService>();
            _manufacturerService = EngineContext.Current.Resolve<IManufacturerService>();
            _dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
            _productService = EngineContext.Current.Resolve<IProductService>();
            _productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
            _storeService = EngineContext.Current.Resolve<IStoreService>();
        }

        [HttpPost]
        public string ChangeOrderStatus(JMAOrder jmaOrder)
        {
            int orderId = 0;

            int.TryParse(jmaOrder.Other, out orderId);

            if (orderId == 0)
                return "NOT FOUND";

            Order order = _orderService.GetOrderById(int.Parse(jmaOrder.Other));

            if (order == null)
                return "NOT FOUND";

            MapOrderStatus(jmaOrder, order);

            AddShipmentInfo(jmaOrder, order);

            order.PaidDateUtc = jmaOrder.PaidDate.HasValue && jmaOrder.PaidDate.Value.Year > 1 ? jmaOrder.PaidDate : jmaOrder.CreationDate;
            order.PaymentStatus = PaymentStatus.Paid;
            order.PaymentStatusId = 30;

            _orderService.UpdateOrder(order);

            return "OK";
        }

        private void AddShipmentInfo(JMAOrder jmaOrder, Order order)
        {
            Shipment sh = new Shipment();
            sh.CreatedOnUtc = DateTime.Now;
            sh.DeliveryDateUtc = DateTime.Now;
            sh.Order = order;
            sh.ShippedDateUtc = jmaOrder.ShippingDate.HasValue && jmaOrder.ShippingDate.Value.Year > 1 ? jmaOrder.ShippingDate : jmaOrder.CreationDate;
            sh.TrackingNumber = jmaOrder.TrackingNumber;
            sh.OrderId = order.Id;
            order.Shipments.Add(sh);
        }

        private void MapOrderStatus(JMAOrder jmaOrder, Order order)
        {
            if (String.IsNullOrEmpty(jmaOrder.OrderStatus))
            {
                order.OrderStatus = OrderStatus.Complete;
            }
            else
            {
                OrderStatus orderStatus;
                if (!String.IsNullOrEmpty(jmaOrder.OrderStatus))
                {
                    bool parseResult = Enum.TryParse(jmaOrder.OrderStatus, out orderStatus);
                    order.OrderStatus = orderStatus;
                }
            }
        }

        public string UpdateInventory(string sku, decimal? stockQuantity, decimal? price)
        {
            IProductService productService = EngineContext.Current.Resolve<IProductService>();

            Product pv = productService.GetProductBySku(sku);

            if (pv == null)
            {
                return "NOT FOUND";
            }

            //skip shipping and non inventory items
            if (pv.ManageInventoryMethod == ManageInventoryMethod.DontManageStock)
            {
                return "OK";
            }

            if (stockQuantity.HasValue)
                pv.StockQuantity = Convert.ToInt16(stockQuantity.Value);

            if (price != null && price.Value > 0)
                pv.Price = price.Value;

            productService.UpdateProduct(pv);
            return "OK";

        }

        public List<string> GetAllStores()
        {
            return _storeService.GetAllStores().Select(a => a.Name).ToList();
        }

        private List<int> GetStoreIds(QuickBooksSettings quickBooksSettings)
        {
            List<int> storeIds = new List<int>();

            if (!String.IsNullOrEmpty(quickBooksSettings.StoreName))
            {
                List<Store> nopCommerceStores = _storeService.GetAllStores().ToList();

                foreach (string s in quickBooksSettings.StoreName.Split(','))
                {
                    if (String.IsNullOrEmpty(s))
                        continue;

                    Store store = nopCommerceStores.Where(a => !String.IsNullOrEmpty(a.Name)
                    && a.Name.Trim().ToLower() == s.Trim().ToLower()).FirstOrDefault();

                    if (store != null)
                        storeIds.Add(store.Id);
                }
            }

            return storeIds;
        }

        public string MakeProduct(JMAProduct product)
        {
            Product nopCommerceProduct = _productService.GetProductBySku(product.Sku);

            if (nopCommerceProduct == null)
            {
                Product pr = new Product();
                pr.CreatedOnUtc = DateTime.UtcNow;
                pr.UpdatedOnUtc = DateTime.UtcNow;
                pr.AvailableEndDateTimeUtc = DateTime.UtcNow;
                pr.AvailableStartDateTimeUtc = DateTime.UtcNow;
                pr.Sku = product.Sku;
                pr.Price = product.Amount;
                pr.IsTaxExempt = product.IsTaxable;
                pr.Published = product.IsEnabled;
                pr.Name = product.Name;
                pr.FullDescription = product.Description;
                _productService.InsertProduct(pr);
                return "OK";
            }
            else
            {
                return "EXISTS";
            }
        }

        #region private variables

        IDiscountService _discountService;
        IOrderService _orderService;
        ISettingService _settingService;
        IEncryptionService _encryptionService;
        IStoreService _storeService;
        ICheckoutAttributeService _checkoutAttributeService;
        ICheckoutAttributeParser _checkoutAttributeParser;
        IPaymentService _paymentMethodService;
        IManufacturerService _manufacturerService;
        IDateTimeHelper _dateTimeHelper;
        IProductService _productService;
        IProductAttributeParser _productAttributeParser;
        #endregion
    }
}