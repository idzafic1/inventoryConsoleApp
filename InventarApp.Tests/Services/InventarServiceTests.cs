using FluentAssertions;
using InventarApp.Enums;
using InventarApp.Services;
using Moq;
using Xunit;

namespace InventarApp.Tests.Services
{
    public class InventarServiceTests
    {
        private readonly Mock<ObavjestenjeService> _mockObavjestenjeService;
        private readonly Mock<HighTurnoverAnalyzer> _mockTurnoverAnalyzer;

        public InventarServiceTests()
        {
            _mockObavjestenjeService = new Mock<ObavjestenjeService>();
            _mockTurnoverAnalyzer = new Mock<HighTurnoverAnalyzer>();
        }

        private InventarService CreateService()
        {
            return new InventarService(_mockObavjestenjeService.Object, _mockTurnoverAnalyzer.Object);
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T01_Branch_EmptyInventory()
        {
            var service = CreateService();
            var result = service.AnalizirajStanjeInventara();
            result.Should().Contain("Inventar je prazan");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T02_Condition_Loop_CriticalItem()
        {
            var service = CreateService();
            service.DodajProizvod("Crit", 0, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            result.Should().Contain("Kritičnih zaliha (0): 1");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T03_Condition_Loop_LowStockItem()
        {
            var service = CreateService();
            service.DodajProizvod("Low", 2, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            result.Should().Contain("Niskih zaliha: 1");
        }

        // TEHNIKA: Path Coverage, Loop Testing
        [Fact]
        public void T04_Condition_Loop_HealthyItem()
        {
            var service = CreateService();
            service.DodajProizvod("Good", 10, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            result.Should().Contain("Zdravih zaliha: 1");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T05_Branch_CriticalSummary()
        {
            var service = CreateService();
            service.DodajProizvod("Crit", 0, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            result.Should().Contain("bez zaliha!");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T06_Branch_LowStockSummary()
        {
            var service = CreateService();
            service.DodajProizvod("Low", 2, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            result.Should().Contain("ima niske zalihe");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T07_Condition_IncludeCategories()
        {
            var service = CreateService();
            service.DodajProizvod("P1", 10, 5, "D1", KategorijaProizvoda.IT_UREDJAJI);
            
            var result = service.AnalizirajStanjeInventara(ukljuciKategorije: true, false, false);
            result.Should().Contain("Analiza po kategorijama");
            result.Should().Contain("IT_UREDJAJI");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T08_Condition_IncludeSuppliers()
        {
            var service = CreateService();
            service.DodajProizvod("P1", 10, 5, "DobavljacXYZ", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, ukljuciDobavljace: true, false);
            result.Should().Contain("Analiza po dobavljačima");
            result.Should().Contain("Dobavljač DobavljacXYZ");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T09_Condition_DetailedAnalysis()
        {
            var service = CreateService();
            service.DodajProizvod("P1", 10, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, detaljnaAnaliza: true);
            result.Should().Contain("Detaljne metrike");
            result.Should().Contain("Prosječna količina");
        }

        // TEHNIKA: Condition Coverage, Boundary Value Analysis
        [Fact]
        public void T10_Condition_HealthyRatio_Warning()
        {
            var service = CreateService();
            service.DodajProizvod("Bad1", 0, 5, "D1", KategorijaProizvoda.ALATI);
            service.DodajProizvod("Bad2", 0, 5, "D1", KategorijaProizvoda.ALATI);
            service.DodajProizvod("Bad3", 0, 5, "D1", KategorijaProizvoda.ALATI);
            service.DodajProizvod("Good", 10, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            result.Should().Contain("Manje od 50% proizvoda ima zdrave zalihe");
        }

        // TEHNIKA: Path Coverage
        [Fact]
        public void T11_Path_Minimal_Healthy_NoFlags()
        {
            var service = CreateService();
            service.DodajProizvod("Good1", 10, 5, "D1", KategorijaProizvoda.ALATI);
            service.DodajProizvod("Good2", 10, 5, "D1", KategorijaProizvoda.ALATI);
            
            var result = service.AnalizirajStanjeInventara(false, false, false);
            
            result.Should().Contain("Zdravih zaliha: 2");
            result.Should().NotContain("UPOZORENJA");
            result.Should().NotContain("Analiza po");
        }
    }
}
