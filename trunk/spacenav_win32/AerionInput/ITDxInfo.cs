/*
   Interface for the TDxInfo class
 
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
    /// COM interface exposing the methods of TDxInfo.
    /// Forced to use the same GUID as the original driver interface does.
    /// Provides both interface implementations (Dual), IDispatch and IUnknown
    /// for early and late binding.
    /// </summary>
    [
    Guid("00612962-8FB6-47B2-BF98-4E8C0FF5F559"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)
    ]
    public interface ITDxInfo
    {
        /// <summary>
        /// Returns the revision number of the driver.
        /// As usual in a COM environment the stings are of type BSTR,
        /// so marshal strings that way.
        /// The DispId attribute states the vtable entry of the method.
        /// </summary>
        [return: MarshalAs(UnmanagedType.BStr)]
        [DispId(1)]
        string RevisionNumber();

        #region correct oo implementation of RevisionNumber
        /*
        string RevisionNumber
        {
            get;
        }
        */
        #endregion
    }
}
