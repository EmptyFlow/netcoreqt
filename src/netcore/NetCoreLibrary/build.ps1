&"dotnet" publish NetCoreQtLibrary.csproj -c Release -r win-x64
Copy-Item bin\Release\win-x64\publish\*.* ../../../../dlls/