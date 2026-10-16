

public class CmdAide : Commande
{
    const int LARGEUR_COLONNE = 21;
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
                        aidePourCommande(Program.commandes[i], Program.commandes[i].NomFr);
                    }
                    if (Program.connectionOpened)
                    {
                        Console.WriteLine("\nCommandes liées à la base de données :");
                        for (int i = 0; i < Program.commandesBd.Length; i++)
                        {
                            aidePourCommande(Program.commandesBd[i], Program.commandesBd[i].NomFr);
                        }
                    }
                    break;

                case "en":
                    for (int i = 0; i < Program.commandes.Length; i++)
                    {
                        aidePourCommande(Program.commandes[i], Program.commandes[i].NomEn);
                    }
                    if (Program.connectionOpened)
                    {
                        Console.WriteLine("\nCommandes liées à la base de données :");
                        for (int i = 0; i < Program.commandesBd.Length; i++)
                        {
                            aidePourCommande(Program.commandesBd[i], Program.commandesBd[i].NomEn);
                        }
                    }
                    break;

                case "abb":
                    for (int i = 0; i < Program.commandes.Length; i++)
                    {
                        aidePourCommande(Program.commandes[i], Program.commandes[i].NomAbb);
                    }
                    if (Program.connectionOpened)
                    {
                        Console.WriteLine("\nCommandes liées à la base de données :");
                        for (int i = 0; i < Program.commandesBd.Length; i++)
                        {
                            aidePourCommande(Program.commandesBd[i], Program.commandesBd[i].NomAbb);
                        }
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
                            aidePourCommande(Program.commandes[i], Program.commandes[i].NomFr);
                        }
                        if (Program.connectionOpened)
                        {
                            Console.WriteLine("\nCommandes liées à la base de données :");
                            for (int i = 0; i < Program.commandesBd.Length; i++)
                            {
                                aidePourCommande(Program.commandesBd[i], Program.commandesBd[i].NomFr);
                            }
                        }
                    }
                    break;
            }
        }
        else
        {
            for (int i = 0; i < Program.commandes.Length; i++)
            {
                aidePourCommande(Program.commandes[i], Program.commandes[i].NomFr);
            }
            if (Program.connectionOpened)
                {
                    Console.WriteLine("\nCommandes liées à la base de données :");
                    for (int i = 0; i < Program.commandesBd.Length; i++)
                    {
                        aidePourCommande(Program.commandesBd[i], Program.commandesBd[i].NomFr);
                    }
                }
        }
    }

    public string AideCourte => "Affiche l'aide";
    public string AideLongue => 
    "[COMMANDE|fr|en|abb]\n\n\tfr: Affiche toutes les commandes françaises\n\ten: Affiche toutes les commandes anglaises\n\tabb: Affiche toutes les commandes abrégées\n\n\tCOMMANDE: Affiche l'aide pour la commande spécifiée.";
    public string NomFr => "aide";
    public string NomEn => "help";
    public string NomAbb => "?";

    private void aidePourCommande(Commande commande, string nom)
    {
        Console.WriteLine(
            nom
            + new string(' ', LARGEUR_COLONNE - nom.Length)
            + ": "
            + commande.AideCourte);
    }

    void aideFr()
    {
        
    }
}