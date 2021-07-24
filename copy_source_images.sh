#!/bin/bash

# Path to PlanetDataFiles in the game files
SOURCE_DIR="/d/Program Files/Steam/steamapps/common/SpaceEngineers/Content/Data/PlanetDataFiles"
DEST_DIR="./OriginalPlanetDataFiles"

find "$SOURCE_DIR" -name "*_mat.png" | 
	grep -v Tutorial | 
	grep -v SystemTest |
	while read line; do
	    PLANET_DIR_NAME=$(basename "`dirname "$line"`")
		OUTPUT_DIR="$DEST_DIR/$PLANET_DIR_NAME"
		mkdir -p "$OUTPUT_DIR"
		cp "$line" "$OUTPUT_DIR"
	done
