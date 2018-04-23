using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AspergillosisEPR.Extensions
{
    public static class ExtensionMethods
    {
        public static string ToUnderscore(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string[] SplitCamelCaseArray(this string source)
        {
            return Regex.Split(source, @"(?<!^)(?=[A-Z])");
        }

        public static string FirstCharacterToLower(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }


        public static string FirstCharacterToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Char.ToUpperInvariant(str[0]) + str.Substring(1).ToLower();
        }


        public static string GetValueFromProperty(object obj, string Name)
        {
            var methods = Name.Split('.');

            object current = obj;
            object result = null;
            foreach (var method in methods)
            {
                var prop = current.GetType().GetProperty(method);
                result = prop != null ? prop.GetValue(current, null) : null;
                current = result;
            }
            return result == null ? string.Empty : result.ToString();
        }
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}
