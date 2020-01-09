using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace IL2CDR.Model
{
	[Serializable]
	public enum FieldType
	{
		Unknown = -1,
		Text = 0,
		FileOpen = 1,
		FileSave = 2,
		Folder = 3,
		Number = 4,
		Range = 5,
		Flag = 6,
		Password = 7,
		Button = 8
	}

	[Serializable]
	public class ConfigField : NotifyPropertyChangeBase
	{
		public ConfigField()
		{
		}

		public ConfigField(string name, string label, string watermark, FieldType dataType, bool isVisible,
			object value)
		{
			this.Name = name;
			this.Label = label;
			this.Watermark = watermark;
			this.Type = dataType;
			this.IsVisible = isVisible;
			this.Value = value;
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
			get => this._type;

			set
			{
				if (this._type == value) {
					return;
				}

				this._type = value;
				this.RaisePropertyChanged(TypePropertyName);
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
			get => this._isVisible;

			set
			{
				if (this._isVisible == value) {
					return;
				}

				this._isVisible = value;
				this.RaisePropertyChanged(IsVisiblePropertyName);
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
			get => this._value;

			set
			{
				if (this._value == value) {
					return;
				}

				this._value = value;
				this.RaisePropertyChanged(ValuePropertyName);
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
			get => this._label;

			set
			{
				if (this._label == value) {
					return;
				}

				this._label = value;
				this.RaisePropertyChanged(LabelPropertyName);
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
			get => this._name;

			set
			{
				if (this._name == value) {
					return;
				}

				this._name = value;
				this.RaisePropertyChanged(NamePropertyName);
			}
		}

		/// <summary>
		/// The <see cref="Watermark" /> property's name.
		/// </summary>
		public const string WatermarkPropertyName = "Watermark";

		private string _watermark = null;

		/// <summary>
		/// Sets and gets the Watermark property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string Watermark
		{
			get => this._watermark;

			set
			{
				if (this._watermark == value) {
					return;
				}

				this._watermark = value;
				this.RaisePropertyChanged(WatermarkPropertyName);
			}
		}
	}

	[Serializable]
	public class ConfigFieldList : List<ConfigField>
	{
		public void Add(string name, string label, string watermark, FieldType dataType, object value, bool isVisible)
		{
			this.Add(new ConfigField(name, label, watermark, dataType, isVisible, value));
		}

		public void Set(string name, object value)
		{
			var found = this.FirstOrDefault(o => o.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			if (found != null) {
				found.Value = value;
			}
		}
	}
}