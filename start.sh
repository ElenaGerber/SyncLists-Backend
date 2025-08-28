#!/bin/bash

if ! docker info &> /dev/null
then
    echo "Docker is installed but not running. Trying to start it..."
    
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        sudo systemctl start docker
    fi
    
    if [[ "$OSTYPE" == "darwin"* ]] || [[ "$OSTYPE" == "msys"* ]]; then
        echo "Please start Docker Desktop manually."
        read -p "Press Enter after Docker has started..."
    fi

    if ! docker info &> /dev/null
    then
        echo "Docker could not be started. Exiting."
        exit 1
    fi
fi

echo "Docker is running."
echo "Starting backend..."
docker compose up -d --build
