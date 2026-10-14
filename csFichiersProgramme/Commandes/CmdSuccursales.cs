public class CmdSuccursales : Commande
{
    
    public string AideCourte => "Affiche la liste des succursales.";
    public string AideLongue => ": Affiche l'ID et l'adresse de toutes les succursales.";
    public string NomFr => "succursales";
    public string NomEn => "branches";
    public string NomAbb => "ss";
    

    public void Executer(string[] args)
    {

        using (var cmd = new Npgsql.NpgsqlCommand("SELECT * FROM succursale", Program.connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var adresse = Adresse.DepuisJson(reader.GetString(reader.GetOrdinal("adresse")));
                Console.WriteLine($"{reader["id"]}. {adresse?.ToStringComplet()}");
            }
        }
    }
}