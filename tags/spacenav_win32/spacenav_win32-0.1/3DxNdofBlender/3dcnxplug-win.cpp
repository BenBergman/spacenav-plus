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

#include <cassert> 
#include <iostream>

#include "3dcnxplug-win.h"

/* copied from GHOST_Types.h to avoid including the whole thing */

#define WM_BLND_NDOF_AXIS	WM_USER + 1
#define WM_BLND_NDOF_BTN 	WM_USER + 2

typedef	char				GHOST_TInt8;
typedef	unsigned char		GHOST_TUns8;
typedef short				GHOST_TInt16;
typedef unsigned short		GHOST_TUns16;
typedef	int					GHOST_TInt32;
typedef	unsigned int		GHOST_TUns32;
typedef long long			GHOST_TInt64;
typedef unsigned long long	GHOST_TUns64;
typedef void*				GHOST_TUserDataPtr;

typedef struct {
   /** N-degree of freedom device data v2*/
   int 			changed;
   GHOST_TUns64 client;
   GHOST_TUns64 address;			/* to identify between several devices */
   GHOST_TInt16 tx, ty, tz;   	/* -x left, +y up, +z forward */
   GHOST_TInt16 rx, ry, rz;
   GHOST_TInt16 buttons;
   GHOST_TUns64 time;
   GHOST_TUns64 delta;
} GHOST_TEventNDOFData;

// --------------------------------------------------------------------------

using namespace std;

/* pointer to the instance of our COM server */
static NDOFServer *sNdofServer = NULL;

/* pointer to the volatile struct in ndof manager */
static GHOST_TEventNDOFData *sNdofStatus = NULL;

// ---------------------------------------------------------------------------
// functions called from the GHOST system

extern "C"
__declspec(dllexport)
int ndofInit()
{
	if (sNdofServer == NULL)
		sNdofServer = new NDOFServer();

	return 1;
}

extern "C"
__declspec(dllexport)
void* ndofOpen(void* platformData)
{
	return sNdofServer->open(platformData);
}

extern "C"
__declspec(dllexport)
void ndofShutdown(void* deviceHandle)
{
	if (sNdofServer)
	{
		delete sNdofServer;
		sNdofServer = NULL;
	}
}

// ---------------------------------------------------------------------------

NDOFServer::
~NDOFServer()
{
	shutdown();
}

void* NDOFServer::
open(void *platformData)
{
	// set ptr to shared data
	sNdofStatus = (GHOST_TEventNDOFData*)platformData;
	
	if (initCOM())
		return NULL;

	return this;
}

/* Returns 0 if no error. */
int NDOFServer::
initCOM()
{
	HRESULT hr;
	int err = -1;
	CComPtr<IUnknown> cnx_dev;

	hr = ::CoInitializeEx(0, COINIT_APARTMENTTHREADED | COINIT_SPEED_OVER_MEMORY);
	if (!SUCCEEDED(hr))
    {
        cerr<< "3Dconnexion plugin [" << hr << "]: CoInitializeEx failed!\n";
		return err;
    }

	// create the COM device object
	hr = cnx_dev.CoCreateInstance(__uuidof(Device));
	#ifdef _DEBUG
	cerr<<  "3Dconnexion plugin [" << hr << "]: main COM instance=" 
        << cnx_dev.p << endl;
    #endif
    
	if (SUCCEEDED(hr)) {
		CComPtr<ISimpleDevice> cnx_simpledev;

		hr = cnx_dev.QueryInterface(&cnx_simpledev);
		#ifdef _DEBUG
		cerr<< "3Dconnexion plugin [" << hr << "]: COM device interface=" 
            << cnx_simpledev.p << endl;
        #endif
        
		if (SUCCEEDED(hr)) {
			// get the actual interface to the sensor and register for events
			hr = cnx_simpledev->get_Sensor(&m_3DSensor);
            #ifdef _DEBUG
            cerr<< "3Dconnexion plugin [" << hr << "]: sensor handle=" 
                << m_3DSensor.p << endl;
            #endif
            assert(m_3DSensor.p != NULL);
			if (SUCCEEDED(hr) && m_3DSensor.p)
            {
				hr = __hook(&_ISensorEvents::SensorInput, m_3DSensor, 
				            &NDOFServer::OnSensorInput);
                if (FAILED(hr))
                    cerr<< "3Dconnexion plugin [" << hr << "]: " 
                        << "FAILED to register for device sensor events.\n";
            }
			
			// get the interface to the device keys and register for events
			HRESULT hrk = cnx_simpledev->get_Keyboard(&m_DevKeyb);
            #ifdef _DEBUG
            cerr<< "3Dconnexion plugin [" << hrk << "]: device keyboard handle=" 
                << m_DevKeyb.p << endl;
            #endif
			if (SUCCEEDED(hrk) && m_DevKeyb.p)
            {
				hrk = __hook(&_IKeyboardEvents::KeyDown, m_DevKeyb, 
				             &NDOFServer::OnDevKeyDown);
                if (FAILED(hrk))
                    cerr<< "3Dconnexion plugin [" << hrk << "]: " 
                        << "FAILED to register for device button events.\n";
            }

			// connect to the driver
			if (SUCCEEDED(hr) && SUCCEEDED(hrk) 
			    && cnx_simpledev->Connect() == S_OK)
				err = 0;

			// specify our ID (useful if blender can host multiple plugins 
			// using our device)
			BSTR preferencesName;
			preferencesName = SysAllocString(L"Blender");
			cnx_simpledev->LoadPreferences(preferencesName);
			SysFreeString(preferencesName);
		}
		cnx_dev.Release();
	}

	cerr << "3Dconnexion device initialization " 
		 << (err ? "NOT" : "") << "successful" << endl;

	return err;
}

void NDOFServer::
shutdown()
{
	if (m_3DSensor)
	{		
		CComPtr<ISimpleDevice> cnx_simpledev;
		m_3DSensor->get_Device((IDispatch**)&cnx_simpledev);

		// unhook (unadvise) the sensor event sink
		__unhook(&_ISensorEvents::SensorInput, m_3DSensor, 
		         &NDOFServer::OnSensorInput);
		__unhook(&_IKeyboardEvents::KeyDown, m_DevKeyb, 
		         &NDOFServer::OnDevKeyDown);

		m_3DSensor.Release();
		m_DevKeyb.Release();

		// disconnect from the driver
		if (cnx_simpledev)
		{
			cnx_simpledev->Disconnect();
			cnx_simpledev.Release();
		}
	}

	cerr << "3Dconnexion device shutdown." << endl;
}

HRESULT NDOFServer::
OnSensorInput()
{
	static double ndofData[7];

	if (m_3DSensor)
	{
		try 
		{
			CComPtr<IAngleAxis> pRot;
			CComPtr<IVector3D>  pTra;
			double len, ang;

			m_3DSensor->get_Translation(&pTra);
			m_3DSensor->get_Rotation(&pRot);
			pTra->get_Length(&len);
			pRot->get_Angle(&ang);
			m_3DSensor->get_Period(&ndofData[6]);
			sNdofStatus->changed = 1;
			sNdofStatus->delta = ndofData[6] - sNdofStatus->time;
			sNdofStatus->time = ndofData[6];
				
			// Check if the cap is still displaced
			if (len > 0.0 || ang > 0.0)
			{
				pTra->get_X(&ndofData[0]);
				pTra->get_Y(&ndofData[1]);
				pTra->get_Z(&ndofData[2]);
				pRot->get_X(&ndofData[3]);
				pRot->get_Y(&ndofData[4]);
				pRot->get_Z(&ndofData[5]);
				
				sNdofStatus->tx = 1.5f * ndofData[0];
				sNdofStatus->ty = 1.5f * ndofData[1];
				sNdofStatus->tz = -1.5f * ndofData[2];
				sNdofStatus->rx = 8.f * ndofData[3] * ang;
				sNdofStatus->ry = 8.f * ndofData[4] * ang;
				sNdofStatus->rz = 8.f * ndofData[5] * ang;
				
				//cerr << "plugin: [" << sNdofStatus->tx << " "
				//	 << sNdofStatus->ty << " " << sNdofStatus->tz << "] ["
				//	 << sNdofStatus->rx << " " << sNdofStatus->ry << " "
				//	 << sNdofStatus->rz << "]\n" ;
			}
			else 
			{	// zero out axisData so that 3d motion stops.
				memset(&sNdofStatus->tx, 0, sizeof(GHOST_TInt16) * 6);
			}

			pRot.Release();
			pTra.Release();
			
			PostMessage(GetActiveWindow(), WM_BLND_NDOF_AXIS, NULL, NULL);
		}
		catch (...)
		{
			cerr << "3Dconnexion plugin: exception while fetching device sensor data."
			     << endl;
		}
	}

	return S_OK;
}

HRESULT NDOFServer::
OnDevKeyDown(long keycode)
{
	if (m_DevKeyb)
	{
		try 
		{
			sNdofStatus->changed = 2;
			sNdofStatus->buttons = keycode;
			PostMessage(GetActiveWindow(), WM_BLND_NDOF_BTN, NULL, NULL);
		}
		catch (...)
		{
			cerr << "3Dconnexion plugin: exception while fetching device keyboard data."
			     << endl;
		}
	}

	return S_OK;

}
