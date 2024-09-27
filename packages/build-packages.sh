#!/bin/bash

# Get the directory where the script is located
BASE_DIR="$(dirname "$(realpath "$0")")"


# Find all .nuspec files in subdirectories from the current directory
find "$BASE_DIR" -name "*.nuspec" | while read -r NUSPEC_FILE; do
    # Run nuget pack command for each .nuspec file
    echo "Packing $NUSPEC_FILE..."
    nuget pack "$NUSPEC_FILE" -OutputDirectory "$BASE_DIR"
done
