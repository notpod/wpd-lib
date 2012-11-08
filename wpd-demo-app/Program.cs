/*
 * Created by SharpDevelop.
 * User: jaran
 * Date: 05.11.2012
 * Time: 19:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using WindowsPortableDevicesLib;
using WindowsPortableDevicesLib.Domain;
using System.Collections.Generic;

namespace WindowsPortableDevicesLib
{
    class Program
    {
        
        public static void Main(string[] args)
        {
            StandardWindowsPortableDeviceService service = new StandardWindowsPortableDeviceService();
            
            Console.WriteLine("Available devices:");
            
            IList<WindowsPortableDevice> devices = service.Devices;
            
            if(devices.Count == 0) {
                
                Console.WriteLine("No devices.");
            } else {
                
                int index = 0;
                foreach(WindowsPortableDevice device in devices) {
                    
                    device.Connect();
                    Console.WriteLine("-------------------------------------------------------------");
                    Console.WriteLine("{0} {1} {2}", ++index, device.FriendlyName, device.DeviceID);
                    var folder = device.GetContents();
                    foreach(var item in folder.Files)
                    {
                        DisplayObject(item);
                    }
                    Console.WriteLine("-------------------------------------------------------------");
                    
                    device.Disconnect();
                }
            }
            
            
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
        
        public static void DisplayObject(PortableDeviceObject portableDeviceObject)
        {
            Console.WriteLine(portableDeviceObject.Name);
            if (portableDeviceObject is PortableDeviceFolder)
            {
                DisplayFolderContents((PortableDeviceFolder) portableDeviceObject);
            }
        }

        public static void DisplayFolderContents(PortableDeviceFolder folder)
        {
            foreach (var item in folder.Files)
            {
                Console.WriteLine(item.Id);
                if (item is PortableDeviceFolder)
                {
                    DisplayFolderContents((PortableDeviceFolder) item);
                }
            }
        }
    }
}