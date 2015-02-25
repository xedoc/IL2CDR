using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IL2CDR.Model
{
    [Serializable]
    public class Config:NotifyPropertyChangeBase
    {
        public Config()
        {

        }

        /// <summary>
        /// The <see cref="MissonLogFolder" /> property's name.
        /// </summary>
        public const string MissonLogFolderPropertyName = "MissonLogFolder";

        private string _missionLogFilder = null;

        /// <summary>
        /// Sets and gets the MissonLogFolder property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string MissonLogFolder
        {
            get
            {
                return _missionLogFilder;
            }

            set
            {
                if (_missionLogFilder == value)
                {
                    return;
                }

                _missionLogFilder = value;
                RaisePropertyChanged(MissonLogFolderPropertyName);
            }
        }
        /// <summary>
        /// The <see cref="LastMissionLogFile" /> property's name.
        /// </summary>
        public const string LastMissionLogFilePropertyName = "LastMissionLogFile";

        private string _lastMissionLogFile = null;

        /// <summary>
        /// Sets and gets the LastMissionLogFile property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string LastMissionLogFile
        {
            get
            {
                return _lastMissionLogFile;
            }

            set
            {
                if (_lastMissionLogFile == value)
                {
                    return;
                }

                _lastMissionLogFile = value;
                RaisePropertyChanged(LastMissionLogFilePropertyName);
            }
        }
    }
}
