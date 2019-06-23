#!/usr/bin/env bash

set -eux

mkdir -p test-results/unity-editor

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
         /opt/Unity/Editor/Unity -projectPath $(pwd) \
         -runEditorTests \
         -logFile \
         -batchmode \
         -nographics \
         -noUpm \
         -testResults $(pwd)/test-results/unity-editor/results.xml

xsltproc --noout --output test-results/unity-editor/results.junit.xml \
         nunit-transforms/nunit3-junit/nunit3-junit.xslt \
         test-results/unity-editor/results.xml
