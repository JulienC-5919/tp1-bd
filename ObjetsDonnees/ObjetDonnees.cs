public abstract class ObjetDonnees
{
    public abstract IReadOnlyList<string> Colonnes { get; }

    public abstract IReadOnlyList<object?> Valeurs { get; }

    public IReadOnlyList<string> ValeursAffichage =>
        Valeurs
            .Select(valeur => valeur?.ToString() ?? "Inconnu(e)")
            .ToList();
}