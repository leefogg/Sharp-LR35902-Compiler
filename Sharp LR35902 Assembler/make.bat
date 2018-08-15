@echo off
dotnet build -c Release -r win-x64
dotnet build -c Release -r win-x86
dotnet build -c Release -r osx-x64
dotnet build -c Release -r linux-x64
echo.
echo Check /bin/Release
echo.
pause