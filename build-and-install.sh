#!/bin/bash

cd src
dotnet clean
dotnet publish -r ubuntu.16.10-x64 -o bin/dist/ubuntu
cd ..
chmod +x install.sh
./install.sh