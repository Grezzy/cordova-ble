var JStyleManager = function () {
    var self = this;
    var commands = new JStyleCommands();
    var queue = [];
    var callbacks = [];
    var isRunning = false;
    var address = null;

    self.start = function () {
        isRunning = true;
        self.process();
    };

    self.stop = function () {
        isRunning = false;
        queue = [];
        callbacks = [];
    };

    self.process = function () {
        if (!isRunning) return;
        if (queue.length) {
            var cmd = queue.shift();
            cmd.call();
        } else {
            setTimeout(self.process, 100);
        }
    };

    self.onrxsignal = function (data) {
        
        if (callbacks.length) {
            var clb = callbacks.shift();

            var r = data.value ? bluetoothle.encodedStringToBytes(data.value) : data;
            if (clb) clb(r);
        }
    };

    self.enqueue = function (arg, clb) {
        queue.push(arg);
        if (clb) callbacks.push(clb);
    }

    self.dummy = function (data) {
        console.log(JSON.stringify(data));
        self.process();
    };

    self.connect = function (callback) {
        //start scan
        self.enqueue(function () {
            bluetoothle.startScan(function (data) {
                console.log('Start scan: ' + JSON.stringify(data));
                //assume device address is known at once
                address = data.address;
                self.process();
            }, self.dummy);
        });

        //connect
        self.enqueue(function () {
            bluetoothle.connect(function (data) {
                console.log('Connect: ' + JSON.stringify(data));
                self.process();
            }, self.dummy, JSON.stringify({ address: address }));
        });

        //subscribe to RX characteristics update
        self.enqueue(function () {
            bluetoothle.subscribe(function (data) {
                console.log('Subscribe: ' + JSON.stringify(data));
                self.onrxsignal(data);
                self.process();
            }, self.dummy, JSON.stringify({ serviceUuid: commands.serviceId, characteristicUuid: commands.rxId }));
        }, callback);
    };

    self.setDateTime = function (dateTime, callback) {
        self.enqueue(function () {
            var cmdSetTime = bluetoothle.bytesToEncodedString(commands.setDateTime(dateTime));
            bluetoothle.write(self.dummy, self.dummy, JSON.stringify({ serviceUuid: commands.serviceId, characteristicUuid: commands.txId, value: cmdSetTime }));
        }, function (r) {
            if (callback) callback(commands.setDateTimeResult(r));
        });
    };

    self.onread = function (r) {
        console.log('read');
        if (r.value) console.log(JSON.stringify(bluetoothle.encodedStringToBytes(r.value)));
        else console.log(JSON.stringify(r));
    };

    /*
    self.onchanged = function (r) {
        console.log('changed');
        
        if (r.value) console.log(JSON.stringify(bluetoothle.encodedStringToBytes(r.value)));
        else console.log(JSON.stringify(r));        
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
                var cmdSetTime = bluetoothle.bytesToEncodedString(self.commands.setDateTime(new Date()));
                var cmdGetTime = bluetoothle.bytesToEncodedString(self.commands.getDateTime());
                bluetoothle.subscribe(self.onchanged, self.dummy, JSON.stringify({ serviceUuid: self.commands.serviceId, characteristicUuid: self.commands.rxId }));
                setTimeout(function () {
                    //bluetoothle.write(self.onread, self.dummy, JSON.stringify({ serviceUuid: self.commands.serviceId, characteristicUuid: self.commands.txId, value: cmdSetTime }));
                    bluetoothle.write(self.onread, self.dummy, JSON.stringify({ serviceUuid: self.commands.serviceId, characteristicUuid: self.commands.txId, value: cmdGetTime }));
                }, 2000);
            }, self.dummy, JSON.stringify({ serviceUuids: [self.commands.serviceId] }));
            
        }, self.dummy, JSON.stringify({ address: data.address }));
    }, self.dummy, JSON.stringify({ serviceUuids: [self.commands.serviceId] }));
    */
}