using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public static class PrintingConfigExtensions
    {
        public static IPrintingConfig<TOwner> TrimingToLength<TOwner>(this IPropertyPrinting<TOwner, string> config,
            int maxLength)
        {
            var parentConfig = ((IParentConfig<TOwner>) config).GetParentConfig();
            var property = ((ISelectedProperty) config).GetProperty();
            parentConfig.SetPropertyMaxLength(property, maxLength);
            return ((IParentConfig<TOwner>) config).GetParentConfig();
        }
    }
}