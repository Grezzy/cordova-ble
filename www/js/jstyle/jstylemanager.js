var JStyleManager = function()
{
    var self = this;
    self.commands = new JStyleCommands();
    self.dummy = function (data) {
        console.log(JSON.stringify(data));
    };

    self.onread = function (res) {
        console.log(JSON.stringify(res));
    };
    
    self.shutDown = function()
    {
        //disconnect
        bluetoothle.close(self.dummy, self.dummy);
        //stop scanning
        bluetoothle.stopScan(self.dummy, self.dummy);
    }

    //init bluetooth 
    bluetoothle.startScan(function (data) {
        console.log('Start scan: ' + JSON.stringify(data));
        //connect
        bluetoothle.connect(function (result) {            
            console.log('Connect: ' + JSON.stringify(result));
            //discover tracker service
            bluetoothle.services(function (srv) {
                console.log('Service: ' + JSON.stringify(srv));
                var cmd = bluetoothle.bytesToEncodedString(self.commands.setDateTime(new Date()));
                bluetoothle.write(self.onread, self.dummy, JSON.stringify({ serviceUuid: self.commands.serviceId, characteristicUuid: self.commands.txId, value: cmd }));
            }, self.dummy, JSON.stringify({ serviceUuids: [self.commands.serviceId] }));
            
        }, self.dummy, JSON.stringify({ address: data.address }));
    }, self.dummy, JSON.stringify({ serviceUuids: [self.commands.serviceId] }));
}