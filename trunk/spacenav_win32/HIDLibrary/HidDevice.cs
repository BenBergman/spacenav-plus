/*
   HidLibrary.Net   http://sourceforge.net/projects/hidlibrary/
 
   Copyright (C)    Roman Reichel 2006
					Bauhaus University of Weimar

   This library is free software; you can redistribute it and/or
   modify it under the terms of the GNU Lesser General Public
   License as published by the Free Software Foundation; either
   version 2.1 of the License, or (at your option) any later version.

   This library is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
   Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public
   License along with this library; if not, see http://www.gnu.org/licenses .

*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HidLibrary
{
    internal class DeviceAccess
    {
        private String filePath;
        private SafeFileHandle fileHandle;
        private DeviceMode currentMode = DeviceMode.Closed;

        private enum DeviceMode
        {
            Write,
            Read,
            Ioctl,
            Closed
        }

        public bool OpenDeviceForWrite()
        {
            if (currentMode != DeviceMode.Write)
            {
                if (currentMode != DeviceMode.Closed) fileHandle.Close();
                fileHandle = NativeApi.CreateFile(FilePath, FileAccess.Write, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            }
            if (fileHandle.IsInvalid)
                return false;
            return true;
        }

        public bool OpenDeviceForRead()
        {
            if (currentMode != DeviceMode.Read)
            {
                if (currentMode != DeviceMode.Closed) fileHandle.Close();
                fileHandle = NativeApi.CreateFile(FilePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            }
            if (fileHandle.IsInvalid)
                return false;
            return true;
        }

        public bool OpenDeviceForIoctl()
        {
            if (currentMode != DeviceMode.Ioctl)
            {
                if (currentMode != DeviceMode.Closed) fileHandle.Close();
                fileHandle = NativeApi.CreateFile(FilePath, 0, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            }
            if (fileHandle.IsInvalid)
                return false;
            return true;
        }

        public void CloseDevice()
        {
            if (currentMode != DeviceMode.Closed) fileHandle.Close();
        }

        public DeviceAccess(String FilePath)
        {
            filePath = FilePath;
        }

        public SafeFileHandle Handle
        {
            get { return fileHandle; }
        }

        public string FilePath
        {
            get { return filePath; }
        }
    }

    internal class HidDevice : IDisposable, IEquatable<HidDevice>, IHidDevice, IHidDeviceInfo
    {
        private HidControl controller;

        private DeviceAccess dev;

        private DeviceReadThread ReadThread;

        public event EventHandler<HidDataReceivedEventArgs> DataReceived;
        
        private NativeApi.HIDD_ATTRIBUTES Attributes;
        private NativeApi.HIDP_CAPS Capabilities;

        public bool Equals(HidDevice other)
        {
            return (other.DevicePath == this.DevicePath);
        }

        private void InputPipeReportHandler(byte[] report)
        {
            HidDataReceivedEventArgs ea = new HidDataReceivedEventArgs(report);

            try
            {
                if (DataReceived != null) controller.Invoke(DataReceived, new object[] { this, ea });
            }
            catch { }
        }

        public string VendorName
        {
            get
            {
                StringBuilder buf = new StringBuilder(256);
                if (dev.OpenDeviceForIoctl())
                {
                    NativeApi.HidD_GetManufacturerString(dev.Handle, buf, buf.Capacity);
                }
                return buf.ToString();
            }
        }

        public string ProductName
        {
            get
            {
                StringBuilder buf = new StringBuilder(256);
                if (dev.OpenDeviceForIoctl())
                {
                    NativeApi.HidD_GetProductString(dev.Handle, buf, buf.Capacity);
                }
                return buf.ToString();
            }
        }

        public string SerialNumber
        {
            get
            {
                StringBuilder buf = new StringBuilder(256);
                if (dev.OpenDeviceForIoctl())
                {
                    NativeApi.HidD_GetSerialNumberString(dev.Handle, buf, buf.Capacity);
                }
                return buf.ToString();
            }
        }

        public int VendorID
        {
            get
            {
                return Attributes.VendorID;
            }
        }

        public int ProductID
        {
            get
            {
                return Attributes.ProductID;
            }
        }

        public int Version
        {
            get
            {
                return Attributes.VersionNumber;
            }
        }

        public String DevicePath
        {
            get
            {
                return dev.FilePath;
            }
        }

        public int GetReportLength(HidReportType type)
        {
            switch (type)
            {
                case HidReportType.Input:
                    return Capabilities.InputReportByteLength;
                case HidReportType.Output:
                    return Capabilities.OutputReportByteLength;
                case HidReportType.Feature:
                    return Capabilities.FeatureReportByteLength;
                default:
                    return 0;
            }
        }

        public HidReport GetEmptyReport(HidReportType type)
        {
            byte[] buf = new byte[GetReportLength(type)];
 
            switch (type)
            {
                case HidReportType.Input:
                    return new HidInReport(buf);
                case HidReportType.Output:
                    return new HidOutReport(buf);
                case HidReportType.Feature:
                    return new HidFeatureReport(buf);
                default:
                    return null;
            }
        }

        public string GetDeviceString(int idx)
        {
            StringBuilder buf = new StringBuilder(256);
            if (dev.OpenDeviceForIoctl())
            {
                NativeApi.HidD_GetIndexedString(dev.Handle, idx, buf, buf.Capacity);
            }
            return buf.ToString();
        }

        public int NumInputBuffers
        {
            get
            {
                int tmp = 0;
                if (dev.OpenDeviceForIoctl())
                {
                    NativeApi.HidD_GetNumInputBuffers(dev.Handle, ref tmp);
                }
                return tmp;
            }

            set
            {
                if (dev.OpenDeviceForIoctl())
                {
                    NativeApi.HidD_SetNumInputBuffers(dev.Handle, value);
                }
            }
        }

        public void StartInPipeThread()
        {
            if (ReadThread != null) ReadThread.Start();
        }

        public void StopInPipeThread()
        {
            if (ReadThread != null) ReadThread.Stop();
        }

        public void WriteOutputReport(HidReport report)
        {
            if (dev.OpenDeviceForWrite())
            {
                if (report.Length == GetReportLength(HidReportType.Output))
                {
                    if (!NativeApi.HidD_SetOutputReport(dev.Handle, report.Bytes, report.Length))
                    {
                        throw new Exception("SetOutputReport: " + Marshal.GetLastWin32Error().ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Unable to open device for Write");
            }
        }

        public HidReport ReadInputReport()
        {
            byte[] buf = new byte[GetReportLength(HidReportType.Input)];
            HidReport report = new HidInReport(buf);

            if (dev.OpenDeviceForRead())
            {
                if (report.Length == GetReportLength(HidReportType.Input))
                {
                    if (!NativeApi.HidD_GetInputReport(dev.Handle, report.Bytes, report.Length))
                    {
                        throw new Exception("GetInputReport: " + Marshal.GetLastWin32Error().ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Unable to open device for Read");
            }

            return report;
        }

        public HidReport ReadFeatureReport()
        {
            byte[] buf = new byte[GetReportLength(HidReportType.Feature)];
            HidReport report = new HidFeatureReport(buf);

            if (dev.OpenDeviceForRead())
            {
                if (report.Length == GetReportLength(HidReportType.Feature))
                {
                    if (!NativeApi.HidD_GetFeature(dev.Handle, report.Bytes, report.Length))
                    {
                        throw new Exception("GetFeatureReport: " + Marshal.GetLastWin32Error().ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Unable to open device for Read");
            }

            return report;
        }

        public void WriteFeatureReport(HidReport report)
        {
            if (dev.OpenDeviceForWrite())
            {
                if (report.Length == GetReportLength(HidReportType.Feature))
                {
                    if (!NativeApi.HidD_SetFeature(dev.Handle, report.Bytes, report.Length))
                    {
                        throw new Exception("SetFeatureReport: " + Marshal.GetLastWin32Error().ToString());
                    }
                }
            }
            else
            {
                throw new Exception("Unable to open device for Write");
            }
        }

        public void WriteOutPipeReport(HidReport report)
        {
            if (dev.OpenDeviceForWrite())
            {
                if (report.Length == GetReportLength(HidReportType.Output))
                {
                    using (FileStream stream = new FileStream(dev.Handle, FileAccess.Write))
                    {
                        try
                        {
                            stream.Write(report.Bytes, 0, report.Bytes.Length);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Write to outpipe failed: " + e.Message);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Unable to open device for Write");
            }
        }

        internal HidDevice(HidControl control, string devicePath)
        {
            controller = control;

            dev = new DeviceAccess(devicePath);

            if (dev.OpenDeviceForIoctl())
            {
                NativeApi.HidD_GetAttributes(dev.Handle, ref Attributes);

                using (HidPreparsedData preparsedData = new HidPreparsedData(dev.Handle))
                {
                    NativeApi.HidP_GetCaps(preparsedData.Handle, ref Capabilities);
                }

                if (dev.OpenDeviceForRead())
                {
                    ReadThread = new DeviceReadThread(new DeviceAccess(devicePath), new ReceivedReportDelegate(InputPipeReportHandler));
                }
                else
                {
                    ReadThread = null;
                }
            }
            else
            {
                throw new Exception("Unable to access device!");
            }
        }

        ~HidDevice()
        {
            this.Dispose(false);
        }

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                StopInPipeThread();

                if (disposing)
                {
                    if (ReadThread != null) ReadThread.Dispose();
                }

                dev.CloseDevice();

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                

                // Note disposing has been done.
                disposed = true;

            }
        }
    }

    class HidPreparsedData : IDisposable
    {
        IntPtr handle;

        public IntPtr Handle
        {
            get
            {
                return handle;
            }
        }

        public HidPreparsedData(SafeFileHandle FileHandle)
        {
            if (!NativeApi.HidD_GetPreparsedData(FileHandle, ref handle))
                throw new Exception("PreparsedData: " + Marshal.GetLastWin32Error().ToString());
        }

        #region IDisposable Members

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (!NativeApi.HidD_FreePreparsedData(ref handle))
                        throw new Exception();
                    handle = IntPtr.Zero;
                }
            }
            disposed = true;
        }

        ~HidPreparsedData()
        {
            Dispose(false);
        }

        #endregion
    }

    internal class CanNotWriteException : Exception { }

}
