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

    public string ToStringComplet()
    {
        return string.Join(", ", new[] { Rue, Ville, Pays, CodePostal }
            .Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    public static Adresse? DepuisJson(string json)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.SnakeCaseLower
        };
        return System.Text.Json.JsonSerializer.Deserialize<Adresse>(json, options);
    }
}
