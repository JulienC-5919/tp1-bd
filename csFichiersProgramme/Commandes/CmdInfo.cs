public class CmdInfo : Commande
{
    public void Executer(string[] args)
    {
        if (args.Length == 0 || char.ToLower(args[0][0]) == 'c')
        {
            Console.WriteLine("Hôte: " + Program.connectionStringBuilder.Host);
            Console.WriteLine("Utilisateur: " + Program.connectionStringBuilder.Username);
            Console.WriteLine("Base de données: " + Program.connectionStringBuilder.Database);
            Console.WriteLine("Mot de passe défini: " + (!string.IsNullOrEmpty(Program.connectionStringBuilder.Password) ? "Oui" : "Non"));

            Console.WriteLine("\nConnecté: " + (Program.connectionOpened ? "Oui" : "Non"));

            return;
        }
        
        if (char.ToLower(args[0][0]) == 's')
        {
            if (Program.succursale == null)
            {
                Console.WriteLine("Aucune succursale sélectionnée.");
                return;
            }
            Console.WriteLine($"Succursale {Program.succursale.Id} sélectionnée: {Program.succursale.Adresse.ToStringComplet()}");
            return;
        }

        if (char.ToLower(args[0][0]) == 'v')
        {
            if (Program.voiture == null)
            {
                Console.WriteLine("Aucune voiture sélectionnée.");
                return;
            }

            string etat = Program.voiture.etat switch
            {
                EtatVehicule.disponible => "disponible",
                EtatVehicule.loue => "loué",
                EtatVehicule.defectueux => "défectueux",
                EtatVehicule.manquant => "manquant",
                _ => "débarras"
            };

            Console.WriteLine(
                $"{Program.voiture.modele.Marque} {Program.voiture.modele.Nom} sélectionnée."
                + $"\n\tImmatriculation: {Program.voiture.immatriculation}"
                + $"\n\tNoSerie: {Program.voiture.noSerie}"
                + $"\n\tSuccursale: {Program.voiture.succursale.Id}"
                + $"\n\tÉtat: {etat}"
                );
            return;
        }

        Console.WriteLine($"Argument inconnu: {args[0]}");
        
    }

    public string AideCourte => "Affiche des informations sur la connexion ou autre chose.";
    public string AideLongue => "[c | connexion | s | succursale] : Affiche les informations.\n\n\tc | connexion: Affiche les informations de connexion actuelles.\n\ts | succursale: Affiche les informations sur la succursale.";
    public string NomFr => "info";
    public string NomEn => "info";
    public string NomAbb => "i";
}