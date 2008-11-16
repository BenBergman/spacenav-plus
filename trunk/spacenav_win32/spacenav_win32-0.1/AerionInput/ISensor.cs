/*
   Interface for the Sensor class
 
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
    /// COM interface exposing the methods of Sensor.
    /// Forced to use the same GUID as the original driver interface does.
    /// Provides both interface implementations (Dual), IDispatch and IUnknown
    /// for early and late binding.
    /// </summary>
    [
    Guid("F3A6775E-6FA1-4829-BF32-5B045C29078F"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)
    ]
    public interface ISensor
    {
        /// <summary>
        /// Holds lengths of movement for all 3 axes.
        /// For COM interop this must be marshaled explicitly to Interface.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(1)]
        Vector3D Translation
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            [DispId(1)]
            get;
        }

        /// <summary>
        /// Holds angles for all 3 axes.
        /// For COM interop this must be marshaled explicitly to Interface.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(2)]
        AngleAxis Rotation
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            [DispId(2)]
            get;
        }

        /// <summary>
        /// Returns an IDispatch pointer to the parent Device object
        /// associated with the Sensor.
        /// For COM interop compatibility, the object must be marshaled to
        /// IDispatch. (See Blender Plugin)
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(3)]
        object Device
        {
            [return: MarshalAs(UnmanagedType.IDispatch)]
            [DispId(3)]
            get;
        }

        /// <summary>
        /// Holds the time frame the values of Translation and Rotation are scaled to.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [DispId(4)]
        double Period
        {
            [DispId(4)]
            get;
        }
    }
}
