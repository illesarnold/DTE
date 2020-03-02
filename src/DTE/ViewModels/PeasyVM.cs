using DTE.CORE;
using DTE.Domains;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTE.ViewModels
{
    public class PeasyVM : DataBindingBase45
    {
        private string projectName;
        private string businessProjectPath;
        private string dataLayerProjectPath;
        private bool isLoading;
        private int _allTablesCount = 1;
        private ObservableCollection<Table> _tables = new ObservableCollection<Table>();
        private readonly DTECore dTECore;

        public PeasyVM()
        {

        }
        public PeasyVM(List<Table> tables, DTECore dTECore)
        {
            Tables = new ObservableCollection<Table>(tables);
            AllTablesCount = Tables.Count;
            this.dTECore = dTECore;
        }

        public string ProjectName { get => projectName; set { projectName = value; OnPropertyChanged(); } }
        public string BusinessProjectPath { get => businessProjectPath; set { businessProjectPath = value; OnPropertyChanged(); } }
        public string DataLayerProjectPath { get => dataLayerProjectPath; set { dataLayerProjectPath = value; OnPropertyChanged(); } }
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(); } }
        public ObservableCollection<Table> Tables { get => _tables; set { _tables = value; OnPropertyChanged(); } }
        public int AllTablesCount { get => _allTablesCount; set { _allTablesCount = value; OnPropertyChanged(); } }

        public RelayCommand SelectBusinessProjectPathCommand
        {
            get
            {
                return new RelayCommand(_ => SelectBusinessProjectPath());
            }
        }
        public RelayCommand SelectDataLayerProjectPathCommand
        {
            get
            {
                return new RelayCommand(_ => SelectDataLayerProjectPath());
            }
        }
        public RelayCommand GeneratePeasyCommand
        {
            get
            {
                return new RelayCommand(_ => GeneratePeasyAsync());
            }
        }

        private void SelectBusinessProjectPath()
        {
            BusinessProjectPath = Globals.SelectProjectPath() ?? BusinessProjectPath;
        }
        private void SelectDataLayerProjectPath()
        {
            DataLayerProjectPath = Globals.SelectProjectPath() ?? DataLayerProjectPath;
        }
        private async void GeneratePeasyAsync()
        {
            isLoading = true;

            try
            {
                var peasyGenerator = new Architecture.Peasy(projectName, businessProjectPath, dataLayerProjectPath, Tables, dTECore);
                await peasyGenerator.GeneratePeasyArchAsync();
                MessageBox.Show("Generation Completed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            isLoading = false;
        }

    }
}
