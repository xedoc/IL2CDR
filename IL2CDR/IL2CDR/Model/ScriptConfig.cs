using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IL2CDR.Model
{
    [Serializable]
    public class ScriptConfig : NotifyPropertyChangeBase
    {
        public ScriptConfig()
        {
            ConfigFields = new ConfigFieldList();
        }

        public string GetString(string name)
        {
            return ConfigFields.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).With(x => x.Value as string);
        }
        public int GetInt(string name)
        {
            int intValue = 0;
            var value = ConfigFields.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).With( x => x.Value).With( x => x.ToString());
            int.TryParse(value, out intValue);
            return intValue;
        }

        /// <summary>
        /// The <see cref="FileName" /> property's name.
        /// </summary>
        public const string FileNamePropertyName = "FileName";

        private string _fileName = null;

        /// <summary>
        /// Sets and gets the FileName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                if (_fileName == value)
                {
                    return;
                }

                _fileName = value;
                RaisePropertyChanged(FileNamePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="ConfigFields" /> property's name.
        /// </summary>
        public const string ConfigFieldsPropertyName = "ConfigFields";

        private ConfigFieldList _configFields = null;

        /// <summary>
        /// Sets and gets the Parameters property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlArray]
        public ConfigFieldList ConfigFields
        {
            get
            {
                return _configFields;
            }

            set
            {
                if (_configFields == value)
                {
                    return;
                }

                _configFields = value;
                RaisePropertyChanged(ConfigFieldsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _title = null;

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                RaisePropertyChanged(TitlePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Description" /> property's name.
        /// </summary>
        public const string DescriptionPropertyName = "Description";

        private string _description = null;

        /// <summary>
        /// Sets and gets the Description property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                if (_description == value)
                {
                    return;
                }

                _description = value;
                RaisePropertyChanged(DescriptionPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsEnabled" /> property's name.
        /// </summary>
        public const string IsEnabledPropertyName = "IsEnabled";

        private bool _isEnabled = true;

        /// <summary>
        /// Sets and gets the IsEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                _isEnabled = value;
                RaisePropertyChanged(IsEnabledPropertyName);
            }
        }
    }
}
