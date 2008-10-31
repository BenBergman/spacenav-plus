/*
   Interface for the Vector3D class
 
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
    /// COM interface exposing the methods of Vector3D.
    /// Forced to use the same GUID as the original driver interface does.
    /// Provides both interface implementations (Dual), IDispatch and IUnknown
    /// for early and late binding.
    /// </summary>
    [
    Guid("8C2AA71D-2B23-43F5-A6ED-4DF57E9CD8D5"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)
    ]
    public interface IVector3D
    {
        /// <summary>
        /// Gets or sets the x component of the vector.
        /// The DispId attribute states the vtable entry of the method.
        /// param: In states that the argument will be passed to the structure.
        /// </summary>
        [DispId(1)]
        double X
        {
            [DispId(1)]
            get;
            [param: In]
            [DispId(1)]
            set;
        }

        /// <summary>
        /// Gets or sets the y component of the vector.
        /// The DispId attribute states the vtable entry of the method.
        /// param: In states that the argument will be passed to the structure.
        /// </summary>
        [DispId(2)]
        double Y
        {
            [DispId(2)]
            get;
            [param: In]
            [DispId(2)]
            set;
        }

        /// <summary>
        /// Gets or sets the z component of the vector.
        /// The DispId attribute states the vtable entry of the method.
        /// param: In states that the argument will be passed to the structure.
        /// </summary>
        [DispId(3)]
        double Z
        {
            [DispId(3)]
            get;
            [param: In]
            [DispId(3)]
            set;
        }

        /// <summary>
        /// Gets or sets the euclidean length of the vector.
        /// The DispId attribute states the vtable entry of the method.
        /// param: In states that the argument will be passed to the structure.
        /// </summary>
        [DispId(4)]
        double Length
        {
            [DispId(4)]
            get;
            [param: In]
            [DispId(4)]
            set;
        }
    }
}
