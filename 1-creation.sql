CREATE TABLE succursale (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    adresse jsonb NOT NULL -- Rue, ville, code postal, pays
);

CREATE TABLE loueur (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    prenom VARCHAR(255) NOT NULL,
    salaire_heure DECIMAL(10, 2) NOT NULL,
    id_succursale INT NOT NULL REFERENCES succursale(id),
    contact JSONB NOT NULL -- Email, téléphone, etc.
);

CREATE TABLE client (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    prenom VARCHAR(255) NOT NULL,
    contact JSONB NOT NULL -- Email, téléphone, etc.
);

CREATE TABLE marque (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(63) NOT NULL
);

CREATE TABLE type_voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(31) NOT NULL
);

CREATE TABLE modele_voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(63) NOT NULL,
    id_marque INT NOT NULL REFERENCES marque(id),
    id_type INT NOT NULL REFERENCES type_voiture(id),
    details JSONB NOT NULL -- Dimensions, année, etc.
);

CREATE TABLE etat_vehicule (
    id SMALLINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(15) NOT NULL
) ;

CREATE TABLE voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    plaque VARCHAR(15) NOT NULL UNIQUE,
    id_modele INT NOT NULL REFERENCES modele_voiture(id),
    no_serie VARCHAR(17) NOT NULL UNIQUE,
    id_etat SMALLINT NOT NULL REFERENCES etat_vehicule(id),
    kilometrage INT NOT NULL,
    date_construction DATE NOT NULL,
    prix_jour DECIMAL(10, 2) NOT NULL,
    id_succursale INT NOT NULL REFERENCES succursale(id),
    details JSONB NOT NULL -- Couleur, pièces, prix pour longue période, prix par km, etc.
);

CREATE TABLE facture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_client INT NOT NULL REFERENCES client(id),
    id_succursale INT NOT NULL REFERENCES succursale(id),
    date_facture TIMESTAMP NOT NULL,
    montant DECIMAL(10, 2) NOT NULL,
    montant_tps DECIMAL(10, 2) NOT NULL,
    montant_tvq DECIMAL(10, 2) NOT NULL,
    paiement JSONB NOT NULL -- Type de carte, numéro? etc.
);

CREATE TABLE location_voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_voiture INT NOT NULL REFERENCES voiture(id),
    id_facture INT NOT NULL REFERENCES facture(id),
    id_loueur INT NOT NULL REFERENCES loueur(id),
    date_debut DATE NOT NULL,
    date_fin DATE NOT NULL,
    retour TIMESTAMP,
    kilometrage INT NOT NULL
);




-- Modifications à faire dans schéma:
--    id_ en tant que préfixe
--    dimensions modele_voiture dans jsonb
-- diff entre tps tvq
-- suppr. id_client dans location_voiture, car c'est déjà dans facture