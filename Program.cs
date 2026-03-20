using InventarApp.Services;
using InventarApp.Models;
using InventarApp.Enums;

namespace InventarApp
{
    class Program
    {
        private static ObavjestenjeService obavjestenjeService = new ObavjestenjeService();
        private static HighTurnoverAnalyzer turnoverAnalyzer = new HighTurnoverAnalyzer();
        private static CurrencyService currencyService = new CurrencyService();
        private static InventarService inventarService = new InventarService(obavjestenjeService, turnoverAnalyzer);
        private static DobavljacService dobavljacService = new DobavljacService();
        private static NarudzbaService narudzbaService = new NarudzbaService(turnoverAnalyzer);

        static void Main(string[] args)
        {
            InicijalizujTestnePodatke();

            bool radi = true;

            while (radi)
            {
                try
                {
                    PrikaziGlavniMeni();
                    string izbor = Console.ReadLine() ?? "";

                    switch (izbor)
                    {
                        case "1":
                            UpravljanjeProizvodima();
                            break;
                        case "2":
                            UpravljanjeDobavljacima();
                            break;
                        case "3":
                            UpravljanjeNarudzbama();
                            break;
                        case "4":
                            UpravljanjeObavjestenjima();
                            break;
                        case "0":
                            radi = false;
                            Console.WriteLine("\nHvala što ste koristili aplikaciju. Doviđenja!");
                            break;
                        default:
                            UIHelper.PrikaziGresku("Neispravan izbor. Pokušajte ponovo.");
                            UIHelper.Pauza();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.PrikaziGresku($"Neočekivana greška: {ex.Message}");
                    UIHelper.Pauza();
                }
            }
        }

        static void PrikaziGlavniMeni()
        {
            UIHelper.PrikaziNaslov("SISTEM ZA VOĐENJE INVENTARA");

            Console.WriteLine("  [1] Upravljanje proizvodima");
            Console.WriteLine("  [2] Upravljanje dobavljačima");
            Console.WriteLine("  [3] Upravljanje narudžbama");
            Console.WriteLine("  [4] Obavještenja o zalihama");
            Console.WriteLine("  [0] Izlaz");
            Console.WriteLine();
            Console.Write("Izaberite opciju: ");
        }

        // FUNKCIONALNOST 1: Dodavanje i upravljanje proizvodima
        static void UpravljanjeProizvodima()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    UIHelper.PrikaziMeni("UPRAVLJANJE PROIZVODIMA", new[]
                    {
                        "Dodaj novi proizvod",
                        "Prikaži sve proizvode",
                        "Filtriraj proizvode",
                        "Sortiraj proizvode",
                        "Ažuriraj količinu",
                        "Obriši proizvod",
                        "Provjeri kritične zalihe",
                        "Analiziraj stanje inventara",
                        "Prikaži cijene u drugim valutama",
                        "Analiza proizvoda sa visokim prometom"
                    });

                    string izbor = Console.ReadLine() ?? "";

                    switch (izbor)
                    {
                        case "1":
                            DodajProizvod();
                            break;
                        case "2":
                            PrikaziProizvode();
                            break;
                        case "3":
                            FiltrirajProizvode();
                            break;
                        case "4":
                            SortirajProizvode();
                            break;
                        case "5":
                            AzurirajKolicinu();
                            break;
                        case "6":
                            ObrisiProizvod();
                            break;
                        case "7":
                            ProvjeriKriticneZalihe();
                            break;
                        case "8":
                            AnalizirajStanjeInventara();
                            break;
                        case "9":
                            PrikaziCijeneUValutama();
                            break;
                        case "10":
                            AnalizirajVisokPromet();
                            break;
                        case "0":
                            nazad = true;
                            break;
                        default:
                            UIHelper.PrikaziGresku("Neispravan izbor.");
                            UIHelper.Pauza();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.PrikaziGresku(ex.Message);
                    UIHelper.Pauza();
                }
            }
        }

        static void DodajProizvod()
        {
            UIHelper.PrikaziNaslov("DODAVANJE NOVOG PROIZVODA");

            try
            {
                // Prikaži dostupne dobavljače
                var dobavljaci = dobavljacService.PrikaziSveDobavljace();
                if (dobavljaci.Count == 0)
                {
                    UIHelper.PrikaziGresku("Nema dostupnih dobavljača. Molimo prvo dodajte dobavljača.");
                    UIHelper.Pauza();
                    return;
                }

                Console.WriteLine("\nDostupni dobavljači:");
                UIHelper.PrikaziDobavljace(dobavljaci);
                Console.WriteLine();

                string naziv = UIHelper.UcitajString("Naziv proizvoda");
                int kolicina = UIHelper.UcitajInt("Početna količina");
                int minPrag = UIHelper.UcitajInt("Minimalni prag");
                int redniBrojDobavljaca = UIHelper.UcitajInt($"Redni broj dobavljača (1-{dobavljaci.Count})");

                // Provjeri da li je redni broj validan i uzmi dobavljača
                if (redniBrojDobavljaca < 1 || redniBrojDobavljaca > dobavljaci.Count)
                {
                    throw new InvalidOperationException($"Neispravan redni broj. Molimo unesite broj između 1 i {dobavljaci.Count}.");
                }

                var dobavljac = dobavljaci[redniBrojDobavljaca - 1];
                string dobavljacId = dobavljac.Id;

                Console.WriteLine("\nKategorije:");
                Console.WriteLine("  [1] ALATI");
                Console.WriteLine("  [2] OPREMA");
                Console.WriteLine("  [3] KANCELARIJSKI_MATERIJALI");
                Console.WriteLine("  [4] IT_UREDJAJI");

                int kategorijaIzbor = UIHelper.UcitajInt("Izaberite kategoriju (1-4)");

                KategorijaProizvoda kategorija = kategorijaIzbor switch
                {
                    1 => KategorijaProizvoda.ALATI,
                    2 => KategorijaProizvoda.OPREMA,
                    3 => KategorijaProizvoda.KANCELARIJSKI_MATERIJALI,
                    4 => KategorijaProizvoda.IT_UREDJAJI,
                    _ => throw new ArgumentException("Neispravan izbor kategorije.")
                };

                inventarService.DodajProizvod(naziv, kolicina, minPrag, dobavljacId, kategorija);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // FUNKCIONALNOST 2: Pregled i filtriranje
        static void PrikaziProizvode()
        {
            UIHelper.PrikaziNaslov("SVI PROIZVODI");

            var proizvodi = inventarService.PrikaziSveProizvode();
            UIHelper.PrikaziProizvode(proizvodi);

            UIHelper.Pauza();
        }

        static void FiltrirajProizvode()
        {
            UIHelper.PrikaziNaslov("FILTRIRANJE PROIZVODA");

            Console.WriteLine("  [1] Filtriraj po kategoriji");
            Console.WriteLine("  [2] Filtriraj po nazivu");
            Console.WriteLine("  [3] Filtriraj po dobavljaču");
            Console.WriteLine();

            string izbor = UIHelper.UcitajString("Izaberite opciju");

            try
            {
                List<Proizvod> rezultat = new List<Proizvod>();

                if (izbor == "1")
                {
                    Console.WriteLine("\nKategorije:");
                    Console.WriteLine("  [1] ALATI");
                    Console.WriteLine("  [2] OPREMA");
                    Console.WriteLine("  [3] KANCELARIJSKI_MATERIJALI");
                    Console.WriteLine("  [4] IT_UREDJAJI");

                    int kategorijaIzbor = UIHelper.UcitajInt("Izaberite kategoriju (1-4)");

                    KategorijaProizvoda kategorija = kategorijaIzbor switch
                    {
                        1 => KategorijaProizvoda.ALATI,
                        2 => KategorijaProizvoda.OPREMA,
                        3 => KategorijaProizvoda.KANCELARIJSKI_MATERIJALI,
                        4 => KategorijaProizvoda.IT_UREDJAJI,
                        _ => throw new ArgumentException("Neispravan izbor kategorije.")
                    };

                    rezultat = inventarService.FiltrirajPoKategoriji(kategorija);
                }
                else if (izbor == "2")
                {
                    string naziv = UIHelper.UcitajString("Unesite naziv (ili dio naziva)");
                    rezultat = inventarService.FiltrirajPoNazivu(naziv);
                }
                else if (izbor == "3")
                {
                    string dobavljacId = UIHelper.UcitajString("Unesite ID dobavljača");
                    rezultat = inventarService.FiltrirajPoDobavljacu(dobavljacId);
                }
                else
                {
                    throw new ArgumentException("Neispravan izbor.");
                }

                Console.WriteLine("\nRezultati pretrage:");
                UIHelper.PrikaziProizvode(rezultat);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void SortirajProizvode()
        {
            UIHelper.PrikaziNaslov("SORTIRANJE PROIZVODA");

            Console.WriteLine("  [1] Sortiraj po nazivu");
            Console.WriteLine("  [2] Sortiraj po količini");
            Console.WriteLine("  [3] Sortiraj po kategoriji");
            Console.WriteLine();

            string izbor = UIHelper.UcitajString("Izaberite kriterij");
            string smijer = UIHelper.UcitajString("Rastuće (r) ili opadajuće (o)?").ToLower();

            try
            {
                string kriterij = izbor switch
                {
                    "1" => "naziv",
                    "2" => "kolicina",
                    "3" => "kategorija",
                    _ => throw new ArgumentException("Neispravan izbor.")
                };

                bool rastuci = smijer == "r";

                var sortirani = inventarService.SortirajProizvode(kriterij, rastuci);

                Console.WriteLine("\nSortirani proizvodi:");
                UIHelper.PrikaziProizvode(sortirani);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // FUNKCIONALNOST 3: Upravljanje količinom i brisanje
        static void AzurirajKolicinu()
        {
            UIHelper.PrikaziNaslov("AŽURIRANJE KOLIČINE");

            try
            {
                string proizvodId = UIHelper.UcitajString("ID proizvoda");
                int iznos = UIHelper.UcitajInt("Iznos promjene (pozitivan za zaduženje, negativan za razduženje)");
                string razlog = UIHelper.UcitajString("Razlog promjene");

                inventarService.AzurirajKolicinu(proizvodId, iznos, razlog);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void ObrisiProizvod()
        {
            UIHelper.PrikaziNaslov("BRISANJE PROIZVODA");

            try
            {
                string proizvodId = UIHelper.UcitajString("ID proizvoda");
                string razlog = UIHelper.UcitajString("Razlog brisanja");

                bool potvrda = UIHelper.UcitajDaNe("Da li ste sigurni?");

                if (potvrda)
                {
                    inventarService.ObrisiProizvod(proizvodId, razlog);
                }
                else
                {
                    Console.WriteLine("\nBrisanje otkazano.");
                }
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // FUNKCIONALNOST 4: Obavještenja
        static void ProvjeriKriticneZalihe()
        {
            UIHelper.PrikaziNaslov("KRITIČNE ZALIHE");

            var kriticni = inventarService.ProvjeriKriticneZalihe();

            if (kriticni.Count == 0)
            {
                UIHelper.PrikaziUspjeh("Sve zalihe su u redu!");
            }
            else
            {
                Console.WriteLine($"\n! Pronađeno {kriticni.Count} proizvoda sa kritičnim zalihama:\n");
                UIHelper.PrikaziProizvode(kriticni);
            }

            UIHelper.Pauza();
        }

        // NOVO: Analiza stanja inventara
        static void AnalizirajStanjeInventara()
        {
            UIHelper.PrikaziNaslov("ANALIZA STANJA INVENTARA");

            try
            {
                bool ukljuciKategorije = UIHelper.UcitajDaNe("Uključiti analizu po kategorijama?");
                bool ukljuciDobavljace = UIHelper.UcitajDaNe("Uključiti analizu po dobavljačima?");
                bool detaljnaAnaliza = UIHelper.UcitajDaNe("Uključiti detaljne metrike?");

                string izvjestaj = inventarService.AnalizirajStanjeInventara(
                    ukljuciKategorije,
                    ukljuciDobavljace,
                    detaljnaAnaliza
                );

                Console.WriteLine("\n" + izvjestaj);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // NOVO: Prikaz cijena u različitim valutama
        static void PrikaziCijeneUValutama()
        {
            UIHelper.PrikaziNaslov("CIJENE PROIZVODA U RAZLIČITIM VALUTAMA");

            try
            {
                const double CIJENA_PO_JEDINICI = 10.0; // Bazna cijena kao u Narudzba modelu

                var proizvodi = inventarService.PrikaziSveProizvode();

                if (proizvodi.Count == 0)
                {
                    UIHelper.PrikaziGresku("Nema proizvoda u inventaru.");
                    UIHelper.Pauza();
                    return;
                }

                // Izračunaj ukupnu vrijednost inventara u BAM
                double ukupnaVrijednostBAM = 0;

                Console.WriteLine("\n--- Vrijednost po proizvodima (BAM) ---\n");
                Console.WriteLine($"{"Naziv",-30} {"Količina",10} {"Cijena/jed",12} {"Ukupno (BAM)",15}");
                Console.WriteLine(new string('═', 70));

                foreach (var proizvod in proizvodi)
                {
                    double vrijednostProizvoda = proizvod.Kolicina * CIJENA_PO_JEDINICI;
                    ukupnaVrijednostBAM += vrijednostProizvoda;

                    Console.WriteLine($"{proizvod.Naziv,-30} {proizvod.Kolicina,10} {CIJENA_PO_JEDINICI,12:F2} {vrijednostProizvoda,15:F2}");
                }

                Console.WriteLine(new string('═', 70));
                Console.WriteLine($"{"UKUPNA VRIJEDNOST INVENTARA:",-55} {ukupnaVrijednostBAM,15:F2} BAM\n");

                // Ponudi konverziju u druge valute
                Console.WriteLine("\nŽelite li vidjeti vrijednost u drugoj valuti?");
                Console.WriteLine("  [1] EUR");
                Console.WriteLine("  [2] USD");
                Console.WriteLine("  [3] Obje valute");
                Console.WriteLine("  [0] Ne");
                Console.WriteLine();

                string izbor = UIHelper.UcitajString("Izaberite opciju");

                if (izbor == "1" || izbor == "3")
                {
                    double vrijednostEUR = currencyService.KonvertujValutu(ukupnaVrijednostBAM, "BAM", "EUR");
                    Console.WriteLine($"\nUkupna vrijednost u EUR: {vrijednostEUR:F2} €");
                }

                if (izbor == "2" || izbor == "3")
                {
                    double vrijednostUSD = currencyService.KonvertujValutu(ukupnaVrijednostBAM, "BAM", "USD");
                    Console.WriteLine($"Ukupna vrijednost u USD: {vrijednostUSD:F2} $");
                }

                if (izbor == "3")
                {
                    Console.WriteLine("\n--- Poređenje valuta ---");
                    double vrijednostEUR = currencyService.KonvertujValutu(ukupnaVrijednostBAM, "BAM", "EUR");
                    double vrijednostUSD = currencyService.KonvertujValutu(ukupnaVrijednostBAM, "BAM", "USD");
                    Console.WriteLine($"BAM: {ukupnaVrijednostBAM:F2} KM");
                    Console.WriteLine($"EUR: {vrijednostEUR:F2} €");
                    Console.WriteLine($"USD: {vrijednostUSD:F2} $");
                }
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // NOVO: Analiza proizvoda sa visokim prometom
        static void AnalizirajVisokPromet()
        {
            UIHelper.PrikaziNaslov("ANALIZA PROIZVODA SA VISOKIM PROMETOM");

            try
            {
                var turnoverAnalyzer = inventarService.GetTurnoverAnalyzer();
                var proizvodi = inventarService.PrikaziSveProizvode();

                if (proizvodi.Count == 0)
                {
                    UIHelper.PrikaziGresku("Nema proizvoda u inventaru.");
                    UIHelper.Pauza();
                    return;
                }

                // Unos parametara za analizu
                int brojDana = UIHelper.UcitajInt("Unesite period analize (broj dana)");

                if (brojDana <= 0)
                {
                    UIHelper.PrikaziGresku("Broj dana mora biti veći od 0.");
                    UIHelper.Pauza();
                    return;
                }

                Console.WriteLine("\n--- Turnover analiza ---\n");
                Console.WriteLine($"Period: posljednjih {brojDana} dana\n");

                // Prikaži turnover za sve proizvode
                Console.WriteLine($"{"ID",-20} {"Naziv",-30} {"Turnover (promjena/dan)",25}");
                Console.WriteLine(new string('═', 80));

                var proizvodiSaTurnoverom = new List<(string Id, string Naziv, double Turnover)>();

                foreach (var proizvod in proizvodi)
                {
                    double turnover = turnoverAnalyzer.IzracunajTurnover(proizvod.Id, brojDana);
                    proizvodiSaTurnoverom.Add((proizvod.Id, proizvod.Naziv, turnover));
                }

                // Sortiraj po turnover-u (opadajuće)
                proizvodiSaTurnoverom = proizvodiSaTurnoverom
                    .OrderByDescending(p => p.Turnover)
                    .ToList();

                // Prikaži sve proizvode sa turnoverom
                foreach (var item in proizvodiSaTurnoverom)
                {
                    ConsoleColor boja = item.Turnover >= 10 ? ConsoleColor.Green :
                                       item.Turnover >= 5 ? ConsoleColor.Yellow :
                                       ConsoleColor.Gray;

                    Console.Write($"{item.Id,-20} {item.Naziv,-30} ");
                    Console.ForegroundColor = boja;
                    Console.WriteLine($"{item.Turnover,25:F2}");
                    Console.ResetColor();
                }

                Console.WriteLine(new string('═', 80));

                // Analiza high turnover proizvoda
                double prag = UIHelper.UcitajInt("Unesite prag za visok promet (prosječna promjena po danu)");

                Console.WriteLine($"\n--- Proizvodi sa visokim prometom (prag: {prag}) ---\n");

                bool imaHighTurnover = false;

                foreach (var item in proizvodiSaTurnoverom)
                {
                    if (item.Turnover >= prag)
                    {
                        imaHighTurnover = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"✓ {item.Naziv} - Turnover: {item.Turnover:F2} promjena/dan");
                        Console.ResetColor();
                    }
                }

                if (!imaHighTurnover)
                {
                    Console.WriteLine($"Nema proizvoda sa prometom >= {prag} promjena/dan.");
                }

                // Top 5 proizvoda
                Console.WriteLine("\n--- TOP 5 PROIZVODA PO PROMETU ---\n");

                var top5 = proizvodiSaTurnoverom.Take(5).ToList();
                int pozicija = 1;

                foreach (var item in top5)
                {
                    Console.WriteLine($"{pozicija}. {item.Naziv} - {item.Turnover:F2} promjena/dan");
                    pozicija++;
                }

                // Preporuke
                Console.WriteLine("\n--- PREPORUKE ---\n");

                if (proizvodiSaTurnoverom.Any(p => p.Turnover >= 10))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("• Proizvodi sa visokim prometom (>=10) zahtijevaju češće praćenje zaliha");
                    Console.WriteLine("• Razmotriti povećanje minimalnih pragova za high-turnover proizvode");
                    Console.WriteLine("• Planirati češće narudžbe za popularne proizvode");
                    Console.ResetColor();
                }
                else if (proizvodiSaTurnoverom.All(p => p.Turnover < 1))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("• Niska aktivnost - provjeriti da li se proizvodi koriste");
                    Console.WriteLine("• Razmotriti reviziju inventara");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("• Promet je na normalnom nivou");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void UpravljanjeDobavljacima()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    UIHelper.PrikaziMeni("UPRAVLJANJE DOBAVLJAČIMA", new[]
                    {
                        "Dodaj novog dobavljača",
                        "Prikaži sve dobavljače",
                        "Prikaži info o dobavljaču",
                        "Analiziraj dobavljača"
                    });

                    string izbor = Console.ReadLine() ?? "";

                    switch (izbor)
                    {
                        case "1":
                            DodajDobavljaca();
                            break;
                        case "2":
                            PrikaziDobavljace();
                            break;
                        case "3":
                            PrikaziInfoDobavljaca();
                            break;
                        case "4":
                            AnalizirajDobavljaca();
                            break;
                        case "0":
                            nazad = true;
                            break;
                        default:
                            UIHelper.PrikaziGresku("Neispravan izbor.");
                            UIHelper.Pauza();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.PrikaziGresku(ex.Message);
                    UIHelper.Pauza();
                }
            }
        }

        static void DodajDobavljaca()
        {
            UIHelper.PrikaziNaslov("DODAVANJE NOVOG DOBAVLJAČA");

            try
            {
                string naziv = UIHelper.UcitajString("Naziv dobavljača");
                string kontakt = UIHelper.UcitajString("Kontakt (email ili telefon)");

                dobavljacService.DodajDobavljaca(naziv, kontakt);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void PrikaziDobavljace()
        {
            UIHelper.PrikaziNaslov("SVI DOBAVLJAČI");

            var dobavljaci = dobavljacService.PrikaziSveDobavljace();
            UIHelper.PrikaziDobavljace(dobavljaci);

            UIHelper.Pauza();
        }

        static void PrikaziInfoDobavljaca()
        {
            UIHelper.PrikaziNaslov("INFORMACIJE O DOBAVLJAČU");

            try
            {
                string id = UIHelper.UcitajString("ID dobavljača");
                dobavljacService.PrikaziInfoDobavljaca(id);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // NOVO: Analiza dobavljača
        static void AnalizirajDobavljaca()
        {
            UIHelper.PrikaziNaslov("ANALIZA DOBAVLJAČA");

            try
            {
                string id = UIHelper.UcitajString("ID dobavljača");
                bool provjeriDuplikate = UIHelper.UcitajDaNe("Provjeriti duplikate?");
                bool provjeriKvalitet = UIHelper.UcitajDaNe("Provjeriti kvalitet kontakta?");

                string analiza = dobavljacService.AnalizirajDobavljaca(
                    id,
                    provjeriDuplikate,
                    provjeriKvalitet
                );

                Console.WriteLine("\n" + analiza);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void UpravljanjeNarudzbama()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    UIHelper.PrikaziMeni("UPRAVLJANJE NARUDŽBAMA", new[]
                    {
                        "Kreiraj novu narudžbu",
                        "Prikaži sve narudžbe",
                        "Filtriraj narudžbe po statusu",
                        "Označi kao isporučeno",
                        "Otkaži narudžbu",
                        "Analiziraj performanse narudžbi"
                    });

                    string izbor = Console.ReadLine() ?? "";

                    switch (izbor)
                    {
                        case "1":
                            KreirajNarudzbu();
                            break;
                        case "2":
                            PrikaziNarudzbe();
                            break;
                        case "3":
                            FiltrirajNarudzbe();
                            break;
                        case "4":
                            OznaciKaoIsporuceno();
                            break;
                        case "5":
                            OtkaziNarudzbu();
                            break;
                        case "6":
                            AnalizirajPerformanseNarudzbi();
                            break;
                        case "0":
                            nazad = true;
                            break;
                        default:
                            UIHelper.PrikaziGresku("Neispravan izbor.");
                            UIHelper.Pauza();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.PrikaziGresku(ex.Message);
                    UIHelper.Pauza();
                }
            }
        }

        static void KreirajNarudzbu()
        {
            UIHelper.PrikaziNaslov("KREIRANJE NOVE NARUDŽBE");

            try
            {
                var proizvodi = inventarService.PrikaziSveProizvode();
                if (proizvodi.Count == 0)
                {
                    UIHelper.PrikaziGresku("Nema proizvoda u inventaru.");
                    UIHelper.Pauza();
                    return;
                }

                Console.WriteLine("\nDostupni proizvodi:");
                UIHelper.PrikaziProizvode(proizvodi);
                Console.WriteLine();

                int redniBrojProizvoda = UIHelper.UcitajInt($"Redni broj proizvoda (1-{proizvodi.Count})");

                // Provjeri da li je redni broj validan i uzmi proizvod
                if (redniBrojProizvoda < 1 || redniBrojProizvoda > proizvodi.Count)
                {
                    throw new InvalidOperationException($"Neispravan redni broj. Molimo unesite broj između 1 i {proizvodi.Count}.");
                }

                var proizvod = proizvodi[redniBrojProizvoda - 1];
                string proizvodId = proizvod.Id;

                int kolicina = UIHelper.UcitajInt("Količina");

                narudzbaService.KreirajNarudzbu(proizvodId, proizvod.DobavljacId, kolicina);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void PrikaziNarudzbe()
        {
            UIHelper.PrikaziNaslov("SVE NARUDŽBE");

            var narudzbe = narudzbaService.PrikaziSveNarudzbe();
            UIHelper.PrikaziNarudzbe(narudzbe);

            UIHelper.Pauza();
        }

        static void FiltrirajNarudzbe()
        {
            UIHelper.PrikaziNaslov("FILTRIRANJE NARUDŽBI PO STATUSU");

            Console.WriteLine("  [1] NA_CEKANJU");
            Console.WriteLine("  [2] ISPORUCENO");
            Console.WriteLine("  [3] OTKAZANO");
            Console.WriteLine();

            int izbor = UIHelper.UcitajInt("Izaberite status (1-3)");

            try
            {
                StatusNarudzbe status = izbor switch
                {
                    1 => StatusNarudzbe.NA_CEKANJU,
                    2 => StatusNarudzbe.ISPORUCENO,
                    3 => StatusNarudzbe.OTKAZANO,
                    _ => throw new ArgumentException("Neispravan izbor.")
                };

                var filtrirane = narudzbaService.FiltrirajPoStatusu(status);

                Console.WriteLine($"\nNarudžbe sa statusom {status}:");
                UIHelper.PrikaziNarudzbe(filtrirane);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void OznaciKaoIsporuceno()
        {
            UIHelper.PrikaziNaslov("OZNAČAVANJE KAO ISPORUČENO");

            try
            {
                string narudzbaId = UIHelper.UcitajString("ID narudžbe");
                narudzbaService.OznaciKaoIsporuceno(narudzbaId);

                // Ažuriraj količinu proizvoda kada je narudžba isporučena
                var narudzba = narudzbaService.PronadjiNarudzbuPoId(narudzbaId);
                if (narudzba != null)
                {
                    inventarService.AzurirajKolicinu(narudzba.ProizvodId, narudzba.Kolicina, "Isporuka narudžbe " + narudzbaId);
                }
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void OtkaziNarudzbu()
        {
            UIHelper.PrikaziNaslov("OTKAZIVANJE NARUDŽBE");

            try
            {
                string narudzbaId = UIHelper.UcitajString("ID narudžbe");
                narudzbaService.OtkaziNarudzbu(narudzbaId);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // NOVO: Analiza performansi narudžbi
        static void AnalizirajPerformanseNarudzbi()
        {
            UIHelper.PrikaziNaslov("ANALIZA PERFORMANSI NARUDŽBI");

            try
            {
                bool ukljuciVremenskaAnaliza = UIHelper.UcitajDaNe("Uključiti vremensku analizu?");
                bool ukljuciFinansijskaAnaliza = UIHelper.UcitajDaNe("Uključiti finansijsku analizu?");
                bool detaljniIzvjestaj = UIHelper.UcitajDaNe("Uključiti detaljan izvještaj?");

                string izvjestaj = narudzbaService.AnalizirajPerformanseNarudzbi(
                    ukljuciVremenskaAnaliza,
                    ukljuciFinansijskaAnaliza,
                    detaljniIzvjestaj
                );

                Console.WriteLine("\n" + izvjestaj);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void UpravljanjeObavjestenjima()
        {
            bool nazad = false;

            while (!nazad)
            {
                try
                {
                    UIHelper.PrikaziMeni("OBAVJEŠTENJA O ZALIHAMA", new[]
                    {
                        "Prikaži sva obavještenja",
                        "Filtriraj obavještenja po tipu",
                        "Prikaži detaljan izvještaj",
                        "Analiziraj obavještenja"
                    });

                    string izbor = Console.ReadLine() ?? "";

                    switch (izbor)
                    {
                        case "1":
                            PrikaziObavjestenja();
                            break;
                        case "2":
                            FiltrirajObavjestenja();
                            break;
                        case "3":
                            PrikaziDetaljanIzvjestaj();
                            break;
                        case "4":
                            AnalizirajObavjestenja();
                            break;
                        case "0":
                            nazad = true;
                            break;
                        default:
                            UIHelper.PrikaziGresku("Neispravan izbor.");
                            UIHelper.Pauza();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UIHelper.PrikaziGresku(ex.Message);
                    UIHelper.Pauza();
                }
            }
        }

        static void PrikaziObavjestenja()
        {
            UIHelper.PrikaziNaslov("SVA OBAVJEŠTENJA");

            var obavjestenja = obavjestenjeService.PrikaziSvaObavjestenja();
            UIHelper.PrikaziObavjestenja(obavjestenja);

            UIHelper.Pauza();
        }

        static void FiltrirajObavjestenja()
        {
            UIHelper.PrikaziNaslov("FILTRIRANJE OBAVJEŠTENJA PO TIPU");

            Console.WriteLine("  [1] NISKE_ZALIHE");
            Console.WriteLine("  [2] NEMA_NA_STANJU");
            Console.WriteLine("  [3] POTREBNA_NARUDBA");
            Console.WriteLine();

            int izbor = UIHelper.UcitajInt("Izaberite tip (1-3)");

            try
            {
                TipObavjestenja tip = izbor switch
                {
                    1 => TipObavjestenja.NISKE_ZALIHE,
                    2 => TipObavjestenja.NEMA_NA_STANJU,
                    3 => TipObavjestenja.POTREBNA_NARUDBA,
                    _ => throw new ArgumentException("Neispravan izbor.")
                };

                var filtrirane = obavjestenjeService.FiltrirajPoTipu(tip);

                Console.WriteLine($"\nObavještenja tipa {tip}:");
                UIHelper.PrikaziObavjestenja(filtrirane);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        static void PrikaziDetaljanIzvjestaj()
        {
            UIHelper.PrikaziNaslov("DETALJAN IZVJEŠTAJ");

            try
            {
                string obavjestenjeId = UIHelper.UcitajString("ID obavještenja");
                obavjestenjeService.PrikaziDetaljanIzvjestaj(obavjestenjeId);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // NOVO: Analiza obavještenja
        static void AnalizirajObavjestenja()
        {
            UIHelper.PrikaziNaslov("ANALIZA OBAVJEŠTENJA");

            try
            {
                bool ukljuciVremenskaAnaliza = UIHelper.UcitajDaNe("Uključiti vremensku analizu?");
                bool ukljuciStatistiku = UIHelper.UcitajDaNe("Uključiti statističku analizu?");
                bool generirajAkcioniPlan = UIHelper.UcitajDaNe("Generisati akcioni plan?");

                string izvjestaj = obavjestenjeService.AnalizirajObavjestenja(
                    ukljuciVremenskaAnaliza,
                    ukljuciStatistiku,
                    generirajAkcioniPlan
                );

                Console.WriteLine("\n" + izvjestaj);
            }
            catch (Exception ex)
            {
                UIHelper.PrikaziGresku(ex.Message);
            }

            UIHelper.Pauza();
        }

        // Inicijalizacija testnih podataka
        static void InicijalizujTestnePodatke()
        {
            try
            {
                // Dodaj testne dobavljače
                dobavljacService.DodajDobavljaca("Tech Solutions", "info@techsolutions.ba");
                dobavljacService.DodajDobavljaca("Office Supplies Co", "+387 33 123 456");
                dobavljacService.DodajDobavljaca("Tool Masters", "sales@toolmasters.com");

                // Dodaj testne proizvode
                var dobavljaci = dobavljacService.PrikaziSveDobavljace();
                if (dobavljaci.Count >= 3)
                {
                    inventarService.DodajProizvod("Laptop Dell", 15, 5, dobavljaci[0].Id, KategorijaProizvoda.IT_UREDJAJI);
                    inventarService.DodajProizvod("Bušilica", 3, 5, dobavljaci[2].Id, KategorijaProizvoda.ALATI);
                    inventarService.DodajProizvod("Stolica", 25, 10, dobavljaci[1].Id, KategorijaProizvoda.OPREMA);
                    inventarService.DodajProizvod("A4 Papir", 100, 50, dobavljaci[1].Id, KategorijaProizvoda.KANCELARIJSKI_MATERIJALI);
                }
            }
            catch
            {
                // Tiho ignoriši greške pri inicijalizaciji
            }
        }
    }
}
