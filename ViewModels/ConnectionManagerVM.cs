using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTE.Models;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using DTE.Cores;
using Npgsql;

namespace DTE.ViewModels
{
    public class ConnectionManagerVM : DataBindingBase45
    {
        public ConnectionModel EditModel = null;
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

        public ConnectionManagerVM(IDialogCoordinator instance, ConnectionModel connectionModel = null)
        {
            dialogCoordinator = instance;
            if (connectionModel != null)
            {
                EditModel = connectionModel;

                ConnModel.ConnectionName = connectionModel.ConnectionName;
                ConnModel.ConnType = connectionModel.ConnType;
                ConnModel.Host = connectionModel.Host;
                ConnModel.Port = connectionModel.Port;
                ConnModel.UserName = connectionModel.UserName;
                ConnModel.Password = connectionModel.Password;
            }

        }
        private Exception exception;
        private async void TestConnectionAsync()
        {
            ButtonsEnable = false;

            var controller = await dialogCoordinator.ShowProgressAsync(this, "Please wait...", "Try to connecting!");
            controller.SetIndeterminate();

            bool res = await Task.Run(() => CheckConnection());
            await controller.CloseAsync();

            if (res == true)
            {
                await dialogCoordinator.ShowMessageAsync(this, "Successful!", "Connection successfully!");

            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {exception.Message}  /r/nStackTrace: {exception.StackTrace}");

            }



        }
        private async void SaveAndConnect()
        {
            try
            {
                ConnectionCore xMLCore = new ConnectionCore();
                if (EditModel != null)
                {
                    EditModel = xMLCore.Connections.FirstOrDefault(x => x.Connection.ConnectionName == EditModel.ConnectionName)?.Connection;

                    EditModel.ConnectionName = ConnModel.ConnectionName;
                    EditModel.ConnType = ConnModel.ConnType;
                    EditModel.Host = ConnModel.Host;
                    EditModel.Port = ConnModel.Port;
                    EditModel.UserName = ConnModel.UserName;
                    EditModel.Password = ConnModel.Password;
                    EditModel.BuilderBuildConnection();
                }
                else
                {
                    ConnModel.BuilderBuildConnection();
                    xMLCore.Connections.Insert(0, new TreeViewModel() { Connection = ConnModel });

                }
                xMLCore.ConnectionSerialize();



            }
            catch (Exception ex)
            {
                await dialogCoordinator.ShowMessageAsync(this, $"Error!", $"Error message: {ex.Message}  /r/nStackTrace: {ex.StackTrace}");
            }
        }

        private bool CheckConnection()
        {
            try
            {
                IDbConnection connection;
                ConnModel.BuilderBuildConnection();

                switch (ConnModel.ConnType)
                {
                    case ConnectionTypes.MySQL:
                        connection = new MySqlConnection(ConnModel.ConnString);
                        connection.Open();
                        connection.Close();
                        break;
                    case ConnectionTypes.SQL_CE:
                        connection = new SqlConnection(ConnModel.ConnString);
                        connection.Open();
                        connection.Close();
                        break;
                    case ConnectionTypes.SQL_Server:
                        connection = new SqlConnection(ConnModel.ConnString);
                        connection.Open();
                        connection.Close();
                        break;
                    case ConnectionTypes.PostgreSQL:
                        connection = new NpgsqlConnection(ConnModel.ConnString);
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

        ConnectionModel _connModel = new ConnectionModel();
        public ConnectionModel ConnModel
        {
            get
            {
                return _connModel;
            }

            set
            {
                _connModel = value;
                OnPropertyChanged();
            }
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


        bool _buttonsEnable = true;
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
