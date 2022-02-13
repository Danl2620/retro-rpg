#Require -Version 6/0

##    /f:"../../../../.nuget/packages/monogame.extended.content.pipeline/3.8.0/tools/MonoGame.Extended.Content.Pipeline.dll" `

dotnet /Users/dliebgold/.nuget/packages/monogame.content.builder.task/3.8.0.1641/tools/netcoreapp3.1/any/mgcb.dll `
    /f:"/Users/dliebgold/proj/misc/MonoGame.Extended/src/cs/MonoGame.Extended.Content.Pipeline/bin/netcoreapp3.1/publish/MonoGame.Extended.Content.Pipeline.dll" `
    /@:"/Users/dliebgold/proj/misc/ashers_game/Content/Content.mgcb" `
    /platform:DesktopGL `
    /outputDir:"/Users/dliebgold/proj/misc/ashers_game/Content/bin/DesktopGL/Content" `
    /intermediateDir:"/Users/dliebgold/proj/misc/ashers_game/Content/obj/DesktopGL/Content" `
    /workingDir:"/Users/dliebgold/proj/misc/ashers_game/Content/"