using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DTE.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace DTE.Views.Windows
{
    /// <summary>
    /// Interaction logic for ConnectionManagerWin.xaml
    /// </summary>
    public partial class ConnectionManagerWin : MetroWindow
    {
        ConnectionManagerVM vm = new ConnectionManagerVM(DialogCoordinator.Instance);

        public ConnectionManagerWin()
        {
            InitializeComponent();
            vm = new ConnectionManagerVM(DialogCoordinator.Instance);

            DataContext = vm;
        }
        public ConnectionManagerWin(Models.TreeViewModel treeViewModel)
        {
            InitializeComponent();
            vm = new ConnectionManagerVM(DialogCoordinator.Instance,treeViewModel.Connection);

            DataContext = vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (DataContext as ViewModels.ConnectionManagerVM).ConnModel.Password = password.Password;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            vm.Connect.Execute(null);
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            {
                this.DialogResult = true;

            }
            this.Close();
        }
    }
}
