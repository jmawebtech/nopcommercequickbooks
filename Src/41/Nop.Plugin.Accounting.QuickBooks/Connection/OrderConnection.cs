using ConnexForQuickBooks.Model;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Plugin.Accounting.QuickBooks.Interface;
using Nop.Plugin.Accounting.QuickBooks.Mapper;
using Nop.Plugin.Accounting.QuickBooks.Utility;
using Nop.Services.Orders;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nop.Plugin.Accounting.QuickBooks.Connection
{
    public class OrderConnection
    {
        IOrderService orderService;
        IStoreService storeService;
        IRepository<Order> orderRepository;
        QuickBooksSettings settings;

        public OrderConnection(QuickBooksSettings settings)
        {
            this.orderService = EngineContext.Current.Resolve<IOrderService>();
            this.storeService = EngineContext.Current.Resolve<IStoreService>();
            this.orderRepository = EngineContext.Current.Resolve<IRepository<Order>>();
            this.settings = settings;
        }

        public List<JMAOrder> Search()
        {
            IMapper<Order, JMAOrder> mapper = new NopCommerceOrderJMAOrderMapper();
            List<Order> ordersList = GetNopOrders();

            List<JMAOrder> jmaOrderList = new List<JMAOrder>();

            foreach (Order od in ordersList)
            {
                JMAOrder jmaOd = mapper.Map(od);
                jmaOrderList.Add(jmaOd);
            }

            return jmaOrderList;
        }

        /// <summary>
        /// Refunds are pulled from last 30 days. Filter refunds with sync range.
        /// </summary>
        /// <returns></returns>
        public List<JMAOrder> SearchRefunds()
        {
            List<JMAOrder> jmaOrderList = new List<JMAOrder>();

            List<Order> refunds = GetNopRefunds();

            foreach (Order od in refunds)
            {
                JMAOrder jmaOd = MapRefund(od);

                bool isOrderWithinSyncRange = jmaOd.CreationDate >= settings.LastDownloadUtc && jmaOd.CreationDate <= settings.LastDownloadUtcEnd;

                if (!isOrderWithinSyncRange)
                    continue;

                jmaOrderList.Add(jmaOd);
            }

            return jmaOrderList;
        }

        private JMAOrder MapRefund(Order od)
        {
            IMapper<Order, JMAOrder> mapper = new NopCommerceOrderJMAOrderMapper();
            JMAOrder order = mapper.Map(od);
            order.OrderStatus = "Refunded";
            order.RefundedAmount = od.RefundedAmount;

            Dictionary<string, object> values = CustomValueUtility.DeserializeCustomValues(od.CustomValuesXml);
            order.CreationDate = values.ContainsKey("refund_order_date") ? DateTime.Parse(values["refund_order_date"].ToString()) : order.CreationDate;
            order.TotalMerchantFees = values.ContainsKey("refund_payment_fee") ? decimal.Parse(values["refund_payment_fee"].ToString()) : 0;

            return order;
        }

        private List<Order> GetNopOrders()
        {
            IQueryable<Order> query = GetExpandedOrderTable();

            if (settings.LowestOrder > 0 && settings.HighestOrder > 0)
            {
                List<int> orderIDs = new List<int>();
                for (int i = settings.LowestOrder; i <= settings.HighestOrder; i++)
                {
                    orderIDs.Add(i);
                }

                return orderService.GetOrdersByIds(orderIDs.ToArray()).ToList();
            }
            else if (settings.StringOrders != null && settings.StringOrders.Count() > 0)
            {
                List<Order> orders = new List<Order>();

                foreach (string s in settings.StringOrders.Split(','))
                {
                    int id = int.Parse(s);
                    Order ord = orderService.GetOrderById(id);
                    if (ord != null)
                    {
                        orders.Add(ord);
                    }
                }

                return orders;
            }
            else
            {
                DateTime beginTime = settings.LastDownloadUtc;
                DateTime endTime = settings.LastDownloadUtcEnd;

                query = query.Where(a => a.CreatedOnUtc >= beginTime);
                query = query.Where(a => a.CreatedOnUtc <= endTime);

                List<int> storeIds = GetStoreIds();

                if (storeIds.Count() > 0)
                    query = query.Where(a => storeIds.Contains(a.StoreId));

                return new PagedList<Order>(query, 0, 1000).ToList();
            }

        }

        /// <summary>
        /// Expands order items and other sub properties.
        /// Increases performance.
        /// </summary>
        /// <returns></returns>
        private IQueryable<Order> GetExpandedOrderTable()
        {
            return orderRepository.Table
                .Include(a => a.OrderItems)
                .Include("OrderItems.Product")
                .Include(a => a.OrderNotes)
                .Include(a => a.GiftCardUsageHistory)
                .Include(a => a.BillingAddress).ThenInclude(a => a.StateProvince).ThenInclude(a => a.Country)
                .Include(a => a.ShippingAddress).ThenInclude(a => a.StateProvince).ThenInclude(a => a.Country)
                .Include(a => a.Customer)
                .Include(a => a.DiscountUsageHistory)
                .Include(a => a.Shipments);
        }

        private List<int> GetStoreIds()
        {
            List<int> storeIds = new List<int>();

            if (!String.IsNullOrEmpty(settings.StoreName))
            {
                List<Store> nopCommerceStores = storeService.GetAllStores().ToList();

                foreach (string s in settings.StoreName.Split(','))
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

        private List<Order> GetNopRefunds()
        {
            List<Order> refundedOrdersOnly = new List<Order>();
            List<int> paymentStatusIds = new List<int>();
            paymentStatusIds.Add((int)PaymentStatus.Refunded);

            IQueryable<Order> query = GetExpandedOrderTable();

            DateTime beginTime = settings.LastDownloadUtc.AddDays(-30);
            DateTime endTime = settings.LastDownloadUtcEnd;

            query = query.Where(a => a.CreatedOnUtc >= beginTime);
            query = query.Where(a => a.CreatedOnUtc <= endTime);
            query = query.Where(a => paymentStatusIds.Contains(a.PaymentStatusId));

            List<int> storeIds = GetStoreIds();

            if (storeIds.Count() > 0)
                query = query.Where(a => storeIds.Contains(a.StoreId));

            return new PagedList<Order>(query, 0, 1000).ToList();
        }
    }
}
