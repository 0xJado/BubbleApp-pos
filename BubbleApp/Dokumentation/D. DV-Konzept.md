
## DV-Konzept – BubbleApp (WaffleMoments POS App - MVP Version)

<!-- { @FS: Hier würde ich noch einleitend darauf hinweisen, dass die Formatierung/Nummerierung des Folgenden nicht dem Original entspricht, weil es hier in die dieses Dokument übernommen wurde. } -->
<!-- { @FS: Ist das jetzt nur ein Auszug? Wenn ja, auch hier darauf hinweisen! } -->

### Systemarchitektur

Die BubbleApp ist eine plattformübergreifende Point-of-Sale (POS) Anwendung, entwickelt mit .NET MAUI. Die Anwendung wird als MVP entwickelt und besteht aus folgenden Kernkomponenten:

- **Frontend:** .NET MAUI App (UI in XAML, Logik in ViewModels über MVVM Pattern)
- **Services:** Verantwortlich für Preisberechnung, QR-Code-Erzeugung, Blockchain-Verarbeitung, Datenhaltung und Emailversand
- **Backend-Komponente:** Lokal eingebettete SQLite-Datenbank zur Bestellpersistierung
- **Externe Schnittstellen:** Solana Blockchain (via Solnet RPC), CoinGecko API, SMTP-Mailserver via MailKit

Die App ist so konzipiert, dass sie grundlegende Funktionen wie die Erfassung von Bestellungen und die Anzeige der Benutzeroberfläche auch ohne aktive Internetverbindung ausführen kann. Allerdings erfordern bestimmte Funktionen, wie die Verifizierung von Blockchain-Zahlungen und der Versand von E-Mails, eine aktive Internetverbindung. Für den Fall, dass keine Verbindung besteht, implementiert die App Fallback-Mechanismen, wie beispielsweise die Verwendung des zuletzt abgerufenen Wechselkurses von CoinGecko, um die Benutzererfahrung so nahtlos wie möglich zu gestalten.​

<!-- { @FS: Hier nochmal erwähnen, dass der Anwender einen deutlichen Hinweis darauf erhält, wenn keine internetverbindung besteht? } -->

### Datenflüsse & Funktionsweise

#### Bestellprozess

1. Der Nutzer wählt Waffeln über das Frontend aus.
2. Die gewählten Items werden in einem Service (WaffleService) gesammelt.
3. Der Gesamtpreis wird auf Basis der Menge und Waffelpreise berechnet.

#### Zahlung

1. Nach Auswahl wird automatisch der aktuelle SOL-Kurs über die CoinGecko API abgerufen.
2. Auf Basis des berechneten Betrags wird ein Zahlungsrequest im Solana URI-Format generiert und als QR-Code dargestellt.
3. Die Transaktion wird anschließend über das Solana DevNet verfolgt (in der Produktionsumgebung erfolgt der Wechsel auf das MainNet).
<!-- { @FS: Wenn hier schon auf Unterschiede zwischen Test- und Produktiv-Betrieb eingegangen werden muss, dann sollte hauptsächlich das Produktivsystem beschrieben werden und das Testsystem als Randnotiz. } -->
4. Unabhängig vom Ergebnis (erfolgreich, fehlgeschlagen oder ausstehend) wird jeder Zahlungsversuch inklusive des ermittelten Status in der lokalen SQLite-Datenbank gespeichert. Dadurch kann der Betreiber jederzeit den Verlauf aller Transaktionen nachvollziehen – auch bei abgebrochenen oder nicht vollständig durchgeführten Zahlungen.

#### Email Quittung

Optional wird nach erfolgreicher Zahlung eine **Email mit Quittung** generiert und versendet (MailKit + SMTP).

### Technologieübersicht

| Komponente           | Technologie           |
| -------------------- | --------------------- |
| UI                   | .NET MAUI, XAML       |
| Logik + MVVM Binding | CommunityToolkit.MVVM |
| Datenhaltung         | SQLite (local)        |
| Blockchain Tracking  | Solana RPC via Solnet |
| QR-Code Erstellung   | QRCoder               |
| Emailversand         | MailKit + SMTP        |
| Kursdaten            | CoinGecko API         |
| Unit Tests           | NUnit + Moq           |

### Datenmodell

#### Tabelle: `CompletedOrder`

| Feldname     | Typ      | Beschreibung                             |
| ------------ | -------- | ---------------------------------------- |
| OrderNumber  | Integer  | Autoincrement / eindeutige OrderNummer   |
| OrderId      | String   | Zufällig generierte ID zur Txn-Zuordnung |
| ItemsSummary | String   | Kurzbeschreibung der Waffelbestellung    |
| TotalEur     | Decimal  | Bruttobetrag                             |
| TotalSol     | Decimal  | Entsprechender SOL-Betrag                |
| Wallet       | String   | Ziel-Wallet Adresse                      |
| TxSignature  | String   | Blockchain-Transaktionshash              |
| Timestamp    | DateTime | Zeitpunkt der Bestellung                 |

<!-- { @FS: OrderNumber *und* OrderId? } -->
<!-- { @FS: Muss die OrderId nicht besser auch eindeutig sein? } -->
<!-- { @FS: Nur so aus Neugierde: Die Wallet des "Überweisenden" kriegen wir doch auch raus, oder? } -->

### Verarbeitung & Logik

#### Zahlungserkennung via Blockchain

- Es wird regelmäßig die Signaturliste der Wallet abgefragt (`GetSignaturesForAddressAsync`).
- Danach wird jede Transaktion geprüft:
  - **Status:** finalized
  - **Memo:** enthält gültige `Order-ID`
  - **Differenz (Delta):** Der erhaltene Betrag wird als Differenz (PostBalance - PreBalance) überprüft

#### Datenpersistenz

- Jeder Zahlungsversuch wird unabhängig vom Ergebnis in der SQLite-Datenbank gespeichert – inklusive Zeitstempel, Zahlungsstatus (z. B. ausstehend, bestätigt, fehlgeschlagen) sowie ggf. Transaktionshash.
- Der Zugriff erfolgt über den Service OrderStorageService, der mittels SQLiteAsyncConnection gekapselt ist und eine strukturierte sowie performante Datenhaltung gewährleistet.

<!-- { @FS: "...der mittels SQLiteAsyncConnection gekapselt ist...": verstehe ich nicht. } -->
<!-- { @FS: "...und eine strukturierte sowie performante Datenhaltung gewährleistet.": gehört hier nicht hin. } -->

#### Quittungsgenerierung

- Email wird über `EmailService` mit SMTP-Anbindung verschickt.
- Inhalte: Bestellübersicht, Preisaufschlüsselung, Steueranteil, Transaktionsdaten.

### Sicherheits- & Fehlerbehandlung

- Fehlerhafte API- oder Blockchain-Abfragen werden durch konsolenbasiertes Logging erfasst, um Debugging im Entwicklungsmodus zu ermöglichen. Für den Endnutzer erfolgt bei kritischen Fehlern (z. B. API-Ausfall) eine visuelle Rückmeldung über die Benutzeroberfläche – etwa durch Warnhinweise oder fallback-basierte Werte.
- Email-Adressen werden nicht dauerhaft gespeichert, sondern lediglich temporär zur Quittungsübermittlung verwendet. Nach erfolgreichem Versand wird die Adresse verworfen, um die Einhaltung der DSGVO zu gewährleisten.
- Für die Absicherung der Anwendung wurden Unit Tests mit NUnit durchgeführt. Dabei kamen gezielte Mocks mittels Moq zum Einsatz, insbesondere für schwer testbare oder externe Komponenten wie:
	- ICoinGeckoService: Rückgabe eines kontrollierten Wechselkurses
	- IEmailService / ISmtpClientWrapper: Simulation des Emailversands
	- IOrderStorageService: Prüfung von Speicher- und Abrufoperationen ohne echte Datenbank
	- IQrCodeService: Test ohne tatsächliche QR-Code-Generierung
<!-- { @FS: Bin nicht ganz sicher, ob das hier hingehört. Aber wenn, dann nicht in Vergangenheitsform. Und von "Absicherung" würde ich hier auch nicht sprechen. } -->
- Alle Transaktionen werden in der lokalen SQLite-Datenbank als eigenständige Einträge gespeichert. Eine Bestellung erzeugt stets einen neuen Datensatz – es erfolgt kein Überschreiben bestehender Einträge, wodurch auch unvollständige oder abgebrochene Transaktionen nachvollzogen werden können.
