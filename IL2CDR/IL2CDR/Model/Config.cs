using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IL2CDR.Properties;

namespace IL2CDR.Model
{
    [Serializable]
    public class Config : NotifyPropertyChangeBase
    {
        public Config()
        {
            ScriptConfigs = new List<ScriptConfig>();
        }

        /// <summary>
        /// The <see cref="ScriptConfigs" /> property's name.
        /// </summary>
        public const string ScriptConfigsPropertyName = "ScriptConfigs";

        private List<ScriptConfig> _scriptConfigs = null;

        /// <summary>
        /// Sets and gets the ScriptConfigs property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlArray]
        public List<ScriptConfig> ScriptConfigs
        {
            get
            {
                return _scriptConfigs;
            }

            set
            {
                if (_scriptConfigs == value)
                {
                    return;
                }

                _scriptConfigs = value;
                RaisePropertyChanged(ScriptConfigsPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsChatMonitorEnabled" /> property's name.
        /// </summary>
        public const string IsChatMonitorEnabledPropertyName = "IsChatMonitorEnabled";

        private bool _isChatMonitorEnabled = true;

        /// <summary>
        /// Sets and gets the IsChatMonitorEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public bool IsChatMonitorEnabled
        {
            get
            {
                return _isChatMonitorEnabled;
            }

            set
            {
                if (_isChatMonitorEnabled == value)
                {
                    return;
                }

                _isChatMonitorEnabled = value;
                RaisePropertyChanged(IsChatMonitorEnabledPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="MissionLogCleanupInterval" /> property's name.
        /// </summary>
        public const string MissionLogCleanupIntervalPropertyName = "MissionLogCleanupInterval";

        private int _missionLogCleanupInterval = 5;

        /// <summary>
        /// Sets and gets the MissionLogCleanupInterval property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public int MissionLogCleanupInterval
        {
            get
            {
                return _missionLogCleanupInterval;
            }

            set
            {
                if (_missionLogCleanupInterval == value)
                {
                    return;
                }

                _missionLogCleanupInterval = value;
                RaisePropertyChanged(MissionLogCleanupIntervalPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsMissionLogCleanupEnabled" /> property's name.
        /// </summary>
        public const string IsMissionLogCleanupEnabledPropertyName = "IsMissionLogCleanupEnabled";

        private bool _isMissionLogCleanupEnabled = false;

        /// <summary>
        /// Sets and gets the IsMissionLogCleanupEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public bool IsMissionLogCleanupEnabled
        {
            get
            {
                return _isMissionLogCleanupEnabled;
            }

            set
            {
                if (_isMissionLogCleanupEnabled == value)
                {
                    return;
                }

                _isMissionLogCleanupEnabled = value;
                RaisePropertyChanged(IsMissionLogCleanupEnabledPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="IsMissionLogMonitorEnabled" /> property's name.
        /// </summary>
        public const string IsMissionLogMonitorEnabledPropertyName = "IsMissionLogMonitorEnabled";

        private bool _isMissionLogMonitorEnabled = true;

        /// <summary>
        /// Sets and gets the IsMissionLogMonitorEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        [XmlAttribute]
        public bool IsMissionLogMonitorEnabled
        {
            get
            {
                return _isMissionLogMonitorEnabled;
            }

            set
            {
                if (_isMissionLogMonitorEnabled == value)
                {
                    return;
                }

                _isMissionLogMonitorEnabled = value;
                RaisePropertyChanged(IsMissionLogMonitorEnabledPropertyName);
            }
        }
    }
}
