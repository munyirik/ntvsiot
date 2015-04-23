using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EnvDTE;
using System.ComponentModel;

namespace Microsoft.NodejsUap
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface INodejsUapExtender
    {
        [DefaultValue(false)]
        [DisplayName("Exclude StyleCop")]
        [Category("StyleCop")]
        [Description("Specifies that the file is exculded from the StyleCop source analysis.")]
        string HelloWorldProp { get; set; }
    }
}
