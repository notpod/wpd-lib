using System;
using WindowsPortableDevicesLib.Domain;
using PortableDeviceApiLib;
using System.Collections.Generic;

namespace WindowsPortableDevicesLib
{

    public class StandardWindowsPortableDeviceService : WindowsPortableDeviceService
    {
        private PortableDeviceManager portableDeviceManager = new PortableDeviceManagerClass();
        
        public StandardWindowsPortableDeviceService()
        {
            
        }
        
        public IList<WindowsPortableDevice> Devices {
            get {

                portableDeviceManager.RefreshDeviceList();  
                
                uint numberOfDevices = 0;
                portableDeviceManager.GetDevices(null, ref numberOfDevices);

                if (numberOfDevices == 0)
                {
                    return new List<WindowsPortableDevice>();
                }
                
                string[] deviceIds = new string[numberOfDevices];
                portableDeviceManager.GetDevices(deviceIds, ref numberOfDevices);
                
                List<WindowsPortableDevice> devices = new List<WindowsPortableDevice>();
                foreach(string deviceId in deviceIds) {
                    
                    devices.Add(new WindowsPortableDevice(deviceId));
                }
                
                return devices;
            }
        }
       
    }
}
