using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using DTE.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace DTE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        MainWindowVM vm = new MainWindowVM(DialogCoordinator.Instance);
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //vm.LoadFirstConnAsync();
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("DTE.Resources.csharp.xsd"))
            {
                using (XmlTextReader reader = new XmlTextReader(s))
                {
                    HLeditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
        }
        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream("assembly.folder." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as ViewModels.MainWindowVM).SelectedNode = TreeViewConn.SelectedItem;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            var nw = new Views.Windows.ConnectionManagerWin().ShowDialog();
            if (nw != null && nw == true)
            {

                vm.XMLCore.ConnectionDeserialize();
            }
        }

        private void ConnectRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            vm.RefreshConnAsync((sender as Button).Tag.ToString());
        }

        private void HLeditor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;



            HLeditor.FontSize += e.Delta/100;


        }

        private void DatabaseRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Database db = btn.Tag as Database;
            vm.RefreshDatabaseAsync(db);
                
        }
    }
}
