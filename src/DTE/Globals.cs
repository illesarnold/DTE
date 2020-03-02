using DTE.CORE;
using DTE.Cores;
using DTE.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTE
{
    public class Globals
    {
        private static string _lastSelectedPath;
        public static List<KeyValuePair<SupportedConnectionsTypes,string>> ConnTypes = new List<KeyValuePair<SupportedConnectionsTypes, string>>()
        {
            new KeyValuePair<SupportedConnectionsTypes,string>(SupportedConnectionsTypes.MySQL,"MySql"),
            new KeyValuePair<SupportedConnectionsTypes,string>(SupportedConnectionsTypes.MSSQL,"MsSql"),
            new KeyValuePair<SupportedConnectionsTypes,string>(SupportedConnectionsTypes.PostgreSQL,"Postgre SQL")                   
        };
        public static DTECore CreateCoreByConnection(ConnectionBuilderBase builder)
        {
            return new DTECore(builder.ConnectionType, builder.ConnectionString, new DTESettings().Settings);
        }

        public static string SelectFolderPath()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = _lastSelectedPath;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastSelectedPath = dialog.SelectedPath;
                return dialog.SelectedPath; 
            }


            return null;
        }
        public static string SelectProjectPath()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.InitialDirectory = _lastSelectedPath;
            dialog.Filter = "project | *.csproj";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _lastSelectedPath = dialog.FileName;
                return dialog.FileName;
            }

            return null;
        }
    }
}
