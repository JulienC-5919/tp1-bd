@echo off
setlocal enabledelayedexpansion
rem Usage: main.bat [base_de_donnees_par_defaut]
rem connexion.env est l'unique source de verite pour la connexion.
cd /d "%~dp0"

set "DB_HOST=localhost"
set "DB_PORT=5432"
set "DB_NAME=tp1_julien"
set "DB_USER=robot"
set "DB_PASSWORD="
for /f "usebackq eol=# tokens=1,* delims==" %%A in ("connexion.env") do (
    set "k=%%A"
    set "k=!k: =!"
    set "v=%%B"
    if "!k!"=="DB_HOST" set "DB_HOST=!v: =!"
    if "!k!"=="DB_PORT" set "DB_PORT=!v: =!"
    if "!k!"=="DB_NAME" set "DB_NAME=!v: =!"
    if "!k!"=="DB_USER" set "DB_USER=!v: =!"
    if "!k!"=="DB_PASSWORD" set "DB_PASSWORD=!v!"
)
if not "%~1"=="" set "DB_NAME=%~1"

echo Base de donnees: %DB_NAME%
echo 1) Creer la base de donnees
echo 2) Demarrer le remplisseur (remplissage.py)
echo 3) Ouvrir l'application client (Main.cs)
set /p choix=Choix: 
if "%choix%"=="1" goto creer
if "%choix%"=="2" goto remplir
if "%choix%"=="3" goto client
echo Choix invalide.
exit /b 1

:creer
where psql >nul 2>nul || (echo psql est introuvable. & exit /b 1)
if not defined DB_PASSWORD (
    echo Aucun mot de passe defini pour %DB_USER%. Definissez DB_PASSWORD dans connexion.env, puis reessayez.
    exit /b 1
)
set "admin=postgres"
set /p admin=Utilisateur administrateur [postgres]: 
set "reponse_bd="
set /p reponse_bd=Base de donnees ou ajouter les tables [%DB_NAME%]: 
if defined reponse_bd set "DB_NAME=%reponse_bd%"
set "PGPASSWORD="
for /f "delims=" %%P in ('powershell -NoProfile -Command "$s = Read-Host -AsSecureString 'Mot de passe de %admin%'; [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($s))"') do set "PGPASSWORD=%%P"
set "PSQL=psql -w -h %DB_HOST% -p %DB_PORT% -U %admin% -v ON_ERROR_STOP=1 -q"
%PSQL% -d postgres -c "SELECT 1" >nul 2>nul
if errorlevel 1 (
    echo Mot de passe invalide ou connexion impossible, abandon.
    set "PGPASSWORD="
    exit /b 1
)
set "existe="
for /f "delims=" %%E in ('%PSQL% -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname = '%DB_NAME%'"') do set "existe=%%E"
if defined existe (
    echo La base de donnees %DB_NAME% existe deja, utilisation de celle-ci.
) else (
    %PSQL% -d postgres -c "CREATE DATABASE \"%DB_NAME%\"" || exit /b 1
)
:tables
set "avec_mdp=false"
if defined DB_PASSWORD set "avec_mdp=true"
%PSQL% -d "%DB_NAME%" -v db_name="%DB_NAME%" -v db_user="%DB_USER%" -v db_password="%DB_PASSWORD%" -v db_has_password=%avec_mdp% -1 -f creation.sql && echo Base de donnees %DB_NAME% creee. || echo Echec de la creation ^(la base existante doit etre vide^).
set "PGPASSWORD="
exit /b

:remplir
if not exist .venv (
    python -m venv .venv || exit /b 1
    .venv\Scripts\pip install "psycopg[binary]" faker || exit /b 1
)
.venv\Scripts\python remplissage.py
exit /b

:client
dotnet run --project tp1-bd2.csproj
exit /b
