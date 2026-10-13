public class CmdConnecter : Commande
{
    public void Executer(string[] args)
    {
        Program.connect();
    }

    public string AideCourte => "Connecte le programme à la base de données.";
    public string AideLongue => "Connecte le programme à la base de données en utilisant les paramètres actuels de connexion.";
    public string NomFr => "connecter";
    public string NomEn => "connect";
    public string NomAbb => "c";
}