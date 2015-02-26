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
        const string SettingsRootName = "Settings";
        const string RoamingSettingsRootName = "Roaming";
        const string LocalSettingsRootName = "Local";
        const string ConfigNamespace = "IL2CDR";

        readonly string FileName;
        readonly Lazy<XDocument> SettingsXml;

        private String applicationPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

        public readonly string DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\IL2CDR"; // Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);
        public const string DefaultSettingsName = "il2cdr";
        public const string DefaultFileName = DefaultSettingsName + ".settings";

        public PortableSettingsProvider()
            : this(DefaultFileName)
        {
        }

        public PortableSettingsProvider(string settingsFileName)
        {
            string settingsDirectory = DefaultDirectory;
            Directory.CreateDirectory(settingsDirectory);

            FileName = Path.Combine(settingsDirectory, settingsFileName);
            SettingsXml = new Lazy<XDocument>(() => LoadOrCreateSettings(FileName), LazyThreadSafetyMode.PublicationOnly);
        }

        public override void Initialize(string name, NameValueCollection collection)
        {
            base.Initialize(this.ApplicationName, collection);
        }

        public override string ApplicationName
        {
            get { return Path.GetFileNameWithoutExtension(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath); }
            set { }
        }

        public override string Name
        {
            get { return GetType().Name; }
        }
        
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection properties)
        {
            foreach (SettingsPropertyValue propertyValue in properties)
            {
                SetValue(propertyValue);
            }
            try
            {
                SettingsXml.Value.Save(FileName);
            }
            catch (Exception ex)
            {
                Log.WriteError("{0} {1} {2}", "Settings save error: ", ex.Message, FileName);
            }  
        }

        public override SettingsPropertyValueCollection GetPropertyValues(
             SettingsContext context, SettingsPropertyCollection props)
        {
            var values = new SettingsPropertyValueCollection();
            foreach (SettingsProperty setting in props)
            {
                values.Add(new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    SerializedValue = GetValue(setting),
                });
            }
            return values;
        }

        XElement SettingsRoot
        {
            get { return SettingsXml.Value.Root; }
        }

        object GetValue(SettingsProperty setting)
        {
            var propertyPath = IsRoaming(setting)
               ? string.Concat("./", RoamingSettingsRootName, "/", setting.Name)
               : string.Concat("./", LocalSettingsRootName, "/", Environment.MachineName, "/", setting.Name);

            var propertyElement = SettingsRoot.XPathSelectElement(propertyPath);

            if (setting.PropertyType.FullName.Contains(ConfigNamespace))
            {                

                return propertyElement == null ? null : propertyElement.Nodes().Aggregate("", (b, node) => b += node.ToString());
            }
            else
            {
                return propertyElement == null ? setting.DefaultValue : propertyElement.Value;
            }

        }
        void SetValue(SettingsPropertyValue setting)
        {
            var parentElement = IsRoaming(setting.Property)
              ? SettingsRoot.GetOrAddElement(RoamingSettingsRootName)
              : SettingsRoot.GetOrAddElement(LocalSettingsRootName)
                      .GetOrAddElement(Environment.MachineName);

            if (setting.Property.PropertyType.FullName.Contains(ConfigNamespace) && setting.SerializedValue != null )
            {                
                parentElement.GetOrAddElement(setting.Name).ReplaceWith(new XElement(setting.Name, XElement.Parse(setting.SerializedValue.ToString())));
            }
            else
                parentElement.GetOrAddElement(setting.Name).Value = setting.SerializedValue == null ? String.Empty : setting.SerializedValue.ToString();
        }

        static XDocument LoadOrCreateSettings(string filePath)
        {
            XDocument settingsXml = null;
            try
            {
                settingsXml = XDocument.Load(filePath);

                if (settingsXml.Root.Name.LocalName != SettingsRootName)
                {
                    Log.WriteError("{0}", "Invalid settings format");

                    settingsXml = null;
                }
            }
            catch (Exception ex)
            {
                Log.WriteError("{0}", "Invalid settings file {0} {1}",filePath,ex.Message);
            }

            return settingsXml ??
              new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(SettingsRootName, string.Empty)
              );
        }

        static bool IsRoaming(SettingsProperty property)
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
            if (element == null)
            {
                element = new XElement(name);
                parent.Add(element);
            }
            return element;
        }
    }

}
