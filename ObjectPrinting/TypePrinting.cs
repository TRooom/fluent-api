using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class TypePrinting<TOwner, TType> : ITypePrinting<TOwner, TType>, IParentConfig<TOwner>
    {
        private readonly PrintingConfig<TOwner> config;

        public TypePrinting(PrintingConfig<TOwner> config)
        {
            this.config = config;
        }

        public IPrintingConfig<TOwner> Using(Func<TType, string> printing)
        {
            Func<object, string> print = x => printing((TType) x);
            config.AddTypePrinting<TType>(print);
            return config;
        }

        public IPrintingConfig<TOwner> Using(CultureInfo culture)
        {
            config.AddCultureInfo<TType>(culture);
            return config;
        }

        public PrintingConfig<TOwner> GetParentConfig() => config;
    }
}