using InventarApp.Models;
using InventarApp.Enums;

namespace InventarApp.Services
{
    public class ObavjestenjeService
    {
        private List<Obavjestenje> _obavjestenja;

        public ObavjestenjeService()
        {
            _obavjestenja = new List<Obavjestenje>();
        }

        public void KreirajObavjestenje(Proizvod proizvod)
        {
            TipObavjestenja tip;
            string poruka;

            if (proizvod.Kolicina == 0)
            {
                tip = TipObavjestenja.NEMA_NA_STANJU;
                poruka = $"HITNO: Proizvod '{proizvod.Naziv}' (ID: {proizvod.Id}) više NEMA NA STANJU!";
            }
            else if (proizvod.JeIspodMinimalnogPraga())
            {
                tip = TipObavjestenja.NISKE_ZALIHE;
                poruka = $"UPOZORENJE: Proizvod '{proizvod.Naziv}' (ID: {proizvod.Id}) ima niske zalihe. " +
                         $"Trenutna količina: {proizvod.Kolicina}, Minimalni prag: {proizvod.MinimalniPrag}";
            }
            else
            {
                return; // Nema potrebe za obavještenjem
            }

            var obavjestenje = new Obavjestenje(proizvod.Id, poruka, tip);
            _obavjestenja.Add(obavjestenje);

            Console.WriteLine($"\n⚠ {obavjestenje.DajDetaljeObavjestenja()}");
        }

        public void KreirajObavjestenjeZaNarudzbu(string proizvodId, string poruka)
        {
            var obavjestenje = new Obavjestenje(
                proizvodId,
                poruka,
                TipObavjestenja.POTREBNA_NARUDBA
            );

            _obavjestenja.Add(obavjestenje);
            Console.WriteLine($"\n📋 {obavjestenje.DajDetaljeObavjestenja()}");
        }

        public List<Obavjestenje> PrikaziSvaObavjestenja()
        {
            return new List<Obavjestenje>(_obavjestenja);
        }

        public List<Obavjestenje> FiltrirajPoTipu(TipObavjestenja tip)
        {
            return _obavjestenja.Where(o => o.Tip == tip).ToList();
        }

        public void PrikaziDetaljanIzvjestaj(string obavjestenjeId)
        {
            var obavjestenje = PronadjiObavjestenjePoId(obavjestenjeId);

            if (obavjestenje == null)
                throw new InvalidOperationException($"Obavještenje sa ID '{obavjestenjeId}' nije pronađeno.");

            Console.WriteLine("\n" + obavjestenje.PosaljiIzvjestaj());
        }

        public Obavjestenje? PronadjiObavjestenjePoId(string id)
        {
            return _obavjestenja.FirstOrDefault(o => o.ObavjestenjeId == id);
        }

        public int BrojObavjestenja()
        {
            return _obavjestenja.Count;
        }

        // Složeni algoritam - Analiza i prioritizacija obavještenja sa McCabe Complexity >= 8
        // Metoda koja odgovara preduslovima
        public string AnalizirajObavjestenja(bool ukljuciVremenskaAnaliza = true, bool ukljuciStatistiku = true, bool generirajAkcioniPlan = true)
        {
            var izvjestaj = "═══════════════════════════════════════════════\n";
            izvjestaj += "       ANALIZA OBAVJEŠTENJA\n";
            izvjestaj += "═══════════════════════════════════════════════\n\n";

            // 1. Provjera da li postoje obavještenja
            if (_obavjestenja.Count == 0)
            {
                return izvjestaj + "✓ Nema aktivnih obavještenja - sistem je u dobrom stanju.\n";
            }

            var kriticnaAkcija = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 100;

            // 2. OSNOVNA ANALIZA PO TIPU (inline iz AnalizirajPoTipu sa for petljom)
            int nemaNaStanju = 0;
            int niskeZalihe = 0;
            int potrebnaNarudzba = 0;

            // Eksplicitna for petlja za brojanje po tipu
            for (int i = 0; i < _obavjestenja.Count; i++)
            {
                var obavjestenje = _obavjestenja[i];
                if (obavjestenje.Tip == TipObavjestenja.NEMA_NA_STANJU)
                {
                    nemaNaStanju++;
                    ocjena -= 15;
                }
                else if (obavjestenje.Tip == TipObavjestenja.NISKE_ZALIHE)
                {
                    niskeZalihe++;
                    ocjena -= 8;
                }
                else if (obavjestenje.Tip == TipObavjestenja.POTREBNA_NARUDBA)
                {
                    potrebnaNarudzba++;
                    ocjena -= 3;
                }
            }

            izvjestaj += $"Ukupno obavještenja: {_obavjestenja.Count}\n";
            izvjestaj += $"Nema na stanju: {nemaNaStanju}\n";
            izvjestaj += $"Niske zalihe: {niskeZalihe}\n";
            izvjestaj += $"Potrebna narudžba: {potrebnaNarudzba}\n\n";

            // 3. Analiza hitnih obavještenja
            if (nemaNaStanju > 0)
            {
                var (hitnaOcjena, hitneAkcije, hitnePreporuke) = AnalizirajHitnaObavjestenja(nemaNaStanju, _obavjestenja.Count);
                ocjena += hitnaOcjena;
                kriticnaAkcija.AddRange(hitneAkcije);
                preporuke.AddRange(hitnePreporuke);
            }

            // 4. Analiza obavještenja srednjeg prioriteta
            if (niskeZalihe > 0)
            {
                var (srednjaOcjena, srednjeAkcije, srednjePreporuke) = AnalizirajSrednjePrioritetnaObavjestenja(niskeZalihe, _obavjestenja.Count);
                ocjena += srednjaOcjena;
                kriticnaAkcija.AddRange(srednjeAkcije);
                preporuke.AddRange(srednjePreporuke);
            }

            // 5. Vremenska analiza
            if (ukljuciVremenskaAnaliza)
            {
                var (vremenskaIzvjestaj, vremenskaOcjena, vremenskaAkcije, vremenskePreporuke) =
                    GenerirajVremenskuAnalizu();
                izvjestaj += vremenskaIzvjestaj;
                ocjena += vremenskaOcjena;
                kriticnaAkcija.AddRange(vremenskaAkcije);
                preporuke.AddRange(vremenskePreporuke);
            }

            // 6. Statistička analiza
            if (ukljuciStatistiku)
            {
                var (statIzvjestaj, statOcjena, statAkcije, statPreporuke) =
                    GenerirajStatistickuAnalizu();
                izvjestaj += statIzvjestaj;
                ocjena += statOcjena;
                kriticnaAkcija.AddRange(statAkcije);
                preporuke.AddRange(statPreporuke);
            }

            // 7. Akcioni plan
            if (generirajAkcioniPlan)
            {
                izvjestaj += GenerirajAkcioniPlan();
            }

            // 8. Provjera omjera riješenih obavještenja
            int ukupnoHitnih = nemaNaStanju + niskeZalihe;
            if (ukupnoHitnih > _obavjestenja.Count / 2)
            {
                kriticnaAkcija.Add("Više od 50% obavještenja zahtijeva hitnu akciju!");
                ocjena -= 10;
            }

            // 9. Konačni izvještaj
            izvjestaj += GenerirajKonacniIzvjestaj(ocjena, kriticnaAkcija, preporuke);

            return izvjestaj;
        }

        private (int nemaNaStanju, int niskeZalihe, int potrebnaNarudzba, int ocjena) AnalizirajPoTipu()
        {
            int nemaNaStanju = 0, niskeZalihe = 0, potrebnaNarudzba = 0, ocjena = 100;

            foreach (var obavjestenje in _obavjestenja)
            {
                switch (obavjestenje.Tip)
                {
                    case TipObavjestenja.NEMA_NA_STANJU:
                        nemaNaStanju++;
                        ocjena -= 15;
                        break;
                    case TipObavjestenja.NISKE_ZALIHE:
                        niskeZalihe++;
                        ocjena -= 8;
                        break;
                    case TipObavjestenja.POTREBNA_NARUDBA:
                        potrebnaNarudzba++;
                        ocjena -= 3;
                        break;
                }
            }

            return (nemaNaStanju, niskeZalihe, potrebnaNarudzba, ocjena);
        }

        private (int ocjena, List<string> akcije, List<string> preporuke) AnalizirajHitnaObavjestenja(int nemaNaStanju, int ukupno)
        {
            var akcije = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            double postotak = (nemaNaStanju * 100.0) / ukupno;

            if (postotak > 50)
            {
                akcije.Add("KRITIČNO: Više od 50% obavještenja je za proizvode bez zaliha!");
                preporuke.Add("-> Aktivirati hitni plan nabavke");
                preporuke.Add("-> Kontaktirati sve dobavljače odmah");
                ocjena = -30;
            }
            else if (postotak > 30)
            {
                akcije.Add("UPOZORENJE: Više od 30% obavještenja je hitno!");
                preporuke.Add("-> Prioritetno riješiti obavještenja 'NEMA NA STANJU'");
                ocjena = -15;
            }
            else if (postotak > 10)
            {
                akcije.Add("Značajan broj hitnih obavještenja!");
                preporuke.Add("-> Ubrzati proces nabavke za kritične proizvode");
            }

            return (ocjena, akcije, preporuke);
        }

        private (int ocjena, List<string> akcije, List<string> preporuke) AnalizirajSrednjePrioritetnaObavjestenja(int niskeZalihe, int ukupno)
        {
            var akcije = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            double postotak = (niskeZalihe * 100.0) / ukupno;

            if (postotak > 60)
            {
                akcije.Add("Veliki broj proizvoda sa niskim zalihama!");
                preporuke.Add("-> Razmotriti povećanje minimalnih pragova");
                ocjena = -20;
            }
            else if (postotak > 40)
            {
                preporuke.Add("-> Planirati narudžbe za proizvode sa niskim zalihama");
                ocjena = -10;
            }

            return (ocjena, akcije, preporuke);
        }

        private (string izvjestaj, int ocjena, List<string> akcije, List<string> preporuke) GenerirajVremenskuAnalizu()
        {
            var izvjestaj = "\n--- Vremenska analiza ---\n";
            var akcije = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            var sada = DateTime.Now;
            var granica24h = sada.AddHours(-24);
            var granica7dana = sada.AddDays(-7);

            int starijaOd24h = _obavjestenja.Count(o => o.VrijemeKreiranja < granica24h && o.VrijemeKreiranja >= granica7dana);
            int starijaOd7dana = _obavjestenja.Count(o => o.VrijemeKreiranja < granica7dana);

            izvjestaj += $"Novija od 24h: {_obavjestenja.Count - starijaOd24h - starijaOd7dana}\n";
            izvjestaj += $"Starija od 24h: {starijaOd24h}\n";
            izvjestaj += $"Starija od 7 dana: {starijaOd7dana}\n";

            if (starijaOd7dana > 0)
            {
                double postotak = (starijaOd7dana * 100.0) / _obavjestenja.Count;

                if (postotak > 50)
                {
                    akcije.Add($"🚨 KRITIČNO: {starijaOd7dana} obavještenja nije riješeno više od 7 dana!");
                    preporuke.Add("-> Hitno adresirati zaostala obavještenja");
                    ocjena = -25;
                }
                else if (postotak > 25)
                {
                    akcije.Add($"{starijaOd7dana} obavještenja čeka duže od 7 dana");
                    preporuke.Add("-> Razmotriti efikasnost procesa rješavanja obavještenja");
                    ocjena = -15;
                }

                izvjestaj += GenerirajInfoNajstarijegObavjestenja(sada, akcije, ref ocjena);
            }

            return (izvjestaj, ocjena, akcije, preporuke);
        }

        private string GenerirajInfoNajstarijegObavjestenja(DateTime sada, List<string> akcije, ref int ocjena)
        {
            var izvjestaj = "";
            var najstarije = _obavjestenja.OrderBy(o => o.VrijemeKreiranja).FirstOrDefault();

            if (najstarije != null)
            {
                int danaStar = (sada - najstarije.VrijemeKreiranja).Days;
                izvjestaj += $"\nNajstarije obavještenje: {najstarije.ObavjestenjeId}\n";
                izvjestaj += $"Starost: {danaStar} dana\n";
                izvjestaj += $"Tip: {najstarije.Tip}\n";

                if (danaStar > 14 && najstarije.Tip == TipObavjestenja.NEMA_NA_STANJU)
                {
                    akcije.Add($"Proizvod bez zaliha već {danaStar} dana!");
                    ocjena -= 20;
                }
            }

            return izvjestaj;
        }

        private (string izvjestaj, int ocjena, List<string> akcije, List<string> preporuke) GenerirajStatistickuAnalizu()
        {
            var izvjestaj = "\n--- Statistička analiza ---\n";
            var akcije = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            var grupePoProizvodu = _obavjestenja.GroupBy(o => o.ProizvodId);
            var problematicniProizvodi = grupePoProizvodu.Where(g => g.Count() > 1).ToList();

            if (problematicniProizvodi.Any())
            {
                izvjestaj += $"Proizvodi sa višestrukim obavještenjima: {problematicniProizvodi.Count}\n";

                var najproblematicniji = problematicniProizvodi.OrderByDescending(g => g.Count()).First();
                izvjestaj += $"Najproblematičniji proizvod: {najproblematicniji.Key} ({najproblematicniji.Count()} obavještenja)\n";

                if (najproblematicniji.Count() >= 5)
                {
                    akcije.Add($"Proizvod {najproblematicniji.Key} ima {najproblematicniji.Count()} obavještenja!");
                    preporuke.Add($"-> Istražiti sistemski problem sa proizvodom {najproblematicniji.Key}");
                    ocjena = -25;
                }
                else if (najproblematicniji.Count() >= 3)
                {
                    akcije.Add($"Proizvod {najproblematicniji.Key} ima česta obavještenja");
                    preporuke.Add($"-> Procijeniti strategiju zaliha za proizvod {najproblematicniji.Key}");
                    ocjena = -10;
                }
            }
            else
            {
                izvjestaj += "Nema proizvoda sa višestrukim obavještenjima\n";
            }

            var prosjecnaSatima = _obavjestenja.Average(o => (DateTime.Now - o.VrijemeKreiranja).TotalHours);
            izvjestaj += $"Prosječna starost obavještenja: {prosjecnaSatima:F1} sati\n";

            return (izvjestaj, ocjena, akcije, preporuke);
        }

        private string GenerirajAkcioniPlan()
        {
            var izvjestaj = "\n--- Akcioni plan ---\n";

            var hitnaObavjestenja = _obavjestenja
                .Where(o => o.Tip == TipObavjestenja.NEMA_NA_STANJU)
                .OrderBy(o => o.VrijemeKreiranja)
                .ToList();

            if (hitnaObavjestenja.Any())
            {
                izvjestaj += "\nPRIORITET 1 - Hitno riješiti:\n";
                foreach (var obavjestenje in hitnaObavjestenja.Take(5))
                {
                    int sati = (int)(DateTime.Now - obavjestenje.VrijemeKreiranja).TotalHours;
                    izvjestaj += $"  - {obavjestenje.ObavjestenjeId} - Proizvod {obavjestenje.ProizvodId} ({sati}h staro)\n";
                }

                if (hitnaObavjestenja.Count > 5)
                {
                    izvjestaj += $"  ... i još {hitnaObavjestenja.Count - 5} hitnih obavještenja\n";
                }
            }

            var srednjaPrioritetLista = _obavjestenja
                .Where(o => o.Tip == TipObavjestenja.NISKE_ZALIHE)
                .OrderBy(o => o.VrijemeKreiranja)
                .Take(3)
                .ToList();

            if (srednjaPrioritetLista.Any())
            {
                izvjestaj += "\nPRIORITET 2 - Planirati:\n";
                foreach (var obavjestenje in srednjaPrioritetLista)
                {
                    int sati = (int)(DateTime.Now - obavjestenje.VrijemeKreiranja).TotalHours;
                    izvjestaj += $"  - {obavjestenje.ObavjestenjeId} - Proizvod {obavjestenje.ProizvodId} ({sati}h staro)\n";
                }
            }

            return izvjestaj;
        }

        private string GenerirajKonacniIzvjestaj(int ocjena, List<string> kriticnaAkcija, List<string> preporuke)
        {
            var izvjestaj = "\n═══════════════════════════════════════════════\n";

            string statusOcjena = OdrediStatusOcjenu(ocjena);
            izvjestaj += $"STATUS SISTEMA: {statusOcjena} ({ocjena}/100)\n";
            izvjestaj += "═══════════════════════════════════════════════\n";

            if (kriticnaAkcija.Any())
            {
                izvjestaj += "\nKRITIČNE AKCIJE:\n";
                izvjestaj += string.Join("\n", kriticnaAkcija.Select(a => $"  {a}"));
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

        private string OdrediStatusOcjenu(int ocjena) => ocjena switch
        {
            >= 90 => "ODLIČAN",
            >= 70 => "DOBAR",
            >= 50 => "ZADOVOLJAVAJUĆI",
            >= 30 => "LOŠ",
            _ => "KRITIČAN"
        };
    }
}
