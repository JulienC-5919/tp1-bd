# TP1 Julien
Je reconnais qu'il manque beaucoup de documentation et de fonctions, mais j'ai manqué de temps.
---
## Fichiers importants:
- `main.sh`, `main.bat` : Programmes pour créer la base de données. Ils agissent aussi comme routeurs vers le programme de remplissage de base de données et l'application client. Ils ont été générés par Claude Sonnet et `main.bat` n'a pas été testé.
- `connexion.env` : Contient les valeurs importantes pour utiliser la base de données et générer des données. **Il faut y définir `DB_PASSWORD` pour que le script de remplissage fonctionne**
- `remplissage.py` : Programme de remplissage
- `Main.cs` : Application client

## Dépendances python à installer:
- psycopg
- faker

Vérifier le fichier `preparer-python.sh`

## Comment tout faire fonctionner
1. Ajouter un mot de passe et le reste des informations de connexion à la base de données dans `connexion.env`
2. Utiliser PGAdmin ou un autre logiciel pour créer une base de données vide pour tout installer.
3. Exécuter `main.sh` puis entrer 1 pour générer les tables. Entrer le le nom d'utilisateur et mot de passe de l'admin postgresql.
4. Utiliser `remplissage.py` pour remplir la base de données. **Le mot de passe =doit être défini dans `connexion.env`**
5. Utiliser le programme C# pour l'application client.
---
## De quel programme s'agit-il?
C'est un programme pour faire le suivi des voitures dans une entreprise de location de voitures.

___
Diagramme de base de données  dans `diagramme-bd.pdf`