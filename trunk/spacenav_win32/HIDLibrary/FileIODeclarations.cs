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
using System.IO;

namespace HidLibrary
{
	
	partial class NativeApi
	{
		
		// API declarations relating to file I/O.
		
		// ******************************************************************************
		// API constants
		// ******************************************************************************
		
        public const uint FILE_FLAG_OVERLAPPED      = 0x40000000;
		public const uint OPEN_EXISTING            = 3;
        /*
		public const int WAIT_TIMEOUT               = 0x102;
		public const short WAIT_OBJECT_0            = 0;
         */
		
		// ******************************************************************************
		// Structures and classes for API calls, listed alphabetically
		// ******************************************************************************
		
		[StructLayout(LayoutKind.Sequential)]
        public struct OVERLAPPED
		{
			public int Internal;
			public int InternalHigh;
			public int Offset;
			public int OffsetHigh;
			public int hEvent;
		}
		
		[StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public int lpSecurityDescriptor;
			public int bInheritHandle;
		}
		
		// ******************************************************************************
		// API functions, listed alphabetically
		// ******************************************************************************

        [DllImport("kernel32.dll", SetLastError = true)]
        static public extern int CancelIo(SafeFileHandle hFile);
		
		[DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError = true)]
        static public extern SafeFileHandle CreateFile(string lpFileName, 
            [MarshalAs(UnmanagedType.U4)]
            FileAccess dwDesiredAccess, 
            [MarshalAs(UnmanagedType.U4)]
            FileShare dwShareMode, 
            IntPtr lpSecurityAttributes, 
            [MarshalAs(UnmanagedType.U4)]
            FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)]
            uint dwFlagsAndAttributes, 
            IntPtr hTemplateFile);
        /*
		[DllImport("kernel32.dll")]
        static public extern bool ReadFile(SafeFileHandle hFile, ref byte lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, ref OVERLAPPED lpOverlapped);
		
		[DllImport("kernel32.dll", SetLastError = true)]
        static public extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);
        */
	}
	
}
