using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace HIDLibrary
{
    internal class DeviceNotFoundException : Exception { }

    public class HidDeviceController
    {
        private static HidDeviceController instance = null;

        public static HidDeviceController Instance()
        {
            if (instance == null)
            {
                instance = new HidDeviceController();
            }
            return instance;
        }

        private HidDeviceController()
        {
            NativeApi.HidD_GetHidGuid(ref HidGuid);

            DevList = new List<HidDevice>();

            EnumerateDevices();

        }

        ~HidDeviceController()
        {
            foreach (HidDevice HidDev in DevList)
            {
                HidDev.Dispose();
            }
        }

        private Guid HidGuid;

        public event EventHandler OnDeviceChange;

        public event EventHandler<HidDataReceivedEventArgs> OnDataReceived;
        
        private List<HidDevice> DevList;
        
        private void EnumerateDevices()
        {
            List<HidDevice> NewList = new List<HidDevice>();

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
                    int requiredSize = 0;
                    
                    NativeApi.SetupDiGetDeviceInterfaceDetail(DeviceHandle, ref DeviceInterfaceData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero);

                    IntPtr DetailDataBuffer = Marshal.AllocHGlobal(requiredSize);
                    Marshal.WriteInt32(DetailDataBuffer, 4 + Marshal.SystemDefaultCharSize);

                    NativeApi.SetupDiGetDeviceInterfaceDetail(DeviceHandle, ref DeviceInterfaceData, DetailDataBuffer, requiredSize, ref requiredSize, IntPtr.Zero);

                    IntPtr pDevPath = new IntPtr(DetailDataBuffer.ToInt32() + 4);

                    String devPath = Marshal.PtrToStringAuto(pDevPath);

                    
                    try
                    {
                        NewList.Add(new HidDevice(devPath));
                    }
                    catch { }

                    Marshal.FreeHGlobal(DetailDataBuffer);
                }
            }

            // Maintain Existing Devices
            foreach (HidDevice HidDev in NewList)
            {
                if (DevList.Exists(new Predicate<HidDevice>(HidDev.Equals)))
                {
                    NewList.RemoveAll(new Predicate<HidDevice>(HidDev.Equals));
                    NewList.Add(DevList.Find(new Predicate<HidDevice>(HidDev.Equals)));
                }
                else
                {
                    HidDev.IsCheckedOut = false;
                }
            }

            // Swap Device Lists
            DevList = NewList;

            //OnDeviceChange(this, new EventArgs());
        }

        public void CheckIn(ref IHidDevice HidDev)
        {
            ((HidDevice)HidDev).StopDeviceReadThread();
            ((HidDevice)HidDev).DisposePreparsedData();
            ((HidDevice)HidDev).CloseFileHandle();
            ((HidDevice)HidDev).IsCheckedOut = false;
        }

        public IHidDevice CheckOut(Predicate<IHidDevice> pred)
        {
            foreach (HidDevice HidDev in DevList)
            {
                if (pred((IHidDevice)HidDev))
                {
                    HidDev.IsCheckedOut = true;
                    HidDev.OnDataReceived += new EventHandler<HidDataReceivedEventArgs>(HidDev_OnDataReceived);
                    HidDev.StartDeviceReadThread();
                    return (IHidDevice)HidDev;
                }
            }
            throw new DeviceNotFoundException();
        }

        void HidDev_OnDataReceived(object sender, HidDataReceivedEventArgs e)
        {
            OnDataReceived(sender, e);
        }
    }
}
