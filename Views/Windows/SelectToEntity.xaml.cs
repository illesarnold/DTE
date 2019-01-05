using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ModelGen.ViewModels;
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

namespace ModelGen.Views.Windows
{
    /// <summary>
    /// Interaction logic for SelectToEntity.xaml
    /// </summary>
    public partial class SelectToEntity : MetroWindow
    {
        SelectToEntityVM vm = new SelectToEntityVM(DialogCoordinator.Instance);

        public SelectToEntity()
        {
            InitializeComponent();
            vm = new SelectToEntityVM(DialogCoordinator.Instance);
            this.Loaded += SelectToEntity_Loaded;
            DataContext = vm;
        }

        private void SelectToEntity_Loaded(object sender, RoutedEventArgs e)
        {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ModelGen.Resources.sql.xsd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    HLeditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }

        public SelectToEntity(Models.TreeViewModel treeViewModel)
        {
            InitializeComponent();
            this.Loaded += SelectToEntity_Loaded;
            if (treeViewModel != null)
            {
                vm = new SelectToEntityVM(DialogCoordinator.Instance, treeViewModel.Connection);
            }
            else
            {
                vm = new SelectToEntityVM(DialogCoordinator.Instance);
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
