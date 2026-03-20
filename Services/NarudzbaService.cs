using InventarApp.Models;
using InventarApp.Enums;

namespace InventarApp.Services
{
    public class NarudzbaService
    {
        private List<Narudzba> _narudzbe;
        private HighTurnoverAnalyzer _turnoverAnalyzer;

        public NarudzbaService(HighTurnoverAnalyzer turnoverAnalyzer)
        {
            _narudzbe = new List<Narudzba>();
            _turnoverAnalyzer = turnoverAnalyzer;
        }

        public void KreirajNarudzbu(string proizvodId, string dobavljacId, int kolicina)
        {
            try
            {
                var narudzba = new Narudzba(proizvodId, dobavljacId, kolicina);
                _narudzbe.Add(narudzba);

                Console.WriteLine($"\n✓ Narudžba uspješno kreirana!");
                Console.WriteLine($"  ID: {narudzba.NarudzbaId}");
                Console.WriteLine($"  Ukupna cijena: {narudzba.IzracunajUkupno():F2} KM");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška pri kreiranju narudžbe: {ex.Message}", ex);
            }
        }

        public List<Narudzba> PrikaziSveNarudzbe()
        {
            return new List<Narudzba>(_narudzbe);
        }

        public List<Narudzba> FiltrirajPoStatusu(StatusNarudzbe status)
        {
            return _narudzbe.Where(n => n.Status == status).ToList();
        }

        public void OznaciKaoIsporuceno(string narudzbaId)
        {
            var narudzba = PronadjiNarudzbuPoId(narudzbaId);

            if (narudzba == null)
                throw new InvalidOperationException($"Narudžba sa ID '{narudzbaId}' nije pronađena.");

            try
            {
                narudzba.OznacKaoIsporuceno();
                Console.WriteLine($"\n✓ Narudžba {narudzbaId} je označena kao isporučena!");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška: {ex.Message}", ex);
            }
        }

        public void OtkaziNarudzbu(string narudzbaId)
        {
            var narudzba = PronadjiNarudzbuPoId(narudzbaId);

            if (narudzba == null)
                throw new InvalidOperationException($"Narudžba sa ID '{narudzbaId}' nije pronađena.");

            try
            {
                narudzba.OtkaziNarudzbu();
                Console.WriteLine($"\n✓ Narudžba {narudzbaId} je otkazana!");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Greška: {ex.Message}", ex);
            }
        }

        public Narudzba? PronadjiNarudzbuPoId(string id)
        {
            return _narudzbe.FirstOrDefault(n => n.NarudzbaId == id);
        }

        // Metoda koja zadovoljava preduslove
        
        public string AnalizirajPerformanseNarudzbi(bool ukljuciVremenskaAnaliza = true, bool ukljuciFinansijskaAnaliza = true, bool detaljniIzvjestaj = true)
        {
            var izvjestaj = "═══════════════════════════════════════════════\n";
            izvjestaj += "      ANALIZA PERFORMANSI NARUDŽBI\n";
            izvjestaj += "═══════════════════════════════════════════════\n\n";

            // 1. Provjera da li postoje narudžbe
            if (_narudzbe.Count == 0)
            {
                return izvjestaj + "Nema narudžbi za analizu.\n";
            }

            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 100;

            // 2. OSNOVNA ANALIZA STATUSA (inline iz AnalizirajStatuseNarudzbi sa for petljom)
            int naCekanju = 0;
            int isporuceno = 0;
            int otkazano = 0;

            // Eksplicitna for petlja za brojanje po statusu
            for (int i = 0; i < _narudzbe.Count; i++)
            {
                var narudzba = _narudzbe[i];
                if (narudzba.Status == StatusNarudzbe.NA_CEKANJU)
                {
                    naCekanju++;
                }
                else if (narudzba.Status == StatusNarudzbe.ISPORUCENO)
                {
                    isporuceno++;
                }
                else if (narudzba.Status == StatusNarudzbe.OTKAZANO)
                {
                    otkazano++;
                }
            }

            izvjestaj += $"Ukupno narudžbi: {_narudzbe.Count}\n";
            izvjestaj += $"Na čekanju: {naCekanju}\n";
            izvjestaj += $"Isporučeno: {isporuceno}\n";
            izvjestaj += $"Otkazano: {otkazano}\n\n";

            // 3. Analiza narudžbi na čekanju
            if (naCekanju > 0)
            {
                var (cekanjuOcjena, cekanjuUpozorenja, cekanjuPreporuke) =
                    AnalizirajNarudzbeNaCekanju(naCekanju, _narudzbe.Count);
                ocjena += cekanjuOcjena;
                upozorenja.AddRange(cekanjuUpozorenja);
                preporuke.AddRange(cekanjuPreporuke);
            }

            // 4. Analiza otkazanih narudžbi
            if (otkazano > 0)
            {
                var (otkazanoOcjena, otkazanoUpozorenja, otkazanoPreporuke) =
                    AnalizirajOtkazaneNarudzbe(otkazano, _narudzbe.Count);
                ocjena += otkazanoOcjena;
                upozorenja.AddRange(otkazanoUpozorenja);
                preporuke.AddRange(otkazanoPreporuke);
            }

            // 5. Vremenska analiza
            if (ukljuciVremenskaAnaliza)
            {
                var (vremenskaIzvjestaj, vremenskaOcjena, vremenskaUpozorenja, vremenskaPreporuke) =
                    GenerirajVremenskuAnalizu();
                izvjestaj += vremenskaIzvjestaj;
                ocjena += vremenskaOcjena;
                upozorenja.AddRange(vremenskaUpozorenja);
                preporuke.AddRange(vremenskaPreporuke);
            }

            // 6. Finansijska analiza
            if (ukljuciFinansijskaAnaliza)
            {
                var (finansijskaIzvjestaj, finansijskaOcjena, finansijskaUpozorenja, finansijskaPreporuke) =
                    GenerirajFinansijskuAnalizu();
                izvjestaj += finansijskaIzvjestaj;
                ocjena += finansijskaOcjena;
                upozorenja.AddRange(finansijskaUpozorenja);
                preporuke.AddRange(finansijskaPreporuke);
            }

            // 7. Detaljna analiza
            if (detaljniIzvjestaj)
            {
                izvjestaj += GenerirajDetaljnuAnalizu(preporuke);
            }

            // 8. Provjera omjera isporučenih narudžbi
            if (isporuceno < _narudzbe.Count / 2)
            {
                upozorenja.Add("Manje od 50% narudžbi je isporučeno!");
                ocjena -= 10;
            }

            // 9. Konačni izvještaj
            izvjestaj += GenerirajKonacniIzvjestaj(ocjena, upozorenja, preporuke);

            return izvjestaj;
        }

        private (int naCekanju, int isporuceno, int otkazano, int ocjena) AnalizirajStatuseNarudzbi()
        {
            int naCekanju = _narudzbe.Count(n => n.Status == StatusNarudzbe.NA_CEKANJU);
            int isporuceno = _narudzbe.Count(n => n.Status == StatusNarudzbe.ISPORUCENO);
            int otkazano = _narudzbe.Count(n => n.Status == StatusNarudzbe.OTKAZANO);

            return (naCekanju, isporuceno, otkazano, 100);
        }

        private (int ocjena, List<string> upozorenja, List<string> preporuke) AnalizirajNarudzbeNaCekanju(int naCekanju, int ukupno)
        {
            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            double postotak = (naCekanju * 100.0) / ukupno;

            if (postotak > 60)
            {
                upozorenja.Add("KRITIČNO: Više od 60% narudžbi je na čekanju!");
                preporuke.Add("-> Hitno kontaktirajte dobavljače i provjerite statuse");
                ocjena = -40;
            }
            else if (postotak > 40)
            {
                upozorenja.Add("UPOZORENJE: Više od 40% narudžbi je na čekanju!");
                preporuke.Add("-> Razmotriti ubrzavanje procesa isporuke");
                ocjena = -25;
            }
            else if (postotak > 20)
            {
                upozorenja.Add("Povećan broj narudžbi na čekanju");
                ocjena = -10;
            }

            return (ocjena, upozorenja, preporuke);
        }

        private (int ocjena, List<string> upozorenja, List<string> preporuke) AnalizirajOtkazaneNarudzbe(int otkazano, int ukupno)
        {
            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            double postotak = (otkazano * 100.0) / ukupno;

            if (postotak > 30)
            {
                upozorenja.Add("KRITIČNO: Više od 30% narudžbi je otkazano!");
                preporuke.Add("-> Analizirajte razloge otkazivanja i promijenite strategiju nabavke");
                ocjena = -35;
            }
            else if (postotak > 15)
            {
                upozorenja.Add("Visoka stopa otkazivanja narudžbi!");
                preporuke.Add("-> Istražite probleme sa dobavljačima");
                ocjena = -20;
            }
            else if (postotak > 5)
            {
                upozorenja.Add("Povećana stopa otkazivanja");
                ocjena = -10;
            }

            return (ocjena, upozorenja, preporuke);
        }

        private (string izvjestaj, int ocjena, List<string> upozorenja, List<string> preporuke) GenerirajVremenskuAnalizu()
        {
            var izvjestaj = "\n--- Vremenska analiza ---\n";
            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            var starijeDatum = DateTime.Now.AddDays(-30);
            var stareNarudzbe = _narudzbe.Where(n =>
                n.Status == StatusNarudzbe.NA_CEKANJU &&
                n.DatumNarudzbe < starijeDatum).ToList();

            if (stareNarudzbe.Any())
            {
                izvjestaj += $"Narudžbe starije od 30 dana (na čekanju): {stareNarudzbe.Count}\n";
                upozorenja.Add($"{stareNarudzbe.Count} narudžbi čeka više od 30 dana!");
                preporuke.Add("-> Eskalirati stare narudžbe sa dobavljačima");
                ocjena = -15;

                var najstarija = stareNarudzbe.OrderBy(n => n.DatumNarudzbe).FirstOrDefault();
                if (najstarija != null)
                {
                    int danaStara = (DateTime.Now - najstarija.DatumNarudzbe).Days;
                    izvjestaj += $"Najstarija narudžba: {najstarija.NarudzbaId} ({danaStara} dana)\n";

                    if (danaStara > 60)
                    {
                        upozorenja.Add($"KRITIČNO: Narudžba {najstarija.NarudzbaId} čeka {danaStara} dana!");
                        ocjena -= 20;
                    }
                }
            }
            else
            {
                izvjestaj += "Sve narudžbe na čekanju su relativno nove (< 30 dana)\n";
                ocjena = 5;
            }

            return (izvjestaj, ocjena, upozorenja, preporuke);
        }

        private (string izvjestaj, int ocjena, List<string> upozorenja, List<string> preporuke) GenerirajFinansijskuAnalizu()
        {
            var izvjestaj = "\n--- Finansijska analiza ---\n";
            var upozorenja = new List<string>();
            var preporuke = new List<string>();
            int ocjena = 0;

            var finansijskiPodaci = IzracunajFinansijskePodatke();

            izvjestaj += $"Ukupna vrijednost svih narudžbi: {finansijskiPodaci.ukupna:F2} KM\n";
            izvjestaj += $"Vrijednost isporučenih: {finansijskiPodaci.isporuceno:F2} KM\n";
            izvjestaj += $"Vrijednost na čekanju: {finansijskiPodaci.naCekanju:F2} KM\n";
            izvjestaj += $"Vrijednost otkazanih: {finansijskiPodaci.otkazano:F2} KM\n";

            if (finansijskiPodaci.naCekanju > 0 && finansijskiPodaci.ukupna > 0)
            {
                double postotak = (finansijskiPodaci.naCekanju / finansijskiPodaci.ukupna) * 100;
                if (postotak > 50)
                {
                    upozorenja.Add($"{postotak:F1}% kapitala je zamrznuto u narudžbama na čekanju!");
                    preporuke.Add("-> Ubrzati proces isporuke da se oslobodi kapital");
                    ocjena = -15;
                }
            }

            if (finansijskiPodaci.otkazano > 0 && finansijskiPodaci.ukupna > 0)
            {
                double postotak = (finansijskiPodaci.otkazano / finansijskiPodaci.ukupna) * 100;
                if (postotak > 20)
                {
                    upozorenja.Add($"{postotak:F1}% ukupne vrijednosti je otkazano!");
                    preporuke.Add("-> Procijeniti financijski uticaj otkazanih narudžbi");
                    ocjena -= 10;
                }
            }

            return (izvjestaj, ocjena, upozorenja, preporuke);
        }

        private (double ukupna, double naCekanju, double isporuceno, double otkazano) IzracunajFinansijskePodatke()
        {
            double ukupna = 0, naCekanju = 0, isporuceno = 0, otkazano = 0;

            foreach (var narudzba in _narudzbe)
            {
                double cijena = narudzba.IzracunajUkupno();
                ukupna += cijena;

                switch (narudzba.Status)
                {
                    case StatusNarudzbe.NA_CEKANJU:
                        naCekanju += cijena;
                        break;
                    case StatusNarudzbe.ISPORUCENO:
                        isporuceno += cijena;
                        break;
                    case StatusNarudzbe.OTKAZANO:
                        otkazano += cijena;
                        break;
                }
            }

            return (ukupna, naCekanju, isporuceno, otkazano);
        }

        private string GenerirajDetaljnuAnalizu(List<string> preporuke)
        {
            var izvjestaj = "\n--- Analiza po dobavljačima ---\n";
            var grupePoDobavljacu = _narudzbe.GroupBy(n => n.DobavljacId);

            foreach (var grupa in grupePoDobavljacu)
            {
                int ukupno = grupa.Count();
                int otkazano = grupa.Count(n => n.Status == StatusNarudzbe.OTKAZANO);

                izvjestaj += $"Dobavljač {grupa.Key}: {ukupno} narudžbi";

                if (otkazano > 0)
                {
                    double postotak = (otkazano * 100.0) / ukupno;
                    izvjestaj += $" ({otkazano} otkazano - {postotak:F1}%)";

                    if (postotak > 50)
                    {
                        izvjestaj += " PROBLEMATIČAN";
                        preporuke.Add($"-> Razmotriti promjenu dobavljača {grupa.Key}");
                    }
                }
                izvjestaj += "\n";
            }

            izvjestaj += "\n--- Analiza po proizvodima ---\n";
            var najcesca = _narudzbe.GroupBy(n => n.ProizvodId)
                                    .OrderByDescending(g => g.Count())
                                    .FirstOrDefault();

            if (najcesca != null)
            {
                izvjestaj += $"Najčešće naručivani proizvod: {najcesca.Key} ({najcesca.Count()} narudžbi)\n";
            }

            return izvjestaj;
        }

        private string GenerirajKonacniIzvjestaj(int ocjena, List<string> upozorenja, List<string> preporuke)
        {
            var izvjestaj = "\n═══════════════════════════════════════════════\n";

            string performanseOcjena = OdrediPerformanseOcjenu(ocjena);
            izvjestaj += $"PERFORMANSE NARUDŽBI: {performanseOcjena} ({ocjena}/100)\n";
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

        private string OdrediPerformanseOcjenu(int ocjena) => ocjena switch
        {
            >= 90 => "ODLIČNE",
            >= 70 => "DOBRE",
            >= 50 => "ZADOVOLJAVAJUĆE",
            >= 30 => "LOŠE",
            _ => "KRITIČNE"
        };
    }
}
