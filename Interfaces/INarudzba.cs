namespace InventarApp.Interfaces
{
    public interface INarudzba
    {
        string NarudzbaId { get; set; }
        string ProizvodId { get; set; }
        string DobavljacId { get; set; }
        int Kolicina { get; set; }
        DateTime DatumNarudzbe { get; set; }

        double IzracunajUkupno();
        void OznacKaoIsporuceno();
    }
}
