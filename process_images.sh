#!/bin/bash

DEST_DIR="src/SparseScatteredResources/Data/PlanetDataFiles"
SOURCE_DIR="./OriginalPlanetDataFiles"

PATH="/c/ProgramData/Anaconda3:/c/ProgramData/Anaconda3/DLLs:/c/ProgramData/Anaconda3/envs/python:/c/ProgramData/Anaconda3/Library/bin:/c/ProgramData/Anaconda3/pkgs/python-3.7.4-h5263a28_0:/c/ProgramData/Anaconda3/pkgs/vs2015_runtime-14.16.27012-hf0eaf9b_0:/c/ProgramData/Anaconda3/pkgs/xlwings-0.15.10-py37_0:/c/ProgramData/Anaconda3/tcl/dde1.4:/c/ProgramData/Anaconda3/tcl/reg1.3:/c/ProgramData/Anaconda3/tcl/tix8.4.3:$PATH"

PYTHON="/c/ProgramData/Anaconda3/python.exe"

mkdir -p "$DEST_DIR"

"$PYTHON" "src/manipulator/remove_resources.py" "$SOURCE_DIR" "$DEST_DIR"
