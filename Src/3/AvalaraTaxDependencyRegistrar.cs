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
using Nop.Plugin.Tax.AvalaraTax.Interfaces;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Tax.AvalaraTax
{
    public class AvalaraTaxDependencyRegistrar : IDependencyRegistrar 
    {
        public int Order
        {
            get { return 0; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, Core.Configuration.NopConfig config)
        {
            //I want to load AvalaraTaxProvider as a dependency. 
            builder.RegisterType<AvalaraTaxProvider>().As<IAvalaraTaxProvider>();
        }
    }
}
