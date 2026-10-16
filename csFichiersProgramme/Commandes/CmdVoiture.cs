public class CmdVoiture : Commande
{
    public void Executer(string[] args)
    {
        string immatriculation = args.Length > 0 ? args[0] : DemanderImmatriculation();

        if (string.IsNullOrWhiteSpace(immatriculation))
        {
            Console.WriteLine("Aucune immatriculation fournie, voiture non changée.");
            return;
        }

        if (immatriculation == "0")
        {
            Program.voiture = null;
            Console.WriteLine("Voiture désélectionnée.");
            return;
        }

        try
        {
            const string sql = @"
                SELECT v.no_serie, v.kilometrage, v.date_construction, v.prix_jour, v.id_succursale,
                       s.adresse::text, e.nom, m.nom, ma.nom, t.nom, m.details::text
                FROM voiture v
                JOIN succursale s ON s.id = v.id_succursale
                JOIN etat_vehicule e ON e.id = v.id_etat
                JOIN modele_voiture m ON m.id = v.id_modele
                JOIN marque ma ON ma.id = m.id_marque
                JOIN type_voiture t ON t.id = m.id_type
                WHERE v.plaque = @plaque";

            using var cmd = new Npgsql.NpgsqlCommand(sql, Program.connection);
            cmd.Parameters.AddWithValue("plaque", immatriculation);
            using var r = cmd.ExecuteReader();

            if (!r.Read())
            {
                Console.WriteLine($"Aucune voiture avec l'immatriculation {immatriculation}.");
                return;
            }

            var details = System.Text.Json.JsonDocument.Parse(r.GetString(10)).RootElement;
            short? annee = details.TryGetProperty("annee", out var a) ? a.GetInt16() : null;
            System.Numerics.Vector3? dimensions = null;
            if (details.TryGetProperty("dimensions", out var d))
                dimensions = new System.Numerics.Vector3(
                    d.GetProperty("largeur").GetSingle(),
                    d.GetProperty("hauteur").GetSingle(),
                    d.GetProperty("profondeur").GetSingle());

            TypeVoiture type = r.GetString(9) switch
            {
                "berline" => TypeVoiture.Berline,
                "coupé" => TypeVoiture.Coupe,
                "hatchback" => TypeVoiture.Hatchback,
                "VUS" => TypeVoiture.VUS,
                "familiale" => TypeVoiture.Familiale,
                _ => TypeVoiture.Camionnette
            };

            EtatVehicule etat = r.GetString(6) switch
            {
                "disponible" => EtatVehicule.disponible,
                "loué" => EtatVehicule.loue,
                "défectueux" => EtatVehicule.defectueux,
                "manquant" => EtatVehicule.manquant,
                _ => EtatVehicule.debarras
            };

            Program.voiture = new Voiture
            {
                immatriculation = immatriculation,
                noSerie = r.GetString(0),
                kilometrage = r.GetInt32(1),
                dateConstruction = r.GetDateTime(2),
                prixJour = r.GetDecimal(3),
                etat = etat,
                succursale = new Succursale { Id = r.GetInt32(4), Adresse = Adresse.DepuisJson(r.GetString(5))! },
                modele = new ModeleVoiture
                {
                    Nom = r.GetString(7),
                    Marque = r.GetString(8),
                    Type = type,
                    Annee = annee,
                    dimensions = dimensions
                }
            };
            Console.WriteLine($"Voiture {immatriculation} sélectionnée: {Program.voiture.modele.Marque} {Program.voiture.modele.Nom}");
        }
        catch (Npgsql.NpgsqlException ex)
        {
            Console.WriteLine($"Erreur de base de données: {ex.Message}");
        }
    }

    private string DemanderImmatriculation()
    {
        Console.Write("Veuillez saisir l'immatriculation de la voiture : ");
        return Console.ReadLine() ?? "";
    }

    public string AideCourte => "Sélectionne une voiture.";
    public string AideLongue => "[IMMATRICULATION | 0] : Sélectionne la voiture correspondante.\n\n\tIMMATRICULATION : Immatriculation de la voiture. Si omise, le programme demandera à l'utilisateur de la saisir.\n\n\t0 : Désélectionne la voiture actuelle.";
    public string NomFr => "voiture";
    public string NomEn => "car";
    public string NomAbb => "v";
}