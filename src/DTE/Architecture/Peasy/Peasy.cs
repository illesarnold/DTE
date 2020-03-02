using DTE.CORE;
using DTE.CORE.Helpers;
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
        private readonly string _projectName;
        private readonly string _dataLayerProjectPath;
        private static Project _businessProject;
        private static Project _dalProject;
        private string _businessProjectName;
        private string _dataLayerProjectName;

        public IEnumerable<Table> Tables { get; }
        public DTECore DTECore { get; }

        public Peasy()
        {

        }
        public Peasy(string projectName,string businessProjectPath,string dalProjectPath,IEnumerable<Table> tables,DTECore dTECore)
        {
            _businessProjectPath = Path.GetDirectoryName(businessProjectPath);
            _dataLayerProjectPath = Path.GetDirectoryName(dalProjectPath);
            Tables = tables;
            DTECore = dTECore;
            _projectName = projectName;
            _businessProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == businessProjectPath);
            _dalProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == dalProjectPath);
            if (_businessProject is null)
                _businessProject = new Project(businessProjectPath);
            if (_dalProject is null)
                _dalProject = new Project(dalProjectPath);

            _businessProjectName = Path.GetFileNameWithoutExtension(_businessProjectPath);
            _dataLayerProjectName = Path.GetFileNameWithoutExtension(dalProjectPath);
        }

        public async Task GeneratePeasyArchAsync()
        {
            var businessProjectName = Path.GetFileNameWithoutExtension(_businessProjectPath);
            var dataLayerProjectName = Path.GetFileNameWithoutExtension(_dataLayerProjectPath);
            var businessNameSpace = GetNameSpace(businessProjectName,_businessProjectPath);
            var dataLayerNameSpace = GetNameSpace(dataLayerProjectName, _dataLayerProjectPath);

            SaveFileThenAddToProject(_businessProjectPath + "/Domain/DomainBase.cs", PeasyConstants.GetDomainBaseTemplate(businessNameSpace), _businessProject);
            SaveFileThenAddToProject(_businessProjectPath + $"/DataProxy/I{_projectName}DataProxy.cs", PeasyConstants.GetDataProxyBaseTemplate(businessNameSpace, _projectName), _businessProject);
            SaveFileThenAddToProject(_businessProjectPath + $"/Service/{_projectName}SeviceBase.cs", PeasyConstants.GetServiceBaseTemplate(businessNameSpace, _projectName), _businessProject);
            SaveFileThenAddToProject(_dataLayerProjectPath+"/Repository/RepositoryBase.cs", PeasyConstants.GetRepositoryBaseTemplate(dataLayerNameSpace, businessNameSpace, _projectName), _dalProject);

            foreach (var table in Tables)
            {
                await GenerateBusinessLogic(table, _projectName, businessNameSpace);
                GenerateDataLayer(table, dataLayerNameSpace, businessNameSpace);
            }
        }

        private void GenerateDataLayer(Table table, string dalNameSpace, string businessNameSpace)
        {
            var class_name = GetClassName(table);

            var dalRepository = PeasyConstants.GetRepositoryTemplate(dalNameSpace, businessNameSpace, class_name);
            var dalRepositoryfilePath = $"{_dataLayerProjectPath}/Repository/{class_name}Repository.cs";

            SaveFileThenAddToProject(dalRepositoryfilePath, dalRepository, _dalProject);
        }

        private static string GetClassName(Table table)
        {
            return ModelCreateHelper.ColumnNameToPropName(table.TableName.Split('.').Last()).Replace("_", "");
        }

        private string GetNameSpace(string ProjectName,string projectPath)
        {
            return ProjectName + projectPath?.Split(new string[] { ProjectName }, StringSplitOptions.None).Last().Replace("/", ".").Replace("\\", ".");
        }


        private async Task GenerateBusinessLogic(Table table, string projectName, string nameSpace)
        {
            var class_name = GetClassName(table);
            DTECore.Settings.Postfix += " : DomainBase";
            var model_code = await DTECore.CreateModelAsync(table.DataBaseName, table.TableName);
            var businessDomainfilePath = $"{_businessProjectPath}/Domain/{class_name}.cs";
            var businessDataProxyfilePath = $"{_businessProjectPath}/DataProxy/I{class_name}DataProxy.cs";
            var businessServicefilePath = $"{_businessProjectPath}/Service/{class_name}Service.cs";
            var model_domain = PeasyConstants.GetDomainTemplate(nameSpace, model_code);
            var dataProxy = PeasyConstants.GetDataProxyTemplate(nameSpace, projectName, class_name);
            var service = PeasyConstants.GetServiceTemplate(nameSpace, class_name);

            SaveFileThenAddToProject(businessDomainfilePath, model_domain, _businessProject);
            SaveFileThenAddToProject(businessDataProxyfilePath, dataProxy, _businessProject);
            SaveFileThenAddToProject(businessServicefilePath, service, _businessProject);
        }

        private void SaveFileThenAddToProject(string filePath,string fileText,Project project)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            if (File.Exists(file.FullName))
                return;
            System.IO.File.WriteAllText(file.FullName, fileText);
            if (project != null)
                AddToProject(filePath, project);
        }

        private void AddToProject(string filePath, Project project)
        {
            filePath = filePath.Replace("/", "\\");
            var projectName = Path.GetFileNameWithoutExtension(project.FullPath);
            var filePathToProject = filePath?.Split(new string[] { projectName }, StringSplitOptions.None).Last();
            if (filePathToProject.StartsWith("\\"))
                filePathToProject = filePathToProject.Remove(0, 1);

            if (project.Items.FirstOrDefault(x => x.EvaluatedInclude == filePathToProject) is null)
            {
                project.AddItem("Compile", filePathToProject);
                project.Save();
            }
        }
    }
}
