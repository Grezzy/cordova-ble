cordova.define("com.firerunner.cordova.ble.BLE", function (require, exports, module) { // API definition for EvoThings BLE plugin.
    //
    // Use jsdoc to generate documentation.

    // The following line causes a jsdoc error.
    // Use the jsdoc option -l to ignore the error.
    var exec = cordova.require('cordova/exec');

    exports.initialize = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "initialize", [params]);
    };
    exports.isInitialized = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "isInitialized", [params]);
    };
    exports.startScan = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "startScan", [params]);
    };
    exports.stopScan = function (successCallback, errorCallback) {
        cordova.exec(successCallback, errorCallback, "BLE", "stopScan", []);
    };
    exports.connect = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "connect", [params]);
    };

    exports.reconnect = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "reconnect", [params]);
    };
    exports.isConnected = function (successCallback, errorCallback) {
        cordova.exec(successCallback, errorCallback, "BLE", "isConnected");
    };
    exports.wasNeverConnected = function (successCallback, errorCallback) {
        cordova.exec(successCallback, errorCallback, "BLE", "wasNeverConnected");
    };
    exports.subscribe = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "subscribe", [params]);
    };
    exports.discover = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "discover", [params]);
    };
    exports.unsubscribe = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "unsubscribe", [params]);
    };
    exports.write = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "write", [params]);
    };
    exports.services = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "services", [params]);
    };
    exports.characteristics = function (successCallback, errorCallback, params) {
        cordova.exec(successCallback, errorCallback, "BLE", "characteristics", [params]);
    };
    exports.encodedStringToBytes = function (string) {
        var data = atob(string);
        var bytes = new Uint8Array(data.length);
        for (var i = 0; i < bytes.length; i++) {
            bytes[i] = data.charCodeAt(i);
        }
        return bytes;
    };

    exports.bytesToEncodedString = function (bytes) {
        return btoa(String.fromCharCode.apply(null, bytes));
    };

    exports.stringToBytes = function (string) {
        var bytes = new ArrayBuffer(string.length * 2);
        var bytesUint16 = new Uint16Array(bytes);
        for (var i = 0; i < string.length; i++) {
            bytesUint16[i] = string.charCodeAt(i);
        }
        return new Uint8Array(bytesUint16);
    };

    exports.bytesToString = function (bytes) {
        return String.fromCharCode.apply(null, new Uint16Array(bytes));
    };

    // Windows Runtime only code
    if (WinJS && WinJS.Namespace) {

        var WinBLE = function () {
            var gatt = Windows.Devices.Bluetooth.GenericAttributeProfile;
            var pnp = Windows.Devices.Enumeration.Pnp;
            var devInfo = Windows.Devices.Enumeration.DeviceInformation;
            var watcher, device;
            var isConnected = false;
            var rxCallback = null;

            var self = this;
            self.isInitialized = false;

            self.initialize = function (win, fail, params) {
                console.log("WINBLE is intializing...");
                self.isInitialized = true;
                win({ isInitialized: true });
            };

            self.startScan = function (win, fail, params) {

                try {
                    var serviceUuids = params[0].serviceUuids;
                    var callbackId = Object.keys(cordova.callbacks)[0];
                    devInfo.findAllAsync().done(
                        function (devices) {
                            if (devices.length > 0) {
                                for (var i = 0; i < devices.length; i++) {
                                    var device = devices[i];
                                    for (var j = 0; j < serviceUuids.length; j++) {
                                        if (device.id.indexOf(serviceUuids[j]) > 0 && device.id.indexOf(serviceUuids[j]) == device.id.lastIndexOf(serviceUuids[j]) && device.isEnabled) {
                                            cordova.callbackSuccess(callbackId, { keepCallback: true, status: cordova.callbackStatus.OK, message: { status: 'scanResult', address: device.id, name: device.name } });
                                        }
                                    }
                                }
                            } else {
                                if (fail) fail("Could not find any compatible devices. Please make sure your device is paired and powered on.");
                            }
                        }, function (error) {
                            if (fail) fail("Finding devices failed with error :" + error);
                        });
                } catch (e) {
                    if (fail) fail(e);
                }

            };

            self.discover = function (win, fail, params) {
                if (device && isConnected) {
                    win({ status: isConnected ? 'connected' : 'connecting', address: device.deviceId, name: device.name });
                } else {
                    fail('Not connected.');
                }
            };

            self.connect = function (win, fail, params) {
                try {
                    var address = params[0].address;

                    Windows.Devices.Bluetooth.BluetoothLEDevice.fromIdAsync(address).done(function (dev) {
                        device = dev;
                        isConnected = true;//dev.connectionStatus == gatt.GattCommunicationStatus.success;
                        win({ status: isConnected ? 'connected' : 'connecting', address: device.deviceId, name: device.name });
                    });

                } catch (e) {
                    if (fail) fail(e);
                }
            }

            self.reconnect = function (win, fail, params) {
                self.connect(win, fail, params);
            }

            self.isConnected = function (win, fail) {
                win({ isConnected: isConnected });
            }

            self.wasNeverConnected = function (win, fail) {
                win({ wasNeverConnected: !isConnected });
            }

            self.services = function (win, fail, params) {
                try {
                    if (device && isConnected) {
                        var requested = params[0].serviceUuids;
                        var ids = [];
                        for (var i = 0; i < device.gattServices.length; i++) {
                            if (requested && requested.length) {
                                for (var j = 0; j < requested.length; j++) {
                                    if (requested[j] == device.gattServices[i].uuid) {
                                        ids.push(device.gattServices[i].uuid);
                                    }
                                }
                            } else {
                                ids.push(device.gattServices[i].uuid);
                            }
                        }
                        win({ status: 'discoveredServices', serviceUuids: ids, address: device.deviceId, name: device.name });
                    } else {
                        fail('Not connected.');
                    }
                } catch (e) {
                    if (fail) fail(e);
                }
            };

            self.characteristics = function (win, fail, params) {
                try {
                    if (device && isConnected) {
                        var serviceId = params[0].serviceUuid;
                        var requested = params[0].characteristicUuids;
                        var ids = [];

                        var service = device.getGattService(serviceId);
                        var chars = service.getAllCharacteristics();
                        for (var i = 0; i < chars.length; i++) {
                            var chara = chars[i];
                            if (requested && requested.length) {
                                for (var j = 0; j < requested.length; j++) {
                                    if (requested[j] == chara.uuid) {
                                        // TODO: chara.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)
                                        ids.push({ properties: [], characteristicUuid: chara.uuid });
                                    }
                                }
                            } else {
                                // TODO: chara.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)
                                ids.push({ properties: [], characteristicUuid: chara.uuid });
                            }
                        }
                        win({ status: 'discoveredCharacteristics', serviceUuid: serviceId, characteristicUuids: ids, address: device.deviceId, name: device.name });
                    } else {
                        fail('Not connected.');
                    }
                } catch (e) {
                    if (fail) fail(e);
                }
            };

            self.subscribe = function (win, fail, params) {
                try {
                    if (device && isConnected) {

                        var callbackId = Object.keys(cordova.callbacks)[0];

                        var serviceId = params[0].serviceUuid;
                        var charId = params[0].characteristicUuid;

                        var service = device.getGattService(serviceId);
                        var chara = service.getCharacteristics(charId)[0];

                        rxCallback = function (args) {
                            var data = new Uint8Array(args.characteristicValue.length);
                            Windows.Storage.Streams.DataReader.fromBuffer(args.characteristicValue).readBytes(data);
                            cordova.callbackSuccess(callbackId, { keepCallback: true, status: cordova.callbackStatus.OK, message: { status: 'subscribedResult', value: base64EncArr(data), serviceUuid: serviceId, characteristicUuid: charId, address: device.deviceId, name: device.name } });
                        };

                        chara.addEventListener("valuechanged", rxCallback);

                        chara.writeClientCharacteristicConfigurationDescriptorAsync(
                        gatt.GattClientCharacteristicConfigurationDescriptorValue.notify).then(
                            function (communicationStatus) {
                                if (communicationStatus === gatt.GattCommunicationStatus.unreachable) {
                                    watcher = pnp.PnpObject.createWatcher(pnp.PnpObjectType.deviceContainer, ["System.Devices.Connected"], "");
                                    watcher.onupdated = function (e) {
                                        var isConnected = e.properties["System.Devices.Connected"];
                                        if ((e.id === containerId) && (isConnected === true)) {
                                            // Set the Client Characteristic Configuration descriptor on the device, 
                                            // registering for Characteristic Value Changed notifications.
                                            characteristic.writeClientCharacteristicConfigurationDescriptorAsync(
                                                gatt.GattClientCharacteristicConfigurationDescriptorValue.notify).done(
                                                    function (communicationStatus) {
                                                        if (communicationStatus === gatt.GattCommunicationStatus.success) {
                                                            cordova.callbackSuccess(callbackId, { keepCallback: true, status: cordova.callbackStatus.OK, message: { status: 'subscribed', serviceUuid: serviceId, characteristicUuid: charId, address: device.deviceId, name: device.name } });
                                                            watcher.stop();
                                                            watcher = null;
                                                        }
                                                    }, function (error) {
                                                        if (fail) fail(e);
                                                    });
                                        }
                                    };
                                    watcher.start();
                                } else {
                                    cordova.callbackSuccess(callbackId, { keepCallback: true, status: cordova.callbackStatus.OK, message: { status: 'subscribed', serviceUuid: serviceId, characteristicUuid: charId, address: device.deviceId, name: device.name } });
                                }
                            }
                        );

                    } else {
                        fail('Not connected.');
                    }
                } catch (e) {
                    if (fail) fail(e);
                }
            };

            self.write = function (win, fail, params) {
                try {
                    if (device && isConnected) {
                        var serviceId = params[0].serviceUuid;
                        var charId = params[0].characteristicUuid;

                        var dataWriter = new Windows.Storage.Streams.DataWriter();
                        dataWriter.writeBytes(base64DecToArr(params[0].value));
                        var value = dataWriter.detachBuffer();

                        var service = device.getGattService(serviceId);
                        var chara = service.getCharacteristics(charId)[0];

                        chara.writeValueAsync(value).done(function (status) {
                            win({ status: 'written', value: value, serviceUuid: serviceId, characteristicUuid: charId, address: device.deviceId, name: device.name });
                        });
                    } else {
                        fail('Not connected.');
                    }
                } catch (e) {
                    if (fail) fail(e);
                }
            };
                        
            function b64ToUint6(nChr) {

                return nChr > 64 && nChr < 91 ?
                    nChr - 65
                  : nChr > 96 && nChr < 123 ?
                    nChr - 71
                  : nChr > 47 && nChr < 58 ?
                    nChr + 4
                  : nChr === 43 ?
                    62
                  : nChr === 47 ?
                    63
                  :
                    0;

            }

            function base64DecToArr(sBase64, nBlocksSize) {

                var
                  sB64Enc = sBase64.replace(/[^A-Za-z0-9\+\/]/g, ""), nInLen = sB64Enc.length,
                  nOutLen = nBlocksSize ? Math.ceil((nInLen * 3 + 1 >> 2) / nBlocksSize) * nBlocksSize : nInLen * 3 + 1 >> 2, taBytes = new Uint8Array(nOutLen);

                for (var nMod3, nMod4, nUint24 = 0, nOutIdx = 0, nInIdx = 0; nInIdx < nInLen; nInIdx++) {
                    nMod4 = nInIdx & 3;
                    nUint24 |= b64ToUint6(sB64Enc.charCodeAt(nInIdx)) << 18 - 6 * nMod4;
                    if (nMod4 === 3 || nInLen - nInIdx === 1) {
                        for (nMod3 = 0; nMod3 < 3 && nOutIdx < nOutLen; nMod3++, nOutIdx++) {
                            taBytes[nOutIdx] = nUint24 >>> (16 >>> nMod3 & 24) & 255;
                        }
                        nUint24 = 0;

                    }
                }

                return taBytes;
            }

            function uint6ToB64(nUint6) {

                return nUint6 < 26 ?
                    nUint6 + 65
                  : nUint6 < 52 ?
                    nUint6 + 71
                  : nUint6 < 62 ?
                    nUint6 - 4
                  : nUint6 === 62 ?
                    43
                  : nUint6 === 63 ?
                    47
                  :
                    65;

            }

            function base64EncArr(aBytes) {

                var nMod3 = 2, sB64Enc = "";

                for (var nLen = aBytes.length, nUint24 = 0, nIdx = 0; nIdx < nLen; nIdx++) {
                    nMod3 = nIdx % 3;
                    if (nIdx > 0 && (nIdx * 4 / 3) % 76 === 0) { sB64Enc += "\r\n"; }
                    nUint24 |= aBytes[nIdx] << (16 >>> nMod3 & 24);
                    if (nMod3 === 2 || aBytes.length - nIdx === 1) {
                        sB64Enc += String.fromCharCode(uint6ToB64(nUint24 >>> 18 & 63), uint6ToB64(nUint24 >>> 12 & 63), uint6ToB64(nUint24 >>> 6 & 63), uint6ToB64(nUint24 & 63));
                        nUint24 = 0;
                    }
                }

                return sB64Enc.substr(0, sB64Enc.length - 2 + nMod3) + (nMod3 === 2 ? '' : nMod3 === 1 ? '=' : '==');

            }

            return self;
        }

        WinJS.Namespace.define('BLE', new WinBLE());
        cordova.commandProxy.add("BLE", BLE);
    }
});
