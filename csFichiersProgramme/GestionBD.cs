using System.Data;

class GestionBD
{
    private string connectionString;

    public GestionBD(String serveur, String baseDeDonnees, String utilisateur, String motDePasse)
    {
        connectionString = $"Server={serveur};Database={baseDeDonnees};User Id={utilisateur};Password={motDePasse};";
    }

    public void reconnecter(String serveur, String baseDeDonnees, String utilisateur, String motDePasse)
    {
        connectionString = $"Server={serveur};Database={baseDeDonnees};User Id={utilisateur};Password={motDePasse};";
    }

    private bool ajout(string commande, params object[] parametres)
{
    try
    {
        using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new System.Data.SqlClient.SqlCommand(commande, connection))
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