using InventarApp.Models;
using InventarApp.Enums;

namespace InventarApp.Services
{
    public class InventarService
    {
        private List<Proizvod> _proizvodi;
        private ObavjestenjeService _obavjestenjeService;
        private HighTurnoverAnalyzer _turnoverAnalyzer;

        public InventarService(ObavjestenjeService obavjestenjeService, HighTurnoverAnalyzer turnoverAnalyzer)
        {
            _proizvodi = new List<Proizvod>();
            _obavjestenjeService = obavjestenjeService;
            _turnoverAnalyzer = turnoverAnalyzer;
        }

        // FUNKCIONALNOST 1: Dodavanje nove stavke u inventar
        public void DodajProizvod(string naziv, int kolicina, int minimalniPrag, string dobavljacId, KategorijaProizvoda kategorija)
        {
            try
            {
                var proizvod = new Proizvod(naziv, kolicina, minimalniPrag, dobavljacId, kategorija);
                _proizvodi.Add(proizvod);

                Console.WriteLine($"\n[OK] Proizvod '{naziv}' uspjesno dodan u inventar!");
                Console.WriteLine($"  ID: {proizvod.Id}");

                // Provjeri da li je potrebno kreirati obavještenje
                if (proizvod.JeIspodMinimalnogPraga())
                {
                    _obavjestenjeService.KreirajObavjestenje(proizvod);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška pri dodavanju proizvoda: {ex.Message}", ex);
            }
        }

        // FUNKCIONALNOST 2: Pregled i filtriranje inventara
        public List<Proizvod> PrikaziSveProizvode()
        {
            return new List<Proizvod>(_proizvodi);
        }

        public List<Proizvod> FiltrirajPoKategoriji(KategorijaProizvoda kategorija)
        {
            return _proizvodi.Where(p => p.Kategorija == kategorija).ToList();
        }

        public List<Proizvod> FiltrirajPoNazivu(string naziv)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                throw new ArgumentException("Naziv za pretragu ne može biti prazan.");

            return _proizvodi.Where(p => p.Naziv.Contains(naziv, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Proizvod> FiltrirajPoDobavljacu(string dobavljacId)
        {
            if (string.IsNullOrWhiteSpace(dobavljacId))
                throw new ArgumentException("ID dobavljača ne može biti prazan.");

            return _proizvodi.Where(p => p.DobavljacId == dobavljacId).ToList();
        }

        // Složeni algoritam - Sortiranje proizvoda po različitim kriterijima
        public List<Proizvod> SortirajProizvode(string kriterij, bool rastuci = true)
        {
            List<Proizvod> sortirani = new List<Proizvod>(_proizvodi);

            switch (kriterij.ToLower())
            {
                case "naziv":
                    sortirani = rastuci
                        ? sortirani.OrderBy(p => p.Naziv).ToList()
                        : sortirani.OrderByDescending(p => p.Naziv).ToList();
                    break;

                case "kolicina":
                    sortirani = rastuci
                        ? sortirani.OrderBy(p => p.Kolicina).ToList()
                        : sortirani.OrderByDescending(p => p.Kolicina).ToList();
                    break;

                case "kategorija":
                    sortirani = rastuci
                        ? sortirani.OrderBy(p => p.Kategorija).ToList()
                        : sortirani.OrderByDescending(p => p.Kategorija).ToList();
                    break;

                default:
                    throw new ArgumentException($"Nepoznat kriterij sortiranja: {kriterij}");
            }

            return sortirani;
        }

        // FUNKCIONALNOST 3: Upravljanje količinom i brisanje stavki
        public void AzurirajKolicinu(string proizvodId, int iznos, string razlog)
        {
            var proizvod = PronadjiProizvodPoId(proizvodId);

            if (proizvod == null)
                throw new InvalidOperationException($"Proizvod sa ID '{proizvodId}' nije pronađen.");

            try
            {
                int prethodnaKolicina = proizvod.Kolicina;
                proizvod.AzurirajKolicinu(iznos);

                // Registruj promjenu u HighTurnoverAnalyzer
                _turnoverAnalyzer.RegistrujPromjenu(proizvodId, iznos, DateTime.Now);

                Console.WriteLine($"\n[OK] Kolicina azurirana!");
                Console.WriteLine($"  Proizvod: {proizvod.Naziv}");
                Console.WriteLine($"  Prethodna kolicina: {prethodnaKolicina}");
                Console.WriteLine($"  Nova kolicina: {proizvod.Kolicina}");
                Console.WriteLine($"  Razlog: {razlog}");

                // Provjeri da li je potrebno kreirati obavještenje
                if (proizvod.JeIspodMinimalnogPraga())
                {
                    _obavjestenjeService.KreirajObavjestenje(proizvod);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška pri ažuriranju količine: {ex.Message}", ex);
            }
        }

        public void ObrisiProizvod(string proizvodId, string razlog)
        {
            var proizvod = PronadjiProizvodPoId(proizvodId);

            if (proizvod == null)
                throw new InvalidOperationException($"Proizvod sa ID '{proizvodId}' nije pronađen.");

            _proizvodi.Remove(proizvod);
            Console.WriteLine($"\n✓ Proizvod '{proizvod.Naziv}' je trajno obrisan iz inventara.");
            Console.WriteLine($"  Razlog: {razlog}");
        }

        // FUNKCIONALNOST 4: Obavještavanje o kritičnim zalihama
        public List<Proizvod> ProvjeriKriticneZalihe()
        {
            return _proizvodi.Where(p => p.JeIspodMinimalnogPraga()).ToList();
        }

        public Proizvod? PronadjiProizvodPoId(string id)
        {
            return _proizvodi.FirstOrDefault(p => p.Id == id);
        }

        public int UkupanBrojProizvoda()
        {
            return _proizvodi.Count;
        }

        public HighTurnoverAnalyzer GetTurnoverAnalyzer()
        {
            return _turnoverAnalyzer;
        }

        // Metoda koja zadovoljava preduslove
        
        public string AnalizirajStanjeInventara(bool ukljuciKategorije = true, bool ukljuciDobavljace = true, bool detaljnaAnaliza = true)
        {
            var izvjestaj = "═══════════════════════════════════════════════\n";
            izvjestaj += "         ANALIZA STANJA INVENTARA\n";
            izvjestaj += "═══════════════════════════════════════════════\n\n";

            // 1. Provjera da li je inventar prazan
            if (_proizvodi.Count == 0)
            {
                return izvjestaj + "[!] Inventar je prazan - nema proizvoda za analizu.\n";
            }

            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 100;

            // 2. OSNOVNA ANALIZA PROIZVODA (inline iz AnalizirajProizvode sa for petljom)
            int kriticniProizvodi = 0;
            int niskeZalihe = 0;
            int zdraveZalihe = 0;

            // Eksplicitna for petlja za analizu proizvoda
            for (int i = 0; i < _proizvodi.Count; i++)
            {
                var proizvod = _proizvodi[i];
                if (proizvod.Kolicina == 0)
                {
                    kriticniProizvodi++;
                    ocjena -= 10;
                }
                else if (proizvod.JeIspodMinimalnogPraga())
                {
                    niskeZalihe++;
                    ocjena -= 5;
                }
                else
                {
                    zdraveZalihe++;
                }
            }

            izvjestaj += $"Ukupno proizvoda: {_proizvodi.Count}\n";
            izvjestaj += $"Zdravih zaliha: {zdraveZalihe}\n";
            izvjestaj += $"Niskih zaliha: {niskeZalihe}\n";
            izvjestaj += $"Kritičnih zaliha (0): {kriticniProizvodi}\n\n";

            // 3. Analiza kritičnih proizvoda
            if (kriticniProizvodi > 0)
            {
                var (kriticnaOcjena, kriticnaUpozorenja, kriticnePreporuke) = AnalizirajKriticneProizvode(kriticniProizvodi, _proizvodi.Count);
                ocjena += kriticnaOcjena;
                upozorenja.AddRange(kriticnaUpozorenja);
                preporuke.AddRange(kriticnePreporuke);
            }

            // 4. Analiza niskih zaliha
            if (niskeZalihe > 0)
            {
                var (niskaOcjena, niskaUpozorenja, niskePreporuke) = AnalizirajNiskeZalihe(niskeZalihe, _proizvodi.Count);
                ocjena += niskaOcjena;
                upozorenja.AddRange(niskaUpozorenja);
                preporuke.AddRange(niskePreporuke);
            }

            // 5. Analiza po kategorijama
            if (ukljuciKategorije)
            {
                izvjestaj += GenerirajAnalizuPoKategorijama(preporuke);
            }

            // 6. Analiza po dobavljačima
            if (ukljuciDobavljace)
            {
                izvjestaj += GenerirajAnalizuPoDobavljacima();
            }

            // 7. Detaljna analiza
            if (detaljnaAnaliza)
            {
                var (detaljniIzvjestaj, detaljnaOcjena, detaljnaUpozorenja, detaljnePreporuke) =
                    GenerirajDetaljnuAnalizu();
                izvjestaj += detaljniIzvjestaj;
                ocjena += detaljnaOcjena;
                upozorenja.AddRange(detaljnaUpozorenja);
                preporuke.AddRange(detaljnePreporuke);
            }

            // 8. Provjera omjera zdravih zaliha
            if (zdraveZalihe < _proizvodi.Count / 2)
            {
                upozorenja.Add("Manje od 50% proizvoda ima zdrave zalihe!");
                ocjena -= 10;
            }

            // 9. Konačni izvještaj
            izvjestaj += GenerirajKonacniIzvjestaj(ocjena, upozorenja, preporuke);

            return izvjestaj;
        }

        private (int kriticni, int niske, int zdrave, int ocjena) AnalizirajProizvode()
        {
            int kriticni = 0, niske = 0, zdrave = 0, ocjena = 100;

            foreach (var proizvod in _proizvodi)
            {
                if (proizvod.Kolicina == 0)
                {
                    kriticni++;
                    ocjena -= 10;
                }
                else if (proizvod.JeIspodMinimalnogPraga())
                {
                    niske++;
                    ocjena -= 5;
                }
                else
                {
                    zdrave++;
                }
            }

            return (kriticni, niske, zdrave, ocjena);
        }

        private (int ocjena, List<string> upozorenja, List<string> preporuke) AnalizirajKriticneProizvode(int kriticniProizvodi, int ukupno)
        {
            var upozorenja = new List<string> { $"{kriticniProizvodi} proizvod(a) je bez zaliha!" };
            var preporuke = new List<string> { "-> Hitno naručite proizvode bez zaliha" };
            int ocjena = 0;

            double postotak = (kriticniProizvodi * 100.0) / ukupno;

            if (postotak > 50)
            {
                upozorenja.Add("KRITIČNO: Više od 50% proizvoda je bez zaliha!");
                ocjena = -30;
            }
            else if (postotak > 25)
            {
                upozorenja.Add("UPOZORENJE: Više od 25% proizvoda je bez zaliha!");
                ocjena = -15;
            }

            return (ocjena, upozorenja, preporuke);
        }

        private (int ocjena, List<string> upozorenja, List<string> preporuke) AnalizirajNiskeZalihe(int niskeZalihe, int ukupno)
        {
            var upozorenja = new List<string> { $"{niskeZalihe} proizvod(a) ima niske zalihe" };
            var preporuke = new List<string> { "-> Planirajte narudžbu za proizvode sa niskim zalihama" };
            int ocjena = 0;

            double postotak = (niskeZalihe * 100.0) / ukupno;

            if (postotak > 40)
            {
                upozorenja.Add("Više od 40% proizvoda ima niske zalihe!");
                ocjena = -20;
            }

            return (ocjena, upozorenja, preporuke);
        }

        private string GenerirajAnalizuPoKategorijama(List<string> preporuke)
        {
            var izvjestaj = "\n--- Analiza po kategorijama ---\n";
            var kategorijeGrupe = _proizvodi.GroupBy(p => p.Kategorija);

            foreach (var grupa in kategorijeGrupe)
            {
                int ukupno = grupa.Count();
                int kriticnih = grupa.Count(p => p.Kolicina == 0);

                izvjestaj += $"{grupa.Key}: {ukupno} proizvod(a)";

                if (kriticnih > 0)
                {
                    izvjestaj += $" ({kriticnih} kritičnih)";
                    if ((kriticnih * 100.0) / ukupno > 50)
                    {
                        izvjestaj += " KRITIČNO";
                        preporuke.Add($"-> Fokusirajte se na kategoriju {grupa.Key}");
                    }
                }
                izvjestaj += "\n";
            }

            return izvjestaj;
        }

        private string GenerirajAnalizuPoDobavljacima()
        {
            var izvjestaj = "\n--- Analiza po dobavljačima ---\n";
            var dobavljaciGrupe = _proizvodi.GroupBy(p => p.DobavljacId);

            foreach (var grupa in dobavljaciGrupe)
            {
                int ukupno = grupa.Count();
                int kriticnih = grupa.Count(p => p.Kolicina == 0);

                izvjestaj += $"Dobavljač {grupa.Key}: {ukupno} proizvod(a)";
                if (kriticnih > 0)
                {
                    izvjestaj += $" ({kriticnih} kritičnih)";
                }
                izvjestaj += "\n";
            }

            return izvjestaj;
        }

        private (string izvjestaj, int ocjena, List<string> upozorenja, List<string> preporuke) GenerirajDetaljnuAnalizu()
        {
            var izvjestaj = "\n--- Detaljne metrike ---\n";
            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            double prosjecnaKolicina = _proizvodi.Average(p => p.Kolicina);
            izvjestaj += $"Prosječna količina: {prosjecnaKolicina:F2}\n";

            if (prosjecnaKolicina < 10)
            {
                upozorenja.Add("Prosječna količina je veoma niska!");
                preporuke.Add("-> Razmotrite povećanje standardnih količina narudžbi");
                ocjena = -15;
            }
            else if (prosjecnaKolicina < 20)
            {
                upozorenja.Add("Prosječna količina je ispod preporučenog nivoa");
                ocjena = -5;
            }

            var najniziProizvod = _proizvodi.OrderBy(p => p.Kolicina).FirstOrDefault();
            if (najniziProizvod != null)
            {
                izvjestaj += $"Najniža zaliha: {najniziProizvod.Naziv} ({najniziProizvod.Kolicina})\n";
            }

            return (izvjestaj, ocjena, upozorenja, preporuke);
        }

        private string GenerirajKonacniIzvjestaj(int ocjena, List<string> upozorenja, List<string> preporuke)
        {
            var izvjestaj = "\n═══════════════════════════════════════════════\n";

            string zdravljeOcjena = OdrediZdravljeOcjenu(ocjena);
            izvjestaj += $"ZDRAVLJE INVENTARA: {zdravljeOcjena} ({ocjena}/100)\n";
            izvjestaj += "═══════════════════════════════════════════════\n";

            if (upozorenja.Any())
            {
                izvjestaj += "\nUPOZORENJA:\n";
                izvjestaj += string.Join("\n", upozorenja.Select(u => $"  {u}"));
                izvjestaj += "\n";
            }

            if (preporuke.Any())
            {
                izvjestaj += "\nPREPORUKE:\n";
                izvjestaj += string.Join("\n", preporuke.Select(p => $"  {p}"));
                izvjestaj += "\n";
            }

            return izvjestaj;
        }

        private string OdrediZdravljeOcjenu(int ocjena) => ocjena switch
        {
            >= 90 => "ODLIČNO",
            >= 70 => "DOBRO",
            >= 50 => "ZADOVOLJAVAJUĆE",
            >= 30 => "LOŠE",
            _ => "KRITIČNO"
        };
    }
}
