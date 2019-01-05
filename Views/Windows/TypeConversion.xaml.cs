using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using DTE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DTE.Views.Windows
{
    /// <summary>
    /// Interaction logic for TypeConversion.xaml
    /// </summary>
    public partial class TypeConversion : MetroWindow
    {
        TypeConversionVM vm = new TypeConversionVM(DialogCoordinator.Instance);

        public TypeConversion()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
