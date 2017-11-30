using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public interface IPrintingConfig<TOwner>
    {
        IPrintingConfig<TOwner> Excluding<T>();
        IPrintingConfig<TOwner> Excluding<TPopType>(Expression<Func<TOwner, TPopType>> property);
        ITypePrinting<TOwner, TType> Printing<TType>();
        IPropertyPrinting<TOwner, TPopType> Printing<TPopType>(Expression<Func<TOwner, TPopType>> property);
        string PrintToString(TOwner obj);
    }
}