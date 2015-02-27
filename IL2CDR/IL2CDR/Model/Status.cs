using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    public enum StatusType
    {
        OK,
        Warning,
        Error,
    }
    public class Status : NotifyPropertyChangeBase
    {
        /// <summary>
        /// The <see cref="StatusName" /> property's name.
        /// </summary>
        public const string StatusNamePropertyName = "StatusName";

        private string _statusName = null;

        /// <summary>
        /// Sets and gets the StatusName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string StatusName
        {
            get
            {
                return _statusName;
            }

            set
            {
                if (_statusName == value)
                {
                    return;
                }

                _statusName = value;
                RaisePropertyChanged(StatusNamePropertyName);
            }
        }
        /// <summary>
        /// The <see cref="Type" /> property's name.
        /// </summary>
        public const string TypePropertyName = "Type";

        private StatusType _type = StatusType.Error;

        /// <summary>
        /// Sets and gets the Type property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public StatusType Type
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
        /// The <see cref="ErrorDescription" /> property's name.
        /// </summary>
        public const string ErrorDescriptionPropertyName = "ErrorDescription";

        private string _errorDescription = null;

        /// <summary>
        /// Sets and gets the ErrorDescription property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string ErrorDescription
        {
            get
            {
                return _errorDescription;
            }

            set
            {
                if (_errorDescription == value)
                {
                    return;
                }

                _errorDescription = value;
                RaisePropertyChanged(ErrorDescriptionPropertyName);
            }
        }
        /// <summary>
        /// The <see cref="Title" /> property's name.
        /// </summary>
        public const string TitlePropertyName = "Title";

        private string _title = "Status title";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
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
        
    }
}
