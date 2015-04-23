using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Microsoft.NodejsUap
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("B3704CC1-DB51-4929-A7AD-DDBFF02D5E08")]
    class NodejsUapPropertyExtender : EnvDTE.IFilterProperties, INodejsUapExtender
    {
        private string _prop;
        public EnvDTE.vsFilterProperties IsPropertyHidden(string PropertyName)
        {
            return EnvDTE.vsFilterProperties.vsFilterPropertiesAll;
        }

        [DefaultValue(false)]
        [DisplayName("Hello World Prop")]
        [Category("Test")]
        [Description("Please work!")]
        public string HelloWorldProp
        {
            get
            {
                return _prop;
            }

            set
            {
                _prop = value;
            }
        }
    }
}
