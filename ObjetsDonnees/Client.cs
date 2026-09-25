public class Client : ObjetDonneesSql
{
    public override IReadOnlyList<string> Colonnes => new List<string> { "Nom", "Prenom" };

    public override IReadOnlyList<object?> Valeurs => new List<object?> { Nom, Prenom };

    public override String commandeInsertion => $"INSERT INTO Client (nom, prenom) VALUES (@Nom, @Prenom)";

    public string Nom { get; set; }
    public string Prenom { get; set; }

    public Contact Contact { get; set; }
}