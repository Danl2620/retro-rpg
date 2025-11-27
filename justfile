
PipelineVer := "4.0.0"
NugetCachePath := shell("dotnet nuget locals global-packages -l | awk '{print $2}'")
PipelineDllPath := NugetCachePath / "monogame.extended.content.pipeline" / PipelineVer / "tools"

_default:
    just --choose


install-monogame:
    dotnet add package MonoGame.Framework.DesktopGL --version 3.8.1.303
    dotnet add package MonoGame.Extended --version 3.8.0
    dotnet add package MonoGame.Extended.Content.Pipeline --version {{PipelineVer}}
    dotnet add package MonoGame.Content.Builder.Task --version 3.8.1.303

build-content:
    dotnet tool run mgcb \
        /f:"{{PipelineDllPath}}/MonoGame.Extended.Content.Pipeline.dll" \
        /f:"{{PipelineDllPath}}/MonoGame.Extended.dll" \
        /@:"Content/Content.mgcb" \
        /platform:DesktopGL \
        /outputDir:"Content/bin/DesktopGL/Content" \
        /intermediateDir:"Content/obj/DesktopGL/Content" \
        /workingDir:"Content"