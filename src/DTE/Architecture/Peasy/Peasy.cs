using DTE.CORE;
using DTE.Domains;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE.Architecture
{
    public class Peasy : DataBindingBase45
    {
        private readonly string _businessProjectPath;
        private readonly string _dalProjectPath;
        private static Project _businessProject;
        private static Project _dalProject;

        public List<Table> Tables { get; }
        public DTECore DTECore { get; }

        public Peasy()
        {

        }
        public Peasy(string businessProjectPath,string dalProjectPath,List<Table> tables,DTECore dTECore)
        {
            _businessProjectPath = dalProjectPath;
            Tables = tables;
            DTECore = dTECore;
            _dalProjectPath = businessProjectPath;
            _businessProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == businessProjectPath);
            _dalProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == dalProjectPath);
            if (_businessProject is null)
                _businessProject = new Project(businessProjectPath);
            if (_dalProject is null)
                _dalProject = new Project(dalProjectPath);
        }

        public async void GeneratePeasyArch()
        {
            var businessProjectName = Path.GetFileNameWithoutExtension(_businessProjectPath);
            var businessNameSpace = GetNameSpace(businessProjectName,_businessProjectPath);

            foreach (var table in Tables)
            {
                await GenerateBusinessLogic(table,businessProjectName,businessNameSpace);
            }
        }

        private string GetNameSpace(string ProjectName,string projectPath)
        {
            return ProjectName + _businessProjectPath?.Split(new string[] { ProjectName }, StringSplitOptions.None).Last().Replace("/", ".").Replace("\\", ".");
        }


        private async Task GenerateBusinessLogic(Table table, string projectName, string nameSpace)
        {
            var model_code = await DTECore.CreateModelAsync(table.DataBaseName, table.TableName);
            var businessDomainfilePath = $"{_businessProject}/Domain/{table.TableName}.cs";
            var businessDataProxyfilePath = $"{_businessProject}/DataProxy/{table.TableName}DataProxy.cs";
            var businessServicefilePath = $"{_businessProject}/Service/{table.TableName}Service.cs";
           var model_domain = PeasyConstants.GetDomainTemplate(nameSpace, model_code);
            var dataProxy = PeasyConstants.GetDataProxyTemplate(nameSpace, projectName, table.TableName);
            var service = PeasyConstants.GetServiceTemplate(nameSpace, table.TableName);

            SaveFile(businessDomainfilePath, model_domain, _businessProject);
            SaveFile(businessDataProxyfilePath, dataProxy, _businessProject);
            SaveFile(businessServicefilePath, service, _businessProject);
        }

        private void SaveFile(string filePath,string fileText,Project project)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            System.IO.File.WriteAllText(file.FullName, fileText);
            if (project != null)
                AddToProject(filePath, project);
        }

        private void AddToProject(string filePath, Project _businessProject)
        {
            var projectName = Path.GetFileNameWithoutExtension(_businessProjectPath);
            var filePathToProject = filePath?.Split(new string[] { projectName }, StringSplitOptions.None).Last() + "\\" + Path.GetFileName(filePath);
            if (filePathToProject.StartsWith("\\"))
                filePathToProject = filePathToProject.Remove(0, 1);

            if (_businessProject.Items.FirstOrDefault(x => x.EvaluatedInclude == filePathToProject) is null)
            {
                _businessProject.AddItem("Compile", filePathToProject);
                _businessProject.Save();
            }
        }
    }
}
