
import datetime
import json
import random
import string

import psycopg
from psycopg import sql
from psycopg.types.json import Jsonb
from faker import Faker



SUCCURSALES = 25
LOUEURS = 125
CLIENTS = 4000
VOITURES = 500
LOCATIONS = 10000

OUVERTURE = 9
FERMETURE = 17

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

def genererNumeroSerie():
    lettres = "".join(random.choices(string.ascii_uppercase, k=2))
    chiffres = "".join(random.choices(string.digits, k=10))
    return lettres + chiffres

def etatAleatoire():
    chance = random.randint(1, 20)

    match chance:
        case 1:
            return 4 # manquant
        case 2:
            return 5 # débarras
        case 3:
            return 3 # défectueux
        case _:
            return 1 # disponible

def heureAleatoire():
    return datetime.time(
        hour=random.randint(OUVERTURE, FERMETURE - 1),
        minute=random.randrange(60)
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

        ############# Réinitialisation des tables pour simplifier les associations aléatoires #############
        cursor.execute(
            """
            TRUNCATE TABLE
                location_voiture,
                facture,
                voiture,
                modele_voiture,
                marque,
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
                "INSERT INTO succursale (adresse) VALUES (%s)",
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
                """
                INSERT INTO loueur(nom, prenom, salaire_heure, id_succursale, contact)
                VALUES (%s, %s, %s, %s, %s)
                """,
                (nom, prenom, salaire_heure, random.randint(1, SUCCURSALES), Jsonb(contact)),
            )

        for _ in range(CLIENTS):
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
                """
                INSERT INTO client (nom,prenom,contact)
                VALUES(%s, %s, %s)
                """,
                (
                    faker.last_name(),
                    faker.first_name(),
                    Jsonb(contact)
                ),
            )

        

        for marque in MARQUES:
            cursor.execute(
                """
                INSERT INTO marque (nom)
                VALUES (%s)
                """,
                (marque,)
            )

        for modele in MODELES:
            cursor.execute(

                """
                INSERT INTO modele_voiture(nom, id_marque, id_type, details)
                VALUES (
                    %s, %s, 
                    (
                        SELECT id FROM type_voiture WHERE nom = %s
                    ), 
                    %s
                )
                """,
                (  
                    modele[1], 
                    random.randint(1, len(MARQUES)),
                    modele[2],
                    json.dumps(modele[3]),
                )

            )

        for _ in range(VOITURES):
                        
                    details = {
                        "kilometrage": random.randint(0, 200000),
                        "carburant": random.choice(["essence", "diesel", "électrique", "hybride"]),
                        "transmission": random.choice(["manuelle", "automatique"]),
                    }
                    cursor.execute(
                        """
                        INSERT INTO voiture(plaque, id_modele, no_serie, id_etat, kilometrage, date_construction, prix_jour, id_succursale, details)
                        VALUES (
                            %s, %s, %s, %s, %s, %s, %s, %s, %s
                        )
                        RETURNING id, prix_jour
                        """,
                        (
                            faker.unique.license_plate(), # ----------------------------- plaque d'immatriculation
                            random.randint(1, len(MODELES)), # -------------------------- modèle
                            genererNumeroSerie(), # ------------------------------------- numéro de série
                            etatAleatoire(), # ------------------------------------------ état aléatoire
                            random.randint(0, 200000), # -------------------------------- kilometrage
                            faker.date_between(start_date="-10y", end_date="today"), # -- date de construction
                            round(random.uniform(15000, 80000), 2), # ------------------- Prix par jour
                            random.randint(1, SUCCURSALES), # --------------------------- succursale aléatoire
                            Jsonb(details) # ------------------------------------------- détails supplémentaires
                        )
                    )
                    voiture_id, prix_jour = cursor.fetchone()

                    for _ in range(LOCATIONS):
                        duree = random.randint(3, 30)
                        date_debut = faker.date_between(start_date="-2y", end_date="today")
                        date_fin = date_debut + datetime.timedelta(days=duree)

                        cout = prix_jour * duree

                        paiement = {
                            "type": random.choice(["carte de crédit", "carte de débit", "comptant"]),
                            "fournisseur": faker.credit_card_provider(),
                            "numero": faker.credit_card_number(),
                        }

                        cursor.execute(
                            """
                            INSERT INTO facture (id_client, id_succursale, date_facture, montant, montant_tps, montant_tvq, paiement)
                            VALUES (%s, %s, %s, %s, %s, %s, %s)
                            """,
                            (
                                random.randint(1, CLIENTS), # ------ client
                                random.randint(1, SUCCURSALES), # -- succursale
                                date_debut, # ---------------------- date de la facture
                                cout,
                                round(cout * 0.05, 2), # ----------- montant_tps
                                round(cout * 0.09975, 2), # -------- montant_tvq
                                Jsonb(paiement), # ----------------- mode de paiement
                            )
                        )
            
            
            

            

        