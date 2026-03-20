using Xunit;
using FluentAssertions;
using InventarApp.Services;

namespace InventarApp.Tests
{
    public class CurrencyServiceTests
    {
        // TEST 1: Konverzija BAM → EUR
        [Fact]
        public void KonvertujValutu_BamUEur_VracaTacnuKonverziju()
        {
            // Arrange
            var service = new CurrencyService();
            double iznosUBam = 195.583;

            // Act
            double rezultat = service.KonvertujValutu(iznosUBam, "BAM", "EUR");

            // Assert
            rezultat.Should().BeApproximately(100.0, 0.01); // 195.583 BAM = 100 EUR
        }

        // TEST 2: Konverzija EUR → BAM
        [Fact]
        public void KonvertujValutu_EurUBam_VracaTacnuKonverziju()
        {
            // Arrange
            var service = new CurrencyService();
            double iznosUEur = 100.0;

            // Act
            double rezultat = service.KonvertujValutu(iznosUEur, "EUR", "BAM");

            // Assert
            rezultat.Should().BeApproximately(195.58, 0.01); // 100 EUR = 195.583 BAM
        }

        // TEST 3: Konverzija iste valute BAM → BAM (edge case)
        [Fact]
        public void KonvertujValutu_IstaValuta_VracaIstiIznos()
        {
            // Arrange
            var service = new CurrencyService();
            double iznos = 150.50;

            // Act
            double rezultat = service.KonvertujValutu(iznos, "BAM", "BAM");

            // Assert
            rezultat.Should().Be(150.50);
        }

        // TEST 4: Negativan iznos → exception
        [Fact]
        public void KonvertujValutu_NegativanIznos_BacaException()
        {
            // Arrange
            var service = new CurrencyService();

            // Act
            Action act = () => service.KonvertujValutu(-100, "BAM", "EUR");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Iznos ne može biti negativan.");
        }

        // TEST 5: Nevažeća valuta → exception
        [Fact]
        public void KonvertujValutu_NevazeceValute_BacaException()
        {
            // Arrange
            var service = new CurrencyService();

            // Act
            Action act = () => service.KonvertujValutu(100, "BAM", "GBP");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Nepodržana konverzija valuta: BAM → GBP");
        }

        // TEST 6: USD → BAM konverzija
        [Fact]
        public void KonvertujValutu_UsdUBam_VracaTacnuKonverziju()
        {
            // Arrange
            var service = new CurrencyService();
            double iznosUUsd = 100.0;

            // Act
            double rezultat = service.KonvertujValutu(iznosUUsd, "USD", "BAM");

            // Assert
            rezultat.Should().BeApproximately(185.0, 0.01); // 100 USD = 185 BAM (1 USD = 1.85 BAM)
        }
    }
}
