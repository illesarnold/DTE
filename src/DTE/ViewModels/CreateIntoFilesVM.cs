using DTE.CORE;
using DTE.CORE.Helpers;
using DTE.CORE.Interfaces;
using DTE.Cores;
using DTE.Domains;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DTE.ViewModels
{
    public class CreateIntoFilesVM : DataBindingBase45
    {
        public TreeViewModel SelectedNode { get; }
        public Database SelectedDatabase { get; }
        public Table SelectedTable { get; }
        public bool IsChecked { get; }
        public RelayCommand CreateCommand
        {
            get
            {
                return new RelayCommand(_ => CreateFilesAsync());
            }
        }
        public RelayCommand SelectProjectPathCommand
        {
            get
            {
                return new RelayCommand(_ => SelectProjectPath());
            }
        }
        public RelayCommand SelectFolderPathCommand
        {
            get
            {
                return new RelayCommand(_ => SelectFolderPath());
            }
        }

        private void SelectFolderPath()
        {
            DestinationPath = Globals.SelectFolderPath() ?? DestinationPath;
        }
        private void SelectProjectPath()
        {
            ProjectPath = Globals.SelectProjectPath() ?? ProjectPath;
        }


        public CreateIntoFilesVM()
        {

        }
        public CreateIntoFilesVM(List<Table> tables,DTECore dTECore)
        {
            Tables = new ObservableCollection<Table>(tables);
            AllTablesCount = Tables.Count;
            _dTECore = dTECore;
        }

        public async void CreateFilesAsync()
        {
            try
            {
                CreatedTablesCount = 0;

                if (string.IsNullOrEmpty(DestinationPath))
                    return;

                string base_code = $@"using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
namespace {NameSpace}
{{
[code]
}}
";

                var settings = new DTESettings().Settings;

                AllTablesCount = Tables.Count();
                CreatedTablesCount = 0;

                foreach (var table in Tables)
                {
                    await CreateModelThanSave(base_code, _dTECore, _dTECore.Settings, table.DataBaseName, table);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async Task CreateModelThanSave(string base_code, DTECore dteCore, ISettings settings, string database, Table table)
        {
            var model_code = await dteCore.CreateModelAsync(database, table.TableName);
            var modelname = GetModelName(settings, table);
            modelname = Regex.Replace(modelname, "[^a-zA-Z0-9_.]+", "");
            string cs_file_code = base_code.Replace("[code]", model_code);
            SaveFile(cs_file_code, modelname);
            CreatedTablesCount += 1;
        }

        private string GetModelName(ISettings settings, Table table)
        {
            return $@"{settings.Prefix}{DTE.CORE.Helpers.ModelCreateHelper.ColumnNameToPropName(table.TableName.Split('.').Last()).Replace("_", "")}{ settings.Postfix}";
        }

        private void SaveFile(string cs_file_code, string modelname)
        {
            var filePath = $"{DestinationPath}/{modelname}.cs";

            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            System.IO.File.WriteAllText(file.FullName, cs_file_code);
            if (string.IsNullOrEmpty(ProjectPath) == false)
                AddToProject(filePath);
        }

        private void AddToProject(string filePath)
        {
            var proj = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault(pr => pr.FullPath == ProjectPath);
            if (proj is null)
                proj = new Project(ProjectPath);
            var projectName = Path.GetFileNameWithoutExtension(ProjectPath);
            var filePathToProject = DestinationPath?.Split(new string[] { projectName }, StringSplitOptions.None).Last() + "\\" + Path.GetFileName(filePath);
            if (filePathToProject.StartsWith("\\"))
                filePathToProject = filePathToProject.Remove(0, 1);

            if (proj.Items.FirstOrDefault(x => x.EvaluatedInclude == filePathToProject) is null)
            {
                proj.AddItem("Compile", filePathToProject);
                proj.Save();
            }



        }

        private string _projectPath;

        public string ProjectPath
        {
            get { return _projectPath; }
            set { _projectPath = value; ChangeDestinationPath(); ChangeNameSpace(); OnPropertyChanged(); }
        }

        private void ChangeNameSpace()
        {
            if (string.IsNullOrEmpty(NameSpace) && string.IsNullOrEmpty(ProjectPath) == false)
            {
                var projectName = Path.GetFileNameWithoutExtension(ProjectPath);
                NameSpace = projectName + DestinationPath?.Split(new string[] { projectName }, StringSplitOptions.None).Last().Replace("/", ".").Replace("\\", ".");
            }
        }

        private void ChangeDestinationPath()
        {
            if (string.IsNullOrEmpty(DestinationPath) && string.IsNullOrEmpty(ProjectPath) == false)
                DestinationPath = Path.GetDirectoryName(ProjectPath) + "\\Domain";
        }

        private string _destinationPath;

        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; OnPropertyChanged(); }
        }


        private int _allTablesCount = 1;

        public int AllTablesCount
        {
            get { return _allTablesCount; }
            set { _allTablesCount = value; OnPropertyChanged(); }
        }

        private int _createdTablesCount;

        public int CreatedTablesCount
        {
            get { return _createdTablesCount; }
            set { _createdTablesCount = value; OnPropertyChanged(); }
        }

        private string _nameSpace;

        public string NameSpace
        {
            get { return _nameSpace; }
            set { _nameSpace = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Table> _tables = new ObservableCollection<Table>();
        private readonly DTECore _dTECore;

        public ObservableCollection<Table> Tables
        {
            get { return _tables; }
            set { _tables = value; OnPropertyChanged(); }
        }


    }
}
