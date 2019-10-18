using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Models
{
    [Serializable]
    public class TypeConverter : DataBindingBase45
    {
        public TypeConverter()
        {

        }
        public TypeConverter(string cType, string pType)
        {
            _cType = cType;
            _pType = pType;
        }

        string _cType;

        public string CType
        {
            get
            {
                return _cType;
            }

            set
            {
                _cType = value;
                OnPropertyChanged();
            }
        }
        string _pType;

        public string PType
        {
            get
            {
                return _pType;
            }

            set
            {
                _pType = value;
                OnPropertyChanged();
            }
        }

        public List<TypeConverter> GetDefaultTypes()
        {
            return new List<TypeConverter>()
            {
                new TypeConverter("System.Byte","byte"),
                new TypeConverter("System.SByte","sbyte"),
                new TypeConverter("System.Int32","int"),
                new TypeConverter("System.UInt32","uint"),
                new TypeConverter("System.Int16","short"),
                new TypeConverter("System.UInt16","ushort"),
                new TypeConverter("System.Int64","long"),
                new TypeConverter("System.UInt64","ulong"),
                new TypeConverter("System.Single","float"),
                new TypeConverter("System.Double","double"),
                new TypeConverter("System.Char","char"),
                new TypeConverter("System.Boolean","bool"),
                new TypeConverter("System.Object","object"),
                new TypeConverter("System.String","string"),
                new TypeConverter("System.Decimal","decimal"),
                new TypeConverter("System.DateTime","DateTime")
            };
        }
    }
}
