using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            object i = -10.1;
            var format = new CultureInfo("uz").NumberFormat.CurrencyDecimalSeparator;
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
                Console.WriteLine(culture.NumberFormat.CurrencyDecimalSeparator + culture.Name);
            Console.WriteLine(string.Format(format,"{0}",i));
        }
    }
}
