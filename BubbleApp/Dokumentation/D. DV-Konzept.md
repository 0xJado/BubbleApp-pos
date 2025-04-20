# DV-Konzept – BubbleApp (WaffleMoments POS App - MVP Version)

## 1. Systemarchitektur

Die BubbleApp ist eine plattformübergreifende Point-of-Sale (POS) Anwendung, entwickelt mit .NET MAUI. Die Anwendung wird als MVP entwickelt und besteht aus folgenden Kernkomponenten:

- **Frontend:** .NET MAUI App (UI in XAML, Logik in ViewModels über MVVM Pattern)
- **Services:** Verantwortlich für Preisberechnung, QR-Code-Erzeugung, Blockchain-Verarbeitung, Datenhaltung und Emailversand
- **Backend-Komponente:** SQLite-Datenbank lokal eingebettet zur Bestellpersistierung
- **Externe Schnittstellen:** Solana Blockchain (via Solnet RPC), CoinGecko API, SMTP-Mailserver via MailKit

Die App läuft lokal auf dem Tablet und ist offlinefähig (mit Ausnahme von Blockchain-Abfragen(mit Fallback Wert) und Mailversand).

## 2. Datenflüsse & Funktionsweise

### 2.1 Bestellprozess

1. Der Nutzer wählt Waffeln über das Frontend aus.
2. Die gewählten Items werden in einem Service (WaffleService) gesammelt.
3. Der Gesamtpreis wird auf Basis der Menge und Waffelpreise berechnet.

### 2.2 Zahlung

1. Nach Auswahl wird automatisch der aktuelle SOL-Kurs via **CoinGecko API** geladen.
2. Daraus wird ein Zahlungsrequest als https Link (solana:) generiert und als **QR-Code** dargestellt.
3. Die Transaktion wird auf der Solana DevNet verfolgt (wird bei Produktion auf MainNet gewechselt).
4. Bei Bestätigung wird ein entsprechender Eintrag lokal in der SQLite-Datenbank gespeichert.

### 2.3 E-Mail Quittung

Optional wird nach erfolgreicher Zahlung eine **Email mit Quittung** generiert und versendet (MailKit + SMTP).

## 3. Technologieübersicht

| Komponente            | Technologie           |
|-----------------------|-----------------------|
| UI                    | .NET MAUI, XAML       |
| Logik + MVVM Binding  | CommunityToolkit.MVVM |
| Datenhaltung          | SQLite (local)        |
| Blockchain Tracking   | Solana RPC via Solnet |
| QR-Code Erstellung    | QRCoder               |
| Emailversand          | MailKit + SMTP        |
| Kursdaten             | CoinGecko API         |
| Unit Tests            | NUnit + Moq           |

## 4. Datenmodell

### Tabelle: `CompletedOrder`

| Feldname     | Typ      | Beschreibung                             |
|--------------|----------|------------------------------------------|
| OrderNumber  | Integer  | Autoincrement / eindeutige OrderNummer   |
| OrderId      | String   | Zufällig generierte ID zur Txn-Zuordnung |
| ItemsSummary | String   | Kurzbeschreibung der Waffelbestellung    |
| TotalEur     | Decimal  | Bruttobetrag                             |
| TotalSol     | Decimal  | Entsprechender SOL-Betrag                |
| Wallet       | String   | Ziel-Wallet Adresse                      |
| TxSignature  | String   | Blockchain-Transaktionshash              |
| Timestamp    | DateTime | Zeitpunkt der Bestellung                 |

## 5. Verarbeitung & Logik

### 5.1 Zahlungserkennung via Blockchain

- Es wird regelmäßig die Signaturliste der Wallet abgefragt (`GetSignaturesForAddressAsync`).
- Danach wird jede Transaktion geprüft:
  - **Status:** finalized
  - **Memo:** enthält gültige `Order-ID`
  - **Differenz (Delta):** Der erhaltene Betrag wird als Differenz (PostBalance - PreBalance) überprüft.

### 5.2 Datenpersistenz

- Bei Bestätigung: Speicherung der Bestellung inkl. Zeitstempel und Transaktionshash
- Zugriff über Service `OrderStorageService`, gekapselt mit SQLiteAsyncConnection

### 5.3 Quittungsgenerierung

- Email wird über `EmailService` mit SMTP-Anbindung verschickt.
- Inhalte: Bestellübersicht, Preisaufschlüsselung, Steueranteil, Transaktionsdaten.

## 6. Sicherheits- & Fehlerbehandlung

- Fehlerhafte Blockchain-Abfragen und API-Calls führen zu Logging, ohne App-Crash.
- Email-Adressen werden **nicht gespeichert** (DSGVO-Konformität).
- Unit Tests prüfen alle Kernservices mit Hilfe von **Moq** (z. B. QR-Code, Mail, Preislogik).
- SQLite-Einträge werden pro Bestellung neu erstellt (kein Überschreiben).
