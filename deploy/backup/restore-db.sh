#!/usr/bin/env bash
# =============================================================================
# SQL Server Database Restore Script
# Restores a database from a .bak backup file.
# Usage: restore-db.sh <path-to-backup-file> [-y]
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
# Parse arguments
# ---------------------------------------------------------------------------
SKIP_CONFIRM=false
BACKUP_FILE=""

for arg in "$@"; do
    case "$arg" in
        -y|--yes)
            SKIP_CONFIRM=true
            ;;
        *)
            if [ -z "${BACKUP_FILE}" ]; then
                BACKUP_FILE="$arg"
            else
                log_error "Unexpected argument: $arg"
                echo "Usage: $0 <path-to-backup-file> [-y]" >&2
                exit 1
            fi
            ;;
    esac
done

if [ -z "${BACKUP_FILE}" ]; then
    log_error "No backup file specified."
    echo "Usage: $0 <path-to-backup-file> [-y]" >&2
    exit 1
fi

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

if [ -z "${SA_PASSWORD:-}" ]; then
    log_error "SA_PASSWORD environment variable is required."
    exit 1
fi

# ---------------------------------------------------------------------------
# Validate backup file
# ---------------------------------------------------------------------------
if [ ! -f "${BACKUP_FILE}" ]; then
    log_error "Backup file not found: ${BACKUP_FILE}"
    exit 1
fi

FILE_SIZE=$(stat -c%s "${BACKUP_FILE}" 2>/dev/null || stat -f%z "${BACKUP_FILE}" 2>/dev/null || echo "0")
if [ "${FILE_SIZE}" -eq 0 ]; then
    log_error "Backup file is empty (0 bytes): ${BACKUP_FILE}"
    exit 1
fi

log "Backup file: ${BACKUP_FILE} (${FILE_SIZE} bytes)"

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
# Confirmation prompt
# ---------------------------------------------------------------------------
if [ "${SKIP_CONFIRM}" = false ]; then
    echo ""
    echo "========================================================"
    echo "  WARNING: This will REPLACE the database '${DB_NAME}'"
    echo "  with data from: ${BACKUP_FILE}"
    echo "  All current data in '${DB_NAME}' will be LOST."
    echo "========================================================"
    echo ""
    read -rp "Are you sure you want to proceed? (yes/no): " CONFIRM
    if [ "${CONFIRM}" != "yes" ]; then
        log "Restore cancelled by user."
        exit 0
    fi
fi

# ---------------------------------------------------------------------------
# Perform restore
# ---------------------------------------------------------------------------
log "Starting restore of database '${DB_NAME}' from '${BACKUP_FILE}'..."

$SQLCMD -S "${DB_HOST},${DB_PORT}" -U sa -P "${SA_PASSWORD}" ${SQLCMD_ARGS} -Q \
    "RESTORE DATABASE [${DB_NAME}]
     FROM DISK = N'${BACKUP_FILE}'
     WITH REPLACE,
     MOVE N'${DB_NAME}' TO N'/var/opt/mssql/data/${DB_NAME}.mdf',
     MOVE N'${DB_NAME}_log' TO N'/var/opt/mssql/data/${DB_NAME}_log.ldf',
     STATS = 10"

if [ $? -ne 0 ]; then
    log_error "RESTORE DATABASE command failed."
    exit 1
fi

log "Database '${DB_NAME}' restored successfully from '${BACKUP_FILE}'."
exit 0
