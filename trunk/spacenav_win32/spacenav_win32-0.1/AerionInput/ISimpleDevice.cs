/*
   Interface for the Device class
 
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
    /// COM interface exposing the methods of Device.
    /// Forced to use the same GUID as the original driver interface does.
    /// Provides both interface implementations (Dual), IDispatch and IUnknown
    /// for early and late binding.
    /// </summary>
    [
    Guid("CB3BF65E-0816-482A-BB11-64AF1E837812"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)
    ]
    public interface ISimpleDevice
    {
        /// <summary>
        /// Tries to find connected devices. For proper
        /// operation this method must be called before getting data from the
        /// device.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(1)]
        void Connect();

        /// <summary>
        /// Disconnects already connected devices.
        /// Before the client application shuts down, this method should be
        /// called in order to disconnect event handlers and save application 
        /// profiles.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(2)]
        void Disconnect();

        /// <summary>
        /// Returns an interface pointer to the associated Sensor object.
        /// For COM interop this must be marshaled explicitly to Interface.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(3)]
        Sensor Sensor
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            [DispId(3)]
            get;
        }

        /// <summary>
        /// Returns an interface pointer to the associated Keyboard object.
        /// For COM interop this must be marshaled explicitly to Interface.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(4)]
        Keyboard Keyboard
        {
            [DispId(4)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
        }

        /// <summary>
        /// Returns type of the connected device.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(5)]
        int Type
        {
            [DispId(5)]
            get;
        }

        /// <summary>
        /// Returns true if a device is connected with the driver.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(6)]
        bool IsConnected
        {
            [DispId(6)]
            get;
        }

        /// <summary>
        /// Loads the application profile in argument preferencesName.
        /// As usual in a COM environment the stings are of type BSTR,
        /// so marshal strings that way.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(7)]
        void LoadPreferences([In, MarshalAs(UnmanagedType.BStr)] string preferencesName);
    }
}
