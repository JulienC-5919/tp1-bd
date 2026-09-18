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

-- Éventuellement permettre de louer plusieurs véhicules dans une seule facture
CREATE TABLE location_voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_voiture INT NOT NULL REFERENCES voiture(id),
    id_facture INT NOT NULL REFERENCES facture(id),
    id_loueur INT NOT NULL REFERENCES loueur(id),
    date_debut DATE NOT NULL,
    date_fin DATE NOT NULL,
    retour TIMESTAMP,
    kilometrage DECIMAL(8, 2) NOT NULL
);

CREATE TABLE penalite (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY, -- Supprimer?
    id_location INT NOT NULL REFERENCES location_voiture(id),
    id_facture INT NOT NULL REFERENCES facture(id), -- Facture associée à la pénalité, différente de celle de la location
    montant DECIMAL(10, 2) NOT NULL,
    raison JSONB NOT NULL -- Raison de la pénalité, détails supplémentaires
);

--------------------| Tables dont le contenu ne sera plus modifié |--------------------

TRUNCATE TABLE etat_vehicule, type_voiture RESTART IDENTITY CASCADE;

INSERT INTO etat_vehicule (nom) VALUES
('disponible'),
('loué'),
('défectueux'),
('manquant'),
('débarras');

INSERT INTO type_voiture(nom) VALUES
('berline'),
('coupé'),
('hatchback'),
('VUS'),
('familiale'),
('camionnette');

-------------------| Utilisateur |-------------------

DROP USER IF EXISTS robot;
CREATE USER robot WITH PASSWORD 'yfzvjp9y3s9j37q'; -- Supprimer mdp?
GRANT CONNECT ON DATABASE tp1_julien TO robot;
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO robot;
GRANT INSERT, DELETE ON ALL TABLES IN SCHEMA public TO robot;
REVOKE INSERT, DELETE ON TABLE etat_vehicule, type_voiture FROM robot;
GRANT TRUNCATE ON TABLE succursale, loueur, client, marque,
	modele_voiture, voiture, facture, location_voiture, penalite TO robot;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO robot;
GRANT UPDATE ON ALL SEQUENCES IN SCHEMA public TO robot;
REVOKE UPDATE ON SEQUENCE etat_vehicule_id_seq, type_voiture_id_seq FROM robot;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO robot;

ALTER TABLE location_voiture OWNER TO robot;
ALTER TABLE facture OWNER TO robot;
ALTER TABLE voiture OWNER TO robot;
ALTER TABLE modele_voiture OWNER TO robot;
ALTER TABLE marque OWNER TO robot;
ALTER TABLE client OWNER TO robot;
ALTER TABLE loueur OWNER TO robot;
ALTER TABLE succursale OWNER TO robot;
ALTER TABLE penalite OWNER TO robot;


