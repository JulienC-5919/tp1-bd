public class Voiture
{
    /*public Voiture(ModeleVoiture modele, string immatriculation, EtatVehicule etat)
    {
        this.modele = modele;
        this.immatriculation = immatriculation;
        this.etat = etat;
    }*/
    public required ModeleVoiture modele;
    public required string immatriculation;
    public required string noSerie;
    public required int kilometrage;
    public required DateTime dateConstruction;
    public required decimal prixJour;
    public required Succursale succursale;
    public required EtatVehicule etat;

}