public interface Commande
{
    public void Executer(string[] args);
    public String AideCourte { get; }
    public String AideLongue { get; }
    public String NomFr { get; }
    public String NomEn { get; }
    public String NomAbb { get; }
}