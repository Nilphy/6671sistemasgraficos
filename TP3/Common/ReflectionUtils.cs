using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Common
{
    public static class ReflectionUtils
    {
        public static object InvokeMethod(object obj, string methodName, Object[] objectParams)
        {
            Type objectType = obj.GetType();
            MethodInfo getMethod = objectType.GetMethod(methodName);
            return getMethod.Invoke(obj, objectParams);
        }

        public static object GetProperty(object obj, string properyName)
        {
            Type objectType = obj.GetType();
            return objectType.GetProperty(properyName).GetValue(obj, null);
        }

        public static Type GetPropertyType(object obj, string properyName)
        {
            return GetPropertyValue(obj, properyName).GetType();

        }

        private static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            Type t = obj.GetType();

            // Need to perform reflection in isolation on the object and its supertype due to a problem with Castle Dynamic
            // Proxy and lazy loading
            foreach (PropertyInfo info in t.BaseType.GetProperties())
            {
                if (info.Name == propertyName)
                {
                    return info;
                }
            }

            return t.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }

        public static object GetPropertyValue(object element, string propertyList)
        {
            object value = element;
            object propertyValue = null;

            try
            {
                // Partir el nombre de la propiedad es sus componentes (Ejemplo: property1.property2.property3)
                char[] separator = { '.' };
                string[] properties = propertyList.Split(separator);

                foreach (string property in properties)
                {
                    // Obtener la propiedad del objeto                    
                    propertyValue = GetPropertyInfo(value, property).GetValue(value, null);

                    // Ahora el nuevo elemento es la propiedad
                    value = propertyValue;
                }
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException();
            }

            return value;
        }

    }
}
