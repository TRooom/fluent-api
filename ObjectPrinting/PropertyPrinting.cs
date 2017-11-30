using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class PropertyPrinting<TOwner, TPropType> : IPropertyPrinting<TOwner, TPropType>, IParentConfig<TOwner>,
        ISelectedProperty
    {
        private readonly PrintingConfig<TOwner> config;

        private readonly PropertyInfo property;

        public PropertyPrinting(PrintingConfig<TOwner> config, PropertyInfo property)
        {
            this.config = config;
            this.property = property;
        }

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> printing)
        {
            Func<object, string> print = x => printing((TPropType) x);
            config.AddPropertyPrinting(print, property);
            return config;
        }

        public PrintingConfig<TOwner> GetParentConfig() => config;
        public PropertyInfo GetProperty() => property;
    }
}