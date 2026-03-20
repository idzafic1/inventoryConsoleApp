using InventarApp.Models;

namespace InventarApp.Services
{
    public class DobavljacService
    {
        private List<Dobavljac> _dobavljaci;

        public DobavljacService()
        {
            _dobavljaci = new List<Dobavljac>();
        }

        public void DodajDobavljaca(string naziv, string kontakt)
        {
            try
            {
                var dobavljac = new Dobavljac(naziv, kontakt);
                _dobavljaci.Add(dobavljac);

                Console.WriteLine($"\n✓ Dobavljač '{naziv}' uspješno dodan!");
                Console.WriteLine($"  ID: {dobavljac.Id}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška pri dodavanju dobavljača: {ex.Message}", ex);
            }
        }

        public List<Dobavljac> PrikaziSveDobavljace()
        {
            return new List<Dobavljac>(_dobavljaci);
        }

        public Dobavljac? PronadjiDobavljacaPoId(string id)
        {
            return _dobavljaci.FirstOrDefault(d => d.Id == id);
        }

        public Dobavljac? PronadjiDobavljacaPoNazivu(string naziv)
        {
            return _dobavljaci.FirstOrDefault(d => d.Naziv.Equals(naziv, StringComparison.OrdinalIgnoreCase));
        }

        public void PrikaziInfoDobavljaca(string id)
        {
            var dobavljac = PronadjiDobavljacaPoId(id);

            if (dobavljac == null)
            {
                throw new InvalidOperationException($"Dobavljač sa ID '{id}' nije pronađen.");
            }

            Console.WriteLine("\n" + dobavljac.GetDobavljacInfo());
        }
        
        public string AnalizirajDobavljaca(string id, bool provjeriDuplikate = true, bool provjeriKvalitet = true)
        {
            var dobavljac = PronadjiDobavljacaPoId(id);
            if (dobavljac == null)
            {
                return "GREŠKA: Dobavljač nije pronađen.";
            }

            var rezultat = $"Analiza dobavljača: {dobavljac.Naziv}\n";
            var upozorenja = new List<string>();
            int ocjena = 0;

            var (nazivOcjena, nazivUpozorenje) = AnalizirajNaziv(dobavljac.Naziv);
            if (!string.IsNullOrEmpty(nazivUpozorenje))
                upozorenja.Add(nazivUpozorenje);
            ocjena += nazivOcjena;

            var (kontaktOcjena, kontaktRezultat, kontaktUpozorenje) = AnalizirajKontakt(dobavljac.Kontakt);
            rezultat += kontaktRezultat;
            if (!string.IsNullOrEmpty(kontaktUpozorenje))
                upozorenja.Add(kontaktUpozorenje);
            ocjena += kontaktOcjena;

            int duplikatBodovi = 0;
            if (provjeriDuplikate)
            {
                duplikatBodovi = 10;
                for (int i = 0; i < _dobavljaci.Count; i++)
                {
                    var d = _dobavljaci[i];
                    if (d.Id != id && d.Naziv.Equals(dobavljac.Naziv, StringComparison.OrdinalIgnoreCase))
                    {
                        upozorenja.Add($"- Pronađen duplikat naziva (ID: {d.Id})");
                        duplikatBodovi = -15;
                        break;
                    }
                }
                ocjena += duplikatBodovi;
            }

            if (provjeriKvalitet)
            {
                var (kvalitetBodovi, kvalitetRezultat, kvalitetUpozorenje) = AnalizirajKvalitetDetaljno(dobavljac);
                rezultat += kvalitetRezultat;
                if (!string.IsNullOrEmpty(kvalitetUpozorenje))
                    upozorenja.Add(kvalitetUpozorenje);
                ocjena += kvalitetBodovi;
            }

            string konacnaOcjena = OdrediKonacnuOcjenu(ocjena);

            rezultat += $"Ukupna ocjena: {ocjena} bodova - {konacnaOcjena}\n";

            if (upozorenja.Count > 0)
            {
                rezultat += "\nUpozorenja:\n" + string.Join("\n", upozorenja) + "\n";
            }

            return rezultat;
        }

        private (int ocjena, string upozorenje) AnalizirajNaziv(string naziv)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                return (-20, "- Naziv je prazan");

            if (naziv.Length < 3)
                return (-10, "- Naziv je prekratak (manje od 3 znaka)");

            if (naziv.Length > 50)
                return (-5, "- Naziv je predugačak (više od 50 znakova)");

            return (20, "");
        }

        private (int ocjena, string rezultat, string upozorenje) AnalizirajKontakt(string kontakt)
        {
            if (string.IsNullOrWhiteSpace(kontakt))
                return (-30, "", "- Kontakt informacija nedostaje");

            if (kontakt.Contains("@"))
                return (30, "Tip kontakta: Email\n", "");

            if (kontakt.Any(char.IsDigit))
                return (30, "Tip kontakta: Telefon\n", "");

            return (15, "", "- Neprepoznat format kontakta");
        }

        private (int ocjena, string rezultat, string upozorenje) AnalizirajKvalitetDetaljno(Dobavljac dobavljac)
        {
            try
            {
                var kvalitetInfo = dobavljac.AnalizirajKvalitetKontakta();
                if (kvalitetInfo.Contains("ODLIČAN") || kvalitetInfo.Contains("DOBAR"))
                {
                    return (15, "Kvalitet kontakta: Visok\n", "");
                }
                else if (kvalitetInfo.Contains("ZADOVOLJAVAJUĆI"))
                {
                    return (5, "Kvalitet kontakta: Srednji\n", "");
                }
                else
                {
                    return (-10, "", "- Nizak kvalitet kontakt informacija");
                }
            }
            catch
            {
                return (0, "", "- Greška pri analizi kvaliteta kontakta");
            }
        }

        private string OdrediKonacnuOcjenu(int ocjena) => ocjena switch
        {
            >= 60 => "ODLIČAN",
            >= 40 => "DOBAR",
            >= 20 => "ZADOVOLJAVAJUĆI",
            >= 0 => "LOŠ",
            _ => "NEPRIHVATLJIV"
        };
    }
}
