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
            Parameters = new List<ConfigField>();
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
        /// The <see cref="Parameters" /> property's name.
        /// </summary>
        public const string ParametersPropertyName = "Parameters";

        private List<ConfigField> _parameters = null;

        /// <summary>
        /// Sets and gets the Parameters property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlArray]
        public List<ConfigField> Parameters
        {
            get
            {
                return _parameters;
            }

            set
            {
                if (_parameters == value)
                {
                    return;
                }

                _parameters = value;
                RaisePropertyChanged(ParametersPropertyName);
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
        /// The <see cref="Enabled" /> property's name.
        /// </summary>
        public const string EnabledPropertyName = "Enabled";

        private bool _enabled = false;

        /// <summary>
        /// Sets and gets the Enabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                if (_enabled == value)
                {
                    return;
                }

                _enabled = value;
                RaisePropertyChanged(EnabledPropertyName);
            }
        }
    }
}
