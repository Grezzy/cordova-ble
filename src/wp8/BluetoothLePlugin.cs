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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;


namespace Cordova.Extension.Commands
{
    public class BLE : BaseCommand
    {
        private bool isInitialized = false;
        private DeviceInformationCollection devices;
        private BluetoothLEDevice device;

        public BLE()
            : base()
        {
        }

        public void initialize(string options)
        {
            //TODO: Check if bluetooth enabled
            //TODO: Check if bluetooth engine supported

            isInitialized = true;
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new { status = "initialized" }));
        }

        public async void startScan(string options)
        {
            PluginResult result = new PluginResult(PluginResult.Status.OK);
            result.KeepCallback = true;
            string callbackId = result.CallbackId;

            try
            {
                PluginStartScanOptions opt = Parse<PluginStartScanOptions>(options);
                if (opt != null && opt.serviceUuids != null && opt.serviceUuids.Length > 0)
                {
                    devices = await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(new Guid(opt.serviceUuids[0])));
                }
                else
                {
                    devices = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector());
                }

                if (devices.Count == 0)
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new { status = "scanStarted" }));
                }
                else
                {
                    var d = opt != null && !string.IsNullOrEmpty(opt.name) ? devices.FirstOrDefault(x => x.Name == opt.name) : devices.First();
                    if (d != null)
                        DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginStartScanStatus { status = "scanResult", address = d.Id, name = d.Name, rssi = "" }));
                    else
                        DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new { status = "scanStarted" }));
                }
            }
            catch (Exception ex)
            {
                PluginResult errorResult = new PluginResult(PluginResult.Status.ERROR, new { error = "startScan", message = ex.Message });
                DispatchCommandResult(errorResult, callbackId);
            }
        }

        public void stopScan(string options)
        {
            devices = null;
            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginStartScanStatus { status = "scanStopped" }));
        }

        public void connect(string options)
        {
            try
            {
                PluginConnectOptions opt = Parse<PluginConnectOptions>(options);
                if (opt == null || String.IsNullOrEmpty(opt.address))
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "connect", message = "Invalid device address" }));
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
                if (device.ConnectionStatus != BluetoothConnectionStatus.Connected)
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
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "close", message = ex.Message }));
            }
        }

        public void services(string options)
        {
            try
            {
                PluginStartScanOptions opts = Parse<PluginStartScanOptions>(options);

                List<Guid> ids = new List<Guid>();
                if (opts != null && opts.serviceUuids != null)
                {
                    foreach (string s in opts.serviceUuids)
                    {
                        ids.Add(new Guid(s));
                    }
                }

                List<GattServiceInfo> services = new List<GattServiceInfo>();
                foreach (GattDeviceService srv in device.GattServices)
                {
                    if (ids.Contains(srv.Uuid))
                    {
                        services.Add(new GattServiceInfo(srv));
                    }
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginServiceStatus { status = "discoverServices", serviceUuids = services.Select(ss => ss.serviceUuid).ToArray() }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "service", message = ex.Message }));
            }
        }

        public void characteristics(string options)
        {
            try
            {
                PluginCharacteristicsOptions opts = Parse<PluginCharacteristicsOptions>(options);

                List<Guid> ids = new List<Guid>();
                if (opts != null && opts.characteristicUuids != null)
                {
                    foreach (string s in opts.characteristicUuids)
                    {
                        ids.Add(new Guid(s));
                    }
                }

                //TODO: Find out why GetAllCharacteristics doesn't work
                List<Guid> charas = new List<Guid>();
                GattDeviceService service = device.GetGattService(new Guid(opts.serviceUuid));
                foreach (Guid id in ids)
                {
                    if (service.GetCharacteristics(id).Count > 0)
                    {
                        charas.Add(id);
                    }
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginCharacteristicsStatus { status = "discoverCharacteristics", serviceUuid = opts.serviceUuid, characteristicUuids = charas.Select(c => c.ToString()).ToArray() }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "characteristics", message = ex.Message }));
            }
        }

        public void descriptors(string options)
        {
            //TODO: implement descriptors enumeration
            DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "descriptors", message = "Not implemented" }));
        }

        public void read(string options)
        {
            try
            {
                PluginCharacteristicRWOptions opts = Parse<PluginCharacteristicRWOptions>(options);

                var service = device.GetGattService(new Guid(opts.serviceUuid));
                var chara = service.GetCharacteristics(new Guid(opts.characteristicUuid)).First();
                var value = chara.ReadValueAsync().AsTask().GetAwaiter().GetResult();
                if (value.Status != GattCommunicationStatus.Success)
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "read", message = "Communication error" }));
                    return;
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginCharacteristicRWStatus { status = "read", serviceUuid = opts.serviceUuid, characteristicUuid = opts.characteristicUuid, value = Convert.ToBase64String(value.Value.ToArray()) }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "read", message = ex.Message }));
            }
        }

        public void write(string options)
        {
            try
            {
                PluginCharacteristicRWOptions opts = Parse<PluginCharacteristicRWOptions>(options);

                var service = device.GetGattService(new Guid(opts.serviceUuid));
                var chara = service.GetCharacteristics(new Guid(opts.characteristicUuid)).First();
                var status = chara.WriteValueAsync(Convert.FromBase64String(opts.value).AsBuffer()).AsTask().GetAwaiter().GetResult();
                if (status != GattCommunicationStatus.Success)
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "write", message = "Communication error" }));
                    return;
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginCharacteristicRWStatus { status = "write", serviceUuid = opts.serviceUuid, characteristicUuid = opts.characteristicUuid, value = opts.value }));
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "write", message = ex.Message }));
            }
        }

        public void subscribe(string options)
        {
            try
            {
                PluginCharacteristicRWOptions opts = Parse<PluginCharacteristicRWOptions>(options);

                var service = device.GetGattService(new Guid(opts.serviceUuid));
                var chara = service.GetCharacteristics(new Guid(opts.characteristicUuid)).First();
                if (chara.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    GattCharacteristicValueChanged vc = new GattCharacteristicValueChanged { callbackId = this.CurrentCommandCallbackId, characteristicUuid = opts.characteristicUuid, serviceUuid = opts.serviceUuid };
                    chara.ValueChanged += vc.ValueChanged;
                    vc.Changed += vc_Changed;
                }

                DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginCharacteristicRWStatus { status = "subscribed", serviceUuid = opts.serviceUuid, characteristicUuid = opts.characteristicUuid }) { KeepCallback = true });
            }
            catch (Exception ex)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, new ErrorStatus { error = "subscribe", message = ex.Message }));
            }
        }

        void vc_Changed(object sender, string e)
        {
            var vc = (GattCharacteristicValueChanged)sender;
            this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new PluginCharacteristicRWStatus { status = "subscribedResult", serviceUuid = vc.serviceUuid, characteristicUuid = vc.characteristicUuid, value = e }) { KeepCallback = true }, vc.callbackId);
        }


        private JObject Parse(string options)
        {
            string opt = JsonHelper.Deserialize<string[]>(options)[0];
            return JObject.Parse(opt);
        }

        private T Parse<T>(string options)
        {
            try
            {
                string opt = JsonHelper.Deserialize<string[]>(options)[0];
                return !String.IsNullOrEmpty(opt) ? JsonConvert.DeserializeObject<T>(opt) : default(T);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error parsing arguments: " + ex.Message);
                return default(T);
            }
        }


    }
}
