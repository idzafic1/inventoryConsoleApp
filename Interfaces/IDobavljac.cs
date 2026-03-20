namespace InventarApp.Interfaces
{
    public interface IDobavljac
    {
        string Id { get; set; }
        string Naziv { get; set; }
        string Kontakt { get; set; }

        string GetDobavljacInfo();
    }
}
