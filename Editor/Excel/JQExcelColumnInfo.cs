using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace JQEditor.Excel
{
    public class JQExcelColumnInfo
    {
        private string _columnName;
        public int ColumnIndex;
        public PropertyInfo property;
        
        public Type PropertyType { get; protected set; }
        
        public bool IsNullable { get; protected set; }
        
        public string Name { get; protected set; }
        
        public JQExcelColumnInfo(string columnName, PropertyInfo propertyTypeInfo)
        {
            _columnName = columnName;
            property = propertyTypeInfo;
            
            SetPropertyType(propertyTypeInfo.PropertyType);
        }
        
        public PropertyInfo Property
        {
            get { return property; }
            set
            {
                property = value;
                if (property != null)
                {
                    SetPropertyType(property.PropertyType);
                    Name = property.Name;
                }
                else
                {
                    PropertyType = null;
                    IsNullable = false;
                }
            }
        }
        
        internal void SetPropertyType(Type propertyType)
        {
            if (propertyType.IsValueType)
            {
                var underlyingType = Nullable.GetUnderlyingType(propertyType);
                IsNullable = underlyingType != null;
                PropertyType = underlyingType ?? propertyType;
            }
            else
            {
                IsNullable = true;
                PropertyType = propertyType;
            }

        }
        
        public void SetProperty(object o, object val)
        {
            var v = GetPropertyValue(o, val);
            Property.SetValue(o, v, null);
        }
        
        public virtual object GetPropertyValue(object o, object val)
        {
            object v;
            if (val == null)
                v = null;
            else if (val is string s && s.Length == 0)
                v = string.Empty;
            else if (val is string g && PropertyType == typeof(Guid))
                v = Guid.Parse(g);
            else if (val is string es && PropertyType.IsEnum)
                v = ParseEnum(PropertyType, es);
            else if (val is string && PropertyType == typeof(byte[]))
                v = System.Text.Encoding.UTF8.GetBytes(val as string);
            else if (val is string && PropertyType == typeof(string[]))
                v = val.ToString().Split(';');
            else if (val is DateTime d && PropertyType == typeof(DateTimeOffset))
                v = new DateTimeOffset(d);
            else
                v = Convert.ChangeType(val, PropertyType, CultureInfo.InvariantCulture);

            return v;
        }
        
        private object ParseEnum(Type t, string s)
        {
            var name = Enum.GetNames(t).FirstOrDefault(n => n.Equals(s, StringComparison.OrdinalIgnoreCase));
            return name == null 
                ? throw new Exception("Did not find a matching enum name.") 
                : Enum.Parse(t, name);
        }

        public object GetProperty(object o)
        {
            return Property.GetValue(o, null);
        }
        
        
    }
}