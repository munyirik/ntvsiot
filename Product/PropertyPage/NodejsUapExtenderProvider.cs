using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.NodejsUap
{
    class NodejsUapExtenderProvider : EnvDTE.IExtenderProvider
    {
        static public string Name = "NodejsUapExtender";
        bool EnvDTE.IExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject)
        {
            //EnvDTE80.IInternalExtenderProvider outerHierarchy = Node.GetOuterInterface<EnvDTE80.IInternalExtenderProvider>();

            //if (outerHierarchy != null)
            //{
            //    return outerHierarchy.CanExtend(extenderCATID, extenderName, extendeeObject);
            //}
            return false;
        }

        object EnvDTE.IExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject, EnvDTE.IExtenderSite extenderSite, int cookie)
        {
            //EnvDTE80.IInternalExtenderProvider outerHierarchy = Node.GetOuterInterface<EnvDTE80.IInternalExtenderProvider>();

            //if (outerHierarchy != null)
            //{
            //    return outerHierarchy.GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
            //}

            return null;
        }
    }
}
