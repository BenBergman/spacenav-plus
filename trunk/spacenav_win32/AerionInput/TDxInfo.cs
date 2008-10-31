/*
   Device class implementation
 
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
    /// Provides information about the used Revision of the driver.
    /// Implements the ITDxInfo interface which exposes its methods to COM.
    /// Because of that the ClassInterfaceType must be "None", otherwise the CLR
    /// provides another class interface (and does not use ITDxInfo).
    /// The class is forced to use the same GUID as the original CoClass does
    /// in order to be accessible by binary-only clients.
    /// </summary>
    [
    Guid("1A960ECE-0E57-4A68-B694-8373114F1FF4"),
    ClassInterface(ClassInterfaceType.None)
    ]
    public class TDxInfo : ITDxInfo
    {
        ////////////////////
        // Private fields //
        ////////////////////

        private const string c_RevisionNumber = "0.0.1";

        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// Returns the revision number of the driver.
        /// It is not implemented as a classic getter method in the
        /// original driver but as a simple constant return function.
        /// </summary>
        /// <returns>constant string c_RevisionNumber</returns>
        public string RevisionNumber()
        {
            return c_RevisionNumber;
        }

        /// <summary>
        /// TDxInfo constructor
        /// </summary>
        public TDxInfo()
        {

        }
    }
}