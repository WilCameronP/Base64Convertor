set workDir=%cd%
cd %workDir%/Base64Convertor
dotnet publish ./Base64Convertor.csproj --framework netcoreapp2.1 --runtime win-x64 --self-contained --configuration Debug --output %workDir%/publish