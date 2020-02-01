using DTE.ConnectionCore;
using DTE.CORE;
using DTE.Cores;
using DTE.Domains;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DTE.ViewModels
{
    public class ConnectionManagerVM : DataBindingBase45
    {
        public RelayCommand TestConnection
        {
            get
            {
                return new RelayCommand(_ => TestConnectionAsync());
            }
        }

        public RelayCommand Connect
        {
            get
            {
                return new RelayCommand(_ => SaveAndConnect());
            }
        }

        private IDialogCoordinator dialogCoordinator;

        public ConnectionManagerVM()
        {
        }

        public ConnectionManagerVM(IDialogCoordinator instance, ConnectionBuilderBase connectionModel = null)
        {
            dialogCoordinator = instance;
            if (connectionModel != null)
            {
                isEdit = true;
                DTEXMLConnection xMLCore = new DTEXMLConnection();
                var connection_builder_base = xMLCore.Connections.FirstOrDefault(x => x.ConnectionBuilder.Id == connectionModel.Id)?.ConnectionBuilder;
                ConnectionBuilder = ConnectionBuilderFactory.CreateConnectionBuilder(connection_builder_base.ConnectionType, connection_builder_base.ConnectionString);
                ConnectionBuilder.Id = connection_builder_base.Id;
                ConnectionBuilder.ConnectionName = connection_builder_base.ConnectionName;
                ConnectionBuilder.InitBuilder();
            }
            else
            {
                ConnectionBuilder = ConnectionBuilderFactory.CreateConnectionBuilder(SupportedConnectionsTypes.MySQL);
            }
        }

        private Exception exception;

        private async void TestConnectionAsync()
        {
            ButtonsEnable = false;

            var controller = await dialogCoordinator.ShowProgressAsync(this, "Please wait...", "Try to connecting!");
            controller.SetIndeterminate();

            bool isValid = await Task.Run(() => CheckConnection());
            await controller.CloseAsync();

            if (isValid == true)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Successful!", "Connection successfully!");
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {exception.Message}  \r\nStackTrace: {exception.StackTrace}");
            }
        }

        private async void SaveAndConnect()
        {
            try
            {
                ConnectionBuilder.BuildConnectionString();

                DTEXMLConnection xMLCore = new DTEXMLConnection();
                if (isEdit)
                {
                    var xmlModel = xMLCore.Connections.FirstOrDefault(x => x.ConnectionBuilder.Id == ConnectionBuilder.Id);
                    var index = xMLCore.Connections.IndexOf(xmlModel);
                    xMLCore.Connections.RemoveAt(index);
                    xmlModel.ConnectionBuilder = (ConnectionBuilderBase)ConnectionBuilder;
                    xMLCore.Connections.Insert(index, xmlModel);
                }
                else
                {
                    xMLCore.Connections.Insert(0, new TreeViewModel() { ConnectionBuilder = (ConnectionBuilder as ConnectionBuilderBase) });
                }
                xMLCore.ConnectionSerialize();
            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  \r\nStackTrace: {ex.StackTrace}");
            }
        }

        private bool CheckConnection()
        {
            try
            {
                ConnectionBuilder.BuildConnectionString();
                IDbConnection connection;
                switch (ConnectionBuilder.ConnectionType)
                {
                    case CORE.SupportedConnectionsTypes.MySQL:
                        connection = new MySqlConnection(ConnectionBuilder.ConnectionString);
                        connection.Open();
                        connection.Close();
                        break;

                    case CORE.SupportedConnectionsTypes.MSSQL:
                        connection = new SqlConnection(ConnectionBuilder.ConnectionString);
                        connection.Open();
                        connection.Close();
                        break;

                    case CORE.SupportedConnectionsTypes.PostgreSQL:
                        connection = new NpgsqlConnection(ConnectionBuilder.ConnectionString);
                        connection.Open();
                        connection.Close();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }

            return true;
        }

        private IConnectionBuilder _connectionBuilder = new MySQLConnectionBuilder();

        public IConnectionBuilder ConnectionBuilder
        {
            get
            {
                return _connectionBuilder;
            }

            set
            {
                _connectionBuilder = value;
                OnPropertyChanged();
            }
        }

        private List<KeyValuePair<SupportedConnectionsTypes, string>> _connTypes = Globals.ConnTypes;

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

        public SupportedConnectionsTypes SelectedConnection
        {
            get { return ConnectionBuilder.ConnectionType; }
            set
            {
                if (ConnectionBuilder.ConnectionType != value)
                    ConnectionBuilder = ConnectionBuilderFactory.CreateConnectionBuilder(value);
                OnPropertyChanged();
            }
        }

        private bool _buttonsEnable = true;
        private bool isEdit;

        public bool ButtonsEnable
        {
            get
            {
                return _buttonsEnable;
            }

            set
            {
                _buttonsEnable = value;
                OnPropertyChanged();
            }
        }
    }
}