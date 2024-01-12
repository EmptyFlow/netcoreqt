./dotnet publish NetCoreQtLibrary.csproj -c Release -r win-x64 --self-contained true
Copy-Item bin\Release\net7.0\win-x64\publish\*.* ../../../../dlls/