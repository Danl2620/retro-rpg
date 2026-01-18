
PipelineVer := "4.0.0"
NugetCachePath := justfile_directory() / "packages"
PipelineDllPath := NugetCachePath / "MonoGame.Extended.Content.Pipeline" / "tools"

# Show interactive task chooser
_default:
    just --choose

# Restore .NET tools and Paket dependencies
[group('setup')]
restore:
    dotnet tool restore
    dotnet tool run paket restore -v

# Delete all built files
[group('setup')]
clean:
    git clean -fdx

# Build the project
[group('development')]
build:
    dotnet build

# Run the game
[group('development')]
run:
    DISPLAY=:0 dotnet run
