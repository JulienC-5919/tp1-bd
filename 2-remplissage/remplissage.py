
import datetime
import json
import random
import string

import psycopg
from psycopg.types.json import Jsonb
from faker import Faker

silencieux = False # Ne rien écrire à la console

SUCCURSALES = 25
LOUEURS = 125
CLIENTS = 4000
VOITURES = 600
LOCATIONS = 6000

# Les voitures ne pourront pas être louées ou remises en dehors des heures d'ouverture.
OUVERTURE = 9 # heure d'ouverture des succursales : 9:00 AM
FERMETURE = 17 # heure de fermeture des succursales : 5:00 PM

CHANCE_AMENDE_STATIONNEMENT = 20 # 1/20 chance qu'il y ait une contravention de stationnement pour une location donnée
CHANCE_AMENDE_VITESSE = 50 # 1/50 chance qu'il y ait une contravention pour excès de vitesse pour une location donnée
# Les clients ne font vraiment pas attention haha

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

abandonnees = 0 # Locations qui n'ont pas pu être enregistrées à cause de conflits d'horaire

if SUCCURSALES <= 0:
    raise ValueError("Le nombre de succursales doit être supérieur à 0")

if LOUEURS <= 0:
    raise ValueError("Le nombre de loueurs doit être supérieur à 0")

if CLIENTS <= 0:
    raise ValueError("Le nombre de clients doit être supérieur à 0")

if VOITURES <= 0:
    raise ValueError("Le nombre de voitures doit être supérieur à 0")

if LOUEURS < SUCCURSALES:
    raise ValueError("Le nombre de loueurs doit être au moins égal au nombre de succursales")

if FERMETURE <= OUVERTURE:
    raise ValueError("L'heure de fermeture doit être supérieure à l'heure d'ouverture")

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
        minute=random.randint(0,59)
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
                succursale,
                penalite
            RESTART IDENTITY
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
        if (not silencieux):
            print(f"{SUCCURSALES} succursales créées.")

        for i in range(LOUEURS):
            nom = faker.last_name()
            prenom = faker.first_name()
            salaire_heure = round(random.uniform(17, 47), 2)

            if i < SUCCURSALES:
                succursale = i + 1 # Éviter qu'une succursale n'ait aucun loueur
            else:
                succursale = random.randint(1, SUCCURSALES) # Assigner aléatoirement les loueurs au reste des succursales

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
                (
                    nom,
                    prenom,
                    salaire_heure,
                    succursale,
                    Jsonb(contact)),
                )
        if (not silencieux):
            print(f"{LOUEURS} loueurs créés.")

        cursor.execute("SELECT id, id_succursale FROM loueur")
        loueurs_par_succursale = {succursale_id: [] for succursale_id in range(1, SUCCURSALES + 1)}
        for loueur_id, succursale_id in cursor.fetchall():
            loueurs_par_succursale[succursale_id].append(loueur_id)

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
        if (not silencieux):
            print(f"{CLIENTS} clients créés.")

        

        for marque in MARQUES:
            cursor.execute(
                """
                INSERT INTO marque (nom)
                VALUES (%s)
                """,
                (marque,)
            )
        if (not silencieux):
            print(f"{len(MARQUES)} marques créées.")

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
        if (not silencieux):
            print(f"{len(MODELES)} modèles de voiture créés.") 

        for _ in range(VOITURES):
                        
                    cursor.execute(
                        """
                        INSERT INTO voiture(plaque, id_modele, no_serie, id_etat, kilometrage, date_construction, prix_jour, id_succursale, details)
                        VALUES (
                            %s, %s, %s, %s, %s, %s, %s, %s, %s
                        )
                        """,
                        (
                            faker.unique.license_plate(), # ----------------------------- plaque d'immatriculation
                            random.randint(1, len(MODELES)), # -------------------------- modèle
                            genererNumeroSerie(), # ------------------------------------- numéro de série
                            etatAleatoire(), # ------------------------------------------ état aléatoire
                            random.randint(0, 200000), # -------------------------------- kilometrage
                            faker.date_between(start_date="-10y", end_date="today"), # -- date de construction
                            round(random.uniform(15000, 80000), 2), # ------------------- Prix par jour
                            random.randint(1, SUCCURSALES) # --------------------------- succursale aléatoire
                        )
                    )
        if (not silencieux):
            print(f"{VOITURES} voitures créées.")

        # Prix chargés une seule fois pour éviter une requête par location
        cursor.execute("SELECT id, prix_jour, id_succursale FROM voiture")
        prix_par_voiture = [
            (id_voiture, float(prix_jour), id_succursale)
            for id_voiture, prix_jour, id_succursale in cursor.fetchall()
        ]

        for _ in range(LOCATIONS):

                        voiture_id, prix_jour, succursale = random.choice(prix_par_voiture)
                        
                        duree_location = random.randint(1, 30)
                        date_debut = faker.date_between(start_date="-2y", end_date="today")
                        date_fin = date_debut + datetime.timedelta(days=duree_location)

                        abandonner = False # Conflit d'horaire impossible à résoudre

                        # Recherche de conflits horaire
                        while True:
                            cursor.execute(
                                """
                               SELECT date_debut, date_fin
                               FROM location_voiture
                                WHERE id_voiture = %s
                                  AND date_debut <= %s
                                  AND date_fin >= %s
                                ORDER BY date_debut
                                """,
                                (voiture_id, date_fin, date_debut),
                            )

                            conflit = cursor.fetchone()

                            if conflit is None:
                                break

                            debut_existant, fin_existant = conflit

                            if debut_existant <= date_debut <= fin_existant:
                                # La nouvelle location commence pendant l'ancienne.
                                date_debut = fin_existant + datetime.timedelta(days=1)

                            elif debut_existant <= date_fin <= fin_existant:
                                # La nouvelle location finit pendant l'ancienne.
                                date_fin = debut_existant - datetime.timedelta(days=1)

                            else:
                                # La nouvelle location englobe entièrement l'ancienne.
                                abandonnees += 1
                                abandonner = True
                                break

                            if date_debut > date_fin:
                                abandonnees += 1
                                abandonner = True
                                break
                        # Fin de la recherche de conflits horaire

                        if abandonner:
                            continue

                        client = random.randint(1, CLIENTS)
                        cout = prix_jour * duree_location

                        chance = random.randint(1,20)
                        retard = 0
                        match chance:

                            # Cas rare où la voiture est retournée en retard
                            case 1:
                                retard = random.randint(1, 3)
                                duree_utilisation = duree_location + retard

                            # cas rare où la voiture est retournée beaucoup plus tôt que prévu
                            case 2:
                                duree_utilisation = random.randint(1, duree_location)
                            # Normalement, la durée d'utilisation est légèrement inférieure ou égale à la durée de location
                            case _:
                                duree_utilisation = duree_location - random.randint(0,1)
                        
                        date_retour = date_debut + datetime.timedelta(
                            days=duree_utilisation
                        )

                        retour = datetime.datetime.combine(
                             date_retour,
                            heureAleatoire(),
                        )

                        if retour > datetime.datetime.now():
                            retour = None

                            if retard == 0:
                                cursor.execute(
                                    """
                                    UPDATE voiture
                                 
                                    SET id_etat = (
                                        SELECT id_etat
                                        FROM etat_vehicule
                                        WHERE nom = 'loué'
                                    )
                                    WHERE id = %s
                                    """,
                                    (voiture_id,)
                                )
                            else:
                                cursor.execute(
                                    """
                                    UPDATE voiture

                                    SET id_etat = (
                                        SELECT id_etat
                                        FROM etat_vehicule
                                        WHERE nom = 'manquant'
                                    )
                                    WHERE id = %s
                                    """,
                                    (voiture_id,)
                                )


                        paiement = {
                            "type": random.choice(["carte de crédit", "carte de débit", "comptant"]),
                            "fournisseur": faker.credit_card_provider(),
                            "numero": faker.credit_card_number(),
                        }

                        # Facture normale de la location
                        cursor.execute(
                            """
                            INSERT INTO facture (id_client, id_succursale, date_facture, montant, montant_tps, montant_tvq, paiement)
                            VALUES (%s, %s, %s, %s, %s, %s, %s)
                            RETURNING id
                            """,
                            (
                                client, # -------------------- client
                                succursale, # ---------------- succursale
                                date_debut, # ---------------- date de la facture
                                cout, # ---------------------- coût de la location
                                round(cout * 0.05, 2), # ----- montant_tps
                                round(cout * 0.09975, 2), # -- montant_tvq
                                Jsonb(paiement), # ----------- mode de paiement
                            )
                        )

                        # Location de la voiture
                        cursor.execute(
                            """
                            INSERT INTO location_voiture (id_voiture, id_facture, id_loueur, date_debut, date_fin, retour, kilometrage)
                            VALUES (%s, %s, %s, %s, %s, %s, %s)
                            RETURNING id
                            """,
                            (
                                voiture_id, # ----------------------------------------------- voiture
                                cursor.fetchone()[0], # ------------------------------------- facture
                                random.choice(loueurs_par_succursale[succursale]), # -------- loueur de la même succursale
                                date_debut, # ----------------------------------------------- date de début
                                date_fin, # ------------------------------------------------- date de fin
                                retour, # --------------------------------------------------- Date et heure du retour
                                random.uniform(15 * duree_location, 55 * duree_location) # -- kilométrage
                            )
                        )

                        # Retard
                        penalite_retard = retard * prix_jour * 4

                        # Contravention de stationnement
                        if (random.randint(1, CHANCE_AMENDE_STATIONNEMENT) == 1):
                            penalite_stationnement = random.randint(10, 18) * 5
                        else:
                            penalite_stationnement = 0

                        # Contravention pour excès de vitesse
                        if (random.randint(1, CHANCE_AMENDE_VITESSE) == 1):
                            penalite_vitesse = random.randint(5, 16) * 10
                        else:
                            penalite_vitesse = 0

                        penalite_totale = penalite_retard + penalite_stationnement + penalite_vitesse

                        # Facture pour les pénalités
                        if penalite_totale > 0:

                            id_location = cursor.fetchone()[0]

                            cursor.execute(
                                """
                                INSERT INTO facture (id_client, id_succursale, date_facture, montant, montant_tps, montant_tvq, paiement)
                                VALUES (%s, %s, %s, %s, %s, %s, %s)
                                RETURNING id
                                """,
                                (
                                    client,
                                    succursale,
                                    date_retour,
                                    penalite_totale,
                                    round(penalite_totale * 0.05, 2),
                                    round(penalite_totale * 0.09975, 2),
                                    Jsonb(paiement)
                                )
                            )

                            facture_penalite = cursor.fetchone()[0]

                            if penalite_retard > 0:
                                cursor.execute(
                                    """
                                    INSERT INTO penalite (id_location, id_facture, montant, raison)
                                    VALUES (%s, %s, %s, %s)
                                    """,
                                    (
                                        id_location, # ---------------------------------- location
                                        facture_penalite, # ----------------------------- facture associée à la pénalité
                                        penalite_retard, # ------------------------------ montant de la pénalité
                                        Jsonb({'raison': 'retard', 'duree': retard}) # -- raison de la pénalité
                                    )
                                )

                            if penalite_stationnement > 0:
                                cursor.execute(
                                    """
                                    INSERT INTO penalite (id_location, id_facture, montant, raison)
                                    VALUES (%s, %s, %s, %s)
                                    """,
                                    (
                                        id_location, # --------------------------------------------------------------- location
                                        facture_penalite, # ---------------------------------------------------------- facture associée à la pénalité
                                        penalite_stationnement, # ---------------------------------------------------- montant de la pénalité
                                        Jsonb({'raison': 'contravention', 'type_contravention': 'stationnement'}) # -- raison de la pénalité
                                    )
                                )

                            if penalite_vitesse > 0:
                                cursor.execute(
                                    """
                                    INSERT INTO penalite (id_location, id_facture, montant, raison)
                                    VALUES (%s, %s, %s, %s)
                                    """,
                                    (
                                        id_location, # ----------------------------------------------------------------- location
                                        facture_penalite, # ------------------------------------------------------------ facture associée à la pénalité
                                        penalite_vitesse, # ------------------------------------------------------------ montant de la pénalité
                                        Jsonb({'raison': 'contravention', 'type_contravention': 'excès de vitesse'}) # -- raison de la pénalité
                                    )
                                )


if (abandonnees > 0):
    print(f"{abandonnees} locations n'ont pas pu être enregistrées.")
else:
    print("Toutes les locations ont été enregistrées avec succès.")
        