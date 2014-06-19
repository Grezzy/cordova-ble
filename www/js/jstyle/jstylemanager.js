var JStyleManager = function()
{
    var self = this;
    self.commands = new JStyleCommands();
    self.dummy = function (data) {
        console.log(data);
    };

    //init bluetooth 
    bluetoothle.startScan(function (data) {
        console.log(data);
        bluetoothle.connect(function (result) {
            console.log(result);
            bluetoothle.stopScan(self.dummy, self.dummy);
        }, self.dummy, {});
    }, self.dummy, JSON.stringify({ serviceUuids: [self.commands.serviceId] }));
}