using InventarApp.Interfaces;
using InventarApp.Enums;
using System.Text;

namespace InventarApp.Models
{
    public class Obavjestenje : IObavjestenje
    {
        public string ObavjestenjeId { get; set; }
        public string ProizvodId { get; set; }
        public string Poruka { get; set; }
        public DateTime VrijemeKreiranja { get; set; }
        public TipObavjestenja Tip { get; set; }

        private static int _brojac = 0;

        public Obavjestenje(string proizvodId, string poruka, TipObavjestenja tip)
        {
            if (string.IsNullOrWhiteSpace(proizvodId))
                throw new ArgumentException("ID proizvoda ne može biti prazan.");

            if (string.IsNullOrWhiteSpace(poruka))
                throw new ArgumentException("Poruka ne može biti prazna.");

            ProizvodId = proizvodId;
            Poruka = poruka;
            Tip = tip;
            VrijemeKreiranja = DateTime.Now;
            ObavjestenjeId = GenerisiId();
        }

        private string GenerisiId()
        {
            _brojac++;
            return $"Obavjestenje-{_brojac}";
        }

        // Složeni algoritam - Generisanje detaljnog izvještaja sa prioritizacijom
        public string PosaljiIzvjestaj()
        {
            StringBuilder izvjestaj = new StringBuilder();

            izvjestaj.AppendLine("══════════════════════════════════════════════════════════");
            izvjestaj.AppendLine("          IZVJEŠTAJ O STANJU INVENTARA                    ");
            izvjestaj.AppendLine("══════════════════════════════════════════════════════════");
            izvjestaj.AppendLine();

            // Dodaj prioritet na osnovu tipa obavještenja
            int prioritet = IzracunajPrioritet(Tip);
            string prioritetOznaka = FormirajPrioritetOznaku(prioritet);

            izvjestaj.AppendLine($"Prioritet: {prioritetOznaka}");
            izvjestaj.AppendLine($"ID Obavještenja: {ObavjestenjeId}");
            izvjestaj.AppendLine($"Tip: {FormirajTipOpis(Tip)}");
            izvjestaj.AppendLine($"Vrijeme kreiranja: {VrijemeKreiranja:dd.MM.yyyy HH:mm:ss}");
            izvjestaj.AppendLine($"ID Proizvoda: {ProizvodId}");
            izvjestaj.AppendLine();
            izvjestaj.AppendLine("DETALJI:");
            izvjestaj.AppendLine(new string('-', 58));
            izvjestaj.AppendLine(Poruka);
            izvjestaj.AppendLine(new string('-', 58));
            izvjestaj.AppendLine();

            // Dodaj preporuke na osnovu tipa
            izvjestaj.AppendLine("PREPORUČENE AKCIJE:");
            izvjestaj.AppendLine(GenerirajPreporuke(Tip));

            return izvjestaj.ToString();
        }

        // Algoritam za izračunavanje prioriteta (1-visok, 2-srednji, 3-nizak)
        private int IzracunajPrioritet(TipObavjestenja tip)
        {
            return tip switch
            {
                TipObavjestenja.NEMA_NA_STANJU => 1,        // Najviši prioritet
                TipObavjestenja.NISKE_ZALIHE => 2,          // Srednji prioritet
                TipObavjestenja.POTREBNA_NARUDBA => 2,      // Srednji prioritet
                _ => 3                                       // Nizak prioritet
            };
        }

        private string FormirajPrioritetOznaku(int prioritet)
        {
            return prioritet switch
            {
                1 => "VISOK",
                2 => "SREDNJI",
                3 => "NIZAK",
                _ => "NEPOZNAT"
            };
        }

        private string FormirajTipOpis(TipObavjestenja tip)
        {
            return tip switch
            {
                TipObavjestenja.NISKE_ZALIHE => "Niske zalihe",
                TipObavjestenja.NEMA_NA_STANJU => "Nema na stanju",
                TipObavjestenja.POTREBNA_NARUDBA => "Potrebna narudžba",
                _ => "Nepoznat tip"
            };
        }

        private string GenerirajPreporuke(TipObavjestenja tip)
        {
            return tip switch
            {
                TipObavjestenja.NEMA_NA_STANJU =>
                    "  • Hitno kreirati narudžbu kod dobavljača\n" +
                    "  • Obavijestiti klijente o nedostupnosti\n" +
                    "  • Razmotriti alternativne proizvode",

                TipObavjestenja.NISKE_ZALIHE =>
                    "  • Planirati narudžbu u narednih 7 dana\n" +
                    "  • Provjeriti minimalni prag zaliha\n" +
                    "  • Kontaktirati dobavljača za dostupnost",

                TipObavjestenja.POTREBNA_NARUDBA =>
                    "  • Pripremiti narudžbu prema prognozi\n" +
                    "  • Provjeriti budžet za nabavku\n" +
                    "  • Utvrditi optimalne količine",

                _ => "  • Nema posebnih preporuka"
            };
        }

        public string DajDetaljeObavjestenja()
        {
            return $"[{VrijemeKreiranja:HH:mm:ss}] {FormirajTipOpis(Tip)} - {Poruka}";
        }

        // Složeni algoritam - Analiza urgentnosti obavještenja sa McCabe Complexity >= 8
        // Određuje urgentnost na osnovu tipa, vremena, sadržaja poruke i dodatnih faktora
        // Složeni algoritam - Analiza urgentnosti obavještenja sa McCabe Complexity = 8
        // Složeni algoritam - Analiza urgentnosti obavještenja sa McCabe Complexity = 8
        public string AnalizirajUrgentnost()
        {
            TimeSpan vrijemeOdKreiranja = DateTime.Now - VrijemeKreiranja;

            // 1. Analiza tipa obavještenja
            var (tipBodovi, tipDetalji) = AnalizirajTipObavjestenja();

            // 2. Analiza vremena od kreiranja
            var (vrijemeBodovi, vrijemeDetalji) = AnalizirajVrijemeOdKreiranja(vrijemeOdKreiranja);

            // 3. Analiza sadržaja poruke (ključne riječi)
            var (porukaBodovi, porukaDetalji) = AnalizirajSadrzajPoruke();

            // 4. Analiza vremena u danu (radni sati vs van radnih sati)
            var (satBodovi, satDetalji) = AnalizirajVrijemeDana();

            // Zbrajanje bodova
            int urgentnostBodovi = tipBodovi + vrijemeBodovi + porukaBodovi + satBodovi;
            string analizaDetalji = tipDetalji + vrijemeDetalji + porukaDetalji + satDetalji;

            // 5. Analiza dana u sedmici (inline)
            if (VrijemeKreiranja.DayOfWeek == DayOfWeek.Saturday)
            {
                urgentnostBodovi += 10;
                analizaDetalji += "Kreirano u subotu - povećana urgentnost. ";
            }
            else if (VrijemeKreiranja.DayOfWeek == DayOfWeek.Sunday)
            {
                urgentnostBodovi += 15;
                analizaDetalji += "Kreirano u nedjelju - visoka urgentnost. ";
            }

            // 6. Dodatni bodovi za brzinu reakcije
            if (vrijemeOdKreiranja.TotalMinutes < 30 && urgentnostBodovi >= 50)
            {
                urgentnostBodovi += 20;
                analizaDetalji += "Neophodno hitno djelovanje (<30min). ";
            }

            // 7. Provjera kritičnog stanja
            if (Tip == TipObavjestenja.NEMA_NA_STANJU && vrijemeOdKreiranja.TotalHours < 2)
            {
                urgentnostBodovi += 15;
                analizaDetalji += "Kritično: nema zaliha i svježe obavještenje. ";
            }

            // 8. Provjera dužine poruke za dodatni kontekst
            if (Poruka.Length > 100)
            {
                urgentnostBodovi += 5;
                analizaDetalji += "Detaljna poruka - dodatni kontekst. ";
            }

            // 9. Klasifikacija nivoa urgentnosti
            var (nivoUrgentnosti, preporuka) = OdrediNivoUrgentnosti(urgentnostBodovi);

            return $"Nivo urgentnosti: {nivoUrgentnosti} ({urgentnostBodovi} bodova)\n" +
                   $"Preporuka: {preporuka}\n" +
                   $"Analiza: {analizaDetalji}\n" +
                   $"Vrijeme od kreiranja: {vrijemeOdKreiranja.TotalHours:F1}h";
        }

        private (int bodovi, string detalji) AnalizirajTipObavjestenja() => Tip switch
        {
            TipObavjestenja.NEMA_NA_STANJU => (50, "Kritični tip - nema zaliha. "),
            TipObavjestenja.NISKE_ZALIHE => (30, "Važan tip - niske zalihe. "),
            TipObavjestenja.POTREBNA_NARUDBA => (20, "Informativni tip - potrebna narudžba. "),
            _ => (0, "")
        };

        private (int bodovi, string detalji) AnalizirajVrijemeOdKreiranja(TimeSpan vrijeme) => vrijeme.TotalHours switch
        {
            < 1 => (15, "Novo obavještenje (< 1h). "),
            < 24 => (10, "Nedavno obavještenje (< 24h). "),
            < 72 => (5, "Obavještenje starije od 1 dan. "),
            >= 168 => (-10, "Staro obavještenje (>7 dana) - urgentnost smanjena. "),
            _ => (0, "")
        };

        private (int bodovi, string detalji) AnalizirajSadrzajPoruke()
        {
            int bodovi = 0;
            string detalji = "";
            string porukaMala = Poruka.ToLower();

            string[] hitneRijeci = { "hitno", "kritično" };
            string[] nulteRijeci = { "0", "nula" };
            string[] upozorenjeRijeci = { "upozorenje", "pažnja" };

            if (hitneRijeci.Any(r => porukaMala.Contains(r)))
            {
                bodovi += 20;
                detalji += "Ključna riječ 'HITNO' detektovana. ";
            }

            if (nulteRijeci.Any(r => porukaMala.Contains(r)))
            {
                bodovi += 15;
                detalji += "Nulta količina detektovana. ";
            }

            if (upozorenjeRijeci.Any(r => porukaMala.Contains(r)))
            {
                bodovi += 10;
                detalji += "Upozoravajući termin. ";
            }

            return (bodovi, detalji);
        }

        private (int bodovi, string detalji) AnalizirajVrijemeDana() => VrijemeKreiranja.Hour switch
        {
            >= 9 and <= 17 => (5, "Kreirano u radnim satima. "),
            >= 18 and <= 22 => (10, "Kreirano poslije radnog vremena - urgentno. "),
            _ => (15, "Kreirano van standardnih sati - vrlo urgentno. ")
        };

        private (string nivo, string preporuka) OdrediNivoUrgentnosti(int bodovi) => bodovi switch
        {
            >= 100 => ("EKSTREMNO URGENTNO", "Djelovati ODMAH - kritična situacija!"),
            >= 80 => ("VRLO URGENTNO", "Djelovati u narednih 1-2 sata."),
            >= 60 => ("URGENTNO", "Djelovati danas, prioritetno."),
            >= 40 => ("SREDNJA URGENTNOST", "Djelovati u naredna 2-3 dana."),
            >= 20 => ("NISKA URGENTNOST", "Djelovati u narednoj sedmici."),
            _ => ("MINIMALNA URGENTNOST", "Pratiti stanje, bez hitnosti.")
        };
        public override string ToString()
        {
            return $"ID: {ObavjestenjeId}, Tip: {Tip}, Proizvod: {ProizvodId}, Vrijeme: {VrijemeKreiranja:dd.MM.yyyy HH:mm}";
        }
    }
}
