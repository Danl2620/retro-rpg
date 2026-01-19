{
  description = "Dev shell for F# + MonoGame/XNA using nixpkgs 23.11";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    nixgl.url = "github:nix-community/nixGL";
  };

  outputs = { self, nixpkgs, nixgl }:
    let
      systems = [ "x86_64-darwin" "x86_64-linux" ];
      forAllSystems = nixpkgs.lib.genAttrs systems;
    in
    {
      devShells = forAllSystems (system:
        let
          pkgs = import nixpkgs {
            inherit system;
            overlays = [
              # Override SDL2 to disable all IME support which crashes on XWayland
              (final: prev: {
                SDL2 = prev.SDL2.overrideAttrs (oldAttrs: {
                  configureFlags = (oldAttrs.configureFlags or []) ++ [
                    "--disable-ime"
                    "--disable-ibus"
                    "--disable-fcitx"
                  ];
                });
              })
            ];
          };
          nixgl-pkgs = nixgl.packages.${system};
          dotnet-sdk = pkgs.dotnet-sdk_9;
          dotnet-runtime = pkgs.dotnet-runtime_9;
        in
        {
          default = pkgs.mkShell {
            # .NET SDK (includes F# compiler)
            buildInputs = with pkgs; [
              dotnet-sdk
              dotnet-runtime

              # Native dependencies MonoGame needs
              SDL2
              SDL2_ttf
              SDL2_image
              SDL2_mixer

              # OpenGL libraries
              libGL
              libGLU
              mesa
              mesa_glu

              # X11 libraries for MonoGame
              xorg.libX11
              xorg.libXi
              xorg.libXcursor
              xorg.libXrandr
              xorg.libXext

              # Wayland/Xwayland support
              wayland
              libxkbcommon

              # Diagnostic tools
              mesa-demos  # provides glxinfo
              xorg.xdpyinfo

              powershell

              # NixGL for OpenGL support
              nixgl-pkgs.nixGLDefault
            ];

            # Environment variables that help dotnet behave sanely
            DOTNET_ROOT = "${dotnet-sdk}";
            PATH = [
              "${dotnet-sdk}/bin"
              "${dotnet-runtime}/bin"
            ];

            # Optional QoL things
            shellHook = ''
              export LD_LIBRARY_PATH="${pkgs.lib.makeLibraryPath [
                pkgs.SDL2
                pkgs.SDL2_ttf
                pkgs.SDL2_image
                pkgs.SDL2_mixer
                pkgs.libGL
                pkgs.libGLU
                pkgs.mesa
                pkgs.xorg.libX11
                pkgs.xorg.libXi
                pkgs.xorg.libXcursor
                pkgs.xorg.libXrandr
                pkgs.xorg.libXext
                pkgs.wayland
                pkgs.libxkbcommon
              ]}:$LD_LIBRARY_PATH"
              export LIBGL_DRIVERS_PATH="${pkgs.mesa}/lib/dri"
              export SDL_VIDEODRIVER=x11
              echo "F# + MonoGame dev shell is ready."
            '';
          };
        });
    };
}
