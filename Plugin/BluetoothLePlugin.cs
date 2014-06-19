using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;

namespace Cordova.Extension.Commands
{
    public class BluetoothLePlugin : BaseCommand
    {
        public void initialize(string options)
        {
            // all JS callable plugin methods MUST have this signature!
            // public, returning void, 1 argument that is a string
        }
    }
}
