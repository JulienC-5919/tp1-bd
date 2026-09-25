using System.Text.Json;
using System.Text.Json.Serialization;

public class Adresse : ObjetDonneesJson
{
    public override IReadOnlyList<string> Colonnes => new List<string> { "Pays", "Rue", "Ville", "CodePostal" };

    public override IReadOnlyList<object?> Valeurs => new List<object?> { Pays, Rue, Ville, CodePostal };

    public string Pays { get; set; }
    public string Rue { get; set; }
    public string Ville { get; set; }
    public string? CodePostal { get; set; }

    public string CodePostalAffichage => CodePostal ?? "inconnu";

    public override string ToString()
    {
        return Rue; // Affiche uniquement la rue de l'adresse, pour les petites cases.
    }

    protected override object JsonRepresentation => new { Pays, Rue, Ville, CodePostal };
}