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
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace HidLibrary
{
    class DeviceInfoHandle : SafeHandleMinusOneIsInvalid
    {
        private DeviceInfoHandle() : base(true) { }

        protected override bool ReleaseHandle()
        {
            return Convert.ToBoolean(NativeApi.SetupDiDestroyDeviceInfoList(handle));
        }
    }

	sealed partial class NativeApi
	{

        #region SPDRP Definitions

        const uint SPDRP_DEVICEDESC = 0x00000000; // DeviceDesc (R/W)
        const uint SPDRP_HARDWAREID = 0x00000001; // HardwareID (R/W)
        const uint SPDRP_COMPATIBLEIDS = 0x00000002; // CompatibleIDs (R/W)
        const uint SPDRP_UNUSED0 = 0x00000003; // unused
        const uint SPDRP_SERVICE = 0x00000004; // Service (R/W)
        const uint SPDRP_UNUSED1 = 0x00000005; // unused
        const uint SPDRP_UNUSED2 = 0x00000006; // unused
        const uint SPDRP_CLASS = 0x00000007; // Class (R--tied to ClassGUID)
        const uint SPDRP_CLASSGUID = 0x00000008; // ClassGUID (R/W)
        const uint SPDRP_DRIVER = 0x00000009; // Driver (R/W)
        const uint SPDRP_CONFIGFLAGS = 0x0000000A; // ConfigFlags (R/W)
        const uint SPDRP_MFG = 0x0000000B; // Mfg (R/W)
        const uint SPDRP_FRIENDLYNAME = 0x0000000C; // FriendlyName (R/W)
        const uint SPDRP_LOCATION_INFORMATION = 0x0000000D; // LocationInformation (R/W)
        const uint SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E; // PhysicalDeviceObjectName (R)
        const uint SPDRP_CAPABILITIES = 0x0000000F; // Capabilities (R)
        const uint SPDRP_UI_NUMBER = 0x00000010; // UiNumber (R)
        const uint SPDRP_UPPERFILTERS = 0x00000011; // UpperFilters (R/W)
        const uint SPDRP_LOWERFILTERS = 0x00000012; // LowerFilters (R/W)
        const uint SPDRP_BUSTYPEGUID = 0x00000013; // BusTypeGUID (R)
        const uint SPDRP_LEGACYBUSTYPE = 0x00000014; // LegacyBusType (R)
        const uint SPDRP_BUSNUMBER = 0x00000015; // BusNumber (R)
        const uint SPDRP_ENUMERATOR_NAME = 0x00000016; // Enumerator Name (R)
        const uint SPDRP_SECURITY = 0x00000017; // Security (R/W, binary form)
        const uint SPDRP_SECURITY_SDS = 0x00000018; // Security (W, SDS form)
        const uint SPDRP_DEVTYPE = 0x00000019; // Device Type (R/W)
        const uint SPDRP_EXCLUSIVE = 0x0000001A; // Device is exclusive-access (R/W)
        const uint SPDRP_CHARACTERISTICS = 0x0000001B; // Device Characteristics (R/W)
        const uint SPDRP_ADDRESS = 0x0000001C; // Device Address (R)
        const uint SPDRP_UI_NUMBER_DESC_FORMAT = 0x0000001D;  // UiNumberDescFormat (R/W)
        const uint SPDRP_DEVICE_POWER_DATA = 0x0000001E;  // Device Power Data (R)
        const uint SPDRP_REMOVAL_POLICY = 0x0000001F;  // Removal Policy (R)
        const uint SPDRP_REMOVAL_POLICY_HW_DEFAULT = 0x00000020;  // Hardware Removal Policy (R)
        const uint SPDRP_REMOVAL_POLICY_OVERRIDE = 0x00000021;  // Removal Policy Override (RW)
        const uint SPDRP_INSTALL_STATE = 0x00000022;  // Device Install State (R)
        const uint SPDRP_MAXIMUM_PROPERTY = 0x00000023;  // Upper bound on ordinals

        #endregion

		// API declarations relating to device management (SetupDixxx and
		// RegisterDeviceNotification functions).
		
		// ******************************************************************************
		// API constants
		// ******************************************************************************
		
		// from dbt.h
		public const int DBT_DEVICEARRIVAL = 0x8000;
		public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
		public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
		public const int DBT_DEVTYP_HANDLE = 6;
		public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
		public const int WM_DEVICECHANGE = 0x219;
		
		// from setupapi.h
		public const short DIGCF_PRESENT = 0x00000002;
		public const short DIGCF_DEVICEINTERFACE = 0x00000010;
		
		// ******************************************************************************
		// Structures and classes for API calls, listed alphabetically
		// ******************************************************************************
		
		// There are two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.
		
		// Use this in the call to RegisterDeviceNotification() and
		// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure.
		[StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_DEVICEINTERFACE
		{
			public int dbcc_size;
			public int dbcc_devicetype;
			public int dbcc_reserved;
			public Guid dbcc_classguid;
			public short dbcc_name;
		}
		
		// Use this to read the dbcc_name string and classguid.
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        public class DEV_BROADCAST_DEVICEINTERFACE_1
		{
			public int dbcc_size;
			public int dbcc_devicetype;
			public int dbcc_reserved;
			public Guid dbcc_classguid;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=255)]public char[] dbcc_name;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HANDLE
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
			public int dbch_handle;
			public int dbch_hdevnotify;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
		{
			public int dbch_size;
			public int dbch_devicetype;
			public int dbch_reserved;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
		{
			public int cbSize;
			public System.Guid InterfaceClassGuid;
			public int Flags;
			public int Reserved;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public int cbSize;
			public string DevicePath;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
		{
			public int cbSize;
			public System.Guid ClassGuid;
			public int DevInst;
			public int Reserved;
		}
		
		// ******************************************************************************
		// API functions, listed alphabetically
		// ******************************************************************************
		
        [DllImport("setupapi.dll", SetLastError = true)]
        static public extern int SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, int hwndParent);

        [DllImport("setupapi.dll", SetLastError = true)]
        static public extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        static public extern bool SetupDiEnumDeviceInterfaces(DeviceInfoHandle DeviceInfoSet, int DeviceInfoData, ref System.Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static public extern DeviceInfoHandle SetupDiGetClassDevs(ref System.Guid ClassGuid, string Enumerator, int hwndParent, int Flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static public extern bool SetupDiGetDeviceInterfaceDetail(DeviceInfoHandle DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static public extern bool SetupDiGetDeviceRegistryProperty(
            [In] DeviceInfoHandle DeviceInfoSet,
            [In] ref SP_DEVINFO_DATA DeviceInfoData,
            [In] int Property,
            [Out] IntPtr PropertyRegDataType,
            [Out] IntPtr PropertyBuffer,
            [In] int PropertyBufferSize,
            [Out, In] ref int RequiredSize
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static public extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

        [DllImport("user32.dll", SetLastError = true)]
        static public extern bool UnregisterDeviceNotification(IntPtr Handle);

		
	}
	
}
