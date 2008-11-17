/*
   Events interface for the Device class
 
   Copyright (C)    Christian Bayer 2008
					Friedrich-Schiller University of Jena
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
using System.Runtime.InteropServices;

namespace TDxInput
{
    /// <summary>
    /// Delegate declaration for DeviceChange event handler. Only states the return 
    /// type of the event which is void for COM events and the signature of the event.
    /// This is implicitly COM visible and needed for COM to determine the signature
    /// of the event.
    /// </summary>
    /// <param name="reserved">Not yet defined functionality.</param>
    public delegate void _ISimpleDeviceEvents_DeviceChangeEventHandler(int reserved);

    /// <summary>
    /// COM Events publisher interface forced to use
    /// the same GUID as the original driver interface does.
    /// Must be IDispatch Interface for late binding, not standard dual.
    /// </summary>
    [
    Guid("8FE3A216-E235-49A6-9136-F9D81FDADEF5"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)
    ]
    public interface _ISimpleDeviceEvents
    {
        /// <summary>
        /// The DeviceChange event is fired whenever a device is plugged in
        /// or pulled off. The reserved argument has no use for now.
        /// </summary>
        /// <param name="reserved"></param>
        [DispId(1)]
        void DeviceChange([In] int reserved);

        //event _ISimpleDeviceEvents_DeviceChangeEventHandler DeviceChange;
    }
}
