using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IL2CDR.Model
{
    [Serializable]
    public enum FieldType
    {
        Unknown = -1,
        Text = 0,
        File = 1,
        Folder = 2,
        Number = 3,
        Range = 4,
    }
    [Serializable]
    public class ConfigField : NotifyPropertyChangeBase 
    {
        public ConfigField()
        {

        }
        public ConfigField(string name, string label, FieldType dataType, bool isVisible, object value)
        {
            Name = name;
            Label = label;
            Type = dataType;
            IsVisible = isVisible;
            Value = value;
        }

        /// <summary>
        /// The <see cref="Type" /> property's name.
        /// </summary>
        public const string TypePropertyName = "Type";

        private FieldType _type = FieldType.Unknown;

        /// <summary>
        /// Sets and gets the Type property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public FieldType Type
        {
            get
            {
                return _type;
            }

            set
            {
                if (_type == value)
                {
                    return;
                }

                _type = value;
                RaisePropertyChanged(TypePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsVisible" /> property's name.
        /// </summary>
        public const string IsVisiblePropertyName = "IsVisible";

        private bool _isVisible = true;

        /// <summary>
        /// Sets and gets the IsVisible property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;
                RaisePropertyChanged(IsVisiblePropertyName);
            }
        }
        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        private object _value = null;

        /// <summary>
        /// Sets and gets the Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlElement]
        public object Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Label" /> property's name.
        /// </summary>
        public const string LabelPropertyName = "Label";

        private string _label = null;

        /// <summary>
        /// Sets and gets the Label property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public string Label
        {
            get
            {
                return _label;
            }

            set
            {
                if (_label == value)
                {
                    return;
                }

                _label = value;
                RaisePropertyChanged(LabelPropertyName);
            }
        }
        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = null;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(NamePropertyName);
            }
        }
    }
}
