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
namespace wpd_demo_app
{
    class Program
    {
        
        public static void Main(string[] args)
        {
            StandardWindowsPortableDeviceService service = new StandardWindowsPortableDeviceService();
            
            Console.WriteLine("Available devices:");
            
            string[] ids = service.DeviceIDs;
            
            if(ids.Length == 0) {
                
                Console.WriteLine("No devices.");
            } else {
                
                int index = 0;
                foreach(string id in ids) {
                    
                    Console.WriteLine("{0} {1}", ++index, id);
                }
            }
            
            
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}