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
using System.Collections;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace HidLibrary
{
    
    public class HidControl : UserControl
    {
        private static HidControl instance = new HidControl();

        public event EventHandler DeviceChange;
        public event EventHandler<HidLogEventArgs> LogMessage;

        public static HidControl GetInstance()
        {
            return instance;
        }

        private HidControl()
        {
            this.Visible = false;

            NativeApi.HidD_GetHidGuid(ref HidGuid);

            EnumerateDevices();

            InitializeDeviceNotification();
        }

        public IHidDevice GetDeviceByPredicate(Predicate<IHidDeviceInfo> CheckoutPredicate)
        {
            foreach (HidDevice HidDev in devices.Values)
            {
                if (CheckoutPredicate(HidDev as IHidDeviceInfo))
                {
                    EmitLogMessage(HidLogMessageType.Info, "Device checked out: " + HidDev.DevicePath);

                    return HidDev as IHidDevice;
                }
            }

            return null;
        }

        public IEnumerable<IHidDevice> GetDevicesByPredicate(Predicate<IHidDeviceInfo> CheckoutPredicate)
        {
            foreach (HidDevice HidDev in devices.Values)
            {
                if (CheckoutPredicate(HidDev as IHidDeviceInfo))
                {
                    EmitLogMessage(HidLogMessageType.Info, "Device checked out: " + HidDev.DevicePath);

                    yield return HidDev as IHidDevice;
                }
            }
        }

        public void GiveBackDevice(IHidDevice HidDev)
        {
            if (HidDev != null)
            {
                try
                {
                    HidDev.StopInPipeThread();
                }
                finally
                {
                    HidDev = null;
                }
    
                EmitLogMessage(HidLogMessageType.Info, "Device checked in.");
            }

        }

        private new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Guid HidGuid;

        private Dictionary<string, HidDevice> devices = new Dictionary<string,HidDevice>();

        private IntPtr deviceNotificationHandle = IntPtr.Zero;

        private bool disposed = false;

        internal void EmitLogMessage(HidLogMessageType type, String message)
        {
            if (LogMessage != null) Invoke(LogMessage, new object[]{this, new HidLogEventArgs(type, message)});
        }

        private void InitializeDeviceNotification()
        {
            NativeApi.DEV_BROADCAST_DEVICEINTERFACE DevBroadcastDeviceInterface = new NativeApi.DEV_BROADCAST_DEVICEINTERFACE();

            try
            {

                IntPtr DevBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(DevBroadcastDeviceInterface));

                DevBroadcastDeviceInterface.dbcc_size = Marshal.SizeOf(DevBroadcastDeviceInterface);
                DevBroadcastDeviceInterface.dbcc_devicetype = NativeApi.DBT_DEVTYP_DEVICEINTERFACE;
                DevBroadcastDeviceInterface.dbcc_reserved = 0;
                DevBroadcastDeviceInterface.dbcc_classguid = this.HidGuid;

                Marshal.StructureToPtr(DevBroadcastDeviceInterface, DevBroadcastDeviceInterfaceBuffer, true);

                deviceNotificationHandle = NativeApi.RegisterDeviceNotification(this.Handle, DevBroadcastDeviceInterfaceBuffer, NativeApi.DEVICE_NOTIFY_WINDOW_HANDLE);

                Marshal.FreeHGlobal(DevBroadcastDeviceInterfaceBuffer);

                if (deviceNotificationHandle.Equals(IntPtr.Zero)) throw new Exception();
            }
            catch
            {
                EmitLogMessage(HidLogMessageType.Warning, "Registering for Device Notifications failed!");
            }
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == NativeApi.WM_DEVICECHANGE)
                {
                    HandleDeviceChange(m); 
                }
            }
            finally
            {
                base.WndProc(ref m);
            }
        }

        private void HandleDeviceChange(Message m)
        {
            if (m.WParam.ToInt32() == NativeApi.DBT_DEVICEARRIVAL)
            {
                NativeApi.DEV_BROADCAST_HDR DevBroadcastHeader = new NativeApi.DEV_BROADCAST_HDR();
                NativeApi.DEV_BROADCAST_DEVICEINTERFACE_1 DevBroadcastDeviceInterface = new NativeApi.DEV_BROADCAST_DEVICEINTERFACE_1();

                try
                {
                    Marshal.PtrToStructure(m.LParam, DevBroadcastHeader);

                    if (DevBroadcastHeader.dbch_devicetype == NativeApi.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        int StringSize = System.Convert.ToInt32((DevBroadcastHeader.dbch_size - 28) / 2);
                        DevBroadcastDeviceInterface.dbcc_name = new char[StringSize + 1];
                        Marshal.PtrToStructure(m.LParam, DevBroadcastDeviceInterface);
                        string DeviceNameString = new string(DevBroadcastDeviceInterface.dbcc_name, 0, StringSize);

                        DeviceNameString = DeviceNameString.Substring(0, DeviceNameString.IndexOf('}') + 1);
                        DeviceNameString = DeviceNameString.ToLowerInvariant();

                        AddDeviceToList(DeviceNameString);
                    }
                }
                catch
                {
                    EmitLogMessage(HidLogMessageType.Warning, "Error on arrival!");
                }

                
            }
            else if (m.WParam.ToInt32() == NativeApi.DBT_DEVICEREMOVECOMPLETE)
            {
                NativeApi.DEV_BROADCAST_HDR DevBroadcastHeader = new NativeApi.DEV_BROADCAST_HDR();
                NativeApi.DEV_BROADCAST_DEVICEINTERFACE_1 DevBroadcastDeviceInterface = new NativeApi.DEV_BROADCAST_DEVICEINTERFACE_1();
                string DeviceNameString;

                try
                {
                    Marshal.PtrToStructure(m.LParam, DevBroadcastHeader);

                    if (DevBroadcastHeader.dbch_devicetype == NativeApi.DBT_DEVTYP_DEVICEINTERFACE)
                    {
                        int StringSize = System.Convert.ToInt32((DevBroadcastHeader.dbch_size - 28) / 2);
                        DevBroadcastDeviceInterface.dbcc_name = new char[StringSize + 1];
                        Marshal.PtrToStructure(m.LParam, DevBroadcastDeviceInterface);
                        DeviceNameString = new string(DevBroadcastDeviceInterface.dbcc_name, 0, StringSize);

                        DeviceNameString = DeviceNameString.Substring(0, DeviceNameString.IndexOf('}')+1);
                        DeviceNameString = DeviceNameString.ToLowerInvariant();

                        RemoveDeviceFromList(DeviceNameString);
                    }
                }
                catch
                {
                    EmitLogMessage(HidLogMessageType.Warning, "Error on removal!");
                }
            }
        }

        private void RemoveDeviceFromList(string DeviceNameString)
        {
            devices[DeviceNameString].Dispose();
            devices[DeviceNameString] = null;
            devices.Remove(DeviceNameString);

            EmitLogMessage(HidLogMessageType.Info, "Device Removal: " + DeviceNameString);

            if (DeviceChange != null) DeviceChange.Invoke(this, null);
        }

        private void AddDeviceToList(string DeviceNameString)
        {
            devices.Add(DeviceNameString.ToLowerInvariant(), new HidDevice(this, DeviceNameString));

            EmitLogMessage(HidLogMessageType.Info, "Device Attach: " + DeviceNameString);

            if (DeviceChange != null) DeviceChange.Invoke(this, null);
        }

        private void EnumerateDevices()
        {
            if (devices.Count > 0)
            {
                foreach (HidDevice dev in devices.Values)
                {
                    dev.Dispose();
                }
            }

            devices.Clear();

            // Platform Device Enumeration

            int index = 0;
            NativeApi.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData;

            using (DeviceInfoHandle DeviceHandle = NativeApi.SetupDiGetClassDevs(ref HidGuid, null, 0,
                NativeApi.DIGCF_DEVICEINTERFACE | NativeApi.DIGCF_PRESENT))
            {

                DeviceInterfaceData = new NativeApi.SP_DEVICE_INTERFACE_DATA();
                DeviceInterfaceData.cbSize = Marshal.SizeOf(DeviceInterfaceData);

                while (NativeApi.SetupDiEnumDeviceInterfaces(DeviceHandle, 0, ref HidGuid, index++, ref DeviceInterfaceData))
                {
                    string devPath = GetInterfacePath(ref DeviceInterfaceData, DeviceHandle);
                    AddDeviceToList(devPath);
                }
            }
        }

        private string GetInterfacePath(ref NativeApi.SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, DeviceInfoHandle DeviceHandle)
        {
            int requiredSize = 0;

            NativeApi.SetupDiGetDeviceInterfaceDetail(DeviceHandle, ref DeviceInterfaceData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero);

            IntPtr DetailDataBuffer = Marshal.AllocHGlobal(requiredSize);
            Marshal.WriteInt32(DetailDataBuffer, 4 + Marshal.SystemDefaultCharSize);

            NativeApi.SetupDiGetDeviceInterfaceDetail(DeviceHandle, ref DeviceInterfaceData, DetailDataBuffer, requiredSize, ref requiredSize, IntPtr.Zero);

            IntPtr pDevPath = new IntPtr(DetailDataBuffer.ToInt32() + 4);

            String devPath = Marshal.PtrToStringAuto(pDevPath);

            Marshal.FreeHGlobal(DetailDataBuffer);

            return devPath;
        }

        protected new void Dispose(bool disposing)
        {
            if(!this.disposed)
            {

                if(disposing)
                {
                    foreach (HidDevice HidDev in devices.Values)
                    {
                        HidDev.Dispose();
                    }
                }

                devices.Clear();

                disposed = true;
            }

            base.Dispose();
        }

        ~HidControl()
        {
            Dispose(false);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // HidControl
            // 
            this.Name = "HidControl";
            this.ResumeLayout(false);
            this.Visible = false;

        }

    }

    public enum HidLogMessageType
    {
        Info,
        Warning,
        Critical
    }

    public class HidLogEventArgs : EventArgs
    {
        private HidLogMessageType type;
        private String message;

        internal HidLogEventArgs(HidLogMessageType type, String message)
        {
            this.message = message;
            this.type = type;
        }

        public String Message
        {
            get { return message; }
        }

        public HidLogMessageType Type
        {
            get { return type; }
        }

    }
    
}
