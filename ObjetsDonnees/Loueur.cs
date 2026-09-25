public class Loueur : ObjetDonneesSql
{
    public override IReadOnlyList<string> Colonnes => new List<string> {"Prenom", "Nom" };
    public override IReadOnlyList<object?> Valeurs => new List<object?> { Prenom, Nom };
    public override String commandeInsertion => $"INSERT INTO Loueur (prenom, nom) VALUES (@Prenom, @Nom)";

    public string Prenom { get; set; }
    public string Nom { get; set; }

    public float SalaireHeure { get; set; }
    public Succursale Succursale { get; set; }

    public Contact Contact { get; set; }
}