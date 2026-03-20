using InventarApp.Interfaces;
using System.Text.RegularExpressions;

namespace InventarApp.Models
{
    public class Dobavljac : IDobavljac
    {
        public string Id { get; set; }
        public string Naziv { get; set; }
        public string Kontakt { get; set; }

        private static int _brojac = 0;

        public Dobavljac(string naziv, string kontakt)
        {
            if (string.IsNullOrWhiteSpace(naziv))
                throw new ArgumentException("Naziv dobavljača ne može biti prazan.");

            if (string.IsNullOrWhiteSpace(kontakt))
                throw new ArgumentException("Kontakt informacije ne mogu biti prazne.");

            if (!ValidirajKontakt(kontakt))
                throw new ArgumentException("Kontakt mora biti validan email ili telefonski broj.");

            Naziv = naziv;
            Kontakt = kontakt;
            Id = GenerisiId();
        }

        private string GenerisiId()
        {
            _brojac++;
            return $"Dobavljac-{_brojac}";
        }

        // Složeni algoritam - Validacija kontakta (email ili telefon) koristeći Regex
        private bool ValidirajKontakt(string kontakt)
        {
            // Regex pattern za email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Regex pattern za telefon (različiti formati)
            // Podržava: +387XX XXX XXX, 061/XXX-XXX, 061XXXXXX, itd.
            string telefonPattern = @"^(\+?\d{1,3}[\s-]?)?\(?\d{2,3}\)?[\s.-]?\d{3}[\s.-]?\d{3,4}$";

            bool jeEmail = Regex.IsMatch(kontakt, emailPattern);
            bool jeTelefon = Regex.IsMatch(kontakt, telefonPattern);

            return jeEmail || jeTelefon;
        }

        public string GetDobavljacInfo()
        {
            string tipKontakta = OdrediTipKontakta(Kontakt);
            return $"Dobavljač: {Naziv} (ID: {Id})\nKontakt ({tipKontakta}): {Kontakt}";
        }

        private string OdrediTipKontakta(string kontakt)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (Regex.IsMatch(kontakt, emailPattern))
                return "Email";
            else
                return "Telefon";
        }

        // Složeni algoritam - Rangiranje kvaliteta kontakt informacija sa McCabe Complexity >= 8
        // Analizira različite aspekte kontakt informacija i dodeljuje rejting
        // Složeni algoritam - Rangiranje kvaliteta kontakt informacija sa McCabe Complexity >= 8
        // Složeni algoritam - Rangiranje kvaliteta kontakt informacija sa McCabe Complexity >= 8
        // Složeni algoritam - Rangiranje kvaliteta kontakt informacija sa McCabe Complexity = 8
        public string AnalizirajKvalitetKontakta()
        {
            int bodovi = 0;
            string detalji = "";
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            string telefonPattern = @"^(\+?\d{1,3}[\s-]?)?\(?\d{2,3}\)?[\s.-]?\d{3}[\s.-]?\d{3,4}$";

            bool jeEmail = Regex.IsMatch(Kontakt, emailPattern);
            bool jeTelefon = Regex.IsMatch(Kontakt, telefonPattern);

            // 1. Analiza tipa kontakta
            if (jeEmail)
            {
                bodovi += 10;
                detalji += "Email format detektovan. ";

                string domena = Kontakt.Split('@')[1];
                if (domena.Contains("gmail") || domena.Contains("outlook"))
                {
                    bodovi += 3;
                    detalji += "Popularan email provajder. ";
                }
                else
                {
                    bodovi += 7;
                    detalji += "Korporativna email adresa. ";
                }
            }
            else if (jeTelefon)
            {
                bodovi += 8;
                detalji += "Telefonski broj detektovan. ";

                if (Kontakt.StartsWith("+387"))
                {
                    bodovi += 10;
                    detalji += "BiH međunarodni format. ";
                }
                else if (Kontakt.StartsWith("+"))
                {
                    bodovi += 7;
                    detalji += "Međunarodni format. ";
                }
            }

            // 2. Provjera naziva dobavljača
            if (!string.IsNullOrWhiteSpace(Naziv) && Naziv.Length > 5)
            {
                bodovi += 3;
                detalji += "Detaljan naziv dobavljača. ";
            }

            // 3. Određivanje rejtinga (switch expression ne dodaje complexity)
            string rejting = bodovi switch
            {
                >= 25 => "ODLIČAN",
                >= 18 => "VRLO DOBAR",
                >= 12 => "DOBAR",
                _ => "ZADOVOLJAVAJUĆI"
            };

            return $"Kvalitet kontakta: {rejting} ({bodovi} bodova)\nDetalji: {detalji}";
        }
        public override string ToString()
        {
            return $"ID: {Id}, Naziv: {Naziv}, Kontakt: {Kontakt}";
        }
    }
}
