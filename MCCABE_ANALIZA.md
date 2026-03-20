# McCabe Cyclomatic Complexity - Ručna analiza

## Zahtjev projekta
Svaka funkcionalnost mora imati **barem jednu metodu sa McCabe Complexity >= 8**

## Analiza složenih algoritama

### 1. PROIZVOD.CS - IzracunajPrioritetNabavke()

**Decision Points:**
1. `if (Kategorija == IT_UREDJAJI || Kategorija == ALATI)` → +2 (if + ||)
2. `if (Kolicina == 0)` → +1
3. `if (kriticnaKategorija)` → +1
4. `else if (procenatZaliha < 50)` → +1
5. `if (kriticnaKategorija && Kolicina < 5)` → +2 (if + &&)
6. `else if (kriticnaKategorija)` → +1
7. `else if (Kolicina < 3)` → +1
8. `else if (procenatZaliha < 100)` → +1
9. `if (kriticnaKategorija)` → +1
10. `if (procenatZaliha > 200)` → +1
11. `else if (procenatZaliha > 150)` → +1
12. Ternary operator `? :` → +1

**Total: 14 decision points + 1 = 15**
**Status: ✅ ZADOVOLJAVA (>= 8)**

---

### 2. DOBAVLJAC.CS - AnalizirajKvalitetKontakta()

**Decision Points:**
1. `if (jeEmail)` → +1
2. `if (domena.EndsWith(".com") || ... || ...)` → +3 (if + 2x ||)
3. `if (domena.Contains(...) || ... || ...)` → +3 (if + 2x ||)
4. `if (Kontakt.Contains(".") && Kontakt.Contains("@"))` → +2 (if + &&)
5. `if (dijelovi.Length >= 2)` → +1
6. `else if (jeTelefon)` → +1
7. `if (Kontakt.StartsWith("+"))` → +1
8. `if (Kontakt.StartsWith("+387"))` → +1
9. `if (Kontakt.Contains(" ") || ... || ...)` → +3 (if + 2x ||)
10. `if (samoBrojevi.Length >= 9 && samoBrojevi.Length <= 13)` → +2 (if + &&)
11. `if (!string.IsNullOrWhiteSpace(Naziv) && Naziv.Length > 5)` → +2 (if + &&)
12. `if (bodovi >= 30)` → +1
13. `else if (bodovi >= 20)` → +1
14. `else if (bodovi >= 15)` → +1
15. `else if (bodovi >= 10)` → +1

**Total: 24 decision points + 1 = 25**
**Status: ✅ ZADOVOLJAVA (>= 8)**

---

### 3. NARUDZBA.CS - ProcijeniVrijemeIsporuke()

**Decision Points:**
1. `if (Kolicina >= 100)` → +1
2. `else if (Kolicina >= 50)` → +1
3. `else if (Kolicina >= 20)` → +1
4. `if (hitnaIsporuka)` → +1
5. `if (daniIsporuke > 3)` → +1
6. `if (!domacaDobavljac)` → +1
7. `if (Kolicina >= 50)` → +1
8. `if (danNarudzbe == DayOfWeek.Friday)` → +1
9. `else if (danNarudzbe == ... || danNarudzbe == ...)` → +2 (else if + ||)
10. `if (Status == StatusNarudzbe.OTKAZANO)` → +1
11. `else if (Status == StatusNarudzbe.ISPORUCENO)` → +1
12. `if (procijenjeniDatum.DayOfWeek == DayOfWeek.Saturday)` → +1
13. `else if (procijenjeniDatum.DayOfWeek == DayOfWeek.Sunday)` → +1
14. `if (daniIsporuke <= 2)` → +1
15. `else if (daniIsporuke <= 5)` → +1
16. `else if (daniIsporuke <= 10)` → +1

**Total: 17 decision points + 1 = 18**
**Status: ✅ ZADOVOLJAVA (>= 8)**

---

### 4. OBAVJESTENJE.CS - AnalizirajUrgentnost()

**Decision Points:**
1. `if (Tip == TipObavjestenja.NEMA_NA_STANJU)` → +1
2. `else if (Tip == TipObavjestenja.NISKE_ZALIHE)` → +1
3. `else if (Tip == TipObavjestenja.POTREBNA_NARUDBA)` → +1
4. `if (vrijemeOdKreiranja.TotalHours < 1)` → +1
5. `else if (vrijemeOdKreiranja.TotalHours < 24)` → +1
6. `else if (vrijemeOdKreiranja.TotalDays < 3)` → +1
7. `else if (vrijemeOdKreiranja.TotalDays >= 7)` → +1
8. `if (porukaMala.Contains("hitno") || porukaMala.Contains("kritično"))` → +2 (if + ||)
9. `if (porukaMala.Contains("0") || porukaMala.Contains("nula"))` → +2 (if + ||)
10. `if (porukaMala.Contains("upozorenje") || porukaMala.Contains("pažnja"))` → +2 (if + ||)
11. `if (sat >= 9 && sat <= 17)` → +2 (if + &&)
12. `else if (sat >= 18 && sat <= 22)` → +2 (else if + &&)
13. `if (VrijemeKreiranja.DayOfWeek == DayOfWeek.Saturday || ... == DayOfWeek.Sunday)` → +2 (if + ||)
14. `if (vrijemeOdKreiranja.TotalMinutes < 30 && urgentnostBodovi >= 50)` → +2 (if + &&)
15. `if (urgentnostBodovi >= 100)` → +1
16. `else if (urgentnostBodovi >= 80)` → +1
17. `else if (urgentnostBodovi >= 60)` → +1
18. `else if (urgentnostBodovi >= 40)` → +1
19. `else if (urgentnostBodovi >= 20)` → +1

**Total: 27 decision points + 1 = 28**
**Status: ✅ ZADOVOLJAVA (>= 8)**

---

## ZAKLJUČAK

**SVE 4 FUNKCIONALNOSTI IMAJU METODE SA McCabe Complexity >= 8:**

| Klasa | Metoda | McCabe | Status |
|-------|--------|---------|--------|
| Proizvod | IzracunajPrioritetNabavke() | **15** | ✅ |
| Dobavljac | AnalizirajKvalitetKontakta() | **25** | ✅ |
| Narudzba | ProcijeniVrijemeIsporuke() | **18** | ✅ |
| Obavjestenje | AnalizirajUrgentnost() | **28** | ✅ |

**✅ USLOV ZADOVOLJEN: Sve metode imaju McCabe Complexity >= 8**
