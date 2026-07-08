#!/usr/bin/env bash
# =============================================================================
# SQL Server Database Backup Script
# Backs up the specified database and rotates old backup files.
# =============================================================================
set -euo pipefail

# ---------------------------------------------------------------------------
# Logging
# ---------------------------------------------------------------------------
log() {
    echo "[$(date -Iseconds)] $*"
}

log_error() {
    echo "[$(date -Iseconds)] ERROR: $*" >&2
}

# ---------------------------------------------------------------------------
# Detect sqlcmd
# ---------------------------------------------------------------------------
if [ -x /opt/mssql-tools18/bin/sqlcmd ]; then
    SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
    SQLCMD_ARGS="-C"
elif [ -x /opt/mssql-tools/bin/sqlcmd ]; then
    SQLCMD="/opt/mssql-tools/bin/sqlcmd"
    SQLCMD_ARGS=""
else
    log_error "sqlcmd not found. Install mssql-tools18 or mssql-tools."
    exit 1
fi

# ---------------------------------------------------------------------------
# Configuration (environment variables with defaults)
# ---------------------------------------------------------------------------
DB_HOST="${DB_HOST:-cinema-db}"
DB_PORT="${DB_PORT:-1433}"
DB_NAME="${DB_NAME:-QuanLyRapPhim}"
BACKUP_DIR="${BACKUP_DIR:-/backups}"
RETENTION_DAYS="${RETENTION_DAYS:-7}"

if [ -z "${SA_PASSWORD:-}" ]; then
    log_error "SA_PASSWORD environment variable is required."
    exit 1
fi

# ---------------------------------------------------------------------------
# Wait for SQL Server readiness
# ---------------------------------------------------------------------------
log "Waiting for SQL Server at ${DB_HOST},${DB_PORT}..."
for i in $(seq 1 30); do
    if $SQLCMD -S "${DB_HOST},${DB_PORT}" -U sa -P "${SA_PASSWORD}" ${SQLCMD_ARGS} \
        -Q "SELECT 1" >/dev/null 2>&1; then
        log "SQL Server is ready."
        break
    fi
    if [ "$i" = "30" ]; then
        log_error "SQL Server did not become ready within 30 seconds."
        exit 1
    fi
    sleep 1
done

# ---------------------------------------------------------------------------
# Create backup directory
# ---------------------------------------------------------------------------
mkdir -p "${BACKUP_DIR}"

# ---------------------------------------------------------------------------
# Perform backup
# ---------------------------------------------------------------------------
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="${DB_NAME}_${TIMESTAMP}.bak"
BACKUP_PATH="${BACKUP_DIR}/${BACKUP_FILE}"

log "Starting backup of database '${DB_NAME}' to '${BACKUP_PATH}'..."

$SQLCMD -S "${DB_HOST},${DB_PORT}" -U sa -P "${SA_PASSWORD}" ${SQLCMD_ARGS} -Q \
    "BACKUP DATABASE [${DB_NAME}] TO DISK = N'${BACKUP_PATH}' WITH FORMAT, INIT, NAME = N'${DB_NAME}-Full', SKIP, NOREWIND, NOUNLOAD, STATS = 10"

if [ $? -ne 0 ]; then
    log_error "BACKUP DATABASE command failed."
    exit 1
fi

# ---------------------------------------------------------------------------
# Verify backup file
# ---------------------------------------------------------------------------
if [ ! -f "${BACKUP_PATH}" ]; then
    log_error "Backup file '${BACKUP_PATH}' does not exist after backup."
    exit 1
fi

FILE_SIZE=$(stat -c%s "${BACKUP_PATH}" 2>/dev/null || stat -f%z "${BACKUP_PATH}" 2>/dev/null || echo "0")
if [ "${FILE_SIZE}" -eq 0 ]; then
    log_error "Backup file '${BACKUP_PATH}' is empty (0 bytes)."
    exit 1
fi

log "Backup completed successfully: ${BACKUP_FILE} (${FILE_SIZE} bytes)"

# ---------------------------------------------------------------------------
# Rotate old backups
# ---------------------------------------------------------------------------
log "Removing backups older than ${RETENTION_DAYS} days..."
DELETED_COUNT=$(find "${BACKUP_DIR}" -name "*.bak" -type f -mtime +${RETENTION_DAYS} -print -delete | wc -l)
log "Deleted ${DELETED_COUNT} old backup file(s)."

log "Backup process finished."
exit 0
