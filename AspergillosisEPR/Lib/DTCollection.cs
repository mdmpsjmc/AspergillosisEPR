using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib
{
    public abstract class DTCollection
    {
        public abstract object ValueUntyped { get; }
    }

    public abstract class DTCollection<T> : DTCollection
    {
        public T Value { get; set; }
        public override object ValueUntyped { get { return Value; } }
    }
}


