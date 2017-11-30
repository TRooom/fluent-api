using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        #region Parameters

        private readonly Dictionary<Type, Func<object, string>> specialTypesPrinting;
        private readonly Dictionary<PropertyInfo, Func<object, string>> specialPropertiesPrinting;

        private readonly List<Type> excludedTypes;
        private readonly List<PropertyInfo> excludedProperties;

        private readonly Dictionary<Type, CultureInfo> specialCulture;

        private readonly Dictionary<PropertyInfo, int> propertiesMaxLen;

        #endregion

        public PrintingConfig()
        {
            specialTypesPrinting = new Dictionary<Type, Func<object, string>>();
            specialPropertiesPrinting = new Dictionary<PropertyInfo, Func<object, string>>();
            excludedTypes = new List<Type>();
            excludedProperties = new List<PropertyInfo>();
            specialCulture = new Dictionary<Type, CultureInfo>();
            propertiesMaxLen = new Dictionary<PropertyInfo, int>();
        }

        #region AddParameters

        internal void ExcludeType(Type type) => excludedTypes.Add(type);

        internal void ExcludeProperty(PropertyInfo property) => excludedProperties.Add(property);

        internal void AddCultureInfo<T>(CultureInfo culture) => specialCulture.Add(typeof(T), culture);

        internal void AddTypePrinting<T>(Func<object, string> printing) =>
            specialTypesPrinting.Add(typeof(T), printing);

        internal void AddPropertyPrinting(Func<object, string> printing, PropertyInfo property) =>
            specialPropertiesPrinting.Add(property, printing);

        internal void SetPropertyMaxLength(PropertyInfo property, int maxLength) =>
            propertiesMaxLen.Add(property, maxLength);

        #endregion

        public IPrintingConfig<TOwner> Excluding<T>()
        {
            ExcludeType(typeof(T));
            return this;
        }

        public IPrintingConfig<TOwner> Excluding<TPopType>(Expression<Func<TOwner, TPopType>> propertySelector)
        {
            var property = GetProperty(propertySelector);
            ExcludeProperty(property);
            return this;
        }

        public ITypePrinting<TOwner, TType> Printing<TType>()
        {
            return new TypePrinting<TOwner, TType>(this);
        }

        public IPropertyPrinting<TOwner, TPopType> Printing<TPopType>(
            Expression<Func<TOwner, TPopType>> propertySelector)
        {
            var property = GetProperty(propertySelector);
            return new PropertyPrinting<TOwner, TPopType>(this, property);
        }

        private PropertyInfo GetProperty<TPropType>(Expression<Func<TOwner, TPropType>> propertySelector)
        {
            var member = propertySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException("Expression isn't a member access", nameof(propertySelector));
            var property = member.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("Expression isnt't a property access", nameof(propertySelector));
            return property;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
            if (obj == null)
                return "null" + Environment.NewLine;
            var type = obj.GetType();
            string printed;
            if (TryPrintFinalType(obj, out printed))
                return printed + Environment.NewLine;

            var sb = new StringBuilder();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
                sb.Append(PrintProperty(propertyInfo, obj, nestingLevel));
            return sb.ToString();
        }

        private string GetIdentation(int nestingLevel) => new string('\t', nestingLevel + 1);

        private string PrintProperty(PropertyInfo property, object parent, int nestingLevel)
        {
            if (IsExcluded(property))
                return string.Empty;

            var identation = GetIdentation(nestingLevel);
            var value = GetPropertyValue(property, parent, identation);
            var sb = new StringBuilder();
            Func<object, string> printing;
            if (specialPropertiesPrinting.TryGetValue(property, out printing))
                sb.Append(identation + printing(property.GetValue(parent)) + Environment.NewLine);
            else
                sb.Append(identation + property.Name + " = " +
                          PrintToString(value,
                              nestingLevel + 1));
            return sb.ToString();
        }

        private object GetPropertyValue(PropertyInfo property, object parent, string identation)
        {
            Func<object, string> printing;
            object value = null;
            var type = property.PropertyType;
            if (specialTypesPrinting.TryGetValue(type, out printing))
                value = printing(property.GetValue(parent));

            value = value ?? property.GetValue(parent);
            if (specialCulture.ContainsKey(type))
                value = (object) string.Format(specialCulture[type].NumberFormat, "{0}", value);
            if (propertiesMaxLen.ContainsKey(property))
                value = value.ToString().Substring(0, Math.Min(value.ToString().Length, propertiesMaxLen[property]));
            return value;
        }

        private bool IsExcluded(PropertyInfo property)
        {
            return excludedTypes.Contains(property.PropertyType) || excludedProperties.Contains(property);
        }

        private bool TryPrintFinalType(object obj, out string result)
        {
            result = string.Empty;
            var type = obj.GetType();
            Func<object, string> printing;
            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };

            if (excludedTypes.Contains(type))
                result = string.Empty;
            else if (specialTypesPrinting.TryGetValue(type, out printing))
                result = printing(obj);
            else if (finalTypes.Contains(obj.GetType()))
                result = obj.ToString();
            else
                return false;
            return true;
        }
    }
}