using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Architecture
{
    public class Peasy : DataBindingBase45
    {
        private readonly string _businessProjectPath;
        private readonly object _businessProject;

        public Peasy()
        {

        }
        public Peasy(string businessProjectPath)
        {
            _businessProjectPath = businessProjectPath;
            _businessProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == _businessProjectPath);
            if (_businessProject is null)
                _businessProject = new Project(businessProjectPath);
        }
    }
}
