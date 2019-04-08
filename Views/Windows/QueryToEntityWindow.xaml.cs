using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using DTE.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;

namespace DTE.Views.Windows
{
    /// <summary>
    /// Interaction logic for SelectToEntity.xaml
    /// </summary>
    public partial class QueryToEntityWindow : MetroWindow
    {
        QueryToEntityVM vm = new QueryToEntityVM(DialogCoordinator.Instance);

        public QueryToEntityWindow()
        {
            InitializeComponent();
            vm = new QueryToEntityVM(DialogCoordinator.Instance);
            this.Loaded += SelectToEntity_Loaded;
            DataContext = vm;
        }

        private void SelectToEntity_Loaded(object sender, RoutedEventArgs e)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("DTE.Resources.sql.xsd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    HLeditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public QueryToEntityWindow(Models.TreeViewModel treeViewModel,string databaseName)
        {
            InitializeComponent();
            this.Loaded += SelectToEntity_Loaded;
            if (treeViewModel != null)
            {
                vm = new QueryToEntityVM(DialogCoordinator.Instance, treeViewModel.Connection,databaseName);
            }
            else
            {
                vm = new QueryToEntityVM(DialogCoordinator.Instance);
            }
            DataContext = vm;
        }
        

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
