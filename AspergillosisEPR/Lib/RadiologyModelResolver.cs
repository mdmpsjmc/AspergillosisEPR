using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public class RadiologyModelResolver
    {
        private string _klassName;
        private string _name;
        private object _objectInstance;

        public RadiologyModelResolver(string klassName, string name)
        {
            _klassName = klassName;
            _name = name;
        }

        public T ConvertTo<T>()
        {
            Resolve();
            return (T)Convert.ChangeType(_objectInstance, typeof(T));
        }

        public object Resolve()
        {
            _objectInstance = Activator.CreateInstance(GetModelTypeFromClassName());
            SetModelPropertyName();
            return _objectInstance;
        }

        private Type GetModelTypeFromClassName()
        {
            return Type.GetType("AspergillosisEPR.Models." + _klassName);
        }

        private object SetModelPropertyName()
        {
            Type modelType = _objectInstance.GetType();
            PropertyInfo property = modelType.GetProperty("Name");
            property.SetValue(_objectInstance, _name);
            return _objectInstance;
        }

    }
}
