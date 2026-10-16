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
    public static Voiture? voiture = null;

    public static string utilisateur = "robot";
    static Dictionary<string, string> config = new();
    public static Commande[] commandes = new Commande[] {
        new CmdFin(),
        new CmdAide(),
        new CmdMotDePasse(),
        new CmdHote(),
        new CmdUtilisateur(),
        new CmdBaseDonnees(),
        new CmdConnecter(),
        new CmdInfo()
    };

    public static Commande[] commandesBd = new Commande[] {
        new CmdSuccursales(),
        new CmdSuccursale(),
        new CmdVoiture(),
        new CmdVoitures()
    };
    static void Main(string[] args)
    {
        bool commandeReussie = false;
        config = ChargerConfig();
        string hoteDefaut = config.GetValueOrDefault("DB_HOST", "localhost");
        string utilisateurDefaut = config.GetValueOrDefault("DB_USER", "robot");
        string bdDefaut = config.GetValueOrDefault("DB_NAME", "tp1_julien");

        if (args.Length >= 1) connectionStringBuilder.Host = args[0];
        else connectionStringBuilder.Host = prompt($"({hoteDefaut} par défaut) Hôte: ", hoteDefaut);

        if (args.Length >= 2) {
            connectionStringBuilder.Username = args[1];
            utilisateur = args[1];
            }
        else {
            connectionStringBuilder.Username = prompt($"({utilisateurDefaut} par défaut) Nom d'utilisateur: ", utilisateurDefaut);
            utilisateur = connectionStringBuilder.Username;
        }

        if (args.Length >= 3) connectionStringBuilder.Database = args[2];
        else connectionStringBuilder.Database = prompt($"({bdDefaut} par défaut) Base de données: ", bdDefaut);
        
        if (config.TryGetValue("DB_PORT", out var port) && int.TryParse(port, out var portNum))
            connectionStringBuilder.Port = portNum;

        string mdpConfig = config.GetValueOrDefault("DB_PASSWORD", "");
        string reponseMdp = EntrerMdp(string.IsNullOrEmpty(mdpConfig)
            ? "Mot de passe: "
            : "(mot de passe dans connexion.env par défaut) Mot de passe: ");
        connectionStringBuilder.Password = string.IsNullOrEmpty(reponseMdp) ? mdpConfig : reponseMdp;

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

    static Dictionary<string, string> ChargerConfig()
    {
        var resultat = new Dictionary<string, string>();
        string? dossier = AppContext.BaseDirectory;
        while (dossier != null && !File.Exists(Path.Combine(dossier, "connexion.env")))
            dossier = Path.GetDirectoryName(dossier);
        if (dossier == null)
        {
            Console.WriteLine("connexion.env introuvable, valeurs par défaut utilisées.");
            return resultat;
        }

        foreach (var ligne in File.ReadLines(Path.Combine(dossier, "connexion.env")))
        {
            var l = System.Text.RegularExpressions.Regex.Replace(ligne.Trim(), @"\s+#.*$", "");
            int egal = l.IndexOf('=');
            if (l.Length == 0 || l.StartsWith('#') || egal < 0) continue;
            resultat[l[..egal].Trim()] = l[(egal + 1)..].Trim();
        }
        return resultat;
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