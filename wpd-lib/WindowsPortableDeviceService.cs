/*
 * Created by SharpDevelop.
 * User: jaran
 * Date: 05.11.2012
 * Time: 19:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using WindowsPortableDevicesLib.Domain;

namespace WindowsPortableDevicesLib
{

    public interface WindowsPortableDeviceService
    {
        
        string[] DeviceIDs { get; }
        
        WindowsPortableDevice ConnectDevice(string deviceID);
        
    }

}
