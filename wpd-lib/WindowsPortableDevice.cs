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
using System.IO;
using System.Linq;
using WindowsPortableDevicesLib;

namespace WindowsPortableDevicesLib.Domain
{
    
    public class WindowsPortableDevice
    {
        private ILog l = LogManager.GetLogger(typeof(WindowsPortableDevice));
        
        private PortableDeviceClass device;
        
        public WindowsPortableDevice(string deviceID)
        {
            this.DeviceID = deviceID;
        }

        public string DeviceID { get; set; }

        public string FriendlyName { get; set; }

        public int DeviceType { get; set; }
        
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
            device.Open(DeviceID, clientInfo);

            GetPropertiesFromDevice();

        }

        private void GetPropertiesFromDevice()
        {
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
            string deviceName;
            propertyValues.GetStringValue(ref property, out deviceName);
            this.FriendlyName = deviceName;


            // Retrieve the type of device
            int deviceType;
            propertyValues.GetSignedIntegerValue(ref DevicePropertyKeys.WPD_DEVICE_TYPE, out deviceType);
            this.DeviceType = deviceType;
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
        
        public PortableDeviceFolder GetContents(PortableDeviceFolder parent = null)
        {
            ValidateConnection();
            var root = new PortableDeviceFolder("DEVICE", "DEVICE");
            if (parent != null)
            {
                root = parent;    
            }
            
            IPortableDeviceContent content;
            device.Content(out content);
            EnumerateContentsOfParent(ref content, root);

            return root;
        }
                        
        public void GetFile(PortableDeviceFile file, string saveToPath)
        {
            IPortableDeviceContent content;
            device.Content(out content);

            IPortableDeviceResources resources;
            content.Transfer(out resources);
            
            PortableDeviceApiLib.IStream wpdStream;
            uint optimalTransferSize = 0;

            var property = new _tagpropertykey();
            property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F,
                                      0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
            property.pid = 0;

            resources.GetStream(file.Id, ref property, 0, ref optimalTransferSize,
                                out wpdStream);

            System.Runtime.InteropServices.ComTypes.IStream sourceStream =
                (System.Runtime.InteropServices.ComTypes.IStream) wpdStream;

            var filename = Path.GetFileName(file.Id);
            FileStream targetStream = new FileStream(Path.Combine(saveToPath, filename),
                                                     FileMode.Create, FileAccess.Write);
            
            unsafe
            {
                var buffer = new byte[1024];
                int bytesRead;
                do
                {
                    sourceStream.Read(buffer, 1024, new IntPtr(&bytesRead));
                    targetStream.Write(buffer, 0, 1024);
                } while (bytesRead > 0);

                targetStream.Close();
            }
        }
        
        public void TransferContentToDevice(string fileName,
                                            string parentObjectId) {
            
            IPortableDeviceContent content;
            device.Content(out content);
            
            IPortableDeviceValues values =
                GetRequiredPropertiesForContentType(fileName, parentObjectId);
            
            PortableDeviceApiLib.IStream tempStream;
            uint optimalTransferSizeBytes = 0;
            content.CreateObjectWithPropertiesAndData(
                values,
                out tempStream,
                ref optimalTransferSizeBytes,
                null);
            
            System.Runtime.InteropServices.ComTypes.IStream targetStream =
                (System.Runtime.InteropServices.ComTypes.IStream) tempStream;
            
            try
            {
                using (var sourceStream =
                       new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[optimalTransferSizeBytes];
                    int bytesRead;
                    do
                    {
                        bytesRead = sourceStream.Read(
                            buffer, 0, (int)optimalTransferSizeBytes);
                        IntPtr pcbWritten = IntPtr.Zero;
                        targetStream.Write(
                            buffer, (int)optimalTransferSizeBytes, pcbWritten);
                    } while (bytesRead > 0);
                }
                targetStream.Commit(0);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(tempStream);
            }
        }
        
        private IPortableDeviceValues GetRequiredPropertiesForContentType(
            string fileName,
            string parentObjectId)
        {
            IPortableDeviceValues values =
                new PortableDeviceTypesLib.PortableDeviceValues() as IPortableDeviceValues;
            
            var WPD_OBJECT_PARENT_ID = new _tagpropertykey();
            WPD_OBJECT_PARENT_ID.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_PARENT_ID.pid = 3 ;
            values.SetStringValue(ref WPD_OBJECT_PARENT_ID, parentObjectId);

            FileInfo fileInfo = new FileInfo(fileName);
            var WPD_OBJECT_SIZE = new _tagpropertykey();
            WPD_OBJECT_SIZE.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_SIZE.pid = 11;
            values.SetUnsignedLargeIntegerValue(WPD_OBJECT_SIZE, (ulong) fileInfo.Length);

            var WPD_OBJECT_ORIGINAL_FILE_NAME = new _tagpropertykey();
            WPD_OBJECT_ORIGINAL_FILE_NAME.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_ORIGINAL_FILE_NAME.pid = 12;
            values.SetStringValue(WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(fileName));

            var WPD_OBJECT_NAME = new _tagpropertykey();
            WPD_OBJECT_NAME.fmtid =
                new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC,
                         0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
            WPD_OBJECT_NAME.pid = 4;
            values.SetStringValue(WPD_OBJECT_NAME, Path.GetFileName(fileName));
            
            return values;
        }
        
        public override string ToString() {
            
            bool shouldDisconnect = false;
            if(device == null) {
            
                Connect();
                shouldDisconnect = true;
            }
            string toString = FriendlyName;
            
            if(shouldDisconnect) {
                
                Disconnect();
            }
            
            return toString;
        }

        public PortableDeviceFolder GetFolder(string parentPersistentID, string persistentID)
        {

            IPortableDeviceContent content;
            device.Content(out content);

            // Get the properties of the object
            IPortableDeviceProperties properties;
            content.Properties(out properties);

            if (parentPersistentID == null)
            {
                parentPersistentID = "DEVICE";
            }

            // Enumerate the items contained by the current object
            IEnumPortableDeviceObjectIDs objectIds;
            content.EnumObjects(0, parentPersistentID, null, out objectIds);
            
            uint fetched = 0;
            do
            {
                string objectId;

                objectIds.Next(1, out objectId, ref fetched);
                if (fetched > 0)
                {
                    if (objectId.Equals(persistentID))
                    {

                        return (PortableDeviceFolder)WrapObject(properties, objectId);
                    }
                    

                }
            } while (fetched > 0);

            return null;
        }
        
        private static void EnumerateContentsRecursive(ref IPortableDeviceContent content,
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
                        EnumerateContentsRecursive(ref content, (PortableDeviceFolder)currentObject);
                    }
                }
            } while (fetched > 0);
        }

        private static void EnumerateContentsOfParent(ref IPortableDeviceContent content,
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
            
            string uniqueID;
            values.GetStringValue(DevicePropertyKeys.WPD_OBJECT_PERSISTENT_UNIQUE_ID, out uniqueID);
                        
            PortableDeviceObject deviceObject = null;

            if (contentType == folderType  || contentType == functionalType)
            {
                deviceObject = new PortableDeviceFolder(objectId, name);
                
            } else {
                deviceObject = new PortableDeviceFile(objectId, name);
            }
            
            deviceObject.PersistentId = uniqueID;
            
            return deviceObject;
            
        }

        
    }
}
