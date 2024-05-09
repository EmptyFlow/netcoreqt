&"dotnet" publish NetCoreQtLibrary.csproj -c Release -p:EnableDynamicLoading=true
Copy-Item bin\Release\win-x64\publish\*.* ../../../../runtimeassemblydlls/