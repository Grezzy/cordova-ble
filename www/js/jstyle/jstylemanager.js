var JStyleManager = function()
{
    var self = this;
    self.commands = new JStyleCommands();
    self.dummy = function (data) {
        console.log(data);
    };

    //init bluetooth 
    bluetoothle.initialize(self.dummy, self.dummy, bluetoothleName, 'initialize', '');
}