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

#ifndef NDOF_3DX_PLUG_H_
#define NDOF_3DX_PLUG_H_

#include "3dcnxplug-stdafx.h"

// ----------------------------------------------------------------------------

extern "C"
__declspec(dllexport)
int ndofInit();

extern "C"
__declspec(dllexport)
void* ndofOpen(void* wnd);

extern "C"
__declspec(dllexport)
void ndofShutdown(void* deviceHandle);

// ----------------------------------------------------------------------------

[ event_receiver(com) ]
[ module(type=dll, name="NDOF") ]
class NDOFServer 
{
public:
	NDOFServer() {}
    virtual ~NDOFServer();

	/** Returns NULL if unsuccessful. */
    void* open(void *platformData);

	/** Closes connection with the COM server. */
    void  shutdown();
	
	/** Callback invoked by the COM server when motion input is detected. */
    HRESULT OnSensorInput();

	/** Callback invoked by the COM server when device keys are pressed. */
	HRESULT OnDevKeyDown(long keycode);

protected:
    int initCOM();

private:
    CComPtr<ISensor>    m_3DSensor;
	CComPtr<IKeyboard>  m_DevKeyb;
};

#endif
