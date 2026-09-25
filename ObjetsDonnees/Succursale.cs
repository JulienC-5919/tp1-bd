public class Succursale : ObjetDonneesSql
{
    public override IReadOnlyList<string> Colonnes => new List<string> {"Adresse" };
    public override IReadOnlyList<object?> Valeurs => new List<object?> { Adresse };
    public override String commandeInsertion => $"INSERT INTO Succursales (adresse) VALUES (@Adresse)";

    public Adresse Adresse { get; set; }
}