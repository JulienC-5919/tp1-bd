public class CmdVoitures : Commande
{
    public void Executer(string[] args)
    {
        int succursale;
        if (args.Length == 0)
        {
            succursale = Program.succursale?.Id ?? 0;
        }
        else
        {
            if (!int.TryParse(args[0], out succursale))
            {
                Console.WriteLine("Numéro de succursale invalide.");
                return;
            }
        }

        if (succursale == 0)
        {
            Console.Write("Afficher les voitures de toutes les succursales ? (o/N) : ");
            var reponse = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            if (reponse != "o" && reponse != "oui" && reponse != "y" && reponse != "yes")
            {
                Console.WriteLine("Annulé.");
                return;
            }
        }

        try
        {
            const string sql = @"
                SELECT v.plaque, ma.nom, m.nom, e.nom, v.prix_jour, v.id_succursale
                FROM voiture v
                JOIN etat_vehicule e ON e.id = v.id_etat
                JOIN modele_voiture m ON m.id = v.id_modele
                JOIN marque ma ON ma.id = m.id_marque
                WHERE @succ = 0 OR v.id_succursale = @succ
                ORDER BY v.id_succursale, v.plaque";

            using var cmd = new Npgsql.NpgsqlCommand(sql, Program.connection);
            cmd.Parameters.AddWithValue("succ", succursale);
            using var r = cmd.ExecuteReader();

            int total = 0;
            while (r.Read())
            {
                Console.WriteLine($"{r.GetString(1)} {r.GetString(2)} : {r.GetString(0)}, {r.GetString(3)},{r.GetDecimal(4):0.00} $/jour" + (succursale == 0 ? $" | succursale {r.GetInt32(5)}" : ""));
                total++;
            }
            if (total == 0)
                Console.WriteLine("Aucune voiture trouvée.");
        }
        catch (Npgsql.NpgsqlException ex)
        {
            Console.WriteLine($"Erreur de base de données: {ex.Message}");
        }
    }

    public string AideCourte => "Affiche les voitures.";
    public string AideLongue => "[NoSuccursale | 0] : Affiche les voitures de la succursale correspondante.\n\n\tNoSuccursale : Numéro de la succursale. Si omis, les voitures de la succursale actuelle seront affichées.\n\n\t0 : Affiche les voitures de toutes les succursales.";
    public string NomFr => "voitures";
    public string NomEn => "cars";
    public string NomAbb => "vs";
}