#!/bin/bash

if [ ! -d ~/lib/provision ]; then
    mkdir ~/lib/provision
fi

cp ./src/bin/dist/ubuntu/* ~/lib/provision
mv ~/lib/provision/Provision ~/lib/provision/provision