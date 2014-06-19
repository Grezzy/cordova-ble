using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;
using CordovaBLE.Plugin;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Cordova.Extension.Commands
{
    public class BluetoothLePlugin : BaseCommand
    {
        private bool isInitialized = false;
        private DeviceInformationCollection devices;
        private BluetoothLEDevice device;
        public void initialize(string options)
        {
            //TODO: Check if bluetooth enabled
            //TODO: Check if bluetooth engine supported

            isInitialized = true;
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new { status = "initialized" }));
        }

        public void startScan(string options)
        {
            PluginStartScanOptions opt = Parse<PluginStartScanOptions>(options);
            if (opt != null && opt.serviceUuids != null && opt.serviceUuids.Length > 0)
            {
                devices = DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(new Guid(opt.serviceUuids[0]))).AsTask().GetAwaiter().GetResult();
            }
            else
            {
                devices = DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector()).AsTask().GetAwaiter().GetResult();
            }

            if (devices.Count == 0)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new { status = "scanStarted" }));
            }
            else
            {
                var d = devices.First();
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginStartScanStatus { status = "scanResult", address = d.Id, name = d.Name, rssi = "" }));
            }

        }

        public void stopScan(string options)
        {
            devices = null;
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new { status = "scanStopped" }));
        }

        public void connect(string options)
        {
            try
            {
                PluginConnectOptions opt = Parse<PluginConnectOptions>(options);
                if (opt == null || String.IsNullOrEmpty(opt.address))
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new { error = "connect", message = "Invalid device address" }));
                    return;
                }

                device = BluetoothLEDevice.FromIdAsync(opt.address).AsTask().GetAwaiter().GetResult();                                
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginConnectStatus { status = device.ConnectionStatus.ToString().ToLower(), address = device.DeviceId, name = device.Name }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "connect", message = ex.Message }));                         
            }
        }

        public void reconnect(string options)
        {
            try
            {
                if(device.ConnectionStatus != BluetoothConnectionStatus.Connected)
                {
                    // TODO: connect
                }
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginConnectStatus { status = device.ConnectionStatus.ToString().ToLower(), address = device.DeviceId, name = device.Name }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "reconnect", message = ex.Message }));
            }
        }

        public void disconnect(string options)
        {
            try
            {
                var status = new PluginConnectStatus { status = "disconnected", address = device.DeviceId, name = device.Name };
                device = null;
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, status));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "disconnect", message = ex.Message }));
            }
        }

        public void close(string options)
        {
            try
            {
                var status = new PluginConnectStatus { status = "closed", address = device.DeviceId, name = device.Name };
                device = null;
                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, status));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "close", message = ex.Message }));
            }
        }

        public void discover(string options)
        {
            try
            {
                GattDeviceInfo dvc = new GattDeviceInfo { address = device.DeviceId, name = device.Name };
                foreach (GattDeviceService service in device.GattServices)
                {
                    if (dvc.services.Any(s => s.serviceUuid == service.Uuid)) continue;

                    GattServiceInfo bleService = new GattServiceInfo(service);                    
                    dvc.services.Add(bleService);
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, dvc));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new { error = "close", message = ex.Message }));
            }
        }

        public void service(string options)
        {
            try
            {
                PluginStartScanOptions opts = Parse<PluginStartScanOptions>(options);

                List<Guid> ids = new List<Guid>();
                if(opts != null && opts.serviceUuids != null)
                {
                    foreach(string s in opts.serviceUuids)
                    {
                        ids.Add(new Guid(s));
                    }
                }

                List<GattServiceInfo> services = new List<GattServiceInfo>();
                foreach(GattDeviceService srv in device.GattServices)
                {
                    if(ids.Contains(srv.Uuid))
                    {
                        services.Add(new GattServiceInfo(srv));
                    }
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginServiceStatus { status = "discoverServices", serviceUuids = services.Select(ss => ss.serviceUuid).ToArray() }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new { error = "close", message = ex.Message }));
            }
        }
        private JObject Parse(string options)
        {
            string opt = JsonHelper.Deserialize<string[]>(options)[0];
            return JObject.Parse(opt);
        }

        private T Parse<T>(string options)
        {
            string opt = JsonHelper.Deserialize<string[]>(options)[0];
            return !String.IsNullOrEmpty(opt) ? JsonConvert.DeserializeObject<T>(opt) : default(T);
        }

        private const String keyStatus = "status";
        private const String keyError = "error";
        private const String keyMessage = "message";
        private const String keyRequest = "request";
        private const String keyName = "name";
        private const String keyAddress = "address";
        private const String keyRssi = "rssi";
        private const String keyAdvertisement = "advertisement";
        private const String keyServiceUuids = "serviceUuids";
        private const String keyServiceUuid = "serviceUuid";
        private const String keyCharacteristicUuid = "characteristicUuid";
        private const String keyDescriptorUuid = "descriptorUuid";
        private const String keyServices = "services";
        private const String keyCharacteristics = "characteristics";
        private const String keyDescriptors = "descriptors";
        private const String keyValue = "value";
        private const String keyIsInitialized = "isInitalized";
        private const String keyIsScanning = "isScanning";
        private const String keyIsConnected = "isConnected";
        private const String keyIsDiscovered = "isDiscovered";
        private const String keyIsNotification = "isNotification";
    }
}
