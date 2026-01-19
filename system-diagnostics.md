# System Diagnostics - MonoGame Graphics Issue

Generated: 2026-01-17

## Hardware Information

### System
- **Model**: 2019 MacBook Pro 15" with Touch Bar
- **Hostname**: mpb-silver-surfer
- **CPU**: Intel Core i7-7700HQ @ 2.80GHz (4 cores, 8 threads)
- **RAM**: 16GB
- **Architecture**: x86_64

### Graphics Hardware
- **GPU 0 (Intel)**: HD Graphics 630 (rev 04)
  - PCI ID: 8086:591B
  - Driver: i915 (kernel module)
  - DRM Device: /dev/dri/card0
  - Render Node: /dev/dri/renderD129
  - Status: Active, but internal eDP display issue (see kernel messages)

- **GPU 1 (AMD)**: Baffin [Radeon RX 460/560D / Pro 450/455/460/555/555X/560/560X] (rev c0)
  - PCI ID: 1002:67EF
  - Driver: amdgpu (kernel module)
  - DRM Device: /dev/dri/card1
  - Render Node: /dev/dri/renderD128
  - Status: Primary framebuffer device

### Display Configuration
- **Active Display**: eDP-1 (Internal Retina Display)
  - Resolution: 2880x1800 @ 60Hz
  - Scaling: 2.00x (HiDPI)
  - Manufacturer: Apple Computer Inc
  - Model: Color LCD
  - Physical Size: 330x210mm

## Software Environment

### Operating System
- **Distro**: Arch Linux
- **Kernel**: 6.18.3-arch1-1 #1 SMP PREEMPT_DYNAMIC
- **Boot Parameters**: `quiet splash cryptdevice=... root=/dev/mapper/root zswap.enabled=0 rootflags=subvol=@ rw rootfstype=btrfs`

### Display Server / Compositor
- **Primary Session**: Wayland
- **Compositor**: Hyprland 0.53.1
- **XWayland**: Running on :1
- **X Server**: Xorg 21.1.21 also running (on vt2, display :0)
- **Session Type**: wayland
- **Current Display**: :1 (XWayland)

### Graphics Stack
- **Mesa**: 1:25.3.3-1 (very recent)
- **libdrm**: 2.4.131-1
- **libglvnd**: 1.7.0-3
- **SDL2**: sdl2-compat 2.32.60-1 (compatibility layer, not native SDL2)
- **xorg-xwayland**: 24.1.9-1

### GPU Drivers
- **Intel i915**:
  - Module: i915 (4829184 bytes)
  - Driver version: 1.6.0
  - Firmware: Uses KBL (Kaby Lake) firmware

- **AMD amdgpu**:
  - Module: amdgpu (15716352 bytes)
  - Driver version: 3.64.0
  - Firmware: Various Polaris/Vega firmwares

- **DDX Drivers**: None installed (using modern modesetting)

### Key Environment Variables
```
DISPLAY=:1
XDG_SESSION_TYPE=wayland
XDG_CURRENT_DESKTOP=Hyprland
WAYLAND_DISPLAY=wayland-1
SDL_VIDEODRIVER=wayland
MOZ_ENABLE_WAYLAND=1
```

## GPU Driver Status

### Intel i915 Kernel Messages
```
i915 0000:00:02.0: [drm] Found kabylake (device ID 591b) integrated display version 9.00 stepping C0
i915 0000:00:02.0: [drm] Finished loading DMC firmware i915/kbl_dmc_ver1_04.bin (v1.4)
i915 0000:00:02.0: [drm] [ENCODER:105:DDI A/PHY A] failed to retrieve link info, disabling eDP
i915 0000:00:02.0: [drm] Cannot find any crtc or sizes (repeated 3x)
```

**Issue**: Intel driver reports "failed to retrieve link info, disabling eDP" and cannot find CRTC. This suggests the Intel GPU is having issues with the internal display initialization, though Hyprland successfully uses it.

### AMD amdgpu Kernel Messages
```
amdgpu 0000:01:00.0: amdgpu: [drm] Skipping amdgpu DM backlight registration
[drm] Initialized amdgpu 3.64.0 for 0000:01:00.0 on minor 1
fbcon: amdgpudrmfb (fb0) is primary device
amdgpu 0000:01:00.0: [drm] fb0: amdgpudrmfb frame buffer device
```

**Status**: AMD GPU initialized successfully and is the primary framebuffer device.

## MonoGame Crash Analysis

### .NET Environment
- **SDK Version Required**: 8.0.0 (per global.json)
- **SDK Installed**: 8.0.408 (via Nix)
- **Runtime**: .NET 8.0.15
- **DOTNET_ROOT**: Set via Nix shell
- **Build Status**: Successful
- **Content Build Status**: Successful

### Crash Details
- **Exit Code**: 139 (SIGSEGV - Segmentation Fault)
- **Crash Location**: During MonoGame OpenGL initialization
- **Stack Trace**: Crash in libcoreclr.so during native library loading
- **Symptom**: No window appears, crashes immediately

### Attempted Fixes
1. ✅ .NET SDK via Nix - Available and working
2. ✅ SDL2, OpenGL libraries - Present in Nix environment
3. ✅ Game assets compiled - All .xnb files present
4. ❌ DISPLAY=:0 (Xorg) - Crashes
5. ❌ DISPLAY=:1 (XWayland) - Crashes
6. ❌ DRI_PRIME=0 (force Intel GPU) - Crashes
7. ❌ DRI_PRIME=1 (force AMD GPU) - Crashes
8. ❌ LIBGL_ALWAYS_SOFTWARE=1 - Crashes
9. ❌ GALLIUM_DRIVER=llvmpipe - Crashes
10. ❌ SDL_VIDEODRIVER=x11 - Crashes
11. ❌ SDL_VIDEODRIVER=dummy - Crashes
12. ❌ GraphicsProfile.Reach (OpenGL 2.0) - Crashes
13. ❌ Disabled VSync and MultiSampling - Crashes

### Root Cause Analysis

The crash occurs during MonoGame's SDL/OpenGL initialization, before any rendering happens. The .NET core trace shows the runtime loads successfully (exit code 0x8B after managed assembly load), then crashes during native interop.

**Primary Issues Identified:**

1. **SDL2 Compatibility Layer**: Using `sdl2-compat` instead of native SDL2 may cause issues with MonoGame's native bindings

2. **Wayland/XWayland Conflict**:
   - Global `SDL_VIDEODRIVER=wayland` environment variable set by Hyprland
   - MonoGame tries to use X11/XWayland via DISPLAY=:1
   - Potential conflict between SDL backend expectations

3. **Intel GPU Initialization Failure**:
   - Kernel shows "failed to retrieve link info, disabling eDP"
   - "Cannot find any crtc or sizes" errors
   - May affect OpenGL context creation even when using AMD GPU

4. **Dual GPU Complexity**:
   - Hybrid Intel/AMD setup on Apple hardware
   - VGA arbiter switching between GPUs
   - Both GPUs claim/release VGA decodes during initialization

5. **HiDPI/Retina Display**:
   - 2880x1800 at 2x scaling
   - May cause issues with OpenGL context creation

## Potential Solutions

### CRITICAL DISCOVERY: sdl2-compat Everywhere

**Both Nix and system are using `sdl2-compat`**, not native SDL2:

1. **System (pacman)**: `sdl2-compat 2.32.60-1`
   - Required by ffmpeg, obs-studio, qt6-webengine
   - Cannot be removed

2. **Nix environment**: `/nix/store/.../sdl2-compat-2.32.56`
   - In nixpkgs 25.05, the `SDL2` package provides sdl2-compat by default
   - This is intentional nixpkgs behavior for compatibility

**Verification:**
```bash
nix develop
echo $LD_LIBRARY_PATH | head -1
# Shows: /nix/store/.../sdl2-compat-2.32.56/lib
```

**Implication:** The issue is NOT a conflict between native SDL2 and compat layer. The issue is that **sdl2-compat itself doesn't work with MonoGame on this hardware/software configuration**.

### 1. Try Older nixpkgs with Native SDL2
Nixpkgs switched to sdl2-compat recently. Older versions may have native SDL2:
```nix
# In flake.nix, try older nixpkgs:
inputs = {
  nixpkgs.url = "github:NixOS/nixpkgs/nixos-24.05";  # or 23.11
};
```

### 2. Override SDL2 Package (Advanced)
Force native SDL2 build in flake.nix (if native SDL2 still exists in nixpkgs):
```nix
# This would require custom overlay - complex
```

### 2. Unset SDL_VIDEODRIVER
Run MonoGame with SDL_VIDEODRIVER unset:
```bash
env -u SDL_VIDEODRIVER DISPLAY=:1 dotnet run
```

### 3. Use Xorg Session Instead of Wayland
Boot into a native Xorg session instead of Hyprland/Wayland.

### 4. Disable Intel GPU via Kernel Parameter
Add to boot parameters:
```
modprobe.blacklist=i915
```
Force system to use only AMD GPU.

### 5. Disable AMD GPU via Kernel Parameter
Add to boot parameters:
```
modprobe.blacklist=amdgpu
```
Force system to use only Intel GPU.

### 6. Try Different MonoGame Backend
- Test with MonoGame WindowsDX on Wine
- Try MonoGame develop branch
- Use MonoGame.Framework.DesktopGL.Core

### 7. Alternative Game Frameworks
- Godot with C#/F# support
- Raylib with F# bindings
- Native macOS with MonoGame (not Linux)

### 8. Use Nix-Provided Diagnostic Tools
The `flake.nix` has been updated to include:
```nix
mesa-demos      # provides glxinfo
xorg.xdpyinfo   # provides xdpyinfo
```

After updating the flake:
```bash
nix develop
DISPLAY=:1 glxinfo -B
DISPLAY=:1 xdpyinfo | head -20
```

## Hardware Notes

- MacBook Pro 2019 15" models are known to have hybrid graphics switching issues on Linux
- Apple's T2 chip may interfere with some graphics operations
- The eDP display initialization failure is a known issue with Intel graphics on MacBooks
- AMD Baffin/Polaris GPUs generally work well on Linux but have quirks in hybrid setups
