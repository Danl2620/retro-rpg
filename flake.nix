{
  description = "Dev shell for F# + MonoGame/XNA using nixpkgs 25.05";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/25.05";
  };

  outputs = { self, nixpkgs }:
    let
      systems = [ "x86_64-darwin" "x86_64-linux" ];
      forAllSystems = nixpkgs.lib.genAttrs systems;
    in
    {
      devShells = forAllSystems (system:
        let
          pkgs = import nixpkgs { inherit system; };
        in
        {
          default = pkgs.mkShell {
            # .NET SDK (includes F# compiler)
            buildInputs = with pkgs; [
              dotnet-sdk_8
              dotnet-runtime_8

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

              powershell
            ];

            # Environment variables that help dotnet behave sanely
            DOTNET_ROOT = "${pkgs.dotnet-sdk_8}";
            PATH = [
              "${pkgs.dotnet-sdk_8}/bin"
              "${pkgs.dotnet-runtime_8}/bin"
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
