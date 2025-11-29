
PipelineVer := "4.0.0"
NugetCachePath := justfile_directory() / "packages"
PipelineDllPath := NugetCachePath / "MonoGame.Extended.Content.Pipeline" / "tools"

_default:
    just --choose


install-monogame:
    dotnet tool restore
    dotnet tool run paket restore -v

build-content:
    dotnet tool run mgcb \
        /f:"{{PipelineDllPath}}/MonoGame.Extended.Content.Pipeline.dll" \
        /f:"{{PipelineDllPath}}/MonoGame.Extended.dll" \
        /@:"Content/Content.mgcb" \
        /platform:DesktopGL \
        /outputDir:"Content/bin/DesktopGL/Content" \
        /intermediateDir:"Content/obj/DesktopGL/Content" \
        /workingDir:"Content"
