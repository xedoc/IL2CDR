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
				this.RaisePropertyChanged(nameof(this.ScriptConfigs));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.IsChatMonitorEnabled));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.MissionLogCleanupInterval));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.IsMissionLogCleanupEnabled));
			}
		}


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
				this.RaisePropertyChanged(nameof(this.IsMissionLogMonitorEnabled));
			}
		}


		private bool _isRConEnabled = true;

		/// <summary>
		/// Sets and gets the IsRConEnabled property -- whether this application should connect to the DServer via RCon. 
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[XmlAttribute]
		public bool IsRConEnabled
		{
			get => this._isRConEnabled;

			set
			{
				if (this._isRConEnabled == value) {
					return;
				}

				this._isRConEnabled = value;
				this.RaisePropertyChanged(nameof(this.IsRConEnabled));
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