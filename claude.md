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
  - Launches the game with `DISPLAY=:0 DRI_PRIME=0 dotnet run`
  - `DISPLAY` sets the X11 display (adjust based on your setup: `:0` or `:1`)
  - `DRI_PRIME=0` forces Intel GPU (more stable than AMD for OpenGL on Linux)

## Known Issues

### MonoGame Crashes on MacBook Pro (2019) with Arch Linux

> 📋 **See `system-diagnostics.md` for complete hardware/software configuration and detailed diagnostics**

**Status:** Currently experiencing segmentation faults (SIGSEGV) during MonoGame graphics initialization

**Hardware:**
- 2019 MacBook Pro with Touch Bar
- Dual GPU setup: Intel HD Graphics 630 + AMD Radeon RX 460/560

**Symptoms:**
- Game builds successfully
- Crashes immediately on launch (exit code 139)
- No window appears
- Crash occurs in MonoGame's OpenGL context creation
- Core dumps show failure in `libcoreclr.so` during graphics device initialization

**Attempted Fixes:**
- ✅ .NET SDK 8.0 installed via Nix
- ✅ All dependencies available (SDL2, OpenGL, Mesa drivers)
- ✅ All game assets compiled successfully
- ❌ `DISPLAY=:0` and `DISPLAY=:1` - both crash
- ❌ `DRI_PRIME=0` (force Intel GPU) - crashes
- ❌ `DRI_PRIME=1` (force AMD GPU) - crashes
- ❌ `LIBGL_ALWAYS_SOFTWARE=1` (software rendering) - crashes
- ❌ `GALLIUM_DRIVER=llvmpipe` (Mesa software rasterizer) - crashes
- ❌ `SDL_VIDEODRIVER=x11` - crashes
- ❌ `SDL_VIDEODRIVER=dummy` - crashes
- ❌ `env -u SDL_VIDEODRIVER` (unset Wayland SDL driver) - crashes
- ❌ `GraphicsProfile.Reach` (OpenGL 2.0 compat mode) - crashes

**Root Cause:**
MonoGame's OpenGL initialization has compatibility issues with the hybrid Intel/AMD graphics setup on MacBook Pro hardware running Linux. The crash occurs during native library loading and OpenGL context creation.

**Specific Issues Identified (see system-diagnostics.md for details):**
1. **🔴 CRITICAL: sdl2-compat everywhere** - Both system AND Nix use sdl2-compat (not native SDL2)
   - System has sdl2-compat (required by ffmpeg/obs-studio, cannot remove)
   - Nixpkgs 25.05 provides sdl2-compat by default for SDL2 package
   - MonoGame + sdl2-compat + this hardware = incompatible
2. **Wayland/XWayland complexity** - Running Hyprland (Wayland) with XWayland layer
3. **Intel GPU eDP failure** - Kernel shows "failed to retrieve link info, disabling eDP"
4. **Global SDL_VIDEODRIVER=wayland** - Set by Hyprland, conflicts with X11 expectations
5. **HiDPI Retina Display** - 2880x1800@2x scaling may complicate OpenGL context creation

**Potential Workarounds (in order of likelihood to work):**

1. **Try older nixpkgs with native SDL2** - Current nixpkgs 25.05 uses sdl2-compat
   ```nix
   # Edit flake.nix, change:
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
   Older nixpkgs versions may still provide native SDL2 instead of sdl2-compat.

2. **Test diagnostic tools** - flake.nix now includes mesa-demos and xorg-xdpyinfo
   ```bash
   # Restart nix shell to get new tools
   exit  # if already in nix shell
   nix develop

   # Test OpenGL is working
   DISPLAY=:1 glxinfo -B
   DISPLAY=:1 xdpyinfo | head -20
   ```

3. **Switch to Xorg session** - Boot into native Xorg instead of Hyprland/Wayland
   - Less complex graphics stack
   - MonoGame expects X11 environment
   - Eliminates XWayland layer

4. **Kernel boot parameters** - Disable one GPU entirely via boot parameters
   - Add `modprobe.blacklist=amdgpu` to disable AMD GPU, or
   - Add `modprobe.blacklist=i915` to disable Intel GPU
   - Eliminates hybrid GPU switching issues
   - Edit `/etc/default/grub` and run `sudo grub-mkconfig -o /boot/grub/grub.cfg`

5. **Different MonoGame version** - Try MonoGame develop branch or different version
   - Update Paket dependencies
   - Test with older stable versions

6. **Alternative framework** - Consider switching to:
   - Godot Engine with C# (better Linux/Mac support)
   - Native macOS build with MonoGame
   - Raylib with F# bindings
   - SDL2-based F# game framework

7. **Different hardware** - Test on non-Apple hardware with single GPU

**Diagnostic Commands:**
```bash
# Check GPU drivers
lspci | grep -i vga
lsmod | grep -E 'i915|amdgpu'

# View crash logs
coredumpctl list | tail -5
coredumpctl info

# Check display environment
echo $DISPLAY
echo $XDG_SESSION_TYPE
echo $SDL_VIDEODRIVER

# Test OpenGL (requires mesa-utils)
glxinfo -B

# Test with different display
DISPLAY=:0 nix develop --command dotnet run
DISPLAY=:1 nix develop --command dotnet run

# Complete system diagnostics
cat system-diagnostics.md
```

**References:**
- **Full diagnostics**: See `system-diagnostics.md` for complete hardware/software analysis
- MonoGame OpenGL issues: https://github.com/MonoGame/MonoGame/issues
- Linux on MacBook Pro: https://wiki.archlinux.org/title/MacBookPro15,x
- Mesa Environment Variables: https://docs.mesa3d.org/envvars.html
