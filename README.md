# Introduction

A simple retro pixel art RPG.

## Features

- Tile-based world rendering using Tiled map format (.tmx)
- Character movement with smooth tile-to-tile interpolation
- Collision detection with building layer tiles
- Camera system that follows player position
- Keyboard controls (arrow keys for movement, Escape to exit)
- Support for multiple tilesets (example maps and Oryx sprite sets)
- Orthographic camera with viewport adapter for consistent rendering

## Technology Stack

- Language: F# 8.0
- Framework: .NET 8.0
- Game Engine: MonoGame 3.8.1 (DesktopGL)
- Map Editor: Tiled (TMX format support via MonoGame.Extended)

## Dependencies

### NuGet Packages (managed via Paket)

- FSharp.Core >= 8.0.0
- MonoGame.Framework.DesktopGL 3.8.1.303
- MonoGame.Extended 4.0.0
- MonoGame.Extended.Content.Pipeline 4.0.0
- MonoGame.Content.Builder.Task 3.8.1.303

### .NET Tools

- dotnet-mgcb 3.8.1.303 (MonoGame Content Builder)
- dotnet-mgcb-editor 3.8.1.303 (MonoGame Content Builder Editor)
- paket 9.0.2 (Dependency manager)

### System Dependencies (provided via Nix)

- .NET SDK 8
- .NET Runtime 8
- SDL2 (graphics and input)
- SDL2_ttf (TrueType font support)
- SDL2_image (image loading)
- SDL2_mixer (audio mixing)
- PowerShell

## Set Up

1. Install [just](https://just.systems/man/en/packages.html)
2. Setup nixpkgs
3. `nix develop`
4. `just restore`

## Build and Run

### Available Tasks

- `just restore` - Restore .NET tools and Paket dependencies
- `just build` - Build the project using dotnet build
- `just run` - Run the game