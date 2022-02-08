#Requires -Version 6.0
$ErrorActionPreference = "Stop"

dotnet tool restore
dotnet tool run mgcb Content/*.png
dotnet build                              
dotnet restore
