
{#apdx-testkonzept}
## Testkonzept

### Einleitung

Die BubbleApp wurde als plattformübergreifende Krypto-Bezahllösung für den Foodtruck-Bereich entwickelt. Ziel des Testkonzepts war es, sämtliche funktionalen Anforderungen der Anwendung frühzeitig zu validieren, systematische Fehlerquellen zu identifizieren und die technische Stabilität durch Unit Tests und Integrationstests abzusichern.

Dabei wurde besonderer Wert auf die Testbarkeit der Businesslogik gelegt, um kritische Abläufe wie Preisberechnung, QR-Code-Erzeugung, Blockchain-Transaktionsverfolgung und Email-Versand zuverlässig simulieren und prüfen zu können. Die Anwendung wurde daher bewusst auf lose Kopplung und Interface-basierte Servicearchitektur ausgelegt, was den Einsatz von Mocking-Frameworks wie Moq erleichtert hat.

Dieses Testkonzept definiert die Testmethodik, den Ablauf sowie die Zielsetzung der einzelnen Tests und dient als Grundlage für die nachfolgende Testdokumentation.

### Testfälle
| **Testfall-Nr.** | **Beschreibung**                                                               | **Erwartetes Verhalten**                                                                                   |
|:-----------------|:-------------------------------------------------------------------------------|:-----------------------------------------------------------------------------------------------------------|
| TC-01            | Hinzufügen eines Standardprodukts zur Bestellung                                | Produkt wird mit korrektem Preis und Menge zur Liste hinzugefügt                                           |
| TC-02            | Entfernen eines Produkts                                                        | Produkt wird bei Menge >1 dekrementiert, sonst entfernt                                                    |
| TC-03            | Benutzerdefinierte Bestellung ("Create Your Moment") mit freiem Preis           | Benutzerdefiniertes Produkt wird korrekt erfasst und berechnet                                             |
| TC-04            | Berechnung Gesamtpreis über alle Produkte                                       | Summe aller Waffeln × Preis wird korrekt berechnet                                                         |
| TC-05            | Generierung eines Zahlungs-QR-Codes                                             | QR Code wird korrekt erzeugt, inklusive korrekter Dezimaltrennung                                          |
| TC-06            | Abruf aktueller Wechselkurs SOL/EUR von CoinGecko                               | Kurs wird geladen; Fallback wird verwendet bei Fehlschlag                                                  |
| TC-07            | Datenbankoperation – Speichern einer Bestellung                                 | Bestellung wird in SQLite persistiert                                                                      |
| TC-08            | Datenbankoperation – Abrufen der Bestellhistorie                                | Daten werden chronologisch korrekt zurückgegeben                                                           |
| TC-09            | Senden eines Bestellbelegs per Email mit SendGrid                               | Mail wird asynchron versendet; SMTP simuliert über Wrapper                                                 |
| TC-10            | Transaktionsverfolgung via Solnet – Gültige Memo- und Betragserkennung          | Transaktion wird erkannt, wenn Betrag & Memo korrekt übermittelt                                           |
| TC-11            | Falsche Formatierung im Solana URI (z. B. Komma statt Punkt)                    | QR Code kann nicht gelesen werden – Fehlerbehandlung greift                                                |
| TC-12            | Fehlerhafte API-Integration CoinGecko                                           | Fallback-Wert wird gesetzt, Anwendung bleibt nutzbar                                                       |
| TC-13            | Fehlerhafte SMTP-Verbindung                                                     | Anwendung gibt keine Exception durch, sondern zeigt freundliche Fehlermeldung                              |
| TC-14            | UI Darstellung – Responsivität bei unterschiedlichen Auflösungen                | Buttons, Textfelder und Layout passen sich korrekt an                                                      |
| TC-15            | Bestellung mit doppeltem Produkt – Validierung der Summenlogik                  | Gesamtbetrag entspricht Menge × Preis, ohne exponentielles Aufsummieren (Bugfix geprüft)                   |
