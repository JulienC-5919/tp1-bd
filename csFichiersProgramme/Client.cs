public class Client : AffichableGrille
{
    public required string Nom { get; set; }
    public required string Prenom { get; set; }
    public required Contact Contact { get; set; }

    public override string[] Donnees => [Nom, Prenom, Contact?.ToString() ?? string.Empty];

    protected override IReadOnlyCollection<string> Colonnes => ["Nom", "Prenom", "Contact"];

    public override string get(int index)
    {
        throw new NotImplementedException();
    }

    public override bool set(int index, string value)
    {
        throw new NotImplementedException();
    }
}