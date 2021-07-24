#!/bin/bash

set -x
set -e

rm -r build || true
mkdir -p build
cp -r src/SparseScatteredResources/Data build
cp thumb.png build
cp metadata.mod build
cp modinfo.sbmi build

game_mod_folder="$APPDATA/SpaceEngineers/mods/Sparse Scattered Resources"
rm -r "$game_mod_folder" || true
mkdir -p "$game_mod_folder"
cp -r build/* "$game_mod_folder"