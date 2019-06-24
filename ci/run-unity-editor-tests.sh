#!/usr/bin/env bash

set -eux

mkdir -p test-results/unity-editor

function on_exit {
    if [ -v HAS_XSLTPROC ]; then
        if [ -e test-results/unity-editor/results.xml ]; then
            xsltproc --noout --output test-results/unity-editor/results.junit.xml \
                     nunit-transforms/nunit3-junit/nunit3-junit.xslt \
                     test-results/unity-editor/results.xml
        fi
    fi
}
trap on_exit EXIT

xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
         /opt/Unity/Editor/Unity -projectPath $(pwd) \
         -runEditorTests \
         -logFile \
         -batchmode \
         -nographics \
         -noUpm \
         -testResults $(pwd)/test-results/unity-editor/results.xml
