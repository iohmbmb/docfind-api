#!/bin/sh

docker rm -f test-runner 2>/dev/null || true && docker build --no-cache --network=host -f Dockerfile.testing -t backend-tests . && docker run --name test-runner --network=host --rm -e "JwtSettings__Secret=SuperSecretTestingSigningKey12345" -v "$(pwd)/junit:/app/junit" backend-tests 
