using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CordovaBLE.Plugin
{
    public class PluginCharacteristicsOptions
    {
        public string serviceUuid { get; set; }
        public string[] characteristicUuids { get; set; }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(serviceUuid);
        }
    }
}
