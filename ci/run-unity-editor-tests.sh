#!/usr/bin/env bash

set -eux

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
         /opt/Unity/Editor/Unity -projectPath $(pwd) \
         -runEditorTests \
         -logFile \
         -batchmode \
         -nographics \
         -noUpm \
         -testResults $(pwd)/EditorTestsResults.xml
