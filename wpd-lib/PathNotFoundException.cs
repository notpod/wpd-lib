using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsPortableDevicesLib.Domain
{
    public class PathNotFoundException : Exception
    {

        public PathNotFoundException(string message, params object[] parameters) :base(String.Format(message, parameters))
        {

        }
    }
}
