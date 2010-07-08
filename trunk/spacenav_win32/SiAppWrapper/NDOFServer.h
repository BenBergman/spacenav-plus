#ifndef _NDOFSERVER_H_
#define _NDOFSERVER_H_ 1

#include "stdafx.h"

[ event_receiver(com) ]
//[ module(type=dll, name="SiAppWrapper") ]

class NDOFServer 
{
public:
	NDOFServer()
	{
		m_pEventInitialized = false;
		m_Keys.current = 0;
		m_Keys.last = 0;
		m_Keys.released = 0;
		m_Keys.pressed = 0;
	}

    virtual ~NDOFServer();

	// Returns NULL if unsuccessful.
    void* open();
    
	int initCOM();
	
	// Closes connection with the COM server.
    void  shutdown();
	
	// Callback invoked by the COM server when motion input is detected.
    HRESULT OnSensorInput();

	// Callback invoked by the COM server when device keys are pressed.
	HRESULT OnDevKeyDown(long keycode);

	// Callback invoked by the COM server when device keys are pressed.
	HRESULT OnDevKeyUp(long keycode);

	// Callback invoked by the COM server when device is changed.
	HRESULT OnDeviceChange(long reserved);

	bool get_PEventInitialized();
	void set_PEventInitialized(bool value);
	
	_bstr_t get_AppName();
	void set_AppName(_bstr_t value);

	HWND get_Hwnd();
	void set_Hwnd(HWND value);

	void readSensorData(SiSpwEvent *pEvent);
	void readKeyboardData(SiSpwEvent *pEvent);

	CComPtr<ISensor> get_Sensor();
	CComPtr<ISimpleDevice> get_Device();
	CComPtr<IKeyboard> get_Keyboard();
	CComPtr<ITDxInfo> get_TDxInfo();
	
protected:


private:
    CComPtr<ISensor>    m_3DSensor;
	CComPtr<IKeyboard>  m_Keyboard;
	CComPtr<ISimpleDevice> m_Device;
	CComPtr<ITDxInfo>	m_TDxInfo;
	_bstr_t m_AppName;
	HWND m_hWnd;
	bool m_pEventInitialized;
	SiButtonData m_Keys;
};

#endif