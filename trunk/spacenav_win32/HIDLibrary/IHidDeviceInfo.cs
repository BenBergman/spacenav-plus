using System;
using System.Collections.Generic;
using System.Text;

namespace HidLibrary
{
    public interface IHidDeviceInfo
    {
        string VendorName { get; }
        string ProductName { get; }
        string SerialNumber { get; }

        int VendorID { get; }
        int ProductID { get; }
        int Version { get; }

        string DevicePath { get; }
    }
}
