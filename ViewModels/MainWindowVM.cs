using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using DTE.Cores;
using DTE.Models;
using DTE.Views.Windows;
using System.IO;
using System.Text.RegularExpressions;
using DTE.Models.Interfaces;

namespace DTE.ViewModels
{
    public class MainWindowVM : DataBindingBase45
    {
        private IDialogCoordinator dialogCoordinator;

        public RelayCommand SelectToEntityCommand
        {
            get
            {
                return new RelayCommand(_ => SelectToEntity());
            }
        }

        private void SelectToEntity()
        {
            string databaseName = null;
            if (SelectedNode is Database)
                databaseName = (SelectedNode as Database).DatabaseName;
            else if (SelectedNode is Table)
                databaseName = (SelectedNode as Table).DataBaseName;

            var tree = (SelectedNode as ITreeViewModel)?.ParentTreeBase;



            var sToEntWin = new QueryToEntityWindow(tree, databaseName);
            var res = sToEntWin.ShowDialog();
            if (res != null && res == true)
            {
                Document.Text = sToEntWin.Tag.ToString();
            }
        }

        public RelayCommand DataAnnotationsSettingsCommand
        {
            get
            {
                return new RelayCommand(_ => DataAnnotationsSettings());
            }
        }
        public RelayCommand TemplateSettingsCommand
        {
            get
            {
                return new RelayCommand(p => TemplateSettings((string)p));
            }
        }
        private void TemplateSettings(string type)
        {
            Views.Windows.TemplateWindow templateWindow = null;
            switch (type)
            {
                case "p":
                    templateWindow = new Views.Windows.TemplateWindow(TemplateType.Property);
                    templateWindow.Show();
                    break;
                case "fp":
                     templateWindow = new Views.Windows.TemplateWindow(TemplateType.FullProperty);
                    templateWindow.Show();
                    break;
                case "c":
                     templateWindow = new Views.Windows.TemplateWindow(TemplateType.Class);
                    templateWindow.Show();
                    break;
                default:
                    break;
            }
           
        }
        private void DataAnnotationsSettings()
        {
            Views.Windows.DataAnnotationsWindow annotationsWindow = new Views.Windows.DataAnnotationsWindow();
            annotationsWindow.Show();
        }
        public RelayCommand TypeConversionSettingsCommand
        {
            get
            {
                return new RelayCommand(_ => TypeConversionSettings());
            }
        }

        private void TypeConversionSettings()
        {
            Views.Windows.TypeConversion Window = new Views.Windows.TypeConversion();
            Window.Show();
        }

        public RelayCommand EditConnection
        {
            get
            {
                return new RelayCommand(_ => EditConnectionAsync());
            }
        }


        public RelayCommand CreateIntoFilesCommand
        {
            get
            {
                return new RelayCommand(_ => CreateIntoFilesAsync());
            }
        }

        private async void CreateIntoFilesAsync()
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Load = true;
                        string base_code = @"using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
namespace RenameThisNamespace
{
[code]
}
";
                        var connect = GetConnectionByNode();

                        if (connect == null)
                        {
                            Load = false;
                            return;
                        }

                        Globals.ModelCore = new ModelCore(connect.Connection);
                        Settings.SettingsDeserialize();
                        var _settings = Settings.Settings;
                        bool tableSelected = CheckSelection(connect);

                        if (tableSelected)
                        {
                            foreach (var database in connect.Databases)
                            {
                                if (database == null || database.Tables == null || database.Tables.Count == 0)
                                    continue;

                                var tables = database.Tables.Where(x => x.Checked);

                                if (tables == null)
                                    continue;

                                foreach (var table in tables)
                                {
                                    var model_code = Globals.ModelCore.CreateModel(database.DatabaseName, table.TableName,_settings);
                                    var modelname = $@"{ _settings.Prefix}{Cores.ModelCore.ColumnNameToPropName(table.TableName).Replace("_", "")}{ _settings.Postfix}";
                                    modelname = Regex.Replace(modelname, "[^a-zA-Z0-9_.]+", "");
                                    string cs_file_code = base_code.Replace("[code]", model_code);

                                    File.WriteAllText(dialog.SelectedPath+"/"+modelname+".cs", cs_file_code);
                                }
                            }
                        }
                        else
                        {
                            if (SelectedNode is Table)
                            {
                                var table = SelectedNode as Table;
                                string model_code = await Task.Run(() => Globals.ModelCore.CreateModel(table.DataBaseName, table.TableName, Settings.Settings));
                                string cs_file_code = base_code.Replace("[code]",model_code);
                                var modelname = $@"{ _settings.Prefix}{Cores.ModelCore.ColumnNameToPropName(table.TableName).Replace("_", "")}{ _settings.Postfix}";
                                File.WriteAllText(dialog.SelectedPath + "/" + modelname + ".cs", cs_file_code);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Load = false;
        }

        public RelayCommand ModelCommand
        {
            get
            {
                return new RelayCommand(_ => ModelCreateAsync());
            }
        }

        private async void ModelCreateAsync()
        {
            Load = true;

            var connect = GetConnectionByNode();

            if (connect == null)
            {
                Load = false;
                return;
            }



            Globals.ModelCore = new ModelCore(connect.Connection);
            Settings.SettingsDeserialize();

            bool tableSelected = CheckSelection(connect);

            if (tableSelected)
            {
                Task<string> task = Task.Run(() => Globals.ModelCore.CreateModels(connect.Databases.ToList(), Settings.Settings));

                Document.Text = await task;


            }
            else
            {
                if (SelectedNode is Table)
                {
                    var table = SelectedNode as Table;
                    Task<string> task = Task.Run(() => Globals.ModelCore.CreateModel(table.DataBaseName, table.TableName, Settings.Settings));
                    Document.Text = await task;
                }

            }


            Load = false;


        }

        public RelayCommand CoreCommand
        {
            get
            {
                return new RelayCommand(_ => CoreCreateAsync());
            }
        }

        private async void CoreCreateAsync()
        {
            Load = true;

            var connect = GetConnectionByNode();

            if (connect == null)
            {
                Load = false;
                return;
            }



            Globals.ModelCore = new ModelCore(connect.Connection);
            Settings.SettingsDeserialize();

            bool tableSelected = CheckSelection(connect);

            if (tableSelected)
            {
                Task<string> task = Task.Run(() => Globals.ModelCore.CreateCores(connect.Databases.ToList(), Settings.Settings));
                Document.Text = await task;

            }
            else
            {
                if (SelectedNode is Table)
                {
                    var table = SelectedNode as Table;
                    Task<string> task = Task.Run(() => Globals.ModelCore.CreateCore(table.DataBaseName, table.TableName, Settings.Settings));
                    Document.Text = await task;
                }

            }


            Load = false;
        }
        public async void RefreshDatabaseAsync(Database database)
        {
            try
            {
                database.LoadTablesAsync();
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
            }
        }

        private bool CheckSelection(TreeViewModel connect)
        {
            foreach (var db in connect.Databases)
            {
                if (db.Checked)
                {
                    return true;
                }
                foreach (var table in db.Tables)
                {
                    if (table.Checked)
                    {
                        return true;

                    }
                }
            }
            return false;
        }


        public RelayCommand DeleteConnection
        {
            get
            {
                return new RelayCommand(_ => DeleteConnectionAsync());
            }
        }
        public RelayCommand SettingsShow
        {
            get
            {
                return new RelayCommand(_ => FlyOutOpen = true);
            }
        }

        public MainWindowVM()
        {

        }

        public MainWindowVM(IDialogCoordinator instance)
        {
            this.dialogCoordinator = instance;


        }


        private TreeViewModel GetConnectionByNode()
        {
            if (SelectedNode == null)
            {
                GetFirstCheckedConnection();
            }

            if (SelectedNode is TreeViewModel)
            {
                return (SelectedNode as TreeViewModel);
            }
            else if (SelectedNode is DTE.Models.Database)
            {
                var db = SelectedNode as Database;

                return db.ParentTreeBase;
            }
            else if (SelectedNode is Table)
            {
                var table = SelectedNode as Table;

                return table.ParentTreeBase;
            }



            return null;
        }

        private void GetFirstCheckedConnection()
        {

            foreach (var conn in XMLCore.Connections)
            {
                foreach (var db in conn.Databases)
                {
                    if (db.Checked)
                    {
                        SelectedNode = db.ParentTreeBase;
                        return;
                    }
                    foreach (var table in db.Tables)
                    {
                        if (table.Checked)
                        {
                            SelectedNode = table.ParentTreeBase;
                            return;
                        }
                    }
                }
            }

        }

        private void EditConnectionAsync()
        {
            Views.Windows.ConnectionManagerWin connectionManagerWin = new Views.Windows.ConnectionManagerWin(SelectedNode as TreeViewModel);
            var res = connectionManagerWin.ShowDialog();

            XMLCore.ConnectionDeserialize();
            if (res != null && res == true)
            {
                var cmvm = connectionManagerWin.DataContext as ConnectionManagerVM;
                if (cmvm.EditModel != null)
                {
                    var con = XMLCore.Connections.FirstOrDefault(x => x.Connection.ConnectionName == cmvm.EditModel.ConnectionName);
                    con?.LoadDatabasesAsync();
                }
            }

        }
        private async void DeleteConnectionAsync()
        {
            if (SelectedNode != null && SelectedNode is TreeViewModel)
            {
                var res = await dialogCoordinator.ShowMessageAsync(this, $"Are you sure?", $"Are you sure to delete this connection: {(SelectedNode as TreeViewModel).Connection.ConnectionName}?", MessageDialogStyle.AffirmativeAndNegative);
                if (res == MessageDialogResult.Affirmative)
                {
                    XMLCore.Connections.Remove((SelectedNode as TreeViewModel));
                    XMLCore.ConnectionSerialize();
                    XMLCore.ConnectionDeserialize();
                }
            }

        }
        public async void LoadFirstConnAsync()
        {
            XMLCore.ConnectionDeserialize();
            if (XMLCore.Connections.Count > 0)
            {
                try
                {
                    XMLCore.Connections.First().LoadDatabasesAsync();

                }
                catch (Exception ex)
                {
                    await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
                }
            }
            else
            {
                new Views.Windows.ConnectionManagerWin().ShowDialog();
                XMLCore.ConnectionDeserialize();
                XMLCore.Connections.FirstOrDefault()?.LoadDatabasesAsync();


            }

        }

        object _selectedNode;
        public object SelectedNode
        {
            get
            {
                return _selectedNode;
            }

            set
            {
                _selectedNode = value;
                OnPropertyChanged();
                OnPropertyChanged("EditEnable");
                GetSchemaAsync();
            }
        }

        private async void GetSchemaAsync()
        {
            if (SelectedNode is Table)
            {
                var table = SelectedNode as Table;

                LoadSchema = true;

                var connect = GetConnectionByNode();

                if (connect == null)
                {
                    Load = false;
                    return;
                }



                Globals.ModelCore = new ModelCore(connect.Connection);
                Settings.SettingsDeserialize();

                Task<DataTable> schemaTask = Task.Run(() => Globals.ModelCore.GetSchemaInfo(table.TableName, table.DataBaseName));

                SchemaInfo = await schemaTask;

                LoadSchema = false;

                Load = true;

                Task<string> modelTask = Task.Run(() => Globals.ModelCore.CreateModel(table.DataBaseName, table.TableName, Settings.Settings));

                Document.Text = await modelTask;

                Load = false;
            }
        }

        Cores.ConnectionCore _xMLCore = new Cores.ConnectionCore();

        public Cores.ConnectionCore XMLCore
        {
            get
            {
                return _xMLCore;
            }

            set
            {
                _xMLCore = value;
                OnPropertyChanged();
            }
        }



        public async void RefreshConnAsync(string name)
        {
            try
            {
                var conn = XMLCore.Connections.FirstOrDefault(x => x.Connection.ConnectionName == name);
                if (conn != null)
                {
                    conn.LoadDatabasesAsync();
                    if (conn.Errors.Count > 0)
                    {
                        var ex = conn.Errors.First();
                        await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
            }

        }

        public bool EditEnable
        {
            get
            {
                if (SelectedNode != null && SelectedNode is Models.TreeViewModel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        bool _flyOutOpen;
        public bool FlyOutOpen
        {
            get
            {
                return _flyOutOpen;
            }

            set
            {
                _flyOutOpen = value;
                if (value == false)
                {
                    Settings.SettingsSerialize();
                }
                OnPropertyChanged();
            }
        }


        SettingsCore _settings = new SettingsCore();
        public SettingsCore Settings
        {
            get
            {
                return _settings;
            }

            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }

        TextDocument _editorText = new TextDocument();

        public TextDocument Document
        {
            get
            {
                return _editorText;
            }

            set
            {
                _editorText = value;
                OnPropertyChanged();
            }
        }
        bool _load;
        public bool Load
        {
            get
            {
                return _load;
            }

            set
            {
                _load = value;
                OnPropertyChanged();
            }
        }


        DataTable _schemaInfo;

        public DataTable SchemaInfo
        {
            get
            {
                return _schemaInfo;
            }

            set
            {
                if (value != null || value != _schemaInfo) _schemaInfo = value;
                OnPropertyChanged();
            }
        }

        bool _loadSchema;

        public bool LoadSchema
        {
            get
            {
                return _loadSchema;
            }

            set
            {
                if (value != _loadSchema) _loadSchema = value;
                OnPropertyChanged();
            }
        }
        public List<AccentColorMenuData> AccentColors { get; set; } = ThemeManager.Accents
                                             .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as System.Drawing.Brush, Color = a.Resources["AccentColorBrush"].ToString() })
                                             .ToList();
        public class AccentColorMenuData
        {
            public string Name { get; set; }
            public System.Drawing.Brush BorderColorBrush { get; set; }
            public System.Drawing.Brush ColorBrush { get; set; }
            public string Color { get; set; }
            public AccentColorMenuData()
            {

            }
            public RelayCommand ChangeAccentCommand
            {
                get
                {
                    return new RelayCommand(_ => DoChangeTheme(_.ToString()));
                }
            }

            protected virtual void DoChangeTheme(string sender)
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                var accent = ThemeManager.GetAccent(sender);
                SettingsCore settings = new SettingsCore();
                settings.Settings.AccenColor = sender;
                settings.SettingsSerialize();
                ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            }
        }





    }

}
