using ICSharpCode.AvalonEdit.Document;
using MahApps.Metro.Controls.Dialogs;
using DTE.Cores;
using DTE.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DTE.CORE;

namespace DTE.ViewModels
{
    public class QueryToEntityVM : DataBindingBase45
    {
        IDialogCoordinator _dialogCoordinator;
        public QueryToEntityVM()
        {

        }
        public QueryToEntityVM(IDialogCoordinator instance)
        {
            _dialogCoordinator = instance;
        }

        public QueryToEntityVM(IDialogCoordinator instance, ConnectionBuilderBase connectionBuilder, string databaseName = null)
        {
            _dialogCoordinator = instance;
            if (connectionBuilder != null)
                ConnectionBuilder = connectionBuilder;
            if (databaseName != null)
                ConnectionBuilder.Database = databaseName;
            ConnectionBuilder.BuildConnectionString();
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
            string entity = "";
            try
            {
                var dteCore = Globals.CreateCoreByConnection(ConnectionBuilder);
                entity = dteCore.CreateModelByQuery(Document.Text);
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
                return;
            }

            win.Tag = entity;
            win.DialogResult = true;
            win.Close();

        }
        List<KeyValuePair<SupportedConnectionsTypes, string>> _connTypes = Globals.ConnTypes;

        public List<KeyValuePair<SupportedConnectionsTypes, string>> ConnTypes
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

        public ConnectionBuilderBase ConnectionBuilder { get; set; }
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
