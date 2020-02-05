using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace DigialMaketingLauncher
{
    public class ActionsSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            var myConfigObject = new List<ActionParam>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var action = new ActionParam();
                foreach (XmlAttribute attrib in childNode.Attributes)
                {
                    PropertyInfo prop = typeof(ActionParam).GetProperty(attrib.Name);
                    prop.SetValue(action, Convert.ChangeType(attrib.Value, prop.PropertyType), null);
                }
                myConfigObject.Add(action);

            }
            return myConfigObject;
        }
    }

    public class ActionParam
    {
        public string Url { get; set; }
        public int ExecHour { get; set; }
        public int ExecMinute { get; set; }
    }
}