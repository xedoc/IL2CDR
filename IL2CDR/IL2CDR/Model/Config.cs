using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using IL2CDR.Properties;

namespace IL2CDR.Model
{
	[Serializable]
	public class Config : NotifyPropertyChangeBase
	{
		public Config()
		{
			this.ScriptConfigs = new List<ScriptConfig>();
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
			get => this._scriptConfigs;

			set
			{
				if (this._scriptConfigs == value) {
					return;
				}

				this._scriptConfigs = value;
				this.RaisePropertyChanged(ScriptConfigsPropertyName);
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
			get => this._isChatMonitorEnabled;

			set
			{
				if (this._isChatMonitorEnabled == value) {
					return;
				}

				this._isChatMonitorEnabled = value;
				this.RaisePropertyChanged(IsChatMonitorEnabledPropertyName);
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
			get => this._missionLogCleanupInterval;

			set
			{
				if (this._missionLogCleanupInterval == value) {
					return;
				}

				this._missionLogCleanupInterval = value;
				this.RaisePropertyChanged(MissionLogCleanupIntervalPropertyName);
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
			get => this._isMissionLogCleanupEnabled;

			set
			{
				if (this._isMissionLogCleanupEnabled == value) {
					return;
				}

				this._isMissionLogCleanupEnabled = value;
				this.RaisePropertyChanged(IsMissionLogCleanupEnabledPropertyName);
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
			get => this._isMissionLogMonitorEnabled;

			set
			{
				if (this._isMissionLogMonitorEnabled == value) {
					return;
				}

				this._isMissionLogMonitorEnabled = value;
				this.RaisePropertyChanged(IsMissionLogMonitorEnabledPropertyName);
			}
		}


		private int _applicationLogBufferSize;

		[XmlAttribute]
		public int ApplicationLogBufferSize {
			get => this._applicationLogBufferSize;

			set {
				if (this._applicationLogBufferSize == value) {
					return;
				}

				this._applicationLogBufferSize = value;
				this.RaisePropertyChanged(nameof(ApplicationLogBufferSize));
			}
		}



	}
}