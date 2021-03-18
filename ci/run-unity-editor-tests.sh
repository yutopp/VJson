#!/usr/bin/env bash

set -eux

mkdir -p test-results/unity-editor
mkdir -p test-results/unity-playmode

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

# Editor test
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
         /usr/bin/unity-editor -projectPath "$(pwd)" \
         -runEditorTests \
         -logFile \
         -batchmode \
         -nographics \
         -noUpm \
         -testResults $(pwd)/test-results/unity-editor/results.xml

# Play mode test
xvfb-run --auto-servernum --server-args='-screen 0 640x480x24' \
         /usr/bin/unity-editor -projectPath "$(pwd)" \
         -batchmode \
         -nographics -noUpm \
         -runTests -testPlatform playmode \
         -logFile \
         -testResults "$(pwd)/test-results/unity-playmode/results.xml"
