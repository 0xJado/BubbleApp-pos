
{#apdx-testdokumentation}
## Testdokumentation

### Testdurchführung

Die Testdurchführung gliederte sich in manuelle Tests zur Prüfung der Benutzeroberfläche und systemrelevanter Abläufe sowie in automatisierte Tests zur Validierung der Geschäftslogik und Services.

- **Manuelle Integrationstests:** Über Rider-Simulatoren wurde das visuelle Layout und das Nutzerverhalten in verschiedenen Nutzungsszenarien validiert.
- **Unit Tests mit NUnit:** Einzelne Methoden wie Preisberechnung, Datenbankzugriffe und API-Aufrufe wurden getestet.
- **Moq Tests:** Services wie `EmailService`, `OrderStorageService` oder `CoinGeckoService` wurden vollständig gemockt.

### Testergebnisse

Die wichtigsten Ergebnisse aus den Tests:

- **Preisberechnung (TC-01 bis TC-04):** Die Berechnung war fehlerhaft bei mehrfachen gleichen Items (Sum Stacking). Nach Anpassung wurde dies in den Tests erneut geprüft – Erfolg.
- **QR Code Bug (TC-05, TC-11):** Initial wurde das Dezimaltrennzeichen falsch gesetzt (Komma statt Punkt), was zu unlesbaren QR Codes führte. Bugfix implementiert und als Test abgedeckt.
- **Transaktionsverfolgung (TC-10):** Die Memo-Werte lagen tiefer als erwartet (nicht direkt in der Instruction). Tracking wurde angepasst.
- **Email-Versand (TC-09, TC-13):** Wrapper erfolgreich eingeführt, Simulation via Moq verifiziert die SMTP-Kommunikation ohne realen Mailversand.
- **API-Zugriff CoinGecko (TC-06, TC-12):** Fehlerhafte StatusCodes wurden korrekt mit Fallback behandelt.
- **SQLite Datenbank (TC-07, TC-08):** Persistenztests mit In-Memory-DB erfolgreich, Sortierung chronologisch validiert.

### Erkenntnisse & Fehlerbehebung

- **Framework-Konflikte:** .NET Standard 2.1 war inkompatibel mit .NET 9 – Projekt musste auf .NET 8 zurückgesetzt werden. Doppelte `AssemblyInfo`-Einträge führten zu Build-Fehlern und mussten manuell aus der `.csproj` entfernt werden.
- **WaffleService Preislogik:** `TotalPrice` wurde fälschlich in Summe eingerechnet (`Quantity * (Quantity * Preis)`). Test hat dies früh erkannt.
- **Transaktions-API (Solnet):** `Memo` war tiefer im JSON verschachtelt als erwartet – Parsing musste angepasst werden.
- **Manuelle UI Tests:** Layoutprobleme bei Buttons wurden sichtbar (weißer Text auf weißem Button), nach UI-Redesign behoben.
- **Datenbank Cleanup:** Initiale Tests lieferten mehr Datensätze als erwartet – `DeleteAllAsync<T>()` in Setup-Methode eingeführt.
- **Entwickler-Favorit – Unvollständige QR-Code-Daten durch deutsche Kommas:** Beim ersten Testlauf sah alles perfekt aus – der QR Code war sichtbar, die Wallet-App öffnete sich, aber... keine Daten wurden übergeben. Auch ein Wechsel der Wallet-App half nicht. Die Ursache? Ein unscheinbares Komma! Die Solana URI-Spezifikation verwendet, wie im US-Standard, den Punkt als Dezimaltrenner. Mein Code nutzte jedoch das deutsche Format mit Komma. **Lösung:** `CultureInfo` auf `InvariantCulture` gesetzt – Die Zahlung funktionierte wie vorgesehen.
