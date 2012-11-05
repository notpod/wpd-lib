using System;
using WindowsPortableDevicesLib.Domain;
using PortableDeviceApiLib;

namespace WindowsPortableDevicesLib
{

    public class StandardWindowsPortableDeviceService : WindowsPortableDeviceService
    {
        private PortableDeviceManager portableDeviceManager = new PortableDeviceManagerClass();
        
        public StandardWindowsPortableDeviceService()
        {
            
        }
        
        public string[] DeviceIDs {
            get {
                portableDeviceManager.RefreshDeviceList();

                // Determine how many WPD devices are connected
                var deviceIds = new string[1];
                uint count = 1;
                portableDeviceManager.GetDevices(ref deviceIds[0], ref count);

                // Retrieve the device id for each connected device
                deviceIds = new string[count];
                portableDeviceManager.GetDevices(ref deviceIds[0], ref count);
                
                return deviceIds;
            }
        }
        
        public WindowsPortableDevice ConnectDevice(string deviceID)
        {
            throw new NotImplementedException();
        }
    }
}
