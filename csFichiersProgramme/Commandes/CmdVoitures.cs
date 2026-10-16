public class CmdVoitures : Commande
{
    public void Executer(string[] args)
    {
        int succursale;
        if (args.Length == 0)
        {
            succursale = Program.succursale?.Id ?? 0;
        }
        else
        {
            if (!int.TryParse(args[0], out succursale))
            {
                Console.WriteLine("Numéro de succursale invalide.");
                return;
            }
        }

        
    }

    public string AideCourte => "Affiche les voitures.";
    public string AideLongue => "[NoSuccursale | 0] : Affiche les voitures de la succursale correspondante.\n\n\tNoSuccursale : Numéro de la succursale. Si omis, les voitures de la succursale actuelle seront affichées.\n\n\t0 : Affiche les voitures de toutes les succursales.";
    public string NomFr => "voitures";
    public string NomEn => "cars";
    public string NomAbb => "vs";
}