# Troubleshooting Summary

## Session Date
2026-01-17

## Problem
MonoGame game crashes immediately on launch with SIGSEGV (exit code 139) during OpenGL initialization. No game window appears.

## What We Discovered

### ✅ What Works
- .NET SDK 8.0 via Nix - loads and runs managed code successfully
- OpenGL 4.6 via Mesa 25.0.6 - confirmed working with `glxinfo`
- Direct rendering via AMD Radeon RX GPU (4GB VRAM)
- Game builds successfully
- All assets compile successfully
- Nix flake provides all necessary dependencies

### ❌ What Doesn't Work
MonoGame crashes during SDL/OpenGL context initialization, tested with:
- All display options (`:0`, `:1`, XWayland, native Xorg)
- All GPU options (Intel forced, AMD forced, software rendering)
- All SDL driver modes (x11, wayland, dummy)
- GraphicsProfile.Reach (OpenGL 2.0 compatibility mode)
- Various Mesa environment variables

### 🔍 Root Causes Identified

1. **🔴 CRITICAL: sdl2-compat Everywhere**
   - **System (pacman)**: `sdl2-compat 2.32.60-1` (required by ffmpeg/obs-studio, cannot remove)
   - **Nix environment**: `/nix/store/.../sdl2-compat-2.32.56` (nixpkgs 25.05 default)
   - MonoGame appears incompatible with sdl2-compat on this hardware configuration
   - This is NOT a library conflict - both use the same compat layer

2. **Hybrid GPU Complexity**
   - Dual Intel HD 630 + AMD Radeon RX 460
   - Intel GPU has eDP initialization failures (kernel messages)
   - VGA arbiter switching between GPUs during boot

3. **Display Stack Complexity**
   - Hyprland (Wayland) → XWayland → MonoGame
   - HiDPI Retina display (2880x1800@2x)
   - Global `SDL_VIDEODRIVER=wayland` environment variable

## Files Created

- **`claude.md`** - Project documentation with tasks and known issues
- **`system-diagnostics.md`** - Complete hardware/software diagnostics
- **`TROUBLESHOOTING-SUMMARY.md`** - This file

## Changes Made

### flake.nix
Added diagnostic tools:
```nix
mesa-demos      # provides glxinfo
xorg.xdpyinfo   # provides xdpyinfo
```

### Game1.fs
Added graphics compatibility settings:
```fsharp
graphics.GraphicsProfile <- GraphicsProfile.Reach
graphics.SynchronizeWithVerticalRetrace <- false
graphics.PreferMultiSampling <- false
```

### justfile
Updated run command:
```just
run:
    DISPLAY=:0 DRI_PRIME=0 dotnet run
```
Note: User should adjust `DISPLAY=:1` for their XWayland setup

## Next Steps to Try

### High Priority

1. **Try older nixpkgs with native SDL2** (🔴 MOST LIKELY TO WORK)
   ```nix
   # Edit flake.nix, change nixpkgs input:
   inputs = {
     nixpkgs.url = "github:NixOS/nixpkgs/nixos-24.05";  # or nixos-23.11
   };
   ```
   Then:
   ```bash
   nix flake update
   nix develop
   echo $LD_LIBRARY_PATH | head -1  # verify no sdl2-compat
   just run
   ```
   **Rationale**: Nixpkgs 25.05 uses sdl2-compat by default. Older versions may have native SDL2.
   System sdl2-compat is required by ffmpeg and cannot be removed.

2. **Test in Xorg session**
   - Boot into native Xorg instead of Hyprland
   - Eliminates Wayland/XWayland complexity
   - MonoGame expects traditional X11

### Medium Priority
3. **Disable one GPU via kernel parameters**
   - Edit `/etc/default/grub`
   - Add `modprobe.blacklist=i915` (disable Intel)
   - Run `sudo grub-mkconfig -o /boot/grub/grub.cfg`
   - Reboot and test

4. **Try different MonoGame version**
   - Update paket.dependencies
   - Test with MonoGame 3.8.0 or earlier versions
   - Try MonoGame develop branch

### Low Priority
5. **Test on different hardware**
   - Non-Apple hardware with single GPU
   - Confirms issue is MacBook-specific

6. **Alternative frameworks**
   - Godot Engine with C# support
   - Raylib with F# bindings
   - Native macOS build (not Linux)

## Key Commands

### Check System Status
```bash
# GPU drivers
lspci | grep -i vga
lsmod | grep -E 'i915|amdgpu'

# Display environment
echo $DISPLAY
echo $XDG_SESSION_TYPE
echo $SDL_VIDEODRIVER

# OpenGL test
nix develop
DISPLAY=:1 glxinfo -B

# Recent crashes
coredumpctl list | tail -5
```

### Run Game
```bash
nix develop
just run
# or manually:
DISPLAY=:1 dotnet run
```

## Hardware Configuration

- **Model**: 2019 MacBook Pro 15" Touch Bar (mpb-silver-surfer)
- **CPU**: Intel Core i7-7700HQ (4C/8T @ 2.80GHz)
- **RAM**: 16GB
- **GPU 0**: Intel HD Graphics 630
- **GPU 1**: AMD Radeon RX 460 (4GB VRAM) - Primary
- **Display**: Internal Retina 2880x1800@60Hz (2x scaling)
- **OS**: Arch Linux 6.18.3-arch1-1
- **Desktop**: Hyprland 0.53.1 (Wayland)
- **Graphics**: Mesa 25.0.6, libdrm 2.4.131

## Conclusion

OpenGL works fine on this system (confirmed with `glxinfo`), but MonoGame's initialization routine crashes before creating a window.

### Root Cause
**MonoGame is incompatible with sdl2-compat** on this hardware/software configuration:
- ✅ OpenGL 4.6 works (AMD Radeon via Mesa 25.0.6)
- ✅ .NET 8.0 works
- ✅ All dependencies present
- ❌ MonoGame + sdl2-compat = SIGSEGV crash

### Why sdl2-compat?
- **System**: Required by ffmpeg (obs-studio, qt6-webengine depend on it)
- **Nix**: Nixpkgs 25.05 provides sdl2-compat as default SDL2 implementation

### Most Promising Solutions
1. **Use older nixpkgs** (24.05 or 23.11) that may still have native SDL2
2. **Switch to Xorg** to eliminate Wayland/XWayland complexity
3. **Disable one GPU** to simplify hybrid graphics
4. **Alternative framework** (Godot, Raylib) with better Linux support

The sdl2-compat compatibility layer, while great for most applications, appears to have an incompatibility with MonoGame's SDL initialization code on MacBook Pro dual-GPU systems running Linux.
