/*
 * Created by SharpDevelop.
 * User: jaran
 * Date: 08.11.2012
 * Time: 18:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace WindowsPortableDevicesLib.Domain
{
    /// <summary>
    /// Description of PortableDeviceObject.
    /// </summary>
    public class PortableDeviceObject
    {
        protected PortableDeviceObject(string id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }
    }
}
