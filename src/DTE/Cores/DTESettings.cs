using DTE.Domains;
using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DTE.Cores
{
    public class DTESettings : DataBindingBase45
    {
        private const string ProperyTemplatPath = @"Templates\Property.tpl";
        private const string FullPropertyTemplatePath = @"Templates\FullProperty.tpl";
        private const string ClassPath = @"Templates\Class.tpl";
        private string fileName = "settings";
        public DTESettings()
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
            _settings.PropTemplate = File.ReadAllText(ProperyTemplatPath);
            _settings.FullPropTemplate = File.ReadAllText(FullPropertyTemplatePath);
            _settings.ClassTemplate = File.ReadAllText(ClassPath);
        }
        public void SettingsSerialize()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            FileStream fs = new FileStream(fileName, FileMode.Create);
            serializer.Serialize(fs, Settings);
            fs.Close();

            File.WriteAllText(ProperyTemplatPath, _settings.PropTemplate);
            File.WriteAllText(FullPropertyTemplatePath, _settings.FullPropTemplate);
            File.WriteAllText(ClassPath, _settings.ClassTemplate);
        }
    }
}
