using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Cordova.Extension.Commands
{
    public class ErrorStatus
    {
        public string error { get; set; }
        public string message { get; set; }
    }

    public class GattCharacteristicDescriptorInfo
    {
        public Guid descriptorUuid { get; set; }
    }

    public class GattCharacteristicInfo
    {
        public Guid characteristicUuid { get; set; }
        public List<GattCharacteristicDescriptorInfo> descriptors { get; set; }

        public GattCharacteristicInfo()
        {
            descriptors = new List<GattCharacteristicDescriptorInfo>();
        }
    }

    public class GattCharacteristicValueChanged
    {
        public string callbackId { get; set; }
        public string serviceUuid { get; set; }
        public string characteristicUuid { get; set; }

        public event EventHandler<string> Changed;

        public void ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            byte[] result = args.CharacteristicValue.ToArray();
            if (null != Changed) Changed(this, Convert.ToBase64String(result));
        }
    }

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

    public class PluginCharacteristicRWOptions
    {
        public string serviceUuid { get; set; }
        public string characteristicUuid { get; set; }
        public string value { get; set; }
        public bool isNotification { get; set; }
    }

    public class PluginCharacteristicRWStatus : PluginCharacteristicRWOptions
    {
        public string status { get; set; }
    }

    public class PluginCharacteristicsOptions
    {
        public string serviceUuid { get; set; }
        public string[] characteristicUuids { get; set; }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(serviceUuid);
        }
    }

    public class PluginCharacteristicsStatus
    {
        public string status { get; set; }
        public string serviceUuid { get; set; }
        public string[] characteristicUuids { get; set; }
    }

    public class PluginConnectOptions
    {
        public string address { get; set; }
    }

    public class PluginConnectStatus
    {
        public string status { get; set; }
        public string address { get; set; }
        public string name { get; set; }
    }

    public class PluginServiceStatus
    {
        public string status { get; set; }
        public Guid[] serviceUuids { get; set; }
    }

    public class PluginStartScanOptions
    {
        public string[] serviceUuids { get; set; }
        public string name { get; set; }
    }

    public class PluginStartScanStatus
    {
        public string status { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string rssi { get; set; }
    }

    public class GattDeviceInfo
    {
        public string address { get; set; }
        public string name { get; set; }
        public List<GattServiceInfo> services { get; set; }

        public GattDeviceInfo()
        {
            services = new List<GattServiceInfo>();
        }
    }
}
