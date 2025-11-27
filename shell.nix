# shell.nix
with import <nixpkgs> { };

mkShell {
  name = "dotnet-env";
  packages = [ dotnet-sdk powershell ];
}