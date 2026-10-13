public class CmdInfosConnexion : Commande
{
    public void Executer(string[] args)
    {
        Console.WriteLine("Hôte: " + Program.connectionStringBuilder.Host);
        Console.WriteLine("Utilisateur: " + Program.connectionStringBuilder.Username);
        Console.WriteLine("Base de données: " + Program.connectionStringBuilder.Database);
        Console.WriteLine("Mot de passe défini: " + (!string.IsNullOrEmpty(Program.connectionStringBuilder.Password) ? "Oui" : "Non"));

        Console.WriteLine("\nConnecté: " + (Program.connectionOpened ? "Oui" : "Non"));
    }

    public string AideCourte => "Affiche les informations de connexion actuelles.";
    public string AideLongue => ": Affiche les informations de connexion actuelles.";
    public string NomFr => "infosconnexion";
    public string NomEn => "connectioninfo";
    public string NomAbb => "i";
}