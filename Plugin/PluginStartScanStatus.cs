using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CordovaBLE.Plugin
{
    public class PluginStartScanStatus
    {
        public string status { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string rssi { get; set; }
    }
}
