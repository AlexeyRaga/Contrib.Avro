with (import <nixpkgs> {config.allowUnfree = true;});

let
in mkShell {

  buildInputs = [
    dotnetPackages.Nuget
  ];

}
