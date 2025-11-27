{
  description = "Dev shell for F# + MonoGame/XNA on macOS using nixpkgs 25.05";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/25.05";
  };

  outputs = { self, nixpkgs }: {
    devShells.x86_64-darwin.default =
      let
        pkgs = import nixpkgs {
          system = "x86_64-darwin";
        };
      in pkgs.mkShell {
        # .NET SDK (includes F# compiler)
        buildInputs = with pkgs; [
          dotnet-sdk_8
          dotnet-runtime_8

          # Native dependencies MonoGame needs on macOS
          SDL2
          SDL2_ttf
          SDL2_image
          SDL2_mixer

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
          echo "F# + MonoGame dev shell is ready."
        '';
      };
  };
}
