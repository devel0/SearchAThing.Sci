rem echo Off

set localrun="0"

REM EDIT : set to 1 for local run
set localrun="1"

echo "argument 1 = [%1]"

set config=%1
if "%config%" == "" (
   set config=Release
)

echo "config = [%config%]"

set vv=-Version 1.0.0
if not "%PackageVersion%" == "" (
   set vv=-Version %PackageVersion%
)

echo "version = [%vv%]"

if "%localrun%" == "0" (
	set msbuild=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe
)

if "%localrun%" == "1" (
	set msbuild=%ProgramFiles(x86)%\Microsoft Visual Studio\Preview\Community\MSBuild\15.0\Bin\MSBuild.exe
)

echo "msbuild = [%msbuild%]"

if "%nuget%" == "" (
  set nuget=c:\nuget\nuget.exe
)

REM Build
echo:
echo "====================> Build"

call "%msbuild%" SearchAThing.Sci.sln /t:Restore,Rebuild /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Package
echo:
echo "====================> Package"

rem mkdir Build
rem call %nuget% pack "src\SearchAThing.Sci.csproj" -symbols -o Build -p Configuration=%config% %vv%
rem if not "%errorlevel%"=="0" goto failure


REM Code Coverage
echo:
echo "====================> Coverage"

call %nuget% install xunit.runner.console -Version 2.2.0 -OutputDirectory packages
if not "%errorlevel%"=="0" goto failure

call %nuget% install OpenCover -Version 4.6.519 -OutputDirectory packages
if not "%errorlevel%"=="0" goto failure

packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -target:"packages\xunit.runner.console.2.2.0\tools\xunit.console.exe" -targetargs:".\tests\bin\Release\SearchAThing.Sci.Tests.dll -noshadow" -output:".\coverage.xml" "-filter:+[*]* -[*]Microsoft.Xna.*"
if not "%errorlevel%"=="0" goto failure

echo "---> ensuring codecov"
call npm install codecov -g > nul
if not "%errorlevel%"=="0" goto failure

echo "---> running codecov -f coverage.xml"
codecov -f coverage.xml
if not "%errorlevel%"=="0" goto failure

:success
if "%localrun%" == "0" (
	exit 0
)

:failure
if "%localrun%" == "0" (
	exit -1
)