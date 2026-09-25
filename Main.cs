using System;

public enum EtatVehicule
{
    disponible,
    loue,
    defectueux,
    manquant,
    debarras
}

public enum TypeVoiture 
{
    berline,
    coupé,
    hatchback,
    VUS,
    familiale,
    camionnette
}

class Program
{
    static void Main()
    {
        GestionBD bd = new GestionBD("localhost", "5432", "tp1_julien", "robot", "yfzvjp9y3s9j37q");
        
        Adresse adresse = new Adresse { Pays = "Canada",Rue = "123 Rue Exemple", Ville = "Ville Exemple", CodePostal = "12345" };
        Contact contact = new Contact { Email = "moi@example.com", Telephone = "11111111111", Adresse = adresse };
        Client client = new Client { Nom = "Dupont", Prenom = "Jean", Contact = contact };

        Console.WriteLine(bd.ajouter(client));
    }
}
