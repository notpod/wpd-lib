/*
 * Created by SharpDevelop.
 * User: jaran
 * Date: 05.11.2012
 * Time: 19:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PortableDeviceApiLib;
using Common.Logging;

namespace WindowsPortableDevicesLib.Domain
{
    
    public class WindowsPortableDevice
    {
        private ILog l = LogManager.GetLogger(typeof(WindowsPortableDevice));
        
        private string deviceID;
        
        private PortableDeviceClass device;
        
        public WindowsPortableDevice(string deviceID)
        {
            this.deviceID = deviceID;
        }
        
        public string DeviceID {
            
            get { return deviceID; }

        }
        
        public string FriendlyName
        {
            get
            {
                ValidateConnection();

                // Retrieve the properties of the device
                IPortableDeviceContent content;
                IPortableDeviceProperties properties;
                device.Content(out content);
                content.Properties(out properties);

                // Retrieve the values for the properties
                IPortableDeviceValues propertyValues;
                properties.GetValues("DEVICE", null, out propertyValues);

                // Identify the property to retrieve
                var property = new _tagpropertykey();
                property.fmtid = new Guid(0x26D4979A, 0xE643, 0x4626, 0x9E, 0x2B,
                                          0x73, 0x6D, 0xC0, 0xC9, 0x2F, 0xDC);
                property.pid = 12;

                // Retrieve the friendly name
                string propertyValue;
                propertyValues.GetStringValue(ref property, out propertyValue);

                return propertyValue;
            }
        }
        
        void ValidateConnection()
        {
            if (device == null)
            {
                throw new InvalidOperationException("Not connected to device.");
            }
        }
        
        public void Connect()
        {
            if (this.device != null) {
                
                l.Debug("Device is already connected.");
                return;
            }
            var clientInfo = (IPortableDeviceValues) new PortableDeviceTypesLib.PortableDeviceValuesClass();
            device = new PortableDeviceClass();
            device.Open(deviceID, clientInfo);
        }

        public void Disconnect()
        {
            if (!(this.device == null)) {
                
                l.Debug("Device is not connected.");
                return;
            }
            this.device.Close();
            this.device = null;
        }
        
        public PortableDeviceFolder GetContents()
        {
            ValidateConnection();
            
            var root = new PortableDeviceFolder("DEVICE", "DEVICE");

            IPortableDeviceContent content;
            device.Content(out content);
            EnumerateContents(ref content, root);

            return root;
        }
        
        private static void EnumerateContents(ref IPortableDeviceContent content,
                                              PortableDeviceFolder parent)
        {
            // Get the properties of the object
            IPortableDeviceProperties properties;
            content.Properties(out properties);

            // Enumerate the items contained by the current object
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parent.Id, null, out objectIds);

            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);
                if (fetched > 0)
                {
                    var currentObject = WrapObject(properties, objectId);

                    parent.Files.Add(currentObject);

                    if (currentObject is PortableDeviceFolder)
                    {
                        EnumerateContents(ref content, (PortableDeviceFolder) currentObject);
                    }
                }
            } while (fetched > 0);
        }
        
        private static PortableDeviceObject WrapObject(IPortableDeviceProperties properties,
                                                       string objectId)
        {
            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            // Get the name of the object
            string name;
            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                      0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 4;
            values.GetStringValue(property, out name);

            // Get the type of the object
            Guid contentType;
            property = new _tagpropertykey();
            property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                                      0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            property.pid = 7;
            values.GetGuidValue(property, out contentType);

            var folderType = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C,
                                      0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
            var functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98,
                                          0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);

            if (contentType == folderType  || contentType == functionalType)
            {
                return new PortableDeviceFolder(objectId, name);
            }

            return new PortableDeviceFile(objectId, name);
        }
    }
}
