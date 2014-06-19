using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace CordovaBLE.Plugin
{
    public class GattServiceInfo
    {
        public Guid serviceUuid { get; set; }
        public List<GattCharacteristicInfo> characteristics { get; set; }

        public GattServiceInfo()
        {
            characteristics = new List<GattCharacteristicInfo>();
        }

        public GattServiceInfo(GattDeviceService srv)
        {
            characteristics = new List<GattCharacteristicInfo>();

            serviceUuid = srv.Uuid;
            /*foreach (GattCharacteristic chara in srv.GetAllCharacteristics())
            {
                GattCharacteristicInfo bleChar = new GattCharacteristicInfo { characteristicUuid = chara.Uuid };

                foreach (GattDescriptor desc in chara.GetAllDescriptors())
                {
                    GattCharacteristicDescriptorInfo bleDesc = new GattCharacteristicDescriptorInfo { descriptorUuid = desc.Uuid };
                    bleChar.descriptors.Add(bleDesc);
                }
                bleService.characteristics.Add(bleChar);

            }*/
        }
    }
}
