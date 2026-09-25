public class Contact : ObjetDonneesJson
{
    public override IReadOnlyList<string> Colonnes => new List<string> { "Email", "Telephone" };

    public override IReadOnlyList<object?> Valeurs => new List<object?> { Email, Telephone };

    public string Email { get; set; }
    public string Telephone { get; set; }

    public Adresse Adresse { get; set; }
    protected override object JsonRepresentation => new { Email, Telephone, Adresse = Adresse.JsonRepresentation };
}