@echo off

cd %~dp0
echo 拷贝所需表文件（table目录以及table\client目录下的所有.xlsx文件）到客户端目录
rem 只拷贝table目录下 客户端和服务器通用表文件，不拷贝文件夹
xcopy "..\..\table\*" "%~dp0\table\" /Y
rem 只拷贝table\client目录下 客户端表文件，不拷贝文件夹
xcopy ..\..\table\client\* %~dp0\table\ /Y



set /p platform=请输入平台（1:windows pc、2:android、3:ios、4:webgl）

rem 打印bat文件所在的目录
echo bat file path is:%~dp0
rem 定位到ExcelTool.exe所在的文件夹
cd %~dp0/Debug/


setlocal EnableDelayedExpansion
REM 定义参数（用户可直接修改这些值）
set "param1=table"
set "param2=table_tmp"
set "param3=table_output"
set "param4=code_output_cs"
set "param5=code_output_lua"
set "param6=true"
set "param7=true"
set "param8=false"

rem 启动目标exe
ExcelTool.exe %param1% %param2% %param3% %param4% %param5% %param6% %param7% %param8%

if /i "%param8%"=="true" (
	rem 拷贝lua文件到目标文件夹
	xcopy ..\code_output_lua\*.* ..\..\project\LuaScript\Table /s /e /c /y /h /r
)

if /i "%param7%"=="true" (
	rem 拷贝代码文件到目标文件夹
	xcopy ..\code_output_cs\*.* ..\..\Assets\Scripts\Table\code_output_cs\ /s /e /c /y /h /r
)

if /i "%param6%"=="true" (
	rem 拷贝数据表文件到目标文件夹
	if %platform% equ 1 (
		echo 拷贝数据表文件到 windows 平台对应目录
		xcopy ..\table_output\*.* ..\..\ResEx\pc_win\ /s /e /c /y /h /r
	) else if %platform% equ 2 (
	echo 拷贝数据表文件到 android 平台对应目录
		xcopy ..\table_output\*.* ..\..\ResEx\android\ /s /e /c /y /h /r
	) else if %platform% equ 3 (
		echo 拷贝数据表文件到 ios 平台对应目录
		xcopy ..\table_output\*.* ..\..\ResEx\ios\ /s /e /c /y /h /r
	) else if %platform% equ 4 (
		echo 拷贝数据表文件到 webgl 平台对应目录
		xcopy ..\table_output\*.* ..\..\ResEx\webgl\ /s /e /c /y /h /r
	) else (
		echo 无效的参数值，请提供 1、2、3、4
	)
)
endlocal

pause