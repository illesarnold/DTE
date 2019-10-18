using DTE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.Cores
{
    public class ConnectionCore : DataBindingBase45
    {

        private string fileName = "connections";
        public ConnectionCore()
        {
            ConnectionDeserialize();
        }

        public void ConnectionDeserialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<TreeViewModel>));

            if (File.Exists(fileName))
            {
                StreamReader reader = new StreamReader(fileName);
                Connections = (ObservableCollection<TreeViewModel>)serializer.Deserialize(reader);
                reader.Close();
            }

        }
        public void ConnectionSerialize()
        {

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<TreeViewModel>));
            FileStream fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, Connections);
            fs.Close();
        }

        ObservableCollection<Models.TreeViewModel> _connections = new ObservableCollection<TreeViewModel>();
        public ObservableCollection<Models.TreeViewModel> Connections
        {
            get
            {
                return _connections;
            }

            set
            {
                _connections = value;
                OnPropertyChanged();
            }
        }

    
    }
}
