using System;
using Npgsql;


public enum EtatVehicule
{
    disponible,
    loue,
    defectueux,
    manquant,
    debarras
}



class Program
{
    public static NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder();
    public static NpgsqlConnection? connection = null;
    public static bool connectionOpened = false;

    public static Succursale? succursale = null;

    public static string utilisateur = "robot";
    public static Commande[] commandes = new Commande[] {
        new CmdFin(),
        new CmdAide(),
        new CmdMotDePasse(),
        new CmdHote(),
        new CmdUtilisateur(),
        new CmdBaseDonnees(),
        new CmdConnecter(),
    };

    public static Commande[] commandesBd = new Commande[] {
        new CmdSuccursales(),
        new CmdSuccursale()
    };
    static void Main(string[] args)
    {
        bool commandeReussie = false;

        if (args.Length >= 1) connectionStringBuilder.Host = args[0];
        else connectionStringBuilder.Host = prompt("(localhost par défaut) Hôte: ", "localhost");

        if (args.Length >= 2) {
            connectionStringBuilder.Username = args[1];
            utilisateur = args[1];
            }
        else {
            connectionStringBuilder.Username = prompt("(robot par défaut) Nom d'utilisateur: ", "robot");
        }

        if (args.Length >= 3) connectionStringBuilder.Database = args[2];
        else connectionStringBuilder.Database = prompt("(tp1_julien par défaut) Base de données: ", "tp1_julien");
        
        connectionStringBuilder.Password = EntrerMdp();

        connection = new NpgsqlConnection(connectionStringBuilder.ConnectionString);
        connect();

        while (true)
        {
            Console.Write("> ");
            String[] args2 = split(Console.ReadLine() ?? "");
            
            if (args2.Length == 0) continue;

            foreach (Commande v in Program.commandes)
            {
                if (v.NomFr == args2[0].ToLower() ||
                    v.NomEn == args2[0].ToLower() ||
                    v.NomAbb == args2[0].ToLower())
                {
                    v.Executer(args2.Skip(1).ToArray());
                    commandeReussie = true;
                    break;
                }
            }
            if (!commandeReussie) foreach (Commande v in Program.commandesBd)
            {
                if (v.NomFr == args2[0].ToLower() ||
                    v.NomEn == args2[0].ToLower() ||
                    v.NomAbb == args2[0].ToLower())
                {

                    if (!Program.connectionOpened) {
                        Console.WriteLine("Vous devez être connecté à la base de données pour utiliser cette commande.");
                        break;
                    }

                    v.Executer(args2.Skip(1).ToArray());
                    commandeReussie = true;
                    break;
                }
            }
            if (!commandeReussie) {
                Console.WriteLine(args2[0] + ": Commande inconnue, écrivez 'aide' pour la liste des commandes.");
            }
            commandeReussie = false;
        }
    }

    public static string EntrerMdp(string prompt = "Mot de passe: ")
        {
        Console.Write(prompt);
        var sb = new System.Text.StringBuilder();
        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return sb.ToString();
            }
            if (key.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0) sb.Length--;
            }
            else if (!char.IsControl(key.KeyChar))
            {
                sb.Append(key.KeyChar);
            }
        }
    }

    public static String prompt(string message, string defaultValue = "") {
        Console.Write(message);
        string answer = Console.ReadLine() ?? defaultValue;

        if (string.IsNullOrWhiteSpace(answer))
            return defaultValue;
        return answer;
    }

    // Les séquences d'échappement (\" et \\) sont conservées telles quelles; format() les interprète.
    static String[] split(String input) {
        var args = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;
        bool inToken = false;

        for (int i = 0; i < input.Length; i++) {
            char c = input[i];

            if (c == '\\' && i + 1 < input.Length) {
                current.Append(c).Append(input[++i]);
                inToken = true;
            } else if (c == '"') {
                inQuotes = !inQuotes;
                inToken = true;
            } else if (char.IsWhiteSpace(c) && !inQuotes) {
                if (inToken) {
                    args.Add(format(current.ToString()));
                    current.Clear();
                    inToken = false;
                }
            } else {
                current.Append(c);
                inToken = true;
            }
        }

        if (inToken)
            args.Add(format(current.ToString()));
        return args.ToArray();
    }

    static String format(String input) {
        input = input.Replace("\\\\", "\\").Replace("\\\"", "\"").Replace("\\n", "\n");
        return input;
    }

    static void afficherAide() 
    {
        Console.WriteLine("Aide:");
        Console.WriteLine("  exit, fin   : Quitter le programme");
        Console.WriteLine("  help, aide  : Afficher cette aide");

        if (connectionOpened); // todo afficher commandes bd
    }

    public static void connect() {
        try
        {
            connection.ConnectionString = connectionStringBuilder.ConnectionString;
            connection.Open();
            connectionOpened = true;
            Console.WriteLine("Connexion réussie. De nouvelles commandes sont disponibles.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la connexion: {ex.Message}");
        }
    }
}