var JStyleCommands = function () {

    var self = this;
    self.serviceId = '0000fff0-0000-1000-8000-00805f9b34fb';
    self.txId = '0000fff6-0000-1000-8000-00805f9b34fb';
    self.rxId = '0000fff7-0000-1000-8000-00805f9b34fb';

    self.setDateTime = function (dateTime) {

        var command = self.getEmptyCommand();

        command[0] = 0x01;
        command[1] = self.toBCD(dateTime.getFullYear() - 2000);
        command[2] = self.toBCD(dateTime.getMonth());
        command[3] = self.toBCD(dateTime.getDay());
        command[4] = self.toBCD(dateTime.getHour());
        command[5] = self.toBCD(dateTime.getMinute());
        command[6] = self.toBCD(dateTime.getSecond());

        command[15] = self.CRC(command);

        return command;
    }

    self.getEmptyCommand = function () {
        var a = new Array(16);
        for (var i = 0; i < a.length; i++) {
            a[i] = 0;
        }
        return a;
    }

    self.CRC = function (command) {
        var crc = 0;
        for (var i = 0; i < command.length - 1; i++) {
            crc += command[i];
        }
        return crc & 0xFF;
    }

    self.toBCD = function (n) {
        // extract each digit from the input number n
        var d1 = n / 10;
        var d2 = n % 10;
        // combine the decimal digits into a BCD number
        return (d1 << 4) | d2;
    }
}