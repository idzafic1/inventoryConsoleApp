using FluentAssertions;
using InventarApp.Models;
using InventarApp.Services;
using Xunit;

namespace InventarApp.Tests.Services
{
    public class DobavljacServiceTests
    {
        // TEHNIKA: Branch Coverage
        [Fact]
        public void T01_Branch_DobavljacNull()
        {
            var service = new DobavljacService();
            var result = service.AnalizirajDobavljaca("999");
            result.Should().Be("GREŠKA: Dobavljač nije pronađen.");
        }

        // TEHNIKA: Condition Coverage, Boundary Value Analysis
        [Fact]
        public void T02_Condition_NazivShort()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "test@test.com");
            var dobavljac = service.PronadjiDobavljacaPoNazivu("Valid");
            
            dobavljac!.Naziv = "Ab"; 

            var result = service.AnalizirajDobavljaca(dobavljac.Id);
            result.Should().Contain("Naziv je prekratak");
        }

        // TEHNIKA: Condition Coverage, Boundary Value Analysis
        [Fact]
        public void T03_Condition_NazivLong()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "test@test.com");
            var dobavljac = service.PronadjiDobavljacaPoNazivu("Valid");
            
            dobavljac!.Naziv = new string('A', 51);

            var result = service.AnalizirajDobavljaca(dobavljac.Id);
            result.Should().Contain("Naziv je predugačak");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T04_Branch_ContactEmpty()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "test@test.com");
            var dobavljac = service.PronadjiDobavljacaPoNazivu("Valid");
            
            dobavljac!.Kontakt = "";

            var result = service.AnalizirajDobavljaca(dobavljac.Id);
            result.Should().Contain("Kontakt informacija nedostaje");
        }

        // TEHNIKA: Path Coverage
        [Fact]
        public void T05_Path_ContactEmail()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "test@email.com");
            var id = service.PronadjiDobavljacaPoNazivu("Valid")!.Id;

            var result = service.AnalizirajDobavljaca(id);
            result.Should().Contain("Tip kontakta: Email");
        }

        // TEHNIKA: Path Coverage
        [Fact]
        public void T06_Path_ContactPhone()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "061-123-456");
            var id = service.PronadjiDobavljacaPoNazivu("Valid")!.Id;

            var result = service.AnalizirajDobavljaca(id);
            result.Should().Contain("Tip kontakta: Telefon");
        }

        // TEHNIKA: Branch Coverage
        [Fact]
        public void T07_Branch_ContactUnknown()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "test@test.com");
            var dobavljac = service.PronadjiDobavljacaPoNazivu("Valid");
            
            dobavljac!.Kontakt = "Nepoznato"; 

            var result = service.AnalizirajDobavljaca(dobavljac.Id);
            result.Should().Contain("Neprepoznat format kontakta");
        }

        // TEHNIKA: Loop Coverage
        [Fact]
        public void T08_Loop_DuplicatesCheck_NoMatch()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("A", "test@a.com");
            service.DodajDobavljaca("B", "test@b.com");
            service.DodajDobavljaca("Target", "test@target.com");
            
            var id = service.PronadjiDobavljacaPoNazivu("Target")!.Id;
            var result = service.AnalizirajDobavljaca(id);
            
            result.Should().NotContain("Pronađen duplikat naziva");
        }

        // TEHNIKA: Loop Coverage
        [Fact]
        public void T09_Loop_DuplicatesCheck_MatchFound()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Duplikat", "test@a.com");
            service.DodajDobavljaca("Duplikat", "test@b.com");
            
            var dobavljac = service.PrikaziSveDobavljace().Last();
            
            var result = service.AnalizirajDobavljaca(dobavljac.Id);
            result.Should().Contain("Pronađen duplikat naziva");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T10_Condition_QualityCheck_High()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("HighQual", "test@gmail.com"); 
            
            var id = service.PronadjiDobavljacaPoNazivu("HighQual")!.Id;
            var result = service.AnalizirajDobavljaca(id);
            
            result.Should().Contain("Kvalitet kontakta: Visok");
        }

        // TEHNIKA: Condition Coverage
        [Fact]
        public void T11_Condition_QualityCheck_Medium()
        {
            var service = new DobavljacService();
            service.DodajDobavljaca("Valid", "061-123-456");
            var dobavljac = service.PronadjiDobavljacaPoNazivu("Valid");
            
            dobavljac!.Naziv = "Ab";
            dobavljac.Kontakt = "060-123-456";
            
            var result = service.AnalizirajDobavljaca(dobavljac.Id);
            result.Should().Contain("Kvalitet kontakta: Srednji");
        }

        // TEHNIKA: Exception Handling
        [Fact]
        public void T12_Exception_ForceQualityWarning()
        {
           var service = new DobavljacService();
           service.DodajDobavljaca("Valid", "061-123-456");
           var dobavljac = service.PronadjiDobavljacaPoNazivu("Valid");
            
           dobavljac!.Kontakt = null!; 

           var result = service.AnalizirajDobavljaca(dobavljac.Id);
            
           result.Should().Contain("Greška pri analizi kvaliteta kontakta");
        }
    }
}
