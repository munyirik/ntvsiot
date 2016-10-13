using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUtilities.NodejsUwp {
    public class TargetInfo {
        public string IP { get; set; }

        public string Plat { get; set; }

        public static TargetInfo GetTargetInfo()
        {
            TargetInfo ti = new TargetInfo();
            ti.IP = Environment.GetEnvironmentVariable("IOT_TARGET_IP_ADDRESS").Trim();
            ti.Plat = Environment.GetEnvironmentVariable("IOT_TARGET_PLATFORM").Trim();
            return ti;
        }
    }
}
