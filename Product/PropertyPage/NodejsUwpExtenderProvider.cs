using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.NodejsUwp
{
    /// <summary>
    /// This extender will hide the properties of the base NTVS project in properties window.
    /// TODO: Show Node.js UWP properties in Property Window (currently only accessible in project properties - Project Menu->Properties).
    /// </summary>
    public class NodejsUwpExtenderProvider : EnvDTE.IExtenderProvider
    {
        static public string uwpExtenderName = "NodejsUwpExtender";
        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            return ExtenderName == uwpExtenderName && string.Equals(ExtenderCATID, GuidList.guidPropertyExtenderCATID.ToString("B"), StringComparison.OrdinalIgnoreCase);
        }

        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, EnvDTE.IExtenderSite ExtenderSite, int Cookie)
        {
            if (CanExtend(ExtenderCATID, ExtenderName, ExtendeeObject))
            {
                return new NodejsUwpPropertyExtender(ExtenderSite, Cookie);
            }
            return null;
        }
    }
}