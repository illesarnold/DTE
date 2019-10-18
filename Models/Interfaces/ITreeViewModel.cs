using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Models.Interfaces
{

    public interface ITreeViewModel
    {
        TreeViewModel ParentTreeBase { get; set; }
    }
}
