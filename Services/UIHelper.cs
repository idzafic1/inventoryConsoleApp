using InventarApp.Models;
using InventarApp.Enums;

namespace InventarApp.Services
{
    public class UIHelper
    {
        public static void PrikaziNaslov(string naslov)
        {
            try
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            }
            catch
            {
                // Ako Clear ne radi, isprintaj prazne linije
                Console.WriteLine(new string('\n', 50));
            }
            
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  {naslov.PadRight(66)}║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
        }

        public static void PrikaziMeni(string naslov, string[] opcije)
        {
            PrikaziNaslov(naslov);

            for (int i = 0; i < opcije.Length; i++)
            {
                Console.WriteLine($"  [{i + 1}] {opcije[i]}");
            }

            Console.WriteLine($"  [0] Nazad");
            Console.WriteLine();
        }

        public static void PrikaziProizvode(List<Proizvod> proizvodi)
        {
            if (proizvodi.Count == 0)
            {
                Console.WriteLine("  Nema proizvoda za prikaz.");
                return;
            }

            Console.WriteLine(new string('═', 145));
            Console.WriteLine($"{"RB",4} {"ID",-20} {"Naziv",-25} {"Kategorija",-25} {"Količina",10} {"Min.Prag",10} {"Status",-15} {"Dobavljač ID",-15}");
            Console.WriteLine(new string('═', 145));

            int rb = 1;
            foreach (var p in proizvodi)
            {
                string status = p.Kolicina == 0 ? "NEMA" : (p.JeIspodMinimalnogPraga() ? "NISKO" : "OK");
                ConsoleColor boja = p.Kolicina == 0 ? ConsoleColor.Red : (p.JeIspodMinimalnogPraga() ? ConsoleColor.Yellow : ConsoleColor.Green);

                Console.Write($"{rb,4} {p.Id,-20} {p.Naziv,-25} {p.Kategorija,-25} {p.Kolicina,10} {p.MinimalniPrag,10} ");
                Console.ForegroundColor = boja;
                Console.Write($"{status,-15}");
                Console.ResetColor();
                Console.WriteLine($" {p.DobavljacId,-15}");
                rb++;
            }

            Console.WriteLine(new string('═', 145));
            Console.WriteLine($"Ukupno proizvoda: {proizvodi.Count}");
        }

        public static void PrikaziDobavljace(List<Dobavljac> dobavljaci)
        {
            if (dobavljaci.Count == 0)
            {
                Console.WriteLine("  Nema dobavljača za prikaz.");
                return;
            }

            Console.WriteLine(new string('═', 85));
            Console.WriteLine($"{"RB",4} {"ID",-15} {"Naziv",-30} {"Kontakt",-30}");
            Console.WriteLine(new string('═', 85));

            int rb = 1;
            foreach (var d in dobavljaci)
            {
                Console.WriteLine($"{rb,4} {d.Id,-15} {d.Naziv,-30} {d.Kontakt,-30}");
                rb++;
            }

            Console.WriteLine(new string('═', 85));
            Console.WriteLine($"Ukupno dobavljača: {dobavljaci.Count}");
        }

        public static void PrikaziNarudzbe(List<Narudzba> narudzbe)
        {
            if (narudzbe.Count == 0)
            {
                Console.WriteLine("  Nema narudžbi za prikaz.");
                return;
            }

            Console.WriteLine(new string('═', 120));
            Console.WriteLine($"{"ID Narudžbe",-25} {"Proizvod ID",-20} {"Količina",10} {"Status",-15} {"Datum",-15} {"Ukupno (KM)",15}");
            Console.WriteLine(new string('═', 120));

            foreach (var n in narudzbe)
            {
                Console.Write($"{n.NarudzbaId,-25} {n.ProizvodId,-20} {n.Kolicina,10} ");

                ConsoleColor boja = n.Status switch
                {
                    StatusNarudzbe.ISPORUCENO => ConsoleColor.Green,
                    StatusNarudzbe.OTKAZANO => ConsoleColor.Red,
                    _ => ConsoleColor.Yellow
                };

                Console.ForegroundColor = boja;
                Console.Write($"{n.Status,-15}");
                Console.ResetColor();

                Console.WriteLine($" {n.DatumNarudzbe:dd.MM.yyyy}   {n.IzracunajUkupno(),15:F2}");
            }

            Console.WriteLine(new string('═', 120));
            Console.WriteLine($"Ukupno narudžbi: {narudzbe.Count}");
        }

        public static void PrikaziObavjestenja(List<Obavjestenje> obavjestenja)
        {
            if (obavjestenja.Count == 0)
            {
                Console.WriteLine("  Nema obavještenja za prikaz.");
                return;
            }

            Console.WriteLine(new string('═', 130));
            Console.WriteLine($"{"ID",-25} {"Tip",-20} {"Proizvod ID",-20} {"Vrijeme",-20} {"Detalji",-40}");
            Console.WriteLine(new string('═', 130));

            foreach (var o in obavjestenja)
            {
                Console.Write($"{o.ObavjestenjeId,-25} ");

                ConsoleColor boja = o.Tip switch
                {
                    TipObavjestenja.NEMA_NA_STANJU => ConsoleColor.Red,
                    TipObavjestenja.NISKE_ZALIHE => ConsoleColor.Yellow,
                    _ => ConsoleColor.Cyan
                };

                Console.ForegroundColor = boja;
                Console.Write($"{o.Tip,-20}");
                Console.ResetColor();

                string kratkaPoruka = o.Poruka.Length > 40 ? o.Poruka.Substring(0, 37) + "..." : o.Poruka;
                Console.WriteLine($" {o.ProizvodId,-20} {o.VrijemeKreiranja:dd.MM. HH:mm}      {kratkaPoruka,-40}");
            }

            Console.WriteLine(new string('═', 130));
            Console.WriteLine($"Ukupno obavještenja: {obavjestenja.Count}");
        }

        public static string UcitajString(string poruka)
        {
            Console.Write($"{poruka}: ");
            return Console.ReadLine() ?? "";
        }

        public static int UcitajInt(string poruka)
        {
            while (true)
            {
                try
                {
                    Console.Write($"{poruka}: ");
                    return int.Parse(Console.ReadLine() ?? "");
                }
                catch
                {
                    Console.WriteLine("  ✗ Neispravan unos. Molimo unesite cijeli broj.");
                }
            }
        }

        // NOVO: Validacija da/ne unosa
        public static bool UcitajDaNe(string poruka)
        {
            while (true)
            {
                Console.Write($"{poruka} (da/ne): ");
                string unos = (Console.ReadLine() ?? "").ToLower().Trim();

                if (unos == "da")
                {
                    return true;
                }
                else if (unos == "ne")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("  X Neispravan unos. Molimo unesite 'da' ili 'ne'.");
                }
            }
        }

        public static void Pauza()
        {
            Console.WriteLine("\nPritisnite bilo koji taster za nastavak...");
            Console.ReadKey();
        }

        public static void PrikaziGresku(string poruka)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nX GRESKA: {poruka}");
            Console.ResetColor();
        }

        public static void PrikaziUspjeh(string poruka)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n√ {poruka}");
            Console.ResetColor();
        }
    }
}
