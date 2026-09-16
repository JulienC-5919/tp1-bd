--------------------| Tables dont le contenu ne sera plus modifié |--------------------

INSERT INTO etat_vehicule (nom) VALUES
('disponible'),
('loue'),
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

CREATE USER robot WITH PASSWORD 'yfzvjp9y3s9j37q'; -- Supprimer mdp?
GRANT CONNECT ON DATABASE tp1_julien TO robot;
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
GRANT USAGE ON SCHEMA public TO robot;
GRANT INSERT, DELETE ON ALL TABLES IN SCHEMA public TO robot;
REVOKE INSERT, DELETE ON TABLE etat_vehicule, type_voiture FROM robot;
GRANT TRUNCATE ON TABLE succursale, loueur, client, marque_voiture,
	modele_voiture, voiture, facture, location_voiture TO robot;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO robot;
GRANT UPDATE ON ALL SEQUENCES IN SCHEMA public TO robot;
REVOKE UPDATE ON SEQUENCE etat_vehicule_id_seq, type_voiture_id_seq FROM robot;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO robot;