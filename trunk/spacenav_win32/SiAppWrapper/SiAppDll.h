#ifndef _SIAPPWRAPPER_H_
#define _SIAPPWRAPPER_H_ 1

#include "si.h"
#include "stdafx.h"

// Folgender ifdef-Block ist die Standardmethode zum Erstellen von Makros, die das Exportieren 
// aus einer DLL vereinfachen. Alle Dateien in dieser DLL werden mit dem SIAPPWRAPPER_EXPORTS-Symbol
// kompiliert, das in der Befehlszeile definiert wurde. Das Symbol darf nicht für ein Projekt definiert werden,
// das diese DLL verwendet. Alle anderen Projekte, deren Quelldateien diese Datei beinhalten, erkennen 
// SIAPPWRAPPER_API-Funktionen als aus einer DLL importiert, während die DLL
// mit diesem Makro definierte Symbole als exportiert ansieht.
#ifdef SIAPPWRAPPER_EXPORTS
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT __declspec(dllimport)
#endif

[module(dll, name="SiAppDll"), 
 idl_module(name="SiAppDll", dllname="SiAppDll.dll")];


#ifdef __cplusplus
extern "C" {
#endif

/* some enumerated types used in siapp.c */

enum InitResult 
   {
   NOT_LOADED, 
   FAILED, 
   LOADED
   };

enum ErrorCode 
   {
   NO_DLL_ERROR=0,
   DLL_LOAD_FAILURE,
   DLL_FUNCTION_LOAD_FAILURE,
   DLL_VAR_LOAD_FAILURE
   };


/// Global error value.
DLL_EXPORT SpwRetVal	  SpwErrorVal;

/* externally used functions */

// must haves for GeoVision
DLL_EXPORT void           SiTerminate(void);
DLL_EXPORT enum SpwRetVal SiClose (SiHdl hdl);
DLL_EXPORT void           SiGetEventWinInit (SiGetEventData *pData,
                                  UINT msg, WPARAM wParam, LPARAM lParam);
DLL_EXPORT enum SpwRetVal SiGetEvent (SiHdl hdl, int flags, SiGetEventData *pData, 
                           SiSpwEvent *pEvent);
DLL_EXPORT enum SpwRetVal SiInitialize(void);
DLL_EXPORT void           SiOpenWinInit (SiOpenData *pData, HWND hWnd);
DLL_EXPORT SiHdl          SiOpen (LPCSTR pAppName, SiDevID devID, SiTypeMask *pTMask, int mode, 
                       SiOpenData *pData);

DLL_EXPORT int            SiGetNumDevices (void);
DLL_EXPORT SiDevID        SiDeviceIndex (int idx);
DLL_EXPORT int            SiDispatch (SiHdl hdl, SiGetEventData *pData,
                           SiSpwEvent *pEvent, SiSpwHandlers *pDHandlers);
DLL_EXPORT enum           SpwRetVal SiGetDeviceInfo (SiHdl hdl, SiDevInfo *pInfo);
DLL_EXPORT enum           SpwRetVal SiGetDriverInfo (SiVerInfo *pInfo);

//stubs, must be exported for old applications

DLL_EXPORT enum SpwRetVal SiBeep (SiHdl hdl, char *string);
DLL_EXPORT enum SpwRetVal SiRezero (SiHdl hdl);
DLL_EXPORT enum SpwRetVal SiGrabDevice (SiHdl hdl, SPWbool exclusive);
DLL_EXPORT enum SpwRetVal SiReleaseDevice (SiHdl hdl);
DLL_EXPORT int            SiButtonPressed (SiSpwEvent *pEvent);
DLL_EXPORT int            SiButtonReleased (SiSpwEvent *pEvent);
DLL_EXPORT enum SpwRetVal SiSetUiMode (SiHdl hdl, SPWuint32 mode);
DLL_EXPORT enum           SpwRetVal SiGetDevicePort (SiDevID devID, SiDevPort *pPort);

DLL_EXPORT void           SiGetLibraryInfo (SiVerInfo *pInfo);

DLL_EXPORT char * SpwErrorString (enum SpwRetVal val);
DLL_EXPORT enum SpwRetVal SiGetButtonName (SiHdl hdl, SPWuint32 buttonNumber, SiButtonName *pname);
DLL_EXPORT enum SpwRetVal SiGetDeviceName (SiHdl hdl, SiDeviceName *pname);
// deprecated
DLL_EXPORT SiDevID			SiGetDeviceID(SiHdl hdl);
DLL_EXPORT boolean_t		SiIsSpaceWareEvent(SiGetEventData *pData, SiHdl *pHdl);


//not implemented (and no intends to..)
/*
enum SpwRetVal SiSetTypeMask (SiTypeMask *pTMask, int type1, ...);

enum SpwRetVal SiSyncSendQuery(SiHdl hdl);
enum SpwRetVal SiSyncGetVersion(SiHdl hdl, SPWuint32 *pmajor, SPWuint32 *pminor);
enum SpwRetVal SiSyncGetNumberOfFunctions(SiHdl hdl, SPWuint32 *pnumberOfFunctions);
enum SpwRetVal SiSyncGetFunction(SiHdl hdl, SPWuint32 index, SPWint32 *pabsoluteFunctionNumber, WCHAR name[], SPWuint32 *pmaxNameLen);
enum SpwRetVal SiSyncGetButtonAssignment(SiHdl hdl, SPWuint32 buttonNumber, SPWint32 *passignedFunctionIndex);
enum SpwRetVal SiSyncSetButtonAssignment(SiHdl hdl, SPWuint32 buttonNumber, SPWint32 functionIndex);
enum SpwRetVal SiSyncSetButtonAssignmentAbsolute(SiHdl hdl, SPWuint32 buttonNumber, SPWint32 absoluteFunctionNumber );
enum SpwRetVal SiSyncSetButtonName(SiHdl hdl, SPWuint32 buttonNumber, WCHAR name[]);
enum SpwRetVal SiSyncGetAxisLabel (SiHdl hdl, SPWuint32 axisNumber, WCHAR name[], SPWuint32 *pmaxNameLen );
enum SpwRetVal SiSyncSetAxisLabel (SiHdl hdl, SPWuint32 axisNumber, WCHAR name[] );
enum SpwRetVal SiSyncGetOrientation (SiHdl hdl, SPWint32 axes[6] );
enum SpwRetVal SiSyncSetOrientation (SiHdl hdl, SPWint32 axes[6] );
enum SpwRetVal SiSyncGetFilter (SiHdl hdl, SiSyncFilter i, SiSyncFilterValue *pv );
enum SpwRetVal SiSyncSetFilter (SiHdl hdl, SiSyncFilter i, SiSyncFilterValue v );
enum SpwRetVal SiSyncGetAxesState (SiHdl hdl, SiSyncAxesState *pa );
enum SpwRetVal SiSyncSetAxesState (SiHdl hdl, SiSyncAxesState a );
enum SpwRetVal SiSyncSetInfoLine (SiHdl hdl, SPWint32 duration, WCHAR text[] );
enum SpwRetVal SiSyncGetScaleOverall (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleOverall (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncGetScaleTx (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleTx (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncGetScaleTy (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleTy (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncGetScaleTz (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleTz (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncGetScaleRx (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleRx (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncGetScaleRy (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleRy (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncGetScaleRz (SiHdl hdl, SPWfloat32 *pv );
enum SpwRetVal SiSyncSetScaleRz (SiHdl hdl, SPWfloat32 v );
enum SpwRetVal SiSyncInvokeAbsoluteFunction (SiHdl hdl, SiSyncAbsFunctionNumber i );
enum SpwRetVal SiSyncSetButtonState (SiHdl hdl, SPWuint32 buttonNumber, SiSyncButtonState state );
enum SpwRetVal SiGetDeviceImageFileName (SiHdl hdl, char name[], SPWuint32 *pmaxNameLen);
HICON SiGetCompanyIcon(void);
enum SpwRetVal SiGetCompanyLogoFileName (char name[], SPWuint32 *pmaxNameLen);
*/

#ifdef __cplusplus
}
#endif

#endif