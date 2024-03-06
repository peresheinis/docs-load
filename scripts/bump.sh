#!/bin/bash

Prefix=$(echo $3 | sed 's/_//g')
echo Path $1
echo Mode $2
echo Branch $3
echo Prefix ${Prefix}

if [ "$2" = "accept" ]; then
    dotnet version --skip-vcs -f "$1" minor
elif [ "$2" = "prepatch" ]; then
    dotnet version --skip-vcs -f "$1" --prefix ${Prefix} prepatch
elif [ "$2" = "preminor" ]; then
    dotnet version --skip-vcs -f "$1" --prefix ${Prefix} preminor
elif [ "$2" = "premajor" ]; then
    dotnet version --skip-vcs -f "$1" --prefix ${Prefix} premajor
elif [ "$2" = "prerelease" ]; then
    dotnet version --skip-vcs -f "$1" --prefix ${Prefix} prerelease
elif [ "$2" = "patch" ]; then
    dotnet version --skip-vcs -f "$1" patch
else
    echo "Unknown command: $2. Expected accept, patch, prepatch, preminor, premajor or prerelease"
fi