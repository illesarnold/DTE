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
        private readonly string _solutionName;
        private readonly string _businessProjectPath;
        private readonly string _dataLayerProjectPath;
        private static Project _businessProject;
        private static Project _dataLayerProject;
        private string _businessProjectName;
        private string _dataLayerProjectName;
        private PeasyHelper _peasyHelper;
        
        public IEnumerable<Table> Tables { get; }
        public DTECore DTECore { get; }

        public Peasy()
        {

        }
        public Peasy(string solutionName,string businessProjectPath,string dataLayerProjectPath,IEnumerable<Table> tables,DTECore dTECore)
        {
            _businessProjectPath = Path.GetDirectoryName(businessProjectPath);
            _dataLayerProjectPath = Path.GetDirectoryName(dataLayerProjectPath);
            Tables = tables;
            DTECore = dTECore;
            _solutionName = solutionName;
            _businessProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == businessProjectPath);
            _dataLayerProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == dataLayerProjectPath);
            if (_businessProject is null)
                _businessProject = new Project(businessProjectPath);
            if (_dataLayerProject is null)
                _dataLayerProject = new Project(dataLayerProjectPath);

            _businessProjectName = Path.GetFileNameWithoutExtension(businessProjectPath);
            _dataLayerProjectName = Path.GetFileNameWithoutExtension(dataLayerProjectPath);

            _peasyHelper = new PeasyHelper();
        }

        public async Task GeneratePeasyArchAsync()
        {
            var businessNameSpace = GetBaseNameSpace(_businessProjectName, _businessProjectPath);
            var dataLayerNameSpace = GetBaseNameSpace(_dataLayerProjectName, _dataLayerProjectPath);

            SaveFileThenAddToProject(_businessProjectPath + "/Domain/BaseDomain.cs", _peasyHelper.GetDomainBaseTemplate(businessNameSpace), _businessProject);
            SaveFileThenAddToProject(_businessProjectPath + $"/DataProxy/I{_solutionName}DataProxy.cs", _peasyHelper.GetDataProxyBaseTemplate(businessNameSpace, _solutionName), _businessProject);
            SaveFileThenAddToProject(_businessProjectPath + $"/Service/{_solutionName}SeviceBase.cs", _peasyHelper.GetServiceBaseTemplate(businessNameSpace, _solutionName), _businessProject);
            SaveFileThenAddToProject(_dataLayerProjectPath+ "/Repository/BaseRepository.cs", _peasyHelper.GetRepositoryBaseTemplate(dataLayerNameSpace, businessNameSpace, _solutionName), _dataLayerProject);

            DTECore.Settings.Postfix = " : BaseDomain";

            foreach (var table in Tables)
            {
                await GenerateBusinessLogic(table, _solutionName, businessNameSpace);
                GenerateDataLayer(table, dataLayerNameSpace, businessNameSpace);
            }
        }

        private void GenerateDataLayer(Table table, string dalNameSpace, string businessNameSpace)
        {
            var class_name = GetClassName(table);

            var dalRepository = _peasyHelper.GetRepositoryTemplate(dalNameSpace, businessNameSpace, class_name);
            var dalRepositoryfilePath = $"{_dataLayerProjectPath}/Repository/{class_name}Repository.cs";

            SaveFileThenAddToProject(dalRepositoryfilePath, dalRepository, _dataLayerProject);
        }
        private async Task GenerateBusinessLogic(Table table, string projectName, string nameSpace)
        {
            var class_name = GetClassName(table);
            var model_code = await DTECore.CreateModelAsync(table.DataBaseName, table.TableName);
            var businessDomainfilePath = $"{_businessProjectPath}/Domain/{class_name}.cs";
            var businessDataProxyfilePath = $"{_businessProjectPath}/DataProxy/I{class_name}DataProxy.cs";
            var businessServicefilePath = $"{_businessProjectPath}/Service/{class_name}Service.cs";
            var model_domain = _peasyHelper.GetDomainTemplate(nameSpace, model_code);
            var dataProxy = _peasyHelper.GetDataProxyTemplate(nameSpace, projectName, class_name);
            var service = _peasyHelper.GetServiceTemplate(nameSpace, class_name);

            SaveFileThenAddToProject(businessDomainfilePath, model_domain, _businessProject);
            SaveFileThenAddToProject(businessDataProxyfilePath, dataProxy, _businessProject);
            SaveFileThenAddToProject(businessServicefilePath, service, _businessProject);
        }


        private static string GetClassName(Table table)
        {
            return ModelCreateHelper.ColumnNameToPropName(table.TableName.Split('.').Last()).Replace("_", "");
        }
        private string GetBaseNameSpace(string ProjectName,string projectPath)
        {
            return ProjectName + projectPath?.Split(new string[] { ProjectName }, StringSplitOptions.None).Last().Replace("/", ".").Replace("\\", ".");
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
