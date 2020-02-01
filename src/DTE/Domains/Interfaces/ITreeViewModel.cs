using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Domains.Interfaces
{

    public interface ITreeViewModel
    {
        TreeViewModel ParentTreeBase { get; set; }
    }
}
