@echo off
set SLN=%~dp0AspNetCoreLoggingTest.sln
echo VS environment must be initiated first...
echo Starting VisualStudio with %SLN%...
devenv %SLN%
echo Done