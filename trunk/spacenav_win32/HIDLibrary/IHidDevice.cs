using System;

namespace HidLibrary
{
    public interface IHidDevice
    {
        string VendorName { get; }
        string ProductName { get; }
        string SerialNumber { get; }

        int VendorID { get; }
        int ProductID { get; }
        int Version { get; }

        int NumInputBuffers { get; set; }

        event EventHandler<HidDataReceivedEventArgs> DataReceived;

        int GetReportLength(HidReportType type);

        void WriteOutputReport(HidReport report);
        HidReport ReadInputReport();
        HidReport ReadFeatureReport();
        void WriteFeatureReport(HidReport report);

        void WriteOutPipeReport(HidReport report);

        HidReport GetEmptyReport(HidReportType type);

        string GetDeviceString(int idx);

        void StartInPipeThread();
        void StopInPipeThread();
    }
}