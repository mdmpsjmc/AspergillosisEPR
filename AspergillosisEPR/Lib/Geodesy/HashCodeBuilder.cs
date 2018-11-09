using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspergillosisEPR.Lib.Geodesy
{
    internal static class HashCodeBuilder
    {
        internal const int Seed = 17;
        internal static int HashWith<T>(this int hashCode, T other) => unchecked(hashCode * 31 + other?.GetHashCode() ?? 0);
    }
}
