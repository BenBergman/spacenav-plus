/*
   Events interface for the Keyboard class
 
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
using System.Runtime.CompilerServices;

namespace TDxInput
{
    #region interfaces and attributes perhaps useful to implement later
    /*
    IManagedObject
    IProvideClassInfo
    [ event_source(com) ]
    */
    #endregion

    /// <summary>
    /// Delegate declaration for KeyDown event handler. Only states the return 
    /// type of the event which is void for COM events and the signature of the event.
    /// This is implicitly COM visible and needed for COM to determine the signature
    /// of the event.
    /// </summary>
    /// <param name="keyCode">KeyCode of the button which is pressed down.</param> 
    public delegate void _IKeyboardEvents_KeyDownEventHandler(int keyCode);

    /// <summary>
    /// Delegate declaration for KeyUp event handler. Only states the return 
    /// type of the event which is void for COM events and the signature of the event.
    /// This is implicitly COM visible and needed for COM to determine the signature
    /// of the event.
    /// </summary>
    /// <param name="keyCode">KeyCode of the button which pulled up.</param>
    public delegate void _IKeyboardEvents_KeyUpEventHandler(int keyCode);

    /// <summary>
    /// COM Events publisher interface forced to use
    /// the same GUID as the original driver interface does.
    /// Must be IDispatch Interface for late binding, not standard dual.
    /// </summary>
    [
    Guid("6B6BB0A8-4491-40CF-B1A9-C15A801FE151"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch)
    ]
    public interface _IKeyboardEvents
    {
        /// <summary>
        /// The KeyDown event is fired whenever a key is pressed. The keyCode
        /// argument contains the number of the key that fired.
        /// </summary>
        /// <param name="keyCode"></param>
        [DispId(1)]
        void KeyDown([In] int keyCode);

        /// <summary>
        /// The KeyUp event is fired whenever a key is released. The keyCode
        /// argument contains the number of the key that fired.
        /// </summary>
        /// <param name="keyCode"></param>
        [DispId(2)]
        void KeyUp([In] int keyCode);

        #region overloaded int32 functions and old event handlers
        // <summary>
        // Overloaded int32-parameter events (non-functional)
        // </summary>
        // <param name="keycode"></param>
        /*
        [PreserveSig]
        void KeyDown([param: MarshalAs(UnmanagedType.I4)]int keycode);
        [PreserveSig] void KeyUp(int keycode);
         */
        /*
        event _IKeyboardEvents_KeyDownEventHandler KeyDown;
        event _IKeyboardEvents_KeyUpEventHandler KeyUp;
        */
        #endregion
    }
}
