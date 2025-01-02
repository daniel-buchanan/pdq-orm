#!/usr/bin/bash

echo "SONAR_TOKEN=${SONAR_TOKEN}";

./.sonar/scanner/dotnet-sonarscanner begin \
    /k:"daniel-buchanan_pdq" \
    /o:"daniel-buchanan" \
    /d:sonar.token="${SONAR_TOKEN}" \
    /d:sonar.host.url="https://sonarcloud.io" \
    /d:sonar.cs.opencover.reportsPaths=coverage.xml \
    /d:sonar.scanner.scanAll=false

dotnet build --no-incremental decaf-orm.sln 

./.coverlet/coverlet ./tests/decaf.db.common.tests/bin/Debug/net8.0/decaf.db.common.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    -f=json \
    -o="db-common.json"

./.coverlet/coverlet ./tests/decaf.services.tests/bin/Debug/net8.0/decaf.services.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "db-common.json" \
    -f=json \
    -o="services-merged.json"

./.coverlet/coverlet ./tests/decaf.logging.tests/bin/Debug/net8.0/decaf.logging.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "services-merged.json" \
    -f=json \
    -o="logging-merged.json"

./.coverlet/coverlet ./tests/decaf.npgsql.tests/bin/Debug/net8.0/decaf.npgsql.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "logging-merged.json" \
    -f=json \
    -o="npgsql-merged.json"

./.coverlet/coverlet ./tests/decaf.sqlserver.tests/bin/Debug/net8.0/decaf.sqlserver.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "npgsql-merged.json" \
    -f=json \
    -o="sqlserver-merged.json"
    
./.coverlet/coverlet ./tests/decaf.sqlite.tests/bin/Debug/net8.0/decaf.sqlite.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "sqlserver-merged.json" \
    -f=json \
    -o="sqlite-merged.json"

./.coverlet/coverlet ./tests/decaf.core.tests/bin/Debug/net8.0/decaf.core.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "sqlite-merged.json" \
    -f=json \
    -o="decaf-merged.json"
    
./.coverlet/coverlet ./tests/decaf.ddl.tests/bin/Debug/net8.0/decaf.ddl.tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    --merge-with "decaf-merged.json" \
    -f=opencover \
    -o="coverage.xml"

./.sonar/scanner/dotnet-sonarscanner end \
    /d:sonar.token="${SONAR_TOKEN}"