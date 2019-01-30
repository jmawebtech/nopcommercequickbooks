using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Data;
using Autofac;
using System.Data.Entity;
using Nop.Core;
using Nop.Data;

namespace Nop.Plugin.Accounting.QuickBooks
{
    public class DependencyRegistrar : IDependencyRegistrar 
    {

        public int Order
        {
            get { return 0; }
        }

        public void Register(Autofac.ContainerBuilder builder, Core.Infrastructure.ITypeFinder typeFinder)
        {
            //I want to load AvalaraTaxProvider as a dependency. 
            builder.RegisterType<NopQBProcess>().As<INopQBProcess>();
        }
    }
}
