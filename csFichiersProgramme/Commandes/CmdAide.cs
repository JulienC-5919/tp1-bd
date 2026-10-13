public class CmdAide : Commande
{
    public void Executer(string[] args)
    {
        Console.WriteLine();
        if (args.Length >= 1)
        {
            switch (args[0].ToLower())
            {
                case "fr":
                    for (int i = 0; i < Program.commandes.Length; i++)
                    {
                        Console.WriteLine(Program.commandes[i].NomFr + " : " + Program.commandes[i].AideCourte);
                    }
                    break;

                case "en":
                    for (int i = 0; i < Program.commandes.Length; i++)
                    {
                        Console.WriteLine(Program.commandes[i].NomEn + " : " + Program.commandes[i].AideCourte);
                    }
                    break;

                case "abb":
                    for (int i = 0; i < Program.commandes.Length; i++)
                    {
                        Console.WriteLine(Program.commandes[i].NomAbb + " : " + Program.commandes[i].AideCourte);
                    }
                    break;

                default:
                    bool commandeTrouvee = false;
                    for (int i = 0; i < Program.commandes.Length; i++)
                    {
                        if (Program.commandes[i].NomFr.ToLower() == args[0].ToLower() ||
                            Program.commandes[i].NomEn.ToLower() == args[0].ToLower() ||
                            Program.commandes[i].NomAbb.ToLower() == args[0].ToLower())
                        {
                            Console.WriteLine(args[0] + " " + Program.commandes[i].AideLongue);
                            commandeTrouvee = true;
                            break;
                        }
                    }
                    if (!commandeTrouvee)
                    {
                        for (int i = 0; i < Program.commandes.Length; i++)
                        {
                            Console.WriteLine(Program.commandes[i].NomFr + " : " + Program.commandes[i].AideCourte);
                        }
                    }
                    break;
            }
        }
        else
        {
            for (int i = 0; i < Program.commandes.Length; i++)
            {
                Console.WriteLine(Program.commandes[i].NomFr + " : " + Program.commandes[i].AideCourte);
            }
        }
    }

    public string AideCourte => "Affiche l'aide";
    public string AideLongue => 
    "[COMMANDE|fr|en|abb]\n\n\tfr: Affiche toutes les commandes françaises\n\ten: Affiche toutes les commandes anglaises\n\tabb: Affiche toutes les commandes abrégées\n\n\tCOMMANDE: Affiche l'aide pour la commande spécifiée.";
    public string NomFr => "aide";
    public string NomEn => "help";
    public string NomAbb => "?";
}