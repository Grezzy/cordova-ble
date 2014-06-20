using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CordovaBLE.Plugin
{
    public class PluginCharacteristicRWOptions
    {
        public string serviceUuid { get; set; }
        public string characteristicUuid { get; set; }
        public string value { get; set; }
        public bool isNotification { get; set; }
    }
}
