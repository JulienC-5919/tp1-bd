public class CmdUtilisateur : Commande
{
    public void Executer(string[] args)
    {
        if (args.Length == 0) {
            String? response = Program.prompt("Utilisateur: ");

            if (string.IsNullOrWhiteSpace(response)) {
                Console.WriteLine("Utilisateur non modifié.");
            }
            else
            {
                Program.connectionStringBuilder.Username = response;
                Program.utilisateur = response;
                Console.WriteLine("Utilisateur changé. Écrivez connect pour vous connecter.");
            }
        }
            
        else {
            Program.connectionStringBuilder.Username = args[0];
            Program.utilisateur = args[0];
        }
    }
    

    public string AideCourte => "Change l'utilisateur de la base de données.";
    public string AideLongue => "[NOM_UTILISATEUR] : Change l'utilisateur de la base de données.\n\n\tNOM_UTILISATEUR: Le nouvel utilisateur. Si omis, le programme demandera l'utilisateur.";
    public string NomFr => "utilisateur";
    public string NomEn => "user";
    public string NomAbb => "u";
}