namespace InventarApp.Interfaces
{
    public interface IProizvod
    {
        string Id { get; set; }
        string Naziv { get; set; }
        int Kolicina { get; set; }
        int MinimalniPrag { get; set; }
        string DobavljacId { get; set; }
        bool NijeZaliha { get; }

        void AzurirajKolicinu(int iznos);
    }
}
