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

//TODO Windows Vista 64 und Windows 7 64 Test

namespace TDxInput
{
    #region old stuff...
    /*
    /// <summary>
    /// wahrscheinlich überflüssige angepasste DeviceChangeEventArgs
    /// </summary>
    [ComVisible (false)]
    public class DeviceChangeEventArgs : EventArgs
    {
        private int m_Reserved;

        public DeviceChangeEventArgs(int reserved)
        {
            m_Reserved = reserved;
        }

        public int reserved
        {
            get { return m_Reserved; }
        }
    }

    /// [ProgId("TDxInput.Device")]
    /// [ClassInterface(ClassInterfaceType.AutoDual)]
     */
    #endregion

    /// <summary>
    /// Main device class representing the entire NDOF device
    /// encapsulating Keyboard and Sensor.
    /// Indirectly implements the _ISimpleDeviceEvents_Event interface via the
    /// ComSourceInterfaces attribute in order to properly expose events to COM.
    /// Implements the IDisposeable interface which helps to properly dispose
    /// this object and its events.
    /// Implements the ISimpleDevice interface which exposes its methods to COM.
    /// Because of that the ClassInterfaceType must be "None", otherwise the CLR
    /// provides another class interface (and does not use IAngleAxis).
    /// The class is forced to use the same GUID as the original CoClass does
    /// in order to be accessible by binary-only clients.
    /// </summary>
    [
    Guid("82C5AB54-C92C-4D52-AAC5-27E25E22604C"),
    ComSourceInterfaces(typeof(_ISimpleDeviceEvents)),
    ClassInterface(ClassInterfaceType.None)
    ]
    public class Device : ISimpleDevice, IDisposable //, _ISimpleDeviceEvents_Event 
    {
        ////////////////////
        // Private fields //
        ////////////////////

        // Vendor IDs of supported devices
        private const int c_AerionVendorID = 0x3eb;
        private const int c_LogitechVendorID = 0x46D;

        // Product IDs of supported devices
        private const int c_AerionProductID = 0x2013;
        private const int c_SpaceTravelerProductID = 0xC623;
        private const int c_SpacePilotProductID = 0xC625;
        private const int c_SpaceNavigatorProductID = 0xC626;
        private const int c_SpaceExplorerProductID = 0xC627;

        // constants fot field Type
        private const int c_UnknownDeviceType = 0;
        private const int c_SpaceExplorerDeviceType = 4;
        private const int c_SpaceNavigatorDeviceType = 6;
        private const int c_SpaceTravelerDeviceType = 25;
        private const int c_SpacePilotDeviceType = 29;
        private const int c_AerionDeviceType = 63;

        private Sensor m_Sensor = new Sensor();
        private Keyboard m_Keyboard = new Keyboard();
        private int m_Type;
        private bool m_IsConnected;

        /// <summary>
        /// Object which encapsulates LoadPreferencesName() functionality.
        /// </summary>
        private Preferences m_prefs = new Preferences();

        /// <summary>
        /// Used to communicate with the sigleton instance of HidControl for searching
        /// devices and registering DeviceChange events.
        /// </summary>
        private HidControl m_HidControl;

        /// <summary>
        /// Object of HidLibrary used to raise events when Hid Reports
        /// are recieved.
        /// </summary>
        private IHidDevice m_IHidDevice;

        //IDisposeable implementation help variable
        private bool IsDisposed = false;

        ////////////
        // Events //
        ////////////

        /// <summary>
        /// DeviceChange event handler member variable storing the function pointer
        /// (delegate) to the method m_HidControl will call back if a DeviceChange
        /// is detected.
        /// </summary>
        private EventHandler m_HidControlEvents_DeviceChangeEventHandler;

        /// <summary>
        /// Binary semaphore for locking event access.
        /// </summary>
        private readonly object m_lock = new object();

        /// <summary>
        /// Member variable storing registered event handlers for DeviceChange event.
        /// This is neccessary for a (hopefully) thread save event implementation.
        /// </summary>
        private event _ISimpleDeviceEvents_DeviceChangeEventHandler m_DeviceChange;

        /// <summary>
        /// Overwritten add() and remove() handlers for DeviceChange event which
        /// makes registering event handlers respect our member variable m_DeviceChange.
        /// This is neccessary for a (hopefully) thread save event implementation.
        /// </summary>
        public event _ISimpleDeviceEvents_DeviceChangeEventHandler DeviceChange
        {
            add
            {
                lock (m_lock)
                {
                    m_DeviceChange += value;
                }

            }
            remove
            {

                lock (m_lock)
                {
                    m_DeviceChange -= value;
                }
            }
        }
        
        /////////////////////       
        // Private methods //
        /////////////////////

        /// <summary>
        /// Predicate which queries p_IHidDeviceInfo.VendorID and
        /// p_IHidDeviceInfo.ProductID and returns true if an Aerion
        /// device is connected.
        /// </summary>
        /// <param name="p_IHidDeviceInfo"></param>
        /// <returns>boolean</returns>
        private bool IsAerion(IHidDeviceInfo p_IHidDeviceInfo)
        {
            return (p_IHidDeviceInfo.VendorID == c_AerionVendorID) &&
                (p_IHidDeviceInfo.ProductID == c_AerionProductID);
        }

        /// <summary>
        /// Predicate which queries p_IHidDeviceInfo.VendorID and
        /// p_IHidDeviceInfo.ProductID and returns true if a SpaceExplorer
        /// device is connected.
        /// </summary>
        /// <param name="p_IHidDeviceInfo"></param>
        /// <returns>boolean</returns>
        private bool IsSpaceExplorer(IHidDeviceInfo p_IHidDeviceInfo)
        {
            return (p_IHidDeviceInfo.VendorID == c_LogitechVendorID) &&
                (p_IHidDeviceInfo.ProductID == c_SpaceExplorerProductID);
        }

        /// <summary>
        /// Predicate which queries p_IHidDeviceInfo.VendorID and
        /// p_IHidDeviceInfo.ProductID and returns true if a SpaceNavigator
        /// device is connected.
        /// </summary>
        /// <param name="p_IHidDeviceInfo"></param>
        /// <returns>boolean</returns>
        private bool IsSpaceNavigator(IHidDeviceInfo p_IHidDeviceInfo)
        {
            return (p_IHidDeviceInfo.VendorID == c_LogitechVendorID) &&
                (p_IHidDeviceInfo.ProductID == c_SpaceNavigatorProductID);
        }

        /// <summary>
        /// Predicate which queries p_IHidDeviceInfo.VendorID and
        /// p_IHidDeviceInfo.ProductID and returns true if a SpaceTraveler
        /// device is connected.
        /// </summary>
        /// <param name="p_IHidDeviceInfo"></param>
        /// <returns>boolean</returns>
        private bool IsSpaceTraveler(IHidDeviceInfo p_IHidDeviceInfo)
        {
            return (p_IHidDeviceInfo.VendorID == c_LogitechVendorID) &&
                (p_IHidDeviceInfo.ProductID == c_SpaceTravelerProductID);
        }

        /// <summary>
        /// Predicate which queries p_IHidDeviceInfo.VendorID and
        /// p_IHidDeviceInfo.ProductID and returns true if a SpacePilot
        /// device is connected.
        /// </summary>
        /// <param name="p_IHidDeviceInfo"></param>
        /// <returns>boolean</returns>
        private bool IsSpacePilot(IHidDeviceInfo p_IHidDeviceInfo)
        {
            return (p_IHidDeviceInfo.VendorID == c_LogitechVendorID) &&
                (p_IHidDeviceInfo.ProductID == c_SpacePilotProductID);
        }

        /// <summary>
        /// Connects the AerionInput main instance with a plugged NDOF-device by
        /// passing a predicate.
        /// Instructs m_IHidDevice to handle incoming data and sets the global Type of
        /// the device.
        /// </summary>
        /// <returns>true if device is connected</returns>
        private bool ConnectAerion()
        {
            if ((m_IHidDevice = m_HidControl.GetDeviceByPredicate(new Predicate<IHidDeviceInfo>(IsAerion))) != null)
            {
                Console.Out.WriteLine("Aerion connected");
                m_IHidDevice.StartInPipeThread();
                m_Type = c_AerionDeviceType;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Connects the AerionInput main instance with a plugged NDOF-device by
        /// passing a predicate.
        /// Instructs m_IHidDevice to handle incoming data and sets the global Type of
        /// the device.
        /// </summary>
        /// <returns>true if device is connected</returns>
        private bool ConnectSpaceExplorer()
        {
            if ((m_IHidDevice = m_HidControl.GetDeviceByPredicate(new Predicate<IHidDeviceInfo>(IsSpaceExplorer))) != null)
            {
                Console.Out.WriteLine("SpaceExplorer connected");
                m_IHidDevice.StartInPipeThread();
                m_Type = c_SpaceExplorerDeviceType;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Connects the AerionInput main instance with a plugged NDOF-device by
        /// passing a predicate.
        /// Instructs m_IHidDevice to handle incoming data and sets the global Type of
        /// the device.
        /// </summary>
        /// <returns>true if device is connected</returns>
        private bool ConnectSpaceTraveler()
        {
            if ((m_IHidDevice = m_HidControl.GetDeviceByPredicate(new Predicate<IHidDeviceInfo>(IsSpaceTraveler))) != null)
            {
                Console.Out.WriteLine("SpaceTraveler connected");
                m_IHidDevice.StartInPipeThread();
                m_Type = c_SpaceTravelerDeviceType;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Connects the AerionInput main instance with a plugged NDOF-device by
        /// passing a predicate.
        /// Instructs m_IHidDevice to handle incoming data and sets the global Type of
        /// the device.
        /// </summary>
        /// <returns>true if device is connected</returns>
        private bool ConnectSpaceNavigator()
        {
            if ((m_IHidDevice = m_HidControl.GetDeviceByPredicate(new Predicate<IHidDeviceInfo>(IsSpaceNavigator))) != null)
            {
                Console.Out.WriteLine("SpaceNavigator connected");
                m_IHidDevice.StartInPipeThread();
                m_Type = c_SpaceNavigatorDeviceType;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Connects the AerionInput main instance with a plugged NDOF-device by
        /// passing a predicate.
        /// Instructs m_IHidDevice to handle incoming data and sets the global Type of
        /// the device.
        /// </summary>
        /// <returns>true if device is connected</returns>
        private bool ConnectSpacePilot()
        {
            if ((m_IHidDevice = m_HidControl.GetDeviceByPredicate(new Predicate<IHidDeviceInfo>(IsSpacePilot))) != null)
            {
                Console.Out.WriteLine("SpacePilot connected");
                m_IHidDevice.StartInPipeThread();
                m_Type = c_SpacePilotDeviceType;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Handles the HidControlDeviceChange event. First disconnects the current
        /// device from the driver, then calls Connect() which looks for new devices.
        /// Fires DeviceChange event in order to notify clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HidControlDeviceChange(object sender, EventArgs e)
        {
            // Disconnect and free pointers to events
            Disconnect();
            // Connect again, look for new devices
            Connect();

            // call DeviceChange() in order to notify clients that they should update their Sensor and Keyboard pointers
            _ISimpleDeviceEvents_DeviceChangeEventHandler DeviceChangeEventHandler = null;

            lock (m_lock)
            {
                DeviceChangeEventHandler = m_DeviceChange;
                if (DeviceChangeEventHandler != null)
                {
                    DeviceChangeEventHandler(0);
                }
            }

            #region old stuff
            /*
            if (DeviceChange != null)
            {
                lock (DeviceChange)
                {
                    DeviceChange(0);
                }
            }
             */
            #endregion
        }
        
        ///////////////////////////
        // Getter/Setter methods //
        ///////////////////////////

        /// <summary>
        /// Returns the associated Sensor object.
        /// </summary>
        public Sensor Sensor
        {
            get { return m_Sensor; }
        }
        /// <summary>
        /// Returns the associated Keyboard object.
        /// </summary>
        public Keyboard Keyboard
        {
            get { return m_Keyboard; }
        }

        /// <summary>
        /// Returns type of the connected device.
        /// </summary>
        public int Type
        {
            get { return m_Type; }
        }

        /// <summary>
        /// Returns true if a device is connected with the driver.
        /// </summary>
        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        ////////////////////
        // Public methods //
        ////////////////////

        /// <summary>
        /// Loads the application profile in argument preferencesName
        /// and updates all relevant fields. For now save the config
        /// file, as the Blender plugin does not disconnect (and thereby
        /// save the config file) correctly.
        /// </summary>
        public void LoadPreferences(string preferencesName)
        {
            Console.WriteLine("Loading Preferences: {0}", preferencesName);
            m_prefs.PreferencesName = preferencesName;
            m_Sensor.XScaleFactor = m_prefs.XScaleFactor;
            m_Sensor.YScaleFactor = m_prefs.YScaleFactor;
            m_Sensor.ZScaleFactor = m_prefs.ZScaleFactor;
            m_Sensor.TranslationThreshold = m_prefs.TranslationThreshold;
            m_Sensor.TranslationFunction = m_prefs.TranslationFunction;

            m_Sensor.XRotScaleFactor = m_prefs.XRotScaleFactor;
            m_Sensor.YRotScaleFactor = m_prefs.YRotScaleFactor;
            m_Sensor.ZRotScaleFactor = m_prefs.ZRotScaleFactor;
            m_Sensor.RotationThreshold = m_prefs.RotationThreshold;
            m_Sensor.RotationFunction = m_prefs.RotationFunction;

            m_prefs.saveConfig();
        }

        /// <summary>
        /// Look for connected supported devices and register for HidControl.DeviceChange
        /// event to be notified if devices are connected. If a supported device is found
        /// init the Sensor and the Keyboard. Then load initial "default" profile.
        /// Only one device at a time is currently supported.
        /// </summary>
        public void Connect()
        {
            lock (m_lock)
            {
                m_HidControlEvents_DeviceChangeEventHandler = new EventHandler(this.HidControlDeviceChange);
                m_HidControl.DeviceChange += m_HidControlEvents_DeviceChangeEventHandler;

                m_IsConnected =
                    ConnectAerion() ||
                    ConnectSpaceExplorer() ||
                    ConnectSpaceNavigator() ||
                    ConnectSpacePilot() ||
                    ConnectSpaceTraveler();

                if (m_IsConnected)
                {
                    m_Keyboard.InitKeyboard(this, m_IHidDevice);
                    m_Sensor.InitSensor(this, m_IHidDevice);
                }
                else
                {
                    Console.Out.WriteLine("No supported Device connected!");
                }
            }

            m_prefs.loadConfig();
            LoadPreferences("default");
        }

        /// <summary>
        /// Disconnect a connected device. Stop the HidDevice processing input data,
        /// and deregister from HidDevice.DeviceChange event handler. Save the config file.
        /// </summary>
        public void Disconnect()
        {
            lock (m_lock)
            {
                m_Sensor.DestroySensor();
                m_Keyboard.DestroyKeyboard();

                m_HidControl.DeviceChange -= m_HidControlEvents_DeviceChangeEventHandler;
                if (m_IHidDevice != null)
                    m_IHidDevice.StopInPipeThread();

                m_prefs.saveConfig();
            }
        }

        /// <summary>
        /// Default constructor needed by COM to create objects.
        /// </summary>
        public Device()
        {
            m_HidControl = HidControl.GetInstance();
        }

        /// <summary>
        /// Destructor supporting IDisposeable implementation.
        /// </summary>
        ~Device()
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
                    Disconnect();
                    m_HidControl = null;
                    m_IHidDevice = null;
                    m_Sensor.Dispose();
                    m_Keyboard.Dispose();

                    //Ensure we clean-up references to event
                    m_DeviceChange = null;
                }
                //Clean up unmanaged resources

            }
            IsDisposed = true;
        }
    }
}