using System;
using System.Linq;
using System.Xml.Serialization;

namespace IL2CDR.Model
{
	[Serializable]
	public class ScriptConfig : NotifyPropertyChangeBase
	{
		public ScriptConfig()
		{
			this.ConfigFields = new ConfigFieldList();
		}

		public string GetString(string name)
		{
			return this.ConfigFields
				.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				.With(x => x.Value as string);
		}

		public int GetInt(string name)
		{
			var value = this.ConfigFields
				.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				.With(x => x.Value).With(x => x.ToString());
			int.TryParse(value, out var intValue);
			return intValue;
		}

		public bool GetBool(string name)
		{
			return (bool) this.ConfigFields
				.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				.With(x => x.Value);
		}

		/// <summary>
		/// The <see cref="FileName" /> property's name.
		/// </summary>
		public const string FILE_NAME_PROPERTY_NAME = "FileName";

		private string _fileName = null;

		/// <summary>
		/// Sets and gets the FileName property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[XmlAttribute]
		public string FileName
		{
			get => this._fileName;

			set
			{
				if (this._fileName == value) {
					return;
				}

				this._fileName = value;
				this.RaisePropertyChanged(FILE_NAME_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="ConfigFields" /> property's name.
		/// </summary>
		public const string CONFIG_FIELDS_PROPERTY_NAME = "ConfigFields";

		private ConfigFieldList _configFields = null;

		/// <summary>
		/// Sets and gets the Parameters property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[XmlArray]
		public ConfigFieldList ConfigFields
		{
			get => this._configFields;

			set
			{
				if (this._configFields == value) {
					return;
				}

				this._configFields = value;
				this.RaisePropertyChanged(CONFIG_FIELDS_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="Title" /> property's name.
		/// </summary>
		public const string TITLE_PROPERTY_NAME = "Title";

		private string _title = null;

		/// <summary>
		/// Sets and gets the Title property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[XmlAttribute]
		public string Title
		{
			get => this._title;

			set
			{
				if (this._title == value) {
					return;
				}

				this._title = value;
				this.RaisePropertyChanged(TITLE_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="Description" /> property's name.
		/// </summary>
		public const string DESCRIPTION_PROPERTY_NAME = "Description";

		private string _description = null;

		/// <summary>
		/// Sets and gets the Description property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[XmlAttribute]
		public string Description
		{
			get => this._description;

			set
			{
				if (this._description == value) {
					return;
				}

				this._description = value;
				this.RaisePropertyChanged(DESCRIPTION_PROPERTY_NAME);
			}
		}

		/// <summary>
		/// The <see cref="IsEnabled" /> property's name.
		/// </summary>
		public const string IS_ENABLED_PROPERTY_NAME = "IsEnabled";

		private bool _isEnabled = true;

		/// <summary>
		/// Sets and gets the IsEnabled property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		[XmlAttribute]
		public bool IsEnabled
		{
			get => this._isEnabled;

			set
			{
				if (this._isEnabled == value) {
					return;
				}

				this._isEnabled = value;
				this.RaisePropertyChanged(IS_ENABLED_PROPERTY_NAME);
			}
		}
	}
}