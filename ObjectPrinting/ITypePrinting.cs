using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public interface ITypePrinting<TOwner, TType>
    {
        IPrintingConfig<TOwner> Using(Func<TType, string> printing);
        IPrintingConfig<TOwner> Using(CultureInfo culture);
    }
}