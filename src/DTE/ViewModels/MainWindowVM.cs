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
using DTE.Domains;
using DTE.Views.Windows;
using System.IO;
using System.Text.RegularExpressions;
using DTE.Domains.Interfaces;
using DTE.CORE;
using DTE.CORE.Helpers;

namespace DTE.ViewModels
{
    public partial class MainWindowVM : DataBindingBase45
    {
        private IDialogCoordinator dialogCoordinator;

        #region Commands
        public RelayCommand SelectToEntityCommand
        {
            get
            {
                return new RelayCommand(_ => SelectToEntity());
            }
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
        public RelayCommand TypeConversionSettingsCommand
        {
            get
            {
                return new RelayCommand(_ => TypeConversionSettings());
            }
        }
        public RelayCommand ModelCommand
        {
            get
            {
                return new RelayCommand(_ => ModelCreateAsync());
            }
        }
        public RelayCommand DeleteConnection
        {
            get
            {
                return new RelayCommand(_ => DeleteConnectionAsync());
            }
        }
        public RelayCommand SettingsCommand
        {
            get
            {
                return new RelayCommand(_ => FlyOutOpen = true);
            }
        }
        #endregion

        #region Constructor
        public MainWindowVM()
        {

        }

        public MainWindowVM(IDialogCoordinator instance)
        {
            this.dialogCoordinator = instance;


        }

        #endregion

        #region Methods
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
        private void TypeConversionSettings()
        {
            Views.Windows.TypeConversion Window = new Views.Windows.TypeConversion();
            Window.Show();
        }
        private void CreateIntoFilesAsync()
        {
            var selectedTables = GetTablesForModelCreate();
            if (selectedTables is null)
                return;

            new CreateIntoFileWindow(selectedTables).ShowDialog();
        }

        public List<Table> GetTablesForModelCreate()
        {
            try
            {
                var selectedTreeModel = GetConnectionByNode();
                if (selectedTreeModel is null)
                    return null;

                var isChecked = CheckSelection(selectedTreeModel);
                List<Table> Tables = new List<Table>();

                if (isChecked)
                {
                    foreach (var database in (SelectedNode as ITreeViewModel).ParentTreeBase.Databases)
                    {
                        if (database == null || database.Tables == null || database.Tables.Count == 0)
                            continue;

                        var local_tables = database.Tables.Where(x => x.Checked);

                        if (local_tables == null)
                            continue;
                        foreach (var table in local_tables)
                            Tables.Add(table);
                    }
                }
                else if ((SelectedNode as Database) != null)
                {
                    foreach (var table in (SelectedNode as Database).Tables)
                        Tables.Add(table);

                }
                else if ((SelectedNode as Table) != null)
                {
                    Tables.Add((SelectedNode as Table));
                }

                return Tables;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return null;
        }

        private async void ModelCreateAsync()
        {
            Load = true;
            try
            {
                var selectedNode = GetConnectionByNode();

                if (selectedNode == null)
                {
                    Load = false;
                    return;
                }

                Settings.SettingsDeserialize();
                var tables = GetTablesForModelCreate();
                Document.Text = await CreateModelsAsync(tables) ?? "";

            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }

            Load = false;


        }
        private async Task<string> CreateModelsAsync(List<Table> tables)
        {
            string model_code = "";
            try
            {
                var selectedNode = GetConnectionByNode();
                if (selectedNode == null)
                    return model_code;

                var dteCore = Globals.CreateCoreByConnection(selectedNode.ConnectionBuilder);

                foreach (var table in tables)
                {
                    model_code += await dteCore.CreateModelAsync(table.DataBaseName, table.TableName);
                    model_code += Environment.NewLine;
                }
            }
            catch (Exception ex)
            {

                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }


            return model_code;
        }
        private bool CheckSelection(TreeViewModel connect)
        {
            if (connect is null)
                return false;

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
        private TreeViewModel GetConnectionByNode()
        {
            if (SelectedNode == null)
                GetFirstCheckedConnection();

            return (SelectedNode as ITreeViewModel).ParentTreeBase;
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
        private async void EditConnectionAsync()
        {
            try
            {
                Views.Windows.ConnectionManagerWin connectionManagerWin = new Views.Windows.ConnectionManagerWin(SelectedNode as TreeViewModel);
                var res = connectionManagerWin.ShowDialog();

                XMLCore.ConnectionDeserialize();
                if (res != null && res == true)
                {
                    var cmvm = connectionManagerWin.DataContext as ConnectionManagerVM;
                    if (cmvm.ConnectionBuilder != null)
                    {
                        var con = XMLCore.Connections.FirstOrDefault(x => x.ConnectionBuilder.Id == cmvm.ConnectionBuilder.Id);
                        con?.LoadDatabasesAsync();
                    }
                }
            }
            catch (Exception ex)
            {

                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }


        }
        private async void DeleteConnectionAsync()
        {
            if (SelectedNode != null && SelectedNode is TreeViewModel)
            {
                var res = await dialogCoordinator.ShowMessageAsync(this, $"Are you sure?", $"Are you sure to delete this connection: {(SelectedNode as TreeViewModel).ConnectionBuilder.ConnectionName}?", MessageDialogStyle.AffirmativeAndNegative);
                if (res == MessageDialogResult.Affirmative)
                {
                    XMLCore.Connections.Remove((SelectedNode as TreeViewModel));
                    XMLCore.ConnectionSerialize();
                    XMLCore.ConnectionDeserialize();
                }
            }

        }

        private async void GetSchemaAsync()
        {
            try
            {
                if (SelectedNode is Table)
                {
                    var table = SelectedNode as Table;

                    LoadSchema = true;

                    var selectedNode = GetConnectionByNode();

                    if (selectedNode == null)
                    {
                        Load = false;
                        return;
                    }



                    var dteCore = Globals.CreateCoreByConnection(selectedNode.ConnectionBuilder);
                    Settings.SettingsDeserialize();

                    SchemaInfo = await dteCore.GetSchemaAsync(table.DataBaseName, table.TableName);

                    LoadSchema = false;

                    Load = true;

                    Document.Text = await dteCore.CreateModelAsync(table.DataBaseName, table.TableName) ?? "";


                    Load = false;
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }


        }
        public async void RefreshDatabaseAsync(Database database)
        {
            try
            {
                await database.LoadTablesAsync();
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }
        }
        public async void LoadFirstConnAsync()
        {
            try
            {
                XMLCore.ConnectionDeserialize();
                if (XMLCore.Connections.Count > 0)
                {
                    try
                    {
                        await XMLCore.Connections.First().LoadDatabasesAsync();

                    }
                    catch (Exception ex)
                    {
                        await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
                    }
                }
                else
                {
                    new Views.Windows.ConnectionManagerWin().ShowDialog();
                    XMLCore.ConnectionDeserialize();
                    XMLCore.Connections.FirstOrDefault()?.LoadDatabasesAsync();
                }
            }
            catch (Exception ex)
            {

                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }


        }
        public async void RefreshConnAsync(string id)
        {
            try
            {
                var conn = XMLCore.Connections.FirstOrDefault(x => x.ConnectionBuilder.Id.ToString() == id);
                if (conn != null)
                {
                    await conn.LoadDatabasesAsync();
                }
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }

        }
        #endregion

        #region Properties & Variables


        object _selectedNode;
        Cores.DTEXMLConnection _xmlCore = new Cores.DTEXMLConnection();
        bool _flyOutOpen;
        DTESettings _settings = new DTESettings();
        bool _load;
        DataTable _schemaInfo;
        bool _loadSchema;
        TextDocument _editorText = new TextDocument();


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
        public Cores.DTEXMLConnection XMLCore
        {
            get
            {
                return _xmlCore;
            }

            set
            {
                _xmlCore = value;
                OnPropertyChanged();
            }
        }
        public bool EditEnable
        {
            get
            {
                if (SelectedNode != null && SelectedNode is Domains.TreeViewModel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
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
        public DTESettings Settings
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

        public RelayCommand ChangeAccentCommand
        {
            get
            {
                return new RelayCommand(p => DoChangeTheme(p.ToString()));
            }
        }
        Random rnd = new Random();
        protected virtual void DoChangeTheme(string sender)
        {
            if (sender == "R")
                sender = AccentColors[rnd.Next(0, AccentColors.Count - 1)].Name;
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(sender);
            Settings.Settings.AccentName = sender;
            Settings.SettingsSerialize();
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
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

        }


        #endregion

    }

}
