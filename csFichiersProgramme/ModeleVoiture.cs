public enum TypeVoiture
{
    Berline,
    Coupe,
    Hatchback,
    VUS,
    Familiale,
    Camionnette
}

public class ModeleVoiture : AffichableGrille
{
    public string Nom { get; set; }
    public String Marque { get; set; }


    public override string[] Donnees => throw new NotImplementedException();

    protected override IReadOnlyCollection<string> Colonnes => throw new NotImplementedException();

    public override string get(int index)
    {
        throw new NotImplementedException();
    }

    public override bool set(int index, string value)
    {
        throw new NotImplementedException();
    }
}