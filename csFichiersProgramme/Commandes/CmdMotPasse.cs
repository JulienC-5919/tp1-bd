public class CmdMotDePasse : Commande
{
    public void Executer(string[] args)
    {
        String? response = Program.EntrerMdp();

        if (string.IsNullOrWhiteSpace(response)) 
        {
            Console.WriteLine("Mot de passe non modifié.");
        }
        else {
            Program.connectionStringBuilder.Password = response;
            Console.WriteLine("Mot de passe changé. Écrivez connect pour vous connecter.");
        }
    }
    

    public string AideCourte => "Change le mot de passe de connexion à la base de données.";
    public string AideLongue => ": Change le mot de passe de connexion à la base de données.";
    public string NomFr => "motpasse";
    public string NomEn => "password";
    public string NomAbb => "mdp";
}