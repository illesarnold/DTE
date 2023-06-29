using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using DTE.Domains;
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
using MahApps.Metro.IconPacks;

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

        private void ThemeManager_IsThemeChanged(object sender, Fluent.OnThemeChangedEventArgs args)
        {
            // Sync Fluent and MahApps ThemeManager
            var fluentRibbonTheme = args?.AppTheme ?? Fluent.ThemeManager.DetectAppStyle().Item1;
             }

        private void MahMetroWindow_Closed(object sender, EventArgs e)
        {
            Fluent.ThemeManager.IsThemeChanged -= ThemeManager_IsThemeChanged;
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
            this.TitleBar = this.FindChild<Fluent.RibbonTitleBar>("RibbonTitleBar");
            this.TitleBar.InvalidateArrange();
            this.TitleBar.UpdateLayout();
            //this.ThemeManager_IsThemeChanged(null, null);

            Fluent.ThemeManager.IsThemeChanged += ThemeManager_IsThemeChanged;
            Fluent.ThemeManager.ChangeAppTheme(Application.Current, "BaseDark");

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
            if (sender is Button btn)
            {                
                Database db = btn.Tag as Database;
                vm.RefreshDatabaseAsync(db);
            }           
        }

        #region TitelBar

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public Fluent.RibbonTitleBar TitleBar
        {
            get { return (Fluent.RibbonTitleBar)this.GetValue(TitleBarProperty); }
            private set { this.SetValue(TitleBarPropertyKey, value); }
        }


        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey TitleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(Fluent.RibbonTitleBar), typeof(MainWindow), new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

        #endregion
    }
}
