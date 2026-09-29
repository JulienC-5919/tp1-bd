using System;
using Npgsql;

public enum EtatVehicule
{
    disponible,
    loue,
    defectueux,
    manquant,
    debarras
}

class Program
{
    static void Main()
    {
        /*
        Adresse adresse = new Adresse { Pays = "Canada", Rue = "123 Rue Exemple", Ville = "Ville Exemple", CodePostal = "12345" };
        Contact contact = new Contact { Email = "moi@example.com", Telephone = "11111111111", Adresse = adresse };
        Client client = new Client { Nom = "Dupont", Prenom = "Jean", Contact = contact };/*/
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create("Host=localhost;Username=robot;Password=yfzvjp9y3s9j37q;Database=tp1_julien");
        //ClientRepository clientRepository = new ClientRepository(dataSource);
        MarqueRepository marqueRepository = new MarqueRepository(dataSource);
        
        try
        {
            int c = new MarqueRepository(dataSource).AjouterAsync("Comet").GetAwaiter().GetResult();
            Console.WriteLine(c);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
        }
        // int c = clientRepository.AjouterAsync(client).GetAwaiter().GetResult();
        // Console.WriteLine(c);
    }
}
