&"dotnet" publish NetCoreQtLibrary.csproj -c Release -r win-x64 -p:UseAppHost=false
Copy-Item bin\Release\win-x64\publish\*.* ../../../../dlls/