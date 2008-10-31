/*
   Copyright 2007 3Dconnexion, Inc.
 
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

   Contributor(s): Ettore Pasquini
*/

#if defined(_WIN32)

// Modify the following defines if you have to target a platform prior to the
// ones specified below. Refer to MSDN for the latest info on corresponding 
// values for different platforms.
#ifndef WINVER			// Allow use of features specific to Windows XP or later.
#define WINVER 0x0501		// Change this to to target other versions of Windows.
#endif

#ifndef _WIN32_WINNT		// Allow use of features specific to WinXP or later.
#define _WIN32_WINNT 0x0501	// (was 0x0501) Change to target other versions of Windows
#endif						

#ifndef _WIN32_WINDOWS		// Allow use of features specific to Windows 98 or later.
#define _WIN32_WINDOWS 0x0410 // Change this to target Windows Me or later.
#endif

#ifndef _WIN32_IE			// Allow use of features specific to IE 6.0 or later.
#define _WIN32_IE 0x0600	// Change this to target other versions of IE.
#endif

#define _ATL_APARTMENT_THREADED
#define _ATL_NO_AUTOMATIC_NAMESPACE
#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS	// some CString constructors will be explicit

#include <atlbase.h>
#include <atlcom.h>
#include <atlwin.h>
#include <atltypes.h>
#include <atlctl.h>
#include <atlhost.h>

using namespace ATL;

#import "progid:TDxInput.Device" embedded_idl no_namespace
//#import "progid:TDxInput.Device" no_namespace

//#define  INC_OLE2
//#include <windows.h>

#endif //_WIN32
