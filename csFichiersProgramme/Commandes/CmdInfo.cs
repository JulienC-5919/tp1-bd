public class CmdInfo : Commande
{
    public void Executer(string[] args)
    {
        if (args.Length == 0 || char.ToLower(args[0][0]) == 'c')
        {
            Console.WriteLine("Hôte: " + Program.connectionStringBuilder.Host);
            Console.WriteLine("Utilisateur: " + Program.connectionStringBuilder.Username);
            Console.WriteLine("Base de données: " + Program.connectionStringBuilder.Database);
            Console.WriteLine("Mot de passe défini: " + (!string.IsNullOrEmpty(Program.connectionStringBuilder.Password) ? "Oui" : "Non"));

            Console.WriteLine("\nConnecté: " + (Program.connectionOpened ? "Oui" : "Non"));

            return;
        }
        
        if (char.ToLower(args[0][0]) == 's')
        {
            if (Program.succursale == null)
            {
                Console.WriteLine("Aucune succursale sélectionnée.");
                return;
            }
            Console.WriteLine($"Succursale {Program.succursale.Id} sélectionnée: {Program.succursale.Adresse.ToStringComplet()}");
            return;
        }

        if (char.ToLower(args[0][0]) == 'v')
        {
            if (Program.voiture == null)
            {
                Console.WriteLine("Aucune voiture sélectionnée.");
                return;
            }

            string etat = Program.voiture.etat switch
            {
                EtatVehicule.disponible => "disponible",
                EtatVehicule.loue => "loué",
                EtatVehicule.defectueux => "défectueux",
                EtatVehicule.manquant => "manquant",
                _ => "débarras"
            };

            Console.WriteLine(
                $"{Program.voiture.modele.Marque} {Program.voiture.modele.Nom} sélectionnée."
                + $"\n\tImmatriculation: {Program.voiture.immatriculation}"
                + $"\n\tNoSerie: {Program.voiture.noSerie}"
                + $"\n\tSuccursale: {Program.voiture.succursale.Adresse.ToStringComplet()}"
                + $"\n\tÉtat: {etat}"
                + $"\n\tKilométrage: {Program.voiture.kilometrage} km"
                + $"\n\tDate de construction: {Program.voiture.dateConstruction:yyyy-MM-dd}"
                + $"\n\tPrix par jour: {Program.voiture.prixJour:0.00} $"
                );

            if (Program.voiture.etat == EtatVehicule.loue)
                AfficherLocationCourante(Program.voiture.immatriculation);
            return;
        }

        Console.WriteLine($"Argument inconnu: {args[0]}");
        
    }

    private void AfficherLocationCourante(string immatriculation)
    {
        const string sql = @"
            SELECT c.prenom, c.nom, c.contact->>'email', l.date_fin
            FROM location_voiture l
            JOIN voiture v ON v.id = l.id_voiture
            JOIN facture f ON f.id = l.id_facture
            JOIN client c ON c.id = f.id_client
            WHERE v.plaque = @plaque AND l.retour IS NULL
            ORDER BY l.date_debut DESC
            LIMIT 1";

        using var cmd = new Npgsql.NpgsqlCommand(sql, Program.connection);
        cmd.Parameters.AddWithValue("plaque", immatriculation);
        using var r = cmd.ExecuteReader();
        if (!r.Read())
            return;

        Console.WriteLine($"\tLoué par : {r.GetString(0)} {r.GetString(1)}");
        Console.WriteLine($"\tCourriel : {(r.IsDBNull(2) ? "(aucun)" : r.GetString(2))}");
        Console.WriteLine($"\tRetour prévu : {r.GetDateTime(3):yyyy-MM-dd}");
    }

    public string AideCourte => "Affiche des informations sur la connexion ou autre chose.";
    public string AideLongue => "[c | connexion | s | succursale] : Affiche les informations.\n\n\tc | connexion: Affiche les informations de connexion actuelles.\n\ts | succursale: Affiche les informations sur la succursale.";
    public string NomFr => "info";
    public string NomEn => "info";
    public string NomAbb => "i";
}