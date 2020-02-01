using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.CORE.Helpers
{
    public class ModelCreateHelper
    {
        public static string ColumnNameToPropName(string colname)
        {
            var parts = colname.Split('_');
            var propName = "";
            if (parts.Count() != 0)
            {
                int count = 0;
                foreach (var item in parts)
                {
                    if (item != string.Empty)
                    {
                        if (count == parts.Count() - 1)
                        {
                            propName += item.First().ToString().ToUpper() + item.Substring(1);
                        }
                        else
                        {
                            propName += item.First().ToString().ToUpper() + item.Substring(1) + "_";
                        }
                    }
                    count++;
                }
            }
            else
            {
                propName += colname.First().ToString().ToUpper() + colname.Substring(1);
            }

            return propName;

        }
    }
}
