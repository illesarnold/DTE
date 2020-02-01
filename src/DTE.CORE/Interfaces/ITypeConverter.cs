using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DTE.CORE.Interfaces
{
    public interface ITypeConverter
    {
        string CType { get; set; }
        string PType { get; set; }
        
    }
}