@echo off
setlocal

set DB_PATH=stride.db
set SQL_FILE=seed.sql

echo === Seeding Stride DB ===

where sqlite3 >nul 2>&1
if errorlevel 1 (
    echo ERROR: sqlite3 not found on PATH.
    echo Download it from https://www.sqlite.org/download.html and add it to your PATH.
    exit /b 1
)

if not exist "%DB_PATH%" (
    echo ERROR: Database not found at %DB_PATH%
    echo Make sure you have run the API at least once so migrations can create the DB.
    exit /b 1
)

sqlite3 "%DB_PATH%" < "%SQL_FILE%"
if errorlevel 1 (
    echo ERROR: Seeding failed. Check the output above for details.
    exit /b 1
)

echo === Seeding complete! ===
endlocal
