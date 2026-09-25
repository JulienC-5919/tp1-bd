using System.Data;
using Npgsql;

class GestionBD
{
    private string connectionString;

    public GestionBD(String host, String port, String database, String user, String password)
    {
        connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};";
    }

    public void reconnecter(String host, String port, String database, String user, String password)
    {
        connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};";
    }

    private bool ajout(string commande, params object[] parametres)
{
    try
    {
        using (var connection = new Npgsql.NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new Npgsql.NpgsqlCommand(commande, connection))
            {
                for (int i = 0; i < parametres.Length; i++)
                    command.Parameters.AddWithValue($"@param{i}", parametres[i]);

                command.ExecuteNonQuery();
                return true;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur : {ex.Message}");
        return false;
    }
}
    /*

    public bool ajouterSuccursale(Adresse adresse)
    {
        return ajout(
            "INSERT INTO Succursales (pays, ville, rue, codePostal) VALUES (@param0, @param1, @param2, @param3)",
            adresse.Pays, adresse.Ville, adresse.Rue, adresse.CodePostal
            );
    }*/

    public bool ajouter(ObjetDonneesSql objet)
    {
        return ajout(objet.commandeInsertion, objet.Valeurs.ToArray());
    }
     
}