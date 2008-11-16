/*
   Interface for the Keyboard class
 
   Copyright (C) Christian Bayer 2008

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
    /// COM interface exposing the methods of Keyboard.
    /// Forced to use the same GUID as the original driver interface does.
    /// Provides both interface implementations (Dual), IDispatch and IUnknown
    /// for early and late binding.
    /// </summary>
    [
    Guid("D6F968E7-2993-48D7-AF24-8B602D925B2C"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)
    ]
    public interface IKeyboard
    {
        /// <summary>
        /// Returns number of keys the device offers.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(1)]
        int Keys
        {
            [DispId(1)]
            get;
        }

        /// <summary>
        /// Returns number of programmable keys the device offers.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(2)]
        int ProgrammableKeys
        {
            [DispId(2)]
            get;
        }

        /// <summary>
        /// Returns the Label of the supplied key, if any.
        /// As usual in a COM environment the stings are of type BSTR,
        /// so marshal strings that way.
        /// "In" states that the argument will be passed to the structure.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(3)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetKeyLabel([In] int key);

        /// <summary>
        /// Returns the Name of the supplied key, if any.
        /// As usual in a COM environment the stings are of type BSTR,
        /// so marshal strings that way.
        /// "In" states that the argument will be passed to the structure.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(4)]
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetKeyName([In] int key);

        /// <summary>
        /// Returns an IDispatch pointer to the parent Device object
        /// associated with the Sensor.
        /// For COM interop compatibility, the object must be marshaled to
        /// IDispatch. (See Blender Plugin)
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(5)]
        object Device
        {
            [return: MarshalAs(UnmanagedType.IDispatch)]
            [DispId(5)]
            get;
        }

        /// <summary>
        /// Returns true if the supplied key is pressed down at the time.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(6)]
        bool IsKeyDown([In]int key);

        /// <summary>
        /// Returns true if the supplied key is up at the time.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        bool IsKeyUp([In]int key);
    }
}
