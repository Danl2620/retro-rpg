# Retro RPG Project

## Available Tasks

This project uses [just](https://github.com/casey/just) as a task runner. Run `just` to see an interactive task chooser.

### Setup Tasks

- **`just restore`** - Restore .NET tools and Paket dependencies
  - Restores dotnet tools defined in the project
  - Runs Paket to restore package dependencies

- **`just clean`** - Delete all built files
  - Runs `git clean -fdx` to remove all untracked files and build artifacts
  - ⚠️ This will delete all untracked files, use with caution

### Development Tasks

- **`just build`** - Build the project
  - Compiles the project using `dotnet build`

- **`just run`** - Run the game
  - Launches the game with `DISPLAY=:0 dotnet run`
  - Note: Sets X11 display to `:0` (required for macOS/Linux GUI)
