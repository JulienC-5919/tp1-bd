public class CmdFin : Commande
{
    public void Executer(string[] args)
    {
        Program.connection?.Dispose();
        Environment.Exit(0);
    }

    public string AideCourte => "Quitte le programme";
    public string AideLongue => ": Ferme la connexion et quitte le programme.";
    public string NomFr => "fin";
    public string NomEn => "exit";
    public string NomAbb => "q";
}