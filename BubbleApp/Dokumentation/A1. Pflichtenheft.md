# Pflichtenheft  
**Projekt:** BubbleApp – Mobile Krypto-Bezahlsystem
**Version:** 1.0  
**Ersteller:** Anthony Dreibholz-Ortiz  
**Unternehmen:** idesis GmbH  
**Betreuer:** Andreas Mersch & Frank Schmidt  
**Datum:** 21 März 2025 

---

## 1 Einleitung  
**BubbleApp** ist ein mobiles Krypto-Bezahlsystem für Foodtrucks, das auf der Solana-Blockchain basiert. Dieses Pflichtenheft beschreibt die funktionalen und technischen Anforderungen für die Entwicklung eines Minimal Viable Products (MVP), das den gesamten Bestell- und Zahlungsprozess effizient und benutzerfreundlich gestaltet.

Die App ermöglicht direkte Zahlungen in Solana (SOL), ohne zentrale Finanzplattformen oder Zahlungsdienstleister. Sie berechnet den Preis automatisch, generiert einen QR-Code für die Zahlung und überprüft in Echtzeit, ob die Transaktion auf der Blockchain bestätigt wurde. Alle Bestellungen und Zahlungen werden lokal in einer SQLite-Datenbank gespeichert. Zusätzlich können nach erfolgreicher Zahlung digitale Quittungen per E-Mail an Kunden versendet werden.

---

## 2 Zielsetzung des Projekts  

### 2.1 Ausgangssituation  
Mobile Foodtrucks haben oft keine effiziente Möglichkeit, Kryptozahlungen entgegenzunehmen. Bestehende Lösungen setzen meist auf zentralisierte Finanzplattformen, die zusätzliche Gebühren verursachen und Abhängigkeiten schaffen.

### 2.2 Projektziel  
Mit der BubbleApp wird ein unabhängiges Zahlungssystem entwickelt, das Foodtruck-Betreibern ermöglicht, Zahlungen direkt in Solana (SOL) zu akzeptieren, ohne zentralisierte Finanzplattformen oder andere Intermediäre zu benötigen.  

**Kernfunktionen:**  
- **Bestellmanagement**: Der Betreiber kann über eine einfache Benutzeroberfläche Bestellungen aufnehmen und verwalten.  
- **Preisberechnung**: Der Gesamtbetrag wird automatisch berechnet und in Euro und Solana (SOL) angezeigt.  
- **Blockchain-Zahlungen**: Die App generiert einen QR-Code, den Kunden mit ihrer Solana-Wallet scannen können, um die Zahlung durchzuführen.  
- **Transaktionsüberwachung**: Die BubbleApp überprüft in Echtzeit, ob die Zahlung auf der Solana-Blockchain bestätigt wurde.  
- **Datenspeicherung**: Alle Transaktionen werden lokal in einer SQLite-Datenbank gespeichert.  
- **E-Mail-Quittungen**: Nach erfolgreicher Zahlung kann eine digitale Quittung automatisch per E-Mail an den Kunden versendet werden.  

---

## 3 Anwendungsfälle (Use Cases)  

| **Use Case** | **Beschreibung** |
|-------------|-----------------|
| **Bestellung aufnehmen** | Der Betreiber kann eine Waffelbestellung erfassen, bearbeiten und stornieren. |
| **Preisberechnung** | Die App berechnet den Gesamtpreis und zeigt ihn in € und SOL an. |
| **QR-Code generieren** | Ein Zahlungs-QR-Code wird basierend auf den Bestelldaten erzeugt. |
| **Transaktion überprüfen** | Die App prüft auf der Solana-Blockchain, ob die Zahlung eingegangen ist. |
| **Daten speichern** | Alle Bestellungen und Transaktionen werden lokal gespeichert. |
| **Quittung per E-Mail senden** | Kunden erhalten eine Zahlungsbestätigung per E-Mail. |

---

## 4 Funktionale Anforderungen  

### 4.1 Bestellmanagement  
- Speisekarten können aus einer CSV-Datei importiert werden.  
- Der Betreiber kann Waffeln zur Bestellung hinzufügen und entfernen.  
- Der Gesamtpreis wird automatisch in Euro und Solana berechnet.  

### 4.2 Zahlungsprozess  
- Die App generiert einen QR-Code, der die Zahlungsdetails enthält.  
- Der aktuelle Wechselkurs (€ → SOL) wird über die CoinGecko API abgerufen.
- Die Zahlung wird über die Solnet-Bibliothek auf der Solana-Blockchain überprüft. 
- Nach erfolgreicher Zahlung kann eine digitale Quittung per E-Mail an den Kunden gesendet werden.  

---

## 5 Nicht-funktionale Anforderungen  
- **Benutzerfreundlichkeit**: Die App muss einfach und intuitiv zu bedienen sein.
- **Leistungsfähigkeit**: Alle Berechnungen und Transaktionen sollten in Echtzeit durchgeführt werden.
- **Datenschutz**: Keine Speicherung sensibler Kundendaten (DSGVO-konform).
- **Plattformübergreifend**: Die Anwendung läuft unter .NET MAUI (iOS & Android). 

---

## 6 Technische Architektur  

Die BubbleApp basiert auf einer dreischichtigen Architektur:  

**1. Datenzugriffsschicht**  
   - Speicherung von Bestellungen und Transaktionen in einer lokalen SQLite-Datenbank
   - Verwendung von Entity Framework Core für .NET 9 

**2. Geschäftslogikschicht**  
   - Zahlungslogik mit Solnet-Bibliothek  
   - Echtzeit-Preisabfrage über die CoinGecko API
   - Berechnung des Gesamtpreises in € und SOL  

**3. Präsentationsschicht**  
   - Benutzeroberfläche mit .NET MAUI 
   - MVVM-Architektur für saubere Trennung von UI und Logik  

---

## 7 Schnittstellen (APIs, Datenbanken)  

| **Schnittstelle** | **Zweck** |
|------------------|-----------|
| **CoinGecko API** | Abfrage des aktuellen SOL/€-Kurses |
| **Solnet API** | Überprüfung der Blockchain-Transaktion |
| **SQLite Datenbank** | Speicherung von Bestellungen & Transaktionen |

---

## 8 Testszenarien & Qualitätssicherung  

| **Testfall** | **Erwartetes Ergebnis** |
|------------|--------------------|
| Bestellung hinzufügen | Die Waffel erscheint im Warenkorb. |
| QR-Code scannen | Die Solana-Wallet zeigt den richtigen Betrag an. |
| Zahlung bestätigen | Die App erkennt die Transaktion als erfolgreich. |
| Bestellung speichern | Die Daten werden korrekt in SQLite gespeichert. |
| Quittung senden | Der Kunde erhält eine E-Mail mit der Zahlungsbestätigung. |

---

## 9 Abgrenzung (Nicht im MVP enthalten)  
Folgende Features sind nicht Teil des MVPs:  
- Keine Cloud-Datenbank (Nur lokale SQLite-Speicherung)  
- Keine Benutzerverwaltung (Nur für Foodtruck-Betreiber)  

---

## 10 Fazit  
Dieses Pflichtenheft definiert die Anforderungen für die **BubbleApp** als modernes, unabhängiges Krypto-Zahlungssystem für Foodtrucks. Mit **direkter Solana-Zahlung, Echtzeit-Preisberechnung, Blockchain-Transaktionsprüfung und E-Mail-Quittungen** wird eine **effiziente, digitale Bezahlmöglichkeit** geschaffen.  

---

## Lizenz & Kontakt  
© 2025 Anthony Dreibholz-Ortiz / idesis GmbH  
Kontakt: jado@idesis.de
