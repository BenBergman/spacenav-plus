// SiAppWrapper.cpp : Definiert die exportierten Funktionen für die DLL-Anwendung.

#include "stdafx.h"
#include "SiAppDll.h"
#include "spwdata.h"
#include "si.h"
#include "spwmacro.h"
#include "spwerror.h"
#include "version.h"
#include "NDOFServer.h"

#include <cassert> 
#include <iostream>
#include <string>
#include <cstring>
#include <bitset>
#include <math.h>
#include <list>

using namespace std;
using namespace stdext;

#define WM_SIAPP_AXIS	WM_USER + 0x1
#define WM_SIAPP_BTN 	WM_USER + 0x2

list <NDOFServer *> vNDOFServers;

/*************************************************************************
**************************************************************************
********************     DLL Exports implementation    *******************
**************************************************************************
**************************************************************************/

#ifdef __cplusplus
extern "C" {
#endif
	/// Creates and initializes COM-Objects of AerionInput. Because the architecture
	/// of SiAppDLL and the COM API do not match exactly this does nothing.
	DLL_EXPORT enum			SpwRetVal SiInitialize(void)
	{
		SpwErrorVal = SPW_NO_ERROR;
		return SPW_NO_ERROR;
	}

	/// Prepares SiOpenData structure. Copies the
	/// passed data in the SiOpenData structure.
	DLL_EXPORT void           SiOpenWinInit (SiOpenData *pData, HWND hWnd)
	{
		memset(&(pData->exeFile), '\0', MAX_PATH);
		pData->hWnd = hWnd;
		pData->transCtl = NULL;
		pData->libFlag = 0;
		pData->processID = 0;
		SpwErrorVal = SPW_NO_ERROR;
	}

	/// Open the connection to AerionInput and initializes the fields of pData to correct values.
	DLL_EXPORT SiHdl          SiOpen (LPCSTR pAppName, SiDevID devID, SiTypeMask *pTMask, int mode, 
		SiOpenData *pData)
	{
		NDOFServer *pNdofServer;
		try
		{
			pNdofServer = new NDOFServer();
		}
		catch (bad_alloc)
		{
			SpwErrorVal = SI_OUT_OF_MEMORY;
			return NULL;
		}
		vNDOFServers.insert(vNDOFServers.end(), pNdofServer);
		if (!pNdofServer->open())
		{
			SpwErrorVal = SPW_ERROR;
			return NULL;
		}
		
		if (!(devID == SI_ANY_DEVICE)) 
		{
			long type;
			if (!SUCCEEDED(pNdofServer->get_Device()->get_Type(&type)))
			{
				SiClose(pNdofServer);
				SpwErrorVal = SPW_ERROR;
				return NULL;
			}
			if (!((SiDevID) type == devID))
			{
				SiClose(pNdofServer);
				SpwErrorVal = SPW_ERROR;
				return NULL;
			}
		}

		pNdofServer->set_Hwnd(pData->hWnd);

		//TODO exeFile enthält eventuell vollst. pfad, also app-profile irgendwie daraus zusammenbasteln
		pNdofServer->set_AppName(pAppName);
		strcpy_s(pData->exeFile, pAppName);
		//TODO: processID rausbekommen
		ATLTRACE("exeFile: %s\n", (LPCSTR) pData->exeFile);
		return pNdofServer;
	}

	/// Terminates all library activity. Clears the list of known NDOFServer instances.
	DLL_EXPORT void			SiTerminate(void)
	{
		list <NDOFServer *>::iterator startIterator;
		for (unsigned int i = 0; i < vNDOFServers.size(); i++ )
		{
			startIterator = vNDOFServers.begin();
			NDOFServer *tmpServer = vNDOFServers.front();
			delete tmpServer;
			tmpServer = NULL;
			vNDOFServers.erase(startIterator);			
		}
		SpwErrorVal = SPW_NO_ERROR;
	}

	/// Closes the connection to AerionInput and frees all its COM Objects.
	DLL_EXPORT enum			SpwRetVal SiClose (SiHdl hdl)
	{
		if (hdl != NULL)
		{
			NDOFServer *pNdofServer = static_cast<NDOFServer *> (hdl);
			vNDOFServers.remove(pNdofServer);
			delete pNdofServer;
			pNdofServer = NULL;
			return SPW_NO_ERROR;
		}		
		else
		{
			return SI_BAD_HANDLE;
		}
	}

	/// Prepares the SiGetEventData structure. Copies the
	/// passed data in the SiGetEventData structure.
	DLL_EXPORT void           SiGetEventWinInit (SiGetEventData *pData,
		UINT msg, WPARAM wParam, LPARAM lParam)
	{		
		pData->lParam = lParam;
		pData->wParam = wParam;
		pData->msg = msg;
		SpwErrorVal = SPW_NO_ERROR;
	}

	/// Checks if passed event was sent from this library and sent to 
	/// it's own hWnd. Returns errorvalue if not.
	DLL_EXPORT enum			SpwRetVal SiGetEvent (SiHdl hdl, int flags, SiGetEventData *pData, 
		SiSpwEvent *pEvent)
	{		
		if (hdl == NULL)
		{
			return SI_BAD_HANDLE;
		}
		NDOFServer *pNdofServer = static_cast<NDOFServer *> (hdl);

		ATLTRACE("msg: %d, WM_SIAPP_AXIS: %d, WM_SIAPP_BTN: %d\n", pData->msg, WM_SIAPP_AXIS, WM_SIAPP_BTN);
		
		if (!pNdofServer->get_PEventInitialized())
		{
			pEvent->type = 0;
			memset(&(pEvent->u), '\0', sizeof(pEvent->u));
			pNdofServer->set_PEventInitialized(true);
		}

		// Wparam contains the hWnd of the currently active window which received the PostMessage() call.
		// Because every app receives the message, check here if the message is for asking window or not.
		HWND h = pNdofServer->get_Hwnd();
		HWND w = (HWND) pData->wParam;
		
		if (pData->msg == WM_SIAPP_AXIS && (h == w))
		{
			pNdofServer->readSensorData(pEvent);
			return SI_IS_EVENT;
		}
		else if (pData->msg == WM_SIAPP_BTN && (h == w))
		{		
			pNdofServer->readKeyboardData(pEvent);
			return SI_IS_EVENT;
		}
		else
			return SI_NOT_EVENT;

	}

	/// Returns information about the device. As there is no "version" 
	/// of the device, it is identical to the driver version.
	DLL_EXPORT enum           SpwRetVal SiGetDeviceInfo (SiHdl hdl, SiDevInfo *pInfo)
	{	
		if (hdl == NULL)
		{
			return SI_BAD_HANDLE;
		}

		NDOFServer *pNdofServer = static_cast<NDOFServer *> (hdl);
		
		long tmp = 0;

		if (!SUCCEEDED(pNdofServer->get_Keyboard()->get_Keys(&tmp)))
		{
			return SPW_ERROR;
		}

		pInfo->numButtons = static_cast<int> (tmp);

		if (!SUCCEEDED(pNdofServer->get_Device()->get_Type(&tmp)))
		{
			return SPW_ERROR;
		}
		pInfo->devType = static_cast<int> (tmp);

		memset(&(pInfo->firmware), '\0', SI_STRSIZE);

		string firmware (SII_FIRMWARE_VER);
		firmware._Copy_s(pInfo->firmware, SI_STRSIZE, firmware.length(), 0);
		pInfo->majorVersion = SII_DVR_MAJOR;
		pInfo->minorVersion = SII_DVR_MINOR;
		pInfo->canBeep = 0;		
		pInfo->numDegrees = 6;

		return SPW_NO_ERROR;
	}

	/// Returns information about the library.
	DLL_EXPORT void         SiGetLibraryInfo (SiVerInfo *pInfo)
	{
		pInfo->build = SII_LIB_BUILD;
		strcpy_s(pInfo->date, SII_LIB_DATE);
		pInfo->major = SII_LIB_MAJOR;
		pInfo->minor = SII_LIB_MINOR;
		strcpy_s(pInfo->version, SII_LIB_VERSION);

		SpwErrorVal = SPW_NO_ERROR;
	}

	/// Returns information about the driver.
	DLL_EXPORT enum           SpwRetVal SiGetDriverInfo (SiVerInfo *pInfo)
	{
		CComPtr<IUnknown> tdxinfo;
		CComPtr<ITDxInfo>	pTDxInfo;
		HRESULT hr = ::CoInitializeEx(0, COINIT_APARTMENTTHREADED | COINIT_SPEED_OVER_MEMORY);
		if (!SUCCEEDED(hr))
		{
			ATLTRACE("3Dconnexion plugin [ %l ]: CoInitializeEx failed!\n", hr);
			return SPW_ERROR;
		}
		// create the COM device object
		HRESULT hrinfo = tdxinfo.CoCreateInstance(__uuidof(TDxInfo));
#ifdef _DEBUG
		ATLTRACE("3Dconnexion plugin [ %d ]: TDxInfo COM instance = %d\n", hrinfo,  tdxinfo.p);
#endif			
		if (!SUCCEEDED(hrinfo)) 
		{
			return SPW_ERROR;
		}
					hrinfo = tdxinfo.QueryInterface(&pTDxInfo);
#ifdef _DEBUG
			ATLTRACE("3Dconnexion plugin [ %d ]: COM TDxInfo interface = %p\n", hrinfo, pTDxInfo.p);
#endif

		tdxinfo.Release();

		BSTR bNumberString;
		if (!SUCCEEDED(pTDxInfo->RevisionNumber(&bNumberString))) 
		{
			return SPW_ERROR;
		}

		_bstr_t tmpBstr = bNumberString;
		LPCSTR tmp = tmpBstr;
		string sNumberString = tmp;
		int majorIndex = sNumberString.find_first_of(".", 0);
		int minorIndex = sNumberString.find_first_of(".", majorIndex + 1);		

		pInfo->major = atoi(sNumberString.substr(0, majorIndex).c_str());
		pInfo->minor = atoi(sNumberString.substr(majorIndex + 1, minorIndex - 1 - majorIndex).c_str());
		pInfo->build = atoi(sNumberString.substr(minorIndex + 1, sNumberString.length() - 1 - minorIndex).c_str());
		SysFreeString(bNumberString);

		memset(&(pInfo->version), '\0', SI_STRSIZE);

		string version (SII_DVR_VERSION);
		version._Copy_s(pInfo->version, SI_STRSIZE, version.length(), 0);

		memset(&(pInfo->date), '\0', SI_STRSIZE);
		string date (SII_DVR_DATE);
		date._Copy_s(pInfo->date, SI_STRSIZE, date.length(), 0);

		return SPW_NO_ERROR;
	}

	/// Returns the pressed buttons in the passed event.
	DLL_EXPORT int			SiButtonPressed (SiSpwEvent *pEvent)
	{
		if (pEvent->type != SI_BUTTON_EVENT)
			return SPW_ERROR;
		else if (!pEvent->u.spwData.bData.pressed)
		{			
			return SI_NO_BUTTON;
		}
		else
		{
			SpwErrorVal = SPW_NO_ERROR;			
			return pEvent->u.spwData.bData.pressed;
		}
	}
	/// Returns the released buttons in the passed event.
	DLL_EXPORT int			SiButtonReleased (SiSpwEvent *pEvent)
	{
		if (pEvent->type != SI_BUTTON_EVENT)
			return SPW_ERROR;
		else if (!pEvent->u.spwData.bData.released)
		{			
			return SI_NO_BUTTON;
		}
		else
		{
			SpwErrorVal = SPW_NO_ERROR;			
			return pEvent->u.spwData.bData.released;
		}
	}

	/// Returns the human readable error messages according to the passed SpwRetVal.
	DLL_EXPORT char*		SpwErrorString (enum SpwRetVal val)
	{	
		static char *errorString;
		switch (val)
		{
		case SPW_NO_ERROR:
			errorString = "No error.";
			break;
		case SPW_ERROR:
			errorString = "Error -- function failed.";
			break;
		case SI_BAD_HANDLE:
			errorString = "Invalid SpaceWare handle.";
			break;
		case SI_BAD_ID:
			errorString = "Invalid device ID.";
			break;
		case SI_BAD_VALUE:
			errorString = "Invalid argument value.";
			break;
		case SI_IS_EVENT:
			errorString = "Event is a SpaceWare event.";
			break;
		case SI_SKIP_EVENT:
			errorString = "Skip this SpaceWare event.";
			break;
		case SI_NOT_EVENT:
			errorString = "Event is not a SpaceWare event.";
			break;
		case SI_NO_DRIVER:
			errorString = "SpaceWare driver is not running.";
			break;
		case SI_NO_RESPONSE:
			errorString = "SpaceWare driver is not responding.";
			break;
		case SI_UNSUPPORTED:
			errorString = "The function is unsupported by this version.";
			break;
		case SI_UNINITIALIZED:
			errorString = "SpaceWare input library is uninitialized.";
			break;
		case SI_WRONG_DRIVER:
			errorString = "Driver is incorrect for this SpaceWare version.";
			break;
		case SI_INTERNAL_ERROR:
			errorString = "Internal SpaceWare error.";
			break;
		case SI_BAD_PROTOCOL:
			errorString = "The transport protocol is unknown.";
			break;
		case SI_OUT_OF_MEMORY:
			errorString = "Unable to malloc space required.";
			break;
		case SPW_DLL_LOAD_ERROR:
			errorString = "Could not load siapp dlls.";
			break;
		case SI_NOT_OPEN:
			errorString = "Spaceball device not open.";
			break;
		case SI_ITEM_NOT_FOUND:
			errorString = "Item not found.\n";
			break;
		case SI_UNSUPPORTED_DEVICE:
			errorString = "The device is not supported.";
			break;
		case SI_NOT_ENOUGH_MEMORY:
			errorString = "Not enough memory (but not a malloc problem).";
			break;
		case SI_SYNC_WRONG_HASHCODE:
			errorString = "Wrong hash code sent to a Sync function.";
			break;
		default:
			return NULL;
		}
		return errorString;
	}

	/// Returns the human readable name of a button assigned in AerionInput.
	DLL_EXPORT enum SpwRetVal	SiGetButtonName (SiHdl hdl, SPWuint32 buttonNumber, SiButtonName *pname)
	{
		if (pname == NULL)
		{
			return SI_BAD_VALUE;
		}
		if (hdl == NULL)
		{
			return SI_BAD_HANDLE;
		}
		
		NDOFServer *pNdofServer = static_cast<NDOFServer *> (hdl);

		BSTR buttonName;
		if (!SUCCEEDED(pNdofServer->get_Keyboard()->GetKeyName(buttonNumber, &buttonName)))
		{
			return SI_UNINITIALIZED;
		}
		_bstr_t resultButtonName = buttonName;
		strcpy_s(pname->name, resultButtonName);

		return SPW_NO_ERROR;
	}

	/// Returns the human readable name of the device passed.
	DLL_EXPORT enum	SpwRetVal	SiGetDeviceName (SiHdl hdl, SiDeviceName *pname)
	{
		if (pname == NULL)
		{
			return SI_BAD_VALUE;
		}

		int type = SiGetDeviceID(hdl);

		switch (type)
		{
		case SI_SPACEBALL_2003:
			strcpy_s(pname->name, "Spaceball 2003");
			break; 
		case SI_SPACEBALL_3003:
			strcpy_s(pname->name, "Spaceball 3003");
			break; 
		case SI_SPACE_CONTROLLER:
			strcpy_s(pname->name, "Space controller");
			break; 
		case SI_AVENGER:
			strcpy_s(pname->name, "Avenger");
			break; 
		case SI_SPACEORB_360:
			strcpy_s(pname->name, "Spaceorb 360");
			break; 
		case SI_NAVIGATOR:
			strcpy_s(pname->name, "Navigator");
			break; 
		case SI_SPACEBALL_2003A:
			strcpy_s(pname->name, "Spaceball 2003A");
			break; 
		case SI_SPACEBALL_2003B:
			strcpy_s(pname->name, "Spaceball 2003B");
			break; 
		case SI_SPACEBALL_2003C:
			strcpy_s(pname->name, "Spaceball 2003C");
			break; 
		case SI_SPACEBALL_3003A:
			strcpy_s(pname->name, "Spaceball 3003A");
			break; 
		case SI_SPACEBALL_3003B:
			strcpy_s(pname->name, "Spaceball 3003B");
			break; 
		case SI_SPACEBALL_3003C:
			strcpy_s(pname->name, "Spaceball 3003C");
			break; 
		case SI_SPACEBALL_4000:
			strcpy_s(pname->name, "Spaceball 4000");
			break; 
		case SI_SPACEMOUSE_CLASSIC:
			strcpy_s(pname->name, "Spacemouse Classic");
			break; 
		case SI_SPACEMOUSE_PLUS:
			strcpy_s(pname->name, "Spacemouse Plus");
			break; 
		case SI_SPACEMOUSE_XT:
			strcpy_s(pname->name, "Spacemouse XT");
			break; 
		case SI_CYBERMAN:
			strcpy_s(pname->name, "Cyberman");
			break; 
		case SI_CADMAN:
			strcpy_s(pname->name, "Cadman");
			break; 
		case SI_SPACEMOUSE_CLASSIC_PROMO:
			strcpy_s(pname->name, "Spacemouse Classic Promo");
			break; 
		case SI_SERIAL_CADMAN:
			strcpy_s(pname->name, "Serial Cadman");
			break; 
		case SI_SPACEBALL_5000:
			strcpy_s(pname->name, "Spaceball 5000");
			break; 
		case SI_TEST_NO_DEVICE:
			strcpy_s(pname->name, "TEST, NO DEVICE");
			break; 
		case SI_3DX_KEYBOARD_BLACK:
			strcpy_s(pname->name, "3DX Keyboard Black");
			break; 
		case SI_3DX_KEYBOARD_WHITE:
			strcpy_s(pname->name, "3DX Keyboard White");
			break; 
		case SI_TRAVELER:
			strcpy_s(pname->name, "Traveler");
			break; 
		case SI_TRAVELER1:
			strcpy_s(pname->name, "Traveler 1");
			break; 
		case SI_SPACEBALL_5000A:
			strcpy_s(pname->name, "Spaceball 5000A");
			break; 
		case SI_SPACEDRAGON:
			strcpy_s(pname->name, "Spacedragon");
			break; 
		case SI_SPACEPILOT:
			strcpy_s(pname->name, "Spacepilot");
			break; 
		case SI_AERION:
			strcpy_s(pname->name, "Aerion");
			break; 

		default: //SI_UNKNOWN_DEVICE
			return SPW_ERROR;
		}

		return SPW_NO_ERROR;
	}

	/// Unknown purpose of function, I implemented something like "SiGetDeviceType()"
	/// which returns the device type (e.g. Space Traveller, Aerion, ...).
	DLL_EXPORT SiDevID			SiGetDeviceID(SiHdl hdl)
	{
		if (hdl == NULL)
		{
			SpwErrorVal = SI_BAD_HANDLE;
			return SI_UNKNOWN_DEVICE;
		}
		
		NDOFServer *pNdofServer = static_cast<NDOFServer *> (hdl);
		long type;
		if (!SUCCEEDED(pNdofServer->get_Device()->get_Type(&type)))
		{
			return SI_UNINITIALIZED;
		}
		int typeResult = static_cast<int> (type);
		SpwErrorVal = SPW_NO_ERROR;
		return typeResult;
	}

	//stubs, must be exported for GeoVision

	/// Aerion cannot beep. Will not be implemented.
	DLL_EXPORT enum			SpwRetVal SiBeep (SiHdl hdl, char *string)
	{
		return SI_UNSUPPORTED;
	}

	/// AerionInput supports only one device at a time for now.
	DLL_EXPORT int            SiGetNumDevices (void)
	{
		if (vNDOFServers.size() < 1) 
		{
			SpwErrorVal = SI_UNINITIALIZED;
			return -1;
		}
		//TODO: Testen!
		VARIANT_BOOL connected;
		if (!SUCCEEDED(vNDOFServers.front()->get_Device()->get_IsConnected(&connected)))
		{
			SpwErrorVal = SI_UNINITIALIZED;
			return -1;
		}
		else if (connected)
		{
			SpwErrorVal = SPW_NO_ERROR;
			return 1;
		}
		else 
		{
			SpwErrorVal = SPW_NO_ERROR;
			return 0;
		}
	}

	/// Unknown purpose of function, probably the index position in a list of connected devices.
	/// As with SiGetNumDevices() returning 0 should be the right thing to do here.
	DLL_EXPORT SiDevID        SiDeviceIndex (int idx)
	{
		SpwErrorVal = SPW_NO_ERROR;
		return NULL;
	}

	// Dispatching events to other handlers is not supported at the moment.
	DLL_EXPORT int            SiDispatch (SiHdl hdl, SiGetEventData *pData,
		SiSpwEvent *pEvent, SiSpwHandlers *pDHandlers)
	{
		return SI_UNSUPPORTED;
	}

	/// Sets current position as rest position. This needs the motion data
	/// structure be stored in NDOFServer which is not the case at the moment.
	/// Even if, it is not that easy to set the current position to the origin
	/// because one must change the union mData structure which is not available
	/// all the time. By the way there has to be an offset to change the origin...
	DLL_EXPORT enum			SpwRetVal SiRezero (SiHdl hdl)
	{
		return SI_UNSUPPORTED;
	}

	/// Changes exclusiveness of the device access. If exclusive access is granted,
	/// PostMessage() sends not only to the active hWnd but to every hWnd.
	/// Can be implemented quite easily.
	DLL_EXPORT enum			SpwRetVal SiGrabDevice (SiHdl hdl, SPWbool exclusive)
	{
		return SI_UNSUPPORTED;
	}

	/// Changes PostMessage() behavoir to default. See SiGrabDevice().
	DLL_EXPORT enum			SpwRetVal SiReleaseDevice (SiHdl hdl)
	{
		return SI_UNSUPPORTED;
	}

	/// There will be no GUI features for AerionInput. Will not be implemented.
	DLL_EXPORT enum			SpwRetVal SiSetUiMode (SiHdl hdl, SPWuint32 mode)
	{
		return SI_UNSUPPORTED;
	}

	/// Unknown purpose of function. Will not be implemented.
	DLL_EXPORT enum         SpwRetVal SiGetDevicePort (SiDevID devID, SiDevPort *pPort)
	{
		return SI_UNSUPPORTED;
	}	

	/// Unknown purpose of function. Use SiGetEvent() instead. Will not be implemented.
	DLL_EXPORT boolean_t		SiIsSpaceWareEvent(SiGetEventData *pData, SiHdl *pHdl)
	{
		SpwErrorVal = SI_UNSUPPORTED;
		return SPW_FALSE;
	}


#ifdef __cplusplus
}
#endif


/*************************************************************************
**************************************************************************
*******                 NDOFServer implementation           **************
**************************************************************************
**************************************************************************
*******		cannot be split into two seperate files because **************
*******		of ATL module attribute going mad if doing so   **************
*******		(implements dllmain() twice...)                 **************
*******		If I have some more time I will reimplement     **************
*******		with .def file some day...						**************
**************************************************************************
**************************************************************************/


//getter/setter

bool NDOFServer::get_PEventInitialized()
{
	return m_pEventInitialized;
}

void NDOFServer::set_PEventInitialized(bool value) 
{
	m_pEventInitialized = value;
}


_bstr_t NDOFServer::get_AppName() 
{
	return m_AppName;
}

void NDOFServer::set_AppName(_bstr_t value)
{
	m_Device->LoadPreferences(value);			
	m_AppName = value;
}

HWND NDOFServer::get_Hwnd() 
{
	return m_hWnd;
}

void NDOFServer::set_Hwnd(HWND value)
{
	m_hWnd = value;
}

CComPtr<ISensor> NDOFServer::get_Sensor()
{
	return m_3DSensor;
}

CComPtr<ISimpleDevice> NDOFServer::get_Device()
{
	return m_Device;
}

CComPtr<IKeyboard> NDOFServer::get_Keyboard()
{
	return m_Keyboard;
}

CComPtr<ITDxInfo> NDOFServer::get_TDxInfo()
{
	return m_TDxInfo;
}


void NDOFServer::readSensorData(SiSpwEvent *pEvent) 
{
	static double ndofData[7];

	if (m_3DSensor)// && sNdofStatus)
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

			pEvent->u.spwData.period = static_cast<long>(ndofData[6]);

			//sNdofStatus->changed = 1;
			//sNdofStatus->delta = ndofData[6] - sNdofStatus->time;
			//sNdofStatus->time = ndofData[6];

			// Check if the cap is still displaced
			if (len > 0.0 || ang > 0.0)
			{
				pTra->get_X(&ndofData[0]);
				pTra->get_Y(&ndofData[1]);
				pTra->get_Z(&ndofData[2]);
				pRot->get_X(&ndofData[3]);
				pRot->get_Y(&ndofData[4]);
				pRot->get_Z(&ndofData[5]);

				pEvent->type = SI_MOTION_EVENT;
				pEvent->u.spwData.mData[SI_TX] = static_cast<long>(ndofData[0]);
				pEvent->u.spwData.mData[SI_TY] = static_cast<long>(ndofData[1]);
				pEvent->u.spwData.mData[SI_TZ] = static_cast<long>(ndofData[2]);

				pEvent->u.spwData.mData[SI_RX] = static_cast<long>(ndofData[3]);
				pEvent->u.spwData.mData[SI_RY] = static_cast<long>(ndofData[4]);
				pEvent->u.spwData.mData[SI_RZ] = static_cast<long>(ndofData[5]);

				//cerr << "plugin: [" << sNdofStatus->tx << " "
				//	 << sNdofStatus->ty << " " << sNdofStatus->tz << "] ["
				//	 << sNdofStatus->rx << " " << sNdofStatus->ry << " "
				//	 << sNdofStatus->rz << "]\n" ;
			}
			else 
			{	// zero out axisData so that 3d motion stops.
				memset(&(pEvent->u.spwData.mData[SI_TX]), 0, sizeof(long) * 6);
			}

			pRot.Release();
			pTra.Release();
		}
		catch (...)
		{
			ATLTRACE("3Dconnexion plugin: exception while fetching device sensor data.");
		}
	}
}


void NDOFServer::readKeyboardData(SiSpwEvent *pEvent) 
{
	pEvent->type = SI_BUTTON_EVENT;
	pEvent->u.spwData.bData.current = m_Keys.current;
	pEvent->u.spwData.bData.last = m_Keys.last;
	pEvent->u.spwData.bData.released = m_Keys.released;
	pEvent->u.spwData.bData.pressed = m_Keys.pressed;
}

NDOFServer::~NDOFServer()
{
	shutdown();
}

void* NDOFServer::open()
{
	if (initCOM())
		return NULL;

	return this;
}

// Returns 0 if no error. 
int NDOFServer::initCOM()
{
	HRESULT hr;
	int err = -1;
	CComPtr<IUnknown> cnx_dev;
	hr = ::CoInitializeEx(0, COINIT_APARTMENTTHREADED | COINIT_SPEED_OVER_MEMORY);
	if (!SUCCEEDED(hr))
	{
		ATLTRACE("3Dconnexion plugin [ %l ]: CoInitializeEx failed!\n", hr);
		return err;
	}

	// create the COM device object
	hr = cnx_dev.CoCreateInstance(__uuidof(Device));
#ifdef _DEBUG
	ATLTRACE("3Dconnexion plugin [ %d ]: main COM instance = %d\n", hr,  cnx_dev.p);
#endif

	if (SUCCEEDED(hr)) {
		//CComPtr<ISimpleDevice> m_Device;

		hr = cnx_dev.QueryInterface(&m_Device);
#ifdef _DEBUG
		ATLTRACE("3Dconnexion plugin [ %d ]: COM device interface = %p\n", hr, m_Device.p);
#endif

		if (SUCCEEDED(hr)) {

			// register for DeviceChange event
			hr = __hook(&_ISimpleDeviceEvents::DeviceChange, m_Device, 
				&NDOFServer::OnDeviceChange);
			if (FAILED(hr))
				ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register for device change events.\n", hr);
			// get the actual interface to the sensor and register for events
			hr = m_Device->get_Sensor(&m_3DSensor);
#ifdef _DEBUG
			ATLTRACE("3Dconnexion plugin [ %d ]: sensor handle = %p\n", m_3DSensor.p);
#endif
			assert(m_3DSensor.p != NULL);
			if (SUCCEEDED(hr) && m_3DSensor.p)
			{
				hr = __hook(&_ISensorEvents::SensorInput, m_3DSensor, 
					&NDOFServer::OnSensorInput);
				if (FAILED(hr))
					ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register " \
					"for device sensor events.\n", hr);
			}

			// get the interface to the device keys and register for events
			HRESULT hrk = m_Device->get_Keyboard(&m_Keyboard);
#ifdef _DEBUG
			ATLTRACE("3Dconnexion plugin [ %d ]: keyboard handle = %p\n", m_Keyboard.p);
#endif
			if (SUCCEEDED(hrk) && m_Keyboard.p)
			{
				hrk = __hook(&_IKeyboardEvents::KeyDown, m_Keyboard, 
					&NDOFServer::OnDevKeyDown);
				if (FAILED(hrk))
					ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register for device button events.\n",
					hrk);
				hrk = __hook(&_IKeyboardEvents::KeyUp, m_Keyboard, 
					&NDOFServer::OnDevKeyUp);
				if (FAILED(hrk))
					ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register for device button events.\n",
					hrk);
			}
			// connect to the driver
			if (SUCCEEDED(hr) && SUCCEEDED(hrk) //&& SUCCEEDED(hrinfo)
				&& m_Device->Connect() == S_OK)
				err = 0;
		}
		cnx_dev.Release();
		//tdxinfo.Release();
	}

	ATLTRACE("3Dconnexion device initialization ");
	ATLTRACE((err ? "NOT" : ""));
	ATLTRACE("successful\n");

	return err;
}

void NDOFServer::shutdown()
{
	if (m_3DSensor)
	{		
		//CComPtr<ISimpleDevice> m_Device;
		//m_3DSensor->get_Device((IDispatch**)&m_Device);

		// unhook (unadvise) the sensor event sink
		__unhook(&_ISensorEvents::SensorInput, m_3DSensor, 
			&NDOFServer::OnSensorInput);
		__unhook(&_IKeyboardEvents::KeyDown, m_Keyboard, 
			&NDOFServer::OnDevKeyDown);
		__unhook(&_IKeyboardEvents::KeyUp, m_Keyboard, 
			&NDOFServer::OnDevKeyUp);
		__unhook(&_ISimpleDeviceEvents::DeviceChange, m_Device, 
			&NDOFServer::OnDeviceChange);

		m_3DSensor.Release();
		m_Keyboard.Release();

		// disconnect from the driver
		if (m_Device)
		{
			m_Device->Disconnect();
			m_Device.Release();
		}
	}
	ATLTRACE("3Dconnexion device shutdown.\n");

}

HRESULT NDOFServer::OnSensorInput()
{

	static double ndofData[6];

	if (m_3DSensor)// && sNdofStatus)
	{
		PostMessage(GetActiveWindow(), WM_SIAPP_AXIS, (WPARAM) GetActiveWindow(), NULL);
	}

	return S_OK;
}

HRESULT NDOFServer::OnDevKeyDown(long keycode)
{
	if (m_Keyboard)// && sNdofStatus)
	{
		try 
		{
			//sNdofStatus->type = SI_BUTTON_EVENT;
			m_Keys.last = m_Keys.current;

			bitset<32> pressed = static_cast<unsigned long> (pow(2.0, keycode));
			m_Keys.current += pressed.to_ulong();
			ATLTRACE("3Dconnexion plugin: current: %d\n", m_Keys.current);
			m_Keys.pressed = pressed.to_ulong();
			m_Keys.released = 0;
			ATLTRACE("3Dconnexion plugin: pressed: %d\n", m_Keys.pressed);
			ATLTRACE("3Dconnexion plugin: released: %d\n", m_Keys.released);

			PostMessage(GetActiveWindow(), WM_SIAPP_BTN, (WPARAM) GetActiveWindow(), NULL);
		}
		catch (...)
		{
			ATLTRACE("3Dconnexion plugin: exception while fetching device keyboard data.\n");
		}
	}

	return S_OK;

}

HRESULT NDOFServer::OnDevKeyUp(long keycode)
{
	if (m_Keyboard)// && sNdofStatus)
	{
		try 
		{
			//sNdofStatus->type = SI_BUTTON_EVENT;
			m_Keys.last = m_Keys.current;

			bitset<32> released = static_cast<unsigned long> (pow(2.0, keycode));

			m_Keys.current -= released.to_ulong();
			ATLTRACE("3Dconnexion plugin: current: %d\n", m_Keys.current);
			m_Keys.released = released.to_ulong();
			m_Keys.pressed -= released.to_ulong();
			ATLTRACE("3Dconnexion plugin: pressed: %d\n", m_Keys.pressed);
			ATLTRACE("3Dconnexion plugin: released: %d\n", m_Keys.released);
			PostMessage(GetActiveWindow(), WM_SIAPP_BTN, (WPARAM) GetActiveWindow(), NULL);
		}
		catch (...)
		{
			ATLTRACE("3Dconnexion plugin: exception while fetching device keyboard data.\n");
		}
	}

	return S_OK;

}

HRESULT NDOFServer::OnDeviceChange(long reserved)
{
	HRESULT hr;
	HRESULT hrk;
	if (m_Device)
	{
		try 
		{		
			// unhook (unadvise) the sensor event sink
			__unhook(&_ISensorEvents::SensorInput, m_3DSensor, 
				&NDOFServer::OnSensorInput);
			__unhook(&_IKeyboardEvents::KeyDown, m_Keyboard, 
				&NDOFServer::OnDevKeyDown);
			__unhook(&_IKeyboardEvents::KeyUp, m_Keyboard, 
				&NDOFServer::OnDevKeyUp);
		}
		catch (...)
		{
			ATLTRACE("3Dconnexion plugin: exception while unregistering device change event sink.\n");
		}

		try 
		{

			if (m_3DSensor.p)
			{
				hr = __hook(&_ISensorEvents::SensorInput, m_3DSensor, 
					&NDOFServer::OnSensorInput);
				if (FAILED(hr))
					ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register for device sensor events.\n",
					hr);
			}

			// get the interface to the device keys and register for events

			if (m_Keyboard.p)
			{
				hrk = __hook(&_IKeyboardEvents::KeyDown, m_Keyboard, 
					&NDOFServer::OnDevKeyDown);
				if (FAILED(hrk))
					ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register for device button events.\n",
					hrk);
				hrk = __hook(&_IKeyboardEvents::KeyUp, m_Keyboard, 
					&NDOFServer::OnDevKeyUp);
				if (FAILED(hrk))
					ATLTRACE("3Dconnexion plugin [ %d ]: FAILED to register for device button events.\n",
					hrk);
			}

			// connect to the driver
			if (!(SUCCEEDED(hr) && SUCCEEDED(hrk) 
				&& m_Device->Connect() == S_OK))
			{
				ATLTRACE("3Dconnexion plugin: Connecting failed.\n");
				//				err = 0;
			}

			// specify our ID (useful if blender can host multiple plugins 
			// using our device)
			m_Device->LoadPreferences(m_AppName);			
		}
		catch (...)
		{
			ATLTRACE("3Dconnexion plugin: exception while re-registering device change event sink.\n");
		}
	}

	return S_OK;
}
