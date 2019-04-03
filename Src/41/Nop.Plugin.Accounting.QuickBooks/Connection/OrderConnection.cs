using ConnexForQuickBooks.Model;
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
using System.Linq;

namespace Nop.Plugin.Accounting.QuickBooks.Connection
{
    public class OrderConnection
    {
        IOrderService orderService;
        IStoreService storeService;
        QuickBooksSettings settings;

        public OrderConnection(QuickBooksSettings settings)
        {
            this.orderService = EngineContext.Current.Resolve<IOrderService>();
            this.storeService = EngineContext.Current.Resolve<IStoreService>();
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

            List<Order> refunds = GetNopRefunds();

            foreach (Order od in refunds)
            {
                JMAOrder jmaOd = MapRefund(od);
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
            List<int> completeStatusIds = new List<int>();
            List<Order> orders = new List<Order>();
            List<int> paymentStatusIds = new List<int>();
            List<Order> refundedOrdersOnly = new List<Order>();

            completeStatusIds.Add((int)OrderStatus.Complete);
            paymentStatusIds.Add((int)PaymentStatus.Refunded);

            List<int> storeIds = GetStoreIds();

            if (settings.LowestOrder > 0 && settings.HighestOrder > 0)
            {
                List<int> orderIDs = new List<int>();
                for (int i = settings.LowestOrder; i <= settings.HighestOrder; i++)
                {
                    orderIDs.Add(i);
                }

                orders.AddRange(orderService.GetOrdersByIds(orderIDs.ToArray()));
            }
            else if (settings.StringOrders != null && settings.StringOrders.Count() > 0)
            {
                orders = new List<Order>();
                foreach (string s in settings.StringOrders.Split(','))
                {
                    int id = int.Parse(s);
                    Order ord = orderService.GetOrderById(id);
                    if (ord != null)
                    {
                        orders.Add(ord);
                    }
                }
            }
            else
            {
                DateTime beginTime = settings.LastDownloadUtc;
                DateTime endTime = settings.LastDownloadUtcEnd;

                if (storeIds.Count() == 0)
                {
                    orders = orderService.SearchOrders(0, 0, 0, 0, 0, 0, 0, null, beginTime, endTime).ToList();
                }
                else
                {
                    foreach (int storeId in storeIds)
                    {
                        orders.AddRange(orderService.SearchOrders(storeId, 0, 0, 0, 0, 0, 0, null, beginTime, endTime));
                    }
                }
            }

            return orders;
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
            List<int> storeIds = GetStoreIds();
            List<Order> refundedOrdersOnly = new List<Order>();
            DateTime beginTime = settings.LastDownloadUtc;
            DateTime endTime = settings.LastDownloadUtcEnd;
            List<int> paymentStatusIds = new List<int>();
            paymentStatusIds.Add((int)PaymentStatus.Refunded);

            if (storeIds.Count() == 0)
            {
                refundedOrdersOnly = orderService.SearchOrders(0, 0, 0, 0, 0, 0, 0, null, beginTime.AddDays(-30), endTime, null, paymentStatusIds).ToList();
            }
            else
            {
                foreach (int storeId in storeIds)
                {
                    refundedOrdersOnly.AddRange(orderService.SearchOrders(storeId, 0, 0, 0, 0, 0, 0, null, beginTime.AddDays(-30), endTime, null, paymentStatusIds));
                }
            }

            return refundedOrdersOnly;
        }
    }
}
