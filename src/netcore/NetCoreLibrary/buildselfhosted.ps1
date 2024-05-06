&"dotnet" publish NetCoreQtLibrary.csproj -c Release --self-contained true
Copy-Item bin\Release\win-x64\publish\*.* ../../../../selfhosteddlls/