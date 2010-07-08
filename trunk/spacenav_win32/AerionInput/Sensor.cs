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
using HidLibrary;

namespace TDxInput
{
    /// <summary>
    /// Filter function delegate. If you want to define a new filter function,
    /// use this signature for it (input value x and maximum value max).
    /// Then process the input value through your function (e.g. f(x)=x^4)
    /// and return this value. Possible extensions can be an accumulation
    /// of function values and returning a median of the last 5 values for example.
    /// </summary>
    /// <param name="p_x"></param>
    /// <param name="p_max"></param>
    /// <returns></returns>
    public delegate double func(double p_x, double p_max);

    /// <summary>
    /// Represents the motion sensor of the device. Encapsulates movements and
    /// twists of the cap or ball of the device. Holds data in the AngleAxis
    /// (for Rotation) and Vector3D (for Translation) members which are accessible
    /// trough COM. Processes (accelerate and multiply with constant) raw Input
    /// data. Fires SensorInput event whenever new Input is detected.
    /// Implements the ISensor interface which exposes its methods to COM.
    /// Because of that the ClassInterfaceType must be "None", otherwise the CLR
    /// provides another class interface (and does not use IVector3D).
    /// The class is forced to use the same GUID as the original CoClass does
    /// in order to be accessible by binary-only clients.
    /// </summary>
    [
    Guid("85004B00-1AA7-4777-B1CE-8427301B942D"),
    ComSourceInterfaces(typeof(_ISensorEvents)),
    ClassInterface(ClassInterfaceType.None)
    ]
    public class Sensor : ISensor, IDisposable//, _ISensorEvents_Event
    {
        ////////////////////
        // Private fields //
        ////////////////////

        /// <summary>
        /// Maximum value for translation data used for scaling with filter function.
        /// THIS MUST BE ADJUSTED IF ANY DEVICE SENDS HIGHER VALUES!!
        /// At the time this value is far from optimal for 3DConnexion Devices but 
        /// seems to work properly. These devices offer 11 bit precison, however setting
        /// MaxValue to 2048 causes the sensor not to move. Will digg in that later.
        /// </summary>
        private const double c_MaxValueTranslation = 128.0;

        /// <summary>
        /// Maximum value for translation data used for scaling with filter function.
        /// THIS MUST BE ADJUSTED IF ANY DEVICE SENDS HIGHER VALUES!!
        /// At the time this value is far from optimal for 3DConnexion Devices but 
        /// seems to work properly. These devices offer 11 bit precison, however setting
        /// MaxValue to 2048 causes the sensor not to move. Will digg in that later.
        /// </summary>
        private const double c_MaxValueRotation = 128.0;

        /// <summary>
        /// Usage page for Translation Reports
        /// </summary>
        private const int c_Translation = 0x1;

        /// <summary>
        /// Usage page for Rotation Reports
        /// </summary>
        private const int c_Rotation = 0x2;

        private double m_Period;
        private double m_XScaleFactor = 1.0;
        private double m_YScaleFactor = 1.0;
        private double m_ZScaleFactor = 1.0;
        private double m_XRotScaleFactor = 1.0;
        private double m_YRotScaleFactor = 1.0;
        private double m_ZRotScaleFactor = 1.0;
        private double m_RotationThreshold = 0.0;
        private double m_TranslationThreshold = 0.0;

        private func m_TranslationFunction;
        private func m_RotationFunction;
        
        private Device m_Device;
        private Vector3D m_Translation = new Vector3D();
        private AngleAxis m_Rotation = new AngleAxis();

        /// <summary>
        /// IHidDevice object of HidLibrary which raises events when Hid Reports
        /// are recieved.
        /// </summary>
        private IHidDevice m_IHidDevice;

        /// <summary>
        /// IDisposeable implementation help variable.
        /// </summary>
        private bool IsDisposed = false;

        ////////////
        // Events //
        ////////////

        /// <summary>
        /// HidDataReceived event handler member variable storing the function pointer
        /// (delegate) to the method m_HidDevice will call back if a HidDataReceived
        /// is detected.
        /// </summary>
        private EventHandler<HidDataReceivedEventArgs> m_HidDeviceEvents_HidDataReceivedEventHandler;

        /// <summary>
        /// Binary semaphore for locking event access.
        /// </summary>
        private readonly object m_lock = new object();

         /// <summary>
        /// Member variable storing registered event handlers for SensorInput event.
        /// This is neccessary for a (hopefully) thread save event implementation. 
        /// </summary>
        private event _ISensorEvents_SensorInputEventHandler m_SensorInput;

        /// <summary>
        /// Overwritten add() and remove() handlers for SensorInput event which
        /// makes registering event handlers respect our member variable m_SensorInput.
        /// This is neccessary for a (hopefully) thread save event implementation.
        /// </summary>
        public event _ISensorEvents_SensorInputEventHandler SensorInput
        {
            add
            {
                lock (m_lock)
                {
                    m_SensorInput += value;
                }

            }
            remove
            {

                lock (m_lock)
                {
                    m_SensorInput -= value;
                }
            }
        }

        #region alternate implementation
        /*
        {
            add
            {
                if (SensorInputEvent != null)
                {
                    lock (SensorInputEvent)
                    {
                        SensorInputEvent += value;
                    }
                }
                else
                {
                    SensorInputEvent = new EventHandler(value);
                }
            }
            remove
            {
                if (SensorInputEvent != null)
                {
                    lock (SensorInputEvent)
                    {
                        SensorInputEvent -= value;
                    }
                }
            }
        }
        */
        #endregion

        ///////////////////////////
        // Getter/Setter methods //
        ///////////////////////////

        /// <summary>
        /// Gets or Sets the X translation scaling factor which is mulitiplied
        /// with the raw input data from the Sensor before passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double XScaleFactor
        {
            get { return m_XScaleFactor; }
            set { m_XScaleFactor = value; }
        }

        /// <summary>
        /// Gets or Sets the Y translation scaling factor which is mulitiplied
        /// with the raw input data from the Sensor before passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double YScaleFactor
        {
            get { return m_YScaleFactor; }
            set { m_YScaleFactor = value; }
        }

        /// <summary>
        /// Gets or Sets the Z translation scaling factor which is mulitiplied
        /// with the raw input data from the Sensor before passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double ZScaleFactor
        {
            get { return m_ZScaleFactor; }
            set { m_ZScaleFactor = value; }
        }

        /// <summary>
        /// Gets or Sets the X rotation scaling factor which is mulitiplied
        /// with the raw input data from the Sensor before passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double XRotScaleFactor
        {
            get { return m_XRotScaleFactor; }
            set { m_XRotScaleFactor = value; }
        }

        /// <summary>
        /// Gets or Sets the Y rotation scaling factor which is mulitiplied
        /// with the raw input data from the Sensor before passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double YRotScaleFactor
        {
            get { return m_YRotScaleFactor; }
            set { m_YRotScaleFactor = value; }
        }

        /// <summary>
        /// Gets or Sets the Z rotation scaling factor which is mulitiplied
        /// with the raw input data from the Sensor before passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double ZRotScaleFactor
        {
            get { return m_ZRotScaleFactor; }
            set { m_ZRotScaleFactor = value; }
        }

        /// <summary>
        /// Sets the Translation filter function which takes the raw input data
        /// from the Sensor and passes it to this function as argument.
        /// The result of this will be passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public functions TranslationFunction
        {
            set
            {
                m_TranslationFunction = set_Function(value);
            }
        }

        /// <summary>
        /// Sets the Rotation filter function which takes the raw input data
        /// from the Sensor and passes it to this function as argument.
        /// The result of this will be passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public functions RotationFunction
        {
            set
            {
                m_RotationFunction = set_Function(value);
            }
        }

        /// <summary>
        /// Gets or Sets the Translation Threshold. The Vector3D.Length attribute
        /// must be > TranslationThreshold to be passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double TranslationThreshold
        {
            get { return m_TranslationThreshold; }
            set { m_TranslationThreshold = value; }
        }

        /// <summary>
        /// Gets or Sets the Rotation Threshold. The AngleAxis.Angle attribute
        /// must be > RotationThreshold to be passed to the client.
        /// This property is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public double RotationThreshold
        {
            get { return m_RotationThreshold; }
            set { m_RotationThreshold = value; }
        }

        /// <summary>
        /// Gets the time frame the values of Translation and Rotation are scaled to.
        /// </summary>
        public double Period
        {
            get { return m_Period; }
        }

        /// <summary>
        /// Gets lengths of movement for all 3 axes.
        /// </summary>
        public Vector3D Translation
        {
            get { return m_Translation; }
        }

        /// <summary>
        /// Gets angles for all 3 axes.
        /// </summary>
        public AngleAxis Rotation
        {
            get { return m_Rotation; }
        }

        /// <summary>
        /// Gets the parent Device object associated with the Sensor.
        /// </summary>
        public object Device
        {
            get { return m_Device; }
        }

        /////////////////////
        // Private methods //
        /////////////////////

        /// <summary>
        /// Converts numeric function types to function pointers for filter processing.
        /// Used to convert the enum type coming from the XML config to delegate.
        /// </summary>
        private func set_Function(functions p_function)
        {
            switch (p_function)
            {
                case (functions.sin):
                    return sin;
                case (functions.square):
                    return square;
                case (functions.cube):
                    return cube;
                default:
                    return none;
            }
        }

        /// <summary>
        /// Handles HidDataReceived events and processes incoming raw data from the
        /// Device. Fills m_Translation and m_Rotation objects with the filtered and
        /// scaled raw data. At the end fire SensorInput to notify clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="p_HidDataReceivedEventArgs"></param>
        private void HidDataReceived(object sender, HidDataReceivedEventArgs p_HidDataReceivedEventArgs)
        {
            short x = 0;
            short y = 0;
            short z = 0;

            ///////////////////////////////
            // set Length of translation //
            // movement to 0 zeroes also //
            // x, y and z components     //
            ///////////////////////////////
            m_Translation.Length = 0;

            ///////////////////////////////
            // set Angle of rotation     //
            // movement to 0 zeroes also //
            // x, y and z components     //
            ///////////////////////////////
            m_Rotation.Angle = 0;

            ///////////////////////////////
            // handling the translation  //
            ///////////////////////////////
            if (p_HidDataReceivedEventArgs[0] == c_Translation)
            {
                try
                {
                    byte[] buffer = {
                        p_HidDataReceivedEventArgs[1],
                        p_HidDataReceivedEventArgs[2],
                        p_HidDataReceivedEventArgs[3],
                        p_HidDataReceivedEventArgs[4],
                        p_HidDataReceivedEventArgs[5],
                        p_HidDataReceivedEventArgs[6],
                    };

                    System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, 0, buffer.Length);
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);

                    x = reader.ReadInt16();
                    y = reader.ReadInt16();
                    z = reader.ReadInt16();

                    /*
                    Console.Out.WriteLine("transX: {0}", x);
                    Console.Out.WriteLine("transY: {0}", y);
                    Console.Out.WriteLine("transZ: {0}", z);
                    */

                    //////////////////////////////////////// 
                    // Filtering and scaling is done here //
                    // X value of Aerion Input data must  //
                    // be multiplied with -1              //
                    ////////////////////////////////////////
                    m_Translation.X = m_TranslationFunction(System.Convert.ToDouble(x), c_MaxValueTranslation) * XScaleFactor;
                    m_Translation.Y = m_TranslationFunction(System.Convert.ToDouble(y), c_MaxValueTranslation) * YScaleFactor;
                    m_Translation.Z = m_TranslationFunction(System.Convert.ToDouble(z), c_MaxValueTranslation) * ZScaleFactor;
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Exception: {0}, {1}, {2}", e.GetType(), e.Message, e.StackTrace);
                }
            }


            ///////////////////////////
            // handling the rotation //
            ///////////////////////////
            else if (p_HidDataReceivedEventArgs[0] == c_Rotation)
            {
                try
                {
                    byte[] buffer = {
                        p_HidDataReceivedEventArgs[1],
                        p_HidDataReceivedEventArgs[2],
                        p_HidDataReceivedEventArgs[3],
                        p_HidDataReceivedEventArgs[4],
                        p_HidDataReceivedEventArgs[5],
                        p_HidDataReceivedEventArgs[6],
                    };

                    System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, 0, buffer.Length);
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);

                    x = reader.ReadInt16();
                    y = reader.ReadInt16();
                    z = reader.ReadInt16();

                    /*
                    Console.Out.WriteLine("rotX: {0}", x);
                    Console.Out.WriteLine("rotY: {0}", y);
                    Console.Out.WriteLine("rotZ: {0}", z);
                    */

                    //////////////////////////////////////// 
                    // Filtering and scaling is done here //
                    ////////////////////////////////////////
                    m_Rotation.X =  m_RotationFunction(System.Convert.ToDouble(x), c_MaxValueRotation) * XRotScaleFactor; //Rotation um X-Achse = rot
                    m_Rotation.Y =  m_RotationFunction(System.Convert.ToDouble(y), c_MaxValueRotation) * YRotScaleFactor; //Rotation um Y-Achse = gr�n
                    m_Rotation.Z =  m_RotationFunction(System.Convert.ToDouble(z), c_MaxValueRotation) * ZRotScaleFactor; //Rotation um Z-Achse = blau
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Exception: {0}, {1}, {2}", e.GetType(), e.Message, e.StackTrace);
                }
                #region old stuff...
                /*
                _xRot = (ushort)(p_HidDataReceivedEventArgs[1]);
                _xRot += (ushort)(p_HidDataReceivedEventArgs[2] << 8);
                xRot = (short)(_xRot);

                m_Rotation.X = xRot * XRotScaleFactor;
                //Console.Out.WriteLine("rotX: {0}", rotX);

                _yRot = (ushort)(p_HidDataReceivedEventArgs[3]);
                _yRot += (ushort)(p_HidDataReceivedEventArgs[4] << 8);
                yRot = (short)(_yRot);

                m_Rotation.Y = yRot * YRotScaleFactor;
                //Console.Out.WriteLine("rotY: {0}", rotY);

                _zRot = (ushort)(p_HidDataReceivedEventArgs[5]);
                _zRot += (ushort)(p_HidDataReceivedEventArgs[6] << 8);
                zRot = (short)(_zRot);

                m_Rotation.Z = zRot * ZRotScaleFactor;
                //Console.Out.WriteLine("angle_internal: {0}", m_Rotation.Angle);
                */
                #endregion
            }
            ///////////////////////////////////////
            // Fire SensorInput iff              //
            // Angle > RotationThreshold         //
            // and Length > TranslationThreshold //
            ///////////////////////////////////////
            if ((m_Rotation.Angle > m_RotationThreshold) || m_Translation.Length > m_TranslationThreshold)
            {
                // Scale down Ticks
                m_Period = Convert.ToDouble(System.DateTime.Now.Ticks)/1000000000 - m_Period;

                // Fire event thread save
                _ISensorEvents_SensorInputEventHandler SensorEventHandler = null;

                lock (m_lock)
                {
                    SensorEventHandler = m_SensorInput;
                    if (SensorEventHandler != null)
                    {
                        SensorEventHandler();
                    }
                }
                #region old stuff
                /*
                if (SensorInput != null)
                {
                    lock (m_lock)
                    {
                        //SensorInput(this, new EventArgs());
                        SensorInput();
                        
                    }
                    
                } 
                */
                #endregion
            }
    }

        //////////////////////////////////////////////
        // Filter or "acceleration" functions which //
        // can be used for translation and rotation //
        //////////////////////////////////////////////

        /// <summary>        
        /// Null filter. Output = Input.
        /// </summary>
        /// <param name="p_x">input value</param>
        /// <param name="p_max">maximum value the function is scaled to</param>
        /// <returns></returns>
        private double none(double p_x, double p_max)
        {
            return p_x;
        }

        /// <summary>
        /// Sinus filter. Output = sin(Input) [Shifted to origin]
        /// </summary>
        /// <param name="p_x">input value</param>
        /// <param name="p_max">maximum value the function is scaled to</param>
        /// <returns></returns>
        private double sin(double p_x, double p_max)
        {
            return Math.Sin((p_x * Math.PI / 2) / p_max) * p_max;
        }

        /// <summary>
        /// Sqaure filter. Output = Input*Input [Shifted to origin]
        /// </summary>
        /// <param name="p_x">input value</param>
        /// <param name="p_max">maximum value the function is scaled to</param>
        /// <returns></returns>
        private double square(double p_x, double p_max)
        {
            return ((p_x / p_max) * (p_x / p_max) * Math.Sign(p_x)) * p_max;
        }

        /// <summary>
        /// Cube filter. Output = Input*Input*Input [Shifted to origin]
        /// </summary>
        /// <param name="p_x">input value</param>
        /// <param name="p_max">maximum value the function is scaled to</param>
        /// <returns></returns>
        private double cube(double p_x, double p_max)
        {
            return ((p_x / p_max) * (p_x / p_max) * (p_x / p_max)) * p_max;
        }


        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// Binds the HidDataReceivedEvent to HidDataReceived() and sets parent
        /// Device object.
        /// This method is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public void InitSensor(Device p_Device, IHidDevice p_IHidDevice)
        {
            m_Device = p_Device;
            m_HidDeviceEvents_HidDataReceivedEventHandler = new EventHandler<HidDataReceivedEventArgs>(HidDataReceived);
            m_IHidDevice = p_IHidDevice;
            m_IHidDevice.DataReceived += m_HidDeviceEvents_HidDataReceivedEventHandler;
        }

        /// <summary>
        /// Unbinds the HidDataReceivedEvent from HidDataReceived().
        /// This method is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public void DestroySensor()
        {
            m_IHidDevice.DataReceived -= m_HidDeviceEvents_HidDataReceivedEventHandler;
        }

        /// <summary>
        /// Constructor which inits the TranslationFunction and RotationFunction attributes.
        /// </summary>
        public Sensor()
        {            
            //m_Rotation.Sensor = this;
            //m_Translation.Sensor = this;
            m_TranslationFunction = set_Function(functions.none);
            m_RotationFunction = set_Function(functions.none);
        }

        /// <summary>
        /// Destructor supporting IDisposeable implementation.
        /// </summary>
        ~Sensor()
        {
            Dispose(false);
        }

        //////////////////////////
        // IDisposeable Members //
        //////////////////////////

        /// <summary>
        /// Suppress garbage collection of this object, take care of it yourself.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up managed and unmanged ressources by hand.
        /// </summary>
        /// <param name="Disposing"></param>
        protected void Dispose(bool Disposing)
        {
            if (!IsDisposed)
            {
                if (Disposing)
                {
                    //Clean Up managed resources
                    m_IHidDevice = null;
                    m_Device = null;

                    //Ensure we clean-up references to event
                    m_SensorInput = null;
                }
                //Clean up unmanaged resources

            }
            IsDisposed = true;
        }
    }
}