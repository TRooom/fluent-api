using System;
using System.Collections.Generic;
using System.Globalization;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
	{
		private readonly PrintingConfig<TOwner> printingConfig;

		public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
		{
			this.printingConfig = printingConfig;
		}

		public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
		{
		    Func<object, string> f = x => print((TPropType)x);
            printingConfig.AddSpecificPropertyPrinting(f);
            
			return printingConfig;
		}

		public PrintingConfig<TOwner> Using(CultureInfo culture)
		{
		    var type = typeof(TPropType);

			return printingConfig;
		}

		PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => printingConfig;
	}

	public interface IPropertyPrintingConfig<TOwner, TPropType>
	{
		PrintingConfig<TOwner> ParentConfig { get; }
	}
}