using System.Numerics;

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
    public required string Nom { get; set; }
    public required String Marque { get; set; }
    public required TypeVoiture Type { get; set; }

    public short? Annee { get; set; }
    public Vector3? dimensions { get; set; }

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