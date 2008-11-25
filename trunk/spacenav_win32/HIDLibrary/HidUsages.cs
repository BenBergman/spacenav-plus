/*
   HidLibrary.Net   http://sourceforge.net/projects/hidlibrary/
 
   Copyright (C)    Roman Reichel 2006
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

namespace HidLibrary
{
    //
    // Usage Pages
    //

    enum HID_USAGE_PAGE
    {
        PAGE_UNDEFINED      = 0x00,
        PAGE_GENERIC        = 0x01,
        PAGE_SIMULATION     = 0x02,
        PAGE_VR             = 0x03,
        PAGE_SPORT          = 0x04,
        PAGE_GAME           = 0x05,
        PAGE_KEYBOARD       = 0x07,
        PAGE_LED            = 0x08,
        PAGE_BUTTON         = 0x09,
        PAGE_ORDINAL        = 0x0A,
        PAGE_TELEPHONY      = 0x0B,
        PAGE_CONSUMER       = 0x0C,
        PAGE_DIGITIZER      = 0x0D,
        PAGE_UNICODE        = 0x10,
        PAGE_ALPHANUMERIC   = 0x14
    }

    //
    // Usages from Generic Desktop Page (0x01;
    //

    enum HID_USAGE_GENERIC
    {
        POINTER      = 0x01,
        MOUSE        = 0x02,
        JOYSTICK     = 0x04,
        GAMEPAD      = 0x05,
        KEYBOARD     = 0x06,
        KEYPAD       = 0x07,
        SYSTEM_CTL   = 0x80,
        X                        = 0x30,
        Y                        = 0x31,
        Z                        = 0x32,
        RX                       = 0x33,
        RY                       = 0x34,
        RZ                       = 0x35,
        SLIDER                   = 0x36,
        DIAL                     = 0x37,
        WHEEL                    = 0x38,
        HATSWITCH                = 0x39,
        COUNTED_BUFFER           = 0x3A,
        BYTE_COUNT               = 0x3B,
        MOTION_WAKEUP            = 0x3C,
        VX                       = 0x40,
        VY                       = 0x41,
        VZ                       = 0x42,
        VBRX                     = 0x43,
        VBRY                     = 0x44,
        VBRZ                     = 0x45,
        VNO                      = 0x46,
        SYSCTL_POWER             = 0x81,
        SYSCTL_SLEEP             = 0x82,
        SYSCTL_WAKE              = 0x83,
        SYSCTL_CONTEXT_MENU      = 0x84,
        SYSCTL_MAIN_MENU         = 0x85,
        SYSCTL_APP_MENU          = 0x86,
        SYSCTL_HELP_MENU         = 0x87,
        SYSCTL_MENU_EXIT         = 0x88,
        SYSCTL_MENU_SELECT       = 0x89,
        SYSCTL_MENU_RIGHT        = 0x8A,
        SYSCTL_MENU_LEFT         = 0x8B,
        SYSCTL_MENU_UP           = 0x8C,
        SYSCTL_MENU_DOWN         = 0x8D
    }
    
    enum HID_USAGE_SIMULATION
    {

        RUDDER                = 0xBA,
        THROTTLE              = 0xBB
    }

    //
    // Virtual Reality Controls Page (0x03;
    //


    //
    // Sport Controls Page (0x04;
    //


    //
    // Game Controls Page (0x05;
    //


    //
    // Keyboard/Keypad Page (0x07;
    //

    // Error "keys"

    enum HID_USAGE_KEYBOARD
    {
        NOEVENT     = 0x00,
        ROLLOVER    = 0x01,
        POSTFAIL    = 0x02,
        UNDEFINED   = 0x03,

        // Letters
        aA          = 0x04,
        zZ          = 0x1D,
        // Numbers
        ONE         = 0x1E,
        ZERO        = 0x27,
        // Modifier Keys
        LCTRL       = 0xE0,
        LSHFT       = 0xE1,
        LALT        = 0xE2,
        LGUI        = 0xE3,
        RCTRL       = 0xE4,
        RSHFT       = 0xE5,
        RALT        = 0xE6,
        RGUI        = 0xE7,
        SCROLL_LOCK = 0x47,
        NUM_LOCK    = 0x53,
        CAPS_LOCK   = 0x39,
        // Funtion keys
        F1          = 0x3A,
        F12         = 0x45,

        RETURN      = 0x28,
        ESCAPE      = 0x29,
        DELETE      = 0x2A,

        PRINT_SCREEN = 0x46
    }
    // and hundreds more...

    //
    // LED Page (0x08;
    //
    enum HID_USAGE_LED
    {
        NUM_LOCK               = 0x01,
        CAPS_LOCK              = 0x02,
        SCROLL_LOCK            = 0x03,
        COMPOSE                = 0x04,
        KANA                   = 0x05,
        POWER                  = 0x06,
        SHIFT                  = 0x07,
        DO_NOT_DISTURB         = 0x08,
        MUTE                   = 0x09,
        TONE_ENABLE            = 0x0A,
        HIGH_CUT_FILTER        = 0x0B,
        LOW_CUT_FILTER         = 0x0C,
        EQUALIZER_ENABLE       = 0x0D,
        SOUND_FIELD_ON         = 0x0E,
        SURROUND_FIELD_ON      = 0x0F,
        REPEAT                 = 0x10,
        STEREO                 = 0x11,
        SAMPLING_RATE_DETECT   = 0x12,
        SPINNING               = 0x13,
        CAV                    = 0x14,
        CLV                    = 0x15,
        RECORDING_FORMAT_DET   = 0x16,
        OFF_HOOK               = 0x17,
        RING                   = 0x18,
        MESSAGE_WAITING        = 0x19,
        DATA_MODE              = 0x1A,
        BATTERY_OPERATION      = 0x1B,
        BATTERY_OK             = 0x1C,
        BATTERY_LOW            = 0x1D,
        SPEAKER                = 0x1E,
        HEAD_SET               = 0x1F,
        HOLD                   = 0x20,
        MICROPHONE             = 0x21,
        COVERAGE               = 0x22,
        NIGHT_MODE             = 0x23,
        SEND_CALLS             = 0x24,
        CALL_PICKUP            = 0x25,
        CONFERENCE             = 0x26,
        STAND_BY               = 0x27,
        CAMERA_ON              = 0x28,
        CAMERA_OFF             = 0x29,
        ON_LINE                = 0x2A,
        OFF_LINE               = 0x2B,
        BUSY                   = 0x2C,
        READY                  = 0x2D,
        PAPER_OUT              = 0x2E,
        PAPER_JAM              = 0x2F,
        REMOTE                 = 0x30,
        FORWARD                = 0x31,
        REVERSE                = 0x32,
        STOP                   = 0x33,
        REWIND                 = 0x34,
        FAST_FORWARD           = 0x35,
        PLAY                   = 0x36,
        PAUSE                  = 0x37,
        RECORD                 = 0x38,
        ERROR                  = 0x39,
        SELECTED_INDICATOR     = 0x3A,
        IN_USE_INDICATOR       = 0x3B,
        MULTI_MODE_INDICATOR   = 0x3C,
        INDICATOR_ON           = 0x3D,
        INDICATOR_FLASH        = 0x3E,
        INDICATOR_SLOW_BLINK   = 0x3F,
        INDICATOR_FAST_BLINK   = 0x40,
        INDICATOR_OFF          = 0x41,
        FLASH_ON_TIME          = 0x42,
        SLOW_BLINK_ON_TIME     = 0x43,
        SLOW_BLINK_OFF_TIME    = 0x44,
        FAST_BLINK_ON_TIME     = 0x45,
        FAST_BLINK_OFF_TIME    = 0x46,
        INDICATOR_COLOR        = 0x47,
        RED                    = 0x48,
        GREEN                  = 0x49,
        AMBER                  = 0x4A,
        GENERIC_INDICATOR      = 0x3B
    }

    //
    //  Button Page (0x09;
    //
    //  There is no need to label these usages.
    //


    //
    //  Ordinal Page (0x0A;
    //
    //  There is no need to label these usages.
    //


    //
    //  Telephony Device Page (0x0B;
    //

    enum HID_USAGE_TELEPHONY
    {
        PHONE                  = 0x01,
        ANSWERING_MACHINE      = 0x02,
        MESSAGE_CONTROLS       = 0x03,
        HANDSET                = 0x04,
        HEADSET                = 0x05,
        KEYPAD                 = 0x06,
        PROGRAMMABLE_BUTTON    = 0x07
    }
        //
        // and others...
        //
    enum HID_USAGE_OTHER
    {
        CONSUMERCTRL          = 0x01,
        DIGITIZER_PEN         = 0x02,
        DIGITIZER_IN_RANGE    = 0x32,
        DIGITIZER_TIP_SWITCH  = 0x42,
        DIGITIZER_BARREL_SWITCH =0x44
    }
}


