public abstract class AffichableGrille
{
    protected abstract IReadOnlyCollection<string> Colonnes { get; }
    public string[] ColonnesArray => Colonnes.ToArray();
    public abstract String[] Donnees { get; }

    public abstract bool set(int index, string value);
    public abstract string get(int index);
}