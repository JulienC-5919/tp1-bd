public class CmdSuccursale : Commande
{
    public string AideCourte => "Sélectionner une succursale";
    public string AideLongue => "[ID | 0]: Sélectionne une succursale spécifique.\n\n\tID : L'identifiant de la succursale à sélectionner. Si omis, le programme demandera l'utilisateur.\n\n\t0 : Désélectionne la succursale actuelle.";
    public string NomFr => "succursale";
    public string NomEn => "branch";
    public string NomAbb => "s";

    public void Executer(string[] args)
    {
        string? saisie = args.Length > 0 ? args[0] : Program.prompt("Entrez l'ID de la succursale: ");

        if (!int.TryParse(saisie, out int id))
        {
            Console.WriteLine("ID invalide.");
            return;
        }

        if (id == 0)
        {
            Program.succursale = null;
            Console.WriteLine("Succursale désélectionnée.");
            return;
        }

        try
        {
            using var cmd = new Npgsql.NpgsqlCommand("SELECT adresse FROM succursale WHERE id = @id", Program.connection);
            cmd.Parameters.AddWithValue("id", id);

            var resultat = cmd.ExecuteScalar();
            if (resultat is not string json)
            {
                Console.WriteLine($"Aucune succursale avec l'ID {id}.");
                return;
            }

            var adresse = Adresse.DepuisJson(json)!;

            Program.succursale = new Succursale { Id = id, Adresse = adresse };
            Console.WriteLine($"Succursale {id} sélectionnée: {adresse.ToStringComplet()}");
        }
        catch (Npgsql.NpgsqlException ex)
        {
            Console.WriteLine($"Erreur de base de données: {ex.Message}");
        }
    }
}
