mkdir c:\Programme\AerionInput
copy hidlibrary.dll c:\Programme\AerionInput
copy tdxinput.dll c:\Programme\AerionInput
copy AerionInputConfig.exe c:\Programme\AerionInput
%WINDIR%\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe c:\Programme\AerionInput\tdxinput.dll /tlb:c:\Programme\AerionInput\tdxinput.tlb /codebase c:\Programme\AerionInput\tdxinput.dll
rem %WINDIR%\Microsoft.NET\Framework\v2.0.50727\gacutil -i c:\Programme\AerionInput\tdxinput.dll
regedit progid_device_register.reg