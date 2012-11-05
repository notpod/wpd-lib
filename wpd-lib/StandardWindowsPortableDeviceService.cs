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
                
                uint numberOfDevices = 0;
                portableDeviceManager.GetDevices(null, ref numberOfDevices);

                if (numberOfDevices == 0)
                {
                    return new string[0];
                }
                
                string[] deviceIds = new string[numberOfDevices];
                portableDeviceManager.GetDevices(deviceIds, ref numberOfDevices);
                
                return deviceIds;
            }
        }
        
        public WindowsPortableDevice ConnectDevice(string deviceID)
        {
            throw new NotImplementedException();
        }
    }
}
