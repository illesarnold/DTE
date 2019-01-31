using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;
using DTE.Cores;
using DTE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DTE.ViewModels
{
    public class QueryToEntityVM : DataBindingBase45
    {
        IDialogCoordinator _dialogCoordinator;
        public QueryToEntityVM()
        {

        }

        public QueryToEntityVM(IDialogCoordinator instance, ConnectionModel connection = null,string databaseName = null)
        {
            _dialogCoordinator = instance;
            if (connection != null)
            {
                Type = connection.ConnType;
                ConnString = GetConnString(connection);
            }
            if (databaseName != null)
            {
                ConnString += $"database={databaseName};";
            }
        }
        public RelayCommand CreateCommand
        {
            get
            {
                return new RelayCommand(_ => CreateAsync(_));
            }
        }

        private async void CreateAsync(object window)
        {
            Window win = window as Window;
            SettingsCore settings = new SettingsCore();
            settings.SettingsSerialize();
            var cc = new Cores.ConnectionCore(new ConnectionModel(Type,ConnString));
            string entity = "";
            try
            {
                entity = cc.CreateModel(Document.Text,settings.Settings);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
                return;
            }

            if(cc.Errors.Count > 0)
            {
                await _dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {cc.Errors.First().Message}  /r/nStackTrace: {cc.Errors.First().StackTrace}");

            }
            else
            {
                win.Tag = entity;
                win.DialogResult = true;
                win.Close();
            }
        }

        private string GetConnString(ConnectionModel connection)
        {
            switch (connection.ConnType)
            {
                case ConnectionTypes.MySQL:
                    return connection.ConnString;
                case ConnectionTypes.SQL_Server:
                    return connection.ConnString;
                case ConnectionTypes.SQL_CE:
                    return connection.ConnString;
                case ConnectionTypes.SQLite:
                    break;
                case ConnectionTypes.Firebird:
                    break;
                case ConnectionTypes.Oracle:
                    break;
                case ConnectionTypes.PostgreSQL:
                    break;
                default:
                    break;
            }
            return "";
        }


        List<Models.ConnectionType> _connTypes = Globals.ConnTypes;

        public List<Models.ConnectionType> ConnTypes
        {
            get
            {
                return _connTypes;
            }

            set
            {
                _connTypes = value;
            }
        }

        ConnectionTypes _type;
        
        public ConnectionTypes Type
        {
            get
            {
                return _type;
            }
        
            set
            {
                _type = value;
        		OnPropertyChanged();
            }
        }
        string _connString;
        
        public string ConnString
        {
            get
            {
                return _connString;
            }
        
            set
            {
                _connString = value;
        		OnPropertyChanged();
            }
        }
        TextDocument _document = new TextDocument();
        
        public TextDocument Document
        {
            get
            {
                return _document;
            }
        
            set
            {
                _document = value;
        		OnPropertyChanged();
            }
        }

       
    }
}
