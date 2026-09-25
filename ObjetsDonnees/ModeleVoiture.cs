public class ModeleVoiture : ObjetDonneesSql
{
    public override IReadOnlyList<string> Colonnes => new List<string> { "Nom", "Type" };

    public override IReadOnlyList<object?> Valeurs => new List<object?> { Nom, Type };

    public override String commandeInsertion => $"INSERT INTO ModeleVoiture (nom, marque, type, details) VALUES (@Nom, @Marque, @Type, @Annee)";

    public string Nom { get; set; }
    public string Marque { get; set; }
    public TypeVoiture Type { get; set; }

    public short? Annee { get; set; }

    public ModeleVoiture(string nom, string marque, TypeVoiture type, short? annee)
    {
        Nom = nom;
        Marque = marque;
        Type = type;
        Annee = annee;
    }
}