using InventarApp.Interfaces;
using InventarApp.Enums;

namespace InventarApp.Models
{
    public class Proizvod : IProizvod
    {
        public string Id { get; set; }
        public string Naziv { get; set; }
        public int Kolicina { get; set; }
        public int MinimalniPrag { get; set; }
        public string DobavljacId { get; set; }
        public KategorijaProizvoda Kategorija { get; set; }

        public bool NijeZaliha => Kolicina > 0;

        private static int _brojac = 0;

        public Proizvod(string naziv, int kolicina, int minimalniPrag, string dobavljacId, KategorijaProizvoda kategorija)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                throw new ArgumentException("Naziv proizvoda ne može biti prazan.");

            if (kolicina < 0)
                throw new ArgumentException("Količina ne može biti negativna.");

            if (minimalniPrag < 0)
                throw new ArgumentException("Minimalni prag ne može biti negativan.");

            if (string.IsNullOrWhiteSpace(dobavljacId))
                throw new ArgumentException("ID dobavljača ne može biti prazan.");

            Naziv = naziv;
            Kolicina = kolicina;
            MinimalniPrag = minimalniPrag;
            DobavljacId = dobavljacId;
            Kategorija = kategorija;
            Id = GenerisiId();
        }

        private string GenerisiId()
        {
            _brojac++;
            return $"Proizvod-{_brojac}";
        }

        public void AzurirajKolicinu(int iznos)
        {
            int novaKolicina = Kolicina + iznos;

            if (novaKolicina < 0)
                throw new InvalidOperationException($"Nemoguće smanjiti količinu. Trenutna količina: {Kolicina}, Tražena promjena: {iznos}");

            Kolicina = novaKolicina;
        }

        public bool JeIspodMinimalnogPraga()
        {
            return Kolicina < MinimalniPrag;
        }

        // Složeni algoritam - Izračunavanje prioriteta nabavke sa McCabe Complexity >= 8
        // Algoritam analizira stanje zaliha i određuje prioritet za porudžbinu
        // Složeni algoritam - Izračunavanje prioriteta nabavke sa McCabe Complexity = 8
        // Složeni algoritam - Izračunavanje prioriteta nabavke sa McCabe Complexity = 8
        public string IzracunajPrioritetNabavke()
        {
            int procenatZaliha = MinimalniPrag > 0 ? (Kolicina * 100) / MinimalniPrag : 100;
            bool kriticnaKategorija = JeKriticnaKategorija();

            string prioritet;
            string dodatniInfo = "";

            // Analiza nivoa zaliha
            if (Kolicina == 0)
            {
                prioritet = kriticnaKategorija
                    ? "KRITIČNO - HITNA NABAVKA (0 zaliha, kritična kategorija)"
                    : "VRLO VISOK - Hitna nabavka potrebna (0 zaliha)";
            }
            else if (procenatZaliha < 50)
            {
                prioritet = OdrediPrioritetNiskeZalihe(kriticnaKategorija);
            }
            else if (procenatZaliha < 100)
            {
                prioritet = kriticnaKategorija
                    ? "SREDNJI - Pratiti stanje, nabavka u narednom mjesecu"
                    : "NIZAK-SREDNJI - Nema hitnosti";
            }
            else
            {
                prioritet = OdrediPrioritetVisokihZaliha(procenatZaliha);
            }

            // Dodatna provjera za veoma niske količine
            if (Kolicina > 0 && Kolicina <= MinimalniPrag / 4)
            {
                dodatniInfo = " [UPOZORENJE: Kritično niska količina]";
            }

            return $"{prioritet} (Procenat: {procenatZaliha}%){dodatniInfo}";
        }

        private bool JeKriticnaKategorija()
        {
            return Kategorija == KategorijaProizvoda.IT_UREDJAJI ||
                   Kategorija == KategorijaProizvoda.ALATI;
        }

        private string OdrediPrioritetNiskeZalihe(bool kriticnaKategorija)
        {
            if (kriticnaKategorija && Kolicina < 5)
            {
                return "VISOK - Planirati nabavku u naredna 3 dana";
            }

            if (kriticnaKategorija)
            {
                return "VISOK - Planirati nabavku u narednih 7 dana";
            }

            if (Kolicina < 3)
            {
                return "SREDNJI-VISOK - Nabavka u narednih 10 dana";
            }

            return "SREDNJI - Nabavka u narednih 14 dana";
        }

        private string OdrediPrioritetVisokihZaliha(int procenatZaliha) => procenatZaliha switch
        {
            > 200 => "NIZAK - Dovoljno zaliha, nema potrebe za nabavkom",
            > 150 => "NIZAK - Zadovoljavajuće stanje",
            _ => "NIZAK-SREDNJI - Dobro stanje zaliha"
        };

        public override string ToString()
        {
            return $"ID: {Id}, Naziv: {Naziv}, Količina: {Kolicina}, Kategorija: {Kategorija}, Dobavljač ID: {DobavljacId}";
        }
    }
}
