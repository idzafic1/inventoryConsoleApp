namespace InventarApp.Interfaces
{
    public interface IObavjestenje
    {
        string ObavjestenjeId { get; set; }
        string ProizvodId { get; set; }
        string Poruka { get; set; }
        DateTime VrijemeKreiranja { get; set; }

        string PosaljiIzvjestaj();
        string DajDetaljeObavjestenja();
    }
}
