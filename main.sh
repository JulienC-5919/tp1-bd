#!/usr/bin/env bash
# Usage: ./main.sh [base_de_donnees_par_defaut]
# connexion.env est l'unique source de vérité pour la connexion.
cd "$(dirname "$0")" || exit 1

declare -A CFG
while IFS= read -r ligne || [ -n "$ligne" ]; do
    ligne=$(sed -E 's/[[:space:]]+#.*$//; s/^[[:space:]]+|[[:space:]]+$//g' <<<"$ligne")
    [[ -z "$ligne" || "$ligne" == \#* || "$ligne" != *=* ]] && continue
    CFG[$(sed -E 's/[[:space:]]+$//' <<<"${ligne%%=*}")]=$(sed -E 's/^[[:space:]]+//' <<<"${ligne#*=}")
done < connexion.env

DB_HOST=${CFG[DB_HOST]:-localhost}
DB_PORT=${CFG[DB_PORT]:-5432}
DB_NAME=${1:-${CFG[DB_NAME]:-tp1_julien}}
DB_USER=${CFG[DB_USER]:-robot}
DB_PASSWORD=${CFG[DB_PASSWORD]:-}
export DB_NAME

creer_bd() {
    command -v psql >/dev/null || { echo "psql est introuvable."; return 1; }
    if [ -z "$DB_PASSWORD" ]; then
        echo "Aucun mot de passe défini pour $DB_USER. Définissez DB_PASSWORD dans connexion.env, puis réessayez."
        return 1
    fi
    read -rp "Utilisateur administrateur [postgres]: " admin
    admin=${admin:-postgres}
    read -rp "Base de données où ajouter les tables [$DB_NAME]: " reponse_bd
    DB_NAME=${reponse_bd:-$DB_NAME}
    read -rsp "Mot de passe de $admin: " mdp_admin; echo
    export PGPASSWORD="$mdp_admin"
    # -w : ne jamais redemander le mot de passe
    local psql_admin=(psql -w -h "$DB_HOST" -p "$DB_PORT" -U "$admin" -v ON_ERROR_STOP=1 -q)

    if ! "${psql_admin[@]}" -d postgres -c "SELECT 1" >/dev/null; then
        echo "Mot de passe invalide ou connexion impossible, abandon."
        unset PGPASSWORD
        return 1
    fi

    existe=$("${psql_admin[@]}" -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname = '${DB_NAME//\'/\'\'}'") || return 1
    if [ -z "$existe" ]; then
        "${psql_admin[@]}" -d postgres -c "CREATE DATABASE \"${DB_NAME//\"/\"\"}\"" || return 1
    else
        echo "La base de données $DB_NAME existe déjà, utilisation de celle-ci."
    fi

    local avec_mdp=false; [ -n "$DB_PASSWORD" ] && avec_mdp=true
    "${psql_admin[@]}" -d "$DB_NAME" \
        -v db_name="$DB_NAME" -v db_user="$DB_USER" \
        -v db_password="$DB_PASSWORD" -v db_has_password=$avec_mdp \
        -1 -f creation.sql && echo "Base de données $DB_NAME créée." \
        || echo "Échec de la création (la base existante doit être vide)."
    unset PGPASSWORD
}

remplir() {
    if [ ! -d .venv ]; then bash preparer-python.sh || return 1; fi
    # shellcheck disable=SC1091
    source .venv/bin/activate
    python remplissage.py
}

client() { dotnet run --project tp1-bd2.csproj; }

echo "Base de données: $DB_NAME"
echo "1) Créer la base de données"
echo "2) Démarrer le remplisseur (remplissage.py)"
echo "3) Ouvrir l'application client (Main.cs)"
read -rp "Choix: " choix
case "$choix" in
    1) creer_bd ;;
    2) remplir ;;
    3) client ;;
    *) echo "Choix invalide."; exit 1 ;;
esac
