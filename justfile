
PipelineVer := "4.0.0"
NugetCachePath := justfile_directory() / "packages"
PipelineDllPath := NugetCachePath / "MonoGame.Extended.Content.Pipeline" / "tools"

_default:
    just --choose

restore:
    dotnet tool restore
    dotnet tool run paket restore -v

build:
    dotnet build

run:
    dotnet run
