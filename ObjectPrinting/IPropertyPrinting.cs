using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public interface IPropertyPrinting<TOwner, TPropType>
    {
        IPrintingConfig<TOwner> Using(Func<TPropType, string> printing);
    }
}