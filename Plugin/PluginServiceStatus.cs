using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CordovaBLE.Plugin
{
    public class PluginServiceStatus
    {
        public string status { get; set; }
        public Guid[] serviceUuids { get; set; }
    }
}
