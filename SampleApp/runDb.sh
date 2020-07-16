#!/bin/bash
echo "starting postgres"
docker run -d --name postgres -p 5432:5432 clkao/postgres-plv8
docker exec -it bash -c 'psql -U postgres -c "CREATE EXTENSION plv8; SELECT extversion FROM pg_extension WHERE extname = ''plv8'';"'

echo "starting redis"
docker run --name redis -d  -p 6379:6379 redis