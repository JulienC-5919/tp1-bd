CREATE EXTENSION IF NOT EXISTS btree_gist;

CREATE TABLE succursale (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    adresse jsonb NOT NULL UNIQUE, -- Rue, ville, code postal, pays

    -- La première lettre du pays et de la ville doit être en majuscule
    CONSTRAINT adresse_premiere_lettre_majuscule CHECK (
        (adresse->>'pays') <> ''
        AND (adresse->>'ville') <> ''
        AND left(adresse->>'pays', 1) = upper(left(adresse->>'pays', 1))
        AND left(adresse->>'ville', 1) = upper(left(adresse->>'ville', 1))
    )
);

CREATE TABLE loueur (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    prenom VARCHAR(255) NOT NULL,
    salaire_heure DECIMAL(10, 2) NOT NULL,
    id_succursale INT NOT NULL REFERENCES succursale(id),
    contact JSONB NOT NULL, -- Email, téléphone, etc.

    CONSTRAINT contact_non_vide CHECK (
        (contact->>'email') <> ''
        OR (contact->>'telephone') <> ''
    ),

    CONSTRAINT prenom_valide CHECK (
        char_length(btrim(prenom)) >= 2
        AND btrim(prenom) ~ '^[[:alpha:]].*[[:alpha:]]$'
    ),

    CONSTRAINT nom_valide CHECK (
        char_length(btrim(nom)) >= 2
        AND btrim(nom) ~ '^[[:alpha:]].*[[:alpha:]]$'
    )
);

CREATE UNIQUE INDEX loueur_email_unique
    ON loueur (lower(contact->>'email'))
    WHERE contact->>'email' IS NOT NULL;

CREATE UNIQUE INDEX loueur_telephone_unique
    ON loueur ((contact->>'telephone'))
    WHERE contact->>'telephone' IS NOT NULL;



CREATE TABLE client (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(255) NOT NULL,
    prenom VARCHAR(255) NOT NULL,
    contact JSONB NOT NULL, -- Email, téléphone, etc.

    CONSTRAINT contact_non_vide CHECK (
        (contact->>'email') <> ''
        OR (contact->>'telephone') <> ''
    ),

    CONSTRAINT prenom_valide CHECK (
        char_length(btrim(prenom)) >= 2
        AND btrim(prenom) ~ '^[[:alpha:]].*[[:alpha:]]$'
    ),

    CONSTRAINT nom_valide CHECK (
        char_length(btrim(nom)) >= 2
        AND btrim(nom) ~ '^[[:alpha:]].*[[:alpha:]]$'
    )
);

CREATE UNIQUE INDEX client_email_unique
    ON client (lower(contact->>'email'))
    WHERE contact->>'email' IS NOT NULL;

CREATE UNIQUE INDEX client_telephone_unique
    ON client ((contact->>'telephone'))
    WHERE contact->>'telephone' IS NOT NULL;



CREATE TABLE marque (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(63) NOT NULL UNIQUE
);

CREATE TABLE type_voiture (
    id SMALLINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(31) NOT NULL UNIQUE
);

CREATE TABLE modele_voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(63) NOT NULL,
    id_marque INT NOT NULL REFERENCES marque(id),
    id_type SMALLINT NOT NULL REFERENCES type_voiture(id),
    details JSONB NOT NULL -- Dimensions, année, etc.
);

CREATE TABLE etat_vehicule (
    id SMALLINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nom VARCHAR(15) NOT NULL UNIQUE
) ;

CREATE TABLE voiture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    plaque VARCHAR(15) NOT NULL UNIQUE,
    id_modele INT NOT NULL REFERENCES modele_voiture(id),
    no_serie VARCHAR(17) NOT NULL,
    id_etat SMALLINT NOT NULL REFERENCES etat_vehicule(id),
    kilometrage INT NOT NULL,
    date_construction DATE NOT NULL,
    prix_jour DECIMAL(10, 2) NOT NULL,
    id_succursale INT NOT NULL REFERENCES succursale(id)
);

CREATE TABLE facture (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_client INT NOT NULL REFERENCES client(id),
    id_succursale INT NOT NULL REFERENCES succursale(id),
    date_facture TIMESTAMP NOT NULL,
    montant DECIMAL(10, 2) NOT NULL,
    montant_tps DECIMAL(10, 2) NOT NULL,
    montant_tvq DECIMAL(10, 2) NOT NULL,
    paiement JSONB NOT NULL, -- Type de carte, numéro? etc.

    CONSTRAINT montants_non_negatifs CHECK (
        montant >= 0
        AND montant_tps >= 0
        AND montant_tvq >= 0
    )
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
    kilometrage DECIMAL(8, 2) NOT NULL,

    CONSTRAINT dates_location_valides CHECK (date_debut <= date_fin),
    CONSTRAINT voiture_dates_non_chevauchantes EXCLUDE USING GIST (
        id_voiture WITH =,
        daterange(date_debut, date_fin, '[]') WITH &&
    ),

    CONSTRAINT retour_apres_debut CHECK (retour IS NULL OR retour >= date_debut),

    CONSTRAINT kilometrage_non_negatif CHECK (kilometrage >= 0)
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

-- Variables fournies par main.sh / main.bat à partir de connexion.env :
-- db_name, db_user, db_password (peut être vide), db_has_password (true/false)
SELECT format('CREATE USER %I', :'db_user')
WHERE NOT EXISTS (SELECT FROM pg_roles WHERE rolname = :'db_user') \gexec
\if :db_has_password
ALTER USER :"db_user" WITH PASSWORD :'db_password';
\endif
GRANT CONNECT ON DATABASE :"db_name" TO :"db_user";
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO :"db_user";
GRANT INSERT, DELETE ON ALL TABLES IN SCHEMA public TO :"db_user";
REVOKE INSERT, DELETE ON TABLE etat_vehicule, type_voiture FROM :"db_user";
GRANT TRUNCATE ON TABLE succursale, loueur, client, marque,
	modele_voiture, voiture, facture, location_voiture, penalite TO :"db_user";
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO :"db_user";
GRANT UPDATE ON ALL SEQUENCES IN SCHEMA public TO :"db_user";
REVOKE UPDATE ON SEQUENCE etat_vehicule_id_seq, type_voiture_id_seq FROM :"db_user";
GRANT SELECT ON ALL TABLES IN SCHEMA public TO :"db_user";

ALTER TABLE location_voiture OWNER TO :"db_user";
ALTER TABLE facture OWNER TO :"db_user";
ALTER TABLE voiture OWNER TO :"db_user";
ALTER TABLE modele_voiture OWNER TO :"db_user";
ALTER TABLE marque OWNER TO :"db_user";
ALTER TABLE client OWNER TO :"db_user";
ALTER TABLE loueur OWNER TO :"db_user";
ALTER TABLE succursale OWNER TO :"db_user";
ALTER TABLE penalite OWNER TO :"db_user";


