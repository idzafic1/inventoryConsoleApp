using Xunit;
using FluentAssertions;
using InventarApp.Services;

namespace InventarApp.Tests
{
    public class HighTurnoverAnalyzerTests
    {
        // TEST 1: Registracija promjene količine
        [Fact]
        public void RegistrujPromjenu_ValidniPodaci_UspjesnoRegistruje()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();
            string proizvodId = "Proizvod-1";

            // Act
            analyzer.RegistrujPromjenu(proizvodId, 10, DateTime.Now);

            // Assert - nema exception
        }

        // TEST 2: Izračunavanje turnover-a za proizvod
        [Fact]
        public void IzracunajTurnover_ProizvodSaPromjenama_VracaTacanTurnover()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();
            string proizvodId = "Proizvod-1";

            // Registrujemo promjene unutar zadnjih 10 dana
            analyzer.RegistrujPromjenu(proizvodId, 50, DateTime.Now.AddDays(-2));
            analyzer.RegistrujPromjenu(proizvodId, 30, DateTime.Now.AddDays(-5));
            analyzer.RegistrujPromjenu(proizvodId, 20, DateTime.Now.AddDays(-8));

            // Act - turnover za zadnjih 10 dana
            double turnover = analyzer.IzracunajTurnover(proizvodId, 10);

            // Assert - ukupna promjena = 100, za 10 dana = 10.0/dan
            turnover.Should().BeApproximately(10.0, 0.1);
        }

        // TEST 3: Turnover za proizvod bez historije
        [Fact]
        public void IzracunajTurnover_ProizvodBezHistorije_Vraca0()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();

            // Act
            double turnover = analyzer.IzracunajTurnover("Nepostojeci-1", 7);

            // Assert
            turnover.Should().Be(0);
        }

        // TEST 4: Identifikacija high-turnover proizvoda
        [Fact]
        public void DaLiJeHighTurnover_VisokaAktivnost_VracaTrue()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();
            string proizvodId = "Proizvod-1";

            // Simuliramo veliku aktivnost
            for (int i = 0; i < 10; i++)
            {
                analyzer.RegistrujPromjenu(proizvodId, 20, DateTime.Now.AddDays(-i));
            }

            // Act - prag je 15 promjena/dan
            bool jeHighTurnover = analyzer.DaLiJeHighTurnover(proizvodId, 10, 15.0);

            // Assert
            jeHighTurnover.Should().BeTrue();
        }

        // TEST 5: Top N proizvoda po turnover-u
        [Fact]
        public void PronadjiTopProizvode_VisetProizvoda_VracaSortiranuListu()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();

            // Proizvod 1 - najaktivniji
            for (int i = 0; i < 10; i++)
            {
                analyzer.RegistrujPromjenu("Proizvod-1", 50, DateTime.Now.AddDays(-i));
            }

            // Proizvod 2 - srednja aktivnost
            for (int i = 0; i < 10; i++)
            {
                analyzer.RegistrujPromjenu("Proizvod-2", 20, DateTime.Now.AddDays(-i));
            }

            // Proizvod 3 - niska aktivnost
            analyzer.RegistrujPromjenu("Proizvod-3", 10, DateTime.Now.AddDays(-2));

            // Act
            var topProizvodi = analyzer.PronadjiTopProizvode(10, 3);

            // Assert
            topProizvodi.Should().HaveCount(3);
            topProizvodi[0].Id.Should().Be("Proizvod-1"); // Najveći turnover
            topProizvodi[1].Id.Should().Be("Proizvod-2");
            topProizvodi[2].Id.Should().Be("Proizvod-3");
        }

        // TEST 6: Čišćenje stare historije
        [Fact]
        public void OcistiHistoriju_StarePromjene_UklanjaSveStarijeOdPraga()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();

            // Dodaj stare promjene (40 dana)
            analyzer.RegistrujPromjenu("Proizvod-1", 10, DateTime.Now.AddDays(-40));

            // Dodaj nove promjene (5 dana)
            analyzer.RegistrujPromjenu("Proizvod-1", 20, DateTime.Now.AddDays(-5));

            // Act - očisti sve starije od 30 dana
            analyzer.OcistiHistoriju(30);

            // Assert - turnover bi trebao biti baziran samo na novim promjenama
            double turnover = analyzer.IzracunajTurnover("Proizvod-1", 10);
            turnover.Should().BeApproximately(2.0, 0.1); // 20/10 dana = 2.0
        }

        // TEST 7: Negativna promjena (prodaja) takođe se prati
        [Fact]
        public void RegistrujPromjenu_NegativnaPromjena_UspjesnoRegistruje()
        {
            // Arrange
            var analyzer = new HighTurnoverAnalyzer();

            // Act - negativna promjena (prodaja)
            analyzer.RegistrujPromjenu("Proizvod-1", -30, DateTime.Now);

            // Turnover prati apsolutnu vrijednost promjene
            double turnover = analyzer.IzracunajTurnover("Proizvod-1", 1);

            // Assert
            turnover.Should().BeApproximately(30.0, 0.1);
        }
    }
}
