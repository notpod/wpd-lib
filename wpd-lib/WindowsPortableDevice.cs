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
    }
}
