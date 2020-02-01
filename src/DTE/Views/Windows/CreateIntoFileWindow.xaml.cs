using DTE.Domains;
using DTE.ViewModels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DTE.Views.Windows
{
    /// <summary>
    /// Interaction logic for CreateIntoFileWindow.xaml
    /// </summary>
    public partial class CreateIntoFileWindow : MetroWindow
    {
        public CreateIntoFileWindow()
        {
            InitializeComponent();
            DataContext = new CreateIntoFilesVM();
        }
        public CreateIntoFileWindow(TreeViewModel selectedNode,Database selectedDatabase,Table selectedTable,bool isChecked)
        {
            InitializeComponent();
            DataContext = new CreateIntoFilesVM(selectedNode,selectedDatabase,selectedTable,isChecked);
        }
    }
}
