#!/usr/bin/env sh

echo "Before starting, make sure that:"
echo "1. Your Unity general preferences are set to use the system JDK (possibly OpenJDK 8);"
echo "   For example, use the folder /usr/lib/jvm/java-8-openjdk"
echo "   Using the embedded JDK might have your system fail!"
echo "2. You are using Unity 2018.4.12f1 with Unity Hub. If you are not, stop this script"
echo "   and modify the variable UNITY_EXECUTABLE below."

sleep 2

# set -x
# Please modify this accordingly to your own distribution.
# If you are using Unity Hub, this should be installed under $HOME/Unity/Hub/etc.
export UNITY_EXECUTABLE=${UNITY_EXECUTABLE:-"$HOME/Unity/Hub/Editor/2018.4.12f1/Editor/Unity"}
export BUILD_NAME=${BUILD_NAME:-"DataFlows"}

BUILD_TARGET=Android ./ci/build.sh

