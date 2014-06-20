using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CordovaBLE.Plugin
{
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
}
