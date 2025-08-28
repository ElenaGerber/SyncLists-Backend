docker#!/bin/bash

echo "Checking for Docker..."
if ! command -v docker &> /dev/null
then
    echo "Docker is not installed. Please install Docker first."
    exit 1
fi

echo "Docker is installed."

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

echo "Stopping and removing old containers..."
docker compose down
echo "Deleting old database volume..."
docker volume rm -f synclists-backend_db_data

read -p "Enter database user: " DB_USER
read -p "Enter database password: " DB_PASS
read -p "Enter database name: " DB_NAME

cat > .env <<EOL
POSTGRES_USER=$DB_USER
POSTGRES_PASSWORD=$DB_PASS
POSTGRES_DB=$DB_NAME
EOL

echo ".env file created."

echo "Starting backend..."
docker-compose up -d --build
