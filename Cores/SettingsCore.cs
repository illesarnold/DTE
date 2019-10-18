using DTE.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.Cores
{
    public class SettingsCore : DataBindingBase45
    {
        private string fileName = "settings";
        public SettingsCore()
        {
            SettingsDeserialize();
        }
        Settings _settings = new Settings();
        public Settings Settings
        {
            get
            {
                return _settings;
            }

            set
            {
                _settings = value;
                OnPropertyChanged();
            }
        }
        public void SettingsDeserialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            if (File.Exists(fileName))
            {
                StreamReader reader = new StreamReader(fileName);
                Settings = (Settings)serializer.Deserialize(reader);
                reader.Close();
            }

        }
        public void SettingsSerialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            FileStream fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, Settings);
            fs.Close();
        }
    }
}
