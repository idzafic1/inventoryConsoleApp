namespace InventarApp.Services
{
    public class HighTurnoverAnalyzer
    {
        // Record za čuvanje promjena
        private class PromjenaRecord
        {
            public string ProizvodId { get; set; }
            public int Promjena { get; set; }
            public DateTime Datum { get; set; }

            public PromjenaRecord(string proizvodId, int promjena, DateTime datum)
            {
                ProizvodId = proizvodId;
                Promjena = promjena;
                Datum = datum;
            }
        }

        // Historija svih promjena
        private List<PromjenaRecord> _historija = new List<PromjenaRecord>();

        // Registruje promjenu količine proizvoda
        public void RegistrujPromjenu(string proizvodId, int promjena, DateTime datum)
        {
            if (string.IsNullOrWhiteSpace(proizvodId))
            {
                throw new ArgumentException("ID proizvoda ne može biti prazan.");
            }

            _historija.Add(new PromjenaRecord(proizvodId, promjena, datum));
        }

        // Izračunava turnover (prosječnu promjenu po danu) za proizvod
        public double IzracunajTurnover(string proizvodId, int brojDana)
        {
            if (brojDana <= 0)
            {
                throw new ArgumentException("Broj dana mora biti veći od 0.");
            }

            var granicniDatum = DateTime.Now.AddDays(-brojDana);

            // Filtriraj promjene za dati proizvod i period
            var relevantnePromjene = _historija
                .Where(p => p.ProizvodId == proizvodId && p.Datum >= granicniDatum)
                .ToList();

            if (!relevantnePromjene.Any())
            {
                return 0;
            }

            // Ukupna apsolutna promjena
            double ukupnaPromjena = relevantnePromjene.Sum(p => Math.Abs(p.Promjena));

            // Turnover = ukupna promjena / broj dana
            return ukupnaPromjena / brojDana;
        }

        // Provjerava da li je proizvod "high turnover"
        public bool DaLiJeHighTurnover(string proizvodId, int brojDana, double prag)
        {
            double turnover = IzracunajTurnover(proizvodId, brojDana);
            return turnover >= prag;
        }

        // Pronalazi top N proizvoda po turnover-u
        public List<(string Id, double Turnover)> PronadjiTopProizvode(int brojDana, int top)
        {
            // Grupiši po proizvodu i izračunaj turnover za svaki
            var proizvodiSaTurnoverom = _historija
                .Select(p => p.ProizvodId)
                .Distinct()
                .Select(id => (Id: id, Turnover: IzracunajTurnover(id, brojDana)))
                .OrderByDescending(x => x.Turnover)
                .Take(top)
                .ToList();

            return proizvodiSaTurnoverom;
        }

        // Čisti historiju stariju od određenog broja dana
        public void OcistiHistoriju(int danaStar)
        {
            var granicniDatum = DateTime.Now.AddDays(-danaStar);
            _historija.RemoveAll(p => p.Datum < granicniDatum);
        }
    }
}
