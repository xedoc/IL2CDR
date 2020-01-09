using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using IL2CDR.Model;

namespace IL2CDR
{
	public class CustomSettings : ApplicationSettingsBase
	{
	}

	public class PortableSettingsProvider : SettingsProvider
	{
		private const string SettingsRootName = "Settings";
		private const string RoamingSettingsRootName = "Roaming";
		private const string LocalSettingsRootName = "Local";
		private const string ConfigNamespace = "IL2CDR";

		private readonly string FileName;
		private readonly Lazy<XDocument> SettingsXml;

		private string applicationPath =
			Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

		public readonly string DefaultDirectory =
			Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
			@"\IL2CDR"; // Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

		public const string DefaultSettingsName = "il2cdr";
		public const string DefaultFileName = DefaultSettingsName + ".settings";

		public PortableSettingsProvider()
			: this(DefaultFileName)
		{
		}

		public PortableSettingsProvider(string settingsFileName)
		{
			var settingsDirectory = this.DefaultDirectory;
			Directory.CreateDirectory(settingsDirectory);

			this.FileName = Path.Combine(settingsDirectory, settingsFileName);
			this.SettingsXml = new Lazy<XDocument>(() => LoadOrCreateSettings(this.FileName),
				LazyThreadSafetyMode.PublicationOnly);
		}

		public override void Initialize(string name, NameValueCollection collection)
		{
			base.Initialize(this.ApplicationName, collection);
		}

		public override string ApplicationName
		{
			get => Path.GetFileNameWithoutExtension(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase)
				.LocalPath);
			set { }
		}

		public override string Name => this.GetType().Name;

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection properties)
		{
			foreach (SettingsPropertyValue propertyValue in properties) {
				this.SetValue(propertyValue);
			}

			try {
				this.SettingsXml.Value.Save(this.FileName);
			} catch (Exception ex) {
				Log.WriteError("{0} {1} {2}", "Settings save error: ", ex.Message, this.FileName);
			}
		}

		public override SettingsPropertyValueCollection GetPropertyValues(
			SettingsContext context, SettingsPropertyCollection props)
		{
			var values = new SettingsPropertyValueCollection();
			foreach (SettingsProperty setting in props) {
				values.Add(new SettingsPropertyValue(setting) {
					IsDirty = false,
					SerializedValue = this.GetValue(setting),
				});
			}

			return values;
		}

		private XElement SettingsRoot => this.SettingsXml.Value.Root;

		private object GetValue(SettingsProperty setting)
		{
			var propertyPath = IsRoaming(setting)
				? string.Concat("./", RoamingSettingsRootName, "/", setting.Name)
				: string.Concat("./", LocalSettingsRootName, "/", Environment.MachineName, "/", setting.Name);

			var propertyElement = this.SettingsRoot.XPathSelectElement(propertyPath);

			if (setting.PropertyType.FullName.Contains(ConfigNamespace)) {
				return propertyElement == null
					? null
					: propertyElement.Nodes().Aggregate("", (b, node) => b += node.ToString());
			} else {
				return propertyElement == null ? setting.DefaultValue : propertyElement.Value;
			}
		}

		private void SetValue(SettingsPropertyValue setting)
		{
			var parentElement = IsRoaming(setting.Property)
				? this.SettingsRoot.GetOrAddElement(RoamingSettingsRootName)
				: this.SettingsRoot.GetOrAddElement(LocalSettingsRootName)
					.GetOrAddElement(Environment.MachineName);

			if (setting.Property.PropertyType.FullName.Contains(ConfigNamespace) && setting.SerializedValue != null) {
				parentElement.GetOrAddElement(setting.Name).ReplaceWith(new XElement(setting.Name,
					XElement.Parse(setting.SerializedValue.ToString())));
			} else {
				parentElement.GetOrAddElement(setting.Name).Value = setting.SerializedValue == null
					? string.Empty
					: setting.SerializedValue.ToString();
			}
		}

		private static XDocument LoadOrCreateSettings(string filePath)
		{
			XDocument settingsXml = null;
			try {
				settingsXml = XDocument.Load(filePath);

				if (settingsXml.Root.Name.LocalName != SettingsRootName) {
					Log.WriteError("{0}", "Invalid settings format");

					settingsXml = null;
				}
			} catch (Exception ex) {
				if (ex is FileNotFoundException) {
					Log.WriteInfo("Initializing application settings with default values...");
				} else {
					Log.WriteError("Error opening configuration file {0}: {1}", filePath, ex.Message);
				}
			}

			return settingsXml ??
					new XDocument(
						new XDeclaration("1.0", "utf-8", "yes"),
						new XElement(SettingsRootName, string.Empty)
					);
		}

		private static bool IsRoaming(SettingsProperty property)
		{
			return property.Attributes
				.Cast<DictionaryEntry>()
				.Any(a => a.Value is SettingsManageabilityAttribute);
		}
	}

	public static class XExtensions
	{
		public static XElement GetOrAddElement(this XContainer parent, XName name)
		{
			var element = parent.Element(name);
			if (element == null) {
				element = new XElement(name);
				parent.Add(element);
			}

			return element;
		}
	}
}