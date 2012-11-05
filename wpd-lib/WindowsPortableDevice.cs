/*
 * Created by SharpDevelop.
 * User: jaran
 * Date: 05.11.2012
 * Time: 19:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WindowsPortableDevicesLib.Domain
{
    
    public class WindowsPortableDevice
    {
        private string deviceID;
        
        public WindowsPortableDevice(string deviceID)
        {
            this.deviceID = deviceID;
        }
        
        public string DeviceID {
            
            get { return deviceID; }

        }
    }
}
