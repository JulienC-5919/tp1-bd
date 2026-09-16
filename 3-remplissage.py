import argparse
import json
import random
from datetime import datetime, timedelta

import psycopg
from psycopg import sql
from psycopg.types.json import Jsonb
from faker import Faker

from faker import Faker

SUCCURSALES = 25
LOUEURS = 125
CLIENTS = 4000
VOITURES = 500
LOCATIONS = 10000

MARQUES = (
    "Toyota",
    "Volkswagen",
    "Ford",
    "Honda",
    "Chevrolet",
    "Nissan",
    "Hyundai",
    "Kia",
    "Mazda",
    "Subaru",
    "BMW",
    "Mercedes-Benz",
    "Audi",
    "Volvo",
    "Tesla",
    "Jeep",
    "Dodge",
    "Lexus",
    "Porsche",
    "Mitsubishi",
)

MODELES = (
    ("Ford", "Focus", "berline", {"annee": 2020, "dimensions": {"largeur": 182, "hauteur": 146, "profondeur": 437}}),
    ("Subaru", "Impreza", "berline", {"annee": 2021, "dimensions": {"largeur": 178, "hauteur": 145, "profondeur": 462}}),
    ("Nissan", "Rogue", "VUS", {"annee": 2023, "dimensions": {"largeur": 184, "hauteur": 169, "profondeur": 465}}),
    ("Lexus", "NX", "VUS", {"annee": 2022, "dimensions": {"largeur": 186, "hauteur": 168, "profondeur": 466}}),
    ("Tesla", "Model 3", "berline", {"annee": 2022, "dimensions": {"largeur": 185, "hauteur": 144, "profondeur": 472}}),
    ("Volvo", "XC60", "VUS", {"annee": 2021, "dimensions": {"largeur": 191, "hauteur": 166, "profondeur": 471}}),
    ("Dodge", "Durango", "VUS", {"annee": 2021, "dimensions": {"largeur": 193, "hauteur": 180, "profondeur": 511}}),
    ("Audi", "Q5", "VUS", {"annee": 2024, "dimensions": {"largeur": 189, "hauteur": 166, "profondeur": 468}}),
    ("Mazda", "CX-5", "VUS", {"annee": 2021, "dimensions": {"largeur": 185, "hauteur": 168, "profondeur": 457}}),
    ("Kia", "Sportage", "VUS", {"dimensions": {"largeur": 186, "hauteur": 165, "profondeur": 467}}),    ("Volkswagen", "Golf", "hatchback", {"dimensions": {"largeur": 179, "hauteur": 149, "profondeur": 428}}),
    ("Chevrolet", "Malibu", "berline", {"annee": 2022, "dimensions": {"largeur": 185, "hauteur": 146, "profondeur": 493}}),
    ("Hyundai", "Elantra", "berline", {"annee": 2021, "dimensions": {"largeur": 183, "hauteur": 144, "profondeur": 471}}),
    ("Volvo", "S60", "berline", {}),
    ("Mitsubishi", "Outlander", "VUS", {"annee": 2020, "dimensions": {"largeur": 186, "hauteur": 175, "profondeur": 471}}),
    ("BMW", "Serie 3", "berline", {"annee": 2023, "dimensions": {"largeur": 182, "hauteur": 144, "profondeur": 471}}),
    ("Mercedes-Benz", "Classe C", "berline", {"annee": 2020, "dimensions": {"largeur": 182, "hauteur": 143, "profondeur": 475}}),
    ("Tesla", "Model Y", "VUS", {"annee": 2024, "dimensions": {"largeur": 192, "hauteur": 162, "profondeur": 475}}),
    ("Subaru", "Forester", "VUS", {"annee": 2020, "dimensions": {"largeur": 184, "hauteur": 173, "profondeur": 464}}),
    ("Audi", "A4", "berline", {"annee": 2023, "dimensions": {"largeur": 185, "hauteur": 143, "profondeur": 477}}),
    ("Porsche", "911", "coupé", {"annee": 2024, "dimensions": {"largeur": 185, "hauteur": 130, "profondeur": 457}}),
    ("Chevrolet", "Tahoe", "VUS", {"annee": 2024}),
    ("Volkswagen", "Tiguan", "VUS", {"annee": 2022, "dimensions": {"largeur": 184, "hauteur": 167, "profondeur": 451}}),
    ("Mazda", "Mazda3", "berline", {"annee": 2024, "dimensions": {"largeur": 180, "hauteur": 144, "profondeur": 466}}),
    ("Mercedes-Benz", "GLC", "VUS", {"annee": 2020, "dimensions": {"largeur": 193, "hauteur": 164, "profondeur": 474}}),
    ("Jeep", "Wrangler", "VUS", {"annee": 2023, "dimensions": {"largeur": 189, "hauteur": 188, "profondeur": 486}}),
    ("BMW", "X3", "VUS", {"annee": 2021, "dimensions": {"largeur": 189, "hauteur": 167, "profondeur": 471}}),
    ("Dodge", "Charger", "berline", {"annee": 2021, "dimensions": {"largeur": 190, "hauteur": 148, "profondeur": 510}}),
    ("Kia", "Forte", "berline", {"annee": 2020, "dimensions": {"largeur": 180, "hauteur": 144, "profondeur": 464}}),
    ("Honda", "Civic", "berline", {"dimensions": {"largeur": 180, "hauteur": 141, "profondeur": 456}}),
    ("Ford", "F-150", "camionnette", {"dimensions": {"largeur": 203, "hauteur": 199, "profondeur": 589}}),
    ("Honda", "CR-V", "VUS", {"annee": 2022, "dimensions": {"largeur": 186, "hauteur": 169, "profondeur": 469}}),
    ("Jeep", "Gladiator", "camionnette", {"annee": 2022, "dimensions": {"largeur": 189, "hauteur": 186, "profondeur": 553}}),
    ("Nissan", "Sentra", "berline", {"annee": 2024, "dimensions": {"largeur": 182, "hauteur": 145, "profondeur": 464}}),
    ("Mitsubishi", "Lancer", "berline", {"annee": 2020, "dimensions": {"largeur": 176, "hauteur": 148, "profondeur": 457}}),
    ("Toyota", "Corolla", "berline", {"annee": 2024}),
    ("Porsche", "Cayenne", "VUS", {"annee": 2023}),
    ("Lexus", "IS", "berline", {"annee": 2023, "dimensions": {"largeur": 184, "hauteur": 144, "profondeur": 471}}),
    ("Hyundai", "Tucson", "VUS", {"annee": 2023, "dimensions": {"largeur": 186, "hauteur": 166, "profondeur": 464}}),
    ("Toyota", "RAV4", "VUS", {"annee": 2022}),
)

with psycopg.connect(
    "dbname=tp1_julien user=robot password=yfzvjp9y3s9j37q"
) as conn:
    with conn.cursor() as cursor:

        #################### Création des requêtes préparées ####################
        cursor.execute(
            """
            PREPARE ajout_succursale (JSONB) AS
            INSERT INTO succursale (adresse) VALUES ($1)

            PREPARE ajout_loueur (VARCHAR, VARCHAR, DECIMAL(10, 2), INT, JSONB) AS
            INSERT INTO loueur (nom, prenom, salaire_heure, id_succursale, contact)
            VALUES ($1, $2, $3, $4, $5)

            PREPARE ajout_client (VARCHAR, VARCHAR, JSONB) AS
            INSERT INTO client (nom, prenom, contact) VALUES ($1, $2, $3)

            PREPARE ajout_marque (VARCHAR) AS
            INSERT INTO marque_voiture (nom) VALUES ($1)
        
            PREPARE ajout_modele (VARCHAR, VARCHAR, VARCHAR, JSONB) AS
            INSERT INTO modele_voiture (nom, id_marque, id_type, details)
            VALUES (
                $1,
                (SELECT id FROM marque_voiture WHERE nom = $2),
                (SELECT id FROM type_voiture WHERE nom = $3),
                $4
            )

            PREPARE ajout_voiture (VARCHAR, VARCHAR, VARCHAR, SMALLINT, INT, DATE, DECIMAL(10, 2), INT, JSONB) AS
            INSERT INTO voiture (immatriculation, id_modele, couleur, annee, id_succursale, date_achat, prix_achat, id_etat, details)
            VALUES (
            $1, 
            (
                SELECT id FROM modele_voiture WHERE nom = $2 AND id_marque = (SELECT id FROM marque_voiture WHERE nom = $3)
            ),
            $3, $4, $5, $6, $7, $8, $9)
            """
        )

        ############# Réinitialisation des tables pour simplifier les associations aléatoires #############
        cursor.execute(
            """
            TRUNCATE TABLE
                location_voiture,
                facture,
                voiture,
                modele_voiture,
                marque_voiture,
                client,
                loueur,
                succursale
            RESTART IDENTITY CASCADE
            """
        )

        #################### Utilisation des requêtes préparées pour générer les données ####################

        faker = Faker("fr_CA")
        for _ in range(SUCCURSALES):
            adresse = {
                "rue": faker.street_address(),
                "ville": faker.city(),
                "code_postal": faker.postcode(),
                "pays": "Canada",
            }
            cursor.execute(
                "EXECUTE ajout_succursale (%s)",
                (Jsonb(adresse),),
            )

        for _ in range(LOUEURS):
            nom = faker.last_name()
            prenom = faker.first_name()
            salaire_heure = round(random.uniform(17, 47), 2)
            contact = {
                "email": faker.email(),
                "telephone": faker.phone_number(),
                "adresse": {
                    "rue": faker.street_address(),
                    "ville": faker.city(),
                    "code_postal": faker.postcode(),
                    "pays": "Canada",
                }
            }
            cursor.execute(
                "EXECUTE ajout_loueur (%s, %s, %s, %s, %s)",
                (nom, prenom, salaire_heure, random.randint(1, SUCCURSALES), Jsonb(contact)),
            )

        for _ in range(CLIENTS):
            nom = faker.last_name()
            prenom = faker.first_name()
            contact = {
                "email": faker.email(),
                "telephone": faker.phone_number(),
                "adresse": {
                    "rue": faker.street_address(),
                    "ville": faker.city(),
                    "code_postal": faker.postcode(),
                    "pays": "Canada",
                }
            }
            cursor.execute(
                "EXECUTE ajout_client (%s, %s, %s)",
                (nom, prenom, Jsonb(contact)),
            )

        

        for marque in MARQUES:
            cursor.execute(
                sql.SQL("EXECUTE ajout_marque ({})").format(
                    sql.Literal(marque)
                )
            )

        for modele in MODELES:
            cursor.execute(
                sql.SQL("EXECUTE ajout_modele ({}, {}, {}, {})").format(
                    sql.Literal(modele[1]),
                    sql.Literal(modele[0]),
                    sql.Literal(modele[2]),
                    sql.Literal(json.dumps(modele[3]))  # Convertir le dictionnaire en JSON
                )
            )


