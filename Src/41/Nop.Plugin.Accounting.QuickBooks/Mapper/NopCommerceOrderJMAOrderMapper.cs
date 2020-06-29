using ConnexForQuickBooks.Model;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Plugin.Accounting.QuickBooks.Interface;
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

namespace Nop.Plugin.Accounting.QuickBooks.Mapper
{
    public class NopCommerceOrderJMAOrderMapper : IMapper<Order, JMAOrder>
    {
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
        Dictionary<string, string> _paymentMethods = new Dictionary<string, string>();
        private readonly IPaymentPluginManager _paymentPluginManager;

        public NopCommerceOrderJMAOrderMapper()
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
            _paymentPluginManager = EngineContext.Current.Resolve<IPaymentPluginManager>();
        }

        public JMAOrder Map(Order od)
        {
            JMAOrder jmaOd = new JMAOrder();

            MapReferenceNumber(od, jmaOd);

            jmaOd.CurrencyCode = od.CustomerCurrencyCode;
            jmaOd.Id = od.Id;
            jmaOd.OrderNumber = od.Id.ToString();
            jmaOd.OrderStatus = od.OrderStatus.ToString();

            MapNotes(od, jmaOd);

            MapDates(od, jmaOd);

            MapShipmentInfo(od, jmaOd);

            MapTotals(od, jmaOd);

            MapDiscounts(od, jmaOd);

            MapRewardPoints(od, jmaOd);

            MapGiftCards(od, jmaOd);

            MapAddresses(od, jmaOd);

            MapPaymentMethod(od, jmaOd);

            MapMerchantFees(od, jmaOd);

            MapOrderItems(od, jmaOd);

            CreateRefundLine(od, jmaOd);

            MapCheckoutAttributes(od, jmaOd);

            MapCheckoutAttributesNotes(od, jmaOd);

            MapCustomFields(od, jmaOd);

            return jmaOd;
        }

        /// <summary>
        /// Maps custom values XML on nopCommerce order into Connex custom field object.
        /// To map custom fields with Connex for QuickBooks Desktop, read this guide: https://help.syncwithconnex.com/hc/en-us/articles/360027546832-How-do-I-map-custom-fields-
        /// </summary>
        /// <param name="od"></param>
        /// <param name="jmaOd"></param>
        private void MapCustomFields(Order od, JMAOrder jmaOd)
        {
            Dictionary<string, object> values = CustomValueUtility.DeserializeCustomValues(od.CustomValuesXml);

            foreach(KeyValuePair<string, object> value in values)
            {
                JMACustomField field = new JMACustomField();
                field.AccountingName = value.Key;
                field.ECommerceName = value.Key;
                field.Value = value.Value.ToString();
                jmaOd.JMACustomFields.Add(field);
            }
        }

        private void MapNotes(Order od, JMAOrder jmaOd)
        {
            foreach (OrderNote note in od.OrderNotes)
            {
                if (note.DisplayToCustomer)
                    jmaOd.Note += note.Note;
                else
                    jmaOd.PrivateNote += note.Note;
            }
        }

        private void MapDates(Order od, JMAOrder jmaOd)
        {
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            jmaOd.CreationDate = TimeZoneInfo.ConvertTimeFromUtc(od.CreatedOnUtc, estZone);

            if (od.PaidDateUtc.HasValue)
                jmaOd.PaidDate = TimeZoneInfo.ConvertTimeFromUtc(od.PaidDateUtc.Value, estZone);
        }

        private void MapTotals(Order od, JMAOrder jmaOd)
        {
            jmaOd.TotalExclTax = od.OrderSubtotalInclTax;
            jmaOd.TotalInclTax = od.OrderSubtotalExclTax;
            jmaOd.Total = od.OrderTotal;
            jmaOd.TotalTax = od.OrderTax;
        }

        private void MapPaymentMethod(Order od, JMAOrder jmaOd)
        {
            jmaOd.CreditCardName = _encryptionService.DecryptText(od.CardType);

            if (!String.IsNullOrEmpty(jmaOd.CreditCardName))
                return;

            if(_paymentMethods.Where(a => a.Key == od.PaymentMethodSystemName).Count() > 0)
            {
                jmaOd.CreditCardName = _paymentMethods.Where(a => a.Key == od.PaymentMethodSystemName).ElementAt(0).Value;
            }
            else
            {
                IPaymentMethod cc = _paymentPluginManager.LoadPluginBySystemName(od.PaymentMethodSystemName);
                if (cc != null)
                {
                    jmaOd.CreditCardName = cc.PluginDescriptor.FriendlyName;
                    jmaOd.CardType = _encryptionService.DecryptText(od.CardType);
                    _paymentMethods.Add(od.PaymentMethodSystemName, jmaOd.CreditCardName);
                }
            }
        }

        private void MapCheckoutAttributes(Order od, JMAOrder jmaOd)
        {
            IList<CheckoutAttributeValue> cList = _checkoutAttributeParser.ParseCheckoutAttributeValues(od.CheckoutAttributesXml);
            foreach (var c in cList)
            {
                IList<string> theValue = _checkoutAttributeParser.ParseValues(od.CheckoutAttributesXml, c.Id);

                JMAOrderDetail odd = new JMAOrderDetail();
                odd.PriceExclTax = c.PriceAdjustment;
                odd.PriceInclTax = c.PriceAdjustment;
                odd.DiscountInclTax = 0;
                odd.DiscountExclTax = 0;
                odd.Name = c.CheckoutAttribute.Name;
                odd.Sku = c.CheckoutAttribute.Name.Replace(" ", "");

                JMAProduct pro = new JMAProduct();
                pro.Amount = c.PriceAdjustment;
                pro.Id = c.Id;
                pro.Name = c.Name;
                pro.Sku = c.Name;
                if (theValue.Count() > 0)
                {
                    pro.Description = theValue.FirstOrDefault();
                }

                pro.PurchaseCost = c.PriceAdjustment;
                pro.ManufacturerPartNumber = string.Empty;
                odd.Product = pro;


                jmaOd.OrderDetails.Add(odd);

            }
        }

        private void MapCheckoutAttributesNotes(Order od, JMAOrder jmaOd)
        {
            var dList = _checkoutAttributeParser.ParseCheckoutAttributes(od.CheckoutAttributesXml);
            foreach (var d in dList.Where(a => a.AttributeControlType == Nop.Core.Domain.Catalog.AttributeControlType.MultilineTextbox))
            {
                IList<string> theValue = _checkoutAttributeParser.ParseValues(od.CheckoutAttributesXml, d.Id);
                JMAOrderDetail odd = new JMAOrderDetail();
                odd.PriceExclTax = 0;
                odd.PriceInclTax = 0;
                odd.DiscountInclTax = 0;
                odd.DiscountExclTax = 0;
                odd.Name = d.Name;
                odd.Sku = d.Name.Replace(" ", "");

                JMAProduct pro = new JMAProduct();
                pro.Amount = 0;
                pro.Id = d.Id;
                pro.Name = d.Name;
                if (theValue.Count() > 0)
                {
                    pro.Description = theValue.FirstOrDefault();
                }

                pro.Sku = d.Name.Replace(" ", "");
                pro.PurchaseCost = 0;
                pro.ManufacturerPartNumber = string.Empty;
                odd.Product = pro;


                jmaOd.OrderDetails.Add(odd);
            }
        }

        private void MapReferenceNumber(Order od, JMAOrder jmaOd)
        {
            jmaOd.ReferenceNumber = od.AuthorizationTransactionId;

            if (!String.IsNullOrEmpty(od.AuthorizationTransactionId) && !String.IsNullOrEmpty(od.AuthorizationTransactionResult))
                jmaOd.PrivateNote = od.AuthorizationTransactionId + ":" + od.AuthorizationTransactionResult;
            else if (!String.IsNullOrEmpty(od.AuthorizationTransactionId))
                jmaOd.PrivateNote = od.AuthorizationTransactionId;
        }

        private void MapShipmentInfo(Order od, JMAOrder jmaOd)
        {
            if (od.Shipments != null && od.Shipments.Count() > 0)
            {
                Shipment shipment = od.Shipments.FirstOrDefault();
                jmaOd.TrackingNumber = shipment.TrackingNumber;

                if (shipment.ShippedDateUtc != null)
                    jmaOd.ShippingDate = shipment.ShippedDateUtc.Value;
            }

            jmaOd.ShippingMethod = od.ShippingMethod;
            jmaOd.ShippingExclTax = od.OrderShippingExclTax;
            jmaOd.ShippingInclTax = od.OrderShippingInclTax;
        }

        private void MapOrderItems(Order od, JMAOrder jmaOd)
        {
            foreach (OrderItem ov in od.OrderItems)
            {
                JMAOrderDetail odd = new JMAOrderDetail();

                odd.Id = ov.Id;
                odd.Name = ov.Product.Name;
                odd.OrderId = jmaOd.Id;
                odd.PriceExclTax = ov.UnitPriceExclTax;
                odd.PriceInclTax = ov.UnitPriceInclTax;

                odd.Quantity = ov.Quantity;

                if(!String.IsNullOrEmpty(ov.AttributesXml))
                    odd.Sku = _productService.FormatSku(ov.Product, ov.AttributesXml);
                else
                    odd.Sku = ov.Product.Sku;

                odd.DiscountExclTax = ov.DiscountAmountExclTax;
                odd.DiscountInclTax = ov.DiscountAmountInclTax;

                if (odd.PriceExclTax != odd.PriceInclTax)
                {
                    odd.HasTax = true;
                }

                JMAProduct pro = new JMAProduct();
                pro.Amount = ov.PriceExclTax;
                pro.Id = ov.Product.Id;
                pro.Name = ov.Product.Name;
                pro.Sku = odd.Sku;
                pro.PurchaseCost = ov.Product.ProductCost;
                pro.ManufacturerPartNumber = ov.Product.ManufacturerPartNumber;

                if (!String.IsNullOrEmpty(ov.AttributeDescription))
                    pro.Description = ov.AttributeDescription + "<br />" + ov.Product.Name;
                else
                    pro.Description = ov.Product.Name;

                odd.Product = pro;


                jmaOd.OrderDetails.Add(odd);
            }
        }

        public void CreateRefundLine(Order od, JMAOrder order)
        {
            if (od.RefundedAmount > 0 && od.OrderTotal != od.RefundedAmount)
            {
                string partialRefund = "PartialRefund";

                JMAOrderDetail detail = new JMAOrderDetail()
                {
                    PriceExclTax = od.RefundedAmount,
                    PriceInclTax = od.RefundedAmount,
                    Quantity = 1,
                    Sku = partialRefund,
                    Name = partialRefund,
                    Product = new JMAProduct()
                    {
                        IsService = true,
                        Amount = od.RefundedAmount,
                        Sku = partialRefund,
                        Name = partialRefund
                    }
                };

                order.OrderDetails.Clear();
                order.Discounts.Clear();
                order.DiscountExclTax = 0;
                order.DiscountInclTax = 0;
                order.ShippingExclTax = 0;
                order.ShippingInclTax = 0;
                order.ShippingMethod = null;

                order.OrderDetails.Add(detail);
            }
        }

        private void MapAddresses(Order od, JMAOrder jmaOd)
        {
            MapBillingAddress(od, jmaOd);

            MapShippingAddress(od, jmaOd);
        }

        private void MapShippingAddress(Order od, JMAOrder jmaOd)
        {
            if (od.ShippingAddress != null)
            {
                jmaOd.ShippingAddress.FirstName = od.ShippingAddress.FirstName;
                jmaOd.ShippingAddress.LastName = od.ShippingAddress.LastName;
                if (!String.IsNullOrEmpty(od.ShippingAddress.Company))
                {
                    jmaOd.ShippingAddress.Company = od.ShippingAddress.Company;
                }

                if (!String.IsNullOrEmpty(od.ShippingAddress.Email))
                {
                    jmaOd.ShippingAddress.Email = od.ShippingAddress.Email;
                }


                jmaOd.ShippingAddress.Address1 = od.ShippingAddress.Address1;
                jmaOd.ShippingAddress.Id = od.ShippingAddress.Id;
                jmaOd.ShippingAddress.Address2 = od.ShippingAddress.Address2;
                jmaOd.ShippingAddress.City = od.ShippingAddress.City;
                jmaOd.ShippingAddress.CountryName = od.ShippingAddress.Country.Name;
                jmaOd.ShippingAddress.PostalCode = od.ShippingAddress.ZipPostalCode;

                if (od.ShippingAddress.StateProvince != null)
                {
                    jmaOd.ShippingAddress.RegionName = od.ShippingAddress.StateProvince.Abbreviation;
                }
                jmaOd.ShippingAddress.TwoLetterIsoCode = od.ShippingAddress.Country.TwoLetterIsoCode;
                jmaOd.ShippingAddress.PhoneNumber = od.ShippingAddress.PhoneNumber;
                jmaOd.ShippingAddress.SubjectToVat = od.ShippingAddress.Country.SubjectToVat;

            }
            else
            {
                jmaOd.ShippingAddress = jmaOd.BillingAddress;
            }
        }

        private void MapBillingAddress(Order od, JMAOrder jmaOd)
        {
            jmaOd.BillingAddress.FirstName = od.BillingAddress.FirstName;
            jmaOd.BillingAddress.LastName = od.BillingAddress.LastName;
            if (!String.IsNullOrEmpty(od.BillingAddress.Company))
            {
                jmaOd.BillingAddress.Company = od.BillingAddress.Company;
            }

            if (!String.IsNullOrEmpty(od.BillingAddress.Email))
            {
                jmaOd.BillingAddress.Email = od.BillingAddress.Email;
            }

            jmaOd.BillingAddress.Address1 = od.BillingAddress.Address1;
            jmaOd.BillingAddress.Id = od.BillingAddress.Id;
            jmaOd.BillingAddress.Address2 = od.BillingAddress.Address2;
            jmaOd.BillingAddress.City = od.BillingAddress.City;
            jmaOd.BillingAddress.CountryName = od.BillingAddress.Country.Name;
            jmaOd.BillingAddress.PostalCode = od.BillingAddress.ZipPostalCode;
            if (od.BillingAddress.StateProvince != null)
            {
                jmaOd.BillingAddress.RegionName = od.BillingAddress.StateProvince.Abbreviation;
            }
            jmaOd.BillingAddress.TwoLetterIsoCode = od.BillingAddress.Country.TwoLetterIsoCode;
            jmaOd.BillingAddress.PhoneNumber = od.BillingAddress.PhoneNumber;
            jmaOd.BillingAddress.SubjectToVat = od.BillingAddress.Country.SubjectToVat;
        }

        private void MapGiftCards(Order od, JMAOrder jmaOd)
        {
            GiftCardUsageHistory giftCardUsageHistory = od.GiftCardUsageHistory.FirstOrDefault();
            if (giftCardUsageHistory != null)
            {
                JMAOrderDetail giftCardDetail = new JMAOrderDetail();
                giftCardDetail.Name = "GiftCard";
                giftCardDetail.OrderId = giftCardUsageHistory.UsedWithOrderId;
                giftCardDetail.PriceExclTax = giftCardUsageHistory.UsedValue * -1;
                giftCardDetail.PriceInclTax = giftCardDetail.PriceExclTax;
                giftCardDetail.Quantity = 1;
                giftCardDetail.Sku = "GiftCard";
                giftCardDetail.Product = new JMAProduct();
                giftCardDetail.Product.Amount = giftCardDetail.PriceExclTax;
                giftCardDetail.Product.Description = "GiftCard";
                giftCardDetail.Product.IsDownload = true;
                giftCardDetail.Product.Sku = giftCardDetail.Sku;
                giftCardDetail.Product.Name = giftCardDetail.Name;
                jmaOd.OrderDetails.Add(giftCardDetail);
            }
        }

        private void MapRewardPoints(Order od, JMAOrder jmaOd)
        {
            if (od.RedeemedRewardPointsEntry != null)
            {
                var dd = od.RedeemedRewardPointsEntry;
                JMADiscount dis = new JMADiscount();
                dis.Amount = dd.UsedAmount;
                dis.Name = dd.Message;
                dis.Code = "RewardPoints";
                dis.Id = dd.Id;
                dis.NonTaxable= true;
                jmaOd.Discounts.Add(dis);

            }
        }

        private void MapDiscounts(Order od, JMAOrder jmaOd)
        {
            jmaOd.DiscountExclTax = od.OrderDiscount + od.OrderSubTotalDiscountExclTax;
            jmaOd.DiscountInclTax = od.OrderDiscount + od.OrderSubTotalDiscountInclTax;

            foreach (DiscountUsageHistory discountUsageHistory in od.DiscountUsageHistory)
            {
                JMADiscount dis = new JMADiscount();

                //shipping discounts are already calculated as part of total shipping. nopCommerce never adds them
                if (discountUsageHistory.Discount.DiscountType == DiscountType.AssignedToShipping || discountUsageHistory.Discount.DiscountType == DiscountType.AssignedToCategories || discountUsageHistory.Discount.DiscountType == DiscountType.AssignedToSkus)
                {
                    continue;
                }
                else if (discountUsageHistory.Discount.DiscountType == DiscountType.AssignedToOrderSubTotal && discountUsageHistory.Discount.UsePercentage)
                {
                    dis.Amount = od.OrderSubTotalDiscountExclTax;
                }

                else if (discountUsageHistory.Discount.UsePercentage)
                {
                    dis.Amount = (discountUsageHistory.Discount.DiscountPercentage / 100) * (od.OrderSubtotalExclTax + od.OrderShippingExclTax);
                }
                else
                {
                    dis.Amount = discountUsageHistory.Discount.DiscountAmount;
                }

                dis.Name = discountUsageHistory.Discount.Name;
                dis.Code = discountUsageHistory.Discount.CouponCode;
                dis.Id = discountUsageHistory.DiscountId;
                jmaOd.Discounts.Add(dis);
            }
        }

        private void MapMerchantFees(Order od, JMAOrder jmaOd)
        {
            Dictionary<string, object> values = CustomValueUtility.DeserializeCustomValues(od.CustomValuesXml);

            if (values.ContainsKey("payment_fee"))
                jmaOd.TotalMerchantFees = decimal.Parse(values["payment_fee"].ToString());
        }

        public IList<JMAOrder> Map(IList<Order> source)
        {
            return source.Select(s => Map(s)).ToList();
        }
    }   
}
