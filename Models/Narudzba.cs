using InventarApp.Interfaces;
using InventarApp.Enums;

namespace InventarApp.Models
{
    public class Narudzba : INarudzba
    {
        public string NarudzbaId { get; set; }
        public string ProizvodId { get; set; }
        public string DobavljacId { get; set; }
        public int Kolicina { get; set; }
        public DateTime DatumNarudzbe { get; set; }
        public StatusNarudzbe Status { get; set; }

        private static int _brojac = 0;
        private const double CIJENA_PO_JEDINICI = 10.0; // Bazna cijena
        private const double PDV_STOPA = 0.17; // 17% PDV

        public Narudzba(string proizvodId, string dobavljacId, int kolicina)
        {
            if (string.IsNullOrWhiteSpace(proizvodId))
                throw new ArgumentException("ID proizvoda ne može biti prazan.");

            if (string.IsNullOrWhiteSpace(dobavljacId))
                throw new ArgumentException("ID dobavljača ne može biti prazan.");

            if (kolicina <= 0)
                throw new ArgumentException("Količina mora biti veća od nule.");

            if (kolicina > int.MaxValue)
                throw new ArgumentException("Količina prelazi maksimalnu dozvoljenu vrijednost.");

            ProizvodId = proizvodId;
            DobavljacId = dobavljacId;
            Kolicina = kolicina;
            DatumNarudzbe = DateTime.Now;
            Status = StatusNarudzbe.NA_CEKANJU;
            NarudzbaId = GenerisiId();
        }

        private string GenerisiId()
        {
            _brojac++;
            return $"Narudzba-{_brojac}";
        }

        // Složeni algoritam - Izračunavanje ukupne cijene sa popustima i PDV-om
        public double IzracunajUkupno()
        {
            double osnovnaCijena = CIJENA_PO_JEDINICI * Kolicina;
            double popust = IzracunajPopust(Kolicina);
            double cijenasPpustom = osnovnaCijena * (1 - popust);
            double pdv = cijenasPpustom * PDV_STOPA;
            double ukupno = cijenasPpustom + pdv;

            return Math.Round(ukupno, 2);
        }

        // Algoritam za izračunavanje popusta prema količini (progresivni popust)
        private double IzracunajPopust(int kolicina)
        {
            if (kolicina >= 100)
                return 0.15; // 15% popust za narudžbe 100+ stavki
            else if (kolicina >= 50)
                return 0.10; // 10% popust za narudžbe 50-99 stavki
            else if (kolicina >= 20)
                return 0.05; // 5% popust za narudžbe 20-49 stavki
            else
                return 0.0; // Bez popusta za manje narudžbe
        }

        public void OznacKaoIsporuceno()
        {
            if (Status == StatusNarudzbe.ISPORUCENO)
                throw new InvalidOperationException("Narudžba je već označena kao isporučena.");

            if (Status == StatusNarudzbe.OTKAZANO)
                throw new InvalidOperationException("Otkazana narudžba ne može biti označena kao isporučena.");

            Status = StatusNarudzbe.ISPORUCENO;
        }

        public void OtkaziNarudzbu()
        {
            if (Status == StatusNarudzbe.ISPORUCENO)
                throw new InvalidOperationException("Isporučena narudžba ne može biti otkazana.");

            Status = StatusNarudzbe.OTKAZANO;
        }

        // Složeni algoritam - Procjena vremena isporuke sa McCabe Complexity >= 8
        // Uzima u obzir količinu, dan u sedmici, status i udaljenost dobavljača
        // Složeni algoritam - Procjena vremena isporuke sa McCabe Complexity = 8
        // Složeni algoritam - Procjena vremena isporuke sa McCabe Complexity = 8
        // Složeni algoritam - Procjena vremena isporuke sa McCabe Complexity = 8
        // Složeni algoritam - Procjena vremena isporuke sa McCabe Complexity = 8
        // Složeni algoritam - Procjena vremena isporuke sa McCabe Complexity = 8
        public string ProcijeniVrijemeIsporuke(bool hitnaIsporuka = false, bool domacaDobavljac = true)
        {
            // 1. Provjera statusa narudžbe (rani izlaz)
            if (Status == StatusNarudzbe.OTKAZANO)
                return "Narudžba otkazana - nema procjene isporuke.";

            if (Status == StatusNarudzbe.ISPORUCENO)
                return "Narudžba već isporučena.";

            // 2. Osnovna procjena prema količini
            var (daniIsporuke, napomena) = OdrediOsnovnuProcjenu();

            // 3. Korekcija za hitnost
            if (hitnaIsporuka)
            {
                (daniIsporuke, string hitnostNapomena) = PrimijeniHitnuIsporuku(daniIsporuke);
                napomena += hitnostNapomena;
            }

            // 4. Korekcija za lokaciju dobavljača
            if (!domacaDobavljac)
            {
                daniIsporuke += 5;
                napomena += "Strani dobavljač - dodatno vrijeme za carinu i transport. ";

                if (Kolicina >= 50)
                {
                    daniIsporuke += 2;
                    napomena += "Veća količina zahtijeva dodatnu logistiku. ";
                }
            }

            // 5. Korekcija za vikend narudžbe
            if (JeVikendIliPetak())
            {
                daniIsporuke += 2;
                napomena += "Narudžba vikendom/petkom - vikend uračunat. ";
            }

            // 6. Provjera za veoma duge isporuke
            if (daniIsporuke > 14)
            {
                napomena += "Upozorenje: Dugo vrijeme isporuke. ";
            }

            // 7. Izračunavanje procijenjenog datuma sa pomakom za vikend
            DateTime procijenjeniDatum = IzracunajDatumIsporuke(daniIsporuke, ref napomena);

            // 8. Određivanje kategorije brzine
            string kategorizacija = OdrediKategorijuBrzine(daniIsporuke);

            return $"Procjena: {daniIsporuke} radnih dana ({kategorizacija})\n" +
                   $"Očekivani datum: {procijenjeniDatum:dd.MM.yyyy}\n" +
                   $"Detalji: {napomena}";
        }

        private (int dani, string napomena) OdrediOsnovnuProcjenu() => Kolicina switch
        {
            >= 100 => (7, "Velika narudžba. "),
            >= 50 => (5, "Srednja narudžba. "),
            >= 20 => (3, "Standardna narudžba. "),
            _ => (2, "Mala narudžba. ")
        };

        private (int dani, string napomena) PrimijeniHitnuIsporuku(int daniIsporuke) =>
            daniIsporuke > 3
                ? ((int)(daniIsporuke * 0.6), "Hitna isporuka omogućava bržu dostavu. ")
                : (Math.Max(1, daniIsporuke - 1), "Hitna isporuka - ekspresna dostava. ");

        private bool JeVikendIliPetak()
        {
            DayOfWeek dan = DatumNarudzbe.DayOfWeek;
            return dan == DayOfWeek.Friday || dan == DayOfWeek.Saturday || dan == DayOfWeek.Sunday;
        }

        private DateTime IzracunajDatumIsporuke(int daniIsporuke, ref string napomena)
        {
            DateTime datum = DatumNarudzbe.AddDays(daniIsporuke);

            if (datum.DayOfWeek == DayOfWeek.Saturday)
            {
                napomena += "Isporuka pomjerena sa subote na ponedjeljak. ";
                return datum.AddDays(2);
            }

            if (datum.DayOfWeek == DayOfWeek.Sunday)
            {
                napomena += "Isporuka pomjerena sa nedjelje na ponedjeljak. ";
                return datum.AddDays(1);
            }

            return datum;
        }

        private string OdrediKategorijuBrzine(int daniIsporuke) => daniIsporuke switch
        {
            <= 2 => "EKSPRESNA",
            <= 5 => "BRZA",
            <= 10 => "STANDARDNA",
            _ => "SPORA"
        };

        public override string ToString()
        {
            return $"Narudžba ID: {NarudzbaId}, Proizvod: {ProizvodId}, Količina: {Kolicina}, " +
                   $"Status: {Status}, Datum: {DatumNarudzbe:dd.MM.yyyy}, Ukupno: {IzracunajUkupno():F2} KM";
        }
    }
}
