#Requires -Version 6.0
$ErrorActionPreference = "Stop"

dotnet tool restore
& ./build_content.ps1
dotnet build                              
dotnet restore
