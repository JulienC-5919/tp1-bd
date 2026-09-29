public class Contact : ObjetJson
{
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    public Adresse? Adresse { get; set; }

    public override string ToString()
    {
        return $"{Email}, {Telephone}, {Adresse}";
    }
}