#!/bin/bash

NODE_ID="${NODE_ID:-SqliteWrapper1}"
LISTEN_PORT="${LISTEN_PORT:-10001}"
TIMEOUT_MS="${TIMEOUT_MS:-5000}"
COMMUNICATION_FORMAT="${COMMUNICATION_FORMAT:-AVRO}"
NETWORK_ADAPTER_TYPE="${NETWORK_ADAPTER_TYPE:-TCP}"
ALLOW_COMMANDS="${ALLOW_COMMANDS:-true}"
EAGER_STARTUP="${EAGER_STARTUP:-true}"
STARTUP_REMOTE_POINTS="${STARTUP_REMOTE_POINTS:-}"
STARTUP_INFER_SCHEMA="${STARTUP_INFER_SCHEMA:-true}"
DB_NAME="${DB_NAME:-test.db}"
DATASOURCE_CONNECTION_STRING="${DATASOURCE_CONNECTION_STRING:-Data Source=${DB_DIR}${DB_NAME}}"
DATASOURCE_NAME="${DATASOURCE_NAME:-AlbumArtistsData}"
PERSISTENCE_CONNECTION_STRING="${PERSISTENCE_CONNECTION_STRING:-./wrapper_database.db}"
WEB_PORT="${WEB_PORT:-8001}"
PROJECT_NAME="${1:-Janus.Sqlite.Wrapper}"

STARTUP_REMOTE_POINTS=$(bash /Scripts/reformat_remote_points.sh ${STARTUP_REMOTE_POINTS})

echo "NODE_ID=${NODE_ID}"
echo "LISTEN_PORT=${LISTEN_PORT}"
echo "TIMEOUT_MS=${TIMEOUT_MS}"
echo "COMMUNICATION_FORMAT=${COMMUNICATION_FORMAT}"
echo "NETWORK_ADAPTER_TYPE=${NETWORK_ADAPTER_TYPE}"
echo "ALLOW_COMMANDS=${ALLOW_COMMANDS}"
echo "EAGER_STARTUP=${EAGER_STARTUP}"
echo "STARTUP_REMOTE_POINTS=${STARTUP_REMOTE_POINTS}"
echo "STARTUP_INFER_SCHEMA=${STARTUP_INFER_SCHEMA}"
echo "DB_NAME=${DB_NAME}"
echo "DB_DIR=${DB_DIR}"
echo "SOURCE_CONNECTION_STRING=${SOURCE_CONNECTION_STRING}"
echo "DATASOURCE_NAME=${DATASOURCE_NAME}"
echo "PERSISTENCE_CONNECTION_STRING=${PERSISTENCE_CONNECTION_STRING}"
echo "WEB_PORT=${WEB_PORT}"
echo "PROJECT_NAME=${PROJECT_NAME}"


envsubst < /App/appsettings.json.template | sed -e 's/§/$/g' > /App/appsettings.json

cat /App/appsettings.json

exec /App/${PROJECT_NAME}
