using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Accounting.QuickBooks.Interface
{
    public interface IMapper<TSource, TDestination>
    {
        TDestination Map(TSource source);
        IList<TDestination> Map(IList<TSource> source);
    }
}
