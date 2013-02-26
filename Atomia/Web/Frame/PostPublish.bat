Rem File: PostPublish.bat 
Rem Company: Atomia AB
Rem Copyright (C) 2009 Atomia AB. All rights reserved
Rem Author: Aleksandar Vucetic
Rem Updater: Dusan Milenkovic
Rem Updater: Ilija Nikolic

@ECHO off

Rem =======================================
Rem Settings
Rem =======================================

Rem Set all plugins and to which theme content files should be copied
Rem Names should start with plugins_ followed by name of the directory where plugins project is located.
SET plugins_PublicOrder=PublicOrder
SET plugins_PaymentForm=PaymentForm


Rem ========================================
Rem Do the work
Rem ========================================

Rem For each defined plugin
FOR /F "tokens=2* delims=_=" %%A IN ('SET plugins') DO (
	 md "%~dp0publish\files\Themes\Default\Scripts"
	 xcopy /s /e /y /I "%~dp0Themes\Default\Scripts" "%~dp0publish\files\Themes\Default\Scripts"
	 md "%~dp0publish\files\Themes\Default\Scripts\PluginScripts\%%A"
	 xcopy /s /e /y /I "%~dp0..\Plugins\%%A\bin\%1" "%~dp0publish\files\bin"
)
xcopy /e /y /I "%~dp0bin" "%~dp0publish\files\bin"

Rem Rename Web.config, unnecessary plugin .dll files that are allready in bin\Plugins folder and create generic kit
copy /y "%~dp0publish\files\bin\Original Files" "%~dp0publish\files\bin"
copy /y "%~dp0publish\files\App_Data\Original Files" "%~dp0publish\files\App_Data"
copy /y "%~dp0publish\files\Original Files" "%~dp0publish\files"

del "%~dp0publish\files\DirtyPackageChecker.exe"
del "%~dp0publish\files\App_Data\*.xml"

md "%~dp0publish\files\App_Data\Transformation Files"
md "%~dp0publish\files\App_Data\elmah_logs"
md "%~dp0publish\files\Transformation Files"
md "%~dp0publish\files\bin\Transformation Files"