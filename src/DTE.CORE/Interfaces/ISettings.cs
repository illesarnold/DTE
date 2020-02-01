using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DTE.CORE.Interfaces
{
    public interface ISettings
    {
        string AccentName { get; set; }
        IAttributes Attributes { get; }
        List<ITypeConverter> Types { get; }
        bool Nullable { get; set; }
        bool CaseSensitivity { get; set; }
        string Prefix { get; set; }
        string FilePrefix { get; set; }
        string Postfix { get; set; }
        string FilePostfix { get; set; }
        bool Comments { get; set; }
        bool DataAnnotations { get; set; }
        bool FullProp { get; set; }
        bool DataMember { get; set; }
        string FullPropTemplate { get; set; }
        string PropTemplate { get; set; }
        string ClassTemplate { get; set; }
    }
}