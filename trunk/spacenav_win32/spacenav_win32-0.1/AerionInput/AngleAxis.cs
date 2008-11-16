/*
   AngeAxis class implementation
 
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
    /// Provides rotational data in three dimensions
    /// and an angle in arbitrary units (euclidean norm).
    /// Implements the IAngleAxis interface which exposes its methods to COM.
    /// Because of that the ClassInterfaceType must be "None", otherwise the CLR
    /// provides another class interface (and does not use IAngleAxis).
    /// The class is forced to use the same GUID as the original CoClass does
    /// in order to be accessible by binary-only clients.
    /// </summary>
    [
    Guid("512A6C3E-3010-401B-8623-E413E2ACC138"),
    ClassInterface(ClassInterfaceType.None)
    ]
    public class AngleAxis : IAngleAxis
    {
        ////////////////////
        // Private fields //
        ////////////////////

        private double m_X;
        private double m_Y;
        private double m_Z;
        private double m_Angle;
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
        /// Represents the X component of the rotation
        /// </summary>
        public double X
        {
            get { return m_X; }
            set
            {
                m_X = value;
                m_Angle = Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z);
            }
        }

        /// <summary>
        /// Represents the Y component of the rotation
        /// </summary>
        public double Y
        {
            get { return m_Y; }
            set
            {
                m_Y = value;
                m_Angle = Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z);
            }
        }

        /// <summary>
        /// Represents the Z component of the rotation
        /// </summary>
        public double Z
        {
            get { return m_Z; }
            set 
            {
                m_Z = value;
                m_Angle = Math.Sqrt(m_X * m_X + m_Y * m_Y + m_Z * m_Z);
            }
        }

        /// <summary>
        /// Represents the angle of the rotation (euclidean length of the three
        /// components). If set to 0.0 all three components are set to 0.0, too.
        /// </summary>
        public double Angle
        {
            get { return m_Angle; }
            ///////////////////////////////////////
            // valid values: 0.0
            // if Angle == 0.0 zero out x, y, z
            ///////////////////////////////////////
            set
            {
                if (value == 0.0)
                {
                    m_X = 0.0;
                    m_Y = 0.0;
                    m_Z = 0.0;
                    m_Angle = 0.0;
                }
            }
        }


        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// AngleAxis constructor
        /// </summary>
        public AngleAxis() 
        {

        }
    }
}