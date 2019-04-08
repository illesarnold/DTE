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
                databaseName =  (SelectedNode as Database).DatabaseName;
            else if (SelectedNode is Table)
                databaseName = (SelectedNode as Table).DataBaseName;

            var tree = (SelectedNode as ITreeViewModel)?.ParentTreeBase;

         

            var sToEntWin = new QueryToEntityWindow(tree,databaseName);
            var res = sToEntWin.ShowDialog();
            if (res != null && res == true)
            {
                Document.Text = sToEntWin.Tag.ToString();
            }
        }

        public RelayCommand DataAnnotationsSettings
        {
            get
            {
                return new RelayCommand(_ => DataAnnotationsSettingsAsync());
            }
        }

        private void DataAnnotationsSettingsAsync()
        {
            Views.Windows.DataAnnotationsWindow annotationsWindow = new Views.Windows.DataAnnotationsWindow();
            annotationsWindow.Show();
        }
        public RelayCommand TypeConversionSettings
        {
            get
            {
                return new RelayCommand(_ => TypeConversionSettingsAsync());
            }
        }

        private void TypeConversionSettingsAsync()
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



            Globals.cc = new ConnectionCore(connect.Connection);
            Settings.SettingsDeserialize();

            bool tableSelected = CheckSelection(connect);

            if (tableSelected)
            {
                Task<string> task = Task.Run(() => Globals.cc.CreateModels(connect.Databases.ToList(), Settings.Settings));

                Document.Text = await task;
               

            }
            else
            {
                if (SelectedNode is Table)
                {
                    var table = SelectedNode as Table;
                    Task<string> task = Task.Run(() => Globals.cc.CreateModel(table.DataBaseName, table.TableName, Settings.Settings));
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



            Globals.cc = new ConnectionCore(connect.Connection);
            Settings.SettingsDeserialize();

            bool tableSelected = CheckSelection(connect);

            if (tableSelected)
            {
                Task<string> task = Task.Run(() => Globals.cc.CreateCores(connect.Databases.ToList(), Settings.Settings));
                Document.Text = await task;

            }
            else
            {
                if (SelectedNode is Table)
                {
                    var table = SelectedNode as Table;
                    Task<string> task = Task.Run(() => Globals.cc.CreateCore(table.DataBaseName, table.TableName, Settings.Settings));
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



                Globals.cc = new ConnectionCore(connect.Connection);
                Settings.SettingsDeserialize();

                Task<DataTable> schemaTask = Task.Run(() => Globals.cc.GetSchemaInfo(table.TableName,table.DataBaseName));

                SchemaInfo = await schemaTask;

                LoadSchema = false;

                Load = true;

                Task<string> modelTask = Task.Run(() => Globals.cc.CreateModel(table.DataBaseName, table.TableName, Settings.Settings));

                Document.Text = await modelTask;

                Load = false;
            }
        }

        Cores.ConnectionXMLCore _xMLCore = new Cores.ConnectionXMLCore();

        public Cores.ConnectionXMLCore XMLCore
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
