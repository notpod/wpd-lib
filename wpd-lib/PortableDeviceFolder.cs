/*
 * Created by SharpDevelop.
 * User: jaran
 * Date: 08.11.2012
 * Time: 18:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace WindowsPortableDevicesLib.Domain
{
    /// <summary>
    /// Description of PortableDeviceFolder.
    /// </summary>
    public class PortableDeviceFolder : PortableDeviceObject
    {
        public PortableDeviceFolder(string id, string name) : base(id, name)
        {
            this.Files = new List<PortableDeviceObject>();
        }

        public IList<PortableDeviceObject> Files { get; set; }
    }
}
