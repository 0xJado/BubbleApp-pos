
{#apdx-activity-diagramm}
## Aktivitätsdiagramm

Das nachfolgende Aktivitätsdiagramm stellt den vollständigen Ablauf einer Bestellung in der BubbleApp dar.

{{~
    aktivitaetsdiagramm = temp_file.load_and_link "C. Aktivitätsdiagramm.png"
~}}
[aktivitaetsdiagramm]: {{aktivitaetsdiagramm}}  "Aktivitätsdiagramm"
![][aktivitaetsdiagramm]{style="width:100%;"}

Das Diagramm unterteilt die Aktionen in zwei Swimlanes: System (BubbleApp) und Kunde (Endnutzer). Nach dem Start der Anwendung und dem Anzeigen der Speisekarte kann der Betreiber entweder einen vordefinierten Artikel hinzufügen (Standard) oder einen benutzerdefinierten Artikel erstellen (Individuell). Die Bestellung wird daraufhin aktualisiert und geprüft. Nach Bestätigung der Bestellung wird ein QR Code generiert, welcher vom Endnutzer mit einer externen Wallet Anwendung gescannt werden muss (Endnutzer Aktion). Anschließend wird die Transaktion innerhalb der Wallet durchgeführt und somit die Zahlung ausgelöst. Erst nach dieser externen Aktion beginnt die App mit der Überwachung der Solana Blockchain und erkennt eine bestätigte Zahlung. Im Anschluss werden die Daten lokal in einer SQLite Datenbank gespeichert und optional eine digitale Quittung per Email an den Kunden versendet. Diese Darstellung betont sowohl die klare Trennung zwischen System- und Nutzerprozessen als auch die asynchrone Interaktion über externe Zahlungssysteme (z. B. Wallets).
