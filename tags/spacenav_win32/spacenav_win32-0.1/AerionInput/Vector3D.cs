/*
   Vector3D class implementation
 
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
    /// Provides translational data in three dimensions
    /// and a length (euclidean norm).
    /// Implements the IVector3D interface which exposes its methods to COM.
    /// Because of that the ClassInterfaceType must be "None", otherwise the CLR
    /// provides another class interface (and does not use IVector3D).
    /// The class is forced to use the same GUID as the original CoClass does
    /// in order to be accessible by binary-only clients.
    /// </summary>
    [
    Guid("740A7479-C7C1-44DA-8A84-B5DE63C78B32"),
    ClassInterface(ClassInterfaceType.None)
    ]
    public class Vector3D : IVector3D
    {
        ////////////////////
        // Private fields //
        ////////////////////

        private double m_X;
        private double m_Y;
        private double m_Z;
        private double m_Length;
        #region unused references to parent sensor
        /*
        private ISensor m_Sensor;

        
        
        [ComVisible(false)]
        public ISensor Sensor
        {
            //get { return Sensor; }
            set { m_Sensor = value; }
        }
        */
        #endregion

        ///////////////////////////
        // Getter/Setter methods //
        ///////////////////////////

        /// <summary>
        /// Represents the X component of the translation.
        /// </summary>
        public double X
        {
            get { return m_X; }
            set
            {
                m_X = value;
                m_Length = Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z);
            }
        }

        /// <summary>
        /// Represents the Y component of the translation.
        /// </summary>
        public double Y
        {
            get { return m_Y; }
            set
            {
                m_Y = value;
                m_Length = Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z);
            }
        }

        /// <summary>
        /// Represents the Z component of the translation.
        /// </summary>
        public double Z
        {
            get { return m_Z; }
            set
            {
                m_Z = value;
                m_Length = Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z);
            }
        }

        /// <summary>
        /// Represents the length of the translation (euclidean length of the
        /// three components). If set to 0.0 all three components are set to 0.0,
        /// if set to 1.0 the components are scaled to be a unit vector.
        /// </summary>
        public double Length
        {
            get { return m_Length; }

            ///////////////////////////////////////
            // valid values: 1.0, 0.0            //
            // norm the vector if length == 1.0  //
            // if length == 0.0 zero out x, y, z //
            ///////////////////////////////////////
            set
            {
                if (value == 0.0)
                {
                    m_X = 0.0;
                    m_Y = 0.0;
                    m_Z = 0.0;
                    m_Length = 0.0;
                }
                else if (value == 1.0)
                {
                    m_X /= m_Length;
                    m_Y /= m_Length;
                    m_Z /= m_Length;
                    m_Length = value;
                }
            }
        }

        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// Vector3D constructor
        /// </summary>
        public Vector3D()
        {

        }
    }
}