using FluentAssertions;
using InventarApp.Enums;
using InventarApp.Services;
using Moq;
using Xunit;

namespace InventarApp.Tests.Services
{
    public class NarudzbaServiceTests
    {
        private readonly Mock<HighTurnoverAnalyzer> _mockTurnoverAnalyzer;

        public NarudzbaServiceTests()
        {
            _mockTurnoverAnalyzer = new Mock<HighTurnoverAnalyzer>();
        }

        private NarudzbaService CreateService()
        {
            return new NarudzbaService(_mockTurnoverAnalyzer.Object);
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T01_Branch_NoOrders()
        {
            var service = CreateService();
            var result = service.AnalizirajPerformanseNarudzbi();
            result.Should().Contain("Nema narudžbi za analizu");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T02_Condition_Loop_Pending()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);
            
            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            result.Should().Contain("Na čekanju: 1");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T03_Condition_Loop_Delivered()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);
            var orders = service.PrikaziSveNarudzbe();
            var id = orders[0].NarudzbaId;
            service.OznaciKaoIsporuceno(id);

            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            result.Should().Contain("Isporučeno: 1");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T04_Condition_Loop_Cancelled()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);
            var orders = service.PrikaziSveNarudzbe();
            var id = orders[0].NarudzbaId;
            service.OtkaziNarudzbu(id);

            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            result.Should().Contain("Otkazano: 1");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T05_Branch_PendingSummary()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);

            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            result.Should().Contain("KRITIČNO: Više od 60% narudžbi je na čekanju");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T06_Branch_CancelledSummary()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);
            var orders = service.PrikaziSveNarudzbe();
            service.OtkaziNarudzbu(orders[0].NarudzbaId);

            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            result.Should().Contain("KRITIČNO: Više od 30% narudžbi je otkazano");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T07_Condition_TimeAnalysis()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);

            var result = service.AnalizirajPerformanseNarudzbi(ukljuciVremenskaAnaliza: true, false, false);
            result.Should().Contain("Vremenska analiza");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T08_Condition_FinancialAnalysis()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);

            var result = service.AnalizirajPerformanseNarudzbi(false, ukljuciFinansijskaAnaliza: true, false);
            result.Should().Contain("Finansijska analiza");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T09_Condition_DetailedReport()
        {
            var service = CreateService();
            service.KreirajNarudzbu("P1", "D1", 10);

            var result = service.AnalizirajPerformanseNarudzbi(false, false, detaljniIzvjestaj: true);
            result.Should().Contain("Analiza po dobavljačima");
        }

        // TEHNIKA: Condition Coverage, Boundary Value Analysis
        [Fact]
        public void T10_Condition_DeliveredRatio_Warning()
        {
            var service = CreateService();
            service.KreirajNarudzbu("Delivered1", "D1", 10); 
            var id1 = service.PrikaziSveNarudzbe()[0].NarudzbaId;
            service.OznaciKaoIsporuceno(id1);

            service.KreirajNarudzbu("Pending1", "D1", 10);
            service.KreirajNarudzbu("Pending2", "D1", 10);
            service.KreirajNarudzbu("Pending3", "D1", 10);

            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            result.Should().Contain("Manje od 50% narudžbi je isporučeno");
        }

        // TEHNIKA: Path Coverage
        [Fact]
        public void T11_Path_Base_Delivered_NoFlags()
        {
            var service = CreateService();
            service.KreirajNarudzbu("Del1", "D1", 10);
            service.KreirajNarudzbu("Del2", "D1", 10);
            var all = service.PrikaziSveNarudzbe();
            service.OznaciKaoIsporuceno(all[0].NarudzbaId);
            service.OznaciKaoIsporuceno(all[1].NarudzbaId);
            
            var result = service.AnalizirajPerformanseNarudzbi(false, false, false);
            
            result.Should().Contain("Isporučeno: 2");
            result.Should().NotContain("UPOZORENJA");
            result.Should().NotContain("Vremenska analiza");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T12_Condition_Financial_FrozenCapital()
        {
            var service = CreateService();
            service.KreirajNarudzbu("BigPending", "D1", 100);
            
            service.KreirajNarudzbu("SmallDelivered", "D1", 10);
            var all = service.PrikaziSveNarudzbe();
            service.OznaciKaoIsporuceno(all[1].NarudzbaId);
            
            var result = service.AnalizirajPerformanseNarudzbi(false, ukljuciFinansijskaAnaliza: true, false);
            result.Should().Contain("kapitala je zamrznuto");
        }
    }
}
