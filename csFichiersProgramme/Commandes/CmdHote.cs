public class CmdHote : Commande
{
    public void Executer(string[] args)
    {
        if (args.Length == 0) {
            String? response = Program.prompt("Hôte: ");

            if (string.IsNullOrWhiteSpace(response)) {
                Console.WriteLine("Hôte non modifié.");
            }
            else {
                Program.connectionStringBuilder.Host = response;
                Console.WriteLine("Hôte changé. Écrivez connect pour vous connecter.");
            }
        }
            
        else {
            Program.connectionStringBuilder.Host = args[0];
        }
    }
    

    public string AideCourte => "Change l'hôte du serveur de base de données.";
    public string AideLongue => "[NOM_HOTE] : Change le nom de l'hôte du serveur de base de données.\n\n\tNOM_HOTE: Le nouveau nom de l'hôte. Si omis, le programme demandera l'hôte.";
    public string NomFr => "hote";
    public string NomEn => "host";
    public string NomAbb => "h";
}