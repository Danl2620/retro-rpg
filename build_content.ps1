#Requires -Version 6.0
$ErrorActionPreference = "Stop"

## dotnet /Users/dliebgold/.nuget/packages/monogame.content.builder.task/3.8.0.1641/tools/netcoreapp3.1/any/mgcb.dll `
##    /f:"../../../../.nuget/packages/monogame.extended.content.pipeline/3.8.0/tools/MonoGame.Extended.Content.Pipeline.dll" `
## /f:"/Users/dliebgold/proj/misc/MonoGame.Extended/src/cs/MonoGame.Extended.Content.Pipeline/bin/netcoreapp3.1/publish/MonoGame.Extended.Content.Pipeline.dll" `

dotnet tool run mgcb `
    /f:"../../../../.nuget/packages/monogame.extended.content.pipeline/3.8.0/tools/MonoGame.Extended.Content.Pipeline.dll" `
    /@:"Content/Content.mgcb" `
    /platform:DesktopGL `
    /outputDir:"Content/bin/DesktopGL/Content" `
    /intermediateDir:"Content/obj/DesktopGL/Content" `
    /workingDir:"Content"