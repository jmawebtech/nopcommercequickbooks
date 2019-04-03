using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using System;
using System.Text;

namespace Nop.Plugin.Accounting.QuickBooks.Helper
{
    public class AuthenticateHelper
    {
        ICustomerService _customerService;
        ICustomerRegistrationService _customerRegistrationService;
        CustomerSettings _customerSettings;
        Controller _controller;

        public AuthenticateHelper(Controller controller)
        {
            _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
            _customerService = EngineContext.Current.Resolve<ICustomerService>();
            _customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            _controller = controller;
        }

        public bool IsAuthorized(string username, string password)
        {
            Customer cust = new Customer();
            CustomerRole cr = new CustomerRole();

            if (!_customerSettings.UsernamesEnabled)
            {
                cust = _customerService.GetCustomerByEmail(username);
            }
            else
            {
                cust = _customerService.GetCustomerByUsername(username);
            }

            if (cust != null)
            {
                foreach (CustomerRole cuRole in cust.CustomerRoles)
                {
                    if (cuRole.Name.ToLower() == "administrators")
                    {
                        CustomerLoginResults results = _customerRegistrationService.ValidateCustomer(username, password);
                        if (results == CustomerLoginResults.Successful)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool AuthenticateToWebservice()
        {
            string authHeader = _controller.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
                // Decode from Base64 to string
                var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
                // Split username and password
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];
                // Check if login is correct
                if (IsAuthorized(username, password))
                {
                    return true;
                }
            }
            // Return authentication type (causes browser to show login dialog)
            _controller.HttpContext.Response.Headers["WWW-Authenticate"] = "Basic";

            return false;
        }

    }
}
