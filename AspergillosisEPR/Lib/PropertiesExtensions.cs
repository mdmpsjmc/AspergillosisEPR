using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public static class PropertiesExtensions
    {
        public static object GetProperty<T>(this T obj, string name) where T : class
        {
            Type t = typeof(T);
            return t.GetProperty(name).GetValue(obj, null);
        }

        public static object GetProperty<T>(dynamic obj, string name) 
        {
            return obj.GetProperty(name).GetValue(obj, null);
        }
    }
}
