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

});
