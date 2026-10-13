public class CmdBaseDonnees : Commande
{
    public void Executer(string[] args)
    {
        if (args.Length == 0) {
            String? response = Program.prompt("Base de données: ");

            if (string.IsNullOrWhiteSpace(response)) {
                Console.WriteLine("Base de données non modifiée.");
            }
            else {
                Program.connectionStringBuilder.Database = response;
                Console.WriteLine("Base de données changée. Écrivez connect pour vous connecter.");
            }
        }
            
        else {
            Program.connectionStringBuilder.Database = args[0];
        }
    }
    

    public string AideCourte => "Change la base de données.";
    public string AideLongue => "[NOM_BASE] : Change la base de données.\n\n\tNOM_BASE: La nouvelle base de données. Si omis, le programme demandera la base de données.";
    public string NomFr => "basedonnees";
    public string NomEn => "database";
    public string NomAbb => "bd";
}