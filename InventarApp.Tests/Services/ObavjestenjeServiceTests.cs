using FluentAssertions;
using InventarApp.Enums;
using InventarApp.Models;
using InventarApp.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace InventarApp.Tests.Services
{
    public class ObavjestenjeServiceTests
    {
        private readonly ObavjestenjeService _service;

        public ObavjestenjeServiceTests()
        {
            _service = new ObavjestenjeService();
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T01_Branch_NoNotifications()
        {
            var result = _service.AnalizirajObavjestenja();
            result.Should().Contain("Nema aktivnih obavještenja");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T02_Condition_Loop_OutOfStock()
        {
            var proizvod = new Proizvod("P1", 0, 5, "D1", KategorijaProizvoda.ALATI);
            _service.KreirajObavjestenje(proizvod);

            var result = _service.AnalizirajObavjestenja(false, false, false);
            result.Should().Contain("Nema na stanju: 1");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T03_Condition_Loop_LowStock()
        {
            var proizvod = new Proizvod("P1", 2, 5, "D1", KategorijaProizvoda.ALATI);
            _service.KreirajObavjestenje(proizvod);

            var result = _service.AnalizirajObavjestenja(false, false, false);
            result.Should().Contain("Niske zalihe: 1");
        }

        // TEHNIKA: Condition Coverage, Loop Testing
        [Fact]
        public void T04_Condition_Loop_OrderRequired()
        {
            _service.KreirajObavjestenjeZaNarudzbu("P1", "Order needed");

            var result = _service.AnalizirajObavjestenja(false, false, false);
            result.Should().Contain("Potrebna narudžba: 1");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T05_Branch_CriticalSummary()
        {
            var proizvod = new Proizvod("P1", 0, 5, "D1", KategorijaProizvoda.ALATI);
            _service.KreirajObavjestenje(proizvod);

            var result = _service.AnalizirajObavjestenja(false, false, false);
            result.Should().Contain("obavještenja je za proizvode bez zaliha");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T06_Branch_MediumPrioritySummary()
        {
            var proizvod = new Proizvod("P1", 2, 5, "D1", KategorijaProizvoda.ALATI);
            _service.KreirajObavjestenje(proizvod);

            var result = _service.AnalizirajObavjestenja(false, false, false);
            result.Should().Contain("proizvoda sa niskim zalihama");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T07_Condition_TimeAnalysis()
        {
            _service.KreirajObavjestenjeZaNarudzbu("P1", "Test");
            var result = _service.AnalizirajObavjestenja(ukljuciVremenskaAnaliza: true, false, false);
            result.Should().Contain("Vremenska analiza");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T08_Condition_Statistics()
        {
            _service.KreirajObavjestenjeZaNarudzbu("P1", "Test");
            var result = _service.AnalizirajObavjestenja(false, ukljuciStatistiku: true, false);
            result.Should().Contain("Statistička analiza");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T09_Condition_ActionPlan()
        {
            _service.KreirajObavjestenjeZaNarudzbu("P1", "Test");
            var result = _service.AnalizirajObavjestenja(false, false, generirajAkcioniPlan: true);
            result.Should().Contain("Akcioni plan");
        }

        // TEHNIKA: Condition Coverage, Boundary Value Analysis
        [Fact]
        public void T10_Condition_UrgentRatio_Warning()
        {
            _service.KreirajObavjestenje(new Proizvod("P1", 0, 5, "D1", KategorijaProizvoda.ALATI));
            _service.KreirajObavjestenje(new Proizvod("P2", 2, 5, "D1", KategorijaProizvoda.ALATI));

            var result = _service.AnalizirajObavjestenja(false, false, false);
            result.Should().Contain("Više od 50% obavještenja zahtijeva hitnu akciju");
        }

        // TEHNIKA: Path Coverage
        [Fact]
        public void T11_Path_Base_NoFlags_NoCritical()
        {
            _service.KreirajObavjestenjeZaNarudzbu("P1", "Normal");
            _service.KreirajObavjestenjeZaNarudzbu("P2", "Normal");

            var result = _service.AnalizirajObavjestenja(false, false, false);
            
            result.Should().NotContain("Vremenska analiza");
            result.Should().NotContain("Statistička analiza");
            result.Should().NotContain("Akcioni plan");
            result.Should().NotContain("hitnu akciju");
            result.Should().Contain("Potrebna narudžba: 2");
        }

        // TEHNIKA: Logic Coverage
        [Fact]
        public void T12_Condition_Time_OldNotifications()
        {
            _service.KreirajObavjestenjeZaNarudzbu("Old", "Outdated");
            var all = _service.PrikaziSvaObavjestenja();
            all[0].VrijemeKreiranja = DateTime.Now.AddDays(-10);

            var result = _service.AnalizirajObavjestenja(ukljuciVremenskaAnaliza: true, false, false);
            
            result.Should().Contain("KRITIČNO: 1 obavještenja nije riješeno više od 7 dana");
        }
    }
}
