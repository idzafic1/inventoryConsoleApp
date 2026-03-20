namespace InventarApp.Services
{
    public class CurrencyService
    {
        private const double BAM_TO_EUR_RATE = 1.95583;
        private const double BAM_TO_USD_RATE = 1.85;

        public double KonvertujValutu(double iznos, string izValute, string uValutu)
        {
            // Validacija: negativan iznos
            if (iznos < 0)
            {
                throw new ArgumentException("Iznos ne može biti negativan.");
            }

            // Konverzija iste valute
            if (izValute == uValutu)
            {
                return iznos;
            }

            // BAM → EUR
            if (izValute == "BAM" && uValutu == "EUR")
            {
                return Math.Round(iznos / BAM_TO_EUR_RATE, 2);
            }

            // EUR → BAM
            if (izValute == "EUR" && uValutu == "BAM")
            {
                return Math.Round(iznos * BAM_TO_EUR_RATE, 2);
            }

            // BAM → USD
            if (izValute == "BAM" && uValutu == "USD")
            {
                return Math.Round(iznos / BAM_TO_USD_RATE, 2);
            }

            // USD → BAM
            if (izValute == "USD" && uValutu == "BAM")
            {
                return Math.Round(iznos * BAM_TO_USD_RATE, 2);
            }

            // EUR → USD (via BAM)
            if (izValute == "EUR" && uValutu == "USD")
            {
                double uBam = iznos * BAM_TO_EUR_RATE;
                return Math.Round(uBam / BAM_TO_USD_RATE, 2);
            }

            // USD → EUR (via BAM)
            if (izValute == "USD" && uValutu == "EUR")
            {
                double uBam = iznos * BAM_TO_USD_RATE;
                return Math.Round(uBam / BAM_TO_EUR_RATE, 2);
            }

            // Nepodržana konverzija
            throw new ArgumentException($"Nepodržana konverzija valuta: {izValute} → {uValutu}");
        }
    }
}
