public class Adresse : ObjetJson
{
    public required string Pays { get; set; }
    public required string Rue { get; set; }
    public required string Ville { get; set; }
    public string? CodePostal { get; set; }

    public override string ToString()
    {
        return Rue; // Seulement la rue pour afficher dans des petites cases
    }
}