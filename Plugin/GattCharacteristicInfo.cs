using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CordovaBLE.Plugin
{
    public class GattCharacteristicInfo
    {
        public Guid characteristicUuid { get; set; }
        public List<GattCharacteristicDescriptorInfo> descriptors { get; set; }

        public GattCharacteristicInfo()
        {
            descriptors = new List<GattCharacteristicDescriptorInfo>();
        }
    }
}
