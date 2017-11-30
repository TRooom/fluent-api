using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ObjectPrinting.Tests;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly List<PropertyInfo> excludedProperties;
        private readonly List<Type> excludedTypes;

        private readonly Dictionary<PropertyInfo, Func<object, string>> specialPropertiesPrinting;
        private readonly Dictionary<Type, Func<object, string>> specialTypesPrinting;

        private readonly Dictionary<Type, CultureInfo> cultureInfoForNumber;

        private readonly Dictionary<PropertyInfo, int> propertyMaxLength;

        public PrintingConfig()
        {
            excludedProperties = new List<PropertyInfo>();
            excludedTypes = new List<Type>();
            specialPropertiesPrinting = new Dictionary<PropertyInfo, Func<object, string>>();
            specialTypesPrinting = new Dictionary<Type, Func<object, string>>();
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var memberExpression = memberSelector.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Expression isn't a member access", nameof(memberSelector));
            var property = typeof(Person).GetProperty(memberExpression.Member.Name);
            excludedProperties.Add(property);
            return this;
        }

        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        internal void AddSpecificCultureForNumber(Type type, CultureInfo info)
        {
            cultureInfoForNumber.Add(type, info);
        }

        internal void AddSpecificPropertyPrinting(Func<object, string> printing, PropertyInfo property)
        {
            specialPropertiesPrinting.Add(property, printing);
        }

        internal void AddSpecificTypePrinting(Type type, Func<object, string> printing)
        {
            specialTypesPrinting.Add(type, printing);
        }

        internal void AddPropertyMaxLength(PropertyInfo property, int maxLength)
        {
            propertyMaxLength.Add(property, maxLength);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
            if (excludedTypes.Contains(obj))
                return string.Empty;


            if (obj == null)
                return "null" + Environment.NewLine;
            var type = obj.GetType();

            if (specialTypesPrinting.ContainsKey(type))
                return specialTypesPrinting[type](obj);

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (specialPropertiesPrinting.ContainsKey(propertyInfo))
                {
                    var value = propertyInfo.GetValue(obj);
                    var res = specialPropertiesPrinting[propertyInfo](value);
                    sb.Append(res);
                }
                else if (!excludedProperties.Contains(propertyInfo)
                         && !excludedTypes.Contains(propertyInfo.PropertyType))
                    sb.Append(identation + propertyInfo.Name + " = " +
                              PrintToString(propertyInfo.GetValue(obj),
                                  nestingLevel + 1));
            }
            return sb.ToString();
        }
    }
}