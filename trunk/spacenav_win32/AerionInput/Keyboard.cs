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
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using HidLibrary;

namespace TDxInput
{
    #region old stuff...
    /*
    /// <summary>
    /// vermutlich �berfl�ssige angepasste EventArgs-Klasse
    /// </summary>
    [ComVisible(false)]
    class KeyboardEventArgs : EventArgs
    {
        private int m_Keycode;

        public KeyboardEventArgs(int keycode)
        {
            m_Keycode = keycode;
        }

        public int Keycode
        {
            get { return m_Keycode; }
        }
    }
     */
    #endregion

    /// <summary>
    /// Represents the keys of the device. Holds boolean array with the state of all keys.
    /// Fires KeyUp and KeyDown events whenever Keys are pressed and released.
    /// Implements the IKeyboard interface which exposes its methods to COM.
    /// Because of that the ClassInterfaceType must be "None", otherwise the CLR
    /// provides another class interface (and does not use IVector3D).
    /// The class is forced to use the same GUID as the original CoClass does
    /// in order to be accessible by binary-only clients.
    /// </summary>
    [
    Guid("25BBE090-583A-4903-A61B-D0EC629AC4EC"),
    ComSourceInterfaces(typeof(_IKeyboardEvents)),
    ClassInterface(ClassInterfaceType.None)
    ]
    public class Keyboard : IKeyboard, IDisposable //, _IKeyboardEvents_Event
    {
        ////////////////////
        // Private fields //
        ////////////////////

        /// <summary>
        /// Usage page for Button Reports
        /// </summary>
        private const int c_Button = 0x3;

        private int m_Keys;
        private int m_ProgrammableKeys;
        private string[] m_KeyNameList;
        private string[] m_KeyLabelList;
        private Device m_Device;

        /// <summary>
        /// BitArray containing one bit (0 = up, 1 = down) for each key of the device.
        /// </summary>
        private BitArray m_KeyDownList;
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
        /// Member variable storing registered event handlers for KeyDown event.
        /// This is neccessary for a (hopefully) thread save event implementation. 
        /// </summary>
        private event _IKeyboardEvents_KeyDownEventHandler m_KeyDown;

        /// <summary>
        /// Member variable storing registered event handlers for KeyUp event.
        /// This is neccessary for a (hopefully) thread save event implementation. 
        /// </summary>
        private event _IKeyboardEvents_KeyUpEventHandler m_KeyUp;

        /// <summary>
        /// Overwritten add() and remove() handlers for KeyDown event which
        /// makes registering event handlers respect our member variable m_KeyDown.
        /// This is neccessary for a (hopefully) thread save event implementation.
        /// </summary>
        public event _IKeyboardEvents_KeyDownEventHandler KeyDown
        {
            add
            {
                lock (m_lock)
                {
                    m_KeyDown += value;
                }

            }
            remove
            {

                lock (m_lock)
                {
                    m_KeyDown -= value;
                }
            }

        }

        /// <summary>
        /// Overwritten add() and remove() handlers for KeyUp event which
        /// makes registering event handlers respect our member variable m_KeyUp.
        /// This is neccessary for a (hopefully) thread save event implementation.
        /// </summary>
        public event _IKeyboardEvents_KeyUpEventHandler KeyUp
        {
            add
            {
                lock (m_lock)
                {
                    m_KeyUp += value;
                }

            }
            remove
            {

                lock (m_lock)
                {
                    m_KeyUp -= value;
                }
            }

        }

        ///////////////////////////
        // Getter/Setter methods //
        ///////////////////////////

        /// <summary>
        /// Returns number of keys the device offers.
        /// </summary>
        public int Keys
        {
            get { return m_Keys; }
        }

        /// <summary>
        /// Returns the parent Device object associated with the Sensor.
        /// </summary>
        public object Device
        {
            get { return m_Device; }
        }

        /// <summary>
        /// Returns number of programmable keys the device offers.
        /// </summary>
        public int ProgrammableKeys
        {
            get { return m_ProgrammableKeys; }
        }

        /////////////////////
        // Private methods //
        /////////////////////

        /// <summary>
        /// Handles HidDataReceived events and processes incoming raw data from the
        /// Device. Fills BitArray m_KeyDownList with key press data.
        /// At the end fire KeyDown or KeyUp to notify clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="p_HidDataReceivedEventArgs"></param>
        private void HidDataReceived(object sender, HidDataReceivedEventArgs p_HidDataReceivedEventArgs)
        {
            ////////////////////////////
            // handling the buttons ////
            ////////////////////////////
            if (p_HidDataReceivedEventArgs[0] == c_Button)
            {
                try
                {
                    byte[] buffer = {
                        p_HidDataReceivedEventArgs[1],
                        
                        // Needed for more keys than 8
                        // uncomment as much bytes as you have keys / 8

                        /*
                        p_HidDataReceivedEventArgs[2],                        
                        p_HidDataReceivedEventArgs[3],
                        p_HidDataReceivedEventArgs[4],
                        p_HidDataReceivedEventArgs[5],
                        p_HidDataReceivedEventArgs[6],
                        */
                    };
                    // s. a.
                    /*
                    System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer, 0, buffer.Length);
                    System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);

                    byte tmp = reader.ReadByte();
                    */

                    BitArray bufferArray = new BitArray(buffer);

                    ///////////////////////////////
                    // Step through each key and //
                    // check if it has changed   //
                    ///////////////////////////////
                    for (int keycode = 0; keycode < m_Keys; keycode++)
                    {
                        // Fire event iff state of the button has changed
                        if (bufferArray[keycode] ^ m_KeyDownList[keycode])
                        {
                            // Fire KeyDown
                            if (bufferArray[keycode])
                            {
                                // Fire event thread save
                                _IKeyboardEvents_KeyDownEventHandler KeyDownEventHandler = null;

                                lock (m_lock)
                                {
                                    KeyDownEventHandler = m_KeyDown;
                                    if (KeyDownEventHandler != null)
                                    {
                                        KeyDownEventHandler(keycode);
                                        Console.WriteLine("KeyDown fired for {0}", keycode);
                                    }
                                }
                                #region old stuff
                                /*
                                lock (KeyDown)
                                {
                                    KeyDown(keycode);
                                    //Console.WriteLine("KeyDown fired for {0}", keycode);
                                }
                                 */
                                #endregion
                            }
                            // Fire KeyUp
                            else if (!(bufferArray[keycode]))
                            {
                                // Fire event thread save
                                _IKeyboardEvents_KeyUpEventHandler KeyUpEventHandler = null;

                                lock (m_lock)
                                {
                                    KeyUpEventHandler = m_KeyUp;
                                    if (KeyUpEventHandler != null)
                                    {
                                        KeyUpEventHandler(keycode);
                                        Console.WriteLine("KeyUp fired for {0}", keycode);
                                    }
                                }
                                #region old stuff
                                /*
                                lock (KeyUp)
                                {
                                    KeyUp(keycode);
                                    //Console.WriteLine("KeyUp fired for {0}", keycode);
                                }
                                 */ 
                                #endregion
                            }
                        }
                    }

                    // Update KeyDownList
                    m_KeyDownList = bufferArray;                    
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Exception: {0}, {1}, {2}", e.GetType(), e.Message, e.StackTrace);
                }
            }
        }

        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// Returns the Name of the supplied key, if any.
        /// </summary>
        public string GetKeyName(int key)
        {
            //what is key <0 good for? (damn, i wrote this code and I don't know...)
            //if ((0 < key) & (key < m_KeyNameList.Length)) return m_KeyNameList[key];
            if (key < m_KeyNameList.Length) return m_KeyNameList[key];
            else return null;
        }

        /// <summary>
        /// Returns the Label of the supplied key, if any.
        /// </summary>
        public string GetKeyLabel(int key)
        {
            //what is key <0 good for? (damn, i wrote this code and I don't know...)
            //if ((0 < key) & (key < m_KeyLabelList.Length)) return m_KeyLabelList[key];
            if (key < m_KeyLabelList.Length) return m_KeyLabelList[key];
            else return null;
        }

        /// <summary>
        /// Returns true if the supplied key is up at the time.
        /// </summary>
        public bool IsKeyUp(int key)
        {
            if ((0 < key) & (key < m_Keys)) return !(m_KeyDownList[key]);
            else return false;
        }

        /// <summary>
        /// Returns true if the supplied key is pressed down at the time.
        /// </summary>
        public bool IsKeyDown(int key)
        {
            if ((0 < key) & (key < m_Keys)) return m_KeyDownList[key];
            else return false;
        }

        /// <summary>
        /// Binds the HidDataReceivedEvent to HidDataReceived() and sets parent
        /// Device object.
        /// TODO: Send feature report to device to query number of keys.
        /// This method is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public void InitKeyboard(Device p_Device, IHidDevice p_IHidDevice)
        {

            m_Device = p_Device;
            m_HidDeviceEvents_HidDataReceivedEventHandler = new EventHandler<HidDataReceivedEventArgs>(HidDataReceived);
            m_IHidDevice = p_IHidDevice;
            m_IHidDevice.DataReceived += m_HidDeviceEvents_HidDataReceivedEventHandler;

            m_Keys = 2;
            m_ProgrammableKeys = 0;

            #region Feature Report??
            //////////////////////////////////////////////
            // hier muss vom Aerion abgefragt werden, wieviele Tasten er hat
            // und der InitKeyboard entsprechend aufgerufen werden.
            // irgendwie so:
            // HidReport m_HIDFeatureReport = p_IHidDevice.ReadFeatureReport();
            /*
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

                m_keys = Convert.ToInt64(reader.ReadInt16());
                bool status = reader.ReadBoolean();

            }

            catch (Exception e)
            {
                Console.Out.WriteLine("Exception: {0}", e.GetType());
            }
            */
            #endregion

            m_KeyDownList = new BitArray(8, false);
            m_KeyNameList = new string[m_Keys];
            m_KeyLabelList = new string[m_Keys];
        
            m_KeyNameList[0] = "L";
            m_KeyNameList[1] = "R";
            m_KeyLabelList[0] = "L";
            m_KeyLabelList[1] = "R";
        }

        /// <summary>
        /// Unbinds the HidDataReceivedEvent from HidDataReceived().
        /// This method is an extension of the orginal API, which is not exposed to COM.
        /// Thatswhy the Attribute ComVisible(false).
        /// </summary>
        [ComVisible(false)]
        public void DestroyKeyboard()
        {
            m_IHidDevice.DataReceived -= m_HidDeviceEvents_HidDataReceivedEventHandler;
        }

        /// <summary>
        /// Default constructor needed by COM to create objects.
        /// </summary>
        public Keyboard()
        {

        }

        /// <summary>
        /// Destructor supporting IDisposeable implementation.
        /// </summary>
        ~Keyboard()
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
                    m_KeyUp = null;
                    m_KeyDown = null;
                }
                //Clean up unmanaged resources

            }
            IsDisposed = true;
        }
    }
}