dotnet restore GladMMO.Library.sln
if not exist "build" mkdir build
start cmd /c build-norestore-client.bat
start cmd /c build-norestore-backend.bat
EXIT 0