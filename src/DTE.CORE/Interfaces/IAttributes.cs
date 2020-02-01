using System.Runtime.Serialization;

namespace DTE.CORE.Interfaces
{
    public interface IAttributes
    {
        string Key { get; set; } 
        string ExplicitKey { get; set; } 
        string Table { get; set; } 
        string Write { get; set; } 
        string Computed { get; set; }
    }
}