using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Microsoft.NodejsUwp
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class NodejsUwpPropertyExtender : EnvDTE.IFilterProperties
    {
        private readonly EnvDTE.IExtenderSite extenderSite;
        private readonly int cookie;

        public NodejsUwpPropertyExtender(EnvDTE.IExtenderSite extenderSite, int cookie)
        {
            this.extenderSite = extenderSite;
            this.cookie = cookie;
        }

        public EnvDTE.vsFilterProperties IsPropertyHidden(string PropertyName)
        {
            // Work to filter out the properties is done here.
            return EnvDTE.vsFilterProperties.vsFilterPropertiesAll;
        }

        ~NodejsUwpPropertyExtender()
        {
            this.extenderSite.NotifyDelete(this.cookie);
        }
    }
}